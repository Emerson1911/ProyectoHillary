using Hillary.DTOs.RolDTOS;
using System.Text;
using System.Text.Json;

namespace FoxRedConstruccion.Services
{
    public class RolService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public RolService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("HillaryApi");
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = null
            };
        }

        // Obtener todos los roles usando SearchResultRolsDTO.RolsDTO
        public async Task<List<SearchResultRolsDTO.RolsDTO>?> GetAllAsync()
        {
            try
            {
                var searchDto = new SearchQueryRolsDTO
                {
                    Nombre = "",
                    PageNumber = 1,
                    PageSize = 100
                };

                var json = JsonSerializer.Serialize(searchDto, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("Rol/search", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<SearchResultRolsDTO>(responseContent, _jsonOptions);

                    return result?.Data;
                }

                Console.WriteLine($"❌ Error en GetAllAsync: {response.StatusCode}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Excepción en GetAllAsync: {ex.Message}");
                return null;
            }
        }

        // Obtener rol por ID
        public async Task<GetIdResultRolsDTO?> GetByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"Rol/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<GetIdResultRolsDTO>(responseContent, _jsonOptions);
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
    }
}