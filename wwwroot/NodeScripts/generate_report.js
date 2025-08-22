const fs = require('fs');
const path = require('path');

const tempDir = process.argv[2] || path.join(process.env.TEMP, 'TaskManagementPortal');
const jsonFile = path.join(tempDir, 'tasks.json');
const htmlFile = path.join(tempDir, 'tasks_report.html');

let tasks = [];
if (fs.existsSync(jsonFile)) {
    const raw = fs.readFileSync(jsonFile, 'utf8');
    tasks = JSON.parse(raw);
}

// Construcción del HTML
let htmlContent = '<!DOCTYPE html><html><head><meta charset="UTF-8"><title>Tasks Report</title>';
htmlContent += '<link rel="stylesheet" href="https://cdn.datatables.net/1.13.5/css/jquery.dataTables.min.css">';
htmlContent += '<script src="https://code.jquery.com/jquery-3.7.0.min.js"></script>';
htmlContent += '<script src="https://cdn.datatables.net/1.13.5/js/jquery.dataTables.min.js"></script>';
htmlContent += '</head><body>';
htmlContent += '<h1>Tasks Report</h1>';
htmlContent += '<table id="tasksTable" class="display" style="width:100%">';
htmlContent += '<thead><tr><th>ID</th><th>Title</th><th>Description</th><th>Assigned Date</th><th>Due Date</th><th>Status</th></tr></thead><tbody>';

tasks.forEach(t => {
    htmlContent += `<tr>
        <td>${t.Id}</td>
        <td>${t.Title}</td>
        <td>${t.Description}</td>
        <td>${t.AssignedDate}</td>
        <td>${t.DueDate}</td>
        <td>${t.IsCompleted ? 'Completed' : 'Pending'}</td>
    </tr>`;
});

htmlContent += '</tbody></table>';
htmlContent += '<script>$(document).ready(function() { $("#tasksTable").DataTable(); });</script>';
htmlContent += '</body></html>';

fs.writeFileSync(htmlFile, htmlContent, 'utf8');
console.log('Report generated at:', htmlFile);
