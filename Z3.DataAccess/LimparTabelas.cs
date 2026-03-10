using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System.Reflection;
using Z3.DataAccess.Database;

namespace Z3.DataAccess
{
    public interface ILimparTabelas
    {
        Task Deletar(string tabela, int meses);
        Task LimparUsuarios();
    }
    public class LimparTabelas : ILimparTabelas
    {
        private readonly IDapper _db;
        private readonly ILogger _logger;

        public LimparTabelas(IDapper db)
        {
            _db = db;
        }

        public async Task Deletar(string tabela, int meses)
        {
            try
            {
                string sql = $@"
DELETE FROM {tabela} WHERE DataDeletado < DATEADD(MONTH, -{meses}, GETDATE())
";
                await _db.ExecuteAsync(sql: sql, commandType: System.Data.CommandType.Text);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task LimparUsuarios()
        {
            try
            {
                string sql = @"[dbo].[usp_LIMPAR_USUARIOS_DELETADOS]";
                await _db.ExecuteAsync(sql: sql, commandType: System.Data.CommandType.StoredProcedure);
                _logger.LogInformation("Limpeza concluída com sucesso!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro na limpeza");
            }
        }
    }
}
