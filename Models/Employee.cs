using System.ComponentModel.DataAnnotations;

namespace TaskManagerApp.Models
{
    // Employee entity - represents a user in the system
    // One employee can have many tasks
    public class Employee
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "First name is required")]
        [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name is required")]
        [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [StringLength(100)]
        [Display(Name = "Position")]
        public string? Position { get; set; }

        [StringLength(15)]
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        [Required]
        [Display(Name = "Role")]
        public string Role { get; set; } = "Employee"; // "Admin" or "Employee"

        [Display(Name = "Hire Date")]
        [DataType(DataType.Date)]
        public DateTime HireDate { get; set; } = DateTime.Now;

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;

        // Navigation property - tasks assigned to this employee
        public virtual ICollection<TaskItem> AssignedTasks { get; set; } = new List<TaskItem>();

        // Computed property for full name
        [Display(Name = "Full Name")]
        public string FullName => $"{FirstName} {LastName}";
    }
}
