# Layout View Options

## The Default Shared _Layout

Web pages should have a consistent look and feel. One of the key ways this is achieved with ASP.net MVC is through the `_layout.cshtml` page.

By default the `Views/Shared/_layout.cshtml` provides a template that all Views will use. The HTML content of any given View is inserted into the template via the `@RenderBody()` command.

This provides a great starting point allowing for a `_layout.cshtml` page to be developed with a common skeleton HTML layout including references to CSS and Javascript files. This is exactly what the starter project provides via the popular Bootstrap CSS Framework.

## Remove MVC starter template UI code

Before adding custom styling and navigation behavior, we can simplify the starter template UI.

Locate `Views/Shared/_Layout.cshtml` and replace the code with:

```html
<!DOCTYPE html>
<html lang="en">
<head>
	<meta charset="utf-8" />
	<meta name="viewport" content="width=device-width, initial-scale=1.0" />
	<title>@ViewData["Title"]</title>
</head>
<body>
	@RenderBody()
</body>
</html>
```
This stripped-down version is useful for understanding how layouts work before adding navigation and custom UI behavior.

## Adding Navigation

We have now lost our navigation between pages.  Instead of adding code to each view, we could add common content to the shared layout file.  Replace ` @RenderBody()` in `Views/Shared/_Layout.cshtml` with:

```html
<nav>
        <menu>
            <li><a href="/">Home</a></li>
            <li><a href="/Home/Privacy">Privacy</a></li>
            <li><a href="/Home/News">News</a></li>
            <li><a href="/Staff">Staff</a>
                <li><a href="/Home/ActionTest">ActionTest</a></li>
            </menu>
        </nav>
        <section>
            @RenderBody()
        </section>
```

## Razor

As we are using Razor we could extend the layout by adding a footer with an automatic date.

```html
<footer>
<p>@DateTime.Now.ToString("dd MMM yyyy")</p>
</footer>
```
The current date will now appear in the footer of all pages.


## Tag Helpers in Hyperlinks

In the above we hardcoded the URLs in the navigation.  We could use tag helpers `asp-controller` and `asp-action` instead of the HTML attribute `href`.   The two attributes tell the application which controller to use and which method/action in that controller.

```html
<menu>
            <li><a asp-controller="Home" asp-action="Index">Home</a></li>
            <li><a asp-controller="Home" asp-action="Privacy">Privacy</a></li>
            <li><a asp-controller="Home" asp-action="News">News</a></li>
            <li><a asp-controller="Staff" asp-action="Index">Staff</a></li>
            <li><a asp-controller="Home" asp-action="ActionTest">ActionTest</a></li>
        </menu>
```

This approach replaces hardcoded URLs (like href="/Home/Index") with dynamic, server-side route generation.  Two of the benefits are:

- If you change your URL routing structure in `Program.cs` later (e.g., changing `/Home/Index` to `/welcome`), the framework updates all your links automatically. You do not need to hunt through your codebase to fix broken paths.
- Modern IDEs provide full IntelliSense, autocomplete, and syntax highlighting for `asp-controller` and `asp-action` attributes, reducing typos.

## Adding Static Files

We have several flexible options for managing and adding CSS styles. These range from global styles shared across the entire site to isolated styles specific to a single view.  These are two of many techiques.

### Global Stylesheets
Global stylesheets are stored in the static assets folder `wwwroot/css` and are loaded on every page via the shared layout template.

* Location: Place your .css files inside the `wwwroot/css/` directory.
* Implementation: Link the stylesheet inside the <head> tag of your shared layout file (usually located at Views/`Shared/_Layout.cshtml`):

Add the following to the `Shared/_Layout.cshtml` ensuring it is within the `<head>` of the document.

```html
<link rel="stylesheet" href="~/css/site.css" asp-append-version="true" />
```
There is a `CSS` file with this repo to use.

> [!Tip]
> The `asp-append-version="true"` attribute is a built-in cache-busting feature. It automatically adds a unique hash suffix to the file URL based on its contents, forcing browsers to download the fresh stylesheet instantly whenever you update your CSS file. 



### CSS Isolation (Component-Style Styling)
Introduced in modern .NET Core versions, CSS Isolation lets you scope styles explicitly to a single view. This prevents styles from leaking out and accidentally modifying other elements on different pages.

* Implementation: Create a CSS file directly next to your view file in the file explorer and name it exactly after the view, appended with `.css`.
* View Path: `Views/Home/Index.cshtml`
* CSS Path: `Views/Home/Index.cshtml.css` 
* How it Works: During compilation, the framework automatically bundles these files into a single, global app stylesheet (usually named `[YourProjectName].styles.css)`. It attaches a unique attribute selector (like `b-1234567890`) to both your generated HTML tags and your CSS selectors behind the scenes to lock down their scope.
