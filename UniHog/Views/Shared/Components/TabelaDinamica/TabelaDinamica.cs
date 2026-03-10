using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace BI_Gowinz.Views.Shared.Components.TabelaDinamica
{
    public class TabelaDinamica : ViewComponent
    {
        public IViewComponentResult Invoke(List<dynamic> lst)
        {
            try
            {
                return View("Default", lst);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}