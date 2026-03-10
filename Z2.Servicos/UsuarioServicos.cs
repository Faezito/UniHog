using System.Transactions;
using Z1.Model;
using Z3.DataAccess;

namespace Z2.Servicos
{
    public interface IUsuarioServicos
    {
        Task<List<UsuarioModel>> Listar(UsuarioRQModel model);
        Task<List<UsuarioModel>> Listar(int? projetoID, int? estudoId);
        Task<List<UsuarioModel>> ListarAniversariantes();
        Task<List<UsuarioModel>> ListarDrop(int? tipoID, string nome);
        Task<PaginacaoModel<UsuarioModel>> ListarPaginado(UsuarioRQModel model, int pagina, int quantidade);
        Task<int?> Cadastrar(UsuarioCadastroModel model);
        Task<int?> Integrar(UsuarioModel model);
        Task Deletar(UsuarioModel model);
        Task<UsuarioModel> Obter(int? id, string? email);
        Task<UsuarioModel> Obter(string email);
        Task<UsuarioModel> Login(string? usuario, string Senha);
        Task AtualizarSenha(UsuarioModel model);
        Task AtualizarSessao(int usuarioId);
        Task DeletarImagem(int pessoaId);

        // TIPOS
        Task<List<TiposModel>> ListarTipos(int usuarioId);
        Task<TiposModel> ObterTipo(int tipoId, int usuarioId);
        Task<TiposModel> ObterTipo(int tipoId);
        Task CadastrarTipo(TiposModel model);
        Task DeletarTipo(TiposModel model);

        // SITUAÇÕES
        Task<List<TiposModel>> ListarSituacoes();
        Task<int?> CadastroSituacao(TiposModel model);

    }

    public class UsuarioServicos : IUsuarioServicos
    {
        #region Construtor-DI
        private readonly IUsuarioDataAccess _daUsuario;
        private readonly IEnderecoDataAccess _daEndereco;
        private readonly IEstudoServicos _estudo;
        private readonly IInteressadoServicos _interessado;
        private readonly IPessoaJuridicaDataAccess _pj;
        private readonly IProfessorDataAccess _professor;
        private readonly ICloudinaryServicos _cloudinary;

        public UsuarioServicos(IUsuarioDataAccess daUsuario,
            IEnderecoDataAccess daEndereco,
            IEstudoServicos estudo,
            IInteressadoServicos interessado,
            IPessoaJuridicaDataAccess pj,
            IProfessorDataAccess professor,
            ICloudinaryServicos cloudinary
            )
        {
            _daUsuario = daUsuario;
            _daEndereco = daEndereco;
            _estudo = estudo;
            _interessado = interessado;
            _pj = pj;
            _professor = professor;
            _cloudinary = cloudinary;
        }
        #endregion

        public async Task AtualizarSenha(UsuarioModel model)
        {
            await _daUsuario.AtualizarSenha(model);
        }

