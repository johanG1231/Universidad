using System.Collections.Generic;
using System.Threading.Tasks;
using _123.Dtos;

namespace _123.Services
{
    public interface ICompraService
    {
        Task<IEnumerable<ProductoDto>> ObtenerProductos();
        Task<ProductoDto> ObtenerProductoPorId(int id);
        Task AgregarProducto(ProductoDto productoDto);
    }
}