using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using TaskManagementPortal.Models;
using TaskManagementPortal.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.IO;




namespace TaskManagementPortal.Controllers
{
    public class TasksController : Controller
    {
        // Static task list to keep data in memory
        public static List<TodoTask> tasks = new List<TodoTask>();
        private string TempDir => Path.Combine(Path.GetTempPath(), "TaskManagementPortal"); // Temporary folder for all generated files

        // Expose tasks for HomeController
        public static List<TodoTask> GetAllTasks()
        {
            return tasks;
        }

        // Seed initial data once when the controller type is first used, to show data  on first load
        static TasksController()
        {
            if (!tasks.Any())
            {
                SeedInitialTasks();
            }
        }

        // GET: /Tasks
        public IActionResult Index()
        {
            return View(tasks);
        }

        // GET: /Tasks/Details/5
        public IActionResult Details(int id)
        {
            var task = tasks.FirstOrDefault(t => t.Id == id);
            if (task == null) return NotFound();
            return View(task);
        }

        // GET: /Tasks/Create
        public IActionResult Create()
        {
            ViewBag.Owners = TaskOwnerService.GetOwners();
            return View();
        }

        // POST: /Tasks/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(TodoTask task, int[] selectedOwners)
        {
            if (selectedOwners != null && selectedOwners.Length > 0)
            {
                task.AssignedOwners = TaskOwnerService.GetOwners()
                    .Where(o => selectedOwners.Contains(TaskOwnerService.GetOwners().IndexOf(o)))
                    .Select(o => o.Name)
                    .ToList();
            }

            if (ModelState.IsValid)
            {
                task.Id = tasks.Count > 0 ? tasks.Max(t => t.Id) + 1 : 1;
                tasks.Add(task);
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Owners = TaskOwnerService.GetOwners();
            return View(task);
        }

        // GET: /Tasks/Edit/5
        public IActionResult Edit(int id)
        {
            var task = tasks.FirstOrDefault(t => t.Id == id);
            if (task == null) return NotFound();
            ViewBag.Owners = TaskOwnerService.GetOwners();
            return View(task);
        }

        // POST: /Tasks/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, TodoTask updatedTask, int[] selectedOwners)
        {
            var task = tasks.FirstOrDefault(t => t.Id == id);
            if (task == null) return NotFound();

            if (selectedOwners != null && selectedOwners.Length > 0)
            {
                updatedTask.AssignedOwners = TaskOwnerService.GetOwners()
                    .Where(o => selectedOwners.Contains(TaskOwnerService.GetOwners().IndexOf(o)))
                    .Select(o => o.Name)
                    .ToList();
            }

            if (ModelState.IsValid)
            {
                task.Title = updatedTask.Title;
                task.Description = updatedTask.Description;
                task.IsCompleted = updatedTask.IsCompleted;
                task.AssignedOwners = updatedTask.AssignedOwners;
                task.AssignedDate = updatedTask.AssignedDate;
                task.DueDate = updatedTask.DueDate;

                return RedirectToAction(nameof(Index));
            }

            ViewBag.Owners = TaskOwnerService.GetOwners();
            return View(updatedTask);
        }

        // GET: /Tasks/Delete/5
        public IActionResult Delete(int id)
        {
            var task = tasks.FirstOrDefault(t => t.Id == id);
            if (task == null) return NotFound();
            return View(task);
        }

        // POST: /Tasks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var task = tasks.FirstOrDefault(t => t.Id == id);
            if (task != null) tasks.Remove(task);
            return RedirectToAction(nameof(Index));
        }

        // Extra: Generate demo tasks
        [HttpGet]
        public IActionResult GenerateDemoTasks()
        {
            tasks.Add(new TodoTask
            {
                Id = tasks.Count + 1,
                Title = "Prepare project documentation",
                Description = "Write and organize the technical documentation for the current sprint.",
                AssignedDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(5),
                IsCompleted = false,
                AssignedOwners = new List<string> { "Alice" }
            });

            tasks.Add(new TodoTask
            {
                Id = tasks.Count + 2,
                Title = "Implement authentication module",
                Description = "Develop and test the user authentication and authorization functionality.",
                AssignedDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(10),
                IsCompleted = false,
                AssignedOwners = new List<string> { "Bob", "Charlie" }

            });

            tasks.Add(new TodoTask
            {
                Id = tasks.Count + 3,
                Title = "Design dashboard layout",
                Description = "Create the initial dashboard wireframe and integrate sample Bootstrap charts.",
                AssignedDate = DateTime.Now.AddDays(-2),
                DueDate = DateTime.Now.AddDays(3),
                IsCompleted = true,
                AssignedOwners = new List<string> { "Alice" }

            });

            return Content("3 demo tasks added!");
        }

        private static void SeedInitialTasks()
        {
            var owners = TaskOwnerService.GetOwners();
            if (owners == null || owners.Count == 0) return;

            // Helper to safely get an owner name by index
            string N(int i) => owners[Math.Min(i, owners.Count - 1)].Name;
            var today = DateTime.Today;

            // 3 pending
            tasks.Add(new TodoTask
            {
                Id = tasks.Count + 1,
                Title = "Prepare monthly KPI report",
                Description = "Compile KPIs and share with ops.",
                AssignedOwners = new List<string> { N(0), N(1) },
                IsCompleted = false,
                AssignedDate = today.AddDays(-2),
                DueDate = today.AddDays(30)
            });
            tasks.Add(new TodoTask
            {
                Id = tasks.Count + 1,
                Title = "Review sprint backlog",
                Description = "Groom items and set priorities.",
                AssignedOwners = new List<string> { N(2) },
                IsCompleted = false,
                AssignedDate = today.AddDays(-1),
                DueDate = today.AddDays(10)
            });
            tasks.Add(new TodoTask
            {
                Id = tasks.Count + 1,
                Title = "QA smoke tests",
                Description = "Run smoke tests on latest build.",
                AssignedOwners = new List<string> { N(3), N(4) },
                IsCompleted = false,
                AssignedDate = today,
                DueDate = today.AddDays(3)
            });

            // 3 completed
            tasks.Add(new TodoTask
            {
                Id = tasks.Count + 1,
                Title = "Database maintenance",
                Description = "Indexes & stats updated.",
                AssignedOwners = new List<string> { N(5) },
                IsCompleted = true,
                AssignedDate = today.AddDays(-20),
                DueDate = today.AddDays(-15)
            });
            tasks.Add(new TodoTask
            {
                Id = tasks.Count + 1,
                Title = "Access review",
                Description = "Quarterly access review finished.",
                AssignedOwners = new List<string> { N(6), N(7) },
                IsCompleted = true,
                AssignedDate = today.AddDays(-8),
                DueDate = today.AddDays(-1)
            });
            tasks.Add(new TodoTask
            {
                Id = tasks.Count + 1,
                Title = "Release notes",
                Description = "Drafted and published.",
                AssignedOwners = new List<string> { N(8) },
                IsCompleted = true,
                AssignedDate = today.AddDays(-5),
                DueDate = today.AddDays(20)
            });
        }

        // GET: /Tasks/ExportTasksToJson - button funcionality  to export tasks to JSON
        public IActionResult ExportTasksToJson()
        {

            // Serialize the tasks list to json
            var jsonString = JsonSerializer.Serialize(tasks, new JsonSerializerOptions
            {
                WriteIndented = true // fromat the json for readability
            });

            // save to server (tmp folder) 

            if (!Directory.Exists(TempDir)) Directory.CreateDirectory(TempDir);

    
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Scripts", "tasks.json");
            System.IO.File.WriteAllText(filePath, jsonString);

            // Return the JSON as a downloadable file ex to use in other process out of the application
            var fileBytes = System.Text.Encoding.UTF8.GetBytes(jsonString);
            return File(fileBytes, "application/json", "tasks.json");
        }

        public IActionResult ExportTasksToTxt()

        {

            // Check if there are any tasks
            if (tasks == null || tasks.Count == 0)
            {
                return Content("There are no tasks to export.");
            }

            // Serialize the tasks to JSON with indentation for readability
            var json = JsonSerializer.Serialize(tasks, new JsonSerializerOptions { WriteIndented = true });

            if (!Directory.Exists(TempDir)) Directory.CreateDirectory(TempDir);
            var path = Path.Combine(TempDir, "tasks.txt"); 
            System.IO.File.WriteAllText(path, json);


            // Read the file into memory
            byte[] fileBytes = System.IO.File.ReadAllBytes(path);

            // Return the file as a download
            return File(fileBytes, "application/json", "tasks.json");
        }



    }
}
