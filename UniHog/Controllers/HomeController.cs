using UniHog.Libraries.Login;
using UniHog.Libraries.Sessao;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Z1.Model;
using Z2.Servicos;
using Z2.Services.Externo;
using UniHog.Extensions;

namespace UniHog.Controllers
{
    [Autorizacoes]
    public class HomeController : Controller
    {
        private readonly IUsuarioServicos _seUsuario;
        private readonly IEmailServicos _emailServicos;
        private readonly LoginUsuario _login;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly Sessao _sessao;

        public HomeController(IUsuarioServicos seUsuario,
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

        [HttpGet]
        public async Task<IActionResult> Login()
        {
            if (HttpContext.Items.ContainsKey("MSG_E"))
            {
                TempData["MSG_E"] = HttpContext.Items["MSG_E"];
            }
            if (this.User.GetUserId() != 0)
            {
                var user = this.User.ObterUsuario();
                await _seUsuario.AtualizarSessao(user.PessoaID.Value);

                if (user.TipoID <= 4)
                {
                    return RedirectToAction("Index", "Menu");
                }
                else if (user.TipoID == 5)
                {
                    return RedirectToAction("Professor", "Menu");
                }
                else if (user.TipoID == 10)
                {
                    return RedirectToAction("Cadastro", "Interessado");
                }
                else if (user.TipoID == 15)
                {
                    return RedirectToAction("AtendimentosSociais", "Menu");
                }
                else
                {
                    return RedirectToAction("AtendimentosSociais", "Menu");
                }
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string? Login, string? Senha)
        {
            UsuarioModel user = new();
            user = await _seUsuario.Login(Login, Senha);

            if (user == null)
            {
                return Problem(title: "Erro", detail: "Credenciais inválidas.");
            }

            try
            {
                // CRIAÇÃO DOS CLAIMS
                var claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.NameIdentifier, user.PessoaID.ToString()),
                    new Claim(ClaimTypes.Name, user.NomeCompleto),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Tipo),
                    new Claim("TipoID", user.TipoID.ToString()),
                    new Claim("usuario", user.Usuario.ToString()),
                    new Claim("senhaTemporaria", user.SenhaTemporaria.ToString())
                };

                // Identidade
                var identity = new ClaimsIdentity(
                    claims,
                    CookieAuthenticationDefaults.AuthenticationScheme
                );

                // Criar principal
                var principal = new ClaimsPrincipal(identity);

                // Efetuar login via cookie
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    principal,
                    new AuthenticationProperties()
                    {
                        IsPersistent = true,       // manter logado
                        ExpiresUtc = DateTime.UtcNow.AddHours(3)
                    }
                );

                // Salvar dados complementares na sessão
                _sessao.Cadastrar("NomeUsuario", user.NomeCompleto);
                _sessao.Cadastrar("ID", user.PessoaID.ToString());
                _sessao.Cadastrar("Tipo", user.Tipo.ToString());
                _sessao.Cadastrar("TipoID", user.TipoID.ToString());

                if (user.TipoID <= 4)
                {
                    return Json(new { success = true, redirectUrl = Url.Action("Index", "Menu") });
                }
                else if (user.TipoID == 5)
                {
                    return Json(new { success = true, redirectUrl = Url.Action("Professor", "Menu") });
                }
                else if (user.TipoID == 10)
                {
                    Json(new { success = true, redirectUrl = Url.Action("Cadastro", "Interessado") });
                }
                return Json(new { success = true, redirectUrl = Url.Action("AtendimentosSociais", "Menu") });
            }
            catch (Exception ex)
            {
                return Problem(
                    title: "Erro",
                    detail: ex.Message
                    );
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Home");
        }


        [HttpGet]
        public IActionResult RecuperarSenha()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RecuperarSenha(string email)
        {
            try
            {
                UsuarioModel usuario = await _seUsuario.Obter(email.Trim().ToLower());

                usuario.Senha = KeyGenerator.GetUniqueKey(6);
                await _seUsuario.AtualizarSenha(usuario);
                await _emailServicos.EnviarSenhaPorEmail(false, usuario);

                return Json(new
                {
                    success = true,
                    detail = "Sua nova senha temporária foi enviada para o e-mail cadastrado."
                });
            }
            catch (Exception ex)
            {
                return Problem(
                    title: "Erro",
                    detail: ex.Message,
                    statusCode: StatusCodes.Status500InternalServerError
                    );
            }
        }
    }
}
