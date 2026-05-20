# MyMvcApp

This README is organized as a step-by-step walkthrough, following the same learning progression as:
- MVC file structure and middleware
- MVC views, controllers, and routing
- Razor views and formatting
- moving data from controllers to views

## 1. Project Creation and First Run

### Step 1.1 Create the app

```bash
dotnet new mvc -n MyMvcApp
```

### Step 1.2 Move into the app folder

```bash
cd MyMvcApp
```

### Step 1.3 Build and run

```bash
dotnet build
dotnet run
```

At this point, the app is the default ASP.NET Core MVC template.

Quick-start notes:
- The app runs on localhost for development.
- Stop the running app before making large structure changes, then run again.

## 2. MVC File Structure and Middleware

### Step 2.1 Understand the main folders

The default project structure is centered around MVC separation of concerns:
- `Controllers`: receives requests and returns responses
- `Models`: data and business logic types
- `Views`: Razor UI templates
- `wwwroot`: static assets (CSS, JavaScript, images)

These folder roles support MVC separation of concerns:
- Controllers decide what response to return
- Views render UI
- Models represent data and business logic

### Step 2.2 Understand key startup files

- `Program.cs`: configures services and middleware pipeline
- `appsettings.json`: application settings and environment config
- `Views/_ViewStart.cshtml`: sets the default layout (`_Layout`)
- `Views/_ViewImports.cshtml`: imports namespaces and Tag Helpers used by Razor views

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

Important middleware note:
- Order matters. Routing and endpoint mapping must be configured in the correct sequence for requests to resolve correctly.

Accuracy note for modern templates:
- Older tutorials often show `Startup.cs` and `UseStaticFiles()`.
- This project uses the newer hosting model in `Program.cs` and static asset endpoint mapping.

### Step 2.4 Static vs dynamic content

The project uses two content paths:
- Static content from `wwwroot` (CSS, JS, images)
- Dynamic content from Controller + View (for example Home, Privacy, News, Staff)

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

### Step 2.6 Verify file-structure accuracy against this project

Checked and confirmed in current source:
1. Layout selection is controlled by `Views/_ViewStart.cshtml`
2. Razor Tag Helpers are enabled in `Views/_ViewImports.cshtml`
3. `Program.cs` uses endpoint-based static asset mapping
4. Routing pattern is `{controller=Home}/{action=Index}/{id?}`

## 3. Views, Controllers, and Routing

### Step 3.1 Explore default controller actions

The starter `HomeController` includes actions for:
- `Index()`
- `Privacy()`

Each action returns `View()`, which maps to matching Razor files in `Views/Home`.

### Step 3.2 Add a custom route and view (`News`)

What was added:
1. `News()` action in `HomeController`
2. `Views/Home/News.cshtml`
3. Navigation link in `Views/Shared/_Layout.cshtml`

Result:
- URL `/Home/News` resolves to the new action and view.

### Step 3.3 Add JSON-backed `Staff` action and view

What was added:
1. `Staff()` action in `HomeController`
2. Sample staff list created in C#
3. Data serialized with `System.Text.Json`
4. JSON stored in `ViewData["StaffJson"]`
5. New Razor view at `Views/Home/Staff.cshtml`
6. Navigation link in layout

Result:
- URL `/Home/Staff` resolves and displays staff data sourced from controller JSON.

## 4. Moving Data from Controller to View

### Step 4.1 Common techniques

MVC commonly passes server data to views using:
1. `ViewBag` for quick dynamic properties
2. `ViewData` for dictionary-based values
3. strongly typed models for larger, structured data

In this project, the Staff page uses `ViewData`.

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

## 5. Razor Views and Formatting

### Step 5.1 Use Razor expressions and code blocks

In the `Staff` view, Razor is used to:
1. Read JSON from `ViewData`
2. Parse JSON in a server-side code block (`@{ ... }`)
3. Loop through items with `@foreach`
4. Render table rows and cells from parsed values

### Step 5.2 Format controller data in HTML

The `Staff` view renders:
- a formatted HTML table (ID, Name, Role, Department, Email)
- a raw JSON preview inside `<pre>`

This demonstrates a full flow from controller-generated JSON to rendered view output.

## 6. Layout, Styling, and Mobile Navigation

### Step 6.1 Replace template styling with custom CSS

Updates made in `wwwroot/css/site.css`:
- custom color tokens
- base typography and page spacing
- nav, content, table, and footer styles

This section follows the earlier template cleanup step, where bootstrap-based starter styling was removed in favor of plain project CSS.

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
