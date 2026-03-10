using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Z1.Model
{
    public class EstudoModel
    {
        public int? ESTUDOID { get; set; }
        public string? Descricao { get; set; }
    }

    public class DiasEstudoModel
    {
        public int ID { get; set; }
        public int PessoaID { get; set; }
        public int DiaID { get; set; }
        public TimeSpan? Horario { get; set; }
    }
}
