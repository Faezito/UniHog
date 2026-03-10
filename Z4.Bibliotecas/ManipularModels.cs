using System.Globalization;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Z1.Model;

namespace Z4.Bibliotecas
{
    public static class ManipularModels
    {
        public static void LimparModelState(ModelStateDictionary modelState, params string[] excecoes)
        {
            var chavesRemover = modelState.Keys
                .Where(k => !excecoes.Contains(k))
                .ToList();

            foreach (var key in chavesRemover)
            {
                modelState.Remove(key);
            }
        }

        public static (bool valido, string? mensagem) ValidarUsuario(UsuarioModel model)
        {
            bool valido = true;
            string mensagem = string.Empty;

            if (string.IsNullOrWhiteSpace(model.NomeCompleto)
                || string.IsNullOrWhiteSpace(model.Email)
                || string.IsNullOrWhiteSpace(model.Enderecos.FirstOrDefault()?.Rua)
                || string.IsNullOrWhiteSpace(model.Enderecos.FirstOrDefault()?.Numero)
                || string.IsNullOrWhiteSpace(model.Enderecos.FirstOrDefault()?.UF)
                || string.IsNullOrWhiteSpace(model.Enderecos.FirstOrDefault()?.Estado)
                || string.IsNullOrWhiteSpace(model.Enderecos.FirstOrDefault()?.Bairro)
                || string.IsNullOrWhiteSpace(model.Enderecos.FirstOrDefault()?.CEP)
                || string.IsNullOrWhiteSpace(model.Telefone)
                || string.IsNullOrWhiteSpace(model.CPF)
                || string.IsNullOrWhiteSpace(model.Enderecos.FirstOrDefault()?.CEP)
                || string.IsNullOrWhiteSpace(model.Genero)
                )
            {
                mensagem += "Preencha todos os campos. <br />";
                valido = false;
            }

            // VALIDAÇÃO DE SENHA

            //if (model.SenhaTemporaria == false)
            //{
            //    if (string.IsNullOrWhiteSpace(model.Senha)
            //        || string.IsNullOrWhiteSpace(model.ConfirmacaoSenha)
            //        )
            //    {
            //        mensagem += "Preencha todos os campos. <br />";
            //        valido = false;
            //    }

            //    if (model.Senha != model.ConfirmacaoSenha)
            //    {
            //        mensagem += "As senhas não coincidem. <br />";
            //        valido = false;
            //    }

            //    if (model.Senha?.Length < 8)
            //    {
            //        mensagem += "Senha insegura! A senha deve conter, no mínimo, 8 caracteres. <br />";
            //        valido = false;
            //    }
            //}

            return (valido, mensagem);
        }

        public static (bool senhaValida, string? mensagem) ValidarSenha(string senha)
        {
            bool senhaValida = true;
            string mensagem = string.Empty;

            if (senha.Length < 8)
            {
                senhaValida = false;
                mensagem = "A senha deve ter no mínimo 8 caracteres.";
            }

            if (!Regex.IsMatch(senha, "[A-Z]"))
            {
                senhaValida = false;
                mensagem = "A senha deve conter pelo menos uma letra maiúscula.";
            }

            if (!Regex.IsMatch(senha, "[a-z]"))
            {
                senhaValida = false;
                mensagem = "A senha deve conter pelo menos uma letra minúscula.";
            }

            if (!Regex.IsMatch(senha, "[0-9]"))
            {
                senhaValida = false;
                mensagem = "A senha deve conter pelo menos um número.";
            }
            return (senhaValida, mensagem);
        }

        public static DateTime ConverterData(string data)
        {
            var cultura = new CultureInfo("pt-BR");

            DateTime dataConvertida = DateTime.ParseExact(
                                                data,
                                                "dd/MM/yyyy",
                                                cultura
                                            );

            return dataConvertida;
        }

        public static string GerarUsuario(string email, string id)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(id))
            {
                throw new Exception("Usuário inválido.");
            }

            var usermail = email.Split("@")[0];
            id = id.Substring(3, 6);

