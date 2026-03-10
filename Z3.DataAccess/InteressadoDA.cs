using Microsoft.Data.SqlClient;
using System.Data;
using Z1.Model;
using Z3.DataAccess.Database;

namespace Z3.DataAccess
{
    public interface IInteressadoDA
    {
        Task<List<InteressadoModel>> ListarInteressados(UsuarioRQModel model);
        Task<int?> Adicionar(InteressadoModel model);
        Task Deletar(int id, string? bairro);
        Task<EnderecoModel> ObterEndereco(int pessoaId);
        Task<int?> InserirEndereco(EnderecoModel model);
        Task DeletarEndereco(int pessoaId);
    }

    public class InteressadoDA : IInteressadoDA
    {
        private readonly IDapper _dapper;

        public InteressadoDA(IDapper dapper)
        {
            _dapper = dapper;
        }

        public async Task<List<InteressadoModel>> ListarInteressados(UsuarioRQModel model)
        {
            try
            {
                string sql = "[dbo].[usp_listar_interessados]";

                var obj = new
                {
                    nome = model.NomeCompleto,
                    id = model.PessoaID
                };

                return await _dapper.QueryAsync<InteressadoModel>(sql: sql, commandType: CommandType.StoredProcedure, param: obj);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<int?> Adicionar(InteressadoModel model)
        {
            try
            {
                string sql = @"
INSERT INTO [interessado].[PessoasInteressadas]
           ([NomeCompleto]
           ,[Email]
           ,[CPF]
           ,[Telefone]
           ,[Genero]
           ,[DataNascimento]
           ,[DataCriacao]
           ,[UsuarioLogado]
           ,[FotoPerfilURL]
           ,[FotoPerfilPublicID]
           ,[ProjetoIndicacaoID]
           ,[EstudoID]
           ,[Interessado]
           ,ProfessorID
           ,Usuario
)
    OUTPUT INSERTED.PessoaID
    VALUES
        (
            @NomeCompleto
           ,@Email
           ,@CPF
           ,@Telefone
           ,@Genero
           ,@DataNascimento
           ,@DataCriacao
           ,@UsuarioLogado
           ,@FotoPerfilURL
           ,@FotoPerfilPublicID
           ,@ProjetoIndicacaoID
           ,@EstudoID
           ,@Interessado
           ,@ProfessorID
           ,@Usuario
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

        public async Task Deletar(int id, string? bairro)
        {
            try
            {
                string sql = @"
UPDATE [interessado].[PessoasInteressadas] 
SET 
NomeCompleto = ''
,Email = null
,CPF = null
,Telefone = null
,FotoPerfilURL = null
,FotoPerfilPublicID = null
,Usuario = null
,Bairro = @bairro
,Deletado = '*'
WHERE PessoaID = @id";

                await _dapper.ExecuteAsync(sql: sql, commandType: System.Data.CommandType.Text, param: new { id = id, bairro = bairro });
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<EnderecoModel> ObterEndereco(int pessoaId)
        {
            try
            {
                string sql = @"
SELECT [ID]
      ,PessoaID
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
  FROM  [interessado].[Enderecos] E WITH(NOLOCK)
  WHERE PessoaID = @pessoaId
  ORDER BY ID
";

                var ret = await _dapper.QueryFirstOrDefaultAsync<EnderecoModel>(sql: sql, param: new { pessoaId = pessoaId }, commandType: CommandType.Text);
                return ret;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int?> InserirEndereco(EnderecoModel model)
        {
            try
            {
                string sql = @"
INSERT INTO [interessado].[Enderecos] (
PessoaID
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
)
OUTPUT INSERTED.ID
VALUES (
@PessoaID
,@CEP
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

        public async Task DeletarEndereco(int pessoaId)
        {
            try
            {
                string sql = @"DELETE FROM [interessado].[Enderecos] WHERE PessoaID = @pessoaId";
                await _dapper.ExecuteAsync(sql: sql, param: new { pessoaId = pessoaId }, commandType: CommandType.Text);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
