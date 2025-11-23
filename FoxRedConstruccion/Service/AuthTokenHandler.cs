// Services/AuthTokenHandler.cs
using Microsoft.AspNetCore.Authentication;
using System.Net.Http.Headers;

namespace FoxRedConstruccion.Services
{
    public class AuthTokenHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthTokenHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            // Obtener el token del usuario autenticado
            var token = _httpContextAccessor.HttpContext?.User.FindFirst("Token")?.Value;

            if (!string.IsNullOrEmpty(token))
            {
                // Agregar el token al header Authorization
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                Console.WriteLine($"🔑 Token agregado a la petición: {token.Substring(0, 20)}...");
            }
            else
            {
                Console.WriteLine("⚠️ No se encontró token en el contexto del usuario");
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}