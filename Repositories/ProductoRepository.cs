using System.Collections.Generic;
using System.Threading.Tasks;
using _123.Models;
using _123.Dtos;
using _123.Services;
using Microsoft.EntityFrameworkCore;

namespace _123.Repositories
{
    public class ProductoRepository : IProductoRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Producto>> ObtenerProductosAsync()
        {
            return await _context.Productos.ToListAsync();
        }
        public async Task<ProductoDto> ObtenerProductoPorId(int id)
        {
            return await _context.Productos
                .Where(p => p.Id == id)
                .Select(p => new ProductoDto { Id = p.Id, Nombre = p.Nombre, Precio = p.Precio, Descripcion = p.Descripcion, Archivo = p.Archivo })
                .FirstOrDefaultAsync();
        }
        public async Task AgregarProducto(Producto producto)
        {
            await _context.Productos.AddAsync(producto);
            await _context.SaveChangesAsync();
        }

        public async Task EliminarProducto(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto != null)
            {
                _context.Productos.Remove(producto);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Producto>> ObtenerTodosLosProductos()
        {
            return await _context.Productos.ToListAsync();
        }
    }
}