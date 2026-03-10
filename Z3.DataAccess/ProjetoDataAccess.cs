using Microsoft.Data.SqlClient;
using System.Data;
using Z1.Model;
using Z3.DataAccess.Database;

namespace Z3.DataAccess
{
    public interface IProjetoDataAccess
    {
        Task<List<ProjetoModel>> Listar(int? id);
        Task Inserir(ProjetoModel model);
        Task Deletar(ProjetoModel model);
        Task Atualizar(ProjetoModel model);

    }

    public class ProjetoDataAccess : IProjetoDataAccess
    {
        private readonly IDapper _dapper;

        public ProjetoDataAccess(IDapper dapper)
        {
           _dapper = dapper; 
        }

        public async Task Atualizar(ProjetoModel model)
        {
            try
            {
                string sql = @"
UPDATE [dbo].[Projetos]
SET
Descricao = @Descricao
WHERE ID = @PROJETOID
";
                await _dapper.ExecuteAsync(sql: sql, commandType: System.Data.CommandType.Text, param: model);
            }
            catch (SqlException ex)
            {
                TratarErrosSQL.TratarErroUnique(ex);
                throw ex;
            }
        }

        public async Task Deletar(ProjetoModel model)
        {
            try
            {
                string sql = @"
DELETE FROM [dbo].[Projetos]
WHERE ID = @PROJETOID
";
                await _dapper.ExecuteAsync(sql: sql, commandType: System.Data.CommandType.Text, param: model);
            }
            catch (SqlException ex)
            {
                TratarErrosSQL.TratarErroUnique(ex);
                throw ex;
            }
        }

        public async Task Inserir(ProjetoModel model)
        {
            try
            {
                string sql = @"
INSERT INTO [dbo].[Projetos] (
Descricao
)
VALUES (
@Descricao
)
";
                await _dapper.ExecuteAsync(sql: sql, commandType: System.Data.CommandType.Text, param: model);
            }
            catch (SqlException ex)
            {
                TratarErrosSQL.TratarErroUnique(ex);
                throw ex;
            }
        }

        public async Task<List<ProjetoModel>> Listar(int? id)
        {
            try
            {
                string sql = @"
SELECT [PROJETOID]
      ,[Descricao]
  FROM [dbo].[Projetos]
WHERE (@id IS NULL OR PROJETOID = @id)
";
                var ret = await _dapper.QueryAsync<ProjetoModel>(sql: sql, commandType: CommandType.Text, param: new { id = id });
                return ret;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
