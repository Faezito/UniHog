using System.ComponentModel.DataAnnotations;

namespace Z1.Model
{
    public class UsuarioCadastroModel : UsuarioModel
    {
        public string? EnderecoCompleto { get; set; }
        public string? EnderecoEncoded { get; set; }
        public string? CEP { get; set; }
        public string? EnderecoMapsURL { get; set; }
        public string? Rua { get; set; }
        public string? Numero { get; set; }

        [Required(ErrorMessage = "Selecione um bairro.", AllowEmptyStrings = false)]
        public string? Bairro { get; set; }
        public string? Cidade { get; set; }
        public string? Estado { get; set; }
        public string? Pais { get; set; } = "Brasil";
        public string? UF { get; set; }
        public string? Regiao { get; set; }
        public string? Complemento { get; set; }
        public string? Referencia { get; set; }
        public int? EnderecoTipoID { get; set; } = 1;
    }
}
