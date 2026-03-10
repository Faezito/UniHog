using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace UniHog.Views.Shared.Components.DropAnos
{
    public class DropAnos : ViewComponent
    {
        public IViewComponentResult Invoke(int anoInicio, int? anoSelecionado = null)
        {
            List<SelectListItem> lst = [];

            int ano = DateTime.Now.Year;

            for (int i = ano; i >= anoInicio; i--)
            {
                if (anoSelecionado != null)
                {
                    lst.Add(new SelectListItem { Value = i.ToString(), Text = i.ToString(), Selected = (i == anoSelecionado) });
                }
                else
                {
                    lst.Add(new SelectListItem { Value = i.ToString(), Text = i.ToString(), Selected = (ano == i) });
                }
            }

            // se estivermos em novembro soma +1 para compor o ano seguinte
            if (DateTime.Now.Month > 10)
                lst.Add(new SelectListItem { Value = (ano + 1).ToString(), Text = (ano + 1).ToString(), Selected = (ano + 1 == anoSelecionado) });

            lst = lst.OrderByDescending(x => x.Value).ToList();

            return View("Default", lst);
        }
    }
}
