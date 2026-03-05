//using Microsoft.AspNetCore.Mvc;
//using Z1.Model;
//using OfficeOpenXml;
//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Globalization;
//using System.IO;
//using System.Linq;
//using System.Reflection;

//namespace UniHog.Libraries
//{
//    public class GeradorExcel : Controller
//    {
//        public IActionResult Excel<T>(string nomeExcel, string nomePlanilha, IList<T> lst)
//        {
//            try
//            {
//                ExcelPackage.License.SetNonCommercialPersonal("EPP Plus");

//                using (ExcelPackage package = new ExcelPackage(new FileInfo($"{nomeExcel}.xlsx")))
//                {
//                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(nomePlanilha);

//                    // PRIMEIRA LINHA
//                    PropertyInfo[] props = lst.FirstOrDefault().GetType().GetProperties();

//                    worksheet = PrimeiraLinha(props, worksheet, lst);
//                    worksheet = Corpo(props, worksheet, lst);

//                    using (var stream = new MemoryStream())
//                    {
//                        package.SaveAs(stream);
//                        return File(
//                            fileContents: stream.ToArray(),
//                            contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
//                            fileDownloadName: $"{nomeExcel}.xlsx"
//                        );
//                    }
//                }
//            }
//            catch (Exception)
//            {
//                throw;
//            }
//        }

//        public IActionResult Excel<T>(string nomeExcel, IDictionary<string, dynamic> dics)
//        {
//            try
//            {
//                ExcelPackage.License.SetNonCommercialPersonal("EPP Plus");

//                using (ExcelPackage package = new ExcelPackage(new FileInfo($"{nomeExcel}.xlsx")))
//                {
//                    foreach (var dic in dics)
//                    {
//                        ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(dic.Key);
//                    }


//                    // PRIMEIRA LINHA
//                    using (var stream = new MemoryStream())
//                    {
//                        package.SaveAs(stream);
//                        return File(
//                            fileContents: stream.ToArray(),
//                            contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
//                            fileDownloadName: $"{nomeExcel}.xlsx"
//                        );
//                    }
//                }
//            }
//            catch (Exception)
//            {
//                throw;
//            }
//        }

//        public IActionResult ExcelDatatable(string nomeExcel, string nomeAba, DataTable dataTable)
//        {
//            try
//            {
//                ExcelPackage.License.SetNonCommercialPersonal("EPP Plus");
//                using (ExcelPackage package = new ExcelPackage(nomeExcel))
//                {
//                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(nomeAba);
//                    worksheet.Cells["A1"].LoadFromDataTable(dataTable, true);

//                    // HABILITA OS FILTROS
//                    worksheet.Cells[worksheet.Columns.Range.ToString()].AutoFilter = true;
//                    worksheet.Cells[1, 1, 1, dataTable.Columns.Count].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
//                    worksheet.Cells[1, 1, 1, dataTable.Columns.Count].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

//                    // CONGELAR A PRIMEIRA LINHA
//                    worksheet.View.FreezePanes(2, 1);

//                    // AJUSTA A LARGURA DAS COLUNAS
//                    worksheet.Cells.AutoFitColumns();

//                    using (var stream = new MemoryStream())
//                    {
//                        package.SaveAs(stream);
//                        return File(
//                            fileContents: stream.ToArray(),
//                            contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
//                            fileDownloadName: $"{nomeExcel}.xlsx"
//                        );
//                    }
//                }
//            }
//            catch (Exception)
//            {
//                throw;
//            }
//        }

//        public IActionResult ExcelComObjeto<T>(string nomeExcel, T model)
//        {
//            try
//            {
//                ExcelPackage.License.SetNonCommercialPersonal("EPP Plus");
//                using (ExcelPackage package = new ExcelPackage(new FileInfo($"{nomeExcel}.xlsx")))
//                {
//                    ExcelWorksheet worksheet = null;

//                    if (model != null)
//                    {
//                        PropertyInfo[] props = model.GetType().GetProperties();

//                        for (int i = 0; i < props.Length; i++)
//                        {
//                            PropertyInfo tempProps = props[i];

//                            string nomeAba = tempProps.Name;
//                            worksheet = package.Workbook.Worksheets.Add(nomeAba);

