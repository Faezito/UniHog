using UniHog.Libraries.Login;
using Microsoft.AspNetCore.Mvc;

namespace UniHog.Controllers
{
    [Autorizacoes(Tipos = ("-1"))]
    public class ConfiguracoesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
