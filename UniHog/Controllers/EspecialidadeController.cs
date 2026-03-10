using UniHog.Libraries.Login;
using Microsoft.AspNetCore.Mvc;
using Z1.Model;
using Z2.Servicos;

namespace UniHog.Controllers
{
    [Autorizacoes]
    public class EspecialidadeController : Controller
    {
        private readonly IEspecialidadeServicos _seEspecialidade;
        public EspecialidadeController(IEspecialidadeServicos seEspecialidade)
        {
            _seEspecialidade = seEspecialidade;
        }

        public async Task<IActionResult> Index()
        {
            EspecialidadeModel model = new();
            var lst = await _seEspecialidade.Listar(model);
            return View(lst);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(int id)
        {
            try
            {
                EspecialidadeModel model = await _seEspecialidade.Obter(id);
                return PartialView("_ModalEspecialidade", model);
            }
            catch (Exception ex)
            {
                return Problem(title: "Erro", detail: ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Cadastro(EspecialidadeModel model)
        {
            try
            {
                await _seEspecialidade.Cadastro(model);
                return Json(new { success = true, redirectUrl = Url.Action("Index", "Especialidade") });
            }
            catch (Exception ex)
            {
                return Problem(title: "Erro", detail: ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Deletar(EspecialidadeModel model)
        {
            try
            {
                await _seEspecialidade.Deletar(model);

                TempData["MSG_S"] = "Deletado com sucesso!";
                return Ok(new { success = true, redirectUrl = Url.Action("Index", "Especialidade") });
            }
            catch (Exception ex)
            {
                return Problem(title: "Erro", detail: ex.Message);
            }
        }

    }
}
