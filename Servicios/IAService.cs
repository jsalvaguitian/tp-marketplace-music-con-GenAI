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
            Reescribí el siguiente mensaje para que sea cordial, profesional y breve. 
            No cambies la intención original. 
            Respondé solo con el texto mejorado. 
            No agregues explicaciones, títulos ni comentarios.

            Mensaje:
            {mensaje}
            """;

        return await EnviarPromptAsync(prompt);
    }

    public async Task<ResultadoAnalisis> AnalizarMensajeAsync(string mensaje)
    {
        string prompt = $@"
        Analizá el siguiente mensaje de un chat privado entre un comprador y un vendedor de un marketplace.
        Clasificá el riesgo utilizando únicamente uno de estos valores:
        - BAJO:
        El mensaje corresponde a una conversación normal sobre el producto, precio, estado, entrega o permuta.

        - MEDIO:
        El mensaje solicita información personal (teléfono, email, dirección, CBU, alias, DNI), intenta sacar la conversación fuera de la plataforma o pide compartir datos que podrían afectar la privacidad.

        - ALTO:
        El mensaje presenta posibles señales de fraude o phishing, por ejemplo:
        - solicita contraseñas o códigos de verificación;
        - solicita datos completos de tarjetas bancarias;
        - envía enlaces sospechosos;
        - intenta suplantar a una empresa o banco;
        - presiona para realizar pagos inseguros o verificar cuentas fuera del marketplace;
        - intenta engañar al usuario para obtener dinero o información sensible.

        Respondé únicamente en formato JSON:

        {{
            ""riesgo"": ""BAJO"",
            ""motivo"": """"
        }}

        No agregues texto fuera del JSON.

        Mensaje:
        {mensaje}";

        string respuesta = await EnviarPromptAsync(prompt);

        Console.WriteLine(respuesta);
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

        var httpRequest = new HttpRequestMessage(
            HttpMethod.Post,
            "https://generativelanguage.googleapis.com/v1beta/models/gemini-3.5-flash:generateContent");

        httpRequest.Headers.Add("x-goog-api-key", apiKey);

        httpRequest.Content = new StringContent(
            json,
            Encoding.UTF8,
            "application/json");

        var response = await httpClient.SendAsync(httpRequest);

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