using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _123.Dtos
{
    [Table("Productos")] // Asegúrate de que este es el nombre de la tabla en la base de datos
    public class ProductoDto
    {
        [Key] // Esto indica que esta propiedad es la clave primaria
        public int Id { get; set; } // Propiedad para el ID del producto

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
        public string Nombre { get; set; } // Propiedad para el nombre del producto

        [Required(ErrorMessage = "El precio es obligatorio.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor que cero.")]
        public decimal Precio { get; set; } // Propiedad para el precio del producto
        public string ContenidoPersonalizado  { get; set; }
    }
}
