using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TaskManagementPortal.Models
{
    public class TodoTask
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Title { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        public bool IsCompleted { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Assignment Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime AssignedDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Due Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DueDate { get; set; }

        [Display(Name = "Owners")]
        public List<string> AssignedOwners { get; set; } = new List<string>();

        // --- Convenience computed fields (not persisted) ---
        public int DaysRemaining => (int)(DueDate.Date - DateTime.Today).TotalDays;
        public bool IsOverdue => !IsCompleted && DueDate.Date < DateTime.Today;
    }
}
