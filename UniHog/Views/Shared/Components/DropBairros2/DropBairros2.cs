using Microsoft.AspNetCore.Mvc;
using Z1.Model;
using Z2.Servicos;

namespace UniHog.Views.Shared.Components.DropEstados
{
    public class DropBairros2 : ViewComponent
    {
        private readonly IEnderecoServicos _endereco;
        public DropBairros2(IEnderecoServicos endereco)
        {
            _endereco = endereco;
        }
        public async Task<IViewComponentResult> InvokeAsync(int congregacaoId)
        {
            List<BairroModel> lst = await _endereco.ListarBairrosDrop(congregacaoId);

            return View("Default", lst);
        }
    }
}



/*

@await Component.InvokeAsync("DropBairros2", new { congregacaoId = congregacaoId })

*/