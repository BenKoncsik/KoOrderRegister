using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using KoOrderRegister.Localization;
using KoOrderRegister.Modules.Database.Models;
using KoOrderRegister.Modules.Database.Services;
using KoOrderRegister.Modules.Export.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Gaming.Input.Custom;
using Windows.Storage;

namespace KoOrderRegister.Modules.Export.Excel
{
    public class ExcelExportService : IExportService
    {
        private readonly IDatabaseModel _databaseModel;

        public ExcelExportService(IDatabaseModel databaseModel)
        {
            _databaseModel = databaseModel;

        }
        public async Task Export(string outputPath, CancellationToken cancellationToken, Action<float> progressCallback = null)
        {
            long totalCustomers = await _databaseModel.CountCustomers() + await _databaseModel.CountOrders() + await _databaseModel.CountFiles();
            long currentCustomer = 0;
            using (var workbook = new XLWorkbook())
            {

                var customerSheet = workbook.Worksheets.Add(AppRes.Customers);
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

                    currentCustomer++;
                    UpdateProgress(currentCustomer, totalCustomers, progressCallback);
                }

                var orderSheet = workbook.Worksheets.Add(AppRes.Orders);
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
                    row++;

                    currentCustomer++;
                    UpdateProgress(currentCustomer, totalCustomers, progressCallback);
                }

                IXLWorksheet fileSheet = workbook.Worksheets.Add(AppRes.Files);
                fileSheet.Cell(1, 1).Value = AppRes.FileName;
                fileSheet.Cell(1, 2).Value = AppRes.OrderNumber;
                fileSheet.Cell(1, 3).Value = AppRes.FilePath;

                row = 2;
                await foreach (var file in _databaseModel.GetAllFilesAsStream(cancellationToken))
                {
                    var filePath = Path.Combine(Path.GetDirectoryName(outputPath), "Files", file.Name);
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                    
                    fileSheet.Cell(row, 1).Value = file.Name;
                    fileSheet.Cell(row, 2).Value = file.Order.OrderNumber;

                    if (file.Content != null)
                    {
                        File.WriteAllBytes(filePath, file.Content);
                        fileSheet.Cell(row, 3).Value = filePath;
                        fileSheet.Cell(row, 3).SetHyperlink(new XLHyperlink(filePath));
                    }
                    else
                    {
                        fileSheet.Cell(row, 3).Value = AppRes.FileError;
                    }

                    SettHyperLinkToFiles(row, file.Order.OrderNumber, fileSheet, orderSheet);
                    row++;

                    currentCustomer++;
                    UpdateProgress(currentCustomer, totalCustomers, progressCallback);
                }

                workbook.SaveAs(outputPath);
            }

        }

        private void SettHyperLinkToFiles(int row, string orderId, IXLWorksheet fileSheet, IXLWorksheet orderSheet)
        {
            int targetRow = -1;

            int maxRow = Math.Min(orderSheet.RowCount(), 1048576);
            
            for (int i = 2; i <= maxRow; i++)
            {
                string cellValue = orderSheet.Cell(i, 1).GetValue<string>();
                if (cellValue.Equals(orderId))
                {
                    targetRow = i;
                    fileSheet.Cell(row, 2).SetHyperlink(new XLHyperlink($"'{AppRes.Order}'!A{targetRow}"));
                }
            }
        }

        private void UpdateProgress(long currentCustomer, long totalCustomers, Action<float> progressCallback = null)
        {
            float precent = (currentCustomer / totalCustomers * 100);
            progressCallback?.Invoke((float)Math.Round(precent, 2));
        }
    }
}
