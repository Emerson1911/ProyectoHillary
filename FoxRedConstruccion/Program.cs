using FoxRedConstruccion.Services;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Leer la URL base de la API desde appsettings.json
var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"];
Console.WriteLine($"🔧 API Base URL configurada: {apiBaseUrl}");

// Configurar HttpClient con políticas de reintentos y timeout
builder.Services.AddHttpClient("HillaryApi", client =>
{
    client.BaseAddress = new Uri(apiBaseUrl!);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.Timeout = TimeSpan.FromSeconds(30);
})
.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    // Permitir certificados SSL auto-firmados en desarrollo
    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
});

// ✅ Registrar los servicios personalizados
builder.Services.AddScoped<EmpresaService>();
builder.Services.AddScoped<UsuarioService>();
builder.Services.AddScoped<AuthService>(); // ⭐ AGREGAR ESTA LÍNEA

// ✅ Configurar autenticación con cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.LogoutPath = "/Auth/Logout";
        options.AccessDeniedPath = "/Auth/Login";
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
        options.SlidingExpiration = true;

        // ✅ Configuración mejorada de cookies
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        options.Cookie.SameSite = SameSiteMode.Lax; // ⭐ AGREGAR
        options.Cookie.Name = "FoxRedAuth";
        options.Cookie.IsEssential = true; // ⭐ AGREGAR

        // ✅ Eventos para debugging
        options.Events = new CookieAuthenticationEvents
        {
            OnSigningOut = async context =>
            {
                Console.WriteLine($"🗑️ Cookie eliminada: {context.HttpContext.User.Identity?.Name}");
                await Task.CompletedTask;
            }
        };
    });

// Agregar soporte para Razor Pages y MVC
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// Configurar el pipeline de la aplicación
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage(); // Mejor debugging en desarrollo
}

// Comentar temporalmente para evitar problemas de redirección
// app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseRouting();

// ✅ IMPORTANTE: Agregar middleware de autenticación ANTES de autorización
app.UseAuthentication(); // ⭐ AGREGAR ESTA LÍNEA
app.UseAuthorization();

// Configurar rutas para MVC
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}");

// Habilitar Razor Pages también
app.MapRazorPages();

Console.WriteLine("✅ Aplicación iniciada correctamente");
Console.WriteLine($"🌐 Frontend corriendo en los puertos configurados");
Console.WriteLine($"🔐 Autenticación con cookies configurada");

app.Run();