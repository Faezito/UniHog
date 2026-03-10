using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Z1.Model.APIs
{
    public class APIModel
    {
        public int? ID { get; set; }
        public int? COD { get; set; }
        public string? Descricao { get; set; }
        public string? Token { get; set; }
        public string? Modelo { get; set; }
        public string? Url { get; set; }
        public string? Usuario { get; set; }
        public string? Senha { get; set; }
    }
}
