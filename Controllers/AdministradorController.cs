using _123.Services;
using _123.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt; // Para JwtSecurityTokenHandler
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
public class AdministradorController : Controller
{
    private readonly ICompraService _compraService;
    private readonly ILogger<AdministradorController> _logger;

    public AdministradorController(ICompraService compraService, ILogger<AdministradorController> logger)
    {
        _compraService = compraService;
        _logger = logger;
    }

    private bool UsuarioEsAdministrador()
    {
        var token = HttpContext.Session.GetString("Token"); // Verifica si el token está en la sesión
        if (string.IsNullOrEmpty(token))
        {
            return false; // Si no hay token, el usuario no es administrador
        }

        // Aquí deberías decodificar el token para obtener los claims y verificar el rol
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token); // Decodifica el token JWT

        // Verifica si el rol del usuario es "Administrador"
        var roleClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
        return roleClaim != null && roleClaim.Value == "Administrador"; // Retorna true si es administrador
    }

    [HttpGet]
    public IActionResult AgregarProducto()
    {
        if (!UsuarioEsAdministrador())
        {
            return RedirectToAction("AccesoDenegado"); // O la acción que desees
        }
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AgregarProducto(ProductoDto productoDto)
    {
        if (!UsuarioEsAdministrador())
        {
            return RedirectToAction("AccesoDenegado");
        }

        if (ModelState.IsValid)
        {
            await _compraService.AgregarProducto(productoDto);
            _logger.LogInformation($"Producto agregado: {productoDto.Nombre}, Contenido: {productoDto.ContenidoPersonalizado}");
            return RedirectToAction("Productos");
        }
        return View(productoDto);
    }


    [HttpGet]
    public async Task<IActionResult> Productos()
    {
        if (!UsuarioEsAdministrador())
        {
            return RedirectToAction("AccesoDenegado"); // O la acción que desees
        }

        var productos = await _compraService.ObtenerProductos();
        return View(productos);
    }

    public IActionResult AccesoDenegado()
    {
        return View(); // Crea una vista que informe al usuario que no tiene acceso
    }
    public async Task<IActionResult> DescargarProducto(int id)
    {
        var producto = await _compraService.ObtenerProductoPorId(id);

        if (producto == null || string.IsNullOrEmpty(producto.ContenidoPersonalizado))
        {
            return NotFound("Producto o contenido no encontrado");
        }

        // Obtener el contenido personalizado del producto
        string contenidoArchivo = producto.ContenidoPersonalizado;
        byte[] archivoBytes = System.Text.Encoding.UTF8.GetBytes(contenidoArchivo);

        // Configurar el nombre del archivo con el nombre del producto
        string nombreArchivo = $"{producto.Nombre}.bat";

        return File(archivoBytes, "text/plain", nombreArchivo);
    }

}

