using UniHog.Libraries.Login;
using Microsoft.AspNetCore.Mvc;
using Z1.Model;

namespace UniHog.Controllers
{
    [Autorizacoes]
    public class EstudoController : Controller
    {
        private readonly IEstudoServicos _Estudo;
        public EstudoController(IEstudoServicos Estudo)
        {
            _Estudo = Estudo;
        }

        public async Task<IActionResult> Index()
        {
            var lst = await _Estudo.Listar(null);
            return View(lst);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(int id)
        {
            try
            {
                EstudoModel model = await _Estudo.Obter(id);
                return PartialView("_ModalEstudo", model);
            }
            catch (Exception ex)
            {
                return Problem(title: "Erro", detail: ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Cadastro(EstudoModel model)
        {
            try
            {
                await _Estudo.Cadastro(model);
                return Json(new { success = true, redirectUrl = Url.Action("Index", "Estudo") });
            }
            catch (Exception ex)
            {
                return Problem(title: "Erro", detail: ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Deletar(int id)
        {
            try
            {
                EstudoModel model = new();
                model.ESTUDOID = id;
                await _Estudo.Deletar(model);

                TempData["MSG_S"] = "Deletado com sucesso!";
                return Ok(new { success = true, redirectUrl = Url.Action("Index", "Estudo") });
            }
            catch (Exception ex)
            {
                return Problem(title: "Erro", detail: ex.Message);
            }
        }
    }
}

