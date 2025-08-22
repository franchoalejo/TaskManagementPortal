const fs = require('fs');
const path = require('path');

// Path temporal donde guardamos tasks.json
const tempDir = path.join(require('os').tmpdir(), 'TaskManagementPortal');
const jsonPath = path.join(tempDir, 'tasks.json');
const outputPath = path.join(tempDir, 'tasks_report.html');

// Leer el JSON de tareas
let tasks = [];
if (fs.existsSync(jsonPath)) {
    const raw = fs.readFileSync(jsonPath, 'utf-8');
    tasks = JSON.parse(raw);
}

// Generar HTML con DataTables
const html = `
<!DOCTYPE html>
<html>
<head>
    <meta charset="UTF-8">
    <title>Task Report</title>
    <link rel="stylesheet" href="https://cdn.datatables.net/1.13.6/css/jquery.dataTables.min.css">
    <script src="https://code.jquery.com/jquery-3.7.0.min.js"></script>
    <script src="https://cdn.datatables.net/1.13.6/js/jquery.dataTables.min.js"></script>
</head>
<body>
    <h2>Task Report</h2>
    <table id="tasksTable" class="display">
        <thead>
            <tr>
                <th>Id</th>
                <th>Title</th>
                <th>Description</th>
                <th>Assigned Date</th>
                <th>Due Date</th>
                <th>Completed</th>
                <th>Owners</th>
            </tr>
        </thead>
        <tbody>
            ${tasks.map(t => `
                <tr>
                    <td>${t.Id}</td>
                    <td>${t.Title}</td>
                    <td>${t.Description}</td>
                    <td>${new Date(t.AssignedDate).toLocaleDateString()}</td>
                    <td>${new Date(t.DueDate).toLocaleDateString()}</td>
                    <td>${t.IsCompleted}</td>
                    <td>${t.AssignedOwners.join(', ')}</td>
                </tr>`).join('')}
        </tbody>
    </table>

    <script>
        $(document).ready(function() {
            $('#tasksTable').DataTable();
        });
    </script>
</body>
</html>
`;

// Guardar HTML
fs.writeFileSync(outputPath, html, 'utf-8');

console.log('Report generated at:', outputPath);
