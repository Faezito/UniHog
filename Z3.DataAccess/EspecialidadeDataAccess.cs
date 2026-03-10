using Microsoft.Data.SqlClient;
using System.Data;
using Z1.Model;
using Z3.DataAccess.Database;

namespace Z3.DataAccess
{
    public interface IEspecialidadeDataAccess
    {
        public Task<int?> Inserir(TiposModel model);
        public Task Atualizar(TiposModel model);
        public Task<int?> Inserir(EspecialidadeModel model);
        public Task Atualizar(EspecialidadeModel model);
        public Task Deletar(EspecialidadeModel model);
        public Task<List<EspecialidadeModel>> Listar(EspecialidadeModel model);
        public Task<List<TiposModel>> ListarAreas();
        public Task<List<EspecialistaModel>> ListarEspecialistas(int? areaId);
    }
    public class EspecialidadeDataAccess : IEspecialidadeDataAccess
    {
        private readonly IDapper _dapper;

        public EspecialidadeDataAccess(IDapper dapper)
        {
            _dapper = dapper;
        }

        public async Task Atualizar(EspecialidadeModel model)
        {
            try
            {
                string sql = @"
UPDATE [atendimentos].[Especializacoes] SET
Descricao = @Descricao,
AreaID = @AreaID
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

        public async Task Atualizar(TiposModel model)
        {
            try
            {
                string sql = @"
UPDATE [atendimentos].[Areas] SET
Descricao = @Descricao,
Icone = @Icone
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

        public async Task Deletar(EspecialidadeModel model)
        {
            try
            {
                string sql = @"
DELETE FROM [atendimentos].[Especializacoes]
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

        public async Task<int?> Inserir(EspecialidadeModel model)
        {
            try
            {
                string sql = @"
INSERT INTO [atendimentos].[Especializacoes] (
Descricao,
AreaID
)
OUTPUT INSERTED.ID
VALUES (
@Descricao,
@AreaID
)
";
                return await _dapper.ExecuteAsync(sql: sql, commandType: System.Data.CommandType.Text, param: model);
            }
            catch (SqlException ex)
            {
                TratarErrosSQL.TratarErroUnique(ex);
                throw ex;
            }
        }

        public async Task<int?> Inserir(TiposModel model)
        {
            try
            {
                string sql = @"
INSERT INTO [atendimentos].[Areas] (
Descricao,
Icone
)
OUTPUT INSERTED.ID
VALUES (
@Descricao,
@Icone
)
";

                return await _dapper.ExecuteAsync(sql: sql, commandType: System.Data.CommandType.Text, param: model);
            }
            catch (SqlException ex)
            {
                TratarErrosSQL.TratarErroUnique(ex);
                throw ex;
            }
        }

        public async Task<List<EspecialidadeModel>> Listar(EspecialidadeModel model)
        {
            try
            {
                string sql = @"
SELECT E.ID
      ,E.Descricao
      ,E.AreaID
      ,A.Descricao AS Area
  FROM [atendimentos].[Especializacoes] E
INNER JOIN atendimentos.Areas A ON A.ID = E.AreaID
WHERE (@ID IS NULL OR E.ID = @ID)
AND (@AreaID IS NULL OR AreaID = @AreaID)
ORDER BY AreaID
";
                var ret = await _dapper.QueryAsync<EspecialidadeModel>(sql: sql, commandType: CommandType.Text, param: model);
                return ret;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<TiposModel>> ListarAreas()
        {
            try
            {
                string sql = @"
SELECT ID
      ,Descricao
      ,Icone
  FROM [atendimentos].[Areas]
";
                var ret = await _dapper.QueryAsync<TiposModel>(sql: sql, commandType: CommandType.Text);
                return ret;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<EspecialistaModel>> ListarEspecialistas(int? areaId)
        {
            try
            {
                string sql = @"
        SELECT
        P.PessoaID,
        P.NomeCompleto,
        ES.ID AS EspecializacaoID,
        ES.AreaID,
        ES.Descricao
        FROM [dbo].[Pessoas] P
        INNER JOIN atendimentos.PessoasEspecialidades PES ON PES.PessoaID = P.PessoaID
        INNER JOIN atendimentos.Especializacoes ES ON ES.ID = PES.EspecializacaoID
        WHERE ES.AreaID = @AreaID
";
                var ret = await _dapper.QueryAsync<EspecialistaModel>(sql: sql, commandType: CommandType.Text, param: new { AreaID = areaId });
                return ret;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

}
