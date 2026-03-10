namespace Z1.Model
{
    public class CestaBasicaBeneficiarioModel : BaseModel
    {
        public int? PessoaID { get; set; }
        public string? NomeCompleto { get; set; }
        public string? Telefone { get; set; }
        public string? FotoPerfilURL { get; set; }
        public string? EnderecoReduzido { get; set; }
        public string? EnderecoCompleto { get; set; }
        public int? Presencas { get; set; }
        public int? Faltas { get; set; }
        public int? EntregadorID { get; set; }
        public string? Entregador { get; set; }
        public string? RecebeCesta { get; set; }
        public bool? Entregue { get; set; }
        public DateTime? DataEntrega { get; set; }
    }
}