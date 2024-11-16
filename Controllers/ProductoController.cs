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


    }
}
