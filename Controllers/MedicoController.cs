using UniHog.Libraries.Login;
using Microsoft.AspNetCore.Mvc;
using Z1.Model;
using Z2.Servicos;

namespace UniHog.Controllers
{
    [Autorizacoes]
    public class MedicoController : Controller
    {
        private readonly IUsuarioServicos _usuarios;
        private readonly IAtendimentoServicos _atendimentos;
        public MedicoController(IUsuarioServicos usuarios, IAtendimentoServicos atendimentos)
        {
             _usuarios = usuarios; 
            _atendimentos = atendimentos;
        }
        public async Task<IActionResult> Index()
        {
            List<DiasDaSemana> diasSemana = await _atendimentos.ListarDiasDaSemana();
            var lst = await _usuarios.Listar(2, null);
            List<AtendimentoModel> atendimentos = lst.Select(u => new AtendimentoModel
            {
                Paciente = u
            }).ToList();

            ViewBag.DiasDaSemana = diasSemana;
            return View(atendimentos);
        }
    }
}
