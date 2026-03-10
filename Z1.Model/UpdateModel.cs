using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Z1.Model
{
    public class UpdateModel
    {
        public int id { get; set; }
        public int sistemaId { get; set; }
        public string titulo { get; set; }
        public string texto { get; set; }
        public DateTime dataAtualizacao { get; set; }

    }
}
