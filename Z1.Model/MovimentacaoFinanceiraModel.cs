using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Z1.Model
{
    public class MovimentacaoFinanceiraModel : BaseModel
    {
        public int? ID { get; set; }

        [Required(ErrorMessage = "Insira uma descrição.")]
        public string? Descricao { get; set; }

        [Required(ErrorMessage = "Tipo de movimentação inválida.")]
        public string? Tipo { get; set; }
        public string? TipoTxt { get; set; }
        public string? TipoCor { get; set; }
        public decimal? Valor { get; set; }
        public decimal? ValorParcela { get; set; }
        public int? QuantidadeParcelas { get; set; }
        public int? Parcela { get; set; }
        public int? PessoaID { get; set; }
        public int? MotivoID { get; set; }
        public int? PaiID { get; set; }
        public string? Motivo { get; set; }
        public string? Usuario { get; set; }

        [Required(ErrorMessage = "Data de Movimentacao inválida.")]
        [Range(typeof(DateTime), "2000-01-01", "9999-12-31",
        ErrorMessage = "Data de Movimentacao inválida.")]
        public DateTime? DataMovimentacao { get; set; }
        public IFormFile? NotaFiscal { get; set; }
        public string? NotaFiscalURL { get; set; }
        public string? NotaFiscalPublicID { get; set; }
    }
}
