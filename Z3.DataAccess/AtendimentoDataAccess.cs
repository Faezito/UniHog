using Microsoft.Data.SqlClient;
using System.Data;
using Z1.Model;
using Z3.DataAccess.Database;

namespace Z3.DataAccess
{
    public interface IAtendimentoDataAccess
    {
        Task<List<AtendimentoModel>> ListarAtendimento(AtendimentoRQModel model);
        Task<List<DiasDaSemana>> ListarDiasDaSemana();
        Task<List<Funcionamento>> ListarFuncionamento(AtendimentoModel model);
        Task<int?> Inserir(AtendimentoModel model);
        Task<int?> Atualizar(AtendimentoModel model);
        Task Deletar(AtendimentoModel model);
    }

    public class AtendimentoDataAccess : IAtendimentoDataAccess
    {
        private readonly IDapper _dapper;
        public AtendimentoDataAccess(IDapper dapper)
        {
            _dapper = dapper;
        }

        public async Task<int?> Atualizar(AtendimentoModel model)
        {
            try
            {
                string sql = @"
UPDATE [atendimentos].[Consultas]
SET
    [NomeCompleto]  = COALESCE(@NomeCompleto, [NomeCompleto]),
    [PacienteID]      = COALESCE(@PacienteID, [PacienteID]),
    [EspecialidadeID]  = COALESCE(@EspecialidadeID, [EspecialidadeID]),
    [AreaID]            = COALESCE(@AreaID, [AreaID]),
    [DataAtendimento]   = COALESCE(@DataAtendimento, [DataAtendimento]),
    [Telefone]          = COALESCE(@Telefone, [Telefone]),
    [DataNascimento]    = COALESCE(@DataNascimento, [DataNascimento]),
    [Custo]           = COALESCE(@Custo, [Custo]),
    [SituacaoID]      = COALESCE(@SituacaoID, [SituacaoID]),
    [Observacao]      = COALESCE(@Observacao, [Observacao]),
    [DataCriacao]     = COALESCE(@DataCriacao, [DataCriacao]),
    [UsuarioLogado]   = COALESCE(@UsuarioLogado, [UsuarioLogado])

OUTPUT      inserted.PacienteID
WHERE       (PacienteID = @ID OR ID = @ID);
";

                return await _dapper.ExecuteAsync(sql: sql, commandType: System.Data.CommandType.Text, param: model);
            }
            catch (SqlException ex)
            {
                TratarErrosSQL.TratarErroUnique(ex);
                throw;
            }
        }

        public async Task Deletar(AtendimentoModel model)
        {
            try
            {
                string sql = @"
UPDATE [atendimentos].[Consultas]
SET
PacienteID = NULL,
NomeCompleto = NULL,
Telefone = NULL,
[Observacao] = NULL,
SituacaoID = 'E',
[DataDeletado] = @DataDeletado,
[UsuarioLogado] = @UsuarioLogado
WHERE PacienteID = @PacienteID;
";

                await _dapper.ExecuteAsync(sql: sql, commandType: System.Data.CommandType.Text, param: model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int?> Inserir(AtendimentoModel model)
        {
            try
            {
                string sql = @"
INSERT INTO [atendimentos].[Consultas]
      ([NomeCompleto]
      ,PacienteID
      ,[Genero]
      ,[DataNascimento]
      ,EspecialidadeID
      ,Telefone
      ,[AreaID]
      ,TipoID   
      ,[DataAtendimento]
      ,[Custo]
      ,[SituacaoID]
      ,[Observacao]
      ,[DataCriacao]
      ,[UsuarioLogado])
VALUES (
       @NomeCompleto
      ,@PacienteID
      ,@Genero
      ,@DataNascimento
      ,@EspecialidadeID
      ,@Telefone
      ,@AreaID
      ,@TipoID
      ,@DataAtendimento
      ,@Custo
      ,@SituacaoID
      ,@Observacao
      ,@DataCriacao
      ,@UsuarioLogado
)
SELECT SCOPE_IDENTITY()
";

                return await _dapper.ExecuteAsync(sql: sql, commandType: System.Data.CommandType.Text, param: model);
            }
            catch (SqlException ex)
            {
                TratarErrosSQL.TratarErroUnique(ex);
                throw;
            }
        }


        public async Task<List<AtendimentoModel>> ListarAtendimento(AtendimentoRQModel model)
        {
            try
            {
                string sql = @"[dbo].[usp_listar_atendimentos]";

                var obj = new
                {
                    ID = model.ID,
                    DataIni = model.DataIni,
                    DataFim = model.DataFim,
                    EspecialidadeID = model.EspecialidadeID,
                    PacienteID = model.PacienteID
                };

                var ret = await _dapper.QueryAsync<AtendimentoModel>(sql: sql, commandType: CommandType.StoredProcedure, param: obj);
                return ret;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<DiasDaSemana>> ListarDiasDaSemana()
        {
            try
            {
                string sql = @"
SELECT [ID] AS DiaID
      ,[Dia]
  FROM [dbo].[DiasDaSemana]
";
                var ret = await _dapper.QueryAsync<DiasDaSemana>(sql: sql, commandType: CommandType.Text);
                return ret;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<Funcionamento>> ListarFuncionamento(AtendimentoModel model)
        {
            try
            {
                string sql = @"
    SELECT [ID]
      ,[EspecialistaID]
      ,[DiaID]
      ,[ManhaAbertura]
      ,[ManhaFechamento]
      ,[TardeAbertura]
      ,[TardeFechamento]
        FROM [atendimentos].[PessoasJuridicasFuncionamento]
        WHERE EspecialistaID = @EspecialistaID
  AND(@PessoaJuridicaID IS NULL OR PessoaJuridicaID = @PessoaJuridicaID)
  AND DiaID = DATEPART(WEEKDAY, @Data)
  AND @Hora BETWEEN
          COALESCE(ManhaAbertura, TardeAbertura)
      AND
          COALESCE(TardeFechamento, ManhaFechamento)
  AND(
         ManhaFechamento IS NULL
      OR TardeAbertura IS NULL
      OR @Hora NOT BETWEEN ManhaFechamento AND TardeAbertura
  )
";

                var ret = await _dapper.QueryAsync<Funcionamento>(sql: sql, commandType: CommandType.Text, param: model);
                return ret;
            }
            catch (Exception ex)
            {
                throw ex;
            }



        }


    }
}
