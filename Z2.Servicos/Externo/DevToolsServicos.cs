using Z1.Model;
using Z1.Model.APIs;
using Z2.Services.Externo;
using Z2.Servicos.Lib;

namespace Z2.Servicos.Externo
{
    public interface IDevToolsServicos
    {
        Task<List<ChamadoModel>> ListarChamados();
        Task InserirChamado(DevToolsChamadoModel model);
    }

    public class DevToolsServicos : IDevToolsServicos
    {
        private readonly HttpClient _http;
        private readonly IAPIsServicos _api;
        private readonly IClientFactoryPost _post;
        private readonly IClientFactoryGet _get;
        public DevToolsServicos(HttpClient http, IAPIsServicos api, IClientFactoryPost post, IClientFactoryGet get)
        {
            _http = http;
            _api = api;
            _post = post;
            _get = get;
        }

        public async Task<List<ChamadoModel>> ListarChamados()
        {
            try
            {
                var lst = await _get.Get<List<ChamadoModel>>("/chamados/Listar/2", 151);

                return lst ?? new List<ChamadoModel>();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task InserirChamado(DevToolsChamadoModel model)
        {
            try
            {
                await _post.Post("/chamados/Cadastrar", model, 151);
            }
                catch (Exception)
            {
                throw;
            }
        }
    }
}
