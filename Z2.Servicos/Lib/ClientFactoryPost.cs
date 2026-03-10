using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using Z2.Services.Externo;

namespace Z2.Servicos.Lib
{
    public interface IClientFactoryPost
    {
        Task<S> Post<S, E>(string endPoint, E model, int apiId);
        Task Post<E>(string endPoint, E model, int apiId);
    }

    public class ClientFactoryPost : IClientFactoryPost
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IAPIsServicos _api;

        public ClientFactoryPost(IHttpClientFactory httpClientFactory, IAPIsServicos api)
        {
            _httpClientFactory = httpClientFactory;
            _api = api;
        }

        public async Task<S?> Post<S, E>(string endPoint, E model, int apiId)
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
                string strJson = JsonConvert.SerializeObject(model);
                StringContent httpContent = new(strJson, Encoding.UTF8, "application/json");

                using (HttpResponseMessage response = await httpClient.PostAsync(url, httpContent))
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
                    catch (Exception ex)
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

        public async Task Post<E>(string endPoint, E model, int apiId)
        {
            try
            {
                var api = await _api.Obter(null, apiId); // devtools 151
                var httpClient = _httpClientFactory.CreateClient();

                var auth = Convert.ToBase64String(
                    Encoding.ASCII.GetBytes($"{api.Usuario}:{api.Senha}"));

                httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Basic", auth);

                //string Url = "http://localhost:8080";

                string url = $"{api.Url}{endPoint}";

                var settings = new JsonSerializerSettings
                {
                    DateFormatString = "yyyy-MM-ddTHH:mm:ss"
                };

                httpClient.DefaultRequestHeaders.Accept.Clear();
                StringContent httpContent = new(JsonConvert.SerializeObject(model), Encoding.UTF8, System.Net.Mime.MediaTypeNames.Application.Json);

                var json = JsonConvert.SerializeObject(model);
                Console.WriteLine(json);

                using (HttpResponseMessage response = await httpClient.PostAsync(url, httpContent))
                {
                    try
                    {
                        response.EnsureSuccessStatusCode();
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
