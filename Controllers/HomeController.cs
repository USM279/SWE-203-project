using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagerApp.Data;
using TaskManagerApp.Models;
using System.Security.Claims;
using System.Diagnostics;

namespace TaskManagerApp.Controllers
{
    // Controller for home page and dashboard
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ApplicationDbContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Home/Index
        public async Task<IActionResult> Index()
        {
            try
            {
                if (User.Identity?.IsAuthenticated ?? false)
                {
                    var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

                    if (int.TryParse(userIdString, out int userId))
                    {
                        if (userRole == "Admin")
                        {
                            // Admin sees all statistics
                            ViewBag.TotalEmployees = await _context.Employees.CountAsync(e => e.IsActive);
                            ViewBag.TotalTasks = await _context.TaskItems.CountAsync();
                            ViewBag.CompletedTasks = await _context.TaskItems.CountAsync(t => t.Status == TaskItemStatus.Completed);
                            ViewBag.PendingTasks = await _context.TaskItems.CountAsync(t => t.Status != TaskItemStatus.Completed);
                            ViewBag.OverdueTasks = await _context.TaskItems.CountAsync(t => t.DueDate < DateTime.Now && t.Status != TaskItemStatus.Completed);
                        }
                        else
                        {
                            // Employee sees only their own statistics
                            ViewBag.MyTasks = await _context.TaskItems.CountAsync(t => t.AssignedToId == userId);
                            ViewBag.MyCompletedTasks = await _context.TaskItems.CountAsync(t => t.AssignedToId == userId && t.Status == TaskItemStatus.Completed);
                            ViewBag.MyPendingTasks = await _context.TaskItems.CountAsync(t => t.AssignedToId == userId && t.Status != TaskItemStatus.Completed);
                            ViewBag.MyOverdueTasks = await _context.TaskItems.CountAsync(t => t.AssignedToId == userId && t.DueDate < DateTime.Now && t.Status != TaskItemStatus.Completed);
                        }
                    }
                }

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading home page");
                return View();
            }
        }

        // GET: Home/Privacy
        public IActionResult Privacy()
        {
            return View();
        }

        // GET: Home/Error
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
