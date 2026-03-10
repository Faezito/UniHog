using Microsoft.Data.SqlClient;
using System.Data;
using Z1.Model;
using Z3.DataAccess.Database;

namespace Z3.DataAccess
{
    public interface IProfessorDataAccess
    {
        Task<int?> AdicionarChamada(ChamadaCadastroModel model);
        Task Deletar(UsuarioModel model);
        Task<List<ChamadaModel>> ListarPresenca(ChamadaRQModel model);
        Task LimparChamada(int pessoaId);
    }

    public class ProfessorDataAccess : IProfessorDataAccess
    {
        private readonly IDapper _dapper;

        public ProfessorDataAccess(IDapper dapper)
        {
            _dapper = dapper;
        }
        public async Task<int?> AdicionarChamada(ChamadaCadastroModel model)
        {
            {
                try
                {
                    string sql = @"
DELETE FROM [dbo].[Chamada] WHERE ProfessorID = @ProfessorID AND PessoaID = @PessoaID AND DataEstudo = @DataEstudo

INSERT INTO [dbo].[Chamada] (
      [ProfessorID]
      ,[PessoaID]
      ,[DataEstudo]
      ,[DataEstudo2]
      ,[Presente]
      ,[Licao]
      ,[Obs]
      ,[DataCriacao]
      ,EstudoID
      ,CestaBasica
)
OUTPUT INSERTED.ID
VALUES (
      @ProfessorID
      ,@PessoaID
      ,@DataEstudo
      ,@DataEstudo
      ,@Presente
      ,@Licao
      ,@Obs
      ,@DataCriacao
      ,@EstudoID
      ,@CestaBasica
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
        }

        public async Task Deletar(UsuarioModel model)
        {
            try
            {
                string sql = "DELETE FROM [dbo].[Chamada] WHERE ID = @ID";
                await _dapper.ExecuteAsync(sql: sql, commandType: System.Data.CommandType.Text, param: model);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<ChamadaModel>> ListarPresenca(ChamadaRQModel model)
        {
            try
            {
                string sql = @"[dbo].[usp_chamada]";

                var obj = new
                {
                    Aluno = model.Aluno,
                    ProfessorID = model.ProfessorID,
                    Mes = model.Mes,
                    Ano = model.Ano
                };

                var ret = await _dapper.QueryAsync<ChamadaModel>(sql: sql, param: obj, commandType: CommandType.StoredProcedure);
                return ret;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task LimparChamada(int pessoaId)
        {
            try
            {
                string sql = "UPDATE [dbo].[Chamada] SET Deletado = '*' WHERE PessoaID = @pessoaId";

                await _dapper.ExecuteAsync(sql: sql, param: new { pessoaId = pessoaId }, commandType: CommandType.Text);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}


