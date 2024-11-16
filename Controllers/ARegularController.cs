using _123.Services;
using _123.Dtos;
using _123.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Threading.Tasks;

public class ARegularController : Controller
{
    private readonly ICompraService _compraService;
    private readonly ILogger<ARegularController> _logger;

    public ARegularController(ICompraService compraService, ILogger<ARegularController> logger)
    {
        _compraService = compraService;
        _logger = logger;
    }
    [HttpGet]
    public IActionResult Inicio()
    {
        if (!UsuarioEsRegular())
        {
            return RedirectToAction("AccesoDenegado");
        }
        var productoDto = new ProductoDto();
        return View(productoDto);
    }
    public IActionResult Privacy()
    {
        if (!UsuarioEsRegular())
        {
            return RedirectToAction("AccesoDenegado");
        }
        var productoDto = new ProductoDto();
        return View(productoDto);
    }
    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        return RedirectToAction("Inicio", "Usuario");
    }
    private bool UsuarioEsRegular()
    {
        var token = HttpContext.Session.GetString("Token");
        if (string.IsNullOrEmpty(token))
        {
            return false;
        }

        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        var roleClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
        return roleClaim != null && roleClaim.Value == "Regular";
    }

    [HttpGet]
    public async Task<IActionResult> Productos()
    {
        if (!UsuarioEsRegular())
        {
            return RedirectToAction("AccesoDenegado");
        }

        var productos = await _compraService.ObtenerProductos();
        return View(productos);
    }

    public IActionResult AccesoDenegado()
    {
        return View();
    }
    public async Task<IActionResult> DescargarProducto(int id)
    {
        if (!UsuarioEsRegular())
        {
            return RedirectToAction("AccesoDenegado");
        }

        var producto = await _compraService.ObtenerProductoPorId(id);

        if (producto == null)
        {
            return NotFound("Producto no encontrado");
        }

        string contenidoBat = $"{producto.Archivo}\n";
        byte[] archivoBytes = System.Text.Encoding.UTF8.GetBytes(contenidoBat);

        string nombreArchivo = $"{producto.Nombre.Replace(" ", "_")}.bat";

        return File(archivoBytes, "application/octet-stream", nombreArchivo);
    }

}

