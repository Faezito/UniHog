using UniHog.Extensions;
using UniHog.Libraries.Login;
using Microsoft.AspNetCore.Mvc;
using Z1.Model;
using Z2.Servicos;

namespace UniHog.Controllers
{
    [Autorizacoes]
    public class AtendimentoController : Controller
    {
        private readonly IUsuarioServicos _seUsuario;
        private readonly IAtendimentoServicos _seAtendimento;
        private readonly IEspecialidadeServicos _seEspecialidade;
        public AtendimentoController(IUsuarioServicos seUsuario, IAtendimentoServicos seAtendimento, IEspecialidadeServicos seEspecialidade)
        {
            _seUsuario = seUsuario;
            _seAtendimento = seAtendimento;
            _seEspecialidade = seEspecialidade;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int id)
        {
            try
            {
                var lst = await _seAtendimento.ListarAreas();
                TiposModel area = lst.Where(x => x.ID == id).SingleOrDefault();

                return View(area);
            }
            catch (Exception ex)
            {
                return Problem(title: "Erro", detail: ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Areas()
        {
            try
            {
                var lst = await _seAtendimento.ListarAreas();
                return View(lst);
            }
            catch (Exception ex)
            {
                return Problem(title: "Erro", detail: ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditarArea(int? id)
        {
            try
            {
                TiposModel model = new();
                var lst = await _seAtendimento.ListarAreas();
                model = lst.Where(x => x.ID == id).FirstOrDefault();

                return PartialView("_modalArea", model);
            }
            catch (Exception ex)
            {
                return Problem(title: "Erro", detail: ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CadastroArea(TiposModel model)
        {
            try
            {
                await _seEspecialidade.Cadastro(model);
                return Ok(new { success = true, redirectUrl = Url.Action("Areas", "Atendimento") });
            }
            catch (Exception ex)
            {
                return Problem(title: "Erro", detail: ex.Message);
            }
        }


        public IActionResult Agendamento(int id)
        {
            try
            {
                ViewBag.area = id;
                return View();
            }
            catch (Exception ex)
            {
                return Problem(title: "Erro", detail: ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Pesquisar(UsuarioRQModel model, int areaId, int pagina = 1, int quantidade = 5)
        {
            try
            {
                int usuarioTipoID = this.User.GetUserTipoId();
                ViewBag.area = areaId;

                model.UsuarioTipoID = usuarioTipoID;
                model.TipoID = 20;
                PaginacaoModel<UsuarioModel> usuarios = await _seUsuario.ListarPaginado(model, pagina, quantidade);

                return PartialView("_tabelaPessoas", usuarios);
            }
            catch (Exception ex)
            {
                return Problem(title: "Erro", detail: ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> NovoAgendamento(int pessoaId, int areaId)
        {
            try
            {
                UsuarioModel pessoa = await _seUsuario.Obter(pessoaId, null);
                ViewBag.areaID = areaId;

                return View(pessoa);
            }
            catch (Exception ex)
            {
                return Problem(title: "Erro", detail: ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Agendar(AtendimentoModel model)
        {
            try
            {
                model.DataCriacao = DateTime.Now;
                model.UsuarioLogado = this.User.GetUserId();

                if (model.DataAtendimento < DateTime.Now)
                    throw new Exception("Data inválida.");

                await _seAtendimento.Cadastrar(model);
                return Json(new { success = true, redirectUrl = Url.Action("Agendamento", "Atendimento", new { id = model.AreaID }) });
            }
            catch (Exception ex)
            {
                return Problem(title: "Erro", detail: ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> TabelaFuncionamento(int? especialistaId, int? empresaId)
        {
            try
            {
                List<Funcionamento> funcionamentos = await _seAtendimento.ListarFuncionamento(
                    new AtendimentoModel
                    {
                        EspecialistaID = especialistaId.Value
                    });
                List<DiasDaSemana> diasSemana = await _seAtendimento.ListarDiasDaSemana();

                return PartialView("_tabelaFuncionamento", funcionamentos);
            }
            catch (Exception ex)
            {
                return Problem(title: "Erro", detail: ex.Message);
            }
        }

        [HttpGet]
        public IActionResult Agenda(int id)
        {
            ViewBag.rotaId = id;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> PesquisarAgenda(AtendimentoRQModel pesquisa)
        {
            if (pesquisa.DataIni == null || pesquisa.DataFim == null)
            {
                pesquisa.Data = DateTime.Now;
            }

            List<AtendimentoModel> atendimentos = await _seAtendimento.ListarAtendimentos(pesquisa);
            List<DiasDaSemana> diasSemana = await _seAtendimento.ListarDiasDaSemana();

            ViewBag.areaId = pesquisa.AreaID;
            ViewBag.diasSemana = diasSemana;

            return PartialView("_tabelaAgenda", atendimentos);
        }
    }
}
