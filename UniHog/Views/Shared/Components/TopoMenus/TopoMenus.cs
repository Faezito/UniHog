using Microsoft.AspNetCore.Mvc;

namespace UniHog.Views.Shared.Components.TopoMenus
{
    public class TopoMenus : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(string menu, bool? ajuda, string? rota)
        {
            ViewBag.Menu = menu;
            ViewBag.Rota = rota;

            ajuda = ajuda ?? false;
            ViewBag.Ajuda = ajuda;

            return View("Default");
        }
    }
}

/*
 await Component.InvokeAsync("TopoMenus", new { menu = "Relatórios", ajuda = false, rota = "@Url.Action("","") })
 */
