using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Z1.Model
{
    public class PaginacaoModel<T>
    {
        public List<T> Dados {  get; set; }
        public int Total { get; set; }
        public int PaginaAtual { get; set; }
        public int Quantidade { get; set; }
        public int TotalPaginas => (int)Math.Ceiling((double)Total / Quantidade);
    }
}
