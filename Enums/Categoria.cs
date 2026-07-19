using System.ComponentModel;

namespace Enums;

public enum Categoria
{
    [Description("Bajos")]
    Bajos = 1,

    [Description("Guitarras")]
    Guitarras = 2,

    [Description("Percusión")]
    Percusión = 3,

    [Description("Teclados")]
    Teclados = 4,

    [Description("Equipamiento y Audio")]
    EquipamientoyAudio = 5,

    [Description("Permutas")]
    Permutas = 6,

    [Description("Producción y Mezcla")]
    ProduccionYMezcla = 7,

    [Description("Composición y Escritura")]
    ComposicionYEscritura = 8,

    [Description("Servicios de Sesión / Tracks")]
    ServiciosDeSesionTracks = 9,

    [Description("Clases y Tutorías")]
    ClasesYTutorias = 10,

    [Description("Shows / Giras en Vivo")]
    ShowsGirasEnVivo = 11,

    [Description("Promoción")]
    Promocion = 12,

    [Description("Videoclips")]
    Videoclips = 13,
}