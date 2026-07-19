using System.Text.Json;
using System.Net.Http;
using System.Text;
using Entidades;
using Enums;
using Microsoft.Extensions.Configuration;

namespace WebApplication1.Services;

public interface IIAService
{
    Task<string> MejorarMensajeAsync(string mensaje);

    Task<ResultadoAnalisis> AnalizarMensajeAsync(string mensaje);
}


public class IAService : IIAService
{
    private readonly HttpClient httpClient;
    private readonly IConfiguration configuration;

    public IAService(
        HttpClient httpClient,
        IConfiguration configuration)
    {
        this.httpClient = httpClient;
        this.configuration = configuration;
    }

    public async Task<string> MejorarMensajeAsync(string mensaje)
    {
        string prompt = $"""
            Reescribí el siguiente mensaje para que sea cordial,
            profesional y breve.

            No cambies la intención original.

            Mensaje:
            {mensaje}
            """;

        return await EnviarPromptAsync(prompt);
    }

    public async Task<ResultadoAnalisis> AnalizarMensajeAsync(string mensaje)
    {
        string prompt = $@"
        Analizá el siguiente mensaje.

        Detectá posibles fraudes, estafas, phishing o solicitud de datos personales.

        El campo riesgo DEBE ser únicamente uno de estos valores:
        - BAJO
        - MEDIO
        - ALTO

        Respondé únicamente en formato JSON:

        {{
            ""riesgo"": ""BAJO"",
            ""motivo"": """"
        }}

        Mensaje:
        {mensaje}";

        string respuesta = await EnviarPromptAsync(prompt);

        try
        {
            using var doc = JsonDocument.Parse(respuesta);

            string riesgoTexto = doc.RootElement
                .GetProperty("riesgo")
                .GetString() ?? "BAJO";

            string motivo = doc.RootElement
                .GetProperty("motivo")
                .GetString() ?? string.Empty;


           NivelRiesgo riesgo = riesgoTexto.ToUpper() switch
{
    "ALTO" => NivelRiesgo.Alto,
    "MEDIO" => NivelRiesgo.Medio,
    "BAJO" => NivelRiesgo.Bajo,
    _ => NivelRiesgo.Desconocido
};
            return new ResultadoAnalisis
            {
                Riesgo = riesgo,
                Motivo = motivo
            };
        }
        catch
        {
            return new ResultadoAnalisis
            {
                Riesgo = NivelRiesgo.Bajo,
                Motivo = "No se pudo interpretar la respuesta de Gemini."
            };
        }
    }


    private async Task<string> EnviarPromptAsync(string prompt)
    {
        string apiKey = configuration["Gemini:ApiKey"]!;

        var request = new
        {
            contents = new[]
            {
                new
                {
                    parts = new[]
                    {
                        new
                        {
                            text = prompt
                        }
                    }
                }
            }
        };

        var json = JsonSerializer.Serialize(request);

        var response = await httpClient.PostAsync(
            $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={apiKey}",
            new StringContent(
                json,
                Encoding.UTF8,
                "application/json"));

        response.EnsureSuccessStatusCode();

        string contenido = await response.Content.ReadAsStringAsync();

        using var doc = JsonDocument.Parse(contenido);

        return doc.RootElement
            .GetProperty("candidates")[0]
            .GetProperty("content")
            .GetProperty("parts")[0]
            .GetProperty("text")
            .GetString() ?? string.Empty;
    }
}