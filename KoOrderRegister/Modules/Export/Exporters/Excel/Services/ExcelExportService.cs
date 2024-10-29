using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using KoOrderRegister.Localization;
using KORCore.Modules.Database.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KORCore.Modules.Database.Services;
using KORCore.Modules.Database.Factory;



namespace KoOrderRegister.Modules.Export.Exporters.Excel.Services
{
    public class ExcelExportService : IExcelExportService
    {
        private readonly IDatabaseModel _databaseModel;
        private string _outputPath;
        private bool _isExporting = false;

        private ProgressState progressState;
        public ExcelExportService(IDatabaseModelFactory databaseModel)
        {
            _databaseModel = databaseModel.Get();

        }
        public async Task Export(string outputPath, CancellationToken cancellationToken, Action<float> progressCallback = null)
        {
            long totalCustomers = await _databaseModel.CountCustomers() * 2 + await _databaseModel.CountOrders() * 4 + await _databaseModel.CountFiles() * 3;
            progressState = new ProgressState(totalCustomers, progressCallback);
            var fileName = $"KoOrderRegister_{DateTime.Now:yyyy_MM_dd_HH_mm_ss}.xlsx";
            outputPath = Path.Combine(outputPath, $"kor_export_{DateTime.Now:yyyy_MM_dd_HH_mm_ss}", fileName);
            _outputPath = Path.GetDirectoryName(outputPath);
            string filePath = Path.Combine(Path.GetDirectoryName(outputPath), $"Files_{DateTime.Now:yyyy_MM_dd_HH_mm_ss}");
            string relativePath = $@"./Files_{DateTime.Now:yyyy_MM_dd_HH_mm_ss}";
            using (var workbook = new XLWorkbook())
            {

                IXLWorksheet customerSheet = workbook.Worksheets.Add(AppRes.Customers);
                customerSheet.Cell(1, 1).Value = AppRes.Name;
                customerSheet.Cell(1, 2).Value = AppRes.Address;
                customerSheet.Cell(1, 3).Value = AppRes.Phone;
                customerSheet.Cell(1, 4).Value = AppRes.Email;
                customerSheet.Cell(1, 5).Value = AppRes.NHI;
                customerSheet.Cell(1, 6).Value = AppRes.Note;

                int row = 2;
                await foreach (var customer in _databaseModel.GetAllCustomersAsStream(cancellationToken))
                {
                    customerSheet.Cell(row, 1).Value = customer.Name;
                    customerSheet.Cell(row, 2).Value = customer.Address;
                    customerSheet.Cell(row, 3).Value = customer.Phone;
                    customerSheet.Cell(row, 4).Value = customer.Email;
                    customerSheet.Cell(row, 5).Value = customer.NationalHealthInsurance;
                    customerSheet.Cell(row, 6).Value = customer.Note;

                    row++;

                    progressState.UpdateProgress();
                }

                IXLWorksheet orderSheet = workbook.Worksheets.Add(AppRes.Orders);
                orderSheet.Cell(1, 1).Value = AppRes.OrderNumber;
                orderSheet.Cell(1, 2).Value = AppRes.NHI;
                orderSheet.Cell(1, 3).Value = AppRes.Name;
                orderSheet.Cell(1, 4).Value = AppRes.Price;
                orderSheet.Cell(1, 5).Value = AppRes.StartDate;
                orderSheet.Cell(1, 6).Value = AppRes.EndDate;
                orderSheet.Cell(1, 7).Value = AppRes.Note;

                orderSheet.Cell(1, 8).Value = AppRes.FilesName;



                row = 2;

                await foreach (var order in _databaseModel.GetAllOrdersAsStream(cancellationToken))
                {
                    orderSheet.Cell(row, 1).Value = order.OrderNumber;
                    orderSheet.Cell(row, 2).Value = order.Customer.NationalHealthInsurance;
                    orderSheet.Cell(row, 3).Value = order.Customer.Name;
                    orderSheet.Cell(row, 4).Value = order.Price;
                    orderSheet.Cell(row, 5).Value = order.StartDate;
                    orderSheet.Cell(row, 6).Value = order.EndDate;
                    orderSheet.Cell(row, 7).Value = order.Note;

                    string filesName = "";
                    foreach (var file in order.Files)
                    {
                        filesName += file.Name + "; ";
                    }
                    orderSheet.Cell(row, 8).Value = filesName;
                    SettHyperLinkToOrdersCustomers(row, order.Customer.NationalHealthInsurance, customerSheet, orderSheet);

                    row++;


                    progressState.UpdateProgress();
                }

                IXLWorksheet fileSheet = workbook.Worksheets.Add(AppRes.Files);
                fileSheet.Cell(1, 1).Value = AppRes.FileName;
                fileSheet.Cell(1, 2).Value = AppRes.OrderNumber;
                fileSheet.Cell(1, 3).Value = AppRes.FilePath;

                row = 2;
                await foreach (var file in _databaseModel.GetAllFilesAsStream(cancellationToken))
                {
                    fileSheet.Cell(row, 1).Value = file.Name;
                    fileSheet.Cell(row, 2).Value = file.Order.OrderNumber;
                    if (file.Content != null)
                    {
                        string newFile = Path.Combine(relativePath, file.Name);
                        string newFilePath = Path.Combine(filePath, file.Name);
                        Directory.CreateDirectory(Path.GetDirectoryName(newFilePath));
                        File.WriteAllBytes(newFilePath, file.Content);
                        fileSheet.Cell(row, 3).Value = newFile;
                        fileSheet.Cell(row, 3).SetHyperlink(new XLHyperlink(newFile));
                    }
                    else
                    {
                        fileSheet.Cell(row, 3).Value = AppRes.FileError;
                    }

                    SettHyperLinkToFiles(row, file.Order.OrderNumber, fileSheet, orderSheet);
                    row++;

                    progressState.UpdateProgress();
                }


                int maxRow = Math.Min(orderSheet.RowCount(), 1048576);
                /* for (int i = 2; i <= maxRow; i++)
                 {
                     string cellValue = orderSheet.Cell(i, 8).GetValue<string>();
                     SettHyperLinkToOrdersFiles(row, cellValue, orderSheet, fileSheet);
                 }*/

                workbook.SaveAs(outputPath);
                _isExporting = true;
            }


        }

