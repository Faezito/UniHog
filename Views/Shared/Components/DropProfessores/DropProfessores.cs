using Microsoft.AspNetCore.Mvc;
using Z1.Model;
using Z2.Servicos;

namespace UniHog.Views.Shared.Components.DropEstados
{
    public class DropProfessores : ViewComponent
    {
        private readonly IUsuarioServicos _seUsuario;
        public DropProfessores(IUsuarioServicos seUsuario)
        {
            _seUsuario = seUsuario;
        }
        public async Task<IViewComponentResult> InvokeAsync(int tipoId, int? selecionado)
        {
            UsuarioRQModel usuario = new();
            usuario.TipoID = 5;

            var lst = await _seUsuario.Listar(usuario);
            ViewBag.selecionado = selecionado;

            return View("Default", lst);
        }
    }
}



/*

@await Component.InvokeAsync("DropProfessores", new { tipoId = idprofessor })

*/