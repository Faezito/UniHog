using Microsoft.AspNetCore.Mvc;
using Z2.Servicos;
using Z2.Services.Externo;
using UniHog.Libraries.Login;

namespace UniHog.Controllers
{
    [Autorizacoes(Tipos = "1,-1")]
    public class AdminController : Controller
    {
        private readonly IUsuarioServicos _seUsuario;
        private readonly IEmailServicos _emailServicos;

        public AdminController(IUsuarioServicos seUsuario, IEmailServicos emailServicos)
        {
            _seUsuario = seUsuario;
            _emailServicos = emailServicos;
        }

        public IActionResult Index()
        {
            return View();
        }

        #region Usuarios
        [HttpGet]
        public IActionResult MenuCadastros()
        {
            return View();
        }

        public ActionResult Usuarios(int? rotaId)
        {
            ViewBag.rotaId = rotaId;
            return View();
        }


        #endregion
    }
}
