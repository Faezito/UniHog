using Microsoft.Data.SqlClient;
using System.Data;
using Z1.Model;
using Z3.DataAccess.Database;

namespace Z3.DataAccess
{
    public interface IUsuarioDataAccess
    {
        Task<List<UsuarioModel>> Listar(UsuarioRQModel model);
        Task<List<UsuarioModel>> ListarAniversariantes();
        Task<List<UsuarioModel>> ListarDrop(int? tipoID, string nome);
        Task<List<UsuarioModel>> Listar(int? projetoID, int? estudoId);
        Task<List<UsuarioModel>> ListarPaginado(UsuarioRQModel model, int pagina, int quantidade);
        Task<int?> Adicionar(UsuarioModel model);
        Task<int?> Atualizar(UsuarioModel model);
        Task AtualizarSenha(UsuarioModel model);
        Task Deletar(UsuarioModel model);
        Task<UsuarioModel> Obter(int? PessoaID, string? usuario);
        Task DeletarImagem(int pessoaId);
        Task DeletarTipo(TiposModel model);
        Task InserirTipo(TiposModel model);
        Task<int?> NovaSituacao(TiposModel model);
        Task<TiposModel> ObterTipo(int usuarioTipoID);
        Task<List<TiposModel>> ListarTipos(int usuarioTipoID);
        Task<List<TiposModel>> ListarSituacoes();
        Task AdicionarProfessor(UsuarioModel model);
        Task RemoverProfessor(UsuarioModel model);
        Task<int?> AtualizarSituacao(TiposModel model);
    }

    public class UsuarioDataAccess : IUsuarioDataAccess
    {
        private readonly IDapper _dapper;
        private readonly ILimparTabelas _limpar;

        public UsuarioDataAccess(IDapper dapper, ILimparTabelas limpar)
        {
            _dapper = dapper;
            _limpar = limpar;
        }

