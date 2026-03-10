using System.Transactions;
using Z1.Model;
using Z3.DataAccess;

namespace Z2.Servicos
{
    public interface IPessoaJuridicaServicos
    {
        Task<List<PessoaJuridicaModel>> Listar(PessoaJuridicaRQModel model);
        Task<PessoaJuridicaModel> Obter(int? empresaId);
        Task<int?> Cadastro(PessoaJuridicaModel model);
        Task Deletar(PessoaJuridicaModel model);
    }
    public class PessoaJuridicaServicos : IPessoaJuridicaServicos
    {
        private readonly IPessoaJuridicaDataAccess _pj;
        private readonly IEnderecoServicos _enderecos;

        public PessoaJuridicaServicos(IPessoaJuridicaDataAccess pj, IEnderecoServicos enderecos)
        {
            _pj = pj;
            _enderecos = enderecos;
        }
        public async Task<int?> Cadastro(PessoaJuridicaModel model)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (!model.PessoaJuridicaID.HasValue)
                    {
                        model.PessoaJuridicaID = await _pj.Inserir(model);
                        await _pj.AtribuirEspecialidade(model);

                        //foreach (var dia in model.Funcionamento)
                        //{
                        //    dia.PessoaJuridicaID = model.PessoaJuridicaID;
                        //    dia.EspecialistaID = model.PessoaID;
                        //    await _pj.LimparFuncionamento(dia);
                        //    await _pj.AdicionarFuncionamento(dia);
                        //}
                    }
                    else
                    {
                        await _pj.Atualizar(model);

                        //foreach (var dia in model.Funcionamento)
                        //{
                        //    dia.PessoaJuridicaID = model.PessoaJuridicaID;
                        //    dia.EspecialistaID = model.PessoaID;
                        //    await _pj.LimparFuncionamento(dia);
                        //    await _pj.AdicionarFuncionamento(dia);
                        //}
                    }

                    scope.Complete();
                    return model.PessoaJuridicaID;
                }
                catch (Exception ex)
                {
                    scope.Dispose();
                    throw;
                }
            }
        }

        public async Task Deletar(PessoaJuridicaModel model)
        {
            await _pj.Deletar(model);
        }

        public async Task<List<PessoaJuridicaModel>> Listar(PessoaJuridicaRQModel model)
        {
            List<PessoaJuridicaModel> lst = await _pj.Listar(model);
            return lst;
        }

        public async Task<PessoaJuridicaModel> Obter(int? empresaId)
        {
            PessoaJuridicaRQModel pj = new();
            pj.PessoaJuridicaID = empresaId;

            List<PessoaJuridicaModel> lst = await Listar(pj);
            PessoaJuridicaModel empresa = lst.SingleOrDefault();
            //empresa.Funcionamento = await _pj.ListarFuncionamento(empresa.PessoaJuridicaID, empresa.PessoaID);

            return empresa;
        }
    }
}
