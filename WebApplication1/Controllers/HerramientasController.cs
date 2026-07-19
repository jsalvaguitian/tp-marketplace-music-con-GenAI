using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class HerramientasController : Controller
    {
        private static List<Herramienta> herramientas = new List<Herramienta>
        {
            new Herramienta { Id = 1, Descripcion = "Martillo", Precio = 10.99m, Imagen = "martillo.jpg" },
            new Herramienta { Id = 2, Descripcion = "Destornillador", Precio = 5.49m, Imagen = "destornillador.jpg" }
        };

        public IActionResult Index()
        {
            return View(herramientas);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Herramienta herramienta)
        {
            herramienta.Id = herramientas.Count > 0 ? herramientas.Max(h => h.Id) + 1 : 1;
            herramientas.Add(herramienta);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            var herramienta = herramientas.FirstOrDefault(h => h.Id == id);
            if (herramienta == null)
            {
                return NotFound();
            }
            return View(herramienta);
        }

        [HttpPost]
        public IActionResult Edit(Herramienta herramienta)
        {
            var existingHerramienta = herramientas.FirstOrDefault(h => h.Id == herramienta.Id);
            if (existingHerramienta != null)
            {
                existingHerramienta.Descripcion = herramienta.Descripcion;
                existingHerramienta.Precio = herramienta.Precio;
                existingHerramienta.Imagen = herramienta.Imagen;
            }
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var herramienta = herramientas.FirstOrDefault(h => h.Id == id);
            if (herramienta != null)
            {
                herramientas.Remove(herramienta);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}