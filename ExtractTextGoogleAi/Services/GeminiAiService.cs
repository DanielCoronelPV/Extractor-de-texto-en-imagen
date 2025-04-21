using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ExtractTextGoogleAi.Services
{
    public class GeminiAiService
    {
        private readonly string _apiKey;
        private readonly HttpClient _httpClient;

        public GeminiAiService()
        {
            // Ruta relativa al archivo de la API Key
            string apiKeyPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "gemini-apikey.txt");

            // Leer la clave del archivo y eliminar espacios o saltos de línea
            _apiKey = File.ReadAllText(apiKeyPath).Trim();

            // Configurar el HttpClient con la URL base
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent")
            };
        }

        public async Task<string> GetFilteredProductDataAsync(string rawText)
        {
            try
            {
                var prompt = $@"Extraé los siguientes datos de este texto escaneado, si están disponibles: 
- Código de producto
- Descripción de producto
- Cantidad de producto
- Precio unitario de producto

El texto escaneado es el siguiente:
{rawText}";

                var requestBody = new
                {
                    contents = new[]
                    {
                        new {
                            parts = new[] {
                                new {
                                    text = prompt
                                }
                            }
                        }
                    }
                };

                var json = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Construir la URL completa con la API Key
                string requestUri = $"?key={_apiKey}";

                var response = await _httpClient.PostAsync(requestUri, content);
                var responseText = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    return $"Error: {response.StatusCode} - {responseText}";

                using var doc = JsonDocument.Parse(responseText);
                var text = doc.RootElement
                              .GetProperty("candidates")[0]
                              .GetProperty("content")
                              .GetProperty("parts")[0]
                              .GetProperty("text")
                              .GetString();

                return text ?? "No se encontró contenido.";
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }
    }
}
