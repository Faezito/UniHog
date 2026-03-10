public static class ArquivosPastas
{
    public static void CriarDiretorio(string caminho)
    {
        if (!Directory.Exists(caminho))
        {
            Directory.CreateDirectory(caminho);
        }
    }
}
