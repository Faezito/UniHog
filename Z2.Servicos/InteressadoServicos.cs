using System.Transactions;
using Z1.Model;
using Z3.DataAccess;

namespace Z2.Servicos
{

    public interface IInteressadoServicos
    {
        Task<List<InteressadoModel>> Listar(UsuarioRQModel model);
        Task<int?> Cadastrar(InteressadoCadastroModel model);
        Task Deletar(int id);
        Task<InteressadoModel> Obter(int? id);

        Task<EnderecoModel> ObterEndereco(int pessoaId);
    }

    public class InteressadoServicos : IInteressadoServicos
    {
        private readonly IUsuarioDataAccess _daUsuario;
        private readonly IInteressadoDA _interessado;
        private readonly IEnderecoDataAccess _daEndereco;
        private readonly IEstudoServicos _estudo;
        private readonly ICloudinaryServicos _cloudinary;

        public InteressadoServicos
            (IUsuarioDataAccess daUsuario, 
            IEnderecoDataAccess daEndereco, 
            IEstudoServicos estudo, 
            IInteressadoDA interessado,
            ICloudinaryServicos cloudinary
            )
        {
            _daUsuario = daUsuario;
            _daEndereco = daEndereco;
            _estudo = estudo;
            _interessado = interessado;
            _cloudinary = cloudinary;
        }

        public async Task<int?> Cadastrar(InteressadoCadastroModel model)
        {
            int? pessoaId;
            int? enderecoId;

            InteressadoModel usuario = new InteressadoModel
            {
                PessoaID = model.PessoaID,
                NomeCompleto = model.NomeCompleto,
                Email = model.Email,
                CPF = model.CPF,
                Interessado = model.Interessado,
                Telefone = model.Telefone,
                Genero = model.Genero,
                FotoPerfilURL = model.FotoPerfilURL,
                FotoPerfilPublicID = model.FotoPerfilPublicID,
                DataNascimento = model.DataNascimento,
                ProfessorID = model.ProfessorID,
                EstudoID = model.EstudoID,
                ProjetoIndicacaoID = model.ProjetoIndicacaoID,
                DiasEstudoIDs = model.DiasEstudoIDs,
                DataCriacao = DateTime.Now,
                DataAlteracao = DateTime.Now,
                UsuarioLogado = model.UsuarioLogado,
                Usuario = model.Usuario.ToLower().Trim()
            };

            EnderecoModel endereco = new EnderecoModel
            {
                CEP = model.CEP,
                Rua = model.Rua,
                Numero = model.Numero,
                Bairro = model.Bairro,
                Cidade = model.Cidade,
                Estado = model.Estado,
                Pais = model.Pais ?? "Brasil",
                UF = model.UF,
                Regiao = model.Regiao,
                Complemento = model.Complemento,
                Referencia = model.Referencia,
                TipoID = model.EnderecoTipoID ?? 1
            };

            model.DataCriacao = DateTime.Now;

            using (var scope = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    pessoaId = await _interessado.Adicionar(usuario);

                    endereco.PessoaID = pessoaId;
                    enderecoId = await _interessado.InserirEndereco(endereco);
                    scope.Complete();
                }
                catch (Exception ex)
                {
                    scope.Dispose();
                    throw ex;
                }
            }

            return pessoaId;
        }

        public async Task Deletar(int id)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    InteressadoModel model = await Obter(id);

                    var endereco = await _interessado.ObterEndereco(model.PessoaID.Value);
                    await _interessado.Deletar(id, endereco.Bairro);
                    await _interessado.DeletarEndereco(id);

                    if (!string.IsNullOrWhiteSpace(model.FotoPerfilPublicID))
                    {
                        await _cloudinary.DeletarImagem(model.FotoPerfilPublicID);
                    }
                    scope.Complete();
                }
                catch (Exception)
                {
                    scope.Dispose();
                    throw;
                }
            }
        }

        public async Task<List<InteressadoModel>> Listar(UsuarioRQModel model)
        {
            List<InteressadoModel> lst = await _interessado.ListarInteressados(model);

            foreach (var usuario in lst)
            {
                EnderecoModel endereco = await _interessado.ObterEndereco(usuario.PessoaID.Value);

                var enderecoCompleto = $"{endereco.Rua}, {endereco.Numero}, {endereco.Bairro}, {endereco.Cidade} - {endereco.Estado}, {endereco.CEP}";
                var enderecoEncoded = Uri.EscapeDataString(enderecoCompleto);
                endereco.EnderecoMapsURL = $"https://www.google.com/maps/search/?api=1&query={enderecoEncoded}";

                usuario.Enderecos.Add(endereco);
            }
            return lst;
        }

        public async Task<InteressadoModel> Obter(int? id)
        {
            UsuarioRQModel model = new UsuarioRQModel
            {
                PessoaID = id
            };
            var lst = await Listar(model);
            return lst.SingleOrDefault();
        }

        public async Task<EnderecoModel> ObterEndereco(int pessoaId)
        {
            return await _interessado.ObterEndereco(pessoaId);
        }
    }

}