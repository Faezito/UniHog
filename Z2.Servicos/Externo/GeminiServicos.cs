using GenerativeAI;
using GenerativeAI.Types;
using Z1.Model.APIs;

namespace Z2.Services.Externo
{
    public interface IGeminiServicos
    {
        Task<string> PromptFile(string prompt, string filebase64);
        Task<string> Prompt(string prompt, int idGemini = 1);
    }

    public class GeminiServicos : IGeminiServicos
    {
        private readonly IAPIsServicos _apis;
        public GeminiServicos(IAPIsServicos apis)
        {
            //_model = new GenerativeModel(key, versao);
            _apis = apis;
        }

        public async Task<string> Prompt(string prompt, int idGemini = 1)
        {
            APIModel conf = await _apis.Obter(null, idGemini); // 1 = ID DO GEMINI NO BANCO --- TALVEZ PASSAR PARA O PROGRAMA
            string res = string.Empty;

            var model = new GenerativeModel(conf.Token, conf.Modelo);

            try
            {
                var content = new Content { Parts = new List<Part> { new Part { Text = prompt } } };
                var request = new GenerateContentRequest(content);
                var result = await model.GenerateContentAsync(request);
                var resposta = result.Text?.Trim();

                if (string.IsNullOrWhiteSpace(resposta))
                {
                    throw new Exception("Erro inesperado ao carregar jogo. Tente novamente mais tarde");
                }

                return resposta;
            }
            catch (HttpRequestException httpEx)
            {
                // Captura o código de status HTTP (ex: 429, 500, 503)
                var statusCode = (int?)httpEx.StatusCode;

                return statusCode switch
                {
                    429 => "Erro:429",
                    503 => "Erro:503",
                    500 => "Erro:500",
                    401 => "Erro:401",
                    _ => $"Erro HTTP: {statusCode} - {httpEx.Message}"
                };
            }
            catch (Exception ex)
            {
                // Outros erros (rede, JSON malformado, etc.)
                return $"Erro inesperado: {ex.Message}";
            }
        }

        public async Task<string> PromptFile(string prompt, string filebase64)
        {
            APIModel conf = await _apis.Obter(null, 1); // 1 = ID DO GEMINI NO BANCO --- TALVEZ PASSAR PARA O PROGRAMA

            var model = new GenerativeModel(conf.Token, conf.Modelo);

            try
            {
                var content = new Content
                {
                    Parts = new List<Part>
                    {
                        new Part
                        {
                            Text = prompt
                        },
                        new Part
                        {
                            InlineData = new Blob
                            {
                                MimeType = "image/jpeg",
                                Data = filebase64
                            }
                        }
                    }
                };
                var request = new GenerateContentRequest(content);
                var result = await model.GenerateContentAsync(request);
                var resposta = result.Text?.Trim();

                return resposta ?? "Sem resposta";
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
