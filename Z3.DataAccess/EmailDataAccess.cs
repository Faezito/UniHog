using Microsoft.Data.SqlClient;
using Z1.Model.Email;
using Z3.DataAccess.Database;

namespace Z3.DataAccess
{
    public interface IEmailDataAccess
    {
        Task<EmailConfig> Obter(int id);
        Task<List<EmailConfig>> Listar(int? id);
        Task<int?> Inserir(EmailConfig model);
        Task<int?> Atualizar(EmailConfig model);
        Task Deletar(int id);
    }
    public class EmailDataAccess : IEmailDataAccess
    {
        private readonly IDapper _dapper;

        public EmailDataAccess(IDapper dapper)
        {
            _dapper = dapper;
        }

        public async Task<int?> Atualizar(EmailConfig model)
        {
            try
            {
                string sql = @"
UPDATE [dbo].[ServicosEmail] SET
Descricao = COALESCE(@Descricao, Descricao),
Email = COALESCE(@Email, Email),
Server = COALESCE(@Server, Server),
Port = COALESCE(@Port, Port),
UseSSL = COALESCE(@UseSSL, UseSSL),
UseStartTls = COALESCE(@UseStartTls, UseStartTls),
Username = COALESCE(@Username, Username),
Password = COALESCE(@Password, Password),
Remetente = COALESCE(@Remetente, Remetente),
FromName = COALESCE(@FromName, FromName)
OUTPUT INSERTED.ID
WHERE ID = @ID
";

                return await _dapper.ExecuteAsync(sql: sql, commandType: System.Data.CommandType.Text, param: model);
            }
            catch (SqlException ex)
            {
                TratarErrosSQL.TratarErroUnique(ex);
                throw;
            }
        }

        public async Task Deletar(int id)
        {
            try
            {
                string sql = @"DELETE FROM [dbo].[ServicosEmail] WHERE ID = @id";

                await _dapper.ExecuteAsync(sql: sql, commandType: System.Data.CommandType.Text, param: new { id = id });
            }
            catch (SqlException ex)
            {
                TratarErrosSQL.TratarErroUnique(ex);
                throw;
            }
        }

        public async Task<int?> Inserir(EmailConfig model)
        {
            try
            {
                string sql = @"
INSERT INTO [dbo].[ServicosEmail]
(
Descricao
,Email
,Server
,Port
,UseSSL
,UseStartTls
,Username
,Password
,FromName
,Remetente
)
OUTPUT INSERTED.ID
VALUES
(
@Descricao
,@Email
,@Server
,@Port
,@UseSSL
,@UseStartTls
,@Username
,@Password
,@FromName
,@Remetente
)
";

                return await _dapper.ExecuteAsync(sql: sql, commandType: System.Data.CommandType.Text, param: model);
            }
            catch (SqlException ex)
            {
                TratarErrosSQL.TratarErroUnique(ex);
                throw;
            }
        }

        public async Task<List<EmailConfig>> Listar(int? id)
        {
            try
            {
                string sql = @"
SELECT [ID]
      ,[Descricao]
      ,[Email]
      ,[Server]
      ,[Port]
      ,[UseSSL]
      ,[UseStartTls]
      ,[Username]
      ,[Password]
      ,[FromName]
      ,Remetente
FROM [dbo].[ServicosEmail] WITH(NOLOCK)
WHERE (@id IS NULL OR ID = @id)
";
                var obj = new
                {
                    id = id
                };

                return await _dapper.QueryAsync<EmailConfig>(sql: sql, commandType: System.Data.CommandType.Text, param: obj);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<EmailConfig> Obter(int id)
        {
            try
            {
                string sql = @"
SELECT [ID]
      ,[Descricao]
      ,[Email]
      ,[Server]
      ,[Port]
      ,[UseSSL]
      ,[UseStartTls]
      ,[Username]
      ,[Password]
      ,[FromName]
      ,Remetente
FROM [dbo].[ServicosEmail] WITH(NOLOCK)
WHERE ID = @id
";
                var obj = new
                {
                    id = id
                };

                return await _dapper.QuerySingleOrDefaultAsync<EmailConfig>(sql: sql, commandType: System.Data.CommandType.Text, param: obj);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}