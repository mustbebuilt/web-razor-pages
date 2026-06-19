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