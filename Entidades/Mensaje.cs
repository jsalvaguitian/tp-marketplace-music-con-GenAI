using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices;

namespace Entidades;

public class Mensaje
{
    public int Id { get; set; }

    public string Texto { get; set; }

    public string? ArchivoUrl { get; set; }

    public DateTime FechaEnvio { get; set; }

    public int UsuarioId { get; set; }
    public Usuario Usuario { get; set; }

    public int ConversacionId { get; set; }
    public Conversacion Conversacion { get; set; }
}