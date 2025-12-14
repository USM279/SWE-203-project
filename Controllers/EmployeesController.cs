using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagerApp.Data;
using TaskManagerApp.Models;

namespace TaskManagerApp.Controllers
{
    // Controller for managing employees
    // Only admins can manage employees, employees can view their own profile
    [Authorize]
    public class EmployeesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<EmployeesController> _logger;

        public EmployeesController(ApplicationDbContext context, ILogger<EmployeesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Employees
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            try
            {
                var employees = await _context.Employees
                    .Include(e => e.AssignedTasks)
                    .OrderByDescending(e => e.IsActive)
                    .ThenBy(e => e.FirstName)
                    .ToListAsync();

                return View(employees);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading employees list");
                TempData["ErrorMessage"] = "Error loading employees. Please try again.";
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Employees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var employee = await _context.Employees
                    .Include(e => e.AssignedTasks)
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (employee == null)
                {
                    return NotFound();
                }

                // Employees can only view their own details
                if (!User.IsInRole("Admin"))
                {
                    var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                    if (currentUserId != id.ToString())
                    {
                        return RedirectToAction("AccessDenied", "Account");
                    }
                }

                return View(employee);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error loading employee details for ID: {id}");
                TempData["ErrorMessage"] = "Error loading employee details. Please try again.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Employees/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Employees/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("FirstName,LastName,Email,Password,Position,PhoneNumber,Role,HireDate,IsActive")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Check if email already exists
                    var existingEmployee = await _context.Employees
                        .FirstOrDefaultAsync(e => e.Email == employee.Email);

                    if (existingEmployee != null)
                    {
                        ModelState.AddModelError("Email", "An employee with this email already exists.");
                        return View(employee);
                    }

                    _context.Add(employee);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation($"New employee created: {employee.Email} by {User.Identity?.Name}");
                    TempData["SuccessMessage"] = $"Employee {employee.FullName} created successfully!";
                    
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating employee");
                    ModelState.AddModelError(string.Empty, "Error creating employee. Please try again.");
                }
            }
            return View(employee);
        }

        // GET: Employees/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var employee = await _context.Employees.FindAsync(id);
                if (employee == null)
                {
                    return NotFound();
                }
                return View(employee);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error loading employee for edit, ID: {id}");
                TempData["ErrorMessage"] = "Error loading employee. Please try again.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Employees/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName,Email,Password,Position,PhoneNumber,Role,HireDate,IsActive")] Employee employee)
        {
            if (id != employee.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Check if email is being changed and if it's already in use
                    var existingEmployee = await _context.Employees
                        .FirstOrDefaultAsync(e => e.Email == employee.Email && e.Id != id);

                    if (existingEmployee != null)
                    {
                        ModelState.AddModelError("Email", "An employee with this email already exists.");
                        return View(employee);
                    }

                    _context.Update(employee);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation($"Employee updated: {employee.Email} by {User.Identity?.Name}");
                    TempData["SuccessMessage"] = $"Employee {employee.FullName} updated successfully!";
                    
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!EmployeeExists(employee.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        _logger.LogError(ex, $"Concurrency error updating employee ID: {id}");
                        ModelState.AddModelError(string.Empty, "The employee was modified by another user. Please reload and try again.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error updating employee ID: {id}");
                    ModelState.AddModelError(string.Empty, "Error updating employee. Please try again.");
                }
            }
            return View(employee);
        }

        // GET: Employees/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var employee = await _context.Employees
                    .Include(e => e.AssignedTasks)
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (employee == null)
                {
                    return NotFound();
                }

                return View(employee);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error loading employee for delete, ID: {id}");
                TempData["ErrorMessage"] = "Error loading employee. Please try again.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var employee = await _context.Employees
                    .Include(e => e.AssignedTasks)
                    .FirstOrDefaultAsync(e => e.Id == id);

                if (employee == null)
                {
                    return NotFound();
                }

                // Can't delete employee if they have assigned tasks
                if (employee.AssignedTasks.Any())
                {
                    TempData["ErrorMessage"] = $"Cannot delete {employee.FullName} because they have assigned tasks. Please reassign or complete their tasks first.";
                    return RedirectToAction(nameof(Index));
                }

                _context.Employees.Remove(employee);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Employee deleted: {employee.Email} by {User.Identity?.Name}");
                TempData["SuccessMessage"] = $"Employee {employee.FullName} deleted successfully!";
                
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting employee ID: {id}");
                TempData["ErrorMessage"] = "Error deleting employee. Please try again.";
                return RedirectToAction(nameof(Index));
            }
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.Id == id);
        }
    }
}
