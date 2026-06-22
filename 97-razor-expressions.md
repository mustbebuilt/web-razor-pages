# Razor Syntax Quick Reference Guide

Razor is a markup syntax that lets you embed server-side C# code into webpages alongside standard HTML markup. 

## 1. Transitions and Expressions

### Implicit Razor Expressions
Use the `@` symbol to transition directly from HTML to inline C#. Razor evaluates the expression and renders the output into the HTML.
* **Example:** `<p>Current time is: @DateTime.Now</p>`
* **Formatted Example:** `<p>Formatted Date: @DateTime.Now.ToString("dd MMM yyyy")</p>`

### Explicit Razor Expressions
Wrap an expression in parenthesis `@(...)` when you need to calculate a value or concatenate text before rendering. Ensure there is no trailing whitespace inside the parenthesis.
* **Example:** `<p>One week ago was: @(DateTime.Now - TimeSpan.FromDays(7))</p>`

### Escaping the `@` Symbol
* Razor treats email addresses normally without transitioning to code (e.g., `info@example.com`).
* To print a literal `@` symbol elsewhere on screen, escape it by using a double symbol: `@@`.

---

## 2. Code Blocks

Use `@{ ... }` to define a block of server-side C# code. Code executed within this block is processed on the server but is **not** rendered to the HTML output directly. It is primarily used for defining metadata, setting layout files, or declaring local variables.

```html
@{ 
    var welcomeText = "Hello World!"; 
    int productCount = 5;
}

<!-- Use variables later using implicit expressions -->
<p>@welcomeText</p>
```

---

## 3. Control Structures

### Conditional Statements
You can control what HTML elements render dynamically using standard C# logic blocks (`@if`, `else if`, `else`, and `@switch`).

```html
@if (Model.Products.Count == 0) 
{ 
    <p>No products are currently available.</p> 
}
else if (Model.Products.Count == 1)
{
    <p>Hurry! Only one item left in stock!</p>
}
else 
{ 
    <p>We have plenty of stock available.</p> 
}
```

### Loops
Razor handles regular C# looping mechanisms (`@for`, `@foreach`, `@while`, and `@do while`) to generate repetitive HTML elements, such as list items or table rows.

**Looping an Array:**
```html
@{ 
    string[] cities = ["Mumbai", "London", "New York", "Sheffield", "Manchester"]; 
} 

@foreach (var city in cities) 
{ 
    <p>City Name: @city</p> 
}
```

**Looping a Model Collection:**
When looping properties bound to a Strongly Typed Model, treat them exactly like standard C# objects.
```html
@foreach (var item in Model.Articles) 
{ 
    <h2>@item.Title</h2>
    <p>@item.Content</p> 
}
```

---

## 4. Best Practices Summary
1. **Separation of Concerns:** While you *can* declare lists or fetch data directly inside a view's code block, it is best practice to pass clean data models directly from the Controller.
2. **Context Transitions:** You only need the `@` symbol to start a structural block (like an `@if` or `@foreach`). Inside the curly brackets `{ }`, Razor automatically knows when it encounters an HTML tag (like `<p>`) and gracefully switches back to frontend rendering mode.