            string usuario = usermail + '.' + id;
            return usuario;
        }

        public static string GerarUsuario2(string NomeCompleto, string? cod)
        {
            var nomes = NomeCompleto.Trim().ToLower().Split(" ");

            string userbase = $"{nomes[0]}.{nomes[^1]}";

            string usuario = userbase + '_' + cod;
            return usuario.Trim().ToLower();
        }

        public static string LimparNumeros(string dado)
        {
            if (!string.IsNullOrWhiteSpace(dado))
            {
                dado = dado.Trim();
                return Regex.Replace(dado, "[^0-9]", "");
            }
            return dado;
        }

        public static string CalcularIdadeString(this DateTime dataNascimento)
        {
            DateTime hoje = DateTime.Now;
            int idade = hoje.Year - dataNascimento.Year;
            string idadestring = string.Empty;

            if (dataNascimento.Date > hoje.AddYears(-idade))
                idade--;

            idadestring = idade > 1 ? $"{idade} anos" : $"{idade} ano";

            return idadestring;
        }

        public static string GeneroString(this string genero)
        {
            string generoString = string.Empty;
            switch (genero)
            {
                case "M":
                    generoString = "Masculino";
                    break;
                case "F":
                    generoString = "Feminino";
                    break;
            }

            return generoString;
        }

        public static string FormatarCPF(this string cpf)
        {
            if (!string.IsNullOrEmpty(cpf))
            {
                cpf = cpf.Trim();
                cpf = Convert.ToUInt64(cpf).ToString(@"000\.000\.000\-00");
            }
            return cpf;
        }

        public static string FormatarTelefone(this string? telefone)
        {
            if (string.IsNullOrWhiteSpace(telefone))
                return "Sem telefone cadastrado";

            telefone = telefone.Trim();
            telefone = Convert.ToUInt64(telefone).ToString(@"(000) 00000-0000");
            return telefone;
        }

        public static string FormatarCEP(this string cep)
        {
            if (!string.IsNullOrEmpty(cep))
            {
                cep = cep.Trim();
                cep = Convert.ToUInt64(cep).ToString(@"00000-000");
            }
            return cep;
        }

        public static List<DateTime> ObterDiasDaSemanaNoMes(int ano, int mes, DayOfWeek diaSemana)
        {
            var datas = new List<DateTime>();

            DateTime inicio = new DateTime(ano, mes, 1);
            int diasNoMes = DateTime.DaysInMonth(ano, mes);

            for (int dia = 0; dia < diasNoMes; dia++)
            {
                DateTime data = inicio.AddDays(dia);

                if (data.DayOfWeek == diaSemana)
                {
                    datas.Add(data);
                }
            }

            return datas;
        }

        public static string DiaDaSemanaExtenso(this int diaID)
        {
            switch (diaID)
            {
                case 1: return "Domingo";
                case 2: return "Segunda-feira";
                case 3: return "Terça-feira";
                case 4: return "Quarta-feira";
                case 5: return "Quinta-feira";
                case 6: return "Sexta-feira";
                case 7: return "Sábado";
                default: return string.Empty;
            }
        }

        public static string FormatarNomeCompleto(this string nomeCompleto)
        {
            string[] nomes = nomeCompleto.Trim().Split(" ");
            List<string> nomesFormatados = new List<string>();

            foreach (string s in nomes)
            {
                string nomeFormatado = char.ToUpper(s[0]) + s.Substring(1).ToLower();
                nomesFormatados.Add(nomeFormatado);
            }

            string ret = string.Join(" ", nomesFormatados);
            return ret.Trim();
        }

        public static string PrimeiroNome(this string nome)
        {
            if(string.IsNullOrWhiteSpace(nome)) return "--";

            var nomes = nome.Split(" ");
            return nomes[0].Trim();
        }

        public static string ObterMesCompleto(this int m)
        {
            return m switch
            {
                1 => "Janeiro",
                2 => "Fevereiro",
                3 => "Março",
                4 => "Abril",
                5 => "Maio",
                6 => "Junho",
                7 => "Julho",
                8 => "Agosto",
                9 => "Setembro",
                10 => "Outubro",
                11 => "Novembro",
                12 => "Dezembro",
                _ => throw new ArgumentOutOfRangeException(nameof(m), "Mês inválido.")
            };
        }
    }
}
