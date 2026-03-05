using Microsoft.AspNetCore.Mvc;
using Z1.Model;
using Z2.Servicos;

namespace UniHog.Views.Shared.Components.DropEstados
{
    public class DropUsuariosTipos : ViewComponent
    {
        private readonly IUsuarioServicos _seUsuario;
        public DropUsuariosTipos(IUsuarioServicos seUsuario)
        {
            _seUsuario = seUsuario;
        }
        public async Task<IViewComponentResult> InvokeAsync(int usuarioId, int? selecionado)
        {
            List<TiposModel> lst = await _seUsuario.ListarTipos(usuarioId);
            ViewBag.selecionado = selecionado;

            return View("Default", lst);
        }
    }
}
