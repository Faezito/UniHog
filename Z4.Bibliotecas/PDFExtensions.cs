using iText.Kernel.Pdf;
using iText.Layout.Element;
using Document = iText.Layout.Document;
using System.Linq;

namespace Z4.Bibliotecas
{
    public static class PDFExtensions
    {
        public static byte[] GerarPDF<T>(List<T> obj, string titulo, List<(string Cabecalho, Func<T, string> Valor)> colunas)
        {
            try
            {
                using var stream = new MemoryStream();
                using var writer = new PdfWriter(stream);
                using var pdf = new PdfDocument(writer);
                using var document = new Document(pdf);

                document.Add(new Paragraph(titulo)
                    .SetFontSize(16)
                    .SimulateBold()
                    );

                var tabela = new Table(4).UseAllAvailableWidth();

                foreach (var col in colunas)
                    tabela.AddHeaderCell(new Cell().Add(new Paragraph(col.Cabecalho).SimulateBold()));

                foreach (var item in obj)
                    foreach (var col in colunas)
                        tabela.AddCell(col.Valor(item) ?? "");

                document.Add(tabela);
                document.Close();

                return stream.ToArray();
            }
            catch (Exception ex)
            {
                throw;
            }
            // COMO USAR

            /*
             
                var pdf = PDFExtensions.GerarPDF(
                    atendimentos.Cast<dynamic>().ToList(),
                    $"Atendimentos - {Mes.ObterMesCompleto()}",
                    colunas:
                    [
                        ("Paciente",      item => item.NomeCompleto),
                        ("Telefone",      item => item.Telefone),
                        ("Especialidade", item => item.Especialidade),
                        ("Data",          item => item.DataAtendimento?.ToString("dd/MM/yyyy HH:mm"))
                    ]
                );

                return File(pdf, "application/pdf", $"Atendimentos_{Mes.ObterMesCompleto()}.pdf");
             
             */
        }
    }
}
