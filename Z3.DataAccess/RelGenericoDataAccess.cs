using Z3.DataAccess.Database;

namespace Z3.DataAccess
{
    public interface IRelFaturamentoGenericoDA
    {
        Task<List<dynamic>> Faturamento(int? mes, int ano, string produto);
    }

    public class RelGenericoDataAccess : IRelFaturamentoGenericoDA
    {
        private readonly IDapper _dapper;

        public RelGenericoDataAccess(IDapper dapper)
        {
            _dapper = dapper;
        }

        public async Task<List<dynamic>> Faturamento(int? mes, int ano, string aluno)
        {
            try
            {
                string sql = "[dbo].[usp_rel_controle_presenca]";

                var obj = new
                {
                    mes,
                    ano,
                    aluno
                };

                var result = await _dapper.QueryAsyncDynamic<dynamic>(sql: sql, param: obj, commandType: System.Data.CommandType.StoredProcedure);
                return result.ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
