using FoxRedConstruccion.Services;

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

// Registrar los servicios personalizados
builder.Services.AddScoped<EmpresaService>();

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

// Comentar temporalmente para evitar problemas de redirección
// app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

// Configurar rutas para MVC
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Habilitar Razor Pages también
app.MapRazorPages();

Console.WriteLine("✅ Aplicación iniciada correctamente");
Console.WriteLine($"🌐 Frontend corriendo en los puertos configurados");

app.Run();