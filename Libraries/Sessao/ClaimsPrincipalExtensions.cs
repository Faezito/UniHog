using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Z1.Model;

namespace UniHog.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static int GetUserId(this ClaimsPrincipal user)
        {
            var claim = user?.FindFirst(ClaimTypes.NameIdentifier);

            if (claim == null)
            {
                // tenta limpar o cookie
                try
                {
                    var http = new HttpContextAccessor();
                    http.HttpContext?.SignOutAsync("CookieAuth");
                }
                catch { }

                return 0;
            }

            if (!int.TryParse(claim.Value, out int id))
                return 0;

            return id;
        }

        public static int GetUserTipoId(this ClaimsPrincipal user)
        {
            var claim = user?.FindFirst("TipoID");

            if (claim == null)
            {
                // tenta limpar o cookie
                try
                {
                    var http = new HttpContextAccessor();
                    http.HttpContext?.SignOutAsync("CookieAuth");
                }
                catch { }

                return 0;
            }

            if (!int.TryParse(claim.Value, out int id))
                return 0;

            return id;
        }

        public static string GetUserTipo(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.Role)?.Value;
        }

        public static string GetUserEmail(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.Email)?.Value;
        }

        public static string GetUserName(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.Name)?.Value;
        }

        public static string GetUsername(this ClaimsPrincipal user)
        {
            return user.FindFirst("usuario")?.Value;
        }

        public static bool GetSenhaTemp(this ClaimsPrincipal user)
        {
            var _temp = user.FindFirst("senhaTemporaria")?.Value;
            if(string.IsNullOrEmpty(_temp))
                return false;

            return (bool)Convert.ChangeType(_temp, typeof(bool));
        }

        public static UsuarioModel ObterUsuario(this ClaimsPrincipal user)
        {
            int id = GetUserId(user);
            string tipo = GetUserTipo(user);
            string email = GetUserEmail(user);
            string nome = GetUserName(user);
            int tipoId = GetUserTipoId(user);

            var usuario = new UsuarioModel
            {
                PessoaID = id,
                Email = email,
                NomeCompleto = nome,
                Tipo = tipo,
                TipoID = tipoId,
            };
            return usuario;

            // OBTER USUÁRIO OU QUALQUER PROPRIEDADE DELE ASSIM
            //
            //UsuarioModel usuario = this.User.ObterUsuario();

        }
    }
}
