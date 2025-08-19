using System.ComponentModel.DataAnnotations;

namespace TaskManagementPortal.Models
{
    // simple todo task entity used across the app
    public class TodoTask
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(100, ErrorMessage = "Title cant be longer than 100 chars")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required")]
        [StringLength(500, ErrorMessage = "Max 500 chars for description")]
        public string Description { get; set; } = string.Empty;

        // basic status flag, nothing fancy here, change after finish 
        public bool IsCompleted { get; set; } = false;

        // List of assigned owners (names)
        public List<string> AssignedOwners { get; set; } = new List<string>();
    }
}
