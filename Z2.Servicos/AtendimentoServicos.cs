using Z1.Model;
using Z3.DataAccess;

namespace Z2.Servicos
{
    public interface IAtendimentoServicos
    {
        Task<List<DiasDaSemana>> ListarDiasDaSemana();
        Task<List<AtendimentoModel>> ListarAtendimentos(AtendimentoRQModel model);
        Task<AtendimentoModel> Obter(AtendimentoRQModel model);
        Task<int?> Cadastrar(AtendimentoModel model);
        public Task<List<TiposModel>> ListarAreas();
        public Task<List<EspecialistaModel>> ListarEspecialistas(int? areaId);
        public Task<List<Funcionamento>> ListarFuncionamento(AtendimentoModel model);
        Task Deletar(AtendimentoModel model);
    }

    public class AtendimentoServicos : IAtendimentoServicos
    {
        private readonly IAtendimentoDataAccess _atendimentos;
        private readonly IEspecialidadeDataAccess _especialidade;
        private readonly IUsuarioServicos _usuario;

        public AtendimentoServicos(IAtendimentoDataAccess atendimentos, IEspecialidadeDataAccess especialidade, IUsuarioServicos usuario)
        {
            _atendimentos = atendimentos;
            _especialidade = especialidade;
            _usuario = usuario;
        }

        public async Task<int?> Cadastrar(AtendimentoModel model)
        {
            if (!model.ID.HasValue)
            {
                model.SituacaoID = "R";  // R - Reserva, C - Confirmado, E - Excluido, F - Finalizado, A - Ausente, J - Justificado
                return await _atendimentos.Inserir(model);
            }
            return await _atendimentos.Atualizar(model);
        }

        public async Task Deletar(AtendimentoModel model)
        {
            await _atendimentos.Deletar(model);
        }

        public async Task<List<TiposModel>> ListarAreas()
        {
            return await _especialidade.ListarAreas();
        }

        public async Task<List<AtendimentoModel>> ListarAtendimentos(AtendimentoRQModel model)
        {
            var atendimentos = await _atendimentos.ListarAtendimento(model);

            return atendimentos;
        }

        public async Task<List<DiasDaSemana>> ListarDiasDaSemana()
        {
            return await _atendimentos.ListarDiasDaSemana();
        }

        public async Task<List<EspecialistaModel>> ListarEspecialistas(int? areaId)
        {
            return await _especialidade.ListarEspecialistas(areaId);
        }

        public async Task<List<Funcionamento>> ListarFuncionamento(AtendimentoModel model)
        {
            return await _atendimentos.ListarFuncionamento(model);
        }

        public async Task<AtendimentoModel> Obter(AtendimentoRQModel model)
        {
            var lst = await ListarAtendimentos(model);
            var atendimento = lst.FirstOrDefault(x => x.ID == model.ID);
            return atendimento;
        }
    }
}
