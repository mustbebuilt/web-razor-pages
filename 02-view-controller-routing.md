# Working with Views and Controllers for Routing

## The Controller and Action Results

The starter project provides two public methods in the `Controller/HomeController.cs`. Both actions have a result return type of `IActionResult`. This is an Action Result.

Action Results represent things the application will do as a result of the Controller Action been called. Common Action Results returns are:

`View()`
The Action Result is the corresponding View (see below).

`RedirectToAction()`
The Action Result is to redirect to another action in the application.

`Json()`
Serializes object into JSON. Used for Endpoints.

`Ok()`
Great for testing. Simple HTTP response status code of 200 and string as payload.

## Understanding Routing in MVC

How a View is selected by the Controller is known as routing. The URL request made is picked up by the controller and 'routed' to a view. The specifics for this was set out in the pattern defined in `MapControllerRoute` found in the `program.cs` file

```csharp
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();
```
	
The pattern values dictates how a HTTP request is routed by looking for a controller and action inside that controller (there can also be an option parameter id). As such a call to https://localhost:5001/Home/Privacy is passed to the `Controllers/HomeController` and the `Privacy()` action called, that in turn calls the `Views/Home/Privacy.cshtml` view.

Lets put this in table format:

|URL Called|Contoller Called|Action / Method Called|View Used|
|----|----|----|----|
|localhost:5001/Home/Privacy|Controllers/HomeController.cs|Privacy()|	Views/Home/Privacy|
|localhost:5001/Home/Index|Controllers/HomeController.cs|Index()|	Views/Home/Index|

Naming conventions are important here. The `HomeController` will look for views in `Views/Home`.

> [!NOTE]
> There are default values. The starter template set up Home as the default controller and Index as the default action. Therefore the home page can be called various ways:

| URL Called | Contoller Called | Action / Method Called | View Used |
| ---------- | ---------------- | ---------------------- | --------- |
|localhost:5001/Home/Index|Controllers/HomeController.cs|Index()|Views/Home/Index
localhost:5001/Home/|Controllers/HomeController.cs|Index()|Views/Home/Index|
|localhost:5001/|Controllers/HomeController.cs|Index()|Views/Home/Index|

## Experiment with Controllers and Views

Creating a new Route for the Home Controller
Create a new method in the `Controllers/HomeController` for a view called `News`.

``` c#
public IActionResult News()

        {

            return View();

        }
```
		
You will need to create a new view at `Views/Home/News.cshtml`.

```razor
@{
    ViewData["Title"] = "News";
}
<h1>News</h1>
```

This can be viewed at https://localhost:5001/Home/News

## Creating a new Controller with its own routes

As demonstrated above many views can be controlled by the same Controller file. However, as your application expands you may choose to create multiple controllers that create views within another route.

Create a new Controller called `Controllers/StaffController`.

```csharp
using Microsoft.AspNetCore.Mvc;

namespace MyApp.Controllers
{
    public class StaffController : Controller
    {
    
        public IActionResult Index()
        {
            return View();
        }
    }
}
```
					
If you test the above by attempting to visit `https://localhost:5001/Staff/` then an error is thrown.

## Missing View File

To fix this error create view file at `Views/Staff/Index.cshtml`.

```csharp
Views/Staff/Index.cshtml
@{
    ViewData["Title"] = "Staff";
}
<h1>Staff Page from the StaffController</h1>
```
		
Try creating another view for the `Controllers/StaffController`.

> [!Note]
> You should use multiple controller files to keep your codebase organized, maintainable, and scalable as your application grows. In a real-world application, putting all your endpoint logic into a single file quickly becomes unmanageable.

## Using the Ok Method with Controllers

Actions can be tested without Views using the Ok method. Amend `Controllers/HomeController.cs` with an action of:

```csharp
public IActionResult ActionTest()
        {
            return Ok("Just a Test");
        }
```

This can be tested with https://localhost:xxxx/Home/ActionTest.