        public async Task<int?> Cadastrar(UsuarioCadastroModel model)
        {
            int? pessoaId;
            int? enderecoId;

            UsuarioModel usuario = new UsuarioModel
            {
                PessoaID = model.PessoaID,
                NomeCompleto = model.NomeCompleto,
                Usuario = model.Usuario.ToLower().Trim(),
                Email = model.Email,
                Senha = model.Senha,
                CPF = model.CPF,
                Telefone = model.Telefone,
                SituacaoID = model.SituacaoID,
                Genero = model.Genero,
                TipoID = model.TipoID,
                FotoPerfilURL = model.FotoPerfilURL,
                FotoPerfilPublicID = model.FotoPerfilPublicID,
                DataNascimento = model.DataNascimento,
                EstudoID = model.EstudoID,
                ProfessorID = model.ProfessorID,
                ProjetoIndicacaoID = model.ProjetoIndicacaoID,
                DiasEstudoIDs = model.DiasEstudoIDs,
                DataAlteracao = DateTime.Now,
                DataCriacao = DateTime.Now,
                UsuarioLogado = model.UsuarioLogado,
                SenhaTemporaria = model.SenhaTemporaria,
                EstudaEmCasa = model.EstudaEmCasa
            };

            if (model.PessoaID.HasValue)
            {
                using (var scope = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled))
                {
                    try
                    {
                        pessoaId = await _daUsuario.Atualizar(usuario);

                        if (usuario.TipoID == 20)
                        {
                            await _daUsuario.RemoverProfessor(usuario);
                            await _daUsuario.AdicionarProfessor(usuario);
                            await _estudo.AtualizarDias(usuario, pessoaId.Value);
                        }

                        scope.Complete();
                        return model.PessoaID;
                    }
                    catch (Exception)
                    {
                        scope.Dispose();
                        throw;
                    }
                }
            }
            else  // EDIÇÃO
            {
                EnderecoModel endereco = new EnderecoModel
                {
                    CEP = model.CEP,
                    Rua = model.Rua,
                    Numero = model.Numero,
                    Bairro = model.Bairro,
                    Cidade = model.Cidade,
                    Estado = model.Estado,
                    Pais = model.Pais,
                    UF = model.UF,
                    Regiao = model.Regiao,
                    Complemento = model.Complemento,
                    Referencia = model.Referencia,
                    TipoID = model.EnderecoTipoID
                };

                using (var scope = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled))
                {
                    try
                    {
                        pessoaId = await _daUsuario.Adicionar(usuario);
                        enderecoId = await _daEndereco.Inserir(endereco);
                        await _daEndereco.AtribuirEndereco(pessoaId.Value, enderecoId.Value, model.EnderecoTipoID.Value);

                        if (usuario.TipoID == 20) // 20 = aluno
                        {
                            await _daUsuario.RemoverProfessor(usuario);
                            usuario.PessoaID = pessoaId;
                            await _daUsuario.AdicionarProfessor(usuario);
                            await _estudo.AtualizarDias(usuario, pessoaId.Value);
                        }
                        scope.Complete();
                    }
                    catch (Exception)
                    {
                        scope.Dispose();
                        throw;
                    }
                }

