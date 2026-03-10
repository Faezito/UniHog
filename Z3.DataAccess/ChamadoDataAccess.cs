using Z1.Model;
using Z3.DataAccess.Database;

namespace Z3.DataAccess
{
    public interface IChamadoDataAccess
    {
        Task<int?> Inserir(ChamadoModel chamado);
        Task<int?> Atualizar(ChamadoModel chamado);
    }
    public class ChamadoDataAccess : IChamadoDataAccess
    {
        private readonly IDapper _dapper;
        public ChamadoDataAccess(IDapper dapper)
        {
            _dapper = dapper;
        }

        public async Task<int?> Inserir(ChamadoModel chamado)
        {
            try
            {
                string sql = @"
INSERT INTO ti.Chamados (
[titulo]	
,[descricao]
,[usuarioId]	
,[dataAbertura]
,[dataFechamento]
,[prioridade]	
,[status]	
,[email]	
,[DataAlteracao]	
,[UsuarioLogado]  
)
OUTPUT INSERTED.id
VALUES (
@titulo	
,@descricao
,@usuarioId
,@dataAbertura
,@dataFechamento
,@prioridade	
,@status
,@email
,@DataAlteracao
,@UsuarioLogado
)
";
                int? id = await _dapper.ExecuteAsync(sql: sql, System.Data.CommandType.Text, param: chamado);
                return id;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<int?> Atualizar(ChamadoModel chamado)
        {
            try
            {
                string sql = @"
UPDATE ti.Chamados
[dataFechamento] = COALESCE(@dataFechamento, [dataFechamento]),
[prioridade]     = COALESCE(@prioridade, [prioridade]),
[status]         = COALESCE(@status, [status]),
[DataAlteracao]  = COALESCE(@DataAlteracao, [DataAlteracao]),
[UsuarioLogado]  = COALESCE(@UsuarioLogado, [UsuarioLogado])
OUTPUT INSERTED.id
WHERE id = @id
";
                int? id = await _dapper.ExecuteAsync(sql: sql, System.Data.CommandType.Text, param: chamado);
                return id;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
