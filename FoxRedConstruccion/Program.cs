using FoxRedConstruccion.Services;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Leer la URL base de la API desde appsettings.json
var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"];
Console.WriteLine($"🔧 API Base URL configurada: {apiBaseUrl}");

// ✅ Registrar IHttpContextAccessor (NECESARIO para el AuthTokenHandler)
builder.Services.AddHttpContextAccessor();

// ✅ Registrar el AuthTokenHandler
builder.Services.AddTransient<AuthTokenHandler>();

// Configurar HttpClient con políticas de reintentos y timeout
builder.Services.AddHttpClient("HillaryApi", client =>
{
    client.BaseAddress = new Uri(apiBaseUrl!);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.Timeout = TimeSpan.FromSeconds(30);
})
.AddHttpMessageHandler<AuthTokenHandler>() // ✅ AGREGAR EL HANDLER AQUÍ
.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    // Permitir certificados SSL auto-firmados en desarrollo
    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
});

// ✅ Registrar los servicios personalizados
builder.Services.AddScoped<EmpresaService>();
builder.Services.AddScoped<UsuarioService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<TareaService>();

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
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.Name = "FoxRedAuth";
        options.Cookie.IsEssential = true;

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
    app.UseDeveloperExceptionPage();
}

app.UseStaticFiles();
app.UseRouting();

// ✅ IMPORTANTE: Autenticación ANTES de autorización
app.UseAuthentication();
app.UseAuthorization();

// ✅ FORZAR REDIRECCIÓN AL LOGIN si no está autenticado
app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value?.ToLower() ?? "";

    // Rutas públicas permitidas sin autenticación
    var rutasPublicas = new[] { "/auth/login", "/empresa/create", "/usuarios/register" };
    var esRutaPublica = rutasPublicas.Any(ruta => path.StartsWith(ruta));
    var esArchivoEstatico = path.Contains(".");

    // Si no está autenticado y no es una ruta pública, redirigir al login
    if (!context.User.Identity?.IsAuthenticated == true && !esRutaPublica && !esArchivoEstatico)
    {
        Console.WriteLine($"🚫 Acceso no autorizado a: {path} - Redirigiendo al login");
        context.Response.Redirect("/Auth/Login");
        return;
    }

    await next();
});

// ✅ Configurar ruta por defecto al Login
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}");

app.MapRazorPages();

Console.WriteLine("✅ Aplicación iniciada correctamente");
Console.WriteLine($"🌐 Frontend corriendo en los puertos configurados");
Console.WriteLine($"🔐 Autenticación con cookies configurada");
Console.WriteLine($"🔑 AuthTokenHandler registrado - Token se enviará automáticamente");
Console.WriteLine($"🔒 Ruta por defecto: /Auth/Login");

app.Run();