                return pessoaId;
            }
        }

        public async Task Deletar(UsuarioModel model)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    UsuarioModel usuario = await Obter(model.PessoaID.Value, null);
                    var enderecos = await _daEndereco.ListarEnderecosDoUsuario(usuario.PessoaID.Value);
                    await _daEndereco.DeletarEnderecosDaPessoa(usuario.PessoaID.Value); // Deleta os endereços da tabela de relacionamento usuario x endereço
                    foreach (var endereco in enderecos) // deleta cada endereço da tabela de endereços
                    {
                        await _daEndereco.Deletar(endereco.ID.Value);
                    }
                    await _daUsuario.RemoverProfessor(usuario);
                    await _estudo.LimparDiasDeEstudo(usuario.PessoaID.Value);
                    await _professor.LimparChamada(usuario.PessoaID.Value);
                    if (!string.IsNullOrWhiteSpace(usuario.FotoPerfilPublicID))
                    {
                        await _cloudinary.DeletarImagem(usuario.FotoPerfilPublicID);
                    }
                    await _daUsuario.Deletar(model);
                    scope.Complete();
                }
                catch (Exception)
                {
                    scope.Dispose();
                    throw;
                }
            }
        }

        public async Task<List<UsuarioModel>> Listar(UsuarioRQModel model)
        {
            List<UsuarioModel> lst = await _daUsuario.Listar(model);

            foreach (var usuario in lst)
            {
                List<EnderecoModel> enderecos = await _daEndereco.ListarEnderecosDoUsuario(usuario.PessoaID.Value);
                List<DiasEstudoModel> diasEstudo = await _estudo.ListarDiasDeEstudoDaPessoa(usuario.PessoaID.Value);

                foreach (var endereco in enderecos)
                {
                    var enderecoCompleto = $"{endereco.Rua}, {endereco.Numero}, {endereco.Bairro}, {endereco.Cidade} - {endereco.Estado}, {endereco.CEP}";
                    var enderecoEncoded = Uri.EscapeDataString(enderecoCompleto);
                    endereco.EnderecoMapsURL = $"https://www.google.com/maps/search/?api=1&query={enderecoEncoded}";

                    usuario.Enderecos.Add(endereco);
                }
                usuario.DiasEstudoIDs = diasEstudo.Select(d => d.DiaID).ToArray();

            }

            return lst;
        }

        public async Task<List<UsuarioModel>> Listar(int? projetoID, int? estudoId)
        {
            List<UsuarioModel> lst = await _daUsuario.Listar(projetoID, estudoId);

            foreach (var usuario in lst)
            {
                List<EnderecoModel> enderecos = await _daEndereco.ListarEnderecosDoUsuario(usuario.PessoaID.Value);
                List<DiasEstudoModel> diasEstudo = await _estudo.ListarDiasDeEstudoDaPessoa(usuario.PessoaID.Value);

                foreach (var endereco in enderecos)
                {
                    var enderecoCompleto = $"{endereco.Rua}, {endereco.Numero}, {endereco.Bairro}, {endereco.Cidade} - {endereco.Estado}, {endereco.CEP}";
                    var enderecoEncoded = Uri.EscapeDataString(enderecoCompleto);
                    endereco.EnderecoMapsURL = $"https://www.google.com/maps/search/?api=1&query={enderecoEncoded}";

                    usuario.Enderecos.Add(endereco);
                }
                usuario.DiasEstudoIDs = diasEstudo.Select(d => d.DiaID).ToArray();

            }

            return lst;
        }


        public async Task<UsuarioModel> Login(string? usuario, string Senha)
        {
            var user = await _daUsuario.Obter(null, usuario);

            if (user != null)
            {
                if (user.Senha == Senha)
                {
                    List<EnderecoModel> enderecos = await _daEndereco.ListarEnderecosDoUsuario(user.PessoaID.Value);
                    foreach (var endereco in enderecos)
                    {
                        var enderecoCompleto = $"{endereco.Rua}, {endereco.Numero}, {endereco.Bairro}, {endereco.Cidade} - {endereco.Estado}, {endereco.CEP}";
                        var enderecoEncoded = Uri.EscapeDataString(enderecoCompleto);
                        endereco.EnderecoMapsURL = $"https://www.google.com/maps/search/?api=1&query={enderecoEncoded}";

                        user.Enderecos.Add(endereco);
                    }

                    return user;
                }
            }
            return null;
        }

        public async Task<UsuarioModel> Obter(string email)
        {
            var user = await _daUsuario.Obter(null, email);

            if (user == null)
            {
                throw new Exception("Não encontramos um usuário com o e-mail informado.");
            }
            return user;
        }

        public async Task<UsuarioModel> Obter(int? id, string? email)
        {
            UsuarioRQModel model = new UsuarioRQModel { Email = email, PessoaID = id };

            var lst = await Listar(model);
            UsuarioModel usuario = lst.SingleOrDefault();

            if (id.HasValue)
            {
                var empresa = await _pj.Listar(new PessoaJuridicaRQModel { PessoaID = id });
                if (empresa.Count() > 0)
                    usuario.Empresa = true;
            }
            return usuario;
        }

        public async Task DeletarImagem(int pessoaId)
        {
            await _daUsuario.DeletarImagem(pessoaId);
        }

        public async Task<int?> Integrar(UsuarioModel model)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    // Cadastra usuário com os dados do interessado
                    int? pessoaId = await _daUsuario.Adicionar(model);
                    model.PessoaID = pessoaId;

                    //Obtém endereço da tabela de endereços dos interessados
                    EnderecoModel enderecoOriginal = await _interessado.ObterEndereco(model.interessadoId.Value);

                    // Cadastra endereço na tabela principal de endereços e atribui o id do novo endereço ao id do usuário
                    int? enderecoId = await _daEndereco.Inserir(enderecoOriginal);
                    await _daEndereco.AtribuirEndereco(pessoaId.Value, enderecoId.Value, 1);

                    // Deleta endereço e usuário da tabela anterior
                    await _interessado.Deletar(model.interessadoId.Value);

                    // Atualiza professor e dias de aula
                    if (model.TipoID > 5)
                    {
                        await _daUsuario.RemoverProfessor(model);

                        await _daUsuario.AdicionarProfessor(model);
                        await _estudo.AtualizarDias(model, pessoaId.Value);
                    }

                    scope.Complete();
                    return model.PessoaID;
                }
                catch (Exception ex)
                {
                    scope.Dispose();
                    throw;
                }
            }
        }

        public async Task<PaginacaoModel<UsuarioModel>> ListarPaginado(UsuarioRQModel model, int pagina, int quantidade)
        {
            var usuarios = await _daUsuario.ListarPaginado(model, pagina, quantidade);

            int total = usuarios.FirstOrDefault()?.TotalRegistros ?? 0;

            foreach (var usuario in usuarios)
            {
                List<EnderecoModel> enderecos = await _daEndereco.ListarEnderecosDoUsuario(usuario.PessoaID.Value);
                List<DiasEstudoModel> diasEstudo = await _estudo.ListarDiasDeEstudoDaPessoa(usuario.PessoaID.Value);

                foreach (var endereco in enderecos)
                {
                    var enderecoCompleto = $"{endereco.Rua}, {endereco.Numero}, {endereco.Bairro}, {endereco.Cidade} - {endereco.Estado}, {endereco.CEP}";
                    var enderecoEncoded = Uri.EscapeDataString(enderecoCompleto);
                    endereco.EnderecoMapsURL = $"https://www.google.com/maps/search/?api=1&query={enderecoEncoded}";

                    usuario.Enderecos.Add(endereco);
                }
                usuario.DiasEstudoIDs = diasEstudo.Select(d => d.DiaID).ToArray();
            }

            PaginacaoModel<UsuarioModel> paginado = new()
            {
                PaginaAtual = pagina,
                Quantidade = quantidade,
                Dados = usuarios,
                Total = total
            };

            return paginado;
        }

        public async Task<List<UsuarioModel>> ListarDrop(int? tipoID, string nome)
        {
            return await _daUsuario.ListarDrop(tipoID, nome);
        }

        public Task<List<UsuarioModel>> ListarAniversariantes()
        {
            return _daUsuario.ListarAniversariantes();
        }

        public async Task AtualizarSessao(int usuarioId)
        {
            UsuarioModel model = new();
            model.PessoaID = usuarioId;
            model.UltimaSessao = DateTime.Now;

            await _daUsuario.Atualizar(model);
        }





        #region Tipos
        public async Task<List<TiposModel>> ListarTipos(int usuarioId)
        {
            var usuario = await Obter(usuarioId, null);

            return await _daUsuario.ListarTipos(usuario.TipoID.Value);
        }
        public async Task<TiposModel> ObterTipo(int tipoId, int usuarioId)
        {
            var lst = await _daUsuario.ListarTipos(usuarioId);
            TiposModel tipo = lst.Where(x => x.ID == tipoId).SingleOrDefault();
            return tipo;
        }
        public async Task<TiposModel> ObterTipo(int tipoId)
        {
            var tipo = await _daUsuario.ObterTipo(tipoId);
            return tipo;
        }
        public async Task CadastrarTipo(TiposModel model)
        {
            await _daUsuario.InserirTipo(model);
        }
        public async Task DeletarTipo(TiposModel model)
        {
            UsuarioRQModel usuario = new();
            usuario.TipoID = model.ID;

            var ret = await _daUsuario.Listar(usuario);
            if (ret.Count() > 0)
            {
                throw new Exception("Não é possivel deletar Categorias que contém usuários.");
            }

            await _daUsuario.DeletarTipo(model);
        }
        #endregion

        #region Situacoes
        public async Task<List<TiposModel>> ListarSituacoes()
        {
            return await _daUsuario.ListarSituacoes();
        }

        public async Task<int?> CadastroSituacao(TiposModel model)
        {
            if (model.ID.HasValue)
            {
                return await _daUsuario.AtualizarSituacao(model);
            }
            return await _daUsuario.NovaSituacao(model);
        }

        #endregion



    }

}