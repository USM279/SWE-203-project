using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TaskManagerApp.Data;
using TaskManagerApp.Models;
using System.Security.Claims;

namespace TaskManagerApp.Controllers
{
    // Controller for managing tasks
    // Admins can manage all tasks, employees can only view/edit their own tasks
    [Authorize]
    public class TaskItemsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<TaskItemsController> _logger;

        public TaskItemsController(ApplicationDbContext context, ILogger<TaskItemsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: TaskItems
        public async Task<IActionResult> Index()
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var isAdmin = User.IsInRole("Admin");

                IQueryable<TaskItem> tasksQuery = _context.TaskItems.Include(t => t.AssignedTo);

                // Employees only see their own tasks
                if (!isAdmin && currentUserId.HasValue)
                {
                    tasksQuery = tasksQuery.Where(t => t.AssignedToId == currentUserId.Value);
                }

                var tasks = await tasksQuery
                    .OrderByDescending(t => t.Priority)
                    .ThenBy(t => t.DueDate)
                    .ToListAsync();

                ViewBag.IsAdmin = isAdmin;
                return View(tasks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading tasks");
                TempData["ErrorMessage"] = "Error loading tasks. Please try again.";
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: TaskItems/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var taskItem = await _context.TaskItems
                    .Include(t => t.AssignedTo)
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (taskItem == null)
                {
                    return NotFound();
                }

                // Check permissions
                if (!CanAccessTask(taskItem))
                {
                    return RedirectToAction("AccessDenied", "Account");
                }

                return View(taskItem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error loading task details for ID: {id}");
                TempData["ErrorMessage"] = "Error loading task details. Please try again.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: TaskItems/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            PopulateEmployeesDropdown();
            return View();
        }

        // POST: TaskItems/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Title,Description,Status,Priority,DueDate,AssignedToId,Notes")] TaskItem taskItem)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    taskItem.CreatedDate = DateTime.Now;
                    taskItem.LastUpdated = DateTime.Now;

                    _context.Add(taskItem);
                    await _context.SaveChangesAsync();

                    var assignedEmployee = await _context.Employees.FindAsync(taskItem.AssignedToId);
                    _logger.LogInformation($"Task '{taskItem.Title}' created and assigned to {assignedEmployee?.FullName} by {User.Identity?.Name}");
                    
                    TempData["SuccessMessage"] = $"Task '{taskItem.Title}' created successfully and assigned to {assignedEmployee?.FullName}!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating task");
                    ModelState.AddModelError(string.Empty, "Error creating task. Please try again.");
                }
            }
            
            PopulateEmployeesDropdown(taskItem.AssignedToId);
            return View(taskItem);
        }

        // GET: TaskItems/Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var taskItem = await _context.TaskItems.FindAsync(id);
                
                if (taskItem == null)
                {
                    return NotFound();
                }

                // Check permissions
                if (!CanAccessTask(taskItem))
                {
                    return RedirectToAction("AccessDenied", "Account");
                }

                var isAdmin = User.IsInRole("Admin");
                ViewBag.IsAdmin = isAdmin;

                // Only admins can reassign tasks
                if (isAdmin)
                {
                    PopulateEmployeesDropdown(taskItem.AssignedToId);
                }

                return View(taskItem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error loading task for edit, ID: {id}");
                TempData["ErrorMessage"] = "Error loading task. Please try again.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: TaskItems/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Status,Priority,DueDate,AssignedToId,Notes,CreatedDate,CompletedDate")] TaskItem taskItem)
        {
            if (id != taskItem.Id)
            {
                return NotFound();
            }

            var isAdmin = User.IsInRole("Admin");

            if (ModelState.IsValid)
            {
                try
                {
                    var existingTask = await _context.TaskItems.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id);
                    
                    if (existingTask == null)
                    {
                        return NotFound();
                    }

                    // Check permissions
                    if (!CanAccessTask(existingTask))
                    {
                        return RedirectToAction("AccessDenied", "Account");
                    }

                    // Employees can only update status and notes
                    if (!isAdmin)
                    {
                        taskItem.Title = existingTask.Title;
                        taskItem.Description = existingTask.Description;
                        taskItem.Priority = existingTask.Priority;
                        taskItem.DueDate = existingTask.DueDate;
                        taskItem.AssignedToId = existingTask.AssignedToId;
                        taskItem.CreatedDate = existingTask.CreatedDate;
                    }

                    taskItem.LastUpdated = DateTime.Now;

                    // Handle completion date
                    if (taskItem.Status == TaskItemStatus.Completed && existingTask.Status != TaskItemStatus.Completed)
                    {
                        taskItem.CompletedDate = DateTime.Now;
                    }
                    else if (taskItem.Status != TaskItemStatus.Completed)
                    {
                        taskItem.CompletedDate = null;
                    }

                    _context.Update(taskItem);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation($"Task '{taskItem.Title}' updated by {User.Identity?.Name}");
                    TempData["SuccessMessage"] = $"Task '{taskItem.Title}' updated successfully!";
                    
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!TaskItemExists(taskItem.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        _logger.LogError(ex, $"Concurrency error updating task ID: {id}");
                        ModelState.AddModelError(string.Empty, "The task was modified by another user. Please reload and try again.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error updating task ID: {id}");
                    ModelState.AddModelError(string.Empty, "Error updating task. Please try again.");
                }
            }

            ViewBag.IsAdmin = isAdmin;
            if (isAdmin)
            {
                PopulateEmployeesDropdown(taskItem.AssignedToId);
            }
            
            return View(taskItem);
        }

        // GET: TaskItems/Delete 
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var taskItem = await _context.TaskItems
                    .Include(t => t.AssignedTo)
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (taskItem == null)
                {
                    return NotFound();
                }

                return View(taskItem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error loading task for delete, ID: {id}");
                TempData["ErrorMessage"] = "Error loading task. Please try again.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: TaskItems/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var taskItem = await _context.TaskItems.FindAsync(id);
                
                if (taskItem == null)
                {
                    return NotFound();
                }

                _context.TaskItems.Remove(taskItem);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Task '{taskItem.Title}' deleted by {User.Identity?.Name}");
                TempData["SuccessMessage"] = $"Task '{taskItem.Title}' deleted successfully!";
                
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting task ID: {id}");
                TempData["ErrorMessage"] = "Error deleting task. Please try again.";
                return RedirectToAction(nameof(Index));
            }
        }

        // Helper methods
        private bool CanAccessTask(TaskItem task)
        {
            if (User.IsInRole("Admin"))
            {
                return true;
            }

            var currentUserId = GetCurrentUserId();
            return currentUserId.HasValue && task.AssignedToId == currentUserId.Value;
        }

        private int? GetCurrentUserId()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdString, out int userId))
            {
                return userId;
            }
            return null;
        }

        private void PopulateEmployeesDropdown(object? selectedEmployee = null)
        {
            var employees = _context.Employees
                .Where(e => e.IsActive)
                .OrderBy(e => e.FirstName)
                .Select(e => new
                {
                    e.Id,
                    FullName = e.FirstName + " " + e.LastName + " (" + e.Position + ")"
                })
                .ToList();

            ViewBag.AssignedToId = new SelectList(employees, "Id", "FullName", selectedEmployee);
        }

        private bool TaskItemExists(int id)
        {
            return _context.TaskItems.Any(e => e.Id == id);
        }
    }
}
