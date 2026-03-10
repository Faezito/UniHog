using Microsoft.AspNetCore.Mvc;
using Z2.Services;

namespace UniHog.Views.Shared.Components.DropEstados
{
    public class DropEstados : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View("Default");
        }
    }
}
