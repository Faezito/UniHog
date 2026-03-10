using Z1.Model;
using Z3.DataAccess;

public interface IChamadoServicos
{
    Task<int?> Inserir(ChamadoModel chamado);
    Task<int?> Atualizar(ChamadoModel chamado);
}
public class ChamadoServicos : IChamadoServicos
{
    private readonly IChamadoDataAccess _chamados;
    public ChamadoServicos(IChamadoDataAccess chamados)
    {
        _chamados = chamados;
    }

    public async Task<int?> Atualizar(ChamadoModel chamado)
    {
        return await _chamados.Atualizar(chamado);
    }

    public async Task<int?> Inserir(ChamadoModel chamado)
    {
        return await _chamados.Inserir(chamado);
    }
}