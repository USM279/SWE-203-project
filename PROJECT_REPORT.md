# Task Manager System
## SWE203 Web Programming - Course Project Report

---

### Student Information
**Course:** SWE203 Web Programming  
**Semester:** Fall 2025  
**Project:** Task Manager System  
**Submission Date:** December 15, 2025

---

## 1. Project Overview

The Task Manager System is a comprehensive web application designed to facilitate efficient task management and employee coordination within organizations. The application implements a role-based access control system with two distinct user roles: Administrators and Employees.

### 1.1 Purpose
The primary objective of this system is to provide a centralized platform where administrators can create, assign, and monitor tasks while employees can view and update the status of their assigned responsibilities. This promotes transparency, accountability, and efficient workflow management.

### 1.2 Key Objectives
- Implement a full-featured MVC web application using ASP.NET Core
- Demonstrate proficiency in Entity Framework Core for data management
- Apply authentication and authorization mechanisms
- Create a user-friendly interface for task and employee management
- Implement CRUD operations on multiple entities

---

## 2. System Architecture

### 2.1 Technology Stack

**Backend Framework:**
- ASP.NET Core 8.0 MVC
- C# Programming Language

**Data Access Layer:**
- Entity Framework Core 8.0
- SQLite Database

**Frontend Technologies:**
- HTML5
- CSS3
- Bootstrap 5
- JavaScript

**Authentication:**
- Cookie-based Authentication
- Role-based Authorization

### 2.2 Design Pattern
The application strictly follows the Model-View-Controller (MVC) architectural pattern:

- **Models:** Represent data entities (Employee, TaskItem) and business logic
- **Views:** Handle the presentation layer and user interface
- **Controllers:** Manage user requests and coordinate between models and views

---

## 3. Core Features Implementation

### 3.1 Authentication & Authorization

**Login System:**
- Cookie-based authentication mechanism
- Secure session management
- "Remember Me" functionality for extended sessions
- Access control based on user roles

**User Roles:**
1. **Administrator:** Full system access including employee and task management
2. **Employee:** Limited access to view and update assigned tasks only

**Authorization Implementation:**
```csharp
[Authorize(Roles = "Admin")]  // Admin-only actions
[Authorize]                    // Authenticated users only
```

### 3.2 Employee Management (Admin Only)

**Features:**
- Create new employee accounts with role assignment
- View all employees with their task assignments
- Edit employee information (name, email, position, role)
- Delete employees (with validation for assigned tasks)
- View detailed employee profiles

**CRUD Operations:**
- **Create:** Add new employees with validation
- **Read:** List all employees, view individual profiles
- **Update:** Modify employee details
- **Delete:** Remove employees (protected if tasks are assigned)

### 3.3 Task Management

**Task Creation (Admin):**
- Create tasks with title, description, and due date
- Assign tasks to specific employees
- Set priority levels (Low, Medium, High, Critical)
- Set initial status (Not Started, In Progress, Under Review, Completed)
- Add optional notes

**Task Tracking (All Users):**
- View assigned tasks (employees see only their tasks)
- Update task status
- Add notes and comments
- Monitor due dates with overdue indicators
- Track progress with visual indicators

**Task Status Workflow:**
1. **Not Started (0%):** Task created but not begun
2. **In Progress (50%):** Work is underway
3. **Under Review (75%):** Task completed, awaiting approval
4. **Completed (100%):** Task finished and approved

### 3.4 Dashboard Features

**Admin Dashboard:**
- Total number of employees
- Total number of tasks
- Completed tasks count and percentage
- Overdue tasks alert
- Quick action buttons

**Employee Dashboard:**
- Personal task count
- Completed tasks percentage
- Pending tasks overview
- Overdue tasks alert
- Quick access to assigned tasks

---

## 4. Database Design

### 4.1 Entity Relationships

**Employee Entity:**
```csharp
public class Employee
{
    public int Id { get; set; }                           // Primary Key
    public string FirstName { get; set; }                 // Required
    public string LastName { get; set; }                  // Required
    public string Email { get; set; }                     // Required, Unique
    public string Password { get; set; }                  // Required
    public string? Position { get; set; }                 // Optional
    public string? PhoneNumber { get; set; }              // Optional
    public string Role { get; set; }                      // Admin/Employee
    public DateTime HireDate { get; set; }                // Default: Now
    public bool IsActive { get; set; }                    // Default: true
    public ICollection<TaskItem> AssignedTasks { get; set; } // Navigation
}
```

