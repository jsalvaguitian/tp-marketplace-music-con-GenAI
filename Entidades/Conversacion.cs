using System;
using System.Collections.Generic;
using System.Text;

namespace Entidades
{
    public class Conversacion
    {
        public int Id { get; set; }

        public int PublicacionId { get; set; }
        public Publicacion Publicacion { get; set; }

        // dueño de la publicación
        public int VendedorId { get; set; }
        public Usuario Vendedor { get; set; }

        // quien hizo la oferta
        public int CompradorId { get; set; }
        public Usuario Comprador { get; set; }

        public bool Cerrada { get; set; }

        // opcional, para saber desde qué oferta nació
        public int? OfertaId { get; set; }
        public Oferta? Oferta { get; set; }

        public ICollection<Mensaje> Mensajes { get; set; }
            = new List<Mensaje>();
    }
}
