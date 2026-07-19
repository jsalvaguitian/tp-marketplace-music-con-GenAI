using System.Security.Claims;
using Entidades;
using Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Servicios;

namespace WebApplication1.Controllers
{
    public class PublicacionController : Controller
    {
        private readonly IPublicacionService _publicacionService;
        private readonly IWebHostEnvironment _environment;

        public PublicacionController(IPublicacionService publicacionService, IWebHostEnvironment environment)
        {
            _publicacionService = publicacionService;
            _environment = environment;
        }

        private List<object> ObtenerCategoriasParaVista()
        {
            return Enum.GetValues<Categoria>()
                .Select(c => new { Value = (int)c, Text = c.GetDescription() })
                .Cast<object>()
                .ToList();
        }

        public async Task<IActionResult> Index()
        {
            var publicaciones = await _publicacionService.ObtenerActivasAsync();
            return View(publicaciones);
        }

        public async Task<IActionResult> Detalle(int id)
        {
            var publicacion = await _publicacionService.ObtenerPorIdAsync(id);
            if (publicacion == null) return NotFound();
            return View(publicacion);
        }

        [Authorize]
        [HttpGet]
        public IActionResult Crear()
        {
            ViewBag.Categorias = ObtenerCategoriasParaVista();
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(
            [Bind("Titulo,Descripcion,FechaCierre")] Publicacion publicacion,
            int categoria,
            IFormFile imagenArchivo)
        {
            if (imagenArchivo == null || imagenArchivo.Length == 0)
                ModelState.AddModelError("", "Tenés que subir una imagen");

            if (categoria == 0)
                ModelState.AddModelError("Categoria", "Debes seleccionar una categoría");

            ModelState.Remove("ImagenUrl");
            ModelState.Remove("Usuario");

            if (!ModelState.IsValid)
            {
                ViewBag.Categorias = ObtenerCategoriasParaVista();
                return View(publicacion);
            }

            publicacion.Categoria = (Categoria)categoria;
            publicacion.ImagenUrl = await GuardarImagenAsync(imagenArchivo!);
            publicacion.UsuarioId = ObtenerUsuarioIdLogueado();

            await _publicacionService.CrearAsync(publicacion);
            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var publicacion = await _publicacionService.ObtenerPorIdAsync(id);
            if (publicacion == null) return NotFound();

            if (publicacion.UsuarioId != ObtenerUsuarioIdLogueado())
                return Forbid();

            if (publicacion.Estado == EstadoPublicacion.Finalizada)
                return Forbid();

            ViewBag.Categorias = ObtenerCategoriasParaVista();
            return View(publicacion);
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(
            int id,
            [Bind("Id,Titulo,Descripcion,FechaCierre")] Publicacion publicacionForm,
            int categoria,
            IFormFile? imagenArchivo)
        {
            if (id != publicacionForm.Id) return BadRequest();

            var publicacion = await _publicacionService.ObtenerPorIdAsync(id);
            if (publicacion == null) return NotFound();

            if (publicacion.Estado == EstadoPublicacion.Finalizada)
                return Forbid();

            if (categoria == 0)
                ModelState.AddModelError("Categoria", "Debes seleccionar una categoría");

            ModelState.Remove("ImagenUrl");
            ModelState.Remove("Usuario");

            if (!ModelState.IsValid)
            {
                ViewBag.Categorias = ObtenerCategoriasParaVista();
                return View(publicacionForm);
            }

            publicacionForm.Categoria = (Categoria)categoria;

            string? nuevaImagenUrl = null;
            if (imagenArchivo != null && imagenArchivo.Length > 0)
                nuevaImagenUrl = await GuardarImagenAsync(imagenArchivo);

            var exito = await _publicacionService.EditarAsync(id, ObtenerUsuarioIdLogueado(), publicacionForm, nuevaImagenUrl);
            if (!exito) return Forbid();

            return RedirectToAction(nameof(Detalle), new { id });
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cerrar(int id)
        {
            var exito = await _publicacionService.CerrarAsync(id, ObtenerUsuarioIdLogueado());
            if (!exito) return Forbid();

            return RedirectToAction(nameof(Detalle), new { id });
        }

        private async Task<string> GuardarImagenAsync(IFormFile archivo)
        {
            var carpeta = Path.Combine(_environment.WebRootPath, "images", "publicaciones");
            Directory.CreateDirectory(carpeta);

            var nombreArchivo = $"{Guid.NewGuid()}{Path.GetExtension(archivo.FileName)}";
            var rutaCompleta = Path.Combine(carpeta, nombreArchivo);

            using var stream = new FileStream(rutaCompleta, FileMode.Create);
            await archivo.CopyToAsync(stream);

            return $"/images/publicaciones/{nombreArchivo}";
        }

        private int ObtenerUsuarioIdLogueado()
        {
            var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.Parse(idClaim!);
        }
    }
}