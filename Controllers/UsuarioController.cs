using UniHog.Extensions;
using UniHog.Libraries.Filtros;
using UniHog.Libraries.Login;
using UniHog.Libraries.Sessao;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Z1.Model;
using Z2.Services.Externo;
using Z2.Servicos;
using Z4.Bibliotecas;

namespace UniHog.Controllers
{
    [Autorizacoes]
    [ValidateHttpRefererAttributes]
    #region Construtor
    public class UsuarioController : Controller
    {
        private readonly IUsuarioServicos _seUsuario;
        private readonly IEnderecoServicos _seEndereco;
        private readonly IEmailServicos _emailServicos;
        private readonly Sessao _sessao;
        private readonly LoginUsuario _login;
        private readonly ICloudinaryServicos _cloudinary;
        private readonly IInteressadoServicos _interessado;
        private readonly IViaCepServicos _cep;

        public UsuarioController(
            IUsuarioServicos seUsuario,
            IEmailServicos emailServicos,
            Sessao sessao,
            LoginUsuario login,
            IEnderecoServicos seEndereco,
            ICloudinaryServicos cloudinary,
            IInteressadoServicos interessado,
            IViaCepServicos cep
            )
        {
            _seUsuario = seUsuario;
            _emailServicos = emailServicos;
            _sessao = sessao;
            _login = login;
            _seEndereco = seEndereco;
            _cloudinary = cloudinary;
            _interessado = interessado;
            _cep = cep;
        }
        #endregion

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Cadastro(int? id, bool? senhaTemporaria)
        {
            UsuarioModel model = new();

            if (id.HasValue)
            {
                try
                {
                    model = await _seUsuario.Obter(id.Value, null);
                }
                catch (Exception ex)
                {
                    return Problem(title: "Erro", detail: ex.Message);
                }
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Cadastro(UsuarioCadastroModel model)
        {
            try
            {
                int? id;
                model.UsuarioLogado = this.User.GetUserId();

                (string? url, string? pubicId) url = (string.Empty, string.Empty);

                if (!string.IsNullOrWhiteSpace(model.CPF))
                    model.CPF = ManipularModels.LimparNumeros(model.CPF);

                if (string.IsNullOrWhiteSpace(model.Telefone))
                    model.Telefone = KeyGenerator.GetUniqueNumber(11);

                model.Telefone = ManipularModels.LimparNumeros(model.Telefone);
                model.NomeCompleto = ManipularModels.FormatarNomeCompleto(model.NomeCompleto);

                if (!model.PessoaID.HasValue) // CADASTRAR
                {
                    try
                    {
                        if (!ModelState.IsValid)
                        {
                            var mensagens = string.Join(" ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                            return Problem(detail: mensagens, title: "Erro");
                        }

                        if (string.IsNullOrWhiteSpace(model.CEP))
                        {
                            if (string.IsNullOrWhiteSpace(model.UF) || string.IsNullOrWhiteSpace(model.Cidade))
                            {
                                model.CEP = await _cep.EncontrarCEP("RJ", "Rio de Janeiro", model.Rua); // TODO: TROCAR PARA DADOS DA CONGRECAÇÃO
                            }
                            else
                            {
                                model.CEP = await _cep.EncontrarCEP(model.UF, model.Cidade, model.Rua);
                            }
                        }

                        if (string.IsNullOrWhiteSpace(model.Usuario))
                            model.Usuario = ManipularModels.GerarUsuario2(model.NomeCompleto, KeyGenerator.GetUniqueKey(4));
                        else
                            model.Usuario = model.Usuario.Trim().ToLower();

                        if (string.IsNullOrWhiteSpace(model.Senha))
                            model.Senha = KeyGenerator.GetUniqueKey(6);

                        string email = model.Email ?? model.Usuario + "@bopemail.com";

                        model.ProfessorID = model.ProfessorID ?? this.User.GetUserId();
                        model.CEP = ManipularModels.LimparNumeros(model.CEP);
                        model.TipoID = model.TipoID ?? 20;
                        model.Pais = model.Pais ?? "Brasil";
                        model.Email = email.Trim().Replace(" ", string.Empty);

                        if (model.Foto != null)
                        {
                            url = await _cloudinary.UploadFotoPerfil(model.Foto, model.Usuario);
                            model.FotoPerfilURL = url.url;
                            model.FotoPerfilPublicID = url.pubicId;
                        }

                        id = await _seUsuario.Cadastrar(model);

                        var tiposEmail = new HashSet<int> { -1, 1, 2, 3, 4, 5, 6, 10, 15 };
                        if (tiposEmail.Contains(model.TipoID.Value))
                        {
                            await _emailServicos.EnviarSenhaPorEmail(true, model);
                        }

                        if (model.Empresa == true)
                        {
                            return Json(new { success = true, redirectUrl = Url.Action("Cadastro", "PessoaJuridica", new { id = id }) });
                        }

                        return Json(new { success = true, redirectUrl = Url.Action("Cadastros", "Menu") });
                    }
                    catch (Exception ex)
                    {
                        if (model.FotoPerfilPublicID != null)
                            await _cloudinary.DeletarImagem(model.FotoPerfilPublicID);
                        return Problem(title: "Erro", detail: ex.Message);
                    }
                }
                else  // EDITAR
                {
                    var usuario = await _seUsuario.Obter(model.PessoaID, null);

                    bool trocouSenha = false;
                    if (!string.IsNullOrWhiteSpace(model.Senha) && usuario.Senha != model.Senha)
                    {
                        model.SenhaTemporaria = false;
                        trocouSenha = true;
                    }

                    if (model.Foto != null)
                    {
                        url = await _cloudinary.UploadFotoPerfil(model.Foto, model.Usuario);
                        model.FotoPerfilURL = url.url;
                        model.FotoPerfilPublicID = url.pubicId;
                    }

                    id = await _seUsuario.Cadastrar(model);

                    if (trocouSenha == true)
                    {
                        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                        HttpContext.Session.Clear();
                        TempData["MSG_S"] = "Senha alterada com sucesso, será necessário efetuar o login novamente com a nova senha.";
                        Json(new { success = true, redirectUrl = Url.Action("Login", "Home") });
                    }
                    if (model.Empresa == true)
                    {
                        return Json(new { success = true, redirectUrl = Url.Action("Cadastro", "PessoaJuridica", new { id = model.PessoaID }) });
                    }
                    return Json(new { success = true, redirectUrl = Url.Action("Cadastros", "Menu") });
                }

            }
            catch (Exception ex)
            {
                return Problem(title: "Erro", detail: ex.Message);
            }
        }

        [HttpGet]
        [ValidateHttpRefererAttributes]
        public async Task<IActionResult> Editar(int pessoaId)
        {
            UsuarioModel model = new();
            try
            {
                model = await _seUsuario.Obter(pessoaId, null);

                return View(model);
            }
            catch (Exception ex)
            {
                return Problem(title: "Erro", detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost]
        public async Task<IActionResult> ListarUsuarios(UsuarioRQModel model, int? rotaId, int pagina = 1, int quantidade = 10)
        {
            model.UsuarioTipoID = this.User.GetUserTipoId();

            switch (rotaId)
            {
                case null: // se for null, listará os usuários
                    model.TipoIDs = "-1,1,2,3,4,5,10,15";
                    break;
                case 1: // Usuários do sistema
                    model.TipoIDs = "-1,1,2,3,4,5,10,15";
                    break;
                case 2: // Estudantes
                    model.TipoID = 20;
                    break;
                default:
                    break;
            }

            PaginacaoModel<UsuarioModel> usuarios = await _seUsuario.ListarPaginado(model, pagina, quantidade);

            return PartialView("_TabelaUsuarios", usuarios);
        }

        [HttpGet]
        public async Task<IActionResult> ListarDrop(string nome)
        {
            int usuarioTipoID = this.User.GetUserTipoId();

            List<UsuarioModel> usuarios = await _seUsuario.ListarDrop(null, nome);
            List<UsuarioDropModel> model = [];

            foreach (var usuario in usuarios)
            {
                UsuarioDropModel pessoa = new UsuarioDropModel
                {
                    id = usuario.PessoaID.Value,
                    text = usuario.NomeCompleto
                };
                model.Add(pessoa);
            }

            return Json(model);
        }

        [HttpPost]
        public async Task<IActionResult> UsuarioDetalhes(int id)
        {
            try
            {
                UsuarioModel model = await _seUsuario.Obter(id, null);
                return PartialView("_ModalUsuario", model);
            }
            catch (Exception ex)
            {
                return Problem(title: "Erro", detail: ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AdicionarEndereco(EnderecoModel model)
        {
            try
            {
                var lst = await _seEndereco.ListarEnderecosDoUsuario(model.PessoaID.Value);
                EnderecoModel enderecoPrimario = lst.Where(x => x.TipoID == 1).SingleOrDefault();

                if (enderecoPrimario != null && model.TipoID == 1)
                    return Problem(title: "Erro", detail: "Esta pessoa já possui um endereço primário.");

                if (string.IsNullOrWhiteSpace(model.CEP))
                {
                    if (string.IsNullOrWhiteSpace(model.UF) || string.IsNullOrWhiteSpace(model.Cidade))
                    {
                        model.CEP = await _cep.EncontrarCEP("RJ", "Rio de Janeiro", model.EnderecoCompleto); // TODO: TROCAR PARA DADOS DA CONGRECAÇÃO
                    }
                    else
                    {
                        model.CEP = await _cep.EncontrarCEP(model.UF, model.Cidade, model.EnderecoCompleto);
                    }
                }

                model.CEP = model.CEP.FormatarCEP();

                await _seEndereco.Cadastro(model);

                return Json(new
                {
                    success = true,
                    redirectUrl = Url.Action("Editar", "Usuario", new { model.PessoaID })
                });
            }
            catch (Exception ex)
            {
                return Problem(title: "Erro", detail: ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeletarUsuario(int id)
        {
            try
            {
                UsuarioModel usuario = new UsuarioModel
                {
                    DataDeletado = DateTime.Now,
                    PessoaID = id,
                    UsuarioLogado = this.User.GetUserId(),
                    DataAlteracao = DateTime.Now
                };

                await _seUsuario.Deletar(usuario);

                return Json(new { success = true, detail = "Deletado com sucesso!", redirectUrl = Url.Action("Cadastros", "Menu") });
            }
            catch (Exception ex)
            {
                return Problem(title: "Erro", detail: ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeletarEndereco(int enderecoId, int pessoaId)
        {
            try
            {
                await _seEndereco.DeletarEnderecoDoUsuario(enderecoId, pessoaId);
                var referer = Request.Headers["Referer"].ToString();

                if (!string.IsNullOrEmpty(referer))
                    return Redirect(referer);

                return Json(new
                {
                    success = true,
                    redirectUrl = Url.Action("Editar", "Usuario", new { pessoaId = pessoaId })
                });
            }
            catch (Exception ex)
            {
                return Problem(title: "Erro", detail: ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeletarImagem(UsuarioCadastroModel model)
        {
            try
            {
                bool result = false;

                if (model.FotoPerfilPublicID != null && model.PessoaID.HasValue)
                    result = await _cloudinary.DeletarImagem(model.FotoPerfilPublicID);
                await _seUsuario.DeletarImagem(model.PessoaID.Value);

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Problem(title: "Erro", detail: ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> ResetarSenha(int id)
        {
            try
            {
                UsuarioModel model = new();
                model.Senha = KeyGenerator.GetUniqueKey(6);
                model.PessoaID = id;
                model.UsuarioLogado = this.User.GetUserId();
                model.DataAlteracao = DateTime.Now;

                await _seUsuario.AtualizarSenha(model);
                model = await _seUsuario.Obter(id, null);

                await _emailServicos.EnviarSenhaPorEmail(false, model);
                return Json(new
                {
                    success = true,
                    redirectUrl = Url.Action("Usuarios", "Admin", new { rotaId = 1 }),
                    detail = "A nova senha temporária foi enviada para o e-mail cadastrado."
                });
            }
            catch (Exception ex)
            {
                return Problem(title: "Erro", detail: ex.Message);
            }
        }
    }
}
