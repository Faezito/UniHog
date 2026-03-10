using UniHog.Extensions;
using UniHog.Libraries.Login;
using Microsoft.AspNetCore.Mvc;
using Z1.Model;
using Z2.Servicos;

namespace UniHog.Controllers
{
    [Autorizacoes(Tipos = "-1,1,2,3")]
    public class TipoController : Controller
    {
        private readonly IUsuarioServicos _usuario;
        private readonly IEnderecoServicos _endereco;
        private readonly IMovimentacaoFinanceiraServicos _movimentacaoFinanceira;

        public TipoController(IEnderecoServicos endereco, IUsuarioServicos usuario, IMovimentacaoFinanceiraServicos movimentacaoFinanceira)
        {
            _endereco = endereco;
            _usuario = usuario;
            _movimentacaoFinanceira = movimentacaoFinanceira;
        }

        #region Usuario
        public async Task<IActionResult> Lista(int rotaId)
        {
            try
            {
                var usuario = this.User.ObterUsuario();
                ViewBag.Rota = rotaId;
                List<TiposModel> lst = new List<TiposModel>();
                switch (rotaId)
                {
                    case 1:
                        lst = await _usuario.ListarTipos(usuario.PessoaID.Value);
                        break;
                    case 2:
                        lst = await _endereco.ListarTipos();
                        break;
                    case 3:
                        lst = await _usuario.ListarSituacoes();
                        break;
                    case 4:
                        lst = await _movimentacaoFinanceira.ListarMotivos();
                        break;
                }

                return View(lst);
            }
            catch (Exception ex)
            {
                return Problem(title: "Erro", detail: ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            try
            {
                var situacoes = await _usuario.ListarSituacoes();
                var situacao = situacoes.Where(x => x.ID == id).FirstOrDefault();
                situacao.Selecao = 3;

                return View("Cadastro", situacao);
            }
            catch (Exception ex)
            {
                return Problem(title: "Erro", detail: ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Cadastro(TiposModel model)
        {
            try
            {
                int usuarioId = this.User.GetUserId();

                if (string.IsNullOrWhiteSpace(model.Descricao) || model.ID == 0)
                    return Problem(title: "Erro", detail: "Preencha todos os campos");

                switch (model.Selecao)
                {
                    case 1:
                        var tipoExiste = await _usuario.ObterTipo(model.ID.Value);
                        if (tipoExiste == null)
                            await _usuario.CadastrarTipo(model);
                        else
                            return Problem(title: "Erro", detail: "Este ID já está em uso.");
                        break;
                    case 2:
                        await _endereco.CadastrarTipo(model);
                        break;
                    case 3:
                        await _usuario.CadastroSituacao(model);
                        break;
                    case 4:
                        await _movimentacaoFinanceira.InserirMotivo(model);
                        break;
                }

                TempData["MSG_S"] = "Cadastrado com sucesso!";
                return Json(new { success = true, redirectUrl = Url.Action("Lista", "Tipo", new { rotaId = model.Selecao }) });
            }
            catch (Exception ex)
            {
                return Problem(title: "Erro", detail: ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Deletar(TiposModel model)
        {
            try
            {
                int rota = model.Selecao;

                switch (model.Selecao)
                {
                    case 1:
                        await _usuario.DeletarTipo(model);
                        break;
                    case 4:
                        await _movimentacaoFinanceira.DeletarMotivo(model.ID.Value);
                        break;
                    default:
                        break;
                }

                TempData["MSG_S"] = "Deletado com sucesso!";
                return Json(new { success = true, redirectUrl = Url.Action("Lista", "Tipo", new { rotaId = rota }) });
            }
            catch (Exception ex)
            {
                return Problem(title: "Erro", detail: ex.Message);
            }
        }
        #endregion
    }
}