**TaskItem Entity:**
```csharp
public class TaskItem
{
    public int Id { get; set; }                           // Primary Key
    public string Title { get; set; }                     // Required
    public string Description { get; set; }               // Required
    public TaskStatus Status { get; set; }                // Enum
    public TaskPriority Priority { get; set; }            // Enum
    public DateTime DueDate { get; set; }                 // Required
    public DateTime CreatedDate { get; set; }             // Auto
    public DateTime LastUpdated { get; set; }             // Auto
    public DateTime? CompletedDate { get; set; }          // Optional
    public int AssignedToId { get; set; }                 // Foreign Key
    public Employee AssignedTo { get; set; }              // Navigation
    public string? Notes { get; set; }                    // Optional
}
```

### 4.2 Relationship Type
**One-to-Many Relationship:**
- One Employee can have many TaskItems
- Each TaskItem is assigned to exactly one Employee
- Implemented using Foreign Key (AssignedToId)

### 4.3 Data Seeding
The application includes initial seed data:
- 1 Admin account (admin@taskmanager.com)
- 2 Employee accounts (john.doe@taskmanager.com, jane.smith@taskmanager.com)
- 3 Sample tasks with various statuses

---

## 5. User Interface Design

### 5.1 Layout Structure
The application uses a consistent layout across all pages:
- **Navigation Bar:** Logo, menu items, user info, login/logout
- **Content Area:** Page-specific content
- **Footer:** Copyright and privacy links

### 5.2 Color Coding System

**Task Status:**
- Grey: Not Started
- Blue: In Progress
- Yellow: Under Review
- Green: Completed

**Task Priority:**
- Light Blue: Low
- Yellow: Medium
- Light Red: High
- Red: Critical

**User Roles:**
- Red Badge: Admin
- Blue Badge: Employee

### 5.3 Responsive Design
- Mobile-friendly interface using Bootstrap
- Responsive tables and cards
- Collapsible navigation for small screens

---

## 6. Project Requirements Fulfillment

### 6.1 MVC Framework ✅
- Implemented using ASP.NET Core 8.0 MVC
- Clear separation of concerns (Models, Views, Controllers)

### 6.2 Entity Framework Core ✅
- DbContext implementation with proper configuration
- One-to-Many relationship between Employee and TaskItem
- LINQ queries for data operations
- Database migrations support

### 6.3 CRUD Operations ✅
- **Employees:** Complete CRUD implementation
- **Tasks:** Complete CRUD implementation
- Proper validation and error handling

### 6.4 Controllers & Views ✅
- **4 Controllers:** Account, Home, Employees, TaskItems
- **16+ Views:** Exceeds minimum requirement
  - Account: Login, Register, AccessDenied (3 views)
  - Home: Index, Privacy (2 views)
  - Employees: Index, Create, Edit, Details, Delete (5 views)
  - TaskItems: Index, Create, Edit, Details, Delete (5 views)
  - Shared: Layout, Error, ValidationScripts (3 views)

### 6.5 Authentication & Authorization ✅
- Cookie-based authentication implemented
- Role-based authorization (Admin, Employee)
- Login/Logout functionality
- User registration with validation
- Protected routes with [Authorize] attribute

### 6.6 Additional Requirements ✅
- **Layouts:** _Layout.cshtml with consistent design
- **ViewBag/ViewData:** Used for passing data to views
- **Tag Helpers:** Used extensively in forms and links
- **Forms & Validation:** Client and server-side validation
- **Error Handling:** Try-catch blocks with logging
- **Status Messages:** TempData for user feedback

---

## 7. Application Workflow

### 7.1 Admin Workflow
1. Login with admin credentials
2. View dashboard with system statistics
3. Navigate to Employees section
4. Create new employee account
5. Navigate to Tasks section
6. Create new task and assign to employee
7. Monitor task progress
8. Update or delete tasks as needed

### 7.2 Employee Workflow
1. Login with employee credentials
2. View personal dashboard with task statistics
3. Navigate to Tasks section
4. View assigned tasks
5. Select a task to update
6. Change task status (Not Started → In Progress → Under Review → Completed)
7. Add notes about progress
8. Save changes

### 7.3 Task Lifecycle
```
Created (Admin) → Assigned to Employee → 
Not Started → In Progress → Under Review → 
Completed (Auto-set CompletedDate)
```

