using Microsoft.AspNetCore.Mvc;
using Z1.Model;
using Z2.Servicos;

namespace UniHog.Views.Shared.Components.DropEstados
{
    public class DropAlunos : ViewComponent
    {
        private readonly IUsuarioServicos _seUsuario;
        public DropAlunos(IUsuarioServicos seUsuario)
        {
            _seUsuario = seUsuario;
        }
        public async Task<IViewComponentResult> InvokeAsync(int professorId)
        {
            UsuarioRQModel usuario = new();
            usuario.ProfessorID = professorId;

            var lst = await _seUsuario.Listar(usuario);

            return View("Default", lst);
        }
    }
}



/*

@await Component.InvokeAsync("DropAlunos", new { professorId = professorId })

*/