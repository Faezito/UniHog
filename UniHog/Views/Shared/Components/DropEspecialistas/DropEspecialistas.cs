using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Z1.Model;
using Z2.Servicos;

namespace UniHog.Views.Shared.Components.DropEspecialistas
{
    public class DropEspecialistas : ViewComponent
    {
        private readonly IEspecialidadeServicos _seEspecialidade;
        private readonly IPessoaJuridicaServicos _pj;

        public DropEspecialistas(IEspecialidadeServicos seEspecialidade, IPessoaJuridicaServicos pj)
        {
            _seEspecialidade = seEspecialidade;
            _pj = pj;
        }
        public async Task<IViewComponentResult> InvokeAsync(int? areaId)
        {
            PessoaJuridicaRQModel model = new PessoaJuridicaRQModel { AreaID = areaId };
            List<PessoaJuridicaModel> especialistas = await _pj.Listar(model);

            return View("Default", especialistas);
        }
    }
}
