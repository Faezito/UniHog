using UniHog.Extensions;
using Microsoft.AspNetCore.Mvc;
using Z1.Model;
using Z2.Services.Externo;
using Z2.Servicos.Externo;

namespace UniHog.Controllers
{
    public class DevToolsController : Controller
    {
        private readonly IDevToolsServicos _dev;
        private readonly IEmailServicos _email;
        private readonly IChamadoServicos _chamado;
        public DevToolsController(IDevToolsServicos dev, IEmailServicos email, IChamadoServicos chamado)
        {
            _dev = dev;  
            _email = email;
            _chamado = chamado;
        }

        public IActionResult Chamado()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Chamado(ChamadoModel model)
        {
            try
            {
                model.usuarioId = this.User.GetUserId();
                model.email = this.User.GetUserEmail();
                model.status = 1;
                model.dataAbertura = DateTime.Now;
                model.DataAlteracao = DateTime.Now;
                model.UsuarioLogado = this.User.GetUserId();

                int? chamado = await _chamado.Inserir(model);

                // TODO: Remover após DevTools Completo
                string usuario = this.User.GetUserName();
                string cargo = this.User.GetUserTipo();

                string assunto = $"BOPE - Novo Chamado (Nro: {chamado}) Tipo: {model.prioridade}";
                string corpo = $@"
Novo chamado do sistema BOPE: <br />
Usuário: {usuario} - {cargo} | {model.email} <br />
Título: {model.titulo} <br />
Texto: {model.descricao} <br />
Urgência: {model.prioridade}
";
                await _email.EnviarEmailAsync("matheusfae@gmail.com", assunto, corpo);
                //await _dev.Inserir(model);
                return Json( new { success = true, detail = "Chamado criado com sucesso. Vamos trabalhar para atendê-lo o mais rápido possível." });
            }
            catch (Exception ex)
            {
                return Problem(title: "Erro", detail: ex.Message);
            }
        }
    }
}
