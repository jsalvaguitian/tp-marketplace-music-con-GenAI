
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models.DTOsIA;
using WebApplication1.Services;

namespace WebApplication1.Controllers;

public class IAController : Controller
{
    private readonly IIAService _iaService;
    private readonly IConfiguration configuration;

    public IAController(IIAService iaService, IConfiguration configuration)
    {
        _iaService = iaService;
        this.configuration = configuration;
    }

    [HttpGet]
    public IActionResult TestApiKey()
    {
        var apiKey = configuration["Gemini:ApiKey"];

        return Ok(apiKey is null
            ? "API KEY NO ENCONTRADA"
            : $"API KEY OK ({apiKey.Length} caracteres)");
    }

    [HttpGet]
    public IActionResult Test()
    {
        return Content("Funciona");
    }

    [HttpPost]
    public IActionResult TestPost()
    {
        return Ok("POST Funciona");
    }
    /*
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
    */

    [HttpPost]
    public async Task<IActionResult> MejorarMensaje(
        [FromBody] MejorarMensajeDto request)
    {
        try
        {
            var mensajeMejorado =
                await _iaService.MejorarMensajeAsync(request.Mensaje);

            return Json(new
            {
                mensaje = mensajeMejorado
            });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.ToString());
        }
    }
    [HttpPost]
    public async Task<IActionResult> AnalizarMensaje(
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