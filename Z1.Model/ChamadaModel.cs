using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Z1.Model
{
    public class ChamadaModel
    {
        public string? Professor { get; set; }
        public int? ProfessorID { get; set; }
        public string? Aluno { get; set; }
        public int? PessoaID { get; set; }
        public string? FotoPerfilURL { get; set; }
        public string? Estudo { get; set; }
        public string? PresenteTxt { get; set; }
        public string? PresenteCor { get; set; }
        public DateTime? DataEstudo { get; set; }
        public TimeSpan? Horario { get; set; }
        public bool? Presente { get; set; }
        public int? Licao { get; set; }
        public string? Obs { get; set; }
        public string? Tooltip { get; set; }
    }

    public class AlunoModel
    {
        public int? PessoaID { get; set; }
        public string? NomeCompleto { get; set; }
        public string? FotoPerfilURL { get; set; }
        public string? Estudo { get; set; }
        public string? PresenteTxt { get; set; }
        public string? PresenteCor { get; set; }
        public bool? Presente { get; set; }
        public int? Licao { get; set; }
        public string? Obs { get; set; }
    }
}
