# MyMvcApp

This quick start guide will set up a web application using the .net MVC template. The aim of this quick start guide is to:

 - Create and Configure a basic application from the starter project provided.
 - Review the Structure of the Model-View-Controller Application.
 - Edit the View and remove the Bootstrap framework to use a more basic HTML set up.
 - Add additional static pages to the wwwroot folder.
 - Add additional dynamic pages via the View-Controller.
 - Pass Data from the Controller to the View.

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

Quick-start notes:
- The app runs on localhost for development.
- Stop the running app before making large structure changes, then run again.  To stop the application use `Ctrl-C`

## 2. MVC File Structure and Middleware

### Step 2.1 Understand the main folders

The default project structure is centered around MVC separation of concerns:

The main folders are:

#### Models ####
Models hold the business logic. Models to represent the data used in the application will be created here. In this simple application we won't make use of the Model.

#### Views ####
The Views that contain the HTML and other content. By default file _ViewStart.cshtml links to the _Layout.cshtml where the default Bootstrap HTML/CSS is set.

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

- `Program.cs`: configures services and middleware pipeline
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
3. `UseHttpsRedirection()`
4. `UseRouting()`
5. `UseAuthorization()`
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

### Step 2.5 Remove MVC starter template UI code

Before adding custom styling and navigation behavior, the starter template UI was simplified.

What was removed or replaced:
1. Bootstrap-oriented layout structure from `Views/Shared/_Layout.cshtml`
2. Template Bootstrap references and default scaffold markup
3. Template-centric starter CSS rules in `wwwroot/css/site.css`

What replaced it:
1. A basic shared layout shell (`<head>`, `<nav>`, `@RenderBody()`, footer)
2. Project-specific CSS written in plain CSS (no framework dependency)

Minimal teaching example used at this stage (`Views/Shared/_Layout.cshtml`):

```cshtml
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

Why this step is useful:
- Makes the page structure easier to learn in an MVC beginner flow
- Keeps focus on Controller -> View behavior instead of framework utility classes

Practical benefit for beginners:

- By reducing starter-template complexity first, each new feature (routes, data binding, responsive nav) is easier to isolate and understand.
- Troubleshooting becomes simpler because fewer moving parts are involved.

### Step 2.6 Verify file-structure accuracy against this project

Checked and confirmed in current source:
1. Layout selection is controlled by `Views/_ViewStart.cshtml`
2. Razor Tag Helpers are enabled in `Views/_ViewImports.cshtml`
3. `Program.cs` uses endpoint-based static asset mapping
4. Routing pattern is `{controller=Home}/{action=Index}/{id?}`

### Step 2.7 Adding Navigation Links in MVC vs Standard HTML

In ASP.NET Core MVC, navigation links are typically added using Razor Tag Helpers, which generate URLs based on your routing configuration. This is different from standard HTML links, which use static `href` attributes.

MVC Tag Helper link example:

```cshtml
<a asp-controller="Home" asp-action="Index">Home</a>
```

- `asp-controller` and `asp-action` are processed by MVC to generate the URL.
- If your routing changes, generated links remain accurate without manual path updates.

Standard HTML link example:

```html
<a href="/Home/Index">Home</a>
```

- This is a hardcoded path and must be changed manually if routes are updated.

Basic layout with navigation links (`Views/Shared/_Layout.cshtml`):

```cshtml
<!DOCTYPE html>
<html lang="en">
<head>
	<meta charset="utf-8" />
	<meta name="viewport" content="width=device-width, initial-scale=1.0" />
	<title>@ViewData["Title"]</title>
</head>
<body>
	<nav>
		<ul>
			<li><a asp-controller="Home" asp-action="Index">Home</a></li>
			<li><a asp-controller="Home" asp-action="Privacy">Privacy</a></li>
		</ul>
	</nav>

	@RenderBody()

	<footer>
		<p>&copy; 2024 My Application</p>
	</footer>
