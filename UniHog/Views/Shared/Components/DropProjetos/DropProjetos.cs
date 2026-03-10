using Microsoft.AspNetCore.Mvc;
using Z1.Model;
using Z2.Services;

namespace UniHog.Views.Shared.Components.DropAnos
{
    public class DropProjetos : ViewComponent
    {
        private readonly IProjetoServicos _projetos;
        public DropProjetos(IProjetoServicos projetos)
        {
            _projetos = projetos;
        }

        public async Task<IViewComponentResult> InvokeAsync(int? selecionado)
        {
            var lst = await _projetos.Listar(null);
            ViewBag.selecionado = selecionado;
            return View("Default", lst);
        }
    }
}
