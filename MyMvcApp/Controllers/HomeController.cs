using System.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using MyMvcApp.Models;

namespace MyMvcApp.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult News()

     {

            return View();

        }

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

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
