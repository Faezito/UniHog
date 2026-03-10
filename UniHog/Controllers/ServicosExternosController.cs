using UniHog.Libraries.Login;
using Microsoft.AspNetCore.Mvc;
using Z1.Model.APIs;
using Z1.Model.Email;
using Z2.Services.Externo;

namespace UniHog.Controllers
{
    [Autorizacoes(Tipos = "-1")]
    public class ServicosExternosController : Controller
    {
        private readonly IAPIsServicos _api;
        private readonly IEmailServicos _email;

        public ServicosExternosController(IAPIsServicos api, IEmailServicos email)
        {
            _api = api;
            _email = email;
        }

        public async Task<IActionResult> ListarAPIs()
        {
            try
            {
                var lst = await _api.Listar(null);
                return View(lst);
            }
            catch (Exception ex)
            {
                return Problem(title: "Erro", detail: ex.Message);
            }

        }

        [HttpGet]
        public async Task<IActionResult> CadastroAPI(int? apiID)
        {
            try
            {
                APIModel api = new();
                if (apiID.HasValue)
                    api = await _api.Obter(apiID.Value, null);

                return View(api);
            }
            catch (Exception ex)
            {
                return Problem(title: "Erro", detail: ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CadastroAPI(APIModel model)
        {
            try
            {
                if (model.COD == 0 || string.IsNullOrWhiteSpace(model.Descricao))
                    return BadRequest("Preencha todos os campos!");

                await _api.Cadastro(model);
                //TempData["MSG_S"] = "Cadastrado com sucesso!";
                return Ok(new { success = true, redirectUrl = Url.Action("ListarAPIs", "ServicosExternos") });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public async Task<IActionResult> ListarEmails()
        {
            try
            {
                var lst = await _email.Listar(null);
                return View(lst);
            }
            catch (Exception ex)
            {
                return Problem(title: "Erro", detail: ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> CadastroEmail(int? emailID)
        {
            try
            {
                EmailConfig api = new();
                if (emailID.HasValue)
                    api = await _email.Obter(emailID.Value);

                return View(api);
            }
            catch (Exception ex)
            {
                return Problem(title: "Erro", detail: ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CadastroEmail(EmailConfig model)
        {
            try
            {
                await _email.Cadastro(model);
                return Ok( new { success = true, detail = "Cadastrado com sucesso!" });
            }
            catch (Exception ex)
            {
                return Problem(title: "Erro", detail: ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Deletar(int id, string tipo)
        {
            try
            {
                switch (tipo)
                {
                    case "api":
                        await _api.Deletar(id);
                        TempData["MSG_S"] = "Deletado com sucesso!";
                        return RedirectToAction(nameof(ListarAPIs));
                    case "email":
                        await _email.Deletar(id);
                        TempData["MSG_S"] = "Deletado com sucesso!";
                        return RedirectToAction(nameof(ListarEmails));
                    default:
                        throw new Exception("Erro ao designar exclusão, contate um administrador do sistema.");
                }

            }
            catch (Exception ex)
            {
                return Problem(title: "Erro", detail: ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> TestarSMTP(string destinatarioTeste)
        {
            try
            {
                await _email.EnviarEmailAsync(destinatarioTeste, "Teste", "E-mail enviado com sucesso!");
                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message);
            }
        }
    }
}
