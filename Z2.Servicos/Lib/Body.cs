namespace Z2.Servicos.Lib
{
    public static class Body
    {
        private static string _path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "TemplateBoleto");
        private static string _pathLogoBanco = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "LogoBanco");

        public static string Template(string template)
        {
            string path = Path.Combine(_path, template);
            return File.ReadAllText(path);
        }

        public static string TemplateLogoBanco(string template)
        {
            string path = Path.Combine(_pathLogoBanco, template);
            return path;
        }
    }
}
