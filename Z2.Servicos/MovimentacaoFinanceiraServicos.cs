using Z1.Model;
using Z3.DataAccess;

namespace Z2.Servicos
{
    public interface IMovimentacaoFinanceiraServicos
    {
        Task Inserir(MovimentacaoFinanceiraModel model);
        Task<List<MovimentacaoFinanceiraModel>> Listar(int? mes, int? ano, string? tipo);
        Task<MovimentacaoFinanceiraModel> Obter(int id);
        Task DeletarPagamento(MovimentacaoFinanceiraModel pagamento);


        Task<int?> InserirMotivo(TiposModel model);
        Task<List<TiposModel>> ListarMotivos();
        Task DeletarMotivo(int id);

    }
    public class MovimentacaoFinanceiraServicos : IMovimentacaoFinanceiraServicos
    {
        private readonly IMovimentacaoFinanceiraDataAccess _daMov;
        public MovimentacaoFinanceiraServicos(IMovimentacaoFinanceiraDataAccess daMov)
        {
            _daMov = daMov;
        }

        public async Task DeletarMotivo(int id)
        {
            await _daMov.DeletarMotivo(id);
        }

        public async Task DeletarPagamento(MovimentacaoFinanceiraModel pagamento)
        {
            if (pagamento.PaiID.HasValue && pagamento.QuantidadeParcelas > 0)
            {
                await _daMov.DeletarParcelas(pagamento.PaiID.Value);
            }
            await _daMov.DeletarPagamento(pagamento.ID.Value);
        }

        public async Task Inserir(MovimentacaoFinanceiraModel model)
        {
            if (model.QuantidadeParcelas > 0)
            {
                if (model.ValorParcela == null)
                    model.ValorParcela = model.Valor / model.QuantidadeParcelas;

                model.Parcela = 1;
                int? paiId = await _daMov.Inserir(model);

                model.PaiID = paiId;
                int mes = 1;
                for (int i = 2; i <= model.QuantidadeParcelas; i++)
                {
                    model.Parcela = i;
                    model.DataMovimentacao = model.DataMovimentacao?.AddMonths(1);
                    await _daMov.Inserir(model);
                    mes++;
                }
            }
            else
            {
                model.Parcela = 0;
                model.ValorParcela = model.Valor;
                await _daMov.Inserir(model);
            }
        }

        public async Task<int?> InserirMotivo(TiposModel model)
        {
            return await _daMov.InserirMotivo(model);
        }

        public async Task<List<MovimentacaoFinanceiraModel>> Listar(int? mes, int? ano, string? tipo)
        {
            return await _daMov.Listar(mes, ano, tipo);
        }

        public async Task<List<TiposModel>> ListarMotivos()
        {
            return await _daMov.ListarMotivos();
        }

        public Task<MovimentacaoFinanceiraModel> Obter(int id)
        {
            return _daMov.Obter(id);
        }
    }
}
