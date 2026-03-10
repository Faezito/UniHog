using Microsoft.Data.SqlClient;
using Z1.Model;
using Z3.DataAccess.Database;

namespace Z3.DataAccess
{
    public interface ICestaBasicaDataAccess
    {
        public Task<int?> Inserir(CestaBasicaModel model);
        public Task<int?> Atualizar(CestaBasicaModel model);
        public Task<int?> AdicionarEstoque(CestaBasicaModel model);
        public Task<int?> RemoverEstoque(CestaBasicaModel model);
        public Task<List<CestaBasicaModel>> Listar();
        public Task<CestaBasicaModel> Obter(int id);
        public Task DeletarCesta(int id);


        public Task<List<CestaBasicaBeneficiarioModel>> ListarBeneficiarios(DateTime? date);
        public Task<List<CestaBasicaBeneficiarioModel>> ListarBeneficiariosPJ();
        public Task<int?> RegistrarSaida(CestaEntregaModel model);
    }
    public class CestaBasicaDataAccess : ICestaBasicaDataAccess
    {
        private readonly IDapper _dapper;

        public CestaBasicaDataAccess(IDapper dapper)
        {
            _dapper = dapper;
        }


        public async Task<int?> Atualizar(CestaBasicaModel model)
        {
            try
            {
                string sql = @"
UPDATE [dbo].[CestasBasicas] SET
Descricao = COALESCE(@Descricao, Descricao),
Itens = COALESCE(@Itens, Itens),
Custo = COALESCE(@Custo, Custo),
Quantidade = COALESCE(@Quantidade, Quantidade),
EstoqueMin = COALESCE(@EstoqueMin, EstoqueMin),
DataAlteracao = COALESCE(@DataAlteracao, DataAlteracao),
UsuarioLogado = COALESCE(@UsuarioLogado, UsuarioLogado)
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

        public async Task<int?> Inserir(CestaBasicaModel model)
        {
            try
            {
                string sql = @"
INSERT INTO dbo.CestasBasicas
(
Descricao,
Itens,
Custo,
Quantidade,
EstoqueMin,
DataAlteracao,
UsuarioLogado
)
OUTPUT INSERTED.ID
VALUES(
@Descricao,
@Itens,
@Custo,
@Quantidade,
@EstoqueMin,
@DataAlteracao,
@UsuarioLogado
);
";

                return await _dapper.ExecuteAsync(sql: sql, commandType: System.Data.CommandType.Text, param: model);
            }
            catch (SqlException ex)
            {
                TratarErrosSQL.TratarErroUnique(ex);
                throw;
            }
        }

        public async Task<List<CestaBasicaModel>> Listar()
        {
            try
            {
                string sql = @"SELECT * FROM dbo.CestasBasicas";

                return await _dapper.QueryAsync<CestaBasicaModel>(sql: sql, commandType: System.Data.CommandType.Text);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<CestaBasicaModel> Obter(int id)
        {
            try
            {
                string sql = @"
SELECT * FROM dbo.CestasBasicas
WHERE ID = @id
";

                return await _dapper.QuerySingleOrDefaultAsync<CestaBasicaModel>(param: new { id = id }, sql: sql, commandType: System.Data.CommandType.Text);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<int?> RegistrarSaida(CestaEntregaModel model)
        {
            try
            {
                string sql = @"
INSERT INTO dbo.CestasEntregas
(
PessoaID,
EntregadorID,
CestaID,
DataEntrega,
DataAlteracao,
UsuarioLogado
)
OUTPUT INSERTED.ID
VALUES(
@PessoaID,
@EntregadorID,
@CestaID,
@DataEntrega,
@DataAlteracao,
@UsuarioLogado
);
";

                return await _dapper.ExecuteAsync(sql: sql, commandType: System.Data.CommandType.Text, param: model);
            }
            catch (SqlException ex)
            {
                TratarErrosSQL.TratarErroUnique(ex);
                throw;
            }
        }

        public async Task<int?> AdicionarEstoque(CestaBasicaModel model)
        {
            try
            {
                string sql = @"
UPDATE dbo.CestasBasicas 
SET 
Quantidade += @Quantidade, 
UsuarioLogado = @UsuarioLogado,
DataAlteracao = @DataAlteracao 
OUTPUT INSERTED.ID
WHERE ID = @ID
";

                return await _dapper.ExecuteAsync(sql: sql, commandType: System.Data.CommandType.Text, param: model);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<int?> RemoverEstoque(CestaBasicaModel model)
        {
            try
            {
                string sql = @"
UPDATE dbo.CestasBasicas 
SET 
Quantidade -= 1, 
UsuarioLogado = @UsuarioLogado,
DataAlteracao = @DataAlteracao 
OUTPUT INSERTED.ID
WHERE ID = @ID
";

                return await _dapper.ExecuteAsync(sql: sql, commandType: System.Data.CommandType.Text, param: model);
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public async Task<List<CestaBasicaBeneficiarioModel>> ListarBeneficiarios(DateTime? data)
        {
            try
            {
                string sql = "[dbo].[usp_recebe_cesta_basica]";

                var param = new
                {
                    data = data
                };

                return await _dapper.QueryAsync<CestaBasicaBeneficiarioModel>(sql: sql, param: param, commandType: System.Data.CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<CestaBasicaBeneficiarioModel>> ListarBeneficiariosPJ()
        {
            try
            {
                string sql = @"
SELECT 
P.PessoaID,
P.NomeCompleto,
P.Telefone,
P.FotoPerfilURL,
CONCAT(E.Rua, ', ', E.Numero, ', ', E.Bairro) AS EnderecoReduzido,
CONCAT(E.Rua, ', ', E.Numero, ', ', E.Bairro, ', ', E.Cidade) AS EnderecoCompleto,
CE.DataEntrega,
CE.EntregadorID,
PS.NomeCompleto AS Entregador,
CASE WHEN CE.DataEntrega IS NULL THEN 0 ELSE 1 END AS Entregue
FROM [dbo].[Pessoas] P
LEFT JOIN PessoasEnderecos PE ON PE.PessoaID = P.PessoaID AND PE.TipoID = 1
LEFT JOIN Enderecos E ON E.ID = PE.EnderecoID
LEFT JOIN CestasEntregas CE ON CE.PessoaID = P.PessoaID
LEFT JOIN Pessoas PS ON PS.PessoaID = CE.EntregadorID
WHERE P.DataDeletado IS NULL
AND P.TipoID = 15
AND CE.DataEntrega IS NULL OR MONTH(CE.DataEntrega) = MONTH(GETDATE())
";

                return await _dapper.QueryAsync<CestaBasicaBeneficiarioModel>(sql: sql, commandType: System.Data.CommandType.Text);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task DeletarCesta(int id)
        {
            try
            {
                string sql = @"DELETE FROM dbo.CestasBasicas WHERE ID = @ID";

                await _dapper.ExecuteAsync(sql: sql, commandType: System.Data.CommandType.Text, param: new { ID = id });
            }
            catch (SqlException ex)
            {
                throw;
            }
        }
    }
}
