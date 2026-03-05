using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lugano_Erbe.Views.Shared.Components.Tabela_NenhumRegistro
{
    public class TabelaSemRegistros : ViewComponent
    {

        //@await Component.InvokeAsync("TabelaSemRegistros")

        public IViewComponentResult Invoke()
        {
            try
            {
                return View("Default");
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
