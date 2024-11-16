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
using System.IO;
using System.Text;
using System.Threading.Tasks;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;

public class ManagerController : Controller
{
    private readonly ICompraService _compraService;
    private readonly ILogger<ManagerController> _logger;

    public ManagerController(ICompraService compraService, ILogger<ManagerController> logger)
    {
        _compraService = compraService;
        _logger = logger;
    }
    [HttpGet]
    public IActionResult Inicio()
    {
        if (!UsuarioEsManager())
        {
            return RedirectToAction("AccesoDenegado");
        }
        var productoDto = new ProductoDto();
        return View(productoDto);
    }
    public IActionResult Privacy()
    {
        if (!UsuarioEsManager())
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
    private bool UsuarioEsManager()
    {
        var token = HttpContext.Session.GetString("Token");
        if (string.IsNullOrEmpty(token))
        {
            return false;
        }

        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        var roleClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
        return roleClaim != null && roleClaim.Value == "Manager";
    }


    [HttpGet]
    public async Task<IActionResult> Productos()
    {
        if (!UsuarioEsManager())
        {
            return RedirectToAction("AccesoDenegado");
        }

        var productos = await _compraService.ObtenerProductos();
        return View(productos); 
    }
    public IActionResult Estadisticas()
    {
        if (!UsuarioEsManager())
        {
            return RedirectToAction("AccesoDenegado");
        }
        var productoDto = new ProductoDto();
        return View(productoDto);
    }
    public IActionResult AccesoDenegado()
    {
        return View();
    }
    public async Task<IActionResult> DescargarProductosPdf()
    {
        if (!UsuarioEsManager())
        {
            return RedirectToAction("AccesoDenegado");
        }

        // Obtén todos los productos desde tu servicio
        var productos = await _compraService.ObtenerTodosLosProductos();

        if (productos == null || !productos.Any())
        {
            return NotFound("No hay productos para generar el PDF.");
        }

        // Crear un stream de memoria para almacenar el PDF
        using (var memoryStream = new MemoryStream())
        {
            // Crear el documento PDF
            var pdfWriter = new PdfWriter(memoryStream);
            var pdf = new PdfDocument(pdfWriter);
            var document = new Document(pdf);

            // Agregar título al PDF
            document.Add(new Paragraph("Lista de Productos").SetFontSize(18).SetBold());

            // Crear una tabla para los productos
            var table = new Table(3); // 3 columnas: Nombre, Precio, Descripción
            table.AddHeaderCell("Nombre");
            table.AddHeaderCell("Precio");
            table.AddHeaderCell("Descripción");

            // Agregar cada producto a la tabla
            foreach (var producto in productos)
            {
                table.AddCell(producto.Nombre);
                table.AddCell($"{producto.Precio:C}"); // Formato de precio
                table.AddCell(producto.Descripcion);
            }

            document.Add(table);
            document.Close();

            // Preparar el PDF para descarga
            var nombreArchivo = "Lista_de_Productos.pdf";
            var contentType = "application/pdf";
            byte[] archivoBytes = memoryStream.ToArray();

            return File(archivoBytes, contentType, nombreArchivo);
        }
    }


}

