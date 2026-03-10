using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Z1.Model;

namespace UniHog.Libraries.Login
{
    public class LoginUsuario
    {
        private readonly IHttpContextAccessor _context;

        public LoginUsuario(IHttpContextAccessor context)
        {
            _context = context;
        }

        public async Task LoginAsync(UsuarioModel usuario)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.PessoaID.ToString()),
                new Claim(ClaimTypes.Name, usuario.NomeCompleto),
                new Claim(ClaimTypes.Role, usuario.Tipo),
                new Claim("TipoID", usuario.TipoID.ToString())
            };

            var identity = new ClaimsIdentity(claims, "CookieAuth");
            var principal = new ClaimsPrincipal(identity);

            await _context.HttpContext.SignInAsync("CookieAuth", principal);
        }

        public async Task LogoutAsync()
        {
            await _context.HttpContext.SignOutAsync("CookieAuth");
        }

        public int? GetUserId()
        {
            var idClaim = _context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            if (idClaim == null) return null;

            if (int.TryParse(idClaim.Value, out int id))
                return id;

            return null;
        }
    }
}
