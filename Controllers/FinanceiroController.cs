using UniHog.Extensions;
using UniHog.Libraries.Login;
using Microsoft.AspNetCore.Mvc;
using Z1.Model;
using Z2.Servicos;

namespace UniHog.Controllers
{
    [Autorizacoes]
    public class FinanceiroController : Controller
    {
        private readonly IMovimentacaoFinanceiraServicos _seMov;
        private readonly ICloudinaryServicos _cloudinary;
        public FinanceiroController(IMovimentacaoFinanceiraServicos seMov, ICloudinaryServicos cloudinary)
        {
            _seMov = seMov;
            _cloudinary = cloudinary;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult MovimentacoesFinanceiras()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> NotaFiscal(int id)
        {
            try
            {
                var movimentacao = await _seMov.Obter(id);
                return PartialView("_modalNotaFiscal", movimentacao);
            }
            catch (Exception ex)
            {
                return Problem(title: "Erro", detail: ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> MovimentacoesFinanceiras(int mes, int ano, string? tipo)
        {
            try
            {
                var lst = await _seMov.Listar(mes, ano, tipo);
                return PartialView("_Movimentacoes", lst);
            }
            catch (Exception ex)
            {
                return Problem(title: "Erro", detail: ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> CadastroMovimentacao()
        {
            var motivos = await _seMov.ListarMotivos();
            ViewBag.motivos = motivos;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> NovaMovimentacao(MovimentacaoFinanceiraModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var mensagens = string.Join("<br />", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                    return Problem(detail: mensagens, title: "Erro");
                }

                if (model.Tipo == "S" && model.MotivoID == null)
                {
                    return Problem(detail: "Selecione um Motivo", title: "Erro");
                }

                if(model.Tipo == "E")
                {
                    model.MotivoID = null;
                }

                (string? url, string? pubicId) url = (string.Empty, string.Empty);
                string id = Guid.NewGuid().ToString();

                if (model.NotaFiscal != null)
                {
                    url = await _cloudinary.UploadNotaFiscal(model.NotaFiscal, id);
                    model.NotaFiscalURL = url.url;
                    model.NotaFiscalPublicID = url.pubicId;
                }

                model.UsuarioLogado = this.User.GetUserId();
                model.DataAlteracao = DateTime.Now;

                await _seMov.Inserir(model);
                return Json(new { success = true, message = "Cadastrado com sucesso!", redirectUrl = Url.Action("MovimentacoesFinanceiras", "Financeiro") });
            }
            catch (Exception ex)
            {
                if (model.NotaFiscalPublicID != null)
                    await _cloudinary.DeletarImagem(model.NotaFiscalPublicID);
                return Problem(title: "Erro", detail: ex.Message);
            }
        }


    }
}
