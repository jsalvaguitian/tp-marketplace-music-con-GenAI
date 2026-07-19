using System.ComponentModel.DataAnnotations;

namespace Entidades;

public class Provincia
{
    [Key]
    public int Id { get; set; }
    public string Nombre { get; set; }

    public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();

}