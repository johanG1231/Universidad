// Services/CompraService.cs
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _123.Dtos;
using _123.Models;
using _123.Services;
using _123.Repositories;

namespace _123.Services
{
    public class CompraService : ICompraService
    {
        private readonly IProductoRepository _productoRepository;

        public CompraService(IProductoRepository productoRepository)
        {
            _productoRepository = productoRepository;
        }

        public async Task<IEnumerable<ProductoDto>> ObtenerProductos()
        {
            var productos = await _productoRepository.ObtenerProductosAsync();
            return productos.Select(p => new ProductoDto
            {
                Id = p.Id,
                Nombre = p.Nombre,
                Precio = p.Precio,
                Descripcion = p.Descripcion,
                Archivo = p.Archivo
            });
        }
        public async Task<ProductoDto> ObtenerProductoPorId(int id)
        {
            return await _productoRepository.ObtenerProductoPorId(id);
        }
        public async Task AgregarProducto(ProductoDto productoDto)
        {
            var producto = new Producto
            {
                Nombre = productoDto.Nombre,
                Precio = productoDto.Precio,
                Descripcion = productoDto.Descripcion,
                Archivo = productoDto.Archivo,
            };

            await _productoRepository.AgregarProducto(producto);
        }
        public async Task EliminarProducto(int id)
        {
            await _productoRepository.EliminarProducto(id);
        }
        public async Task<List<ProductoDto>> ObtenerTodosLosProductos()
        {
            var productos = await _productoRepository.ObtenerTodosLosProductos();
            return productos.Select(p => new ProductoDto
            {
                Id = p.Id,
                Nombre = p.Nombre,
                Precio = p.Precio,
                Descripcion = p.Descripcion,
                Archivo = p.Archivo
            }).ToList();
        }

    }
}