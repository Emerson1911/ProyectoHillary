// Services/TareaService.cs
using Hillary.DTOs.TareaDTOS;
using System.Text;
using System.Text.Json;

namespace FoxRedConstruccion.Services
{
    public class TareaService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public TareaService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("HillaryApi");

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = null  // Mantiene PascalCase
            };
        }

        // Crear tarea
        public async Task<bool> CreateAsync(CreateTareaDTO tarea)
        {
            var json = JsonSerializer.Serialize(tarea, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("Tarea", content);

            if (response.IsSuccessStatusCode)
            {
                return true;
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Error al crear tarea: {response.StatusCode}");
            Console.WriteLine($"Respuesta API: {errorContent}");

            return false;
        }

        // Obtener tarea por ID
        public async Task<GetIDResultTareaDTO?> GetByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"Tarea/{id}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<GetIDResultTareaDTO>(content, _jsonOptions);
            }

            return null;
        }

        // Buscar tareas con paginación
        public async Task<SearchResultTareaDTO?> SearchAsync(SearchQueryTareaDTO searchQuery)
        {
            var json = JsonSerializer.Serialize(searchQuery, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("Tarea/search", content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<SearchResultTareaDTO>(responseContent, _jsonOptions);
            }

            return new SearchResultTareaDTO { CountRow = 0, Data = new List<SearchResultTareaDTO.TareaDTO>() };
        }

        // Obtener mis tareas (de mi empresa)
        public async Task<List<SearchResultTareaDTO.TareaDTO>?> GetMisTareasAsync()
        {
            var response = await _httpClient.GetAsync("Tarea/mis-tareas");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<SearchResultTareaDTO.TareaDTO>>(content, _jsonOptions);
            }

            return new List<SearchResultTareaDTO.TareaDTO>();
        }

        // Editar tarea
        public async Task<bool> EditAsync(int id, EditTareaDTO tarea)
        {
            var json = JsonSerializer.Serialize(tarea, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"Tarea/{id}", content);

            return response.IsSuccessStatusCode;
        }

        // Eliminar tarea
        public async Task<bool> DeleteAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"Tarea/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}