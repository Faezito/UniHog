namespace Z1.Model
{
    public class BaseModel
    {
        public int? UsuarioLogado { get; set; }
        public string? UsuarioLogadoString { get; set; }
        public DateTime? DataAlteracao { get; set; }
        public DateTime? DataCriacao { get; set; }
        public DateTime? DataDeletado { get; set; }
    }
}
