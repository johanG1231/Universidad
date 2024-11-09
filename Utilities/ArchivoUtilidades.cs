using System.IO;
using _123.Dtos;

namespace _123.Utilities
{
    public static class ArchivoUtilidades
    {
        public static void GenerarArchivoProducto(ProductoDto producto)
        {
            string filePath = Path.Combine("wwwroot", $"{producto.Nombre}_detalles.txt");

            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine("Detalles del producto:");
                writer.WriteLine($"ID: {producto.Id}");
                writer.WriteLine($"Nombre: {producto.Nombre}");
                writer.WriteLine($"Precio: ${producto.Precio}");
            }
        }
    }
}
