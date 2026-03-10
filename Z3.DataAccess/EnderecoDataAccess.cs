using Microsoft.Data.SqlClient;
using System.Data;
using Z1.Model;
using Z3.DataAccess.Database;

namespace Z3.DataAccess
{
    public interface IEnderecoDataAccess
    {
        Task<List<EnderecoModel>> ListarEnderecosDoUsuario(int usuarioId);
        Task<int?> Inserir(EnderecoModel model);
        Task<int?> Atualizar(EnderecoModel model);
        Task AtribuirEndereco(int usuarioId, int enderecoId, int tipoId);
        Task Deletar(int id);
        Task DeletarEnderecoDoUsuario(int enderecoId, int usuarioId);
        Task DeletarEnderecosDaPessoa(int usuarioId);
        Task<List<EnderecoModel>> Listar();
        Task<List<BairroModel>> ListarBairrosDrop(int congregacaoId);

        // TIPOS
        Task<List<TiposModel>> ListarTiposDeEndereco();
        Task InserirTipo(TiposModel model);

    }
    public class EnderecoDataAccess : IEnderecoDataAccess
    {
        private readonly IDapper _dapper;
        public EnderecoDataAccess(IDapper dapper)
        {
            _dapper = dapper;
        }

        public async Task AtribuirEndereco(int usuarioId, int enderecoId, int tipoId)
        {
            try
            {
                string sql = @"INSERT INTO dbo.PessoasEnderecos (PessoaID, EnderecoID, TipoID) VALUES (@pessoaId,@enderecoId,@tipoId)";
                var obj = new
                {
                    pessoaId = usuarioId,
                    enderecoId = enderecoId,
                    tipoId = tipoId
                };

                await _dapper.ExecuteAsync(sql: sql, param: obj, commandType: CommandType.Text);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Task<int?> Atualizar(EnderecoModel model)
        {
            throw new NotImplementedException();
        }

        public async Task Deletar(int id)
        {
            try
            {
                string sql = "UPDATE dbo.Enderecos SET Deletado = '*' WHERE ID = @enderecoId";

                await _dapper.ExecuteAsync(sql: sql, param: new { enderecoId = id }, commandType: CommandType.Text);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task DeletarEnderecoDoUsuario(int enderecoId, int usuarioId)
        {
            try
            {
                string sql = "UPDATE dbo.PessoasEnderecos SET Deletado = '*' WHERE PessoaID = @pessoaId AND EnderecoID = @enderecoId";

                await _dapper.ExecuteAsync(sql: sql, param: new { pessoaId = usuarioId, enderecoId = enderecoId }, commandType: CommandType.Text);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task DeletarEnderecosDaPessoa(int pessoaId)
        {
            try
            {
                string sql = "UPDATE dbo.PessoasEnderecos SET Deletado = '*' WHERE PessoaID = @pessoaId";

                await _dapper.ExecuteAsync(sql: sql, param: new { pessoaId = pessoaId }, commandType: CommandType.Text);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<int?> Inserir(EnderecoModel model)
        {
            try
            {
                string sql = @"
INSERT INTO [dbo].[Enderecos] (
[CEP]
,[Numero]
,[Rua]
,[Bairro]
,[Cidade]
,[UF]
,[Estado]
,[Pais]
,[Complemento]
,[Regiao]
,[Referencia]
)
OUTPUT INSERTED.ID
VALUES (
@CEP
,@Numero
,@Rua
,@Bairro
,@Cidade
,@UF
,@Estado
,'Brasil'
,@Complemento
,@Regiao
,@Referencia
)
";
                return await _dapper.ExecuteAsync(sql: sql, param: model, commandType: CommandType.Text);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<EnderecoModel>> ListarEnderecosDoUsuario(int usuarioId)
        {
            try
            {
                string sql = @"
SELECT E.[ID]
      ,PE.PessoaID
      ,[CEP]
      ,[Numero]
      ,[Rua]
      ,[Bairro]
      ,[Cidade]
      ,[UF]
      ,[Estado]
      ,[Pais]
      ,[Complemento]
      ,[Regiao]
      ,[Referencia]
      ,CASE 
          WHEN E.Complemento IS NOT NULL THEN (E.Rua + ', ' + E.Numero + ', ' + E.Complemento + ', ' + E.Bairro + ', ' + E.Cidade + ', ' + E.UF) 
          ELSE (E.Rua + ', ' + E.Numero + ', ' + E.Bairro + ', ' + E.Cidade + ' - ' + E.UF) 
      END
      AS EnderecoCompleto
      ,CASE 
          WHEN E.Complemento IS NOT NULL THEN (E.Rua + ', ' + E.Numero + ', ' + E.Complemento) 
          ELSE (E.Rua + ', ' + E.Numero) 
      END
      AS EnderecoReduzido
      ,ET.Descricao AS Tipo
      ,TipoID
       
  FROM  [dbo].[Enderecos] E WITH(NOLOCK)
  INNER JOIN dbo.PessoasEnderecos PE WITH(NOLOCK) ON PE.PessoaID = @PessoaID AND PE.EnderecoID = E.ID
  INNER JOIN [dbo].[EnderecosTipos] ET WITH(NOLOCK) ON ET.ID = PE.TipoID
  WHERE E.Deletado IS NULL
  ORDER BY ET.ID
";

                var ret = await _dapper.QueryAsync<EnderecoModel>(sql: sql, param: new { PessoaID = usuarioId }, commandType: CommandType.Text);
                return ret;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        // TIPOS
        public async Task<List<TiposModel>> ListarTiposDeEndereco()
        {
            try
            {
                string sql = @"
SELECT [ID]
      ,[Descricao]
  FROM [dbo].[EnderecosTipos]
";
                var ret = await _dapper.QueryAsync<TiposModel>(sql: sql, commandType: CommandType.Text);
                return ret;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task InserirTipo(TiposModel model)
        {
            try
            {
                string sql = @"
INSERT INTO [dbo].[EnderecosTipos] (
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
                throw;
            }
        }
        public async Task<List<EnderecoModel>> Listar()
        {
            try
            {
                string sql = "SELECT * FROM [dbo].[Enderecos]";
                var ret = await _dapper.QueryAsync<EnderecoModel>(sql: sql, commandType: CommandType.Text);
                return ret;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<BairroModel>> ListarBairrosDrop(int congregacaoId)
        {
            {
                try
                {
                    string sql = @"
SELECT [ID]
      ,[Nome]
      ,CongregacaoID
  FROM [dbo].[Bairros]
WHERE CongregacaoID = @congregacaoId
";

                    var ret = await _dapper.QueryAsync<BairroModel>(param: new { congregacaoId = congregacaoId },
                        sql: sql,
                        commandType: CommandType.Text);
                    return ret;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

    }
}