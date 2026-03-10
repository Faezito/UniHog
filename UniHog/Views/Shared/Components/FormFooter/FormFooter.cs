using Microsoft.AspNetCore.Mvc;

namespace UniHog.Views.Shared.Components.FormFooter
{
    public class FormFooter : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(string? action, string? controller)
        {
            ViewBag.Action = action;
            ViewBag.Controller = controller;

            return View("Default");
        }

        /*
            @await Component.InvokeAsync("FormFooter", new { action = "", controller = "", area = null })
        */
    }
}
