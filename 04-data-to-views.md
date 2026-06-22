# Data to Views

## ViewData and Page Titles

We have created a number of views and each time there has been some `C#` at the top of the file such as:

```html
@{
    ViewData["Title"] = "News";
}
<h1>News</h1>
```

Notice in the layout file the `@ViewData` is also referenced.

```html
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>@ViewData["Title"]</title>
</head>
```
It is important that all HTML pages generate unique `<title>` values. By setting them in the view via a `ViewData`, uniques values can be passed to the layout as a Key-Value Dictionary.

`ViewData` is a weakly typed `ViewDataDictionary` that stores objects using string keys.

## ViewData in the Controller

We can also use the `ViewData` dictionary to pass small amounts of data from a controller to a view.  In `Controllers/HomeController.cs` amend the `Index` method with:

```csharp
public IActionResult Index()
{
    ViewData["Message"] = "Hello from ViewData";

    return View();
}
```

In the `Views/Home/Index.cshtml` we can now reference this value to be displayed:


```html
<h1>@ViewData["Message"]</h1>
```

We can also send other data types such as `int`:


```csharp
ViewData["Count"] = 10;
```

But notice non-strings need to be cast:

```csharp
var count = (int)ViewData["Count"]!;
```

> [!Warning]
> While `ViewData` supports complex objects, using it this way introduces risk. For complex data, it is highly recommended to use Strongly Typed `ViewModels`.

## ViewModels

A `ViewModel` is the preferred way to pass data to a view because it provides:

* Compile-time type checking
* IntelliSense support
* Better maintainability
* Easier testing

### Create a ViewModel

In the `Models` folder create a class called `ProductModel.cs`. In the following we have a string and a `List` of products.

```csharp
namespace MyApp.ViewModels;

public class ProductModel
{
    public string? Message { get; set; }

    public List<string> Products { get; set; } = [];
}
```

### Using the ViewModel in the Controller

Ensure `using` directive at the top of controller is a is set to import the correct namespace. It functions similarly to an import statement in JavaScript/Python.

- It Tells the Compiler Where to Look
- Every class in `C#` lives inside a container called a `namespace` to keep the codebase organized. Because we created the model inside the `Models` folder, its container path is `MyApp.Models`.

```csharp
using MyApp.Models;
```

We can now amend the `Index` action in `Controller/HomeController.cs` as follows:

```csharp
public IActionResult Index()
{
    var model = new ProductModel
    {
        Message = "Welcome to ASP.NET Core MVC",
        Products =
        [
            "Laptop",
            "Monitor",
            "Keyboard"
        ]
    };

    return View(model);
}
```

Here we initiate a `HomeViewModel`, populate it and pass it to thew view.

### Use the ViewModel in a View

We can now use the `ViewModel` in the view:

```csharp
@{
    ViewData["Title"] = "Home Page";
}

<h1>@Model.Message</h1>

<ul>
    @foreach (var product in Model.Products)
    {
        <li>@product</li>
    }
</ul>
```

The above uses a `foreach` loop to display all the products found in the <List>.

## Razor Expressions

We could extend the above with more examples of razor expressions.

Conditional logic `@if` could be used to check for the presence of a `Model.Message` value.

```csharp
@if (string.IsNullOrEmpty(Model.Message))
{
    <h1>Welcome to our Store!</h1>
}
else
{
    <h1>@Model.Message</h1>
}
```

We also have access to properties of `<List>` such as `Count`.  We can add a conditional logic based on this to display a suitable message if no products available.

```csharp

@if (Model.Products.Count == 0)
{
    <p>No products available</p>
}
else
{
    <p>There are @Model.Products.Count products available</p>
    <ul>
        @foreach (var product in Model.Products)
        {
            <li>@product</li>
        }
    </ul>
}
```

> [!Note]
> The above is an illustrative example.  In reality data will not be hardcoded into a Controller but come from a source such as JSON file, API or database.

## ViewModels vs Models

In the above example, for ease, the model was created i the `Models` folder although it respresented data transfer objects (not database data).

As your application grows, your database models (Entities) and your UI models will serve completely different purposes. Separating them avoids massive confusion.

- Models Folder: Reserved strictly for your Domain/Database Entities (e.g., classes that map directly to your SQL tables using Entity Framework Core, like Product or User).
- ViewModels Folder: Reserved for UI-specific data transfer objects.

## UI ViewModel Example

To reinforce this concept lets re-structure the application and use the ViewModel data for a HTML dropdown.

Create a new folder `ViewModels` at the route of the application (the same level as `Models`, `Views`, `Controllers`).

Move the `ProductModel.cs ` from `Models` to this new `ViewModels` folder.  Amend the `namespace` accordingly:

```
namespace MyApp.ViewModels;
```
In the `Controller/HomeController` add the `using` directive to match this new namespace (along with Models for the error handling).

```
using MyMvcApp.Models;
using MyMvcApp.ViewModels;
```

The application will work just as before but to complete the example as a pure UI change amend the `index.cshmtl` view with:



