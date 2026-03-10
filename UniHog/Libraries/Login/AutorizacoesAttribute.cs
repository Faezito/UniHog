using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace UniHog.Libraries.Login
{
    public class AutorizacoesAttribute : Attribute, IAuthorizationFilter
    {
        public string Tipos { get; set; } // Ex: "Admin,Gerente"

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            if (!user.Identity.IsAuthenticated)
            {
                context.Result = new RedirectToActionResult("Login", "Home", null);
                return;
            }

            if (string.IsNullOrEmpty(Tipos))
                return;

            var tiposPermitidos = Tipos
                .Split(',')
                .Select(t => int.Parse(t.Trim()))
                .ToList();

            var tipoUsuario = int.Parse(user.FindFirst("TipoID")?.Value);

            if (tipoUsuario == 0 || !tiposPermitidos.Contains(tipoUsuario))
            {
                bool isAjax = context.HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";

                if (isAjax)
                {
                    context.Result = new JsonResult(new
                    {
                        sucesso = false,
                        erro = "Acesso negado."
                    });
                    context.HttpContext.Response.StatusCode = StatusCodes.Status403Forbidden;
                    return;
                }
                else
                {
                    context.Result = new RedirectToActionResult("Login", "Home", null);
                }
            }
        }
    }
}
