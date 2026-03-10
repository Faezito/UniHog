using UniHog.Extensions;
using UniHog.Libraries.Login;
using Microsoft.AspNetCore.Mvc;
using Z1.Model;
using Z2.Services.Externo;
using Z2.Servicos;

namespace UniHog.Controllers
{
    [Autorizacoes(Tipos = "-1,1,2,3")]
    public class CestaBasicaController : Controller
    {
        private readonly ICestaBasicaServicos _cestaBasicaSE;
        private readonly IEmailServicos _email;
        public CestaBasicaController(ICestaBasicaServicos cestaBasicaSE, IEmailServicos email)
        {
            _cestaBasicaSE = cestaBasicaSE;
            _email = email;
        }

        public async Task<IActionResult> Index()
        {
            List<CestaBasicaModel> lst = await _cestaBasicaSE.Listar();
            return View(lst);
        }

        [HttpPost] // POST Porque é modal
        public async Task<IActionResult> Editar(int? id)
        {
            try
            {
                CestaBasicaModel model = await _cestaBasicaSE.Obter(id.Value);
                return PartialView("_modalCesta", model);
            }
            catch (Exception ex)
            {
                return Problem(title: "Erro", detail: ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Cadastro(CestaBasicaModel model)
        {
            try
            {
                model.DataAlteracao = DateTime.Now;
                model.UsuarioLogado = this.User.GetUserId();
                model.UsuarioAlteracao = this.User.GetUserName();

                if (!ModelState.IsValid)
                {
                    return BadRequest("O campo descrição não pode ser nulo");
                }

                await _cestaBasicaSE.Cadastro(model);
                return Json(new { success = true, redirectUrl = Url.Action("Index", "CestaBasica") });
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpPost] // TODO: IMPLEMENTAR
        public async Task<IActionResult> AdicionarEstoque(CestaBasicaModel model)
        {
            try
            {
                model.DataAlteracao = DateTime.Now;
                model.UsuarioLogado = this.User.GetUserId();

                await _cestaBasicaSE.AdicionarEstoque(model);
                return Json(new { success = true, redirectUrl = Url.Action("Index", "CestaBasica") });
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }


        [HttpGet]
        public async Task<IActionResult> Listagem()
        {
            try
            {
                List<CestaBasicaBeneficiarioModel> lst = await _cestaBasicaSE.ListarBeneficiarios(null, false);

                return View(lst);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> CadastroEntrega(int id)
        {
            try
            {
                CestaBasicaBeneficiarioModel pessoa = await _cestaBasicaSE.ObterBeneficiario(id);
                List<CestaBasicaModel> cestas = await _cestaBasicaSE.Listar();

                ViewBag.cestas = cestas;
                return PartialView("_modalCadastroEntrega", pessoa);
            }
            catch (Exception ex)
            {
                return Problem(title: "Erro", detail: ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CadastroEntrega(CestaEntregaModel model)
        {
            try
            {
                model.DataAlteracao = DateTime.Now;
                model.UsuarioLogado = this.User.GetUserId();

                await _cestaBasicaSE.RegistrarSaida(model);
                var cesta = await _cestaBasicaSE.Obter(model.CestaID.Value);

                CestaEntregaModel entrega = new CestaEntregaModel
                {
                    Entregador = this.User.GetUserName(),
                    Cesta = cesta.Descricao,
                    Quantidade = cesta.Quantidade
                };

                await _email.EnviarCestaPorEmail(entrega);
                return Json(new { success = true, redirectUrl = Url.Action("Listagem", "CestaBasica") });
            }
            catch (Exception ex)
            {
                return Problem(title: "Erro", detail: ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeletarCesta(int id)
        {
            try
            {
                await _cestaBasicaSE.DeletarCesta(id);
                return Json(new { success = true, redirectUrl = Url.Action("Index", "CestaBasica"), detail = "Deletado com sucesso!" });
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }
    }
}
