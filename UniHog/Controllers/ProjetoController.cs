using UniHog.Libraries.Login;
using Microsoft.AspNetCore.Mvc;
using Z1.Model;

namespace UniHog.Controllers
{
    [Autorizacoes(Tipos = "-1,1,2,3")]
    public class ProjetoController : Controller
    {
        private readonly IProjetoServicos _projeto;
        public ProjetoController(IProjetoServicos projeto)
        {
            _projeto = projeto;
        }

        public async Task<IActionResult> Index()
        {
            var lst = await _projeto.Listar(null);
            return View(lst);
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            try
            {
                ProjetoModel model = await _projeto.Obter(id);
                return PartialView("_ModalProjeto", model);
            }
            catch (Exception ex)
            {
                return Problem(title: "Erro", detail: ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Cadastro(ProjetoModel model)
        {
            try
            {
                await _projeto.Cadastro(model);
                return Json(new { success = true, redirectUrl = Url.Action("Index", "Projeto") });
            }
            catch (Exception ex)
            {
                return Problem(title: "Erro", detail: ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Deletar(ProjetoModel model)
        {
            try
            {
                await _projeto.Deletar(model);

                TempData["MSG_S"] = "Deletado com sucesso!";
                return Ok(new { success = true, redirectUrl = Url.Action("Index", "Projeto") });
            }
            catch (Exception ex)
            {
                return Problem(title: "Erro", detail: ex.Message);
            }
        }
    }
}
