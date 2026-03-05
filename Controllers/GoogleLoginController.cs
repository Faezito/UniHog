using UniHog.Libraries.Login;
using UniHog.Libraries.Sessao;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Z1.Model;
using Z2.Servicos;
using Z2.Services.Externo;
using Z4.Bibliotecas;

namespace UniHog.Controllers
{
    public class GoogleLoginController : Controller
    {
        private readonly IUsuarioServicos _seUsuario;
        private readonly IEmailServicos _emailServicos;
        private readonly LoginUsuario _login;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly Sessao _sessao;

        public GoogleLoginController(IUsuarioServicos seUsuario,
            LoginUsuario login,
            IHttpContextAccessor httpContextAccessor,
            IEmailServicos emailServicos,
            Sessao sessao
            )
        {
            _seUsuario = seUsuario;
            _login = login;
            _httpContextAccessor = httpContextAccessor;
            _emailServicos = emailServicos;
            _sessao = sessao;
        }

        public IActionResult LoginGoogle()
        {
            var redirectUrl = Url.Action("GoogleResponse", "GoogleLogin");
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (!result.Succeeded || result.Principal == null)
            {
                return RedirectToAction("Login", "Home");
            }

            var claims = result.Principal.Claims;

            var id = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var nome = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            var nick = ManipularModels.GerarUsuario(email, id);

            // Verifica se usuário já existe no banco
            UsuarioModel usuario = await _seUsuario.Obter(int.Parse(id), email);

            if (usuario == null)
            {
                usuario = new UsuarioModel()
                {
                    NomeCompleto = nome,
                    Usuario = nick,
                    Email = email,
                    Tipo = "C", // padrão
                    Senha = KeyGenerator.GetUniqueKey(10)
                };

                // TODO: Adicionar e-mail de confirmação de cadastro para o usuário
                //int? novoid = await _seUsuario.Cadastrar(usuario);
                //usuario = await _seUsuario.Obter(novoid, null);
            }

            // Configura claims internas do sistema
            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);

            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, usuario.PessoaID.ToString()));
            identity.AddClaim(new Claim(ClaimTypes.Email, usuario.Email));
            identity.AddClaim(new Claim(ClaimTypes.Name, usuario.NomeCompleto));
            identity.AddClaim(new Claim(ClaimTypes.Role, usuario.Tipo));

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity),
                new AuthenticationProperties()
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTime.UtcNow.AddHours(3)
                });

            return RedirectToAction("Index", "Usuario", new { id = usuario.PessoaID });
        }


        public IActionResult VincularGoogle(int usuarioId)
        {
            var redirectUrl = Url.Action("GoogleVincular", "GoogleLogin");
            var props = new AuthenticationProperties
            {
                RedirectUri = redirectUrl,
                Items = { { "usuarioId", usuarioId.ToString() } }
            };

            return Challenge(props, GoogleDefaults.AuthenticationScheme);
        }


        public async Task<IActionResult> GoogleVincular()
        {
            // Autentica via esquema do Google
            var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

            if (!result.Succeeded)
                return Problem(detail: "Falha ao autenticar no Google.");

            var usuarioIdString = result.Properties.Items["usuarioId"];
            if (!int.TryParse(usuarioIdString, out int usuarioId))
                return Problem(detail: "ID de usuário inválido.");

            // Pega claims retornadas pelo Google
            var googleId = result.Principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var email = result.Principal.FindFirst(ClaimTypes.Email)?.Value;

            if (googleId == null)
                return Problem(detail: "GoogleId não encontrado.");

            // Carrega usuário do banco
            var usuario = await _seUsuario.Obter(usuarioId, null);

            if (usuario == null)
                return Problem(detail: "Usuário não encontrado.");

            // Atualiza no banco
            //await _seUsuario.Cadastrar(usuario);

            // Recria o cookie interno com claims completas
            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, usuario.PessoaID.ToString()));
            identity.AddClaim(new Claim(ClaimTypes.Email, usuario.Email));
            identity.AddClaim(new Claim(ClaimTypes.Name, usuario.NomeCompleto));
            identity.AddClaim(new Claim("Tipo", usuario.Tipo));

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity),
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTime.UtcNow.AddHours(3)
                });

            return RedirectToAction("Index", "Usuario");
        }
    }
}
