using Microsoft.Data.SqlClient;
using System.Data;
using Z1.Model;
using Z3.DataAccess.Database;

namespace Z3.DataAccess
{
    public interface IPessoaJuridicaDataAccess
    {
        Task<List<PessoaJuridicaModel>> Listar(PessoaJuridicaRQModel model);
        Task<int?> Inserir(PessoaJuridicaModel model);
        Task Atualizar(PessoaJuridicaModel model);
        Task Deletar(PessoaJuridicaModel model);
        Task AtribuirEspecialidade(PessoaJuridicaModel model);


        // EM DESUSO, MAS PODEM SER ÚTEIS NO FUTURO
        Task AtribuirPessoaEmpresa(PessoaJuridicaModel model);
        Task AdicionarFuncionamento(Funcionamento model);
        Task LimparFuncionamento(Funcionamento model);
        Task<List<Funcionamento>> ListarFuncionamento(int? empresaId, int? especialistaId);
    }

    public class PessoaJuridicaDataAccess : IPessoaJuridicaDataAccess
    {
        private readonly IDapper _dapper;
        public PessoaJuridicaDataAccess(IDapper dapper)
        {
            _dapper = dapper;
        }

        public async Task AdicionarFuncionamento(Funcionamento model)
        {
            try
            {
                string sql = @"
INSERT INTO [atendimentos].[PessoasJuridicasFuncionamento]
(
     [EspecialistaID]
    ,[DiaID]
    ,[ManhaAbertura]
    ,[ManhaFechamento]
    ,[TardeAbertura]
    ,[TardeFechamento]
)
VALUES (
     @EspecialistaID
    ,@DiaID
    ,@ManhaAbertura
    ,@ManhaFechamento
    ,@TardeAbertura
    ,@TardeFechamento
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

        public async Task LimparFuncionamento(Funcionamento model)
        {
            try
            {
                string sql = @"
DELETE FROM [atendimentos].[PessoasJuridicasFuncionamento] 
WHERE EspecialistaID = @EspecialistaID 
";

                await _dapper.ExecuteAsync(sql: sql, commandType: System.Data.CommandType.Text, param: model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task AtribuirPessoaEmpresa(PessoaJuridicaModel model)
        {
            try
            {
                string sql = @"
INSERT INTO [atendimentos].[PessoasEmpresas]
(
       [PessoaID]
      ,[PessoaJuridicaID]
)
VALUES (
       @PessoaID
      ,@PessoaJuridicaID
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

        public async Task Atualizar(PessoaJuridicaModel model)
        {
            try
            {
                string sql = @"
UPDATE [dbo].[PessoasJuridicas]
SET 
       [NomeFantasia]     = COALESCE(@NomeFantasia, [NomeFantasia])
      ,[NomeEmpresarial]  = COALESCE(@RazaoSocial, [NomeEmpresarial])
      ,[Registro]         = COALESCE(@Registro, [Registro])
      ,[CNPJ]             = COALESCE(@CNPJ, [CNPJ])
      ,[Telefone]         = COALESCE(@Telefone, [Telefone])
      ,[Email]            = COALESCE(@Email, [Email])
      ,[AreaID]           = COALESCE(@AreaID, [AreaID])
      ,[Ativo]            = COALESCE(@Ativo, [Ativo])
      ,[Endereco]         = COALESCE(@Endereco, [Endereco])
      ,[Bairro]           = COALESCE(@Bairro, [Bairro])
      ,[Cidade]           = COALESCE(@Cidade, [Cidade])
      ,[UF]               = COALESCE(@UF, [UF])
      ,[Estado]           = COALESCE(@Estado, [Estado])
      ,[Regiao]           = COALESCE(@Regiao, [Regiao])
      ,[CEP]               = COALESCE(@CEP, [CEP])
      ,[UsuarioLogado]    = COALESCE(@UsuarioLogado, [UsuarioLogado])
      ,[DataAlteracao]    = COALESCE(@DataAlteracao, [DataAlteracao])
OUTPUT INSERTED.PessoaJuridicaID
WHERE PessoaJuridicaID = @PessoaJuridicaID
";

                await _dapper.ExecuteAsync(sql: sql, commandType: System.Data.CommandType.Text, param: model);
            }
            catch (SqlException ex)
            {
                TratarErrosSQL.TratarErroUnique(ex);
                throw;
            }
        }

        public async Task Deletar(PessoaJuridicaModel model)
        {
            try
            {
                string sql = @"
UPDATE [dbo].[PessoasJuridicas]
SET 
      [UsuarioLogado]    = COALESCE(@UsuarioLogado, [UsuarioLogado])
      ,[DataDeletado]    = COALESCE(@DataDeletado, [DataDeletado])
OUTPUT INSERTED.PessoaJuridicaID
WHERE PessoaJuridicaID = @PessoaJuridicaID
";

                await _dapper.ExecuteAsync(sql: sql, commandType: System.Data.CommandType.Text, param: model);
            }
            catch (SqlException ex)
            {
                TratarErrosSQL.TratarErroUnique(ex);
                throw;
            }
        }

        public async Task<int?> Inserir(PessoaJuridicaModel model)
        {
            try
            {
                string sql = @"
INSERT INTO [dbo].[PessoasJuridicas]
( 
       [NomeFantasia]
      ,[NomeEmpresarial]
      ,[Registro]
      ,[CNPJ]
      ,[Telefone]
      ,[Email]
      ,[AreaID]
      ,[Ativo]
      ,[Endereco]
      ,[Bairro]
      ,[Cidade]
      ,[UF]
      ,Estado
      ,CEP
      ,Regiao
      ,[UsuarioLogado]
      ,[DataAlteracao]
)
OUTPUT INSERTED.PessoaJuridicaID
VALUES (
       @NomeFantasia
      ,@RazaoSocial
      ,@Registro
      ,@CNPJ
      ,@Telefone
      ,@Email
      ,@AreaID
      ,@Ativo
      ,@Endereco
      ,@Bairro
      ,@Cidade
      ,@UF
      ,@Estado
      ,@CEP
      ,@Regiao
      ,@UsuarioLogado
      ,@DataAlteracao
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

        public async Task<List<PessoaJuridicaModel>> Listar(PessoaJuridicaRQModel model)
        {
            try
            {
                string sql = @"[dbo].[usp_listar_pessoas_juridicas]";

                var obj = new
                {
                    PessoaJuridicaID = model.PessoaJuridicaID,
                    PessoaID = model.PessoaID,
                    NomeFantasia = model.NomeFantasia,
                    AreaID = model.AreaID,
                    EspecializacaoID = model.EspecialidadeID,
                    Ativo = model.Ativo
                };

                var ret = await _dapper.QueryAsync<PessoaJuridicaModel>(sql: sql, commandType: CommandType.StoredProcedure, param: obj);
                return ret;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<Funcionamento>> ListarFuncionamento(int? empresaId, int? especialistaId)
        {
            try
            {
                string sql = @"
SELECT [ID]
      ,[PessoaJuridicaID]
      ,[EspecialistaID]
      ,[DiaID]
      ,[ManhaAbertura]
      ,[ManhaFechamento]
      ,[TardeAbertura]
      ,[TardeFechamento]
  FROM [atendimentos].[PessoasJuridicasFuncionamento]
  WHERE (
  (PessoaJuridicaID = @PessoaJuridicaID) OR
  (EspecialistaID = @EspecialistaID)
  )
";
                var obj = new
                {
                    PessoaJuridicaID = empresaId,
                    EspecialistaID = especialistaId
                };

                var ret = await _dapper.QueryAsync<Funcionamento>(sql: sql, commandType: CommandType.Text, param: obj);
                return ret;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task AtribuirEspecialidade(PessoaJuridicaModel model)
        {
            try
            {
                string sql = @"
INSERT INTO [atendimentos].[PessoasEspecialidades]
(
       [PessoaID]
      ,[EspecializacaoID]
)
VALUES (
       @PessoaJuridicaID
      ,@EspecialidadeID
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
    }
}
