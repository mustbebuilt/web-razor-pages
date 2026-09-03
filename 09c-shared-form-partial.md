# Reusable Form Partials & Validation Tag Helpers

## Overview: The DRY Principle in Form Design

In web applications, the **Create** and **Edit** pages typically require the exact same set of form fields, labels, input formatting, and validation spans.

Duplicating this markup across both pages violates the **DRY (Don't Repeat Yourself)** principle and creates maintenance headaches whenever a new database field or validation rule is introduced.

In ASP.NET Core Razor Pages, you can extract the common form controls into a **Partial View** (`_StaffForm.cshtml`) and share it seamlessly across multiple pages.

---

## 1. Creating the Partial View (`Pages/CMS/_StaffForm.cshtml`)

By convention, partial views in ASP.NET Core begin with an underscore (`_`). The partial declares `@model StaffInput` as its strongly typed model:

```cshtml
@model MyRazorApp.Pages.CMS.StaffInput

@* Overall model validation errors *@
<div asp-validation-summary="ModelOnly" class="text-danger"></div>

<div class="form-row">
    <div class="form-field">
        <label asp-for="FirstName" class="form-label"></label>
        <input asp-for="FirstName" class="search-input full-width" />
        <span asp-validation-for="FirstName" class="text-danger validation-error"></span>
    </div>
    <div class="form-field">
        <label asp-for="LastName" class="form-label"></label>
        <input asp-for="LastName" class="search-input full-width" />
        <span asp-validation-for="LastName" class="text-danger validation-error"></span>
    </div>
</div>

<div class="form-field">
    <label asp-for="Email" class="form-label"></label>
    <input asp-for="Email" type="email" class="search-input full-width" />
    <span asp-validation-for="Email" class="text-danger validation-error"></span>
</div>

<div class="form-row">
    <div class="form-field">
        <label asp-for="Department" class="form-label"></label>
        <input asp-for="Department" class="search-input full-width" />
        <span asp-validation-for="Department" class="text-danger validation-error"></span>
    </div>
    <div class="form-field">
        <label asp-for="JobTitle" class="form-label"></label>
        <input asp-for="JobTitle" class="search-input full-width" />
        <span asp-validation-for="JobTitle" class="text-danger validation-error"></span>
    </div>
</div>

<div class="form-row">
    <div class="form-field">
        <label asp-for="HireDate" class="form-label"></label>
        <input asp-for="HireDate" type="date" class="search-input full-width" />
        <span asp-validation-for="HireDate" class="text-danger validation-error"></span>
    </div>
    <div class="form-field">
        <label asp-for="Salary" class="form-label"></label>
        <input asp-for="Salary" type="number" min="0" step="0.01" class="search-input full-width" />
        <span asp-validation-for="Salary" class="text-danger validation-error"></span>
    </div>
</div>

<div class="form-field">
    <label asp-for="IsActive" class="form-label"></label>
    <select asp-for="IsActive" class="search-input full-width">
        <option value="1">Active</option>
        <option value="0">Inactive</option>
    </select>
</div>
```

---

## 2. Razor Form Tag Helpers Explained

ASP.NET Core provides rich Tag Helpers that bind HTML elements directly to C# model properties:

### 1. Label Tag Helper (`<label asp-for="...">`)
- Generates the HTML `<label for="Input_FirstName">` attribute matching the input ID.
- Automatically reads the `[Display(Name = "First name")]` attribute from the model to render the text.

### 2. Input Tag Helper (`<input asp-for="...">`)
- Generates `name="Input.FirstName"`, `id="Input_FirstName"`, and `value="..."`.
- Automatically emits standard HTML5 validation attributes (such as `required`, `type="email"`, `type="date"`, etc.) derived from Data Annotations for browser-native client-side validation.
- Preserves user input when a form fails server-side validation.

### 3. Validation Message Tag Helper (`<span asp-validation-for="...">`)
- Emits a container that displays field-specific error messages if validation fails (e.g. *"The First name field is required."*).

### 4. Validation Summary Tag Helper (`<div asp-validation-summary="...">`)
- Displays errors that apply to the model as a whole rather than a specific individual field (`ModelOnly`), or displays all errors (`All`).

### 5. Select Tag Helper (`<select asp-for="...">`)
- Automatically marks the `<option>` matching the current model value as `selected`.

---

## 3. Embedding the Partial View

To render the shared form inside any parent page (such as `Create.cshtml` or `Edit.cshtml`), use the `<partial>` Tag Helper and pass the input model via the `model` attribute:

```cshtml
<form method="post">
    <!-- Embed the reusable form partial -->
    <partial name="_StaffForm" model="Model.Input" />

    <button type="submit" class="btn btn-primary">Save changes</button>
    <a asp-page="Index">Cancel</a>
</form>
```

---

## 4. Client-Side (HTML5) vs. Server-Side Validation

When forms are submitted, validation occurs in two layers:

```
 User fills form & clicks submit
               │
               ▼
   [HTML5 Client-Side Validation]  <-- Native browser validation (instant feedback, zero dependencies)
               │ (Passes)
               ▼
   [HTTP POST to Server]
               │
               ▼
   [Server-Side Validation]        <-- ModelState.IsValid (secure, authoritative verification)
```

1. **HTML5 Client-Side Validation (Browser)**: Modern browsers natively validate form inputs using HTML5 attributes (`required`, `type="email"`, `type="date"`, `min`, `step`). Razor Tag Helpers generate these attributes directly from C# Data Annotations, providing instant client-side feedback without requiring external JavaScript libraries or jQuery.
2. **Server-Side Validation (Backend)**: Never trust the client alone! Browser validation can be bypassed or disabled. The server always authoritatively re-evaluates all validation rules on `ModelState.IsValid` before updating the database.

> [!TIP]
> In the next tutorial, **`09d-crud-create.md`**, we will build the **Create** page to process new submissions and save records to the database.
