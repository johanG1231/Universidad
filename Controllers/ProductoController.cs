// Controllers/ProductoController.cs
using _123.Dtos;
using _123.Models;
using _123.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace _123.Controllers
{
    public class ProductoController : Controller
    {
        private readonly ICompraService _compraService;

        public ProductoController(ICompraService compraService)
        {
            _compraService = compraService;
        }
        [HttpGet]
        public async Task<IActionResult> Productos()
        {
            var productos = await _compraService.ObtenerProductos();
            return View(productos);
        }

        // Método de acción para generar y descargar el archivo de producto
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
}
