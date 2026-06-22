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

## Project Creation and First Run

### Create the app

To create a starter project in Visual Studio Code open your terminal and run:

```bash
dotnet new mvc -n MyApp
```
This will build a basic MVC application in the folder `MyApp`.

### Move into the app folder

Move into the application folder with the command:

```bash
cd MyMvcApp
```

### Build and run

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
## What has just happened?

The default application consists of just two pages, a home page (or index) and a privacy page.  Let's look at how these two pages work.

### Views and Razor

Navigate to the `Views` folder in the application.  Notice a folder called `Home`.  In here we find, `Index.cshtml` and `Privacy.cshtml`.  These are c# HTML files, saved as `.cshtml` and contain familiar `HTML` elements but with a small mix of `C#` code for example:

```csharp
@{
    ViewData["Title"] = "Home Page";
}
```

This is known as Razor which allows for the mixing of `HTML` and server-side code using `C#`. Razor supports `C#` and uses the `@` symbol within a view to transition from `HTML` to `C#`. Razor evaluates `C#` expressions and renders them in the `HTML` output.

### Shared Layout Files

The above HTML in both `Index.cshtml` and `Privacy.cshtml` does seem sparse. This is because both of these views are build from a shared layout file that allows for common features to be easily included on multiple files.  This shareed layout file can be found in `Views/Shared/_layout.cshtml`.  Here we find the familar `HTML` structure of `<!DOCTYPE html>`, `<html>`, `<head>`, `<body>` etc.

### Static Files

Note the layout file makes reference to a CSS and Javascript files.  These are described as 'static' files, in that `C#` will not manipulate them in any way.  As such these 'static' files are by convention stored in a `wwwroot` folder.

### The Controller

Next open the controller file found at `Controllers/HomeController.cs`.  The default MVC set up creates this file with 'controls' for both the `Index` and `Privacy` views through two methods with names that match the views. 

```csharp
public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }
```
Public methods on a controller (except those with the `[NonAction]` attribute) are described as Actions. Actions can return anything, but frequently return an instance of `IActionResult` that produce a response. The action method is responsible for choosing what kind of response. Both of the above return Views. The View returned is the one that matches the name of the method.

### Routing in Program.cs

How these the actions in the controller are triggered is configured in the `Program.cs` file.

```csharp
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();
```

This defines the pattern by which URL requests are picked up or 'routed'.  It maps incoming browser requests to specific `C#` controllers and methods.  These are the methods / actions we saw above in the `Controllers/HomeController.cs` file.

Notice this also allows for the use of the static assets.

We will explore these concepts further as we remove some of the boilerplate code and add some of our own. 