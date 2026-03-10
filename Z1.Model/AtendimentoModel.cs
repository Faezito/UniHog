namespace Z1.Model
{
    public class AtendimentoModel : BaseModel
    {
        public int? ID { get; set; }
        public int? PacienteID { get; set; }
        public string? NomeCompleto { get; set; }
        public int? EspecialidadeID { get; set; }
        public List<int> EspecialidadeIDs { get; set; }
        public string EspecialidadeIDsString { get; set; }
        public int? EspecialistaID { get; set; }
        public string EspecialistaIDs { get; set; }
        public int? PessoaJuridicaID { get; set; }
        public string? Especialista { get; set; }
        public DateTime? DataAtendimento { get; set; }
        public DateTime? DataNascimento { get; set; }
        public string SituacaoID { get; set; } = "R";
        public string? Situacao { get; set; }
        public string? Especialidade { get; set; }
        public int AreaID { get; set; }
        public List<int> AreaIDs { get; set; }
        public string AreaIDsString { get; set; }
        public int? TipoID { get; set; }
        public string? Tipo { get; set; }
        public string? Area { get; set; }
        public string? Telefone { get; set; }
        public string? Genero { get; set; }
        public string? Observacao { get; set; }
        public decimal? Custo { get; set; }
        public int DiaID { get; set; }
    }

    public class DiasDaSemana
    {
        public int DiaID { get; set; }
        public string Dia { get; set; }
    }

    public class Funcionamento
    {
        public int? EspecialistaID { get; set; }
        public int? PessoaJuridicaID { get; set; }
        public int? DiaID { get; set; }
        public TimeSpan? ManhaAbertura { get; set; }
        public TimeSpan? ManhaFechamento { get; set; }
        public TimeSpan? TardeAbertura { get; set; }
        public TimeSpan? TardeFechamento { get; set; }
    }

    public class AtendimentoRQModel
    {
        public int? ID { get; set; }
        public int? PacienteID { get; set; }
        public int? EspecialistaID { get; set; }
        public int? EspecialidadeID { get; set; }
        public int? AreaID { get; set; }
        public DateTime? DataIni {  get; set; }
        public DateTime? Data {  get; set; }
        public DateTime? DataFim {  get; set; }
    }
}