</body>
</html>
```

Recommendation:

- Prefer Tag Helpers for internal MVC navigation to keep links aligned with route configuration.

## 3. Views, Controllers, and Routing

### Step 3.1 Explore default controller actions

The starter `HomeController` includes actions for:
- `Index()`
- `Privacy()`

Each action returns `View()`, which maps to matching Razor files in `Views/Home`.

How this convention works:

- Action method name `Index()` maps to `Views/Home/Index.cshtml`.
- Action method name `Privacy()` maps to `Views/Home/Privacy.cshtml`.

This convention-based mapping reduces boilerplate and keeps controllers readable.

### Step 3.2 Add a custom route and view (`News`)

To better understand the MVC set we'll add a new page News.  This will mean we need to:

1. Add a `News()` action in `HomeController`
2. Create a view at `Views/Home/News.cshtml`
3. Add a new navigation link in `Views/Shared/_Layout.cshtml`

Controller update (`Controllers/HomeController.cs`):

```csharp
public IActionResult News()
{
	return View();
}
```

View file (`Views/Home/News.cshtml`):

```cshtml
@{
	ViewData["Title"] = "News";
}

<h1>News</h1>
```

Next add the link to the layout with:

```cshtml
<li><a asp-controller="Home" asp-action="News">News</a></li>
```

How this works:

- The `News()` action responds to `/Home/News`.
- `return View();` tells MVC to render the view that matches the action name.
- MVC convention maps this to `Views/Home/News.cshtml` automatically.

Result:
- URL `/Home/News` resolves to the new action and view.

Learning goal for this step:

- Understand that adding a new page in MVC is usually a two-part change: controller action + matching view.
- Once the route pattern is in place, MVC wires the URL to your action using naming conventions.

### Step 3.3 Add JSON-backed `Staff` action and view

What was added:
1. `Staff()` action in `HomeController`
2. Sample staff list created in C#
3. Data serialized with `System.Text.Json`
4. JSON stored in `ViewData["StaffJson"]`
5. New Razor view at `Views/Home/Staff.cshtml`
6. Navigation link in layout

Controller update (`Controllers/HomeController.cs`):

```csharp
public IActionResult Staff()
{
	var staff = new[]
	{
		new { Id = 1, Name = "Alice Johnson", Role = "Engineering Manager", Department = "Engineering", Email = "alice.johnson@company.com" },
		new { Id = 2, Name = "Brian Lee", Role = "Senior Developer", Department = "Engineering", Email = "brian.lee@company.com" },
		new { Id = 3, Name = "Carla Gomez", Role = "HR Specialist", Department = "Human Resources", Email = "carla.gomez@company.com" }
	};

	ViewData["StaffJson"] = JsonSerializer.Serialize(staff);
	return View();
}
```

What this controller code does:

- Builds a small in-memory list of staff records.
- Converts that list to JSON using `System.Text.Json`.
- Stores the JSON text in `ViewData["StaffJson"]` so the view can read it.

View file (`Views/Home/Staff.cshtml`):

```cshtml
@using System.Text.Json
@{
	ViewData["Title"] = "Staff";

	var staffJson = ViewData["StaffJson"] as string ?? "[]";
	var staffItems = JsonDocument.Parse(staffJson).RootElement.EnumerateArray().ToList();
}

<h1>Staff</h1>

<table>
	<thead>
		<tr>
			<th>ID</th>
			<th>Name</th>
			<th>Role</th>
			<th>Department</th>
			<th>Email</th>
		</tr>
	</thead>
	<tbody>
		@foreach (var staff in staffItems)
		{
			<tr>
				<td>@staff.GetProperty("Id")</td>
				<td>@staff.GetProperty("Name")</td>
				<td>@staff.GetProperty("Role")</td>
				<td>@staff.GetProperty("Department")</td>
				<td>@staff.GetProperty("Email")</td>
			</tr>
		}
	</tbody>
</table>

