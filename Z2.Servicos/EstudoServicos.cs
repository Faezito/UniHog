using Z3.DataAccess;
using Z4.Bibliotecas.Exceptions;

namespace Z1.Model
{
    public interface IEstudoServicos
    {
        Task<List<EstudoModel>> Listar(int? id);
        Task<EstudoModel> Obter(int id);
        Task Cadastro(EstudoModel model);
        Task InserirDiasDeEstudo(DiasEstudoModel model);
        Task LimparDiasDeEstudo(int pessoaId);
        Task<List<DiasEstudoModel>> ListarDiasDeEstudoDaPessoa(int pessoaId);
        Task AtualizarDias(UsuarioModel model, int pessoaId);
        Task Deletar(EstudoModel model);
    }

    public class EstudoServicos : IEstudoServicos
    {
        private readonly IEstudoDataAccess _estudo;
        private readonly IUsuarioDataAccess _usuario;

        public EstudoServicos(IEstudoDataAccess estudo, IUsuarioDataAccess usuario)
        {
            _estudo = estudo;
            _usuario = usuario;

        }

        public async Task Cadastro(EstudoModel model)
        {
            await _estudo.Inserir(model);
        }

        public async Task InserirDiasDeEstudo(DiasEstudoModel model)
        {
            await _estudo.InserirDiasDeEstudo(model);
        }

        public async Task LimparDiasDeEstudo(int pessoaId)
        {
            await _estudo.LimparDiasDeEstudo(pessoaId);
        }

        public async Task<List<EstudoModel>> Listar(int? id)
        {
            return await _estudo.Listar(id);
        }

        public async Task<List<DiasEstudoModel>> ListarDiasDeEstudoDaPessoa(int pessoaId)
        {
            return await _estudo.ListarDiasDeEstudoDaPessoa(pessoaId);
        }

        public async Task<EstudoModel> Obter(int id)
        {
            var lst = await _estudo.Listar(id);
            return lst.SingleOrDefault();
        }

        public async Task AtualizarDias(UsuarioModel model, int pessoaId)
        {
            DiasEstudoModel dias = new();
            if (model.DiasEstudoIDs != null && model.DiasEstudoIDs.Length > 0)
            {
                await _estudo.LimparDiasDeEstudo(pessoaId);
                foreach (int dia in model.DiasEstudoIDs)
                {
                    dias.PessoaID = pessoaId;
                    dias.DiaID = dia;
                    await _estudo.InserirDiasDeEstudo(dias);
                }
            }
        }

        public async Task Deletar(EstudoModel model)
        {
            var usuarios = await _usuario.Listar(null, model.ESTUDOID.Value);
            if (usuarios.Count() > 0)
            {
                List<string>? usuariosNomes = usuarios.Select(x => x.NomeCompleto).ToList();
                throw new ListException("Não é possível excluir Estudos que contém pessoas participando", usuariosNomes);
            }

            await _estudo.Deletar(model);
        }
    }
}
