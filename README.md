# Task Manager Application

A comprehensive web-based task management system built with ASP.NET Core MVC. This application allows administrators to manage employees and tasks, while employees can view and update their assigned tasks.

## 📋 Table of Contents

- [Features](#features)
- [Technologies Used](#technologies-used)
- [Prerequisites](#prerequisites)
- [Installation](#installation)
- [Configuration](#configuration)
- [Usage](#usage)
- [Project Structure](#project-structure)
- [Database Schema](#database-schema)
- [Authentication & Authorization](#authentication--authorization)
- [Default Accounts](#default-accounts)
- [Screenshots](#screenshots)
- [Contributing](#contributing)
- [License](#license)

## ✨ Features

### For Administrators
- ✅ Full CRUD operations for employees
- ✅ Create, assign, edit, and delete tasks
- ✅ View all tasks and employees
- ✅ Dashboard with comprehensive statistics
- ✅ Reassign tasks to different employees

### For Employees
- ✅ View assigned tasks only
- ✅ Update task status and notes
- ✅ View personal dashboard statistics
- ✅ Track task progress and deadlines

### General Features
- 🔐 Secure cookie-based authentication
- 👥 Role-based access control (Admin/Employee)
- 📊 Real-time dashboard statistics
- 📱 Responsive design with Bootstrap 5
- 🎨 Modern and intuitive user interface
- 📝 Task status tracking (Not Started, In Progress, Under Review, Completed)
- ⚡ Priority levels (Low, Medium, High, Critical)
- 📅 Due date management
- 🔍 Task filtering and sorting

## 🛠️ Technologies Used

- **Framework**: ASP.NET Core 8.0 MVC
- **Database**: SQLite (Entity Framework Core)
- **Authentication**: Cookie-based Authentication
- **Frontend**: 
  - Bootstrap 5.3.2
  - Font Awesome 6.5.1
  - Google Fonts (Inter)
- **Language**: C# 12
- **IDE**: Visual Studio / VS Code / Rider

## 📦 Prerequisites

Before running this application, ensure you have the following installed:

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later
- Git (for cloning the repository)
- A code editor (Visual Studio, VS Code, or Rider)

## 🚀 Installation

### Step 1: Clone the Repository

```bash
git clone git@github.com:USM279/SWE-203-project.git
cd SWE-203-project
```

### Step 2: Restore Dependencies

```bash
dotnet restore
```

### Step 3: Build the Project

```bash
dotnet build
```

### Step 4: Run the Application

```bash
dotnet run
```

The application will start and be available at:
- HTTP: `http://localhost:5000`
- HTTPS: `https://localhost:5001`

## ⚙️ Configuration

### Database Connection

The application uses SQLite database. The connection string is configured in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=app.db"
  }
}
```

The database will be automatically created on first run with seed data.

### Logging

Logging is configured in `appsettings.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

## 📖 Usage

### First Time Setup

1. Run the application using `dotnet run`
2. The database will be automatically created with seed data
3. Use the default accounts (see [Default Accounts](#default-accounts)) to log in

### Login

1. Navigate to the login page: `/Account/Login`
2. Enter your email and password
3. Click "Login"

### For Administrators

1. **Manage Employees**: Navigate to `/Employees` to view, create, edit, or delete employees
2. **Manage Tasks**: Navigate to `/TaskItems` to create, assign, edit, or delete tasks
3. **Dashboard**: View comprehensive statistics on the home page

### For Employees

1. **View Tasks**: Navigate to `/TaskItems` to see your assigned tasks
2. **Update Tasks**: Click on a task to view details and update status/notes
3. **Dashboard**: View your personal task statistics on the home page

## 📁 Project Structure

```
SWE-203-project/
├── Controllers/          # MVC Controllers
│   ├── AccountController.cs
│   ├── EmployeesController.cs
│   ├── HomeController.cs
│   └── TaskItemsController.cs
├── Data/                 # Data Access Layer
│   └── ApplicationDbContext.cs
├── Models/              # Entity Models
│   ├── Employee.cs
│   ├── TaskItem.cs
│   └── ViewModels.cs
├── Views/               # Razor Views
│   ├── Account/
│   ├── Employees/
│   ├── Home/
│   ├── TaskItems/
│   └── Shared/
├── Properties/          # Application Properties
│   └── launchSettings.json
├── Program.cs           # Application Entry Point
├── appsettings.json     # Configuration
├── TaskManagerApp.csproj # Project File
└── README.md           # This File
```

## 🗄️ Database Schema

### Employee Table
- `Id` (Primary Key)
- `FirstName`, `LastName`
- `Email` (Unique)
- `Password`
- `Role` (Admin/Employee)
- `Position`, `PhoneNumber`
- `HireDate`, `IsActive`

### TaskItem Table
- `Id` (Primary Key)
- `Title`, `Description`
- `Status` (NotStarted, InProgress, UnderReview, Completed)
- `Priority` (Low, Medium, High, Critical)
- `DueDate`, `CreatedDate`, `CompletedDate`, `LastUpdated`
- `AssignedToId` (Foreign Key → Employee)
- `Notes`

### Relationship
- **One Employee** can have **Many Tasks** (One-to-Many)

## 🔐 Authentication & Authorization

### Authentication
- Cookie-based authentication
- Session timeout: 1 day (30 days with "Remember Me")
- Sliding expiration enabled

### Authorization Policies
- **AdminOnly**: Requires Admin role
- **EmployeeOnly**: Requires Employee role
- **AdminOrEmployee**: Requires either Admin or Employee role

### Access Control
- **Admins**: Full access to all features
- **Employees**: 
  - Can only view/edit their own tasks
  - Can only view their own profile
  - Cannot create or delete tasks
  - Cannot access employee management

## 👤 Default Accounts

The application comes with pre-seeded accounts:

### Administrator
- **Email**: `admin@taskmanager.com`
- **Password**: `admin123`
- **Role**: Admin

### Employee
- **Email**: `obada.test@taskmanager.com`
- **Password**: `Obada1234`
- **Role**: Employee

> **Note**: In production, passwords should be hashed using secure algorithms (BCrypt, Argon2, etc.)

## 📸 Screenshots

### Dashboard (Admin View)
- Total employees count
- Total tasks count
- Completed tasks
- Pending tasks
- Overdue tasks

### Dashboard (Employee View)
- My tasks count
- My completed tasks
- My pending tasks
- My overdue tasks

### Task Management
- Task list with filtering
- Task details view
- Task creation/editing forms
- Task assignment to employees

## 🔧 Development

### Running in Development Mode

```bash
dotnet run --environment Development
```

### Building for Production

```bash
dotnet publish -c Release
```

### Database Migrations

For production, use Entity Framework migrations:

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

Currently, the application uses `EnsureCreated()` for simplicity in development.

## 📝 Code Structure

### Controllers
- Follow MVC pattern
- Use dependency injection
- Implement proper error handling
- Include logging for audit trails

### Models
- Use data annotations for validation
- Include navigation properties for relationships
- Computed properties for derived data

### Views
- Razor syntax for dynamic content
- Bootstrap 5 for responsive design
- Client-side validation
- Consistent layout with shared components

## 🐛 Troubleshooting

### Database Issues
- If database doesn't create: Delete `app.db` and restart the application
- Check connection string in `appsettings.json`

### Authentication Issues
- Clear browser cookies if login fails
- Check that employee account is active (`IsActive = true`)

### Build Errors
- Run `dotnet restore` to restore packages
- Ensure .NET 8.0 SDK is installed
- Check `TaskManagerApp.csproj` for correct package versions

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📄 License

This project is developed for educational purposes as part of SWE-203 course.

## 👥 Authors

- **USM279 Team**

## 🙏 Acknowledgments

- ASP.NET Core documentation
- Entity Framework Core documentation
- Bootstrap team for the UI framework

## 📞 Support

For issues or questions, please open an issue in the GitHub repository.

---

**Last Updated**: December 2024