<h2>Raw JSON</h2>
<pre>@staffJson</pre>
```

What this view code does:

- Reads the JSON string from `ViewData`.
- Parses JSON into an array and loops through each item.
- Renders each staff member in a table row.
- Shows the raw JSON at the bottom for learning/debugging.

Layout navigation link (`Views/Shared/_Layout.cshtml`):

```cshtml
<li><a asp-controller="Home" asp-action="Staff">Staff</a></li>
```

This link adds `/Home/Staff` to your site navigation.

Result:
- URL `/Home/Staff` resolves and displays staff data sourced from controller JSON.

Why use JSON in this learning example:

- It demonstrates a realistic server-to-view data format often used in APIs.
- It helps show where serialization belongs (controller) versus where rendering belongs (view).

## 4. Moving Data from Controller to View

### Step 4.1 Common techniques

MVC commonly passes server data to views using:
1. `ViewBag` for quick dynamic properties
2. `ViewData` for dictionary-based values
3. strongly typed models for larger, structured data

In this project, the Staff page uses `ViewData`.

When to choose each option:

- `ViewBag`: useful for very small, quick values.
- `ViewData`: useful when you want explicit key/value lookup.
- Strongly typed model: preferred for larger pages and compile-time safety.

For production projects, strongly typed models are usually the long-term direction.

### Step 4.2 How Staff data is passed

Current implementation flow:
1. Build a C# object list in `HomeController.Staff()`
2. Serialize to JSON with `System.Text.Json`
3. Store in `ViewData["StaffJson"]`
4. Read that value in the Razor view and parse it
5. Render parsed results into an HTML table

Why this matters:
- Demonstrates real controller-to-view data transfer
- Keeps data preparation in the controller
- Keeps rendering concerns in the view

This is a good foundation before moving to typed view models, where the same separation principle still applies.

## 5. Razor Views and Formatting

### Step 5.1 Use Razor expressions and code blocks

In the `Staff` view, Razor is used to:
1. Read JSON from `ViewData`
2. Parse JSON in a server-side code block (`@{ ... }`)
3. Loop through items with `@foreach`
4. Render table rows and cells from parsed values

Razor benefit:

- You can keep HTML readable while embedding only the minimum server-side logic needed for rendering.

### Step 5.2 Format controller data in HTML

The `Staff` view renders:
- a formatted HTML table (ID, Name, Role, Department, Email)
- a raw JSON preview inside `<pre>`

This demonstrates a full flow from controller-generated JSON to rendered view output.

Formatting tip:

- Keep data extraction close to the top of the view, and keep rendering markup below it.
- This makes views easier to scan and maintain.

### Step 5.3 Render C# expressions in views

Razor allows you to embed C# expressions directly in HTML using the `@` symbol. This is useful for displaying dynamic data, calling methods, and evaluating conditions.

Common Razor syntax:

- `@variable` – outputs the value of a variable
- `@expression` – evaluates a C# expression and outputs the result
- `@{ ... }` – code block for multiple C# statements
- `@if (condition) { ... }` – conditional rendering

Footer copyright year example (`Views/Shared/_Layout.cshtml`):

```cshtml
<footer>
    <p>&copy; @DateTime.Now.Year - My Application</p>
</footer>
```

What this does:

- `@DateTime.Now.Year` evaluates the current year at runtime.
- The footer automatically displays the correct copyright year without manual updates.

More detailed example with formatting:

```cshtml
<footer>
    <p>Page generated on @DateTime.Now.ToString("ddd d MMM yyyy")</p>
    <p>&copy; @DateTime.Now.Year My Application. All rights reserved.</p>
