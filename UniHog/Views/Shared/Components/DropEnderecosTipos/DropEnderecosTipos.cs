using Microsoft.AspNetCore.Mvc;
using Z1.Model;
using Z2.Servicos;

namespace UniHog.Views.Shared.Components.DropEstados
{
    public class DropEnderecosTipos : ViewComponent
    {
        private readonly IEnderecoServicos _seEndereco;
        public DropEnderecosTipos(IEnderecoServicos seEndereco)
        {
            _seEndereco = seEndereco;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            List<TiposModel> lst = await _seEndereco.ListarTipos();

            return View("Default", lst);
        }
    }
}
