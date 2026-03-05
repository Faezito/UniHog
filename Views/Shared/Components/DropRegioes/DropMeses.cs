using Microsoft.AspNetCore.Mvc;
using Z2.Services;

namespace UniHog.Views.Shared.Components.DropRegioes
{
    public class DropRegioes : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View("Default");
        }
    }
}
