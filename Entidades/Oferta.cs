using Enums;

namespace Entidades
{
    public class Oferta
    {
        public int Id { get; set; }

        public decimal? Monto { get; set; }

        public string? ProductoOfrecido { get; set; }

        public string Mensaje { get; set; }

        public DateTime FechaCreacion { get; set; }

        public EstadoOferta Estado { get; set; }

        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; }

        public int PublicacionId { get; set; }
        public Publicacion Publicacion { get; set; }
    }
}
