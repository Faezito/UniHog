using Newtonsoft.Json;

namespace Z2.Servicos.Lib
{
    public interface IClientFactoryDelete
    {
        Task<S> Delete<S>(string endPonint);
    }

    public class ClientFactoryDelete : IClientFactoryDelete
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ClientFactoryDelete(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<S?> Delete<S>(string endPonint)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient("Api_Lugano");
                string url = $"{httpClient.BaseAddress}{endPonint}";

                httpClient.DefaultRequestHeaders.Accept.Clear();

                using (HttpResponseMessage response = await httpClient.DeleteAsync(url))
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