</footer>
```

This example demonstrates:

- Using `DateTime.Now` to get the current date and time.
- Using `ToString()` with a format string to display the date in a readable format (e.g., "Mon 20 May 2026").
- Using `@DateTime.Now.Year` to extract just the year for copyright notices.

Benefits of rendering C# in views:

- Dynamic content is calculated at page render time, so it's always current.
- No need to hard-code dates, years, or other values that change.
- Keeps UI logic where it belongs (in the view) while separating data fetching logic from the controller.

## 6. Layout, Styling, and Mobile Navigation

### Step 6.1 Replace template styling with custom CSS

Updates made in `wwwroot/css/site.css`:
- custom color tokens
- base typography and page spacing
- nav, content, table, and footer styles

This section follows the earlier template cleanup step, where bootstrap-based starter styling was removed in favor of plain project CSS.

Why this helps learning:

- You can see exactly which CSS rules affect each element.
- It avoids framework class dependencies while building confidence with core HTML/CSS.

### Step 6.2 Add a mobile-first burger menu

Layout updates in `Views/Shared/_Layout.cshtml`:
- burger button with accessibility attributes
- menu list container for controlled expand/collapse
- script include for `wwwroot/js/site.js`

JavaScript updates in `wwwroot/js/site.js`:
- toggle menu open/close
- animate burger icon state via CSS class
- update `aria-expanded`
- close menu on mobile link click
- reset menu state on desktop resize

Accessibility note:

- The `aria-controls` and `aria-expanded` attributes communicate menu state to assistive technologies.
- Keeping these in sync in JavaScript is an important part of accessible navigation.

### Step 6.3 Add the burger menu code to `Views/Shared/_Layout.cshtml`

Use this layout code to add the burger button, menu links, and script/CSS wiring:

```cshtml
<!DOCTYPE html>
<html lang="en">
<head>
	<meta charset="utf-8" />
	<meta name="viewport" content="width=device-width, initial-scale=1.0" />
	<title>@ViewData["Title"]</title>
	<link rel="stylesheet" href="~/css/site.css" asp-append-version="true" />
</head>
<body>
	<header>
		<nav class="site-nav" aria-label="Main navigation">
			<button class="burger" type="button" aria-controls="primary-menu" aria-expanded="false" aria-label="Toggle navigation">
				<span></span>
				<span></span>
				<span></span>
			</button>

			<ul id="primary-menu" class="menu">
				<li><a asp-controller="Home" asp-action="Index">Home</a></li>
				<li><a asp-controller="Home" asp-action="News">News</a></li>
				<li><a asp-controller="Home" asp-action="Staff">Staff</a></li>
				<li><a asp-controller="Home" asp-action="Privacy">Privacy</a></li>
			</ul>
		</nav>
	</header>

	<main>
		@RenderBody()
	</main>

	<footer>
		<p>Sample Footer: @DateTime.Now.ToString("ddd d MMM yyyy")</p>
	</footer>

	<script src="~/js/site.js" asp-append-version="true"></script>
</body>
</html>
```

### Step 6.4 JavaScript burger menu explanation

The script is intentionally small and framework-free:

1. Startup and element lookup
- Waits for `DOMContentLoaded` to ensure nav elements exist.
- Finds `.site-nav`, `.burger`, and `#primary-menu`.
- Exits early if any element is missing, preventing runtime errors.

2. Single state function
- Uses one helper (`setOpenState`) to keep behavior consistent.
- Adds/removes `.is-open` on the nav container.
- Updates `aria-expanded` to keep screen readers in sync.

3. Toggle behavior
- Burger click inverts current state.
- CSS transitions handle the animation (menu slide/fade and burger-to-X).

4. Mobile interaction polish
- Clicking a menu link closes the menu on small screens.
- Prevents the menu from staying open after navigation tap.

5. Resize safety
- On resize to desktop width, JS resets mobile open state.
- Avoids stale open/collapsed states when moving between viewport sizes.

CSS behavior (mobile first):
- small screens: collapsed menu opened by burger button
- larger screens (768px+): horizontal always-visible menu, burger hidden

## 7. Current Application Outcome

The project now includes:
1. Default MVC foundation from `dotnet new mvc`
2. Custom `News` route/view
3. `Staff` route/view with JSON passed from controller and formatted in Razor
4. Custom non-framework CSS theme
5. Mobile-first responsive burger menu with vanilla JavaScript and CSS animations

## 8. Commands You Can Reuse

```bash
dotnet build
dotnet run
```

If `dotnet build` fails because the executable is locked, stop the currently running app process and build again.