//                            // Lista
//                            if (tempProps.PropertyType.IsGenericType && tempProps.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
//                            {
//                                foreach (var item in tempProps.GetType().GetProperties())
//                                {

//                                }
//                            }
//                            else
//                            {
//                                // Objeto
//                            }
//                        }
//                    }

//                    using (var stream = new MemoryStream())
//                    {
//                        package.SaveAs(stream);
//                        return File(
//                            fileContents: stream.ToArray(),
//                            contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
//                            fileDownloadName: $"{nomeExcel}.xlsx"
//                        );
//                    }
//                }
//            }
//            catch (Exception)
//            {
//                throw;
//            }
//        }

//        public IActionResult DownloadExcelDRE(FileInfo excelDRE, List<ProdutoModel> lst, int mes)
//        {
//            try
//            {
//                using (var stream = excelDRE.OpenRead())
//                {
//                    ExcelPackage.License.SetNonCommercialPersonal("EPP Plus");

//                    using (var package = new ExcelPackage(stream))
//                    {
//                        ExcelWorksheet worksheet = package.Workbook.Worksheets["base"];
//                        PropertyInfo[] props = lst.FirstOrDefault().GetType().GetProperties();

//                        // Cabeçalhos
//                        worksheet.Cells["A1"].Value = "CdEmpresa";
//                        worksheet.Cells["B1"].Value = "DsApelido";
//                        worksheet.Cells["C1"].Value = "Descricao";
//                        worksheet.Cells["D1"].Value = "Anterior1";
//                        worksheet.Cells["E1"].Value = "Anterior2";
//                        worksheet.Cells["F1"].Value = "Anterior3";
//                        worksheet.Cells["G1"].Value = "FatAnterior1";
//                        worksheet.Cells["H1"].Value = "FatAnterior2";
//                        worksheet.Cells["I1"].Value = "FatAnterior3";
//                        worksheet.Cells["J1"].Value = "RazaoSocial";

//                        for (int row = 0; row < lst.Count; row++)
//                        {
//                            var temp = lst[row];
//                            props = temp.GetType().GetProperties();

//                            worksheet.Cells[row + 2, 1].Value = temp.ProdutoID;
//                            worksheet.Cells[row + 2, 2].Value = temp.ProdutoDescricao;
//                            worksheet.Cells[row + 2, 3].Value = temp.Preco;
//                        }

//                        // COMPOE OS MESES REFERENTES ABAIXO
//                        DateTime dtTemp = new DateTime(2021, mes, 1);

//                        // 2 MESES ANTERIOR
//                        package.Workbook.Worksheets[0].Cells["D3"].Value = dtTemp.AddMonths(-2).ToString("MMM");

//                        // 1 MES ANTERIOR
//                        package.Workbook.Worksheets[0].Cells["E3"].Value = dtTemp.AddMonths(-1).ToString("MMM");

//                        // MÊS ATUAL
//                        package.Workbook.Worksheets[0].Cells["F3"].Value = dtTemp.ToString("MMM");

//                        using (var ms = new MemoryStream())
//                        {
//                            package.SaveAs(ms);
//                            return File(
//                                fileContents: ms.ToArray(),
//                                contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
//                                fileDownloadName: "DRE.xlsx"
//                            );
//                        }
//                    }
//                }
//            }
//            catch (Exception)
//            {
//                throw;
//            }
//        }

//        public IActionResult DownloadExcel2Aba<T>(string nomeExcel, string nomeAba1, string nomeAba2, List<T> lst1, List<T> lst2)
//        {
//            try
//            {
//                ExcelPackage.License.SetNonCommercialPersonal("EPP Plus");
//                using (ExcelPackage package = new ExcelPackage(new FileInfo($"{nomeExcel}.xlsx")))
//                {
//                    ExcelWorksheet worksheet1 = package.Workbook.Worksheets.Add(nomeAba1);

//                    PropertyInfo[] props = lst1.FirstOrDefault().GetType().GetProperties();
//                    for (int i = 0; i < props.Length; i++)
//                    {
//                        worksheet1.Cells[1, i + 1].Value = props[i].Name;
//                    }

//                    for (int row = 0; row < lst1.Count; row++)
//                    {
//                        var temp = lst1[row];
//                        props = temp.GetType().GetProperties();

