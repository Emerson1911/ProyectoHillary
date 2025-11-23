// Controllers/AuthController.cs
using FoxRedConstruccion.Services;
using Hillary.DTOs.UsuarioDTO;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FoxRedConstruccion.Controllers
{
    public class AuthController : Controller
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        // GET: Auth/Login
        public IActionResult Login(string? returnUrl = null)
        {
            // Si ya está autenticado, redirigir al home
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.ReturnUrl = returnUrl;

            // Mostrar mensajes de TempData si vienen del registro
            if (TempData["Success"] != null)
            {
                ViewBag.SuccessMessage = TempData["Success"];
                ViewBag.EmailGenerado = TempData["EmailGenerado"];
                ViewBag.InfoMessage = TempData["InfoMessage"];
            }

            return View(new LoginDTO());
        }

        // POST: Auth/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDTO loginDto, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View(loginDto);
            }

            var result = await _authService.LoginAsync(loginDto);

            if (result != null)
            {
                Console.WriteLine($"✅ Login exitoso para: {result.Email}");

                // ✅ Crear claims del usuario
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, result.UserId.ToString()),
                    new Claim(ClaimTypes.Name, result.Nombre),
                    new Claim(ClaimTypes.Email, result.Email),
                    new Claim("EmpresaId", result.EmpresaId.ToString()),
                    new Claim("EmpresaNombre", result.EmpresaNombre),
                    new Claim(ClaimTypes.Role, result.RolNombre),
                    new Claim("RolId", result.RolId.ToString()),
                    new Claim("Token", result.Token) // Guardar el JWT token
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true, // "Recordarme"
                    ExpiresUtc = result.ExpiresAt
                };

                // ✅ Iniciar sesión con cookie
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                Console.WriteLine($"🍪 Cookie creada para usuario: {result.UserId}");
                Console.WriteLine($"🔑 Claims creados: {claims.Count}");

                TempData["Success"] = $"¡Bienvenido, {result.Nombre}!";

                // Redirigir a la URL original o al home
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                // Redirigir a la página de bienvenida (que detectará que está autenticado)
                return RedirectToAction("Index", "Home");
            }

            TempData["Error"] = "Email o contraseña incorrectos";
            return View(loginDto);
        }

        // POST: Auth/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            TempData["Success"] = "Sesión cerrada exitosamente";
            return RedirectToAction(nameof(Login));
        }
    }
}