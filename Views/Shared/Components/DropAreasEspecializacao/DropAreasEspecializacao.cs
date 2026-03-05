using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Z2.Servicos;

namespace UniHog.Views.Shared.Components.DropAreasEspecializacao
{
    public class DropAreasEspecializacao : ViewComponent
    {
        private readonly IEspecialidadeServicos _especializacao;
        public DropAreasEspecializacao(IEspecialidadeServicos especializacao)
        {
            _especializacao = especializacao;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var areas = await _especializacao.ListarAreas();

            List<SelectListItem> lst = new();

            foreach (var a in areas)
            {
                lst.Add(new SelectListItem { Value = a.ID.ToString(), Text = a.Descricao });
            }
            return View("Default", lst);
        }
    }
}
