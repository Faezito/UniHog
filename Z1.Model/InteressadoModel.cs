using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using Z1.Model;

public class InteressadoModel : BaseModel
{
    public int? PessoaID { get; set; }
    public bool? Interessado { get; set; }
    public string? InteressadoTxt { get; set; }
    public string? NomeCompleto { get; set; }
    public string? Usuario { get; set; }
    public string? Email { get; set; }
    public string? CPF { get; set; }
    public string? Telefone { get; set; }

    [Required(ErrorMessage = "Selecione um gênero")]
    public string? Genero { get; set; }
    public string? GeneroTxt { get; set; }
    public string? FotoPerfilURL { get; set; }
    public string? FotoPerfilPublicID { get; set; }
    public IFormFile? Foto { get; set; }

    [Required(ErrorMessage = "O campo Data de Nascimento precisa ser preenchido.")]
    [Range(typeof(DateTime), "1753-01-01", "9999-12-31",
    ErrorMessage = "A Data de Nascimento deve estar entre 01/01/1753 e 31/12/9999.")]
    public DateTime DataNascimento { get; set; }
    public int[]? DiasEstudoIDs { get; set; }
    public int EstudoID { get; set; }
    public string? DiasEstudoIDsString { get; set; }
    public string? DiasEstudo { get; set; }
    public string? Projeto { get; set; }
    public int? ProjetoIndicacaoID { get; set; }
    public int? ProfessorID { get; set; }
    public string? Professor { get; set; }
    public int? SituacaoID { get; set; }
    public string? SituacaoCor { get; set; }
    public List<EnderecoModel> Enderecos { get; set; } = new List<EnderecoModel>();
}