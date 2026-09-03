using System.ComponentModel.DataAnnotations;
using MyRazorApp.Models;

namespace MyRazorApp.Pages.CMS;

public class StaffInput
{
    public StaffInput() { }

    public StaffInput(Models.Staff staff)
    {
        FirstName = staff.FirstName;
        LastName = staff.LastName;
        Email = staff.Email;
        Department = staff.Department;
        JobTitle = staff.JobTitle;
        HireDate = staff.HireDate;
        Salary = staff.Salary;
        IsActive = staff.IsActive;
    }

    [Required]
    [Display(Name = "First name")]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Last name")]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Department { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Job title")]
    public string JobTitle { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Date)]
    [Display(Name = "Hire date")]
    public string HireDate { get; set; } = string.Empty;

    [Range(0, double.MaxValue)]
    public double Salary { get; set; }

    [Display(Name = "Status")]
    public int IsActive { get; set; } = 1;

    public Models.Staff ToStaff() => new()
    {
        FirstName = FirstName.Trim(),
        LastName = LastName.Trim(),
        Email = Email.Trim(),
        Department = Department.Trim(),
        JobTitle = JobTitle.Trim(),
        HireDate = HireDate.Trim(),
        Salary = Salary,
        IsActive = IsActive == 1 ? 1 : 0
    };

    public void ApplyTo(Models.Staff staff)
    {
        staff.FirstName = FirstName.Trim();
        staff.LastName = LastName.Trim();
        staff.Email = Email.Trim();
        staff.Department = Department.Trim();
        staff.JobTitle = JobTitle.Trim();
        staff.HireDate = HireDate.Trim();
        staff.Salary = Salary;
        staff.IsActive = IsActive == 1 ? 1 : 0;
    }
}