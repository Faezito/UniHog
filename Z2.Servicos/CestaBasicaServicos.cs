using System.Transactions;
using System.Web.Helpers;
using Z1.Model;
using Z2.Services.Externo;
using Z3.DataAccess;

namespace Z2.Servicos
{
    public interface ICestaBasicaServicos
    {
        public Task<int?> Cadastro(CestaBasicaModel model);
        public Task AdicionarEstoque(CestaBasicaModel model);
        public Task<List<CestaBasicaModel>> Listar();
        public Task<CestaBasicaModel> Obter(int id);
        public Task DeletarCesta(int id);


        public Task<List<CestaBasicaBeneficiarioModel>> ListarBeneficiarios(DateTime? data, bool? colaboradores);
        public Task<CestaBasicaBeneficiarioModel> ObterBeneficiario(int id);
        public Task<int?> RegistrarSaida(CestaEntregaModel model);
    }
    public class CestaBasicaServicos : ICestaBasicaServicos
    {
        private readonly ICestaBasicaDataAccess _cestaBasicaDA;
        private readonly IEmailServicos _email;
        public CestaBasicaServicos(ICestaBasicaDataAccess cestaBasicaDA, IEmailServicos email)
        {
            _cestaBasicaDA = cestaBasicaDA;
            _email = email;
        }

        public async Task AdicionarEstoque(CestaBasicaModel model)
        {
            await _cestaBasicaDA.AdicionarEstoque(model);
        }

        public async Task<int?> Cadastro(CestaBasicaModel model)
        {
            if (model.ID.HasValue)
            {
                CestaBasicaModel antiga = await Obter(model.ID.Value);
                CestaBasicaModel modificado = new();
                List<string> mudancas = new List<string>();
                int? id = await _cestaBasicaDA.Atualizar(model);

                if (antiga.Descricao != model.Descricao
                    || antiga.Custo != model.Custo
                    || antiga.Quantidade != model.Quantidade
                    || antiga.EstoqueMin != model.EstoqueMin
                    || antiga.Itens != model.Itens
                    )
                {
                    if (antiga.Descricao != model.Descricao)
                        mudancas.Add($"Descrição alterada: {antiga.Descricao} -> {model.Descricao}");

                    if (antiga.Custo != model.Custo)
                        mudancas.Add($"Custo alterado: {antiga.Custo} -> {model.Custo}");

                    if (antiga.Quantidade != model.Quantidade)
                        mudancas.Add($"Quantidade alterada: {antiga.Quantidade} -> {model.Quantidade}");

                    if (antiga.EstoqueMin != model.EstoqueMin)
                        mudancas.Add($"Estoque mínimo alterado: {antiga.EstoqueMin} -> {model.EstoqueMin}");

                    if (antiga.Itens != model.Itens)
                        mudancas.Add($"Itens alterados: {antiga.Itens} -> {model.Itens}");

                    string alteracoes = string.Join("<br />", mudancas);
                    await _email.EnviarCestaPorEmail(model, alteracoes);
                }
                return id.Value;
            }
            return await _cestaBasicaDA.Inserir(model);
        }

        public async Task DeletarCesta(int id)
        {
            await _cestaBasicaDA.DeletarCesta(id);
        }

        public async Task<List<CestaBasicaModel>> Listar()
        {
            return await _cestaBasicaDA.Listar();
        }

        public async Task<CestaBasicaModel> Obter(int id)
        {
            return await _cestaBasicaDA.Obter(id);
        }


        public async Task<List<CestaBasicaBeneficiarioModel>> ListarBeneficiarios(DateTime? data, bool? colaboradores)
        {
            if (colaboradores == true)
            {
                return await _cestaBasicaDA.ListarBeneficiariosPJ();
            }
            return await _cestaBasicaDA.ListarBeneficiarios(data);
        }

        public async Task<int?> RegistrarSaida(CestaEntregaModel model)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    CestaBasicaModel cesta = await _cestaBasicaDA.Obter(model.CestaID.Value);
                    await _cestaBasicaDA.RegistrarSaida(model);
                    await _cestaBasicaDA.RemoverEstoque(cesta);
                    scope.Complete();
                    return cesta.ID.Value;
                }
                catch (Exception ex)
                {
                    scope.Dispose();
                    throw ex;
                }
            }
        }

        public async Task<CestaBasicaBeneficiarioModel> ObterBeneficiario(int id)
        {
            var lst = await ListarBeneficiarios(null, null);
            var pessoa = lst.FirstOrDefault(x => x.PessoaID == id);

            return pessoa;
        }
    }
}
