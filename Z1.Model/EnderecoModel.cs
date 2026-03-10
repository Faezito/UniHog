using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Z1.Model
{
    public class EnderecoModel
    {
        public int? ID { get; set; }
        public int? PessoaID { get; set; }
        public string? EnderecoCompleto { get; set; }
        public string? EnderecoReduzido { get; set; }
        public string? Tipo { get; set; }
        public int? TipoID { get; set; }
        public string? CEP { get; set; }
        public string? EnderecoMapsURL { get; set; }
        public string? Rua { get; set; }
        public string? Numero { get; set; }
        public string? Bairro { get; set; }
        public string? Cidade { get; set; }
        public string? Estado { get; set; }
        public string? Pais { get; set; }
        public string? UF { get; set; }
        public string? Regiao { get; set; }
        public string? Complemento { get; set; }
        public string? Referencia { get; set; }

    }

    public class BairroModel
    {
        public int? ID { get; set; }
        public string Nome { get; set; }
        public int? CongregacaoID { get; set; }
    }
}
