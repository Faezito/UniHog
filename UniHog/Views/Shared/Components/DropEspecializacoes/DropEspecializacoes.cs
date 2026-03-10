using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Z1.Model;
using Z2.Servicos;

namespace UniHog.Views.Shared.Components.DropEspecializacoes
{
    public class DropEspecializacoes : ViewComponent
    {
        private readonly IEspecialidadeServicos _seEspecialidade;
        public DropEspecializacoes(IEspecialidadeServicos seEspecialidade)
        {
            _seEspecialidade = seEspecialidade;
        }
        public async Task<IViewComponentResult> InvokeAsync(int? areaId)
        {
            List<EspecialidadeModel> especialidades = await _seEspecialidade.Listar(new EspecialidadeModel { AreaID = areaId });
            ViewBag.Areas = especialidades.Select(x=>x.Area).Distinct().ToList();

            return View("Default", especialidades);
        }
    }
}
