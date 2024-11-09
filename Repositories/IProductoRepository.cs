// Repositories/IProductoRepository.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using _123.Models;
using _123.Dtos;

namespace _123.Repositories
{
    public interface IProductoRepository
    {
        Task<IEnumerable<Producto>> ObtenerProductosAsync();
        Task<ProductoDto> ObtenerProductoPorId(int id);
        Task AgregarProducto(Producto producto);
    }
}