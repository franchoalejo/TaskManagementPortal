using Microsoft.AspNetCore.Mvc;
using TaskManagementPortal.Models;
using TaskManagementPortal.Services;
using System.Collections.Generic;
using System.Linq;

namespace TaskManagementPortal.Controllers
{
    // TasksController handles task-related HTTP requests and responses
    public class TasksController : Controller
    {
        // In-memory storage for demo purposes
        // In a real project, this would connect to a database via EF Core
        private static List<TodoTask> tasks = new List<TodoTask>();

        // GET: /Tasks/
        // Display the list of all tasks
        public IActionResult Index()
        {
            return View(tasks);
        }

        // GET: /Tasks/Details/5
        // Display details for a specific task
        public IActionResult Details(int id)
        {
            var task = tasks.FirstOrDefault(t => t.Id == id);
            if (task == null)
                return NotFound();
            return View(task);
        }

        // GET: /Tasks/Create
        // Display the form to create a new task
        public IActionResult Create()
        {
            ViewBag.Owners = TaskOwnerService.GetOwners(); // Load task owners for selection
            return View();
        }

        // POST: /Tasks/Create
        // Handle form submission to add a new task
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(TodoTask task, List<string> selectedOwners)
        {
            // Assign a new Id based on current list count
            task.Id = tasks.Count + 1;

            // Assign selected owners
            task.AssignedOwners = selectedOwners ?? new List<string>();

            // Ensure IsCompleted field is set (default false if not provided)
            task.IsCompleted = task.IsCompleted;

            tasks.Add(task);

            return RedirectToAction(nameof(Index));
        }

        // GET: /Tasks/Edit/5
        // Display form to edit a task
        public IActionResult Edit(int id)
        {
            var task = tasks.FirstOrDefault(t => t.Id == id);
            if (task == null)
                return NotFound();

            ViewBag.Owners = TaskOwnerService.GetOwners(); // Load task owners for selection
            return View(task);
        }

        // POST: /Tasks/Edit/5
        // Handle form submission to update a task
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, TodoTask updatedTask, List<string> selectedOwners)
        {
            var task = tasks.FirstOrDefault(t => t.Id == id);
            if (task == null)
                return NotFound();

            // Update fields
            task.Title = updatedTask.Title;
            task.Description = updatedTask.Description;
            task.IsCompleted = updatedTask.IsCompleted; // Preserve completion status
            task.AssignedOwners = selectedOwners ?? new List<string>();

            return RedirectToAction(nameof(Index));
        }

        // GET: /Tasks/Delete/5
        // Display confirmation to delete a task
        public IActionResult Delete(int id)
        {
            var task = tasks.FirstOrDefault(t => t.Id == id);
            if (task == null)
                return NotFound();
            return View(task);
        }

        // POST: /Tasks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var task = tasks.FirstOrDefault(t => t.Id == id);
            if (task != null)
            {
                tasks.Remove(task);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
