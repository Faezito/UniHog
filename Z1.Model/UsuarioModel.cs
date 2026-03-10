using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Z1.Model
{
    public class UsuarioModel : BaseModel
    {
        public int? PessoaID { get; set; }
        public int? TotalRegistros { get; set; }
        public int? SituacaoID { get; set; }
        public string? Situacao { get; set; }
        public string? SituacaoCor { get; set; }

        [Required(ErrorMessage = "O campo Nome Completo precisa ser preenchido.")]
        public string? NomeCompleto { get; set; }
        public string? Usuario { get; set; }
        public string? Email { get; set; }
        public string? Senha { get; set; }
        public string? ConfirmacaoSenha { get; set; }
        public string? CPF { get; set; }
        public string? Telefone { get; set; }

        [Required(ErrorMessage = "O campo Gênero precisa ser preenchido.")]
        public string? Genero { get; set; }
        public string? GeneroTxt { get; set; }
        public string? Tipo { get; set; }

        [Required(ErrorMessage = "O campo Cargo precisa ser preenchido.")]
        public int? TipoID { get; set; }
        public string? FotoPerfilURL { get; set; }
        public string? FotoPerfilPublicID { get; set; }
        public IFormFile? Foto { get; set; }

        [Required(ErrorMessage = "O campo Data de Nascimento precisa ser preenchido.")]
        [Range(typeof(DateTime), "1753-01-01", "9999-12-31",
        ErrorMessage = "A Data de Nascimento deve estar entre 01/01/1753 e 31/12/9999.")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DataNascimento { get; set; } = new DateTime(2001, 1, 1);
        public bool SenhaTemporaria { get; set; }
        public int? EstudoID { get; set; }
        public string? EstudoBiblico { get; set; }
        public int[]? DiasEstudoIDs { get; set; }
        public string? DiasEstudoIDsString { get; set; }
        public string? DiasEstudo { get; set; }
        public string? Projeto { get; set; }
        public int? ProjetoIndicacaoID { get; set; }
        public int? ProfessorID { get; set; }
        public string? Professor { get; set; }
        public int? interessadoId { get; set; }
        public bool? Empresa { get; set; }
        public DateTime? UltimaSessao { get; set; }
        public bool EstudaEmCasa { get; set; }
        public string? LocalEstudo
        {
            get
            {
                return EstudaEmCasa == true ? "Casa" : "Igreja";
            }
        }
        public List<EnderecoModel> Enderecos { get; set; } = new List<EnderecoModel>();
    }

    public class UsuarioDropModel
    {
        public int id { get; set; }
        public string text { get; set; }
    }
}
