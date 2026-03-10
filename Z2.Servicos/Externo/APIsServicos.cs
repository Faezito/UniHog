using Z1.Model.APIs;
using Z3.DataAccess.Externo;

namespace Z2.Services.Externo
{
    public interface IAPIsServicos
    {
        Task<APIModel> Obter(int? id, int? cod);
        Task<int?> Cadastro(APIModel model);
        Task Deletar(int id);
        Task<List<APIModel>> Listar(int? id);
    }

    public class APIsServicos : IAPIsServicos
    {
        private readonly IAPIsDataAccess _apis;
        public APIsServicos(IAPIsDataAccess apis)
        {
            _apis = apis;
        }

        public async Task<int?> Cadastro(APIModel model)
        {
            int? APIid = null;

            if (model.ID == null)
            {
                APIid = await _apis.Inserir(model);
                return APIid;
            }
            await _apis.Atualizar(model);
            return APIid;
        }

        public async Task Deletar(int id)
        {
            await _apis.Deletar(id);
        }

        public async Task<List<APIModel>> Listar(int? id)
        {
            return await _apis.Listar(id);
        }

        public async Task<APIModel> Obter(int? id, int? cod)
        {
            return await _apis.Obter(id, cod);
        }
    }
}
