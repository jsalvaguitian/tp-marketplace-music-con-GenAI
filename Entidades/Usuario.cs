using Microsoft.VisualBasic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entidades;

public class Usuario
{
    [Key]
    public int Id { get; set; }
    public string Nombre { get; set; }
    public string Apellido {get; set;}
    public string Telefono {get; set;}
    public string Dni {get; set;}
    [Required]
    public string Email { get; set; }
    [Required]
    public string Password { get; set; }

    [ForeignKey(nameof(Provincia))]
    public int ProvinciaId { get; set; }
    public Provincia Provincia { get; set; }

    public ICollection<Publicacion> Publicaciones { get; set; } = new List<Publicacion>();
    public ICollection<Mensaje> Mensajes { get; set; } = new List<Mensaje>();

    public ICollection<Conversacion> ConversacionesComoVendedor { get; set; }
    = new List<Conversacion>();

    public ICollection<Conversacion> ConversacionesComoComprador { get; set; }
        = new List<Conversacion>();
}