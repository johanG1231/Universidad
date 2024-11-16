using _123.Dtos;
using _123.Models;
using _123.Repositories;
using _123.Utilities;
using System;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using BCrypt.Net;


namespace _123.Services
{
    public interface IUsuarioService
    {
        Task<bool> RegistrarAsync(RegistroDto registroDto);
        Task<bool> ConfirmarEmailAsync(string token, string correo);
        Task<string> LoginAsync(LoginDto loginDto);
        Task<bool> SolicitarRestablecimientoAsync(SolicitarRestablecimientoDto solicitudDto);
        Task<bool> RestablecerClaveAsync(RestablecerClaveDto restablecerDto);
    }

    public class UsuarioService : IUsuarioService
    {
        private readonly IConfiguration _configuration;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IEmailService _emailService;
        private readonly ApplicationDbContext _context;

        public UsuarioService(IUsuarioRepository usuarioRepository, IEmailService emailService, ApplicationDbContext context, IConfiguration configuration)
        {
            _usuarioRepository = usuarioRepository;
            _emailService = emailService;
            _context = context;
            _configuration = configuration;
        }
        public static string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }
        /*public static string HashPassword(string password)
        {
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
            return hashedPassword;
        }*/

        public async Task<bool> RegistrarAsync(RegistroDto registroDto)
        {
            var usuarioExistente = await _usuarioRepository.ObtenerPorCorreoAsync(registroDto.Correo);
            if (usuarioExistente != null)
                return false;

            var hashedPassword = HashPassword(registroDto.Clave);

            var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

            var usuario = new Usuario
            {
                Nombre = registroDto.Nombre,
                Correo = registroDto.Correo,
                Clave = hashedPassword,
                Confirmado = false,
                Token = token
            };

            await _usuarioRepository.CrearAsync(usuario);

            var linkConfirmacion = $"http://localhost:5161/Usuario/ConfirmarEmail?correo={usuario.Correo}&token={Uri.EscapeDataString(usuario.Token)}";
            await _emailService.EnviarCorreoAsync(usuario.Correo, "Confirma tu cuenta", $"Confirma tu cuenta haciendo clic en <a href='{linkConfirmacion}'>este enlace</a>.");

            return true;
        }

        public async Task<bool> ConfirmarEmailAsync(string token, string correo)
        {
            // Buscar al usuario por correo
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Correo == correo);

            if (usuario == null)
            {
                return false; // Usuario no encontrado
            }

            // Verificar si el token es correcto
            if (usuario.Token != token)
            {
                return false; // Token incorrecto
            }

            // Actualizar la confirmación
            usuario.Confirmado = true;
            usuario.Token = null; // O eliminar el token una vez confirmado
            await _context.SaveChangesAsync();

            return true;
        }


        



        public async Task<string> LoginAsync(LoginDto loginDto) // Asegúrate de que este tipo de retorno sea Task<string>
        {
            // Verifica que el dto no sea nulo
            if (loginDto == null || string.IsNullOrEmpty(loginDto.Correo) || string.IsNullOrEmpty(loginDto.Clave))
            {
                throw new ArgumentException("Correo y clave son obligatorios.");
            }

            var usuario = await _usuarioRepository.ObtenerPorCorreoAsync(loginDto.Correo);

            // Verifica si el usuario existe
            if (usuario == null)
            {
                throw new UnauthorizedAccessException("El usuario no existe.");
            }

            // Hash de la clave proporcionada
            var hashedPassword = HashPassword(loginDto.Clave);

            // Verifica si el hash coincide
            if (usuario.Clave != hashedPassword)
            {
                throw new UnauthorizedAccessException("Credenciales incorrectas.");
            }

            // Generar y devolver el token de acceso
            var token = GenerateToken(usuario); // Genera el token
            return token; // Devuelve el token
        }

        private string GenerateToken(Usuario usuario)
        {
            var claims = new[]
            {
        new Claim(JwtRegisteredClaimNames.Sub, usuario.Correo),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim(ClaimTypes.Role, usuario.Rol.ToString()) // Asegúrate de que el rol se convierta a cadena correctamente
    };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }









        public async Task<bool> SolicitarRestablecimientoAsync(SolicitarRestablecimientoDto solicitudDto)
        {
            var usuario = await _usuarioRepository.ObtenerPorCorreoAsync(solicitudDto.Correo);
            if (usuario == null)
                return false;

            // Generar token de restablecimiento
            var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            usuario.Token = token;
            usuario.RestablecerTokenExpira = DateTime.UtcNow.AddHours(1);
            await _usuarioRepository.ActualizarAsync(usuario);

            // Enviar correo de restablecimiento
            var linkRestablecimiento = $"http://localhost:5161/Usuario/Login?correo={usuario.Correo}&token={usuario.Token}";

            await _emailService.EnviarCorreoAsync(usuario.Correo, "Restablece tu contraseña", $"Restablece tu contraseña haciendo clic en <a href='{linkRestablecimiento}'>este enlace</a>.");

            return true;
        }

        public async Task<bool> RestablecerClaveAsync(RestablecerClaveDto restablecerDto)
        {
            var usuario = await _usuarioRepository.ObtenerPorCorreoAsync(restablecerDto.Correo);
            if (usuario == null || usuario.Token != restablecerDto.Token || usuario.RestablecerTokenExpira < DateTime.UtcNow)
                return false;

            // Hash de la nueva contraseña
            var nuevaClaveHash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: restablecerDto.NuevaClave,
                salt: Encoding.UTF8.GetBytes("salt"), // Usa una sal segura
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            usuario.Clave = nuevaClaveHash;
            usuario.Token = null;
            usuario.RestablecerTokenExpira = null;
            await _usuarioRepository.ActualizarAsync(usuario);
            return true;
        }
    }
}
