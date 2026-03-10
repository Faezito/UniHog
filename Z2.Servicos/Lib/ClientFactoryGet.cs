using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using Z2.Services.Externo;

namespace Z2.Servicos.Lib
{
    public interface IClientFactoryGet
    {
        Task<S> Get<S>(string endPoint, int apiId);
    }

    public class ClientFactoryGet : IClientFactoryGet
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IAPIsServicos _api;

        public ClientFactoryGet(IHttpClientFactory httpClientFactory, IAPIsServicos api)
        {
            _httpClientFactory = httpClientFactory;
            _api = api;
        }

        public async Task<S?> Get<S>(string endPoint, int apiId)
        {
            try
            {
                var api = await _api.Obter(null, apiId); // devtools 151
                var httpClient = _httpClientFactory.CreateClient(api.Url);

                var auth = Convert.ToBase64String(
                    Encoding.ASCII.GetBytes($"{api.Usuario}:{api.Senha}"));

                httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Basic", auth);

                string url = $"{api.Url}{endPoint}";

                httpClient.DefaultRequestHeaders.Accept.Clear();

                using (HttpResponseMessage response = await httpClient.GetAsync(url))
                {
                    try
                    {
                        response.EnsureSuccessStatusCode();
                        string json = await response.Content.ReadAsStringAsync();
                        if (string.IsNullOrWhiteSpace(json))
                        {
                            return default;
                        }
                        return JsonConvert.DeserializeObject<S>(json);
                    }
                    catch (Exception)
                    {
                        throw await ExceptionCustom.Exception(response);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
