// Services/AuthService.cs
using Hillary.DTOs.UsuarioDTO;
using System.Text;
using System.Text.Json;

namespace FoxRedConstruccion.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public AuthService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("HillaryApi");

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = null  // Mantiene PascalCase
            };
        }

        // Login
        public async Task<LoginResponseDTO?> LoginAsync(LoginDTO loginDto)
        {
            try
            {
                var json = JsonSerializer.Serialize(loginDto, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("Usuario/login", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<LoginResponseDTO>(responseContent, _jsonOptions);
                }

                // Debug en caso de error
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"❌ Error al hacer login: {response.StatusCode}");
                Console.WriteLine($"📤 JSON enviado: {json}");
                Console.WriteLine($"📥 Respuesta API: {errorContent}");

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Excepción en LoginAsync: {ex.Message}");
                return null;
            }
        }
    }
}