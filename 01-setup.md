# MyMvcApp

This quick start guide will set up a web application using the .net MVC template. The aim of this quick start guide is to:

 - Create and Configure a basic application from the starter project provided.
 - Review the Structure of the Model-View-Controller Application.
 - Edit the View and remove the Bootstrap framework to use a more basic HTML set up.
 - Add additional static pages to the wwwroot folder.
 - Add additional dynamic pages via the View-Controller.
 - Pass Data from the Controller to the View.

## Requirements

This tutorial will require:

- Visual Studio Code - Free, cross platform, [download](https://code.visualstudio.com/)
- .net 10.0 - Free, cross platform, [download](https://dotnet.microsoft.com/en-us/download)
- Also recommend Extensions .Net Install Tool, C# Dev Kit

## 1. Project Creation and First Run

### Step 1.1 Create the app

To create a starter project in Visual Studio Code open your terminal and run:

```bash
dotnet new mvc -n MyMvcApp
```
This will build a basic MVC application in the folder `MyMvcApp`.

### Step 1.2 Move into the app folder

Move into the application folder with the command:

```bash
cd MyMvcApp
```

### Step 1.3 Build and run

```bash
dotnet build
dotnet run
```

At this point, the app is the default ASP.NET Core MVC template.

Test the application with:

```bash
dotnet build
dotnet run
```
> [!NOTE]
> The app runs on localhost for development.
> Stop the running app before making large structure changes, then run again.  To stop the application use `Ctrl-C`

To stop having to stop and restart the application you can enable 'hot reloads' with the following:

```bash
dotnet watch run
```

## 2. MVC File Structure and Middleware

### Step 2.1 Understand the main folders

The default project structure is centered around MVC separation of concerns:

The main folders are:

#### Models ####
Models hold the business logic. Models to represent the data used in the application will be created here. In this simple application we won't make use of the Model.

#### Views ####
The Views that contain the HTML and other content. By default file `_ViewStart.cshtml` links to the `_Layout.cshtml` where the default Bootstrap HTML/CSS is set.

As a developer you can choose to make more folders inside Views to help structure your content.

#### Controllers ####
Communicates between the View and Model. MVC uses a 'separation of concerns' approach to development. The controller controls which views are displayed and whether they need the support of the Model (not all pages will).

#### wwwroot ####
Location of all 'static' files such as client side Javascript, images and CSS files. May also include HTML files that don't require any .net server side logic

These folder roles support MVC separation of concerns:
- Controllers decide what response to return
- Views render UI
- Models represent data and business logic

### Step 2.2 Understand key startup files


- `Program.cs`: configures services and middleware pipeline (`startup.up` from earlier .net versions no longer used)
- `appsettings.json`: application settings and environment config
- `Views/_ViewStart.cshtml`: sets the default layout (`_Layout`)
- `Views/_ViewImports.cshtml`: imports namespaces and Tag Helpers used by Razor views

Why these files matter in practice:

- `Program.cs` is the first place to check when routes do not resolve, middleware behaves unexpectedly, or static assets are not loading.
- `appsettings.json` allows you to keep environment-specific values out of source code and change behavior without editing controllers/views.
- `_ViewStart.cshtml` helps avoid repeating layout declarations in every view.
- `_ViewImports.cshtml` makes Razor syntax cleaner by centralizing imports and enabling MVC Tag Helpers globally.

As you work through the project, these files become your "startup checklist" whenever the app runs but page behavior is not what you expect.

### Step 2.3 Middleware and routing pipeline

In this project, `Program.cs` configures:
1. `builder.Services.AddControllersWithViews()` to register MVC services
2. Production error handling with `UseExceptionHandler` and `UseHsts`
3. `UseHttpsRedirection()`automatically redirects HTTP web requests to HTTPS.
4. `UseRouting()`analyzes the URL path and decides which controller action or page should handle the request.
5. `UseAuthorization()` authorizes a user to access secure resources (more set up required).
6. Static asset mapping via `MapStaticAssets()`
7. Default controller route mapping via `MapControllerRoute(...)`
8. `.WithStaticAssets()` chained to endpoint mapping

This enables URLs like `/Home/Index` and `/Home/Privacy` to map to controller actions and views.

How to think about this flow:

- A browser request enters the middleware pipeline in order.
- Routing middleware (`UseRouting`) matches the URL against route patterns.
- Endpoint mapping (`MapControllerRoute`) connects a match to the correct controller action.
- The action returns a view, and Razor renders the final HTML response.

If one middleware step is missing or out of order, the request may fail before it reaches your controller.

Important middleware note:
- Order matters. Routing and endpoint mapping must be configured in the correct sequence for requests to resolve correctly.

Accuracy note for modern templates:
- Older tutorials often show `Startup.cs` and `UseStaticFiles()`.
- This project uses the newer hosting model in `Program.cs` and static asset endpoint mapping.

### Step 2.4 Static vs dynamic content

The project uses two content paths:
- Static content from `wwwroot` (CSS, JS, images)
- Dynamic content from Controller + View (for example Home, Privacy, News, Staff)

Rule of thumb:

- Use `wwwroot` when the file is served directly as-is.
- Use Controller + View when server-side logic decides what should be rendered.

Examples:

- `wwwroot/css/site.css` is sent as a plain file.
- `HomeController.Staff()` prepares data, and `Views/Home/Staff.cshtml` renders it.

