using Dapper;
using System.Data;
using System.Data.Common;

namespace Z3.DataAccess.Database
{
    public interface IDapper
    {
        Task<int?> ExecuteAsync(string sql, CommandType commandType, object param = null, int? commandTimeout = null, string? nomeTabela = null);

        Task<List<T>> QueryAsync<T>(string sql, CommandType commandType, object param = null, int? commandTimeout = null);
        Task<T> QueryFirstOrDefaultAsync<T>(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null);
        Task<T> QuerySingleOrDefaultAsync<T>(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null);
        Task<List<dynamic>> QueryAsyncDynamic<T>(string sql, CommandType commandType, object param = null, int? commandTimeout = null);
    }

    public class DapperDatabase : IDapper
    {
        private IDbTransaction _transaction;
        private readonly IDbConnection _conn;

        public DapperDatabase(string strProv, string strConn)
        {
            _conn = DbProviderFactories.GetFactory(strProv).CreateConnection();
            _conn.ConnectionString = strConn;
        }
        public async Task<int?> ExecuteAsync(string sql, CommandType commandType, object param = null, int? commandTimeout = null, string? nomeTabela = null)
        {
            try
            {
                var ret = await _conn.ExecuteScalarAsync<int>(sql: sql, param: param, commandTimeout: commandTimeout, commandType: commandType).ConfigureAwait(false);
                return ret;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<T>> QueryAsync<T>(string sql, CommandType commandType, object param = null, int? commandTimeout = null)
        {
            List<T> ret = (List<T>)await _conn.QueryAsync<T>(sql: sql, commandType: commandType, param: param, commandTimeout: commandTimeout).ConfigureAwait(false);
            return ret;
        }

        public async Task<T> QueryFirstOrDefaultAsync<T>(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            try
            {
                return await _conn.QueryFirstOrDefaultAsync<T>(sql: sql, param: param, transaction: transaction, commandTimeout: commandTimeout, commandType: commandType).ConfigureAwait(false);
            }
            catch (System.Exception ex)
            {
                throw;
            }
            finally
            {
                _conn.Close();
            }
        }

        public async Task<T> QuerySingleOrDefaultAsync<T>(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            try
            {
                return await _conn.QuerySingleOrDefaultAsync<T>(sql: sql, param: param, transaction: transaction, commandTimeout: commandTimeout, commandType: commandType).ConfigureAwait(false);
            }
            catch (System.Exception ex)
            {
                throw;
            }
            finally
            {
                _conn.Close();
            }
        }

        public async Task<List<dynamic>> QueryAsyncDynamic<T>(string sql, CommandType commandType, object param = null, int? commandTimeout = null)
        {
            try
            {
                List<dynamic> ret = (List<dynamic>)await _conn.QueryAsync<T>(sql: sql, commandType: commandType, param: param, commandTimeout: commandTimeout).ConfigureAwait(false);
                return ret;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                _conn?.Close();
            }
        }
    }

}