using System.ComponentModel.DataAnnotations;

namespace Z1.Model.Email
{
    public class ContatoModel
    {
        //[Required(ErrorMessageResourceType = typeof(Mensagem), ErrorMessageResourceName = "MSG_E001")]
        [Required(ErrorMessage = "Campo obrigatório!")]
        [MinLength(3, ErrorMessage = "O mínimo de caracteres necessário é 3.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "Campo obrigatório!")]
        [EmailAddress(ErrorMessage = "E-mail inválido.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Campo obrigatório!")]
        [MaxLength(1000, ErrorMessage = "O máximo de caracteres ultrapassado (1000).")]
        [MinLength(5, ErrorMessage = "O mínimo de caracteres necessário é 5.")]
        public string Texto { get; set; }
    }
}

