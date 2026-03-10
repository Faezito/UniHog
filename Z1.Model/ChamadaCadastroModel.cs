namespace Z1.Model
{
    public class ChamadaCadastroModel : BaseModel
    {
        public int? ID { get; set; }
        public int ProfessorID { get; set; }
        public string? Professor { get; set; }
        public int? PessoaID { get; set; }
        public string? Aluno { get; set; }
        public DateTime DataEstudo { get; set; }
        public DateTime DataEstudo2 { get; set; }
        public TimeOnly? Horario { get; set; }
        public bool? Presente { get; set; }
        public int? Licao { get; set; }
        public string? Obs { get; set; }
        public int EstudoID { get; set; }
        public string? Estudo { get; set; }
        public bool? CestaBasica { get; set; }
    }

    public class ChamadaRQModel
    {
        public int? ID { get; set; }
        public int? ProfessorID { get; set; }
        public string? Aluno { get; set; }
        public int? PessoaID { get; set; }
        public int? Mes { get; set; }
        public int? Ano { get; set; }
        public int? LicaoID { get; set; }
    }

    public class ChamadaEmailModel
    {
        public int? ID { get; set; }
        public string? Professor { get; set; }
        public string? Alunos { get; set; }
        public int? LicaoID { get; set; }
        public string? Estudo { get; set; }
        public DateTime? DataEstudo { get; set; }
    }
}
