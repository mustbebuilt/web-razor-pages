using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyMvcApp.Models;

[Table("staff")]
public class Staff
{
    [Key]
    [Column("staff_id")]
    public int StaffId { get; set; }

    [Required(ErrorMessage = "First name is required.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "First name must be between 2 and 50 characters.")]
    [Column("first_name")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last name is required.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Last name must be between 2 and 50 characters.")]
    [Column("last_name")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email address is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address format.")]
    [Column("email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Department is required.")]
    [StringLength(50)]
    [Column("department")]
    public string Department { get; set; } = string.Empty;

    [Required(ErrorMessage = "Job title is required.")]
    [StringLength(50)]
    [Column("job_title")]
    public string JobTitle { get; set; } = string.Empty;

    [Column("hire_date")]
    public string HireDate { get; set; } = DateTime.Now.ToString("yyyy-MM-dd");

    [Required(ErrorMessage = "Salary is required.")]
    [Range(10000, 500000, ErrorMessage = "Salary must be between £10,000 and £500,000.")]
    [Column("salary")]
    public double Salary { get; set; }

    [Column("is_active")]
    public int IsActive { get; set; } = 1; // 0 or 1 matching SQLite integer constraint
}
