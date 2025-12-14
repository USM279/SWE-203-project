using Microsoft.EntityFrameworkCore;
using TaskManagerApp.Models;

namespace TaskManagerApp.Data
{
    // Database context - handles all database operations
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<TaskItem> TaskItems { get; set; }

        // Configure entity relationships and constraints
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Employee entity
            modelBuilder.Entity<Employee>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.Email).IsRequired();
                entity.Property(e => e.FirstName).IsRequired();
                entity.Property(e => e.LastName).IsRequired();
                entity.Property(e => e.Role).HasDefaultValue("Employee");
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.HireDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            // Configure TaskItem entity
            modelBuilder.Entity<TaskItem>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.Property(t => t.Title).IsRequired();
                entity.Property(t => t.Description).IsRequired();
                entity.Property(t => t.Status).HasDefaultValue(TaskItemStatus.NotStarted);
                entity.Property(t => t.Priority).HasDefaultValue(TaskPriority.Medium);
                entity.Property(t => t.CreatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(t => t.LastUpdated).HasDefaultValueSql("CURRENT_TIMESTAMP");

                // Configure relationship: One Employee can have many Tasks
                entity.HasOne(t => t.AssignedTo)
                      .WithMany(e => e.AssignedTasks)
                      .HasForeignKey(t => t.AssignedToId)
                      .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete
            });

            // Seed initial data
            SeedData(modelBuilder);
        }

        // Seed initial data for testing
        private void SeedData(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>().HasData(
                new Employee
                {
                    Id = 1,
                    FirstName = "Admin",
                    LastName = "User",
                    Email = "admin@taskmanager.com",
                    Password = "admin123",
                    Role = "Admin",
                    Position = "System Administrator",
                    PhoneNumber = "1234567890",
                    HireDate = DateTime.Now.AddYears(-2),
                    IsActive = true
                },
                new Employee
                {
                    Id = 2,
                    FirstName = "Obada",
                    LastName = "Test",
                    Email = "obada.test@taskmanager.com",
                    Password = "Obada1234",
                    Role = "Employee",
                    Position = "Software Developer",
                    PhoneNumber = "53190030211",
                    HireDate = DateTime.Now.AddMonths(-6),
                    IsActive = true
                }
                
            );

            modelBuilder.Entity<TaskItem>().HasData(
                new TaskItem
                {
                    Id = 1,
                    Title = "Complete Project Documentation",
                    Description = "Write comprehensive documentation for the Task Manager system including user guide and technical documentation.",
                    Status = TaskItemStatus.InProgress,
                    Priority = TaskPriority.High,
                    DueDate = DateTime.Now.AddDays(5),
                    CreatedDate = DateTime.Now.AddDays(-3),
                    LastUpdated = DateTime.Now.AddDays(-1),
                    AssignedToId = 2
                },
                new TaskItem
                {
                    Id = 2,
                    Title = "Database Optimization",
                    Description = "Optimize database queries and add proper indexes to improve application performance.",
                    Status = TaskItemStatus.NotStarted,
                    Priority = TaskPriority.Medium,
                    DueDate = DateTime.Now.AddDays(10),
                    CreatedDate = DateTime.Now.AddDays(-2),
                    LastUpdated = DateTime.Now.AddDays(-2),
                    AssignedToId = 2
                },
                new TaskItem
                {
                    Id = 3,
                    Title = "UI/UX Review",
                    Description = "Review the user interface and user experience of the application and suggest improvements.",
                    Status = TaskItemStatus.UnderReview,
                    Priority = TaskPriority.Low,
                    DueDate = DateTime.Now.AddDays(7),
                    CreatedDate = DateTime.Now.AddDays(-5),
                    LastUpdated = DateTime.Now,
                    AssignedToId = 3
                }
            );
        }
    }
}
