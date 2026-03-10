using Microsoft.Data.SqlClient;
using Z1.Model.APIs;
using Z3.DataAccess.Database;

namespace Z3.DataAccess.Externo  // TODO: CRIAR UMA API E PASSAR TUDO ISSO PARA LÁ
{
    public interface IAPIsDataAccess
    {
        Task<APIModel> Obter(int? id, int? cod);
        Task<int?> Inserir(APIModel model);
        Task Atualizar(APIModel model);
        Task Deletar(int id);
        Task<List<APIModel>> Listar(int? id);
    }

    public class APIsDataAccess : IAPIsDataAccess
    {
        private readonly IDapper _dapper;
        public APIsDataAccess(IDapper dapper)
        {
            _dapper = dapper;
        }

        public async Task Atualizar(APIModel model)
        {
            try
            {
                string sql = @"
UPDATE dbo.APIs
SET 
COD = COALESCE(@COD, COD),
Token = COALESCE(@Token, Token),
Usuario = COALESCE(@Usuario, Usuario),
Descricao = COALESCE(@Descricao, Descricao),
Senha = COALESCE(@Senha, Senha),
Url = COALESCE(@Url, Url),
Modelo = COALESCE(@Modelo, Modelo)
WHERE ID = @ID
";
                await _dapper.ExecuteAsync(sql: sql, commandType: System.Data.CommandType.Text, param: model);
            }
            catch (SqlException ex)
            {
                TratarErrosSQL.TratarErroUnique(ex);
                throw ex;
            }
        }

        public async Task Deletar(int id)
        {
            try
            {
                string sql = "DELETE FROM dbo.APIs WHERE ID = @ID";
                await _dapper.ExecuteAsync(sql: sql, commandType: System.Data.CommandType.Text, param: new { ID = id });
            }
            catch (SqlException ex)
            {
                throw;
            }
        }

        public async Task<int?> Inserir(APIModel model)
        {
            try
            {
                string sql = @"
INSERT INTO dbo.APIs 
(
COD
,[Descricao]
,[Token]
,[Modelo]
,[Url]
,[Usuario]
,[Senha])
OUTPUT INSERTED.COD
VALUES (
@COD,
@Descricao,
@Token,
@Modelo,
@Url,
@Usuario,
@Senha)
";
                return await _dapper.ExecuteAsync(sql: sql, commandType: System.Data.CommandType.Text, param: model);
            }
            catch (SqlException ex)
            {
                TratarErrosSQL.TratarErroUnique(ex);
                throw;
            }
        }

        public async Task<List<APIModel>> Listar(int? id)
        {
            try
            {
                string sql = @"
SELECT [ID]
,COD
,[Descricao]
,[Token]
,[Modelo]
,[Url]
,[Usuario]
,[Senha] 
FROM dbo.APIs WITH(NOLOCK)
WHERE (@id IS NULL OR ID = @id)
";
                var obj = new
                {
                    ID = id
                };
                return await _dapper.QueryAsync<APIModel>(sql: sql, commandType: System.Data.CommandType.Text, param: obj);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<APIModel> Obter(int? id, int? cod)
        {
            try
            {
                string sql = @"
SELECT 
[ID]
,COD
,[Descricao]
,[Token]
,[Modelo]
,[Url]
,[Usuario]
,[Senha] 
FROM dbo.APIs WITH(NOLOCK)
WHERE (ID = @id OR COD = @cod)
";
                var obj = new
                {
                    ID = id,
                    Cod = cod
                };
                return await _dapper.QueryFirstOrDefaultAsync<APIModel>(sql: sql, commandType: System.Data.CommandType.Text, param: obj);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
