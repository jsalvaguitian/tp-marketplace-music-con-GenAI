using Entidades;

namespace WebApplication1.Models.ViewModels
{
    public class HomeViewModel
    {
        public List<Publicacion> UltimasPublicaciones { get; set; } = new();
    }
}
