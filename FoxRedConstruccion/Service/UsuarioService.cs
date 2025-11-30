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

        // Buscar usuarios con filtros y paginación
        public async Task<SearchResultUsuarioDTO?> SearchAsync(SearchQueryUsuarioDTO searchDto)
        {
            try
            {
                var json = JsonSerializer.Serialize(searchDto, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("Usuario/search", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<SearchResultUsuarioDTO>(responseContent, _jsonOptions);
                }

                Console.WriteLine($"❌ Error en SearchAsync: {response.StatusCode}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Excepción en SearchAsync: {ex.Message}");
                return null;
            }
        }

        // Obtener usuario por ID
        public async Task<GetlResultUsuarioDTO?> GetByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"Usuario/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<GetlResultUsuarioDTO>(responseContent, _jsonOptions);
                }

                Console.WriteLine($"❌ Error en GetByIdAsync: {response.StatusCode}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Excepción en GetByIdAsync: {ex.Message}");
                return null;
            }
        }

        // Crear usuario (uso interno) - Retorna objeto con la respuesta del backend
        public async Task<(bool Success, string? Message, int Id, string? Email, string? Nombre)> CreateAsync(CreateUsuarioDTO usuario)
        {
            try
            {
                var json = JsonSerializer.Serialize(usuario, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("Usuario", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
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

                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"❌ Error en CreateAsync: {response.StatusCode}");
                Console.WriteLine($"📤 JSON enviado: {json}");
                Console.WriteLine($"📥 Respuesta API: {errorContent}");

                return (Success: false, Message: "Error al crear usuario", Id: 0, Email: null, Nombre: null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Excepción en CreateAsync: {ex.Message}");
                return (Success: false, Message: ex.Message, Id: 0, Email: null, Nombre: null);
            }
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

                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"❌ Error en RegisterAsync: {response.StatusCode}");
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

        // Editar usuario
        public async Task<bool> EditAsync(int id, EditUsuarioDTO editDto)
        {
            try
            {
                var json = JsonSerializer.Serialize(editDto, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"Usuario/{id}", content);

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"❌ Error en EditAsync: {response.StatusCode}");
                Console.WriteLine($"📥 Respuesta API: {errorContent}");

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Excepción en EditAsync: {ex.Message}");
                return false;
            }
        }

        // Eliminar usuario
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"Usuario/{id}");

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }

                Console.WriteLine($"❌ Error en DeleteAsync: {response.StatusCode}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Excepción en DeleteAsync: {ex.Message}");
                return false;
            }
        }

        // Cambiar estado activo/inactivo
        public async Task<bool> ChangeStatusAsync(int id, bool activo)
        {
            try
            {
                var statusDto = new UserChangeStatusRequestDTO { Activo = activo };
                var json = JsonSerializer.Serialize(statusDto, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PatchAsync($"Usuario/{id}/change-status", content);

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }

                Console.WriteLine($"❌ Error en ChangeStatusAsync: {response.StatusCode}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Excepción en ChangeStatusAsync: {ex.Message}");
                return false;
            }
        }
    }
}