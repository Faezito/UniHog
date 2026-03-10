using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Z1.Model;
using Z1.Model.Email;
using Z3.DataAccess;

namespace Z2.Services.Externo
{
    public interface IEmailServicos
    {
        Task EnviarEmailAsync(string destinatario, string assunto, string corpoHtml);
        Task EnviarSenhaPorEmail(bool cadastro, UsuarioModel model);
        Task EnviarInteressadoPorEmail(InteressadoCadastroModel model);
        Task EnviarCestaPorEmail(CestaEntregaModel model);
        Task EnviarCestaPorEmail(CestaBasicaModel model, string? alteracoes);
        Task EnviarMovimentacaoPorEmail(MovimentacaoFinanceiraModel model);
        Task EnviarChamadaPorEmail(ChamadaEmailModel model);
        Task<EmailConfig> Obter(int id);
        Task<int?> Cadastro(EmailConfig model);
        Task Deletar(int id);
        Task<List<EmailConfig>> Listar(int? id);
    }

    public class EmailServicos : IEmailServicos
    {
        private string _email = "bope.jesus2@gmail.com";

        private readonly IEmailDataAccess _daEmail;
        public EmailServicos(IEmailDataAccess daEmail)
        {
            _daEmail = daEmail;
        }
        public async Task EnviarEmailAsync(string destinatario, string assunto, string corpoHtml)
        {
            var mensagem = new MimeMessage();
            EmailConfig model = await _daEmail.Obter(1);

            mensagem.From.Add(new MailboxAddress(
                model.FromName,
                model.Remetente
            ));

            mensagem.To.Add(MailboxAddress.Parse(destinatario));
            mensagem.Subject = assunto;

            mensagem.Body = new TextPart("html")
            {
                Text = corpoHtml
            };

            using var client = new SmtpClient();

            try
            {
                if (model.UseStartTls)
                {
                    await client.ConnectAsync(model.Server, model.Port, SecureSocketOptions.StartTls);
                }
                else if (model.UseSSL)
                {
                    await client.ConnectAsync(model.Server, model.Port, SecureSocketOptions.SslOnConnect);
                }
                else
                {
                    await client.ConnectAsync(model.Server, model.Port, SecureSocketOptions.None);
                }

                await client.AuthenticateAsync(model.Username, model.Password);

                await client.SendAsync(mensagem);
            }
            catch
            {
                throw new Exception("Erro ao conectar ao serviço de email. Contate um administrador.");
            }
            finally
            {
                await client.DisconnectAsync(true);
            }
        }


        public async Task<int?> Cadastro(EmailConfig model)
        {
            if (model.ID.HasValue)
            {
                return await _daEmail.Atualizar(model);
            }
            return await _daEmail.Inserir(model);
        }

        public async Task Deletar(int id)
        {
            await _daEmail.Deletar(id);
        }

        public async Task<List<EmailConfig>> Listar(int? id)
        {
            return await _daEmail.Listar(id);
        }

        public async Task<EmailConfig> Obter(int id)
        {
            return await _daEmail.Obter(id);
        }


