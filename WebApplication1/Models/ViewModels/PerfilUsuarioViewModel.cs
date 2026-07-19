using Entidades;
namespace WebApplication1.Models.ViewModels
{
    public class PerfilUsuarioViewModel
    {
        public Usuario Usuario { get; set; } = null!;

        // Listas separadas de publicaciones por su estado
        public List<Publicacion> PublicacionesActivas { get; set; } = new();
        public List<Publicacion> PublicacionesFinalizadas { get; set; } = new();

        public bool esPerfilPropio { get; set; }
    }
}
