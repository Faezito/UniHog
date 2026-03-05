namespace UniHog.Libraries.Sessao
{
    public class Sessao
    {
        private readonly IHttpContextAccessor _context;

        public Sessao(IHttpContextAccessor context)
        {
            _context = context;
        }

        public void Cadastrar(string key, string valor)
        {
            _context.HttpContext?.Session.SetString(key, valor);
        }

        public void Atualizar(string key, string valor)
        {
            _context.HttpContext?.Session.SetString(key, valor);
        }

        public void Remover(string key)
        {
            _context.HttpContext?.Session.Remove(key);
        }

        public T Consultar<T>(string key)
        {
            var valor = _context.HttpContext?.Session.GetString(key);
            if (string.IsNullOrWhiteSpace(valor))
                return default;

            return (T)Convert.ChangeType(valor, typeof(T));
        }

        public bool Existe(string key)
        {
            return _context.HttpContext?.Session.GetString(key) != null;
        }

        public void RemoverTodos()
        {
            _context.HttpContext?.Session.Clear();
        }
    }
}