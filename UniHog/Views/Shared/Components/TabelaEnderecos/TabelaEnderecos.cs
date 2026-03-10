using Microsoft.AspNetCore.Mvc;
using Z2.Servicos;

namespace UniHog.Views.Shared.Components.TabelaEnderecos
{
    public class TabelaEnderecos : ViewComponent
    {
        private readonly IEnderecoServicos _seEndereco;
        private readonly IUsuarioServicos _seUsuario;
        public TabelaEnderecos(IEnderecoServicos seEndereco, IUsuarioServicos seUsuario)
        {
            _seEndereco = seEndereco;
            _seUsuario = seUsuario;
        }
        public async Task<IViewComponentResult> InvokeAsync(int pessoaId, bool podeExcluir)
        {
            var lst = await _seEndereco.ListarEnderecosDoUsuario(pessoaId);
            ViewBag.Deletar = podeExcluir;
            return View("Default", lst);
        }
        /*
                @await Component.InvokeAsync("TabelaEnderecos", new { pessoaId = pessoaId, podeExcluir = false })
         */
    }
}
