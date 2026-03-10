using Microsoft.AspNetCore.Mvc;
using Z1.Model;

namespace UniHog.Views.Shared.Components.DropEstados
{
    public class DropEstudos : ViewComponent
    {
        private readonly IEstudoServicos _estudo;

        public DropEstudos(IEstudoServicos estudo)
        {
            _estudo = estudo;
        }

        public async Task<IViewComponentResult> InvokeAsync(int? selecionado)
        {
            var lst = await _estudo.Listar(null);
            ViewBag.selecionado = selecionado;

            return View("Default", lst);
        }
    }
}
