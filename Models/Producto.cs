using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _123.Models
{
    [Table("Productos")] // Nombre de la tabla en la base de datos
    public class Producto
    {
        [Key]
        public int Id { get; set; } // Llave primaria
        public string Nombre { get; set; } // Nombre del producto
        public decimal Precio { get; set; } // Precio del producto
        public string? ContenidoPersonalizado { get; set; }
    }
}
