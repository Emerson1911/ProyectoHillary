// Services/EmpresaService.cs
using Hillary.DTOs.EmpresaDTOS;
using System.Text;
using System.Text.Json;

namespace FoxRedConstruccion.Services
{
    public class EmpresaService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public EmpresaService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("HillaryApi");

            // ✅ SOLUCIÓN: Configurar para que serialice con PascalCase (Primera letra mayúscula)
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = null  // ← ESTO ES CLAVE: null = mantiene PascalCase
            };
        }

        // Crear empresa
        public async Task<GetIDResultEmpresaDTO?> CreateAsync(CreateEmpresaDTO empresa)
        {
            // ✅ Ahora serializará como: {"Nombre":"...", "Ruc":"..."} en vez de {"nombre":"...", "ruc":"..."}
            var json = JsonSerializer.Serialize(empresa, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("Empresa", content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<GetIDResultEmpresaDTO>(responseContent, _jsonOptions);
            }

            // Para debug
            var errorContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Error al crear empresa: {response.StatusCode}");
            Console.WriteLine($"JSON enviado: {json}");
            Console.WriteLine($"Respuesta API: {errorContent}");

            return null;
        }

        // Obtener empresa por ID
        public async Task<GetIDResultEmpresaDTO?> GetByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"Empresa/{id}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<GetIDResultEmpresaDTO>(content, _jsonOptions);
            }

            return null;
        }

        // Buscar empresas con paginación
        public async Task<SearchResultEmpresaDTO?> SearchAsync(SearchQueryEmpresaDTO searchQuery)
        {
            var json = JsonSerializer.Serialize(searchQuery, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("Empresa/search", content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<SearchResultEmpresaDTO>(responseContent, _jsonOptions);
            }

            return new SearchResultEmpresaDTO { CountRow = 0, Data = new List<SearchResultEmpresaDTO.EmpresaDTO>() };
        }

        // Editar empresa
        public async Task<GetIDResultEmpresaDTO?> EditAsync(int id, EditEmpresaDTO empresa)
        {
            var json = JsonSerializer.Serialize(empresa, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"Empresa/{id}", content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<GetIDResultEmpresaDTO>(responseContent, _jsonOptions);
            }

            return null;
        }

        // Eliminar empresa
        public async Task<bool> DeleteAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"Empresa/{id}");
            return response.IsSuccessStatusCode;
        }

        // Cambiar estado de empresa
        public async Task<bool> ChangeStatusAsync(int id, bool activo)
        {
            var statusRequest = new ChangeStatusRequestDTO { Activo = activo };
            var json = JsonSerializer.Serialize(statusRequest, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PatchAsync($"Empresa/{id}/status", content);
            return response.IsSuccessStatusCode;
        }
    }
}