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


        // Action to run the Node.js script for generating a report - remember to have Node.js installed on the server - initially this will run the script in the NodeScripts folder

        public IActionResult RunNodeReport()
        {
            try
            {
                // Create the temporary reports directory if it doesn't exist
                // CHANGE: Same as original
                string tempDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "reports");
                if (!Directory.Exists(tempDir))
                {
                    Directory.CreateDirectory(tempDir);
                }

                // Define the path to the Node.js script
                // CHANGE: Same as original
                string scriptPath = Path.Combine(Directory.GetCurrentDirectory(), "NodeScripts", "generate_report.js");
                if (!System.IO.File.Exists(scriptPath))
                {
                    return Content("Node script not found at " + scriptPath);
                }

                // Serialize tasks data to JSON file
                // CHANGE: Same as original
                string jsonPath = Path.Combine(tempDir, "tasks.json");
                var tasksData = TasksController.GetAllTasks().Select(t => new
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

                // Setup process start info for Node.js script
                // CHANGE: Same as original, but output/error redirection will now be sent to Console for Azure Logging
                var psi = new ProcessStartInfo
                {
                    FileName = "node",
                    Arguments = $"\"{scriptPath}\" \"{tempDir}\"", // Pass the path to tasks.json
                    RedirectStandardOutput = true, // CHANGE: Capture stdout
                    RedirectStandardError = true,  // CHANGE: Capture stderr
                    UseShellExecute = false,       // CHANGE: Required to redirect output
                    CreateNoWindow = true
                };

                using var process = Process.Start(psi);

                // Redirect Node.js output to Console for Application Logging in Azure
                // CHANGE: New code block to ensure console.log is captured by Azure
                process.OutputDataReceived += (sender, e) => { if (e.Data != null) Console.WriteLine(e.Data); };
                process.ErrorDataReceived += (sender, e) => { if (e.Data != null) Console.WriteLine(e.Data); };
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                // Wait for Node.js process to finish
                // CHANGE: Same as original
                process.WaitForExit();

                // Store information for display in the view
                // CHANGE: Instead of reading output, instruct user to check Azure logs
                ViewBag.NodeOutput = $"Script Path: {scriptPath}\nTempDir: {tempDir}\nJSON Path: {jsonPath}";
                ViewBag.NodeErrors = "Check Application Logging (Log Stream) for Node logs."; // CHANGE: Inform user

                // Read generated report if exists
                // CHANGE: Same as original
                string reportPath = Path.Combine(tempDir, "tasks_report.html");
                ViewBag.ReportHtml = System.IO.File.Exists(reportPath)
                    ? System.IO.File.ReadAllText(reportPath)
                    : "<p>No report was generated.</p>";

                return View("NodeOutput");

            }
            catch (Exception ex)
            {
                // Capture any exceptions
                // CHANGE: Same as original
                ViewBag.NodeErrors = ex.Message;
                return View("NodeOutput");
            }
        }



        // Download the Node report
        public IActionResult DownloadNodeReport()
        {
            string filePath = Path.Combine(TempDir, "tasks_report.html");
            if (!System.IO.File.Exists(filePath))
                return Content("Report file not found.");

            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, "text/html", "tasks_report.html");
        }






        //script to show a reminder using PowerShell, this is only for show the use; using  ExecutionPolicy Bypass -- in production chamge this to RemoteSigned or AllSigned

        public IActionResult RunPowerShellReminder()
        {
            try
            {
                if (!Directory.Exists(TempDir)) Directory.CreateDirectory(TempDir);

                // Instead of generating tasks.json, generate the summary text file that PowerShell expects
                string summaryPath = Path.Combine(TempDir, "task_summary.txt");

                // Prepare summary content
                var completed = TasksController.tasks.Where(t => t.IsCompleted).ToList();
                var pending = TasksController.tasks.Where(t => !t.IsCompleted).ToList();
                var nearDue = pending
                    .Where(t => (t.DueDate - DateTime.Today).TotalDays <= 3)
                    .Select(t => t.Title)
                    .ToList();

                var lines = new List<string>
        {
            "Task Summary Report",
            "-------------------",
            $"Total Tasks: {TasksController.tasks.Count}",
            $"Completed: {completed.Count}",
            $"Pending: {pending.Count}",
            $"Tasks Near Due: {string.Join(", ", nearDue)}"
        };

                // Overwrite the file each time
                System.IO.File.WriteAllLines(summaryPath, lines);

                // Execute the PowerShell script
                string scriptPath = Path.Combine(Directory.GetCurrentDirectory(), "Scripts", "show_reminder.ps1");

                var psi = new ProcessStartInfo
                {
                    FileName = "powershell",
                    Arguments = $"-ExecutionPolicy Bypass -File \"{scriptPath}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using var process = Process.Start(psi);
                process.WaitForExit();

                // Show the confirmation page
                return View("ReminderConfirmation");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View("ReminderError");
            }
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


