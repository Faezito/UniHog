using Microsoft.Data.SqlClient;
using System.Text.RegularExpressions;

namespace Z3.DataAccess
{
    public static class TratarErrosSQL
    {
        public static void TratarErroUnique(SqlException ex)
        {
            if (ex.Number != 2627 && ex.Number != 2601)
                return;

            var match = Regex.Match(ex.Message, @"constraint '(.+?)'");
            var constraint = match.Success ? match.Groups[1].Value : null;

            var mensagens = new Dictionary<string, string>
{
    { "UK_ID",        "Este nome de ID já está sendo usado." },
    { "UK_COD",       "Este nome de ID já está sendo usado." },
    { "UK_Usuario",   "Este nome de usuário já está sendo usado." },
    { "UK_Email",     "Este e-mail já está cadastrado. Tente recuperar sua senha." },
    { "UK_CPF",       "Este CPF já está cadastrado." },
    { "UK_GoogleId",  "Esta conta do Google já está vinculada." },
    { "UK_Telefone",  "Este telefone já está cadastrado." },
    { "UK_Consulta",  "Este horário já está reservado." }
};

            if (constraint != null && mensagens.TryGetValue(constraint, out var msg))
                throw new Exception(msg);

            throw new Exception("Já existe um registro duplicado.");
        }
    }
}
