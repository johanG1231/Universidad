using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using _123.Models;

namespace _123.Controllers;

public class RegistradoController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public RegistradoController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult InicioRegistrado()
    {
        return View();
    }

    public IActionResult Productos()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
