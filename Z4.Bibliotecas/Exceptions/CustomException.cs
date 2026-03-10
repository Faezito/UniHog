using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Z4.Bibliotecas.Exceptions
{
    public abstract class CustomException : Exception
    {
        public string Cod { get; }
        public List<string> Itens { get; }

        protected CustomException(string cod, string message, List<string>? itens = null, Exception? innerException = null) : base(message, innerException)
        {
            Cod = cod;
            Itens = itens ?? new List<string>();
        }
    }

    public class ListException : CustomException
    {
        public ListException(string message, List<string> items)
            : base("VALIDATION_ERROR", message, items)
        {
        }
    }
}
