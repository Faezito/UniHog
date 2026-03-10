using UniHog.Libraries.Login;
using UniHog.Libraries.Sessao;
using Microsoft.AspNetCore.Mvc;
using Z1.Model;
using Z2.Servicos;
using Z2.Servicos.Externo;

namespace UniHog.Controllers
{
    [Autorizacoes]
    public class MenuController : Controller
    {
        private readonly IEspecialidadeServicos _especialidade;
        private readonly IUsuarioServicos _usuario;
        private readonly IDevToolsServicos _dev;
        private readonly Sessao _sessao;
        public MenuController(IEspecialidadeServicos especialidade, IDevToolsServicos dev, IUsuarioServicos usuario, Sessao sessao)
        {
            _especialidade = especialidade;
            _dev = dev;
            _usuario = usuario;
            _sessao = sessao;
        }

        [Autorizacoes]
        public async Task<IActionResult> Index()
        {
            List<UsuarioModel> aniversariantes = await _usuario.ListarAniversariantes();
            ViewBag.aniversariantes = aniversariantes;

            //var notas = await _dev.Listar();
            //string titulo = notas.Select(x => x.titulo).FirstOrDefault();
            //string texto = notas.Select(x => x.texto).FirstOrDefault();

            return View();
        }

        [Autorizacoes(Tipos = "-1,1,2,3,4")]
        public IActionResult Cadastros()
        {
            return View();
        }

        [Autorizacoes(Tipos = "-1,1,2,3,4")]
        public IActionResult CestaBasica()
        {
            return View();
        }

        [Autorizacoes(Tipos = "-1,1,2,3,4")]
        public IActionResult Relatorios()
        {
            return View();
        }

        [Autorizacoes(Tipos = "-1,1,2,3,4,5")]
        public IActionResult Professor()
        {
            return View();
        }

        [Autorizacoes(Tipos = "-1,1,2,3,4,15,16")]
        public async Task<IActionResult> AtendimentosSociais()
        {
            var lst = await _especialidade.ListarAreas();
            return View(lst);
        }

        [Autorizacoes(Tipos = "-1")]
        public IActionResult Configuracoes()
        {
            return View();
        }
    }
}
