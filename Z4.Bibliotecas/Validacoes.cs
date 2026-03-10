using System.Text.RegularExpressions;
using Z1.Model;

namespace Z4.Bibliotecas
{
    public static class Validacoes
    {
        public static (bool valido, string? mensagem) ValidarUsuario(UsuarioCadastroModel model)
        {
            bool valido = true;
            string mensagem = string.Empty;

            if (string.IsNullOrWhiteSpace(model.NomeCompleto)
                || string.IsNullOrWhiteSpace(model.Email)
                || string.IsNullOrWhiteSpace(model.Telefone)
                || string.IsNullOrWhiteSpace(model.CPF)
                || string.IsNullOrWhiteSpace(model.Genero)
                )
            {
                mensagem += "Preencha todos os campos. <br />";
                valido = false;
            }
            return (valido, mensagem);
        }
        public static (bool valido, string? mensagem) ValidarUsuario(UsuarioModel model)
        {
            bool valido = true;
            string mensagem = string.Empty;

            if (string.IsNullOrWhiteSpace(model.NomeCompleto)
                || string.IsNullOrWhiteSpace(model.Email)
                || string.IsNullOrWhiteSpace(model.Telefone)
                || string.IsNullOrWhiteSpace(model.Genero)
                )
            {
                mensagem += "Preencha todos os campos. <br />";
                valido = false;
            }
            return (valido, mensagem);
        }
        public static (bool valido, string? mensagem) ValidarInteressado(InteressadoCadastroModel model)
        {
            bool valido = true;
            string mensagem = string.Empty;

            if (string.IsNullOrWhiteSpace(model.NomeCompleto)
                || string.IsNullOrWhiteSpace(model.Telefone)
                || string.IsNullOrWhiteSpace(model.Genero)
                )
            {
                mensagem += "Preencha todos os campos. <br />";
                valido = false;
            }
            return (valido, mensagem);
        }
        public static (bool valido, string? mensagem) ValidarEmpresa(PessoaJuridicaModel model)
        {
            bool valido = true;
            string mensagem = string.Empty;

            if (string.IsNullOrWhiteSpace(model.Endereco) 
                || string.IsNullOrWhiteSpace(model.Bairro)
                || string.IsNullOrWhiteSpace(model.Cidade)
                || string.IsNullOrWhiteSpace(model.UF)
                ) 
            {
                mensagem += "Preencha o endereço. <br/>";
            }

            if (string.IsNullOrWhiteSpace(model.NomeFantasia)
                || string.IsNullOrWhiteSpace(model.CNPJ)
                || !model.AreaID.HasValue
                )
            {
                mensagem += "Preencha todos os campos. <br />";
                valido = false;
            }
            return (valido, mensagem);
        }

        public static (bool valido, string? mensagem) ValidarEndereco(UsuarioModel model)
        {
            bool valido = true;
            string mensagem = string.Empty;

            if (   string.IsNullOrWhiteSpace(model.Enderecos.FirstOrDefault()?.Rua)
                || string.IsNullOrWhiteSpace(model.Enderecos.FirstOrDefault()?.Numero)
                || string.IsNullOrWhiteSpace(model.Enderecos.FirstOrDefault()?.UF)
                || string.IsNullOrWhiteSpace(model.Enderecos.FirstOrDefault()?.Estado)
                || string.IsNullOrWhiteSpace(model.Enderecos.FirstOrDefault()?.Bairro)
              //  || string.IsNullOrWhiteSpace(model.Enderecos.FirstOrDefault()?.CEP)
                )
            {
                mensagem += "Preencha todos os campos. <br />";
                valido = false;
            }
            return (valido, mensagem);
        }
        public static (bool valido, string? mensagem) ValidarEndereco(EnderecoModel model)
        {
            bool valido = true;
            string mensagem = string.Empty;

            if (   string.IsNullOrWhiteSpace(model.Rua)
                || string.IsNullOrWhiteSpace(model.Numero)
                || string.IsNullOrWhiteSpace(model.UF)
                || string.IsNullOrWhiteSpace(model.Estado)
                || string.IsNullOrWhiteSpace(model.Bairro)
              //  || string.IsNullOrWhiteSpace(model.Enderecos.FirstOrDefault()?.CEP)
                )
            {
                mensagem += "Preencha todos os campos. <br />";
                valido = false;
            }
            return (valido, mensagem);
        }

