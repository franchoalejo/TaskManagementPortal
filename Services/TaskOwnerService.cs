// Services/TaskOwnerService.cs
using TaskManagementPortal.Models;
using System.Collections.Generic;
using System.IO;

namespace TaskManagementPortal.Services
{
    // Simple service to load task owners from a text file
    public static class TaskOwnerService
    {
        private static List<TaskOwner> _owners;

        public static List<TaskOwner> GetOwners()
        {
            if (_owners == null)
            {
                _owners = new List<TaskOwner>();
                var lines = File.ReadAllLines("Data/TaskOwners.txt"); // Path to the TXT file
                foreach (var line in lines)
                {
                    var parts = line.Split(',');
                    if (parts.Length == 2)
                        _owners.Add(new TaskOwner { Name = parts[0].Trim(), Role = parts[1].Trim() });
                }
            }
            return _owners;
        }
    }
}
