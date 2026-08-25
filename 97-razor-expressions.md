# Razor Syntax Quick Reference Guide

Razor is a markup syntax that allows server-side C# code to be embedded directly into HTML web pages. In Razor Pages, expressions access page model properties via `@Model`.

---

## 1. Transitions and Expressions

### Implicit Razor Expressions
Use `@` to transition from HTML to inline C#. Razor evaluates the expression and renders the output as string content into HTML.
- **Example:** `<p>Current time is: @DateTime.Now</p>`
- **Formatted Example:** `<p>Formatted Date: @DateTime.Now.ToString("dd MMM yyyy")</p>`
- **PageModel Property:** `<h1>@Model.Message</h1>`

### Explicit Razor Expressions
Wrap an expression in parentheses `@(...)` to calculate values or perform string operations before rendering.
- **Example:** `<p>One week ago was: @(DateTime.Now - TimeSpan.FromDays(7))</p>`

### Escaping the `@` Symbol
- Email addresses are parsed automatically as text without triggering Razor code (e.g. `info@example.com`).
- To output a literal `@` symbol elsewhere on screen, escape it with a double symbol: `@@`.

---

## 2. Code Blocks (`@{ ... }`)

Use `@{ ... }` to define a block of server-side C# code. Code inside code blocks executes during page processing but is not rendered directly to HTML output.

```cshtml
@{ 
    var welcomeText = "Welcome to Razor Pages!"; 
    int itemCount = Model.StaffMembers.Count;
}

<p>@welcomeText (Staff Count: @itemCount)</p>
```

---

## 3. Control Structures

### Conditional Statements (`@if`, `@else`)

```cshtml
@if (Model.StaffMembers.Count == 0) 
{ 
    <p>No staff records are currently available.</p> 
}
else if (Model.StaffMembers.Count == 1)
{
    <p>Only one staff member listed.</p>
}
else 
{ 
    <p>We have @Model.StaffMembers.Count staff members listed.</p> 
}
```

### Loops (`@foreach`, `@for`)

**Looping a PageModel Collection:**

```cshtml
<ul>
    @foreach (var staff in Model.StaffMembers) 
    { 
        <li>@staff.Name - @staff.Department</li> 
    }
</ul>
```

---

## 4. Razor Pages Tag Helpers

Razor Pages includes built-in Tag Helpers to enhance HTML elements with server-side behavior:

- `asp-page`: Generates page URL paths (e.g., `<a asp-page="/Staff/Index">Staff Directory</a>`).
- `asp-page-handler`: Targets specific POST/GET handler methods (e.g., `<form method="post" asp-page-handler="Save">`).
- `asp-for`: Binds form inputs to `PageModel` properties (`<input asp-for="Staff.Email" />`).

---

## 5. Best Practices

1. **Keep Views Focused on Rendering**: Perform data fetching and processing inside the code-behind `PageModel` (`.cshtml.cs`), leaving the `.cshtml` view focused strictly on HTML markup.
2. **Context Transitions**: Razor automatically detects HTML tags inside C# blocks (such as `@if`) and switches seamlessly between C# execution and HTML output.
