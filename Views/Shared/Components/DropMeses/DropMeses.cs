using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace UniHog.Views.Shared.Components.DropMeses
{
    public class DropMeses : ViewComponent
    {
        public IViewComponentResult Invoke(string mes = null)
        {
            List<SelectListItem> lst = new();

            if (string.IsNullOrWhiteSpace(mes))
            {
                mes = DateTime.Now.Month.ToString();
            }

            lst.Add(new SelectListItem { Value = "1", Text = "Janeiro", Selected = mes == "1" });
            lst.Add(new SelectListItem { Value = "2", Text = "Fevereiro", Selected = mes == "2" });
            lst.Add(new SelectListItem { Value = "3", Text = "Março", Selected = mes == "3" });
            lst.Add(new SelectListItem { Value = "4", Text = "Abril", Selected = mes == "4" });
            lst.Add(new SelectListItem { Value = "5", Text = "Maio", Selected = mes == "5" });
            lst.Add(new SelectListItem { Value = "6", Text = "Junho", Selected = mes == "6" });
            lst.Add(new SelectListItem { Value = "7", Text = "Julho", Selected = mes == "7" });
            lst.Add(new SelectListItem { Value = "8", Text = "Agosto", Selected = mes == "8" });
            lst.Add(new SelectListItem { Value = "9", Text = "Setembro", Selected = mes == "9" });
            lst.Add(new SelectListItem { Value = "10", Text = "Outubro", Selected = mes == "10" });
            lst.Add(new SelectListItem { Value = "11", Text = "Novembro", Selected = mes == "11" });
            lst.Add(new SelectListItem { Value = "12", Text = "Dezembro", Selected = mes == "12" });

            return View("Default", lst);
        }
    }
}
