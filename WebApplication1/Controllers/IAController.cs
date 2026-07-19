
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models.DTOsIA;
using WebApplication1.Services;

public class IAController : Controller
{
    private readonly IIAService _iaService;

    public IAController(IIAService iaService)
    {
        _iaService = iaService;
    }

    [HttpPost]
    public async Task<IActionResult> MejorarMensajeAsync(
    [FromBody] MejorarMensajeDto request)
    {
        var mensajeMejorado =
            await _iaService.MejorarMensajeAsync(request.Mensaje);

        var dto = new MejorarMensajeDto
        {
            Mensaje = mensajeMejorado
        };

        return Json(dto);
    }

    [HttpPost]
    public async Task<IActionResult>AnalizarMensajeAsync(
    [FromBody] AnalizarMensajeRequestDto request)
    {
        var resultado =
            await _iaService.AnalizarMensajeAsync(request.Mensaje);

        var dto = new AnalizarMensajeResponseDto
        {
            Riesgo = resultado.Riesgo.ToString().ToUpper(),
            Motivo = resultado.Motivo
        };

        return Json(dto);
    }
}