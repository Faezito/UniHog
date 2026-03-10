using UniHog.Extensions;
using UniHog.Libraries.Login;
using Microsoft.AspNetCore.Mvc;
using Z1.Model;

namespace UniHog.Views.Shared.Components.Navbar
{
    public class Navbar : ViewComponent
    {
        private readonly LoginUsuario _login;
        public Navbar(LoginUsuario login)
        {
            _login = login;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            UsuarioModel usuario = new();
            usuario = ViewContext.HttpContext.User.ObterUsuario();

            return View("Default", usuario);
        }

        /*
            @await Component.InvokeAsync("Navbar")
        */
    }
}
