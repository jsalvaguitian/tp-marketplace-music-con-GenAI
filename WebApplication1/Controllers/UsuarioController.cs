using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Servicios;
using System.Security.Claims;
using WebApplication1.Models.ViewModels;
using Enums;
using System.Net.WebSockets;
namespace WebApplication1.Controllers
{
    [Authorize] //se requiere iniciar sesion para ver perffil
    public class UsuarioController : Controller
    {
        private readonly IUsuarioService _usuarioService;

        public UsuarioController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [HttpGet]
        public async Task<ActionResult> Perfil(int? id)
        {
            var obtenerID = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (obtenerID == null) return RedirectToAction("Login", "Cuenta");


            int idLogueado = int.Parse(obtenerID);

            int idBuscar = id ?? idLogueado;

            bool esPerfilPropio = idBuscar == idLogueado;

            var usuario = await _usuarioService.ObtenerPorIdConPublicacionesAsync(idBuscar);
            if (usuario == null)
            {
                return NotFound(); // Retorna 404 si el usuario no existe
            }

            // Separamos las publicaciones por su estado (Activa vs Finalizada)
            var viewModel = new PerfilUsuarioViewModel
            {
                Usuario = usuario,
                esPerfilPropio = esPerfilPropio,
                PublicacionesActivas = usuario.Publicaciones
                    .Where(p => p.Estado == EstadoPublicacion.Activa)
                    .OrderByDescending(p => p.FechaCreacion)
                    .ToList(),
                PublicacionesFinalizadas = usuario.Publicaciones
                    .Where(p => p.Estado == EstadoPublicacion.Finalizada)
                    .OrderByDescending(p => p.FechaCreacion)
                    .ToList()
            };
            return View(viewModel);



        }

    }
}
