// Repositories/ProductoRepository.cs
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
                .Select(p => new ProductoDto { Id = p.Id, Nombre = p.Nombre, Precio = p.Precio })
                .FirstOrDefaultAsync();
        }
        public async Task AgregarProducto(Producto producto)
        {
            // Asegúrate de que el modelo Producto esté correctamente definido
            await _context.Productos.AddAsync(producto);
            await _context.SaveChangesAsync(); // Guarda los cambios en la base de datos
        }
    }
}