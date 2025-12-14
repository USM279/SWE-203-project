using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManagerApp.Models
{
    // TaskItem entity - represents a task in the system
    // Many tasks belong to one employee
    public class TaskItem
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Task title is required")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        [Display(Name = "Task Title")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Task description is required")]
        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        [Display(Name = "Description")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Status")]
        public TaskItemStatus Status { get; set; } = TaskItemStatus.NotStarted;

        [Required]
        [Display(Name = "Priority")]
        public TaskPriority Priority { get; set; } = TaskPriority.Medium;

        [Required(ErrorMessage = "Due date is required")]
        [Display(Name = "Due Date")]
        [DataType(DataType.Date)]
        public DateTime DueDate { get; set; } = DateTime.Now.AddDays(7);

        [Display(Name = "Created Date")]
        [DataType(DataType.DateTime)]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Display(Name = "Completed Date")]
        [DataType(DataType.DateTime)]
        public DateTime? CompletedDate { get; set; }

        [Display(Name = "Last Updated")]
        [DataType(DataType.DateTime)]
        public DateTime LastUpdated { get; set; } = DateTime.Now;

        // Foreign key to Employee
        [Required(ErrorMessage = "Please assign this task to an employee")]
        [Display(Name = "Assigned To")]
        public int AssignedToId { get; set; }

        // Navigation property to assigned employee
        [ForeignKey("AssignedToId")]
        public virtual Employee? AssignedTo { get; set; }

        [StringLength(500)]
        [Display(Name = "Notes")]
        [DataType(DataType.MultilineText)]
        public string? Notes { get; set; }

        // Helper property - check if task is overdue
        [NotMapped]
        public bool IsOverdue => DueDate < DateTime.Now && Status != TaskItemStatus.Completed;

        // Helper property - calculate progress percentage based on status
        [NotMapped]
        public int ProgressPercentage
        {
            get
            {
                return Status switch
                {
                    TaskItemStatus.NotStarted => 0,
                    TaskItemStatus.InProgress => 50,
                    TaskItemStatus.UnderReview => 75,
                    TaskItemStatus.Completed => 100,
                    _ => 0
                };
            }
        }
    }

    // Task status enum
    public enum TaskItemStatus
    {
        [Display(Name = "Not Started")]
        NotStarted = 0,
        
        [Display(Name = "In Progress")]
        InProgress = 1,
        
        [Display(Name = "Under Review")]
        UnderReview = 2,
        
        [Display(Name = "Completed")]
        Completed = 3
    }

    // Task priority enum
    
    public enum TaskPriority
    {
        [Display(Name = "Low")]
        Low = 0,
        
        [Display(Name = "Medium")]
        Medium = 1,
        
        [Display(Name = "High")]
        High = 2,
        
        [Display(Name = "Critical")]
        Critical = 3
    }
}
