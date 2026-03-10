using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Z1.Model.APIs
{
    public class DevToolsChamadoModel : BaseModel
    {
        public int? id { get; set; }
        public string? titulo { get; set; }
        public string? descricao { get; set; }
        public int? status { get; set; }
        public int? prioridade { get; set; }
        public int? usuarioId { get; set; }
        public DateTime? dataAbertura { get; set; }
        public DateTime? dataFechamento { get; set; }
    }
}
