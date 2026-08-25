# MyRazorApp - Razor Pages Quick Start & Tutorial Guide

This quick start guide sets up a modern web application using ASP.NET Core **Razor Pages** (.NET 10). The aim of this application and tutorial is to demonstrate:

- Creating and configuring a basic Razor Pages application.
- Understanding folder structure (`Pages/`, `wwwroot/`, `Program.cs`).
- Customizing page layouts (`Pages/Shared/_Layout.cshtml`) and CSS styling.
- Navigating pages using Razor Tag Helpers (`asp-page`).
- PageModel handler methods (`OnGet()`, `OnPost()`, named handlers).
- Form validation (`[BindProperty]`, `ModelState.IsValid`, Data Annotations).
- Complete **CRUD Operations** (Create, Read/Filter, Edit, Delete).
- **Custom Database Authentication** (Register, Login, Logout, Hashed Passwords, Cookie Authentication).
- Connecting to SQLite / SQL Server databases using Entity Framework Core.

---

## 1. Project Creation and First Run

### Step 1.1 Create the Application

```bash
dotnet new razor -n MyMvcApp
```

### Step 1.2 Build and Run

```bash
cd MyMvcApp
dotnet build
dotnet run
```

Or enable hot reloads:

```bash
dotnet watch run
```

---

## 2. File Structure and Pipeline

- **`Pages/`**: Contains `.cshtml` view templates and `.cshtml.cs` `PageModel` code-behinds.
  - **`Pages/Staff/`**: Staff CRUD pages (`Index`, `Create`, `Edit`, `Delete`).
  - **`Pages/Account/`**: Authentication pages (`Login`, `Register`, `Logout`, `AccessDenied`).
  - **`Pages/DataToViews/`**: Demos for `ViewModel`, `ViewData`, `ViewBag`, and `TempData`.
- **`Models/`**: Domain entities (`Staff.cs`, `User.cs`).
- **`Services/`**: `PasswordHasherService.cs` providing PBKDF2 password hashing.
- **`Data/`**: `ApplicationDbContext.cs` mapping `Staff` and `Users` entity sets.

---

## 3. Form Validation and CRUD Workflows

- **Validation Rules**: Declared using Data Annotations on `Staff.cs` (`[Required]`, `[EmailAddress]`, `[Range]`, `[StringLength]`).
- **Server Validation**: Checked in handler methods via `if (!ModelState.IsValid) return Page();`.
- **Client Validation**: Enabled automatically via `<partial name="_ValidationScriptsPartial" />`.

### Staff CRUD Pages:
- **`Pages/Staff/Index.cshtml`**: Displays employee table with search filter and department dropdown.
- **`Pages/Staff/Create.cshtml`**: Form to create new staff member.
- **`Pages/Staff/Edit.cshtml`**: Form to edit existing staff member (route `@page "{id:int}"`).
- **`Pages/Staff/Delete.cshtml`**: Confirmation page to delete a staff record (route `@page "{id:int}"`).

---

## 4. Custom Database Authentication (No Identity Framework)

Authentication uses custom user records stored in the database with secure password hashing:

- **Password Hashing**: `PasswordHasherService` uses ASP.NET Core's `PasswordHasher<User>` (PBKDF2 with HMAC-SHA256).
- **Cookie Session**: Managed via ASP.NET Core Cookie Authentication (`AddAuthentication` & `AddCookie`).
- **Registration**: `/Account/Register` validates unique username/email and hashes passwords.
- **Login**: `/Account/Login` verifies password hash and calls `HttpContext.SignInAsync()`.
- **Logout**: `/Account/Logout` calls `HttpContext.SignOutAsync()`.

---

## 5. Build and Test Commands

```bash
dotnet build
dotnet run
```
