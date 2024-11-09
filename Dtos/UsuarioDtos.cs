using System.ComponentModel.DataAnnotations;

namespace _123.Dtos
{
    public class RegistroDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El correo es obligatorio.")]
        [EmailAddress(ErrorMessage = "El formato del correo no es válido.")]
        public string Correo { get; set; }

        [Required(ErrorMessage = "La clave es obligatoria.")]
        [StringLength(256, MinimumLength = 6, ErrorMessage = "La clave debe tener entre 6 y 256 caracteres.")]
        public string Clave { get; set; }

        [Compare("Clave", ErrorMessage = "Las claves no coinciden.")]
        public string ConfirmarClave { get; set; }
    }

    public class LoginDto
    {
        [Required(ErrorMessage = "El correo es obligatorio.")]
        [EmailAddress(ErrorMessage = "El formato del correo no es válido.")]
        public string Correo { get; set; }

        [Required(ErrorMessage = "La clave es obligatoria.")]
        public string Clave { get; set; }
    }

    public class LoginResultDto
    {
        public string Token { get; set; }
        public int Rol { get; set; }
    }
    public class SolicitarRestablecimientoDto
    {
        [Required(ErrorMessage = "El correo es obligatorio.")]
        [EmailAddress(ErrorMessage = "El formato del correo no es válido.")]
        public string Correo { get; set; }
    }

    public class RestablecerClaveDto
    {
        [Required(ErrorMessage = "El correo es obligatorio.")]
        [EmailAddress(ErrorMessage = "El formato del correo no es válido.")]
        public string Correo { get; set; }

        [Required(ErrorMessage = "El token es obligatorio.")]
        public string Token { get; set; }

        [Required(ErrorMessage = "La nueva clave es obligatoria.")]
        [StringLength(256, MinimumLength = 6, ErrorMessage = "La nueva clave debe tener entre 6 y 256 caracteres.")]
        public string NuevaClave { get; set; }

        [Compare("NuevaClave", ErrorMessage = "Las nuevas claves no coinciden.")]
        public string ConfirmarNuevaClave { get; set; }
    }
    public class UsuarioDto
    {
        [Required]
        public string Token { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "El formato del correo no es válido.")]
        public string Correo { get; set; }

        [Required]
        public string Rol { get; set; } // Asegúrate de que esto refleje el rol del usuario
    }
}
