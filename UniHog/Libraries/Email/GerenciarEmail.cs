using System.Net.Mail;
using Z1.Model.Email;

namespace UniHog.Libraries.Email
{
    public class GerenciarEmail
    {
        private SmtpClient _smtp;
        private IConfiguration _config;
        public GerenciarEmail(SmtpClient smp, IConfiguration config)
        {
            _smtp = smp;
            _config = config;
        }

        public void EnviarContatoPorEmail(ContatoModel contato)
        {
            string corpoMsg = string.Format("<h2>Contato - Loja Virtual</h2>"
                + "<b>Nome:</b> {0} <br/>" +
                "<b>Email:</b> {1} <br/>" +
                "<b>Texto:</b> {2} <br/>",

                contato.Nome,
                contato.Email,
                contato.Texto
                );

            string assunto = string.Format("Contato app Minha Cozinha - Email: " + contato.Email);


            MailMessage mensagem = new();
            mensagem.From = new MailAddress(_config.GetValue<string>("Email:Username"));
            mensagem.To.Add("matheusfae@gmail.com");
            mensagem.Subject = assunto;
            mensagem.Body = corpoMsg;
            mensagem.IsBodyHtml = true;

            _smtp.Send(mensagem);

        }

        public void EnviarSenhaPorEmail(dynamic model)
        {
            string corpoMsg = string.Format("<h2>Redefinição de senha - Minha Cozinha</h2>"
                + "<b>Nome:</b> {0} <br/>" +
                "<b>Email:</b> {1} <br/>" +
                "<b>Nova senha:</b> {2} <br/>",

                model.Nome,
                model.Email,
                model.Senha
                );

            string assunto = string.Format(model.Nome + " - Redefinição de senha - LojaVirtual");


            MailMessage mensagem = new();
            mensagem.From = new MailAddress(_config.GetValue<string>("Email:Username"));
            mensagem.To.Add(model.Email);
            mensagem.Subject = assunto;
            mensagem.Body = corpoMsg;
            mensagem.IsBodyHtml = true;

            _smtp.Send(mensagem);

        }



    }
}

