using System.Transactions;
using Z1.Model;
using Z3.DataAccess;

namespace Z2.Servicos
{
    public interface IEnderecoServicos
    {
        Task AtribuirEndereco(EnderecoModel model, int pessoaId);
        Task<int?> Cadastro(EnderecoModel endereco);
        Task<List<EnderecoModel>> ListarEnderecosDoUsuario(int usuarioId);
        Task<EnderecoModel> ObterEndereco(int usuarioId);
        Task DeletarEnderecoDoUsuario(int enderecoId, int pessoaId);
        Task<List<EnderecoModel>> Listar();
        Task<List<BairroModel>> ListarBairrosDrop(int congregacaoId);

        // TIPOS
        Task<List<TiposModel>> ListarTipos();
        Task<TiposModel> ObterTipo(int tipoId);
        Task CadastrarTipo(TiposModel model);
    }

    public class EnderecoServicos : IEnderecoServicos
    {
        private readonly IUsuarioDataAccess _daUsuario;
        private readonly IEnderecoDataAccess _daEndereco;

        public EnderecoServicos(IUsuarioDataAccess daUsuario, IEnderecoDataAccess daEndereco)
        {
            _daUsuario = daUsuario;
            _daEndereco = daEndereco;
        }

        public async Task AtribuirEndereco(EnderecoModel model, int pessoaId)
        {
            await _daEndereco.AtribuirEndereco(pessoaId, model.ID.Value, model.TipoID.Value);
        }

        public async Task<int?> Cadastro(EnderecoModel endereco)
        {
            if (!endereco.ID.HasValue && endereco.PessoaID.HasValue)
            {
                using (var scope = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled))
                {
                    try
                    {
                        int? enderecoId = await _daEndereco.Inserir(endereco);
                        await _daEndereco.AtribuirEndereco(endereco.PessoaID.Value, enderecoId.Value, endereco.TipoID.Value);
                        scope.Complete();
                        return enderecoId.Value;
                    }
                    catch (Exception ex)
                    {
                        scope.Dispose();
                        throw ex;
                    }
                }

            }
            return await _daEndereco.Atualizar(endereco);
        }

        public async Task DeletarEnderecoDoUsuario(int enderecoId, int pessoaId)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    await _daEndereco.Deletar(enderecoId);
                    await _daEndereco.DeletarEnderecoDoUsuario(enderecoId, pessoaId);
                    scope.Complete();
                }
                catch (Exception ex)
                {
                    scope.Dispose();
                    throw ex;
                }
            }
            ;
        }

        public async Task<List<EnderecoModel>> ListarEnderecosDoUsuario(int usuarioId)
        {
            return await _daEndereco.ListarEnderecosDoUsuario(usuarioId);
        }


        // Tipos
        public async Task<List<TiposModel>> ListarTipos()
        {
            return await _daEndereco.ListarTiposDeEndereco();
        }

        public async Task<TiposModel> ObterTipo(int tipoId)
        {
            var lst = await _daEndereco.ListarTiposDeEndereco();
            TiposModel tipo = lst.Where(x => x.ID == tipoId).SingleOrDefault();
            return tipo;
        }
        public async Task CadastrarTipo(TiposModel model)
        {
            await _daEndereco.InserirTipo(model);
        }

        public async Task<List<EnderecoModel>> Listar()
        {
            return await _daEndereco.Listar();
        }

        public async Task<EnderecoModel> ObterEndereco(int usuarioId)
        {
            var enderecos = await ListarEnderecosDoUsuario(usuarioId);
            EnderecoModel endereco = new();
            if (enderecos != null)
            {
                endereco = enderecos.FirstOrDefault();
            }

            return endereco;
        }

        public async Task<List<BairroModel>> ListarBairrosDrop(int congregacaoId)
        {
            return await _daEndereco.ListarBairrosDrop(congregacaoId);
        }
    }
}