---

## 8. Code Quality & Best Practices

### 8.1 Code Organization
- Clear project structure with logical folder organization
- Separation of concerns between layers
- Consistent naming conventions

### 8.2 Documentation
- XML documentation comments on all classes and methods
- README file with comprehensive instructions
- Inline comments for complex logic

### 8.3 Error Handling
- Try-catch blocks in all controller actions
- Logging of errors and important operations
- User-friendly error messages
- Validation at multiple levels

### 8.4 Security Considerations
- Input validation on all forms
- CSRF protection using ValidateAntiForgeryToken
- Role-based access control
- Secure cookie configuration

**Note:** Passwords are stored in plain text for demonstration purposes. In a production environment, passwords should be hashed using bcrypt, PBKDF2, or similar algorithms.

---

## 9. Testing Scenarios

### 9.1 Admin Testing
1. **Login:** Test admin login with correct/incorrect credentials
2. **Employee CRUD:** Create, view, edit, and delete employees
3. **Task CRUD:** Create, view, edit, and delete tasks
4. **Authorization:** Verify admin-only features are inaccessible to employees
5. **Dashboard:** Verify statistics are accurate

### 9.2 Employee Testing
1. **Login:** Test employee login
2. **View Tasks:** Verify employee sees only assigned tasks
3. **Update Status:** Test task status updates
4. **Access Control:** Verify limited access (cannot create tasks, manage employees)
5. **Profile:** View personal profile

### 9.3 Data Integrity Testing
1. **Validation:** Test form validations (required fields, email format, etc.)
2. **Relationships:** Verify task-employee relationships
3. **Delete Protection:** Test that employees with assigned tasks cannot be deleted
4. **Unique Email:** Test duplicate email prevention

---

## 10. Challenges & Solutions

### 10.1 Challenge: Role-Based Views
**Problem:** Different users need different interfaces and permissions.
**Solution:** Implemented ViewBag.IsAdmin checks in views and [Authorize(Roles)] attributes in controllers to conditionally show/hide features.

### 10.2 Challenge: Task Status Updates
**Problem:** Employees should only update status, not task details.
**Solution:** Created separate authorization logic in Edit action that limits employee modifications to status and notes only.

### 10.3 Challenge: Data Relationships
**Problem:** Preventing deletion of employees with assigned tasks.
**Solution:** Added validation in Delete action to check for assigned tasks before allowing deletion.

---

## 11. Future Enhancements

1. **Security Improvements:**
   - Implement password hashing (bcrypt)
   - Add email verification
   - Implement password reset functionality
   - Two-factor authentication

2. **Feature Additions:**
   - Task comments and attachments
   - Email notifications for task assignments
   - Task priority escalation
   - Advanced filtering and search
   - Calendar view for due dates
   - Task history and audit trail

3. **User Experience:**
   - Real-time updates using SignalR
   - Drag-and-drop task management
   - Dark mode support
   - Export functionality (PDF, Excel)
   - Mobile application

4. **Performance:**
   - Caching implementation
   - Database indexing optimization
   - Pagination for large datasets

---

## 12. Conclusion

The Task Manager System successfully demonstrates comprehensive understanding and practical implementation of web development concepts covered in SWE203. The project fulfills all course requirements including:

- Complete MVC architecture implementation
- Entity Framework Core with proper relationships
- Full CRUD operations on multiple entities
- Authentication and authorization
- Responsive user interface
- Clean, maintainable code structure

The application provides a solid foundation for a production-ready task management system and showcases proficiency in ASP.NET Core MVC development, Entity Framework Core, and modern web development practices.

### Key Achievements:
✅ 4 Controllers with comprehensive functionality  
✅ 16+ Views with consistent design  
✅ Proper MVC pattern implementation  
✅ Entity Framework with One-to-Many relationship  
✅ Complete authentication & authorization  
✅ CRUD operations with validation  
✅ Role-based access control  
✅ Professional user interface  
✅ Comprehensive documentation  

---

## 13. References

- Microsoft ASP.NET Core Documentation: https://docs.microsoft.com/aspnet/core
- Entity Framework Core Documentation: https://docs.microsoft.com/ef/core
- Bootstrap 5 Documentation: https://getbootstrap.com/docs/5.0
- Course Materials: SWE203 Lecture Notes and Labs

---

**End of Report**

---

*This project was developed as part of the SWE203 Web Programming course requirements.*