        public static (bool valido, string? mensagem) ValidarSenha(UsuarioModel model)
        {
            bool valido = true;
            string mensagem = string.Empty;

            if (model.SenhaTemporaria == false)
            {
                if (string.IsNullOrWhiteSpace(model.Senha)
                    || string.IsNullOrWhiteSpace(model.ConfirmacaoSenha)
                    )
                {
                    mensagem += "Preencha todos os campos. <br />";
                    valido = false;
                }

                if (model.Senha != model.ConfirmacaoSenha)
                {
                    mensagem += "As senhas não coincidem. <br />";
                    valido = false;
                }

                if (model.Senha?.Length < 8)
                {
                    mensagem += "Senha insegura! A senha deve conter, no mínimo, 8 caracteres. <br />";
                    valido = false;
                }
            }

            return (valido, mensagem);
        }

        public static (bool valido, string? mensagem) ValidarCPF(string cpf)
        {
            string mensagem = string.Empty;

            if (string.IsNullOrWhiteSpace(cpf))
            {
                mensagem = "CPF inválido!"; 
                return (false, mensagem);
            }

            // Remove tudo que não for número
            cpf = Regex.Replace(cpf, @"\D", "").Trim();

            // CPF deve ter 11 dígitos
            if (cpf.Length != 11)
                return (false, "CPF inválido!");

            // Rejeita CPFs com todos os dígitos iguais
            if (cpf.Distinct().Count() == 1)
                return (false, "CPF inválido!");

            // Calcula o primeiro dígito verificador
            int soma = 0;
            for (int i = 0; i < 9; i++)
                soma += (cpf[i] - '0') * (10 - i);

            int resto = (soma * 10) % 11;
            if (resto == 10) resto = 0;

            if (resto != (cpf[9] - '0'))
                return (false, "CPF inválido!");

            // Calcula o segundo dígito verificador
            soma = 0;
            for (int i = 0; i < 10; i++)
                soma += (cpf[i] - '0') * (11 - i);

            resto = (soma * 10) % 11;
            if (resto == 10) resto = 0;

            if (resto != (cpf[10] - '0'))
                return (false, "CPF inválido!");

            return (true, cpf);
       }

        public static (bool valido, string? mensagem) ValidarCNPJ(string cnpj)
        {
            string mensagem = string.Empty;

            if (string.IsNullOrWhiteSpace(cnpj))
            {
                mensagem = "CNPJ inválido!";
                return (false, mensagem);
            }

            // Remove tudo que não for número
            cnpj = Regex.Replace(cnpj, @"\D", "").Trim();

            // CNPJ deve ter 14 dígitos
            if (cnpj.Length != 14)
                return (false, null);

            // Rejeita CNPJs com todos os dígitos iguais
            if (cnpj.Distinct().Count() == 1)
                return (false, null);

            int[] pesosPrimeiroDigito = { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] pesosSegundoDigito = { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

            // Primeiro dígito verificador
            int soma = 0;
            for (int i = 0; i < 12; i++)
                soma += (cnpj[i] - '0') * pesosPrimeiroDigito[i];

            int resto = soma % 11;
            int digito1 = resto < 2 ? 0 : 11 - resto;

            if (digito1 != (cnpj[12] - '0'))
                return (false, null);

            // Segundo dígito verificador
            soma = 0;
            for (int i = 0; i < 13; i++)
                soma += (cnpj[i] - '0') * pesosSegundoDigito[i];

            resto = soma % 11;
            int digito2 = resto < 2 ? 0 : 11 - resto;

            if (digito2 != (cnpj[13] - '0'))
                return (false, null);

            return (true, cnpj);
        }
    }
}
