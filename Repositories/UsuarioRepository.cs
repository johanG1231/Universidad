using _123.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;

namespace _123.Repositories
{
    public interface IUsuarioRepository
    {
        Task<Usuario> ObtenerPorCorreoAsync(string correo);
        Task CrearAsync(Usuario usuario);
        Task ActualizarAsync(Usuario usuario);
    }

    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly ApplicationDbContext _context;

        public UsuarioRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task CrearAsync(Usuario usuario)
        {
            await _context.Usuarios.AddAsync(usuario);
            await _context.SaveChangesAsync();
        }

        public async Task<Usuario> ObtenerPorCorreoAsync(string correo)
        {
            
            // Verificar que el correo no sea nulo o vacío
            if (string.IsNullOrEmpty(correo))
            {
                throw new ArgumentException("El correo no puede ser nulo o vacío.", nameof(correo));
            }

            // Buscar el usuario por correo
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Correo == correo);

            // Retornar el usuario encontrado o null si no existe
            return usuario;
        }

        // Otros métodos del repositorio...



        public async Task ActualizarAsync(Usuario usuario)
        {
            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();
        }
    }
}
