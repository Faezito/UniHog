namespace Z1.Model
{
    public class ChamadoModel : BaseModel
    {
        public int? id { get; set; }
        public string? NomeUsuario { get; set; }
        public string? titulo { get; set; }
        public string? descricao { get; set; }
        public string? email { get; set; }
        public int? status { get; set; }
        public int? prioridade { get; set; }
        public int? usuarioId { get; set; }
        public DateTime? dataAbertura { get; set; }
        public DateTime? dataFechamento { get; set; }
    }
}
