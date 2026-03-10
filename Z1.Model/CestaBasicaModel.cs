using System.ComponentModel.DataAnnotations;

namespace Z1.Model
{
    public class CestaBasicaModel : BaseModel
    {
        public int? ID { get; set; }

        [Required(ErrorMessage = "Um nome é necessário.")]
        [StringLength(100, ErrorMessage = "Um nome é necessário.")]
        public string? Descricao { get; set; }
        public string? Itens { get; set; }
        public decimal? Custo { get; set; }
        public int? Quantidade { get; set; }
        public int? EstoqueMin { get; set; }
        public string? UsuarioAlteracao { get; set; }
    }

    public class CestaEntregaModel : BaseModel
    {
        public int? ID { get; set; }

        [Required(ErrorMessage = "Beneficiário não encontrado")]
        public int? PessoaID { get; set; }

        [Required(ErrorMessage = "Uma cesta é necessária")]
        public int? CestaID { get; set; }

        [Required(ErrorMessage = "Usuário não encontrado")]
        public int? EntregadorID { get; set; }

        [Required(ErrorMessage = "A Data da entrega precisa ser preenchida")]
        public DateTime? DataEntrega { get; set; }
        public string? Cesta { get; set; }
        public string? Entregador { get; set; }
        public int? Quantidade { get; set; }
    }
}