//                        for (int col = 0; col < props.Length; col++)
//                        {
//                            worksheet1.Cells[row + 2, col + 1].Value = props[col].GetValue(temp);
//                        }
//                    }

//                    ExcelWorksheet worksheet2 = package.Workbook.Worksheets.Add(nomeAba2);

//                    PropertyInfo[] props2 = lst2.FirstOrDefault().GetType().GetProperties();
//                    for (int i = 0; i < props2.Length; i++)
//                    {
//                        worksheet2.Cells[1, i + 1].Value = props2[i].Name;
//                    }

//                    for (int row = 0; row < lst2.Count; row++)
//                    {
//                        var temp = lst2[row];
//                        props2 = temp.GetType().GetProperties();

//                        for (int col = 0; col < props.Length; col++)
//                        {
//                            worksheet2.Cells[row + 2, col + 1].Value = props2[col].GetValue(temp);
//                        }
//                    }

//                    using (var stream = new MemoryStream())
//                    {
//                        package.SaveAs(stream);
//                        return File(
//                            fileContents: stream.ToArray(),
//                            contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
//                            fileDownloadName: $"{nomeExcel}.xlsx"
//                        );
//                    }
//                }
//            }
//            catch (Exception)
//            {
//                throw;
//            }
//        }





//        // CONFIGURAÇÕES PADRÕES
//        private static ExcelWorksheet PrimeiraLinha<T>(PropertyInfo[] props, ExcelWorksheet worksheet, IList<T> lst)
//        {
//            for (int i = 0; i < props.Length; i++)
//            {
//                PropertyInfo pro = props[i];
//                Attribute attr = pro.GetCustomAttributes().FirstOrDefault();

//                if (attr == null)
//                {
//                    worksheet.Cells[1, i + 1].Value = pro.Name;
//                }
//                else
//                {
//                    string nome = string.Empty;
//                    try
//                    {
//                        System.ComponentModel.DisplayNameAttribute attrName = (System.ComponentModel.DisplayNameAttribute)attr;
//                        nome = attrName.DisplayName;
//                    }
//                    catch (Exception)
//                    {
//                        nome = pro.Name;
//                    }

//                    worksheet.Cells[1, i + 1].Value = nome;
//                }
//            }

//            // HABILITA OS FILTROS
//            worksheet.Cells[worksheet.Columns.Range.ToString()].AutoFilter = true;
//            worksheet.Cells[1, 1, 1, props.Length].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
//            worksheet.Cells[1, 1, 1, props.Length].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

//            return worksheet;
//        }

//        private static ExcelWorksheet Corpo<T>(PropertyInfo[] props, ExcelWorksheet worksheet, IList<T> lst)
//        {
//            // CORPO
//            for (int row = 0; row < lst.Count; row++)
//            {
//                var temp = lst[row];
//                props = temp.GetType().GetProperties();

//                for (int col = 0; col < props.Length; col++)
//                {
//                    var tempCol = props[col];
//                    var tempWorksheet = worksheet.Cells[row + 2, col + 1];

//                    string tipoColuna = string.Empty;

//                    if (tempCol.PropertyType.GenericTypeArguments.Count() == 0)
//                    {
//                        tipoColuna = tempCol.PropertyType.Name;
//                    }
//                    else
//                    {
//                        tipoColuna = tempCol.PropertyType.GenericTypeArguments[0].Name;
//                    }

//                    if (tipoColuna == "String")
//                    {
//                        tempWorksheet.Style.Numberformat.Format = "@";
//                    }
//                    else if (tipoColuna == "DateTime")
//                    {
//                        tempWorksheet.Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
//                        tempWorksheet.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
//                    }
//                    else if (tipoColuna == "Decimal")
//                    {
//                        tempWorksheet.Style.Numberformat.Format = "#,##0.00";
//                    }
//                    else if (tipoColuna == "Int32" || tipoColuna == "int")
//                    {
//                        tempWorksheet.Style.Numberformat.Format = "0";
//                    }

//                    tempWorksheet.Value = tempCol.GetValue(temp);
//                }
//            }

//            // CONGELAR A PRIMEIRA LINHA
//            worksheet.View.FreezePanes(2, 1);

//            // AJUSTA A LARGURA DAS COLUNAS
//            worksheet.Cells.AutoFitColumns();

//            return worksheet;
//        }
//    }
//}
