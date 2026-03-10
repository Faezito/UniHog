using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Z1.Model.Email
{
    public class EmailConfig
    {
        public int? ID { get; set; }
        public string Descricao { get; set; }
        public string Email { get; set; }
        public string Server { get; set; }
        public int Port { get; set; }
        public bool UseSSL { get; set; }
        public bool UseStartTls { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string FromName { get; set; }
        public string? Remetente { get; set; }
    }

}
