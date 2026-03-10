using Z3.DataAccess;
using Z4.Bibliotecas.Exceptions;

namespace Z1.Model
{
    public interface IProjetoServicos
    {
        Task<List<ProjetoModel>> Listar(int? id);
        Task<ProjetoModel> Obter(int id);
        Task Cadastro(ProjetoModel model);
        Task Deletar(ProjetoModel model);
    }

    public class ProjetoServicos : IProjetoServicos
    {
        private readonly IProjetoDataAccess _projeto;
        private readonly IUsuarioDataAccess _usuario;

        public ProjetoServicos(IProjetoDataAccess Projeto, IUsuarioDataAccess usuario)
        {
            _projeto = Projeto;
            _usuario = usuario;
        }

        public async Task Cadastro(ProjetoModel model)
        {
            if (model.PROJETOID.HasValue)
            {
                await _projeto.Atualizar(model);
            }
            await _projeto.Inserir(model);
        }

        public async Task Deletar(ProjetoModel model)
        {
            var usuarios = await _usuario.Listar(model.PROJETOID.Value, null);
            if (usuarios.Count() > 0)
            {
                List<string>? usuariosNomes = usuarios.Select(x => x.NomeCompleto).ToList();
                throw new ListException("Não é possível excluir Projetos que contém pessoas participando", usuariosNomes);
            }

            await _projeto.Deletar(model);
        }

        public async Task<List<ProjetoModel>> Listar(int? id)
        {
            return await _projeto.Listar(id);
        }

        public async Task<ProjetoModel> Obter(int id)
        {
            var lst = await _projeto.Listar(id);
            return lst.SingleOrDefault();
        }
    }
}
