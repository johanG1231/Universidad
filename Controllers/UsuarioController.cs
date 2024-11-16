using Microsoft.AspNetCore.Mvc;
using _123.Dtos;
using _123.Services;
using _123.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Threading.Tasks;

namespace _123.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly UsuarioService _usuarioService;
        private readonly ILogger<UsuarioController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly ICompraService _compraService;

        public UsuarioController(ICompraService compraService, ILogger<UsuarioController> logger, UsuarioService usuarioService, ApplicationDbContext context)
        {
            _usuarioService = usuarioService;
            _logger = logger;
            _context = context;
            _compraService = compraService;
        }
        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult Ingresa()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Inicio", "Usuario");
        }
        public IActionResult Register()
        {
            return View();
        }
        public IActionResult Inicio()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Productos()
        {
            var productos = await _compraService.ObtenerProductos();
            return View(productos);
        }

        public IActionResult VistaEspecifica()
        {
            var token = HttpContext.Session.GetString("Token");
            if (token == null)
            {
                return RedirectToAction("Login");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegistroDto registroDto)
        {
            if (ModelState.IsValid)
            {
                if (registroDto.Clave != registroDto.ConfirmarClave)
                {
                    ModelState.AddModelError("", "Las contraseñas no coinciden.");
                    return View(registroDto);
                }
                
                var result = await _usuarioService.RegistrarAsync(registroDto);
                if (result)
                {
                    ViewBag.MensajeExito = "Registro exitoso. Verifica tu correo para confirmar tu cuenta.";
                    return View(registroDto);
                }
                else
                {
                    ModelState.AddModelError("", "El correo ya está registrado.");
                }
            }
            return View(registroDto);
        }

        public IActionResult ConfirmarEmail()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmarEmail(string correo, string token)
        {
            if (string.IsNullOrEmpty(correo) || string.IsNullOrEmpty(token))
            {
                return View("ConfirmacionExitosa", false);
            }

            var usuario = await _context.Usuarios.SingleOrDefaultAsync(u => u.Correo == correo);

            if (usuario == null)
            {
                return View("ConfirmacionExitosa", false);
            }

            string decodedToken = Uri.UnescapeDataString(token);

            if (usuario.Token != decodedToken)
            {
                return View("ConfirmacionExitosa", false);
            }

            usuario.Confirmado = true;
            usuario.Token = null;

            try
            {
                _context.Update(usuario);
                await _context.SaveChangesAsync();
                return View("ConfirmacionExitosa", true);
            }
            catch (Exception)
            {
                return View("ConfirmacionExitosa", false);
            }
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {

            _logger.LogInformation("Intentando iniciar sesión con el correo: {Correo}", loginDto.Correo);

            if (ModelState.IsValid)
            {
                try
                {
                    var result = await _usuarioService.LoginAsync(loginDto);

                    if (!string.IsNullOrEmpty(result))
                    {
                        HttpContext.Session.SetString("Token", result);
                        Console.WriteLine($"Token guardado en la sesión: {result}");

                        var usuario = await _context.Usuarios
                            .FirstOrDefaultAsync(u => u.Correo == loginDto.Correo);

                        if (usuario != null)
                        {
                            if (usuario.Rol == RolUsuario.Administrador)
                            {
                                return RedirectToAction("Inicio", "Administrador");
                            }
                            else if (usuario.Rol == RolUsuario.Regular)
                            {
                                return RedirectToAction("Inicio", "ARegular");
                            }
                            else if (usuario.Rol == RolUsuario.Manager)
                            {
                                return RedirectToAction("Inicio", "Manager");
                            }
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Credenciales inválidas.");
                    }
                }
                catch (UnauthorizedAccessException ex)
                {
                    _logger.LogError("Error de autenticación: {Message}", ex.Message);
                    return RedirectToAction("Error", "Home"); 
                }
                catch (Exception ex)
                {
                    _logger.LogError("Error inesperado: {Message}", ex.Message);
                    return RedirectToAction("Error", "Home");
                }
            }

            return View(loginDto);
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }

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

        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        public IActionResult ResetPassword(string correo, string token)
        {
            var model = new RestablecerClaveDto
            {
                Correo = correo,
                Token = token
            };
            return View(model);
        }

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

        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }
    }
}