        private void SettHyperLinkToFiles(int row, string orderId, IXLWorksheet fileSheet, IXLWorksheet orderSheet)
        {
            int maxRow = Math.Min(orderSheet.RowCount(), 1048576);

            for (int i = 2; i <= maxRow; i++)
            {
                string cellValue = orderSheet.Cell(i, 1).GetValue<string>();
                if (cellValue.Equals(orderId))
                {
                    string hyperlinkTXT = $"'{AppRes.Orders}'!A{i}";
                    fileSheet.Cell(row, 2).SetHyperlink(new XLHyperlink(hyperlinkTXT));
                }
            }
        }

        private void SettHyperLinkToOrdersFiles(int row, string filesName, IXLWorksheet fileSheet, IXLWorksheet orderSheet)
        {
            int maxRow = Math.Min(orderSheet.RowCount(), 1048576);
            int maxRowFile = Math.Min(fileSheet.RowCount(), 1048576);

            List<string> filesNameList = filesName.Split("; ").ToList();
            for (int i = 2; i <= maxRow; i++)
            {
                string cellValue = orderSheet.Cell(i, 8).GetValue<string>();
                if (cellValue.Equals(filesName))
                {
                    var cell = fileSheet.Cell(row, 3);
                    IXLComment comment = cell.CreateComment();

                    foreach (string file in filesNameList)
                    {
                        for (int j = 2; j <= maxRowFile; j++)
                        {
                            if (fileSheet.Cell(j, 1).GetValue<string>().Equals(file))
                            {
                                string hyperlinkTXT = $"'{AppRes.Files}'!A{i}";
                                Hyperlink hyperlink = new Hyperlink { Reference = hyperlinkTXT };
                                comment.SetVisible();
                                comment.AddNewLine();
                                comment.AddText($"{file} {AppRes.Row}: {j}");
                                comment.Style.Alignment.SetAutomaticSize();
                                break;
                            }
                        }
                    }
                }
            }

        }

        private void SettHyperLinkToOrdersCustomers(int row, string nth, IXLWorksheet customerSheet, IXLWorksheet orderSheet)
        {
            int maxRow = Math.Min(customerSheet.RowCount(), 1048576);

            for (int i = 2; i <= maxRow; i++)
            {
                string cellValue = customerSheet.Cell(i, 5).GetValue<string>();
                if (cellValue.Equals(nth))
                {
                    string hyperlinkTXT = $"'{AppRes.Customers}'!A{i}";
                    orderSheet.Cell(row, 2).SetHyperlink(new XLHyperlink(hyperlinkTXT));
                }
            }
        }

        public void CreateZip()
        {
            if (!_isExporting) throw new InvalidOperationException("Export must be called before creating a zip file");
            string parentDirectory = Directory.GetParent(_outputPath)?.FullName;
            string destinationZipFilePath = Path.Combine(parentDirectory, Path.GetFileName(_outputPath) + ".zip");
            ZipFile.CreateFromDirectory(_outputPath, destinationZipFilePath);
        }
    }

    internal class ProgressState
    {
        public long TotalRunner { get; }
        private int _currentRunner;
        private Action<float> progressCallback;

        public ProgressState(long totalRunner, Action<float> progressCallback = null)
        {
            TotalRunner = totalRunner;
            _currentRunner = 0;
            this.progressCallback = progressCallback;
        }

        public void UpdateProgress()
        {
            if (progressCallback == null) return;
            _currentRunner++;
            float precent = _currentRunner / TotalRunner * 100;
            Debug.WriteLine($"Excel precent: {precent}");
            progressCallback?.Invoke((float)Math.Round(precent, 2));
        }
    }
}
