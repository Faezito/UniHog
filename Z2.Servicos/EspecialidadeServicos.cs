using Z1.Model;
using Z3.DataAccess;

namespace Z2.Servicos
{
    public interface IEspecialidadeServicos
    {
        public Task<int?> Cadastro(EspecialidadeModel model);
        public Task<int?> Cadastro(TiposModel model);
        public Task Deletar(EspecialidadeModel model);
        public Task<List<EspecialidadeModel>> Listar(EspecialidadeModel model);
        public Task<EspecialidadeModel> Obter(int id);
        public Task<List<TiposModel>> ListarAreas();

    }
    public class EspecialidadeServicos : IEspecialidadeServicos
    {
        private readonly IEspecialidadeDataAccess _daEspecialidade;
        public EspecialidadeServicos(IEspecialidadeDataAccess daEspecialidade)
        {
            _daEspecialidade = daEspecialidade;
        }

        public async Task<int?> Cadastro(EspecialidadeModel model)
        {
            if (!model.ID.HasValue)
            {
                return await _daEspecialidade.Inserir(model);
            }
            await _daEspecialidade.Atualizar(model);
            return model.ID;
        }

        public async Task<int?> Cadastro(TiposModel model)
        {
            if (!model.ID.HasValue)
            {
                return await _daEspecialidade.Inserir(model);
            }
            await _daEspecialidade.Atualizar(model);
            return model.ID;
        }

        public async Task Deletar(EspecialidadeModel model)
        {
            await _daEspecialidade.Deletar(model);
        }

        public async Task<List<EspecialidadeModel>> Listar(EspecialidadeModel model)
        {
            return await _daEspecialidade.Listar(model);
        }

        public async Task<List<TiposModel>> ListarAreas()
        {
            return await _daEspecialidade.ListarAreas();
        }

        public async Task<EspecialidadeModel> Obter(int id)
        {
            EspecialidadeModel model = new();
            List<EspecialidadeModel> lst = await Listar(model);

            return lst.Where(x => x.ID == id).SingleOrDefault();
        }
    }
}
