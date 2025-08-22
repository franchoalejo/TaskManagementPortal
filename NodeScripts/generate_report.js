// generate_report.js (CommonJS / ES5 compatible)

// Use var instead of let/const
var fs = require('fs');
var path = require('path');

// Temporary folder path passed from C# or use default
var tempDir = process.argv[2] || path.join(process.env.TEMP, 'TaskManagementPortal');

// File paths
var jsonFile = path.join(tempDir, 'tasks.json');
var htmlFile = path.join(tempDir, 'tasks_report.html');

// Read tasks JSON
var tasks = [];
if (fs.existsSync(jsonFile)) {
    try {
        var raw = fs.readFileSync(jsonFile, 'utf8');
        tasks = JSON.parse(raw);
    } catch (e) {
        console.error('Error parsing JSON:', e.message);
    }
}

// Simple HTML template for report
var htmlContent = '<!DOCTYPE html><html><head><meta charset="UTF-8"><title>Tasks Report</title>';
htmlContent += '<link rel="stylesheet" href="https://cdn.datatables.net/1.13.5/css/jquery.dataTables.min.css">';
htmlContent += '<script src="https://code.jquery.com/jquery-3.7.0.min.js"></script>';
htmlContent += '<script src="https://cdn.datatables.net/1.13.5/js/jquery.dataTables.min.js"></script>';
htmlContent += '</head><body>';
htmlContent += '<h1>Tasks Report</h1>';
htmlContent += '<table id="tasksTable" class="display" style="width:100%">';
htmlContent += '<thead><tr><th>ID</th><th>Title</th><th>Description</th><th>Assigned Date</th><th>Due Date</th><th>Status</th></tr></thead><tbody>';

// Populate table rows
for (var i = 0; i < tasks.length; i++) {
    var t = tasks[i];
    htmlContent += '<tr>';
    htmlContent += '<td>' + t.Id + '</td>';
    htmlContent += '<td>' + t.Title + '</td>';
    htmlContent += '<td>' + t.Description + '</td>';
    htmlContent += '<td>' + t.AssignedDate + '</td>';
    htmlContent += '<td>' + t.DueDate + '</td>';
    htmlContent += '<td>' + (t.IsCompleted ? 'Completed' : 'Pending') + '</td>';
    htmlContent += '</tr>';
}

htmlContent += '</tbody></table>';

// Initialize DataTable safely
htmlContent += '<script>$(document).ready(function() { $("#tasksTable").DataTable(); });</script>';

htmlContent += '</body></html>';

// Write HTML report
try {
    fs.writeFileSync(htmlFile, htmlContent, 'utf8');
    console.log('Report generated at:', htmlFile);
} catch (e) {
    console.error('Error writing HTML file:', e.message);
}

