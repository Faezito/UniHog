using Microsoft.Data.SqlClient;
using System.Data;
using Z1.Model;
using Z3.DataAccess.Database;

namespace Z3.DataAccess
{
    public interface IEstudoDataAccess
    {
        Task<List<EstudoModel>> Listar(int? id);
        Task Inserir(EstudoModel model);
        Task InserirDiasDeEstudo(DiasEstudoModel model);
        Task LimparDiasDeEstudo(int pessoaId);
        Task<List<DiasEstudoModel>> ListarDiasDeEstudoDaPessoa(int pessoaId);
        Task Deletar(EstudoModel model);

    }

    public class EstudoDataAccess : IEstudoDataAccess
    {
        private readonly IDapper _dapper;

        public EstudoDataAccess(IDapper dapper)
        {
           _dapper = dapper; 
        }

        public async Task Inserir(EstudoModel model)
        {
            try
            {
                string sql = @"
INSERT INTO [dbo].[Estudos] (
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

        public async Task InserirDiasDeEstudo(DiasEstudoModel model)
        {
            try
            {
                string sql = @"
INSERT INTO [dbo].[PessoasDiasDeEstudo] ([PessoaID],[DiaID],[Horario])
VALUES (@PessoaID, @DiaID, '07:00:00')
";
                await _dapper.ExecuteAsync(sql: sql, commandType: System.Data.CommandType.Text, param: model);
            }
            catch (SqlException ex)
            {
                throw ex;
            }
        }

        public async Task LimparDiasDeEstudo(int pessoaId)
        {
            try
            {
                string sql = "DELETE FROM [dbo].[PessoasDiasDeEstudo] WHERE PessoaID = @pessoaId";
                await _dapper.ExecuteAsync(sql: sql, commandType: System.Data.CommandType.Text, param: new { pessoaId = pessoaId });
            }
            catch (SqlException ex)
            {
                throw ex;
            }
        }

        public async Task<List<EstudoModel>> Listar(int? id)
        {
            try
            {
                string sql = @"
SELECT [ESTUDOID]
      ,[Descricao]
  FROM [dbo].[Estudos]
WHERE (@id IS NULL OR ESTUDOID = @id)
";
                var ret = await _dapper.QueryAsync<EstudoModel>(sql: sql, commandType: CommandType.Text, param: new { id = id });
                return ret;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<DiasEstudoModel>> ListarDiasDeEstudoDaPessoa(int pessoaId)
        {
            try
            {
                string sql = @"
SELECT ID,[PessoaID],[DiaID],[Horario]
  FROM [dbo].[PessoasDiasDeEstudo]
WHERE (@pessoaId IS NULL OR PessoaID = @pessoaId)
";
                var ret = await _dapper.QueryAsync<DiasEstudoModel>(sql: sql, commandType: CommandType.Text, param: new { pessoaId = pessoaId });
                return ret;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task Deletar(EstudoModel model)
        {
            try
            {
                string sql = @"
DELETE FROM [dbo].[Estudos]
WHERE ESTUDOID = @ESTUDOID
";
                await _dapper.ExecuteAsync(sql: sql, commandType: System.Data.CommandType.Text, param: model);
            }
            catch (SqlException ex)
            {
                TratarErrosSQL.TratarErroUnique(ex);
                throw ex;
            }
        }

    }
}
