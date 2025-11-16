// Services/UsuarioService.cs
using Hillary.DTOs.UsuarioDTO;
using System.Text;
using System.Text.Json;

namespace FoxRedConstruccion.Services
{
    public class UsuarioService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public UsuarioService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("HillaryApi");

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = null  // Mantiene PascalCase
            };
        }

        // Registrar usuario (público) - Retorna objeto con la respuesta del backend
        public async Task<(bool Success, string? Message, int Id, string? Email, string? Nombre)> RegisterAsync(CreateUsuarioDTO usuario)
        {
            try
            {
                var json = JsonSerializer.Serialize(usuario, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("Usuario/register", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();

                    // Deserializar la respuesta a un objeto dinámico
                    using var document = JsonDocument.Parse(responseContent);
                    var root = document.RootElement;

                    return (
                        Success: true,
                        Message: root.GetProperty("message").GetString(),
                        Id: root.GetProperty("id").GetInt32(),
                        Email: root.GetProperty("email").GetString(),
                        Nombre: root.GetProperty("nombre").GetString()
                    );
                }

                // En caso de error
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"❌ Error al registrar usuario: {response.StatusCode}");
                Console.WriteLine($"📤 JSON enviado: {json}");
                Console.WriteLine($"📥 Respuesta API: {errorContent}");

                return (Success: false, Message: "Error al registrar usuario", Id: 0, Email: null, Nombre: null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Excepción en RegisterAsync: {ex.Message}");
                return (Success: false, Message: ex.Message, Id: 0, Email: null, Nombre: null);
            }
        }
    }
}