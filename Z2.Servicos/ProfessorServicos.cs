using Z1.Model;
using Z3.DataAccess;

namespace Z2.Servicos
{
    public interface IProfessorServicos
    {
        Task<int?> AdicionarChamada(ChamadaCadastroModel model);
        Task Deletar(ChamadaCadastroModel model);
        Task<List<ChamadaModel>> ListarPresenca(ChamadaRQModel model);
    }

    public class ProfessorServicos : IProfessorServicos
    {
        private readonly IUsuarioDataAccess _daUsuario;
        private readonly IEnderecoDataAccess _daEndereco;
        private readonly IProfessorDataAccess _daProfessor;

        public ProfessorServicos(IUsuarioDataAccess daUsuario, IProfessorDataAccess daProfessor)
        {
            _daUsuario = daUsuario;
            _daProfessor = daProfessor;
        }

        public async Task<int?> AdicionarChamada(ChamadaCadastroModel model)
        {
            return await _daProfessor.AdicionarChamada(model);
        }

        public async Task Deletar(ChamadaCadastroModel model)
        {
            //await _daEndereco.Deletar(model.ID.Value);
            throw new NotImplementedException();
        }

        public async Task<List<ChamadaModel>> ListarPresenca(ChamadaRQModel model)
        {
            return await _daProfessor.ListarPresenca(model);
        }
    }
}