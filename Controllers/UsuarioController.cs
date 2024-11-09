using Microsoft.AspNetCore.Mvc;
using _123.Dtos;
using _123.Services;
using _123.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace _123.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly UsuarioService _usuarioService;
        private readonly ILogger<UsuarioController> _logger;
        private readonly ApplicationDbContext _context;

        public UsuarioController(ILogger<UsuarioController> logger, UsuarioService usuarioService, ApplicationDbContext context)
        {
            _usuarioService = usuarioService;
            _logger = logger;
            _context = context;
        }

        // GET: Usuario/Register
        public IActionResult Register()
        {
            return View();
        }

        public IActionResult Productos()
        {
            return View();
        }


        public IActionResult VistaEspecifica()
        {
            var token = HttpContext.Session.GetString("Token");
            if (token == null)
            {
                return RedirectToAction("Login"); // Redirige si no está logueado
            }
            return View(); // Muestra la vista específica si está logueado
        }


        // POST: Usuario/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegistroDto registroDto)
        {
            if (ModelState.IsValid)
            {
                // Verificar si las contraseñas coinciden
                if (registroDto.Clave != registroDto.ConfirmarClave)
                {
                    ModelState.AddModelError("", "Las contraseñas no coinciden.");
                    return View(registroDto);
                }

                // Intentar registrar al usuario
                var result = await _usuarioService.RegistrarAsync(registroDto);
                if (result)
                {
                    // En lugar de redirigir, solo muestra un mensaje de éxito en la misma página
                    ViewBag.MensajeExito = "Registro exitoso. Verifica tu correo para confirmar tu cuenta.";
                    return View(registroDto); // Permanecer en la misma página
                }
                else
                {
                    ModelState.AddModelError("", "El correo ya está registrado.");
                }
            }

            // Si el modelo no es válido o hubo algún error, regresar a la vista con el modelo
            return View(registroDto);
        }



        // GET: Usuario/ConfirmarEmail
        public IActionResult ConfirmarEmail()
        {
            return View();
        }

        // GET: Usuario/ConfirmarEmail?correo=...&token=...
        [HttpGet]
        public async Task<IActionResult> ConfirmarEmail(string correo, string token)
        {
            if (string.IsNullOrEmpty(correo) || string.IsNullOrEmpty(token))
            {
                return View("ConfirmacionExitosa", false); // Error, faltan parámetros
            }

            // Buscar al usuario por correo
            var usuario = await _context.Usuarios.SingleOrDefaultAsync(u => u.Correo == correo);

            if (usuario == null)
            {
                return View("ConfirmacionExitosa", false); // Error, usuario no encontrado
            }

            // Decodificar el token recibido de la URL
            string decodedToken = Uri.UnescapeDataString(token);

            // Comparar el token con el de la base de datos
            if (usuario.Token != decodedToken)
            {
                return View("ConfirmacionExitosa", false); // Error, token no coincide
            }

            // Confirmar el usuario
            usuario.Confirmado = true;
            usuario.Token = null; // Limpiar el token después de confirmarlo

            try
            {
                _context.Update(usuario); // Actualizar los cambios
                await _context.SaveChangesAsync(); // Guardar en la base de datos
                return View("ConfirmacionExitosa", true); // Confirmación exitosa
            }
            catch (Exception)
            {
                return View("ConfirmacionExitosa", false); // Error al guardar los cambios
            }
        }

        // GET: Usuario/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: Usuario/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            _logger.LogInformation("Intentando iniciar sesión con el correo: {Correo}", loginDto.Correo);

            if (ModelState.IsValid)
            {
                var result = await _usuarioService.LoginAsync(loginDto);

                if (!string.IsNullOrEmpty(result))
                {
                    HttpContext.Session.SetString("Token", result);
                    Console.WriteLine($"Token guardado en la sesión: {result}");

                    // Obtén el usuario para verificar su rol
                    var usuario = await _context.Usuarios
                        .FirstOrDefaultAsync(u => u.Correo == loginDto.Correo);

                    if (usuario != null)
                    {
                        // Redirige según el rol del usuario
                        if (usuario.Rol == RolUsuario.Administrador)
                        {
                            return RedirectToAction("AgregarProducto", "Administrador");
                        }
                        else if (usuario.Rol == RolUsuario.Regular)
                        {
                            return RedirectToAction("Productos", "Producto");
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Credenciales inválidas.");
                }
            }

            return View(loginDto);
        }












        // GET: Usuario/ForgotPassword
        public IActionResult ForgotPassword()
        {
            return View();
        }

        // POST: Usuario/ForgotPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(SolicitarRestablecimientoDto solicitudDto)
        {
            if (ModelState.IsValid)
            {
                var result = await _usuarioService.SolicitarRestablecimientoAsync(solicitudDto);
                if (result)
                {
                    return RedirectToAction("ForgotPasswordConfirmation", "Usuario");
                }
                ModelState.AddModelError("", "El correo no está registrado.");
            }
            return View(solicitudDto);
        }

        // GET: Usuario/ForgotPasswordConfirmation
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        // GET: Usuario/ResetPassword
        public IActionResult ResetPassword(string correo, string token)
        {
            var model = new RestablecerClaveDto
            {
                Correo = correo,
                Token = token
            };
            return View(model);
        }

        // POST: Usuario/ResetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(RestablecerClaveDto restablecerDto)
        {
            if (ModelState.IsValid)
            {
                if (restablecerDto.NuevaClave != restablecerDto.ConfirmarNuevaClave)
                {
                    ModelState.AddModelError("", "Las contraseñas no coinciden.");
                    return View(restablecerDto);
                }

                var result = await _usuarioService.RestablecerClaveAsync(restablecerDto);
                if (result)
                {
                    return RedirectToAction("ResetPasswordConfirmation", "Usuario");
                }
                ModelState.AddModelError("", "Token inválido o expirado.");
            }
            return View(restablecerDto);
        }

        // GET: Usuario/ResetPasswordConfirmation
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }
    }
}
