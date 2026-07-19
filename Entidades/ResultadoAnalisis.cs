using Enums;

namespace Entidades;

public class ResultadoAnalisis
{
    public NivelRiesgo Riesgo { get; set; }
    public string Motivo { get; set; } = string.Empty;
}