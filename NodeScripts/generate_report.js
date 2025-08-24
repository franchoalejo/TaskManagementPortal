const fs = require('fs');
const path = require('path');

// Temp directory is passed as argument
const tempDir = process.argv[2];
const jsonFile = path.join(tempDir, 'tasks.json');
const htmlFile = path.join(tempDir, 'tasks_report.html');

console.log("Node script started...");
console.log("Node version:", process.version);
console.log("Temp directory received:", tempDir);
console.log("Looking for JSON file at:", jsonFile);

// Load tasks from JSON
let tasks = [];
if (fs.existsSync(jsonFile)) {
    const raw = fs.readFileSync(jsonFile, 'utf8');
    tasks = JSON.parse(raw);
    console.log(`Tasks loaded: ${tasks.length}`);
} else {
    console.error(`ERROR: tasks.json file not found at ${jsonFile}`);
    process.exit(1);
}

// Helper to format date as DD/MM/YYYY
function formatDate(dateString) {
    if (!dateString) return '';
    const date = new Date(dateString);
    if (isNaN(date)) return dateString; // fallback if parsing fails
    const day = String(date.getDate()).padStart(2, '0');
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const year = date.getFullYear();
    return `${day}/${month}/${year}`;
}

// Generate styled HTML report
let html = `
<html>
<head>
    <meta charset="UTF-8">
    <title>Tasks Report</title>
    <style>
        body { font-family: "Segoe UI", Arial, sans-serif; margin: 40px; color: #333; }
        h1 { text-align: center; color: #444; }
        .report-container { max-width: 1000px; margin: 0 auto; }
        table { width: 100%; border-collapse: collapse; margin-top: 20px; box-shadow: 0 2px 8px rgba(0,0,0,0.05); }
        th, td { border: 1px solid #ddd; padding: 10px; text-align: left; font-size: 14px; }
        th { background-color: #4CAF50; color: white; text-transform: uppercase; font-size: 13px; }
        tr:nth-child(even) { background-color: #f9f9f9; }
        tr:hover { background-color: #f1f1f1; }
        .completed { color: green; font-weight: bold; }
        .pending { color: red; font-weight: bold; }
    </style>
</head>
<body>
    <div class="report-container">
        <h1>Tasks Report</h1>
        <table>
            <thead>
                <tr>
                    <th>ID</th>
                    <th>Title</th>
                    <th>Description</th>
                    <th>Assigned Date</th>
                    <th>Due Date</th>
                    <th>Status</th>
                    <th>Owners</th>
                </tr>
            </thead>
            <tbody>
`;

tasks.forEach(task => {
    html += `
        <tr>
            <td>${task.Id}</td>
            <td>${task.Title}</td>
            <td>${task.Description || ''}</td>
            <td>${formatDate(task.AssignedDate)}</td>
            <td>${formatDate(task.DueDate)}</td>
            <td class="${task.IsCompleted ? 'completed' : 'pending'}">${task.IsCompleted ? 'Completed' : 'Pending'}</td>
            <td>${Array.isArray(task.AssignedOwners) ? task.AssignedOwners.join(', ') : (task.AssignedOwners || '')}</td>
        </tr>
    `;
});

html += `
            </tbody>
        </table>
    </div>
</body>
</html>
`;

fs.writeFileSync(htmlFile, html, 'utf8');
console.log(`Report generated at: ${htmlFile}`);

