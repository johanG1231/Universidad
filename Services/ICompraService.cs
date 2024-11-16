using System.Collections.Generic;
using System.Threading.Tasks;
using _123.Dtos;
using _123.Models;

namespace _123.Services
{
    public interface ICompraService
    {
        Task<IEnumerable<ProductoDto>> ObtenerProductos();
        Task<ProductoDto> ObtenerProductoPorId(int id);
        Task AgregarProducto(ProductoDto productoDto);
        Task EliminarProducto(int id);
        Task<List<ProductoDto>> ObtenerTodosLosProductos();
        

    }
}