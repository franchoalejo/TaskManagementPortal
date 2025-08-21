using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using TaskManagementPortal.Controllers; // Import TasksController
using TaskManagementPortal.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TaskManagementPortal.Controllers
{
    public class HomeController : Controller
    {
        // GET: /
        public IActionResult Index()
        {
            // Get all tasks from the static list in TasksController
            var tasks = TasksController.GetAllTasks();
            ViewBag.Tasks = tasks;

            // Title and subtitle for the homepage 
            ViewBag.TitleMessage = "Professional Task Management Portal";
            ViewBag.Subtitle = "A demonstration of ASP.NET Core, Bootstrap, and modern web integrations.";

            return View();
        }



private string TempDir => Path.Combine(Path.GetTempPath(), "TaskManagementPortal"); // Temporary folder for all generated files


    public IActionResult RunPythonScript()
        {
            try
            {
                if (!Directory.Exists(TempDir)) Directory.CreateDirectory(TempDir);

                // Check if Python is available in PATH to prevent errors when running the script 
                var psiCheck = new ProcessStartInfo
                {
                    FileName = "python",
                    Arguments = "--version",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using var checkProcess = Process.Start(psiCheck);
                string versionOutput = checkProcess.StandardOutput.ReadToEnd();
                string versionError = checkProcess.StandardError.ReadToEnd();
                checkProcess.WaitForExit();

                // If both outputs are empty, Python is likely not installed
                if (string.IsNullOrWhiteSpace(versionOutput) && string.IsNullOrWhiteSpace(versionError))
                {
                    // Redirect to a page showing installation instructions
                    ViewBag.ErrorMessage = "Python is not installed. Please install Python to run this feature. Download it from https://www.python.org/downloads/";
                    return View("PythonNotFound");
                }

                // If Python exists, continue to execute the script
                string scriptPath = Path.Combine(Directory.GetCurrentDirectory(), "Scripts", "task_summary.py");

            
                // Save JSON to user's Downloads folder instead of Scripts
                string jsonPath = Path.Combine(TempDir, "tasks.json");

                // Serialize the tasks to JSON with indentation for readability
                var tasksData = TasksController.tasks.Select(t => new
                {
                    t.Id,
                    t.Title,
                    t.Description,
                    t.AssignedDate,
                    t.DueDate,
                    t.IsCompleted,
                    AssignedOwners = t.AssignedOwners
                }).ToList();

                var json = JsonSerializer.Serialize(tasksData, new JsonSerializerOptions { WriteIndented = true });
                System.IO.File.WriteAllText(jsonPath, json);



                // Check if script exists
                if (!System.IO.File.Exists(scriptPath))
                {
                    return Content("Python script not found at " + scriptPath);
                }

              
                var psi = new ProcessStartInfo
                {
                    FileName = "python",
                    Arguments = $"\"{scriptPath}\" \"{TempDir}\"", // pasar carpeta temporal
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };


                using var process = Process.Start(psi);
                string output = process.StandardOutput.ReadToEnd();
                string errors = process.StandardError.ReadToEnd();
                process.WaitForExit();


                ViewBag.PythonOutput = output;
                ViewBag.PythonErrors = errors;


                // Read generated task_summary.txt 
             

                string summaryPath = Path.Combine(TempDir, "task_summary.txt"); // same tmp folder
                if (System.IO.File.Exists(summaryPath))
                {
                    ViewBag.SummaryContent = System.IO.File.ReadAllText(summaryPath);
                }
                else
                {
                    ViewBag.SummaryContent = "No summary file was generated.";
                }


                return View("PythonOutput");
            }
            catch (Exception ex)
            {
                ViewBag.PythonErrors = ex.Message;
                return View("PythonOutput");
            }
        }


        public IActionResult DownloadSummary() // download generated summary file to use 
        {
            string filePath = Path.Combine(TempDir, "task_summary.txt");

            if (!System.IO.File.Exists(filePath))
            {
                return Content("Summary file not found.");
            }

            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            var fileName = "task_summary.txt";

            // Force browser to download file
            return File(fileBytes, "text/plain", fileName);
        }




    }
}


