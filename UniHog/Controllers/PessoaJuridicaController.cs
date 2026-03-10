using UniHog.Extensions;
using UniHog.Libraries.Login;
using Microsoft.AspNetCore.Mvc;
using Z1.Model;
using Z2.Servicos;
using Z4.Bibliotecas;

namespace UniHog.Controllers
{
    [Autorizacoes]
    public class PessoaJuridicaController : Controller
    {
        private readonly IPessoaJuridicaServicos _pj;
        private readonly IEnderecoServicos _enderecos;
        private readonly IUsuarioServicos _usuarios;
        private readonly IEspecialidadeServicos _especialidade;

        public PessoaJuridicaController(IPessoaJuridicaServicos pj, IEnderecoServicos endereco, IUsuarioServicos usuarios, IEspecialidadeServicos especialidade)
        {
            _pj = pj;
            _enderecos = endereco;
            _usuarios = usuarios;
            _especialidade = especialidade;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Listar(PessoaJuridicaRQModel model)
        {
            try
            {
                var lst = await _pj.Listar(model);

                return PartialView("_TabelaEmpresas", lst);
            }
            catch (Exception ex)
            {
                return Problem(title: "Erro", detail: ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Cadastro(int? id)
        {
            try
            {
                PessoaJuridicaModel empresa = new();

                if (id.HasValue)
                {
                    UsuarioModel model = await _usuarios.Obter(id, null);
                    List<PessoaJuridicaModel> empresas = await _pj.Listar(new PessoaJuridicaRQModel { PessoaID = model.PessoaID });
                    empresa = empresas.FirstOrDefault() ?? new();
                }

                return View(empresa);
            }
            catch (Exception ex)
            {
                return Problem(title: "Erro", detail: ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int pessoajuridicaId)
        {
            try
            {
                PessoaJuridicaModel empresa = await _pj.Obter(pessoajuridicaId);
                return View("Cadastro", empresa);
            }
            catch (Exception ex)
            {
                return Problem(title: "Erro", detail: ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Cadastro(PessoaJuridicaModel model)
        {
            try
            {
                EspecialidadeModel area = await _especialidade.Obter(model.EspecialidadeID.Value);
                model.AreaID = area.AreaID;
                model.UsuarioLogado = this.User.GetUserId();
                model.DataAlteracao = DateTime.Now;

                var validarEmpresa = Validacoes.ValidarEmpresa(model);

                if (!string.IsNullOrWhiteSpace(model.CNPJ))
                    model.CNPJ = ManipularModels.LimparNumeros(model.CNPJ);

                await _pj.Cadastro(model);
                return Json(new { success = true, redirectUrl = Url.Action("Cadastros", "Menu") });
            }
            catch (Exception ex)
            {
                return Problem(title: "Erro", detail: ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Detalhes(int id)
        {
            try
            {
                PessoaJuridicaModel model = await _pj.Obter(id);
                return PartialView("_modalEmpresa", model);
            }
            catch (Exception ex)
            {
                return Problem(title: "Erro", detail: ex.Message);
            }
        }

    }
}
