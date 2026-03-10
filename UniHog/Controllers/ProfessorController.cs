using UniHog.Extensions;
using UniHog.Libraries.Login;
using Microsoft.AspNetCore.Mvc;
using System.Transactions;
using Z1.Model;
using Z2.Services.Externo;
using Z2.Servicos;
using Z4.Bibliotecas;

namespace UniHog.Controllers
{
    [Autorizacoes(Tipos = "-1, 1, 2, 3, 4, 5")]
    public class ProfessorController : Controller
    {
        private readonly IProfessorServicos _prof;
        private readonly IUsuarioServicos _usuario;
        private readonly IInteressadoServicos _interessado;
        private readonly IAtendimentoServicos _atendimentos;
        private readonly IEmailServicos _email;

        public ProfessorController(IProfessorServicos prof,
            IUsuarioServicos usuario,
            IInteressadoServicos interessado,
            IAtendimentoServicos atendimentos,
            IEmailServicos email)
        {
            _prof = prof;
            _usuario = usuario;
            _interessado = interessado;
            _atendimentos = atendimentos;
            _email = email;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Agenda()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Agenda(UsuarioRQModel model)
        {
            try
            {
                model.TipoID = 20;

                List<UsuarioModel> alunos = await _usuario.Listar(model);
                ViewBag.diasSemana = await _atendimentos.ListarDiasDaSemana();

                return PartialView("_Calendario", alunos);
            }
            catch (Exception ex)
            {
                return Problem(title: "Erro", detail: ex.Message);
            }
        }


        [HttpPost]
        public async Task<IActionResult> Pesquisar(int PessoaID)
        {
            try
            {
                var interessado = await _interessado.Obter(PessoaID);
                return PartialView("_Form", interessado);
            }
            catch (Exception ex)
            {
                return Problem(title: "Erro", detail: ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Chamada(int diaId)
        {
            try
            {
                UsuarioRQModel model = new();

                if (this.User.GetUserTipoId() >= 4)
                {
                    model.ProfessorID = this.User.GetUserId();
                }

                model.TipoID = 20;
                model.EstudaEmCasa = false;

                DateTime hoje = DateTime.Now;
                DayOfWeek diaSemana = (DayOfWeek)(diaId - 1);

                // Pesquisar alunos, filtrando pelo dia de estudo e removendo quem estuda pelo Whatsapp
                List<UsuarioModel> lst = await _usuario.Listar(model);
                lst = lst.Where(x => x.DiasEstudoIDs?.Contains(diaId) == true && x.EstudoID != 6).OrderBy(x => x.NomeCompleto).ToList();

                var datas = ManipularModels.ObterDiasDaSemanaNoMes(hoje.Year, hoje.Month, diaSemana);
                datas = datas.Where(x => x.Date >= hoje.AddDays(-3)).ToList();

                var dias = await _atendimentos.ListarDiasDaSemana();

                ViewBag.datas = datas;
                ViewBag.dia = dias.Where(x => x.DiaID == diaId).FirstOrDefault();

                return View(lst);
            }
            catch (Exception ex)
            {
                return Problem(title: "Erro", detail: ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Chamada(List<ChamadaCadastroModel> model, int NumLicao)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    List<string> _alunos = new List<string>();

                    foreach (var item in model)
                    {
                        item.Licao = NumLicao;
                        item.DataEstudo = item.Horario.HasValue
                            ? item.DataEstudo.Date + item.Horario.Value.ToTimeSpan()
                            : item.DataEstudo.Date.AddHours(7);

                        item.DataCriacao = DateTime.Now;
                        item.UsuarioLogado = this.User.GetUserId();

                        if (item.Presente == true)
                            _alunos.Add(item.Aluno);

                        await _prof.AdicionarChamada(item);
                    }

                    if (model.Count > 0)
                    {
                        ChamadaEmailModel chamada = new ChamadaEmailModel()
                        {
                            Professor = this.User.GetUserName(),
                            Estudo = model.FirstOrDefault()?.Estudo,
                            LicaoID = model.FirstOrDefault()?.Licao,
                            Alunos = string.Join(", ", _alunos) + "(Presentes)",
                            DataEstudo = model.FirstOrDefault()?.DataEstudo
                        };

                        await _email.EnviarChamadaPorEmail(chamada);
                    }

                    scope.Complete();
                    return Json(new { success = true, redirectUrl = Url.Action(nameof(Agenda)) });
                }
                catch (Exception ex)
                {
                    scope.Dispose();
                    return Problem(title: "Erro", detail: ex.Message);
                }
            }
        }

        [HttpGet]
        public IActionResult ChamadaEmCasa()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ListarAlunos(UsuarioRQModel model)
        {
            try
            {
                model.SituacaoID = 4;
                model.EstudaEmCasa = true;

                List<UsuarioModel> lst = await _usuario.Listar(model);

                return PartialView("_TabelaAlunos", lst);
            }
            catch (Exception ex)
            {
                return Problem(title: "Erro", detail: ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Formulario(int id)
        {
            try
            {
                UsuarioModel model = await _usuario.Obter(id, null);
                ViewBag.aluno = model;
                return View();
            }
            catch (Exception ex)
            {
                return Problem(title: "Erro", detail: ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Formulario(ChamadaCadastroModel model)
        {
            try
            {
                var aluno = await _usuario.Obter(model.PessoaID, null);

                model.DataEstudo = model.Horario.HasValue
                    ? model.DataEstudo.Date + model.Horario.Value.ToTimeSpan()
                    : model.DataEstudo.Date.AddHours(7);

                model.DataCriacao = DateTime.Now;
                model.UsuarioLogado = this.User.GetUserId();

                ChamadaEmailModel chamada = new ChamadaEmailModel()
                {
                    Professor = this.User.GetUserName(),
                    Estudo = model.Estudo,
                    LicaoID = model.Licao,
                    Alunos = $"{aluno.NomeCompleto} ({(model.Presente == true ? "Presente" : "Ausente")})",
                    DataEstudo = model.DataEstudo
                };

                await _prof.AdicionarChamada(model);
                await _email.EnviarChamadaPorEmail(chamada);

                return Json(new { success = true, detail = "Cadastrado com sucesso!", redirectUrl = Url.Action(nameof(ChamadaEmCasa)) });
            }
            catch (Exception ex)
            {
                return Problem(title: "Erro", detail: ex.Message);
            }
        }


        [HttpGet]
        public IActionResult Presencas()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ListarPresencas(ChamadaRQModel model)
        {
            try
            {
                var lst = await _prof.ListarPresenca(model);

                List<DateTime?> dias = lst.OrderBy(x => x.DataEstudo).Select(x => x.DataEstudo).Distinct().ToList();
                var alunos = lst.GroupBy(x => x.PessoaID)
                                    .Select(g => new AlunoModel
                                    {
                                        PessoaID = g.Key,
                                        NomeCompleto = g.First().Aluno,
                                        FotoPerfilURL = g.First().FotoPerfilURL,
                                        Estudo = g.First().Estudo
                                    })
                                    .ToList();

                ViewBag.Alunos = alunos;
                ViewBag.Dias = dias;

                return PartialView("_TabelaPresencas", lst);
            }
            catch (Exception ex)
            {
                return Problem(title: "Erro", detail: ex.Message);
            }
        }

    }
}
