using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Z1.Model
{
    public class EspecialistaModel : UsuarioModel
    {
        public int? EspecialidadeID { get; set; }
        public string? Especialidade {  get; set; }
        public List<Funcionamento>? Funcionamento { get; set; }
    }
}
