using UniHog.Extensions;
using UniHog.Libraries.Filtros;
using UniHog.Libraries.Login;
using UniHog.Libraries.Sessao;
using Microsoft.AspNetCore.Mvc;
using Z1.Model;
using Z2.Services.Externo;
using Z2.Servicos;
using Z4.Bibliotecas;

namespace UniHog.Controllers
{
    [Autorizacoes]
    public class InteressadoController : Controller
    {
        #region CTOR e DI
        private readonly IUsuarioServicos _seUsuario;
        private readonly IEmailServicos _email;
        private readonly IEnderecoServicos _seEndereco;
        private readonly ICloudinaryServicos _cloudinary;
        private readonly IInteressadoServicos _interessado;
        private readonly IViaCepServicos _cep;

        [Autorizacoes]
        [ValidateHttpRefererAttributes]
        public InteressadoController(
                IUsuarioServicos seUsuario,
                IEmailServicos email,
                Sessao sessao,
                LoginUsuario login,
                IEnderecoServicos seEndereco,
                ICloudinaryServicos cloudinary,
                IInteressadoServicos interessado,
                IViaCepServicos cep
                )
        {
            _seUsuario = seUsuario;
            _email = email;
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

        [HttpPost]
        public async Task<IActionResult> Pesquisar(UsuarioRQModel model)
        {
            int usuarioTipoID = this.User.GetUserTipoId();

            List<InteressadoModel> lst = await _interessado.Listar(model);

            return PartialView("_TabelaInteressados", lst);
        }

        [HttpGet]
        [Autorizacoes(Tipos = "-1,1,2,3,4,5,6,7,8,9,10")]
        public IActionResult Cadastro()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Cadastro(InteressadoCadastroModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var mensagens = string.Join(" ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                    return Problem(detail: mensagens, title: "Erro");
                }
                model.NomeCompleto = ManipularModels.FormatarNomeCompleto(model.NomeCompleto);

                if (model.Interessado == true)
                {

                    model.UsuarioLogado = this.User.GetUserId();

                    (string? url, string? pubicId) url = (string.Empty, string.Empty);

                    if (!string.IsNullOrWhiteSpace(model.CPF))
                        model.CPF = ManipularModels.LimparNumeros(model.CPF);

                    if (string.IsNullOrWhiteSpace(model.Telefone))
                        model.Telefone = KeyGenerator.GetUniqueNumber(11);

                    if (string.IsNullOrWhiteSpace(model.CEP))
                    {
                        if (string.IsNullOrWhiteSpace(model.UF) || string.IsNullOrWhiteSpace(model.Cidade))
                        {   // TODO: TROCAR PARA DADOS DA CONGRECAÇÃO
                            model.Cidade = "Rio de Janeiro";
                            model.Estado = "Rio de Janeiro";
                            model.Regiao = "Sudeste";
                            model.UF = "RJ";
                        }
                        model.CEP = await _cep.EncontrarCEP(model.UF, model.Cidade, $"{model.Rua}, {model.Numero}, {model.Bairro}");
                    }

                    model.Telefone = ManipularModels.LimparNumeros(model.Telefone);

                    try
                    {
                        string email = model.Email ?? model.Usuario + "@bopemail.com";

                        model.Usuario = ManipularModels.GerarUsuario2(model.NomeCompleto, KeyGenerator.GetUniqueKey(4));
                        model.Email = email.Trim().Replace(" ", string.Empty);
                        model.ProfessorID = model.ProfessorID ?? this.User.GetUserId();
                        model.CEP = ManipularModels.LimparNumeros(model.CEP);

                        if (model.Foto != null)
                        {
                            url = await _cloudinary.UploadFotoPerfil(model.Foto, model.Usuario);
                            model.FotoPerfilURL = url.url;
                            model.FotoPerfilPublicID = url.pubicId;
                        }

                        await _interessado.Cadastrar(model);

                        model.Professor = this.User.GetUserName();
                        await _email.EnviarInteressadoPorEmail(model);

                        return Json(new { success = true, redirectUrl = Url.Action("Index", "Menu") });
                    }
                    catch (Exception ex)
                    {
                        if (model.FotoPerfilPublicID != null)
                            await _cloudinary.DeletarImagem(model.FotoPerfilPublicID);
                        return Problem(title: "Erro", detail: ex.Message);
                    }
                }
                else
                {
                    model.UsuarioLogado = this.User.GetUserId();

                    if (!string.IsNullOrWhiteSpace(model.CPF))
                        model.CPF = ManipularModels.LimparNumeros(model.CPF);

                    if (string.IsNullOrWhiteSpace(model.Telefone))
                        model.Telefone = KeyGenerator.GetUniqueNumber(11);

                    if (string.IsNullOrWhiteSpace(model.CEP))
                    {
                        // TODO: TROCAR PARA DADOS DA CONGRECAÇÃO
                        model.Cidade = model.Cidade ?? "Rio de Janeiro";
                        model.UF = model.UF ?? "RJ";
                        model.Rua = model.Rua ?? "Rua Rio 1";
                        model.Numero = model.Numero ?? "0";
                        model.Estado = model.Estado ?? "Rio de Janeiro";
                        model.Regiao = "Sudeste";

                        model.CEP = await _cep.EncontrarCEP(model.UF, model.Cidade, model.Rua);
                    }

                    model.Telefone = ManipularModels.LimparNumeros(model.Telefone);

                    model.ProfessorID = model.ProfessorID ?? this.User.GetUserId();
                    model.CEP = ManipularModels.LimparNumeros(model.CEP);
                    model.Usuario = ManipularModels.GerarUsuario2(model.NomeCompleto, KeyGenerator.GetUniqueKey(4));
                    model.Email = model.Email ?? model.Usuario + "@bopemail.com";

                    await _interessado.Cadastrar(model);

                    if (this.User.GetUserTipoId() < 5)
                    {
                        return Json(new { success = true, redirectUrl = Url.Action("Index", "Interessado") });
                    }
                    return Json(new { success = true, redirectUrl = Url.Action("Index", "Menu") });
                }

            }
            catch (Exception ex)
            {
                return Problem(title: "Erro", detail: ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Detalhes(int id)
        {
            try
            {
                InteressadoModel model = await _interessado.Obter(id);
                return PartialView("_ModalDetalhes", model);
            }
            catch (Exception ex)
            {
                return Problem(title: "Erro", detail: ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Integrar(int id)
        {
            InteressadoModel interessado = await _interessado.Obter(id);
            UsuarioModel usuario = new();

            usuario.NomeCompleto = interessado.NomeCompleto;
            usuario.Usuario = interessado.Usuario;
            usuario.Email = interessado.Email;
            usuario.CPF = interessado.CPF;
            usuario.Telefone = interessado.Telefone;
            usuario.Genero = interessado.Genero;
            usuario.FotoPerfilURL = interessado.FotoPerfilURL;
            usuario.FotoPerfilPublicID = interessado.FotoPerfilPublicID;
            usuario.DataNascimento = interessado.DataNascimento;
            usuario.DiasEstudoIDs = interessado.DiasEstudoIDs;
            usuario.ProjetoIndicacaoID = interessado.ProjetoIndicacaoID;
            usuario.ProfessorID = interessado.ProfessorID;
            usuario.Professor = interessado.Professor;
            usuario.Enderecos = interessado.Enderecos;
            usuario.Foto = interessado.Foto;
            usuario.FotoPerfilURL = interessado.FotoPerfilURL;
            usuario.FotoPerfilPublicID = interessado.FotoPerfilPublicID;
            usuario.DataNascimento = interessado.DataNascimento;
            usuario.interessadoId = interessado.PessoaID;

            ViewBag.InteressadoID = id;
            return View(usuario);
        }

        [HttpPost]
        public async Task<IActionResult> Integrar(UsuarioModel model)
        {
            try
            {
                int? id;
                (string? url, string? pubicId) url = (string.Empty, string.Empty);

                if (!string.IsNullOrWhiteSpace(model.CPF))
                    model.CPF = ManipularModels.LimparNumeros(model.CPF);

                if (!string.IsNullOrWhiteSpace(model.Telefone))
                {
                    model.Telefone = ManipularModels.LimparNumeros(model.Telefone);
                }
                else
                {
                    model.Telefone = KeyGenerator.GetUniqueNumber(11);
                }

                try
                {
                    model.Senha = KeyGenerator.GetUniqueKey(6);
                    model.DataCriacao = DateTime.Now;
                    model.UsuarioLogado = this.User.GetUserId();
                    model.ProfessorID = model.ProfessorID ?? this.User.GetUserId();

                    if (model.Foto != null)
                    {
                        url = await _cloudinary.UploadFotoPerfil(model.Foto, model.Usuario);
                        model.FotoPerfilURL = url.url;
                        model.FotoPerfilPublicID = url.pubicId;
                    }

                    id = await _seUsuario.Integrar(model);

                    return Json(new { success = true, redirectUrl = Url.Action("Index", "Menu") });
                }
                catch (Exception ex)
                {
                    if (model.FotoPerfilPublicID != null)
                        await _cloudinary.DeletarImagem(model.FotoPerfilPublicID);
                    return Problem(title: "Erro", detail: ex.Message);
                }
            }
            catch (Exception ex)
            {
                return Problem(title: "Erro", detail: ex.Message);
            }

        }

        [HttpPost]
        public async Task<IActionResult> Deletar(int interessadoId)
        {
            try
            {
                await _interessado.Deletar(interessadoId);
                return Ok(new { success = true, detail = "Deletado com sucesso!", redirectUrl = Url.Action(nameof(Index)) });

            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}


