using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MyRazorApp.Pages.HttpDemo;

public class PostDemoModel : PageModel
{
    [BindProperty]
    public FeedbackInput Input { get; set; } = new();

    // TempData flag to support Post-Redirect-Get (PRG) pattern demonstration
    [TempData]
    public string? FlashMessage { get; set; }

    [TempData]
    public string? SubmittedStudentName { get; set; }

    [TempData]
    public string? SubmittedCategory { get; set; }

    [TempData]
    public string? SubmittedFeedback { get; set; }

    public void OnGet()
    {
        // OnGet handles displaying the page or the redirected success message after PRG
    }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
        {
            return Page(); // Return page with validation errors if invalid
        }

        // Store feedback details in TempData across redirect (Post-Redirect-Get pattern)
        FlashMessage = "Form successfully submitted via HTTP POST!";
        SubmittedStudentName = Input.StudentName;
        SubmittedCategory = Input.Category;
        SubmittedFeedback = Input.Comments;

        // Post-Redirect-Get (PRG): Redirect back to GET page to prevent double-submit on browser refresh
        return RedirectToPage("/HttpDemo/PostDemo");
    }
}

public class FeedbackInput
{
    [Required(ErrorMessage = "Please enter your student name.")]
    [Display(Name = "Student Name")]
    public string StudentName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please enter your email address.")]
    [EmailAddress(ErrorMessage = "Invalid email address format.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please select a feedback category.")]
    public string Category { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please enter your comments.")]
    [StringLength(500, MinimumLength = 10, ErrorMessage = "Comments must be between 10 and 500 characters.")]
    public string Comments { get; set; } = string.Empty;
}
