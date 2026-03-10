using Microsoft.Data.SqlClient;
using Z1.Model;
using Z3.DataAccess.Database;

namespace Z3.DataAccess
{
    public interface IMovimentacaoFinanceiraDataAccess
    {
        Task<int?> Inserir(MovimentacaoFinanceiraModel model);
        Task<List<MovimentacaoFinanceiraModel>> Listar(int? mes, int? ano, string? tipo);
        Task<MovimentacaoFinanceiraModel> Obter(int id);
        Task DeletarPagamento(int id);
        Task DeletarParcelas(int PaiId);


        Task<int?> InserirMotivo(TiposModel model);
        Task<List<TiposModel>> ListarMotivos();
        Task DeletarMotivo(int id);
    }
    public class MovimentacaoFinanceiraDataAccess : IMovimentacaoFinanceiraDataAccess
    {
        private readonly IDapper _dapper;
        public MovimentacaoFinanceiraDataAccess(IDapper dapper)
        {
            _dapper = dapper;
        }

        public async Task<int?> Inserir(MovimentacaoFinanceiraModel model)
        {
            try
            {
                string sql = @"
INSERT INTO [dbo].[MovimentacaoFinanceira] (
       [Descricao]
      ,[Tipo]
      ,[Valor]
      ,[QuantidadeParcelas]
      ,[ValorParcela]
      ,[Parcela]
      ,[PessoaID]
      ,Pai
      ,MotivoID
      ,[DataMovimentacao]
      ,[NotaFiscalURL]
      ,[NotaFiscalPublicID]
      ,[UsuarioLogado]
      ,[DataAlteracao]
)
OUTPUT INSERTED.ID
VALUES (
       @Descricao
      ,@Tipo
      ,@Valor
      ,@QuantidadeParcelas
      ,@ValorParcela
      ,@Parcela
      ,@PessoaID
      ,@PaiID
      ,@MotivoID
      ,@DataMovimentacao
      ,@NotaFiscalURL
      ,@NotaFiscalPublicID
      ,@UsuarioLogado
      ,@DataAlteracao
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

        public async Task<List<MovimentacaoFinanceiraModel>> Listar(int? mes, int? ano, string? tipo)
        {
            try
            {
                string sql = @"
SELECT MF.[ID]
      ,MF.[Descricao]
      ,MF.[Tipo]
      ,CASE WHEN MF.Tipo = 'E' THEN 'Entrada' 
            WHEN MF.Tipo = 'S' THEN 'SAIDA'
      END AS TipoTxt
      ,CASE WHEN MF.Tipo = 'E' THEN 'text-success' 
            WHEN MF.Tipo = 'S' THEN 'text-danger'
      END AS TipoCor
      ,MF.MotivoID
      ,M.Motivo
      ,MF.[Valor]
      ,MF.[QuantidadeParcelas]
      ,MF.[ValorParcela]
      ,MF.[Parcela]
      ,MF.[PessoaID]
      ,MF.[DataMovimentacao]
      ,MF.[NotaFiscalURL]
      ,MF.[NotaFiscalPublicID]
      ,P.NomeCompleto AS Usuario
      ,MF.UsuarioLogado
      ,L.Usuario AS UsuarioLogadoString
      ,MF.[DataAlteracao]
      ,MF.Pai AS PaiID
  FROM [dbo].[MovimentacaoFinanceira] MF
INNER JOIN Pessoas P ON P.PessoaID = MF.PessoaID
INNER JOIN Pessoas L ON L.PessoaID = MF.UsuarioLogado
LEFT JOIN MotivoMovimentacao M ON M.ID = MF.MotivoID
WHERE (@tipo IS NULL OR Tipo = @tipo)
AND MONTH(DataMovimentacao) = @mes
AND LEFT(CONVERT(varchar, DataMovimentacao, 120), 4) = @Ano
";
                var param = new
                {
                    Mes = mes,
                    Ano = ano,
                    Tipo = tipo
                };

                return await _dapper.QueryAsync<MovimentacaoFinanceiraModel>(sql: sql, commandType: System.Data.CommandType.Text, param: param);
            }
            catch (SqlException ex)
            {
                TratarErrosSQL.TratarErroUnique(ex);
                throw;
            }
        }

        public async Task<MovimentacaoFinanceiraModel> Obter(int id)
        {
            try
            {
                string sql = @"
SELECT MF.[ID]
      ,MF.[Descricao]
      ,MF.[Tipo]
      ,CASE WHEN MF.Tipo = 'E' THEN 'Entrada' 
            WHEN MF.Tipo = 'S' THEN 'SAIDA'
      END AS TipoTxt
      ,CASE WHEN MF.Tipo = 'E' THEN 'text-success' 
            WHEN MF.Tipo = 'S' THEN 'text-danger'
      END AS TipoCor
      ,MF.MotivoID
      ,M.Motivo
      ,MF.[Valor]
      ,MF.[QuantidadeParcelas]
      ,MF.[ValorParcela]
      ,MF.[Parcela]
      ,MF.[PessoaID]
      ,MF.[DataMovimentacao]
      ,MF.[NotaFiscalURL]
      ,MF.[NotaFiscalPublicID]
      ,P.NomeCompleto AS Usuario
      ,MF.UsuarioLogado
      ,L.Usuario AS UsuarioLogadoString
      ,MF.[DataAlteracao]
      ,MF.Pai AS PaiID
  FROM [dbo].[MovimentacaoFinanceira] MF
INNER JOIN Pessoas P ON P.PessoaID = MF.PessoaID
INNER JOIN Pessoas L ON L.PessoaID = MF.UsuarioLogado
LEFT JOIN MotivoMovimentacao M ON M.ID = MF.MotivoID
    WHERE MF.ID = @id
";
                var param = new
                {
                    id = id
                };

                return await _dapper.QueryFirstOrDefaultAsync<MovimentacaoFinanceiraModel>(sql: sql, commandType: System.Data.CommandType.Text, param: param);
            }
            catch (SqlException ex)
            {
                TratarErrosSQL.TratarErroUnique(ex);
                throw;
            }
        }

        public async Task<int?> InserirMotivo(TiposModel model)
        {
            try
            {
                string sql = @"
INSERT INTO [dbo].[MotivoMovimentacao]
(Motivo)
OUTPUT INSERTED.ID
VALUES
(@Descricao)
";

                return await _dapper.ExecuteAsync(sql: sql, commandType: System.Data.CommandType.Text, param: model);
            }
            catch (SqlException ex)
            {
                TratarErrosSQL.TratarErroUnique(ex);
                throw;
            }
        }

        public async Task<List<TiposModel>> ListarMotivos()
        {
            try
            {
                string sql = @"
SELECT ID
,Motivo AS Descricao
FROM [dbo].[MotivoMovimentacao]
";

                return await _dapper.QueryAsync<TiposModel>(sql: sql, commandType: System.Data.CommandType.Text);
            }
            catch (SqlException ex)
            {
                throw;
            }
        }

        public async Task DeletarMotivo(int id)
        {
            try
            {
                string sql = "DELETE FROM [dbo].[MotivoMovimentacao] WHERE ID = @id";

                await _dapper.ExecuteAsync(sql: sql, commandType: System.Data.CommandType.Text, param: new { id = id });
            }
            catch (SqlException)
            {
                throw;
            }
        }

        public async Task DeletarPagamento(int id)
        {
            try
            {
                string sql = "DELETE FROM [dbo].[MovimentacaoFinanceira] WHERE ID = @id";

                await _dapper.ExecuteAsync(sql: sql, commandType: System.Data.CommandType.Text, param: new { id = id });
            }
            catch (SqlException)
            {
                throw;
            }
        }

        public async Task DeletarParcelas(int PaiId)
        {
            try
            {
                string sql = @"
DELETE FROM [dbo].[MovimentacaoFinanceira] 
WHERE Pai = @PaiId
OR ID = @PaiId
";

                await _dapper.ExecuteAsync(sql: sql, commandType: System.Data.CommandType.Text, param: new { PaiId = PaiId });
            }
            catch (SqlException)
            {
                throw;
            }
        }
    }
}