        public async Task EnviarSenhaPorEmail(bool cadastro, UsuarioModel model)
        {
            try
            {
                string primeiroNome = model.NomeCompleto.Split(" ")[0];
                string destinatario = model.Email.Trim();
                string assunto = string.Empty;
                string corpo = string.Empty;
                string link = "";

                if (cadastro == true)
                {
                    assunto = "Bem-vindo ao BOPE irmão, sua conta está pronta!";
                    corpo = $@"
<!DOCTYPE html>
<html>
<body style='
    margin:0;
    padding:0;
    background:#f4f7f9;
    font-family: Arial, Helvetica, sans-serif;
    color:#334155;
'>

<table width='100%' cellpadding='0' cellspacing='0'>
<tr>
<td align='center'>

    <table width='100%' cellpadding='0' cellspacing='0' style='max-width:600px;padding:24px;'>

        <tr>
            <td align='center' style='padding-bottom:24px;'>
                <h1 style='margin:0;color:#003366;'>⛪ BOPE</h1>
                <p style='margin:4px 0 0;color:#64748b;font-size:14px;text-transform:uppercase;letter-spacing:1px;'>
                    Bem Organizados Para Evangelizar - IASD
                </p>
            </td>
        </tr>

        <tr>
            <td style='
                background:#ffffff;
                border-radius:8px;
                padding:40px 24px;
                box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1);
            '>

                <p style='font-size:18px; color:#0f172a;'>
                    Olá, <strong>{primeiroNome}</strong>. Seja bem-vindo(a)! 👋
                </p>

                <p style='font-size:15px;line-height:1.6;color:#475569;'>
                    Seu cadastro no <strong>Portal do BOPE</strong> foi realizado com sucesso. 
                    Estamos felizes em ter você conosco em nossa plataforma digital para facilitar nossa comunhão e organização.
                </p>

                <p style='font-size:15px;line-height:1.6;color:#475569;'>
                    Para o seu primeiro acesso, utilize a <strong>senha provisória</strong> abaixo. 
                    Por medida de segurança, pedimos que você a altere logo após realizar o primeiro login.
                </p>

                <div style='
                    background:#f8fafc;
                    border:1px dashed #003366;
                    border-radius:6px;
                    padding:20px;
                    margin:24px 0;
                    text-align:center;
                '>
                    <p style='margin:0;font-size:13px;color:#64748b;text-transform:uppercase;'>Usuário</p>
                    <p style='
                        margin:8px 0 0;
                        font-size:24px;
                        font-weight:bold;
                        letter-spacing:4px;
                        color:#003366;
                    '>
                        {model.Usuario}
                    </p>
                    <p style='margin:0;font-size:13px;color:#64748b;text-transform:uppercase; margin: 15px 0;'>Senha provisória</p>
                    <p style='
                        margin:8px 0 0;
                        font-size:24px;
                        font-weight:bold;
                        letter-spacing:4px;
                        color:#003366;
                    '>
                        {model.Senha}
                    </p>
                </div>

                <div style='text-align:center;margin:32px 0;'>
                    <a href='{link}'
                       style='
                        background:#003366;
                        color:#ffffff;
                        padding:16px 32px;
                        text-decoration:none;
                        border-radius:6px;
                        font-weight:bold;
                        font-size:16px;
                        display:inline-block;
                       '>
                        Acessar Portal do BOPE
                    </a>
                </div>

                <hr style='border:none;border-top:1px solid #e2e8f0;margin:24px 0;'>
                
                <p style='font-size:13px; color:#94a3b8; text-align:center;'>
                    ""Tudo o que fizerem, façam de todo o coração, como para o Senhor."" - Colossenses 3:23
                </p>
            </td>
        </tr>
    </table>

</td>
</tr>
</table>

</body>
</html>
                    ";
                }
                else
                {
                    assunto = "Recuperação de senha - BOPE";
                    corpo = $@"
<!DOCTYPE html>
<html>
<body style='
    margin:0;
    padding:0;
    background:#f4f7f9;
    font-family: Arial, Helvetica, sans-serif;
    color:#334155;
'>

<table width='100%' cellpadding='0' cellspacing='0'>
<tr>
<td align='center'>

    <table width='100%' cellpadding='0' cellspacing='0' style='max-width:600px;padding:24px;'>

        <tr>
            <td align='center' style='padding-bottom:24px;'>
                <h1 style='margin:0;color:#003366;'>⛪ BOPE</h1>
                <p style='margin:4px 0 0;color:#64748b;font-size:14px;text-transform:uppercase;letter-spacing:1px;'>
                    Bem Organizados Para Evangelizar - IASD
                </p>
            </td>
        </tr>

        <tr>
            <td style='
                background:#ffffff;
                border-radius:8px;
                padding:40px 24px;
                box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1);
            '>

                <p style='font-size:18px; color:#0f172a;'>
                    Olá, <strong>{primeiroNome}</strong>.
                </p>

                <p style='font-size:15px;line-height:1.6;color:#475569;'>
                    Recebemos uma solicitação de redefinição de senha para o seu acesso ao <strong>Portal do BOPE</strong>.
                </p>

                <p style='font-size:15px;line-height:1.6;color:#475569;'>
                    Geramos uma <strong>senha provisória</strong> para você. 
                    Use-a para entrar no sistema e lembre-se de alterá-la para uma senha pessoal assim que realizar o login.
                </p>

                <div style='
                    background:#f8fafc;
                    border:1px dashed #003366;
                    border-radius:6px;
                    padding:20px;
                    margin:24px 0;
                    text-align:center;
                '>
                    <p style='margin:0;font-size:13px;color:#64748b;text-transform:uppercase;'>Usuário</p>
                    <p style='
                        margin:8px 0 0;
                        font-size:24px;
                        font-weight:bold;
                        letter-spacing:4px;
                        color:#003366;
                    '>
                        {model.Usuario}
                    </p>
                    <p style='margin:0;font-size:13px;color:#64748b;text-transform:uppercase; margin: 15px 0;'>Nova senha provisória</p>
                    <p style='
                        margin:8px 0 0;
                        font-size:24px;
                        font-weight:bold;
                        letter-spacing:4px;
                        color:#003366;
                    '>
                        {model.Senha}
                    </p>
                </div>

                <div style='text-align:center;margin:32px 0;'>
                    <a href='{link}'
                       style='
                        background:#003366;
                        color:#ffffff;
                        padding:16px 32px;
                        text-decoration:none;
                        border-radius:6px;
                        font-weight:bold;
                        font-size:16px;
                        display:inline-block;
                       '>
                        Acessar Portal do BOPE
                    </a>
                </div>
            </td>
        </tr>
    </table>

</td>
</tr>
</table>

</body>
</html>
";
                }
                await EnviarEmailAsync(destinatario, assunto, corpo);
            }
            catch (Exception ex)
            {
                throw new Exception($"Senha gerada/resetada, porém não foi possível enviar o e-mail com a senha. Tente novamente. Erro: {ex.Message}");
            }
        }

        // TODO: ADICIONAR LOGGER PARA CAPTURAR E REGISTRAR OS ERROS DAQUI
        public async Task EnviarInteressadoPorEmail(InteressadoCadastroModel model)
        {
            try
            {
                string assunto = string.Empty;
                string corpo = string.Empty;

                assunto = "Novo interessado! - Portal do BOPE";
                corpo = $@"
Um novo interessado foi cadastrado no sistema: <br/>
Nome: {model.NomeCompleto}<br/>
Bairro: {model.Bairro}<br/>
Quem cadastrou: {model.Professor}
                    ";

                await EnviarEmailAsync(_email, assunto, corpo);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao enviar e-mail: {ex.Message}");
            }
        }

        public async Task EnviarCestaPorEmail(CestaEntregaModel model)
        {
            try
            {
                string assunto = string.Empty;
                string corpo = string.Empty;

                assunto = "Nova entrega de Cesta Básica! - Portal do BOPE";
                corpo = $@"
<h3>Uma cesta básica foi entregue: </h3><br/>
Cesta Básica: {model.Cesta}<br/>
Quem entregou: {model.Entregador}<br/>
Quantidade restante: {model.Quantidade}
                    ";

                await EnviarEmailAsync(_email, assunto, corpo);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao enviar e-mail: {ex.Message}");
            }
        }

        public async Task EnviarCestaPorEmail(CestaBasicaModel model, string? alteracoes)
        {
            try
            {
                string assunto = string.Empty;
                string corpo = string.Empty;

                assunto = "Alteração em uma cesta básica! - Portal do BOPE";
                corpo = $@"
Uma alteração foi feita em uma cesta básica: <br/>
Nome: {model.Descricao}<br/>
Quem alterou: {model.UsuarioAlteracao} <br/>
Alterações: {alteracoes} 

                    ";

                await EnviarEmailAsync(_email, assunto, corpo);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao enviar e-mail: {ex.Message}");
            }
        }

        public async Task EnviarChamadaPorEmail(ChamadaEmailModel model)
        {
            try
            {
                string assunto = string.Empty;
                string corpo = string.Empty;

                assunto = "Novo relatório de estudo bíblico! - Portal do BOPE";
                corpo = $@"
<h3>Resumo do estudo:</h3> <br/>
<b>Professor</b>: {model.Professor}<br/>
<b>Aluno(s)</b>: {model.Alunos}<br/>
<b>Estudo</b>: {model.Estudo}<br/>
<b>Lição</b>: {model.LicaoID}<br/>
<b>Data</b>: {model.DataEstudo}<br/>
";

                await EnviarEmailAsync(_email, assunto, corpo);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao enviar e-mail: {ex.Message}");
            }
        }

        public async Task EnviarMovimentacaoPorEmail(MovimentacaoFinanceiraModel model)
        {
            try
            {
                string assunto = string.Empty;
                string corpo = string.Empty;

                assunto = $"Nova {model.Tipo} Financeira Cadastrada! - Portal do BOPE";
                corpo = $@"
<h3>Resumo da Movimentação financeira:</h3> <br/>
<b>Quem realizou</b>: {model.Usuario}<br/>
<b>Tipo</b>: {model.Tipo}<br/>
<b>Valor</b>: {model.Valor}<br/>
<b>Lição</b>: {model.Motivo} - {model.Descricao}<br/>";

                await EnviarEmailAsync(_email, assunto, corpo);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao enviar e-mail: {ex.Message}");
            }
        }
    }
}
