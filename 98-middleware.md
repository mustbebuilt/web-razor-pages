# MVC File Structure and Middleware

### Understand the main folders

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

### Understand key startup files


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

### Middleware and routing pipeline

## The program.cs Explained

The `program.cs` file contains the following:

- `WebApplication.CreateBuilder(args)`: Creates the app builder, loads config, logging, and command-line args.
- `builder.Services.AddControllersWithViews()`: Registers MVC services so controllers and Razor views work.
- `builder.Build()`: Builds the configured app into a runnable `WebApplication`.

- `if (!app.Environment.IsDevelopment())`: Runs enclosed middleware only outside Development (for production-like environments).
- `app.UseExceptionHandler("/Home/Error")`: Sends unhandled exceptions to the `Home/Error` action.
- `app.UseHsts()`: Adds HSTS header so browsers prefer HTTPS (security hardening).

- `app.UseHttpsRedirection()`: Redirects HTTP requests to HTTPS.
- `app.UseRouting()`: Enables endpoint routing so the framework can match URLs to handlers.
- `app.UseAuthorization()`: Enforces authorization policies on matched endpoints.
- `app.MapStaticAssets()`: Maps static web assets (CSS/JS/images/libs) so they can be served (by default from `wwwroot`)
- `app.MapControllerRoute(...)`: Defines the default MVC route: `/{controller=Home}/{action=Index}/{id?}`.
- `.WithStaticAssets()`: Associates static-asset behavior with the mapped route endpoints.

- `app.Run()`: Starts the web server and begins handling incoming requests.

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

### Static vs dynamic content

The project uses two content paths:
- Static content from `wwwroot` (CSS, JS, images)
- Dynamic content from Controller + View (for example Home, Privacy, News, Staff)

Rule of thumb:

- Use `wwwroot` when the file is served directly as-is.
- Use Controller + View when server-side logic decides what should be rendered.

Examples:

- `wwwroot/css/site.css` is sent as a plain file.
- `HomeController.Staff()` prepares data, and `Views/Home/Staff.cshtml` renders it.

