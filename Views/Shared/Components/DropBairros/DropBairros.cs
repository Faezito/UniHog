using Microsoft.AspNetCore.Mvc;
using Z2.Services;
using Z2.Servicos;

namespace UniHog.Views.Shared.Components.DropBairros
{
    public class DropBairros : ViewComponent
    {
        private readonly IEnderecoServicos _endereco;
        public DropBairros(IEnderecoServicos servicos)
        {
            _endereco = servicos;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var lst = await _endereco.Listar();
            string[] bairros = lst.DistinctBy(x=>x.Bairro).Select(x=>x.Bairro).ToArray();
            ViewBag.Bairros = bairros;

            return View("Default");
        }
    }
}