        public async Task<int?> Adicionar(UsuarioModel model)
        {
            try
            {
                string sql = @"
INSERT INTO [dbo].[Pessoas] (
NomeCompleto
,Usuario
,Email
,Senha
,CPF
,Telefone
,Genero
,TipoID
,DataNascimento
,DataCriacao
,FotoPerfilURL
,FotoPerfilPublicID
,UsuarioLogado
,SituacaoID
,ProjetoIndicacaoID
,EstudoID
,SenhaTemporaria
,EstudaEmCasa
)
OUTPUT INSERTED.PessoaID
VALUES (
@NomeCompleto
,@Usuario
,@Email
,@Senha
,@CPF
,@Telefone
,@Genero
,@TipoID
,@DataNascimento
,@DataCriacao
,@FotoPerfilURL
,@FotoPerfilPublicID
,@UsuarioLogado
,@SituacaoID
,@ProjetoIndicacaoID
,@EstudoID
,@SenhaTemporaria
,@EstudaEmCasa
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

        public async Task<int?> Atualizar(UsuarioModel model)
        {
            try
            {
                string sql = @"
UPDATE [dbo].[Pessoas]
SET 
NomeCompleto = COALESCE(@NomeCompleto, NomeCompleto),
Usuario = COALESCE(@Usuario, Usuario),
Email = COALESCE(@Email, Email),
DataNascimento = COALESCE(@DataNascimento, DataNascimento),
Senha = COALESCE(@Senha, Senha),
CPF = COALESCE(@CPF, CPF),
Telefone = COALESCE(@Telefone, Telefone),
Genero = COALESCE(@Genero, Genero),
TipoID = COALESCE(@TipoID, TipoID),
FotoPerfilURL = COALESCE(@FotoPerfilURL, FotoPerfilURL),
FotoPerfilPublicID = COALESCE(@FotoPerfilPublicID, FotoPerfilPublicID),
DataAlteracao = COALESCE(@DataAlteracao, DataAlteracao),
SituacaoID = COALESCE(@SituacaoID, SituacaoID),
ProjetoIndicacaoID = COALESCE(@ProjetoIndicacaoID, ProjetoIndicacaoID),
EstudoID = COALESCE(@EstudoID, EstudoID),
UsuarioLogado = COALESCE(@UsuarioLogado, UsuarioLogado),
UltimaSessao = COALESCE(@UltimaSessao, UltimaSessao),
SenhaTemporaria = COALESCE(@SenhaTemporaria, SenhaTemporaria),
EstudaEmCasa = COALESCE(@EstudaEmCasa, EstudaEmCasa)

OUTPUT INSERTED.PessoaID
WHERE PessoaID = @PessoaID
";
                return await _dapper.ExecuteAsync(sql: sql, commandType: System.Data.CommandType.Text, param: model);
            }
            catch (SqlException ex)
            {
                TratarErrosSQL.TratarErroUnique(ex);
                throw;
            }
        }

        public async Task Deletar(UsuarioModel model)
        {
            try
            {
                string sql = @"
UPDATE [dbo].[Pessoas]
SET DataDeletado = @DataDeletado, UsuarioLogado = @UsuarioLogado
WHERE PessoaID = @PessoaID
";
                await _dapper.ExecuteAsync(sql: sql, commandType: System.Data.CommandType.Text, param: model);
                await _limpar.LimparUsuarios();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<UsuarioModel>> Listar(UsuarioRQModel model)
        {
            try
            {
                string sql = @"[dbo].[listar_pessoas]";

                var obj = new
                {
                    PessoaID = model.PessoaID,
                    NomeCompleto = model.NomeCompleto,
                    CPF = model.CPF,
                    Bairro = model.Bairro,
                    UF = model.UF,
                    SituacaoID = model.SituacaoID,
                    TipoID = model.TipoID,
                    ProfessorID = model.ProfessorID,
                    EstudaEmCasa = model.EstudaEmCasa
                };

                var ret = await _dapper.QueryAsync<UsuarioModel>(sql: sql, param: obj, commandType: CommandType.StoredProcedure);
                return ret;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task AtualizarSenha(UsuarioModel model)
        {
            try
            {
                string sql = @"
UPDATE [dbo].[Pessoas]
SET 
Senha = @Senha
,UsuarioLogado = @UsuarioLogado
,DataAlteracao = @DataAlteracao
,SenhaTemporaria = 1
WHERE PessoaID = @PessoaID
";

                await _dapper.ExecuteAsync(sql: sql, commandType: System.Data.CommandType.Text, param: model);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // SÓ USADO NO LOGIN
        public async Task<UsuarioModel> Obter(int? PessoaID, string? usuario)
        {
            try
            {
                string sql = @"[dbo].[usp_obter_pessoa]";

                var obj = new
                {
                    PessoaID = PessoaID,
                    Login = usuario
                };

                var ret = await _dapper.QuerySingleOrDefaultAsync<UsuarioModel>(sql: sql, param: obj, commandType: CommandType.StoredProcedure);
                return ret;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task DeletarImagem(int pessoaId)
        {
            try
            {
                string sql = @"
UPDATE [dbo].[Pessoas]
SET FotoPerfilURL = NULL, FotoPerfilPublicID = NULL
WHERE PessoaID = @pessoaId
";
                await _dapper.ExecuteAsync(sql: sql, commandType: System.Data.CommandType.Text, param: new { pessoaId = pessoaId });
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public async Task DeletarTipo(TiposModel model)
        {
            try
            {
                string sql = @"DELETE FROM  [dbo].[UsuariosTipos] WHERE ID = @ID";

                await _dapper.ExecuteAsync(sql: sql, commandType: System.Data.CommandType.Text, param: model);
            }
            catch (SqlException ex)
            {
                TratarErrosSQL.TratarErroUnique(ex);
                throw;
            }
        }

        public async Task<List<TiposModel>> ListarTipos(int usuarioTipoID)
        {
            try
            {
                string sql = @"
SELECT [ID]
,[Tipo] AS Descricao
FROM [dbo].[UsuariosTipos]
WHERE ID >= @usuarioTipoID
";
                var obj = new
                {
                    usuarioTipoID = usuarioTipoID
                };

                var ret = await _dapper.QueryAsync<TiposModel>(sql: sql, commandType: System.Data.CommandType.Text, param: obj);
                return ret;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<TiposModel>> ListarSituacoes()
        {
            try
            {
                string sql = @"
SELECT [ID]
      ,[Descricao]
      ,Cor
  FROM [dbo].[PessoasSituacoes]
";

                var ret = await _dapper.QueryAsync<TiposModel>(sql: sql, commandType: System.Data.CommandType.Text);
                return ret;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<TiposModel> ObterTipo(int usuarioTipoID)
        {
            try
            {
                string sql = @"
SELECT [ID]
,[Tipo] AS Descricao
FROM [dbo].[UsuariosTipos]
WHERE ID = @usuarioTipoID
";
                var obj = new
                {
                    usuarioTipoID = usuarioTipoID
                };

                var ret = await _dapper.QuerySingleOrDefaultAsync<TiposModel>(sql: sql, commandType: System.Data.CommandType.Text, param: obj);
                return ret;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task InserirTipo(TiposModel model)
        {
            try
            {
                string sql = @"
INSERT INTO [dbo].[UsuariosTipos] (
ID, Tipo
)
OUTPUT INSERTED.ID
VALUES (
@ID, @Descricao
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

        public async Task<int?> NovaSituacao(TiposModel model)
        {
            try
            {
                string sql = @"
INSERT INTO [dbo].[PessoasSituacoes] (
Descricao
,Cor
)
OUTPUT INSERTED.ID
VALUES (
@Descricao
,@Cor
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

        public async Task<int?> AtualizarSituacao(TiposModel model)
        {
            try
            {
                string sql = @"
UPDATE [dbo].[PessoasSituacoes] SET
Descricao = @Descricao
,Cor = @Cor
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

        public async Task AdicionarProfessor(UsuarioModel model)
        {
            try
            {
                string sql = @"
INSERT INTO [dbo].[PessoasProfessores] (
PessoaID, ProfessorID
)
OUTPUT INSERTED.ID
VALUES (
@PessoaID, @ProfessorID
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

        public async Task RemoverProfessor(UsuarioModel model)
        {
            try
            {
                string sql = "DELETE FROM [dbo].[PessoasProfessores] WHERE PessoaID = @pessoaID";
                await _dapper.ExecuteAsync(sql: sql, commandType: System.Data.CommandType.Text, param: new { pessoaID = model.PessoaID });
            }
            catch (SqlException ex)
            {
                TratarErrosSQL.TratarErroUnique(ex);
                throw;
            }
        }

        public async Task<List<UsuarioModel>> Listar(int? projetoID, int? estudoID)
        {
            try
            {
                string sql = @"
SELECT * FROM [dbo].[Pessoas]
        WHERE (ProjetoIndicacaoID = @projetoID OR EstudoID = @estudoID)
";
                var obj = new
                {
                    projetoID = projetoID,
                    estudoID = estudoID
                };

                return await _dapper.QueryAsync<UsuarioModel>(sql: sql, commandType: System.Data.CommandType.Text, param: obj);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<UsuarioModel>> ListarPaginado(UsuarioRQModel model, int pagina, int quantidade)
        {
            try
            {
                string sql = @"[dbo].[listar_pessoas_paginado]";

                var obj = new
                {
                    PessoaID = model.PessoaID,
                    NomeCompleto = model.NomeCompleto,
                    CPF = model.CPF,
                    Bairro = model.Bairro,
                    UF = model.UF,
                    SituacaoID = model.SituacaoID,
                    TipoID = model.TipoID,
                    TipoIDs = model.TipoIDs,
                    ProfessorID = model.ProfessorID,
                    Pagina = pagina,
                    Quantidade = quantidade,
                    UsuarioTipoID = model.UsuarioTipoID,
                    EstudaEmCasa = model.EstudaEmCasa
                };

                var ret = await _dapper.QueryAsync<UsuarioModel>(sql: sql, param: obj, commandType: CommandType.StoredProcedure);
                return ret;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<UsuarioModel>> ListarDrop(int? tipoID, string nome)
        {
            try
            {
                string sql = @"
SELECT 
PessoaID,
NomeCompleto
FROM dbo.Pessoas
WHERE (@tipoID IS NULL OR TipoID = @tipoID)
AND NomeCompleto LIKE @nome + '%'
";

                var obj = new
                {
                    tipoID = tipoID,
                    nome = nome
                };

                var ret = await _dapper.QueryAsync<UsuarioModel>(sql: sql, param: obj, commandType: CommandType.Text);
                return ret;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<UsuarioModel>> ListarAniversariantes()
        {
            try
            {
                string sql = @"
SELECT P.[PessoaID]
      ,P.[NomeCompleto]
      ,P.[Email]
      ,P.[Telefone]
      ,T.Tipo
      ,CASE WHEN P.Genero = 'M' THEN 'Masculino' ELSE 'Feminino' END AS GeneroTxt
      ,P.[DataNascimento]
      ,P.[FotoPerfilURL]
      ,P.[FotoPerfilPublicID]
      ,P.[ProjetoIndicacaoID]
  FROM Pessoas P
  LEFT JOIN UsuariosTipos T ON T.ID = P.TipoID
  WHERE DataDeletado IS NULL
    AND MONTH(GETDATE()) = MONTH(P.DataNascimento) 
  ORDER BY DAY(DataNascimento) ASC
";

                return await _dapper.QueryAsync<UsuarioModel>(sql: sql, commandType: System.Data.CommandType.Text);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Task<string> GerarSenha(int pessoaId)
        {
            throw new NotImplementedException();
        }
    }
}


