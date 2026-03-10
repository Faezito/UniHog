using System.Text.Json;

public interface IViaCepServicos
{
    Task<string> EncontrarCEP(string uf, string cidade, string logradouro);
}

public class ViaCepServicos : IViaCepServicos
{
    private readonly HttpClient client = new HttpClient();

    public async Task<string> EncontrarCEP(string uf, string cidade, string logradouro)
    {
        if (string.IsNullOrWhiteSpace(logradouro) || logradouro.Length < 3)
            return "Logradouro muito curto.";

        string url = $"https://viacep.com.br/ws/{uf}/{cidade}/{logradouro}/json/";

        try
        {
            HttpResponseMessage response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                string jsonResponse = await response.Content.ReadAsStringAsync();

                // A busca por logradouro SEMPRE retorna um Array []
                var enderecos = JsonSerializer.Deserialize<List<Endereco>>(jsonResponse);

                if (enderecos != null && enderecos.Count > 0)
                {
                    return enderecos[0].cep;
                }

                return "00000000";
            }

            return $"Erro na requisição: {response.StatusCode}";
        }
        catch (Exception ex)
        {
            return $"Erro: {ex.Message}";
        }
    }
}

// Classe para mapear o retorno do ViaCEP
public class Endereco
{
    public string cep { get; set; }
    public string logradouro { get; set; }
    public string bairro { get; set; }
    public string localidade { get; set; }
    public string uf { get; set; }
}