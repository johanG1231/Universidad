using System;
using System.ComponentModel.DataAnnotations;

namespace _123.Models
{
    public enum RolUsuario
    {
        Regular,
        Administrador

    }
    public class Usuario
    {
        [Key]
        public int IdUsuario { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; }

        [Required]
        [EmailAddress]
        public string Correo { get; set; }

        [Required]
        public string Clave { get; set; }

        public bool Confirmado { get; set; }

        public string Token { get; set; }

        public DateTime? RestablecerTokenExpira { get; set; }

        public DateTime FechaCreacion { get; set; }

        public DateTime? FechaActualizacion { get; set; }
        public RolUsuario Rol { get; set; } = RolUsuario.Regular;
    }
}
