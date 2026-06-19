# Passing Data to Views in ASP.NET Core MVC (.NET 10)

Server Side Technologies need to be able to send data from the Controller (often prescribed by the Model) to the View. Let's look at some of the common approaches.

There are four common ways to pass data from a controller to a view in ASP.NET Core MVC:

1. **ViewModel (Recommended)**
2. **ViewData**
3. **ViewBag**
4. **TempData**

## ViewModel (Recommended)

A ViewModel is the preferred way to pass data to a view because it provides:

* Compile-time type checking
* IntelliSense support
* Better maintainability
* Easier testing

### Create a ViewModel

```csharp
namespace MvcDemo.ViewModels;

public class HomeViewModel
{
    public string? Message { get; set; }

    public List<string> Products { get; set; } = [];
}
```

### Populate the ViewModel in a Controller

```csharp
public IActionResult Index()
{
    var model = new HomeViewModel
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

### Use the ViewModel in a View

```html
@model HomeViewModel

<h1>@Model.Message</h1>

<ul>
@foreach (var product in Model.Products)
{
    <li>@product</li>
}
</ul>
```

---

## ViewData

`ViewData` is a dictionary used to pass small amounts of data from a controller to a view.

### Controller

```csharp
public IActionResult Index()
{
    ViewData["Message"] = "Hello from ViewData";

    return View();
}
```

### View

```html
<h1>@ViewData["Message"]</h1>
```

### Passing Objects

```csharp
ViewData["Count"] = 10;
```

```csharp
var count = (int)ViewData["Count"]!;
```

### Pros

* Simple to use
* Available in views, layouts, and partial views

### Cons

* No compile-time checking
* Requires casting for non-string values
* More prone to runtime errors

---

## ViewBag

`ViewBag` provides dynamic access to the same underlying data store used by `ViewData`. It is effectively a wrapper around `ViewData`. ([Microsoft Learn][1])

### Controller

```csharp
public IActionResult Index()
{
    ViewBag.Message = "Hello from ViewBag";

    return View();
}
```

### View

```html
<h1>@ViewBag.Message</h1>
```

### Pros

* Cleaner syntax
* No casting required

### Cons

* No compile-time checking
* Runtime errors if property names are mistyped

---

## TempData

`TempData` stores data between requests and is commonly used after redirects.

Typical use cases include:

* Success messages
* Error messages
* Notifications

### Controller

```csharp
public IActionResult Save()
{
    TempData["Success"] = "Record saved successfully";

    return RedirectToAction(nameof(Index));
}
```

### Target Action

```csharp
public IActionResult Index()
{
    return View();
}
```

### View

```html
@if (TempData["Success"] is string message)
{
    <div class="alert alert-success">
        @message
    </div>
}
```

### This Site's Example

In this project, the TempData demo uses a POST action to set a flash message and then redirects back to the demo page:

```csharp
public IActionResult TempDataDemo()
{
    ViewData["Title"] = "TempData Example";

    return View();
}

[HttpPost]
public IActionResult SetTempDataMessage()
{
    TempData["StatusMessage"] = $"Product saved successfully at {DateTime.Now:t}.";

    return RedirectToAction(nameof(TempDataDemo));
}
```

The view then reads the message once and displays it after the redirect.

### Keep TempData for Another Request

```csharp
TempData.Keep();
```

or

```csharp
var message = TempData.Peek("Success");
```

### Pros

* Survives redirects
* Ideal for one-time notifications

### Cons

* Not intended for large amounts of data
* Automatically removed after reading unless preserved

---

## Comparison

| Feature                        | ViewModel | ViewData | ViewBag | TempData |
| ------------------------------ | --------- | -------- | ------- | -------- |
| Strongly typed                 | ✅         | ❌        | ❌       | ❌        |
| IntelliSense support           | ✅         | ❌        | ❌       | ❌        |
| Compile-time checking          | ✅         | ❌        | ❌       | ❌        |
| Survives redirect              | ❌         | ❌        | ❌       | ✅        |
| Suitable for complex data      | ✅         | ⚠️        | ⚠️       | ❌        |
| Recommended for main view data | ✅         | ❌        | ❌       | ❌        |

---

## Recommended Usage in .NET 10

| Scenario                               | Recommendation |
| -------------------------------------- | -------------- |
| Main page data                         | ViewModel      |
| Lists and complex objects              | ViewModel      |
| Page title or small UI values          | ViewData       |
| Quick prototyping                      | ViewBag        |
| Success/error messages after redirects | TempData       |

## Summary

For modern ASP.NET Core MVC applications targeting .NET 10:

* **Use ViewModels for almost all view data.**
* **Use ViewData sparingly for small UI values.**
* **Use ViewBag only when dynamic access is acceptable.**
* **Use TempData for redirect-based messages and short-lived state.**

This approach aligns with current Microsoft guidance and modern ASP.NET Core MVC development practices. ([Microsoft Learn][1])

[1]: https://learn.microsoft.com/en-us/aspnet/core/mvc/views/overview?view=aspnetcore-9.0&utm_source=chatgpt.com "Views in ASP.NET Core MVC | Microsoft Learn"
[2]: https://www.reddit.com/r/csharp/comments/lb1gah?utm_source=chatgpt.com ".net Core MVC App - why isn't TempData storing the value?"
