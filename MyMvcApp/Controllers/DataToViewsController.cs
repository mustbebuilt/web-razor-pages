using Microsoft.AspNetCore.Mvc;
using MyMvcApp.ViewModels;

namespace MyMvcApp.Controllers;

// /DataToViews/ViewModel
// /DataToViews/ViewData
// /DataToViews/ViewBag
// /DataToViews/TempData

public class DataToViewsController : Controller
{
    public IActionResult ViewModel()
    {
        var model = new ProductListViewModel
        {
            PageTitle = "Products",
            Products =
            [
                new() { Id = 1, Name = "Laptop", Price = 999.99m },
            new() { Id = 2, Name = "Monitor", Price = 249.99m },
            new() { Id = 3, Name = "Keyboard", Price = 79.99m }
            ]
        };

        return View(model);
    }
    public IActionResult ViewDataDemo()
    {
        ViewData["Title"] = "ViewData Example";
        ViewData["ProductCount"] = 3;
        ViewData["Items"] = new List<ProductViewModel>
        {
            new() { Id = 1, Name = "Laptop", Price = 999.99m },
            new() { Id = 2, Name = "Monitor", Price = 249.99m },
            new() { Id = 3, Name = "Keyboard", Price = 79.99m }
        }
        ;

        return View();
    }

    public IActionResult TempDataDemo()
    {
        ViewData["Title"] = "TempData Example";

        return View();
    }

    [HttpPost]
    public IActionResult SetTempDataMessage()
    {
        TempData["StatusMessage"] = $"Product saved successfully at {DateTime.Now:t}.";

        return RedirectToAction(nameof(TempDataDemo));
    }

    public IActionResult ViewBagDemo()
    {
        ViewBag.Title = "ViewBag Example";
        ViewBag.ProductCount = 3;
        ViewBag.Items = new List<ProductViewModel>
        {
            new() { Id = 1, Name = "Laptop", Price = 999.99m },
            new() { Id = 2, Name = "Monitor", Price = 249.99m },
            new() { Id = 3, Name = "Keyboard", Price = 79.99m }
        }
        ;

        return View();
    }
}