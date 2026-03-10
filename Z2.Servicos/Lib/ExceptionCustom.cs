using System.Net;

namespace Z2.Servicos.Lib
{
    public static class ExceptionCustom
    {
        public static async Task<Exception> Exception(HttpResponseMessage response)
        {
            string json = await response.Content.ReadAsStringAsync();

            // StatusCode = 500
            if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            {
                throw new Exception(json);
            }
            // StatusCode = 400
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                throw new Exception(json);
            }
            // StatusCode = 400
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                throw new Exception(json);
            }
            // StatusCode = 401
            else if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new Exception(json);
            }

            throw new Exception(json);
        }
    }
}
