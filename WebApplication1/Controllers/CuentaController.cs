using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Entidades;
using WebApplication1.Models.ViewModels;

namespace WebApplication1.Controllers
{
    public class CuentaController : Controller
    {
        private readonly MusicTradeDbContext _context;
        private readonly PasswordService _passwordService;

        // Provincia por defecto hasta que se agregue el selector en el formulario de registro.
        // Buenos Aires (Id = 1) ya viene cargada por el seed de ProvinciasSeed.
        private const int ProvinciaPorDefectoId = 1;

        public CuentaController(MusicTradeDbContext context, PasswordService passwordService)
        {
            _context = context;
            _passwordService = passwordService;
        }

        // GET: /Cuenta/Registro
        [HttpGet]
        public IActionResult Registro()
        {
            return View(new RegistroViewModel());
        }

        // POST: /Cuenta/Registro
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registro(RegistroViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            bool existeEmail = await _context.Usuarios
                .AnyAsync(u => u.Email == modelo.Email);

            if (existeEmail)
            {
                ModelState.AddModelError(nameof(modelo.Email), "Ese email ya está registrado");
                return View(modelo);
            }

            var nuevoUsuario = new Usuario
            {
                Nombre = modelo.Nombre!,
                Apellido = modelo.Apellido!,
                Telefono = modelo.Telefono ?? string.Empty,
                Dni = modelo.Dni ?? string.Empty,
                Email = modelo.Email!,
                Password = _passwordService.HashPassword(modelo.Password!),
                ProvinciaId = ProvinciaPorDefectoId
            };

            _context.Usuarios.Add(nuevoUsuario);
            await _context.SaveChangesAsync();

            TempData["MensajeExito"] = "¡Cuenta creada con éxito! Ya podés iniciar sesión.";
            return RedirectToAction(nameof(Login));
        }

        // GET: /Cuenta/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginViewModel());
        }

        // POST: /Cuenta/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == modelo.Email);

            if (usuario == null || !_passwordService.VerifyPassword(modelo.Password!, usuario.Password))
            {
                ModelState.AddModelError(string.Empty, "Email o contraseña incorrectos");
                return View(modelo);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, $"{usuario.Nombre} {usuario.Apellido}"),
                new Claim(ClaimTypes.Email, usuario.Email)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                new AuthenticationProperties { IsPersistent = true });

            return RedirectToAction("Index", "Home");
        }

        // POST: /Cuenta/Logout
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult AccesoDenegado()
        {
            return View();
        }
    }
}
