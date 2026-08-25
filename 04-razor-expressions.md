# Razor Expressions

We previously added a `_Layout.cshtml` file but it only displayed the text "Today's Date" in the footer. We can use Razor expressions to display dynamic content in the footer. 

In a Razor view template, use the `@` symbol to transition from HTML to C# code. 

```cshtml
    <footer>
        <p>&copy; @DateTime.Now.Year MyRazorApp - @DateTime.Now.ToString("dd MMM yyyy")</p>
    </footer>
```

The `DateTime.Now.Year` is a Razor expression that displays the current year. The `@` symbol is used to transition from HTML to C# code. 

We will see other examples of razor expressions later in the tutorial such as loops and condition for handling data collections and displaying them in the browser.
