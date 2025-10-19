using Microsoft.EntityFrameworkCore;
using ProyectoHillary1.Models.Dal;

namespace ProyectoHillary1.Utilities
{
    public class EmailGenerator
    {
        private readonly ProyectoHillaryContext _context;


        public EmailGenerator(ProyectoHillaryContext context)
        {
            _context = context;
        }

        // Genera email formato: juan.perez@empresa.com
        public async Task<string> GenerarEmailUnico(string nombre, int empresaId)
        {
            var empresa = await _context.empresa.FindAsync(empresaId);
            string dominioEmpresa = LimpiarTexto(empresa?.Nombre ?? "empresa");
            string nombreLimpio = LimpiarTexto(nombre);
            string emailBase = $"{nombreLimpio}@{dominioEmpresa}.com";

            return await ObtenerEmailDisponible(emailBase, nombreLimpio, dominioEmpresa);
        }

        // Genera email formato corto: jperez@empresa.com
        public async Task<string> GenerarEmailCorto(string nombreCompleto, int empresaId)
        {
            var empresa = await _context.empresa.FindAsync(empresaId);
            string dominioEmpresa = LimpiarTexto(empresa?.Nombre ?? "empresa");

            var partes = nombreCompleto.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (partes.Length < 2)
                return await GenerarEmailUnico(nombreCompleto, empresaId);

            string primeraLetra = partes[0].Substring(0, 1).ToLower();
            string apellido = LimpiarTexto(partes[^1]);
            string emailBase = $"{primeraLetra}{apellido}@{dominioEmpresa}.com";

            return await ObtenerEmailDisponible(emailBase, $"{primeraLetra}{apellido}", dominioEmpresa);
        }

        // Valida si un email ya existe
        public async Task<bool> EmailYaExiste(string email)
        {
            return await _context.usuario.AnyAsync(u => u.Email == email);
        }

        // Limpia texto: remueve espacios, acentos y caracteres especiales
        private string LimpiarTexto(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return "sin-nombre";

            string textoLimpio = texto.ToLower()
                .Replace(" ", ".")
                .Normalize(System.Text.NormalizationForm.FormD);

            var chars = textoLimpio.Where(c =>
                System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c) !=
                System.Globalization.UnicodeCategory.NonSpacingMark &&
                (char.IsLetterOrDigit(c) || c == '.')
            ).ToArray();

            return new string(chars);
        }

        // Agrega número si el email ya existe
        private async Task<string> ObtenerEmailDisponible(string emailBase, string prefijo, string dominio)
        {
            string email = emailBase;
            int contador = 1;

            while (await _context.usuario.AnyAsync(u => u.Email == email))
            {
                email = $"{prefijo}{contador}@{dominio}.com";
                contador++;
            }

            return email;
        }
    }
}
