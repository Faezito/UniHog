namespace Z1.Model
{
    public class PessoaJuridicaModel : BaseModel
    {
        public int? PessoaJuridicaID { get; set; }
        public string? NomeFantasia { get; set; }
        public string? RazaoSocial { get; set; }
        public string? Email { get; set; }
        public string? Telefone { get; set; }
        public bool Ativo { get; set; }
        public string? Situacao { get; set; }
        public string? SituacaoCor { get; set; }
        public string? CNPJ { get; set; }
        public int? AreaID { get; set; }
        public string? Area { get; set; }
        public int? EspecialidadeID { get; set; }
        public string? Especialidade { get; set; }
        public string? Estado { get; set; }
        public string? Referencia { get; set; }
        public string? Complemento { get; set; }
        public string? Numero { get; set; }
        public string? Rua { get; set; }
        public string? CEP { get; set; }
        public string? Endereco { get; set; }
        public string? Bairro { get; set; }
        public string? Cidade { get; set; }
        public string? UF { get; set; }
        public string? Regiao { get; set; }
        public string? EnderecoReduzido { get; set; }
        public string? EnderecoCompleto { get; set; }
        public string? Registro { get; set; }
        public List<Funcionamento>? Funcionamento { get; set; }
    }


    public class PessoaJuridicaRQModel
    {
        public int? PessoaJuridicaID { get; set; }
        public int? PessoaID { get; set; }
        public string? NomeFantasia { get; set; }
        public string? RazaoSocial { get; set; }
        public bool? Ativo { get; set; }
        public string? CNPJ { get; set; }
        public int? AreaID { get; set; }
        public int? EspecialidadeID { get; set; }
    }
}
