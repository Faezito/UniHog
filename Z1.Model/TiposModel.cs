namespace Z1.Model
{
    public class TiposModel
    {
        public int? ID { get; set; }
        public string? Descricao { get; set; }
        public string? Icone { get; set; }
        public string? Cor { get; set; }
        public bool podeExcluir { get; set; }
        public int Selecao { get; set; }
    }

    public class EspecialidadeModel
    {
        public int? ID { get; set; }
        public string? Descricao { get; set; }
        public int? AreaID { get; set; }
        public string? Area { get; set; }
    }
}
