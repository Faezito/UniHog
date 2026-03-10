using Microsoft.AspNetCore.Mvc;
using Z1.Model;
using Z2.Servicos;

namespace UniHog.Views.Shared.Components.DropEstados
{
    public class DropPessoas : ViewComponent
    {
        private readonly IUsuarioServicos _seUsuario;
        public DropPessoas(IUsuarioServicos seUsuario)
        {
            _seUsuario = seUsuario;
        }
        public async Task<IViewComponentResult> InvokeAsync(string tipoIDs)
        {
            int[] tipos = tipoIDs.Split(",").Select(int.Parse).ToArray();
            List<UsuarioModel> usuarios = [];

            foreach (int tipo in tipos)
            {
                UsuarioRQModel usuario = new();
                usuario.TipoID = tipo;
                var lst = await _seUsuario.Listar(usuario);
                usuarios.AddRange(lst);
            }


            return View("Default", usuarios);
        }
    }
}



/*

@await Component.InvokeAsync("DropPessoas", new { tipoID = tipoID (string) })

*/