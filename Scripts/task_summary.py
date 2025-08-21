import json
from datetime import datetime, date
import sys
import os

# Get the folder path from the first argument (passed from .NET)
if len(sys.argv) > 1:
    temp_dir = sys.argv[1]
else:
    temp_dir = "."

# Paths for JSON input and TXT output
json_path = os.path.join(temp_dir, "tasks.json")
summary_path = os.path.join(temp_dir, "task_summary.txt")

# Read tasks from JSON
try:
    with open(json_path, "r") as f:
        tasks = json.load(f)
except FileNotFoundError:
    tasks = []

completed = [t for t in tasks if t["IsCompleted"]]
pending = [t for t in tasks if not t["IsCompleted"]]

near_due = []
today = date.today()
for t in pending:
    # Important: convert string to date object
    due_date = datetime.fromisoformat(t["DueDate"]).date()
    delta = (due_date - today).days
    if delta <= 3:
        near_due.append(t["Title"])

# Write summary
with open(summary_path, "w") as f:
    f.write("Task Summary Report\n")
    f.write("-------------------\n")
    f.write(f"Total Tasks: {len(tasks)}\n")
    f.write(f"Completed: {len(completed)}\n")
    f.write(f"Pending: {len(pending)}\n")
    f.write(f"Tasks Near Due: {', '.join([t for t in near_due])}\n")


