using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MsExcel = Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;
using KoOrderRegister.Modules.Database.Services;
using KoOrderRegister.Localization;
using KoOrderRegister.Modules.Export.Services;

namespace KoOrderRegister.Modules.Export.Excel.Services
{
    public class MSExcelExportService : IExcelExportService
    {
        private readonly IDatabaseModel _databaseModel;
        private ProgressState progressState;

        public MSExcelExportService(IDatabaseModel databaseModel)
        {
            _databaseModel = databaseModel;
        }

        public void CreateZip()
        {
            throw new NotImplementedException();
        }

        public async Task Export(string outputPath, CancellationToken cancellationToken, Action<float> progressCallback = null)
        {
            long totalCustomers = await _databaseModel.CountCustomers() * 2 +
                                  await _databaseModel.CountOrders() * 4 +
                                  await _databaseModel.CountFiles() * 3;
            progressState = new ProgressState(totalCustomers, progressCallback);
            string filePath = Path.Combine(Path.GetDirectoryName(outputPath), $"Files_{DateTime.Now:yyyy_MM_dd_HH_mm_ss}");

            MsExcel.Application excelApp = new MsExcel.Application();
            excelApp.Visible = false;

            try
            {
                MsExcel.Workbooks workbooks = excelApp.Workbooks;
                MsExcel.Workbook workbook = workbooks.Add(Type.Missing);
                MsExcel.Sheets sheets = workbook.Sheets;

                MsExcel.Worksheet customerSheet = (MsExcel.Worksheet)sheets.Add(Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                customerSheet.Name = AppRes.Customers;

                MsExcel.Worksheet orderSheet = (MsExcel.Worksheet)sheets.Add(Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                orderSheet.Name = AppRes.Orders;

                MsExcel.Worksheet fileSheet = (MsExcel.Worksheet)sheets.Add(Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                fileSheet.Name = AppRes.Files;


                customerSheet.Cells[1, 1] = AppRes.Name;
                customerSheet.Cells[1, 2] = AppRes.Address;
                customerSheet.Cells[1, 3] = AppRes.Phone;
                customerSheet.Cells[1, 4] = AppRes.Email;
                customerSheet.Cells[1, 5] = AppRes.NHI;
                customerSheet.Cells[1, 6] = AppRes.Note;

                int row = 2;
                await foreach (var customer in _databaseModel.GetAllCustomersAsStream(cancellationToken))
                {
                    customerSheet.Cells[row, 1] = customer.Name;
                    customerSheet.Cells[row, 2] = customer.Address;
                    customerSheet.Cells[row, 3] = customer.Phone;
                    customerSheet.Cells[row, 4] = customer.Email;
                    customerSheet.Cells[row, 5] = customer.NationalHealthInsurance;
                    customerSheet.Cells[row, 6] = customer.Note;

                    row++;
                    progressState.UpdateProgress();
                }


                orderSheet.Cells[1, 1] = AppRes.OrderNumber;
                orderSheet.Cells[1, 2] = AppRes.NHI;
                orderSheet.Cells[1, 3] = AppRes.Name;
                orderSheet.Cells[1, 4] = AppRes.Price;
                orderSheet.Cells[1, 5] = AppRes.StartDate;
                orderSheet.Cells[1, 6] = AppRes.EndDate;
                orderSheet.Cells[1, 7] = AppRes.Note;
                orderSheet.Cells[1, 8] = AppRes.FilesName;

                row = 2;
                await foreach (var order in _databaseModel.GetAllOrdersAsStream(cancellationToken))
                {
                    orderSheet.Cells[row, 1] = order.OrderNumber;
                    orderSheet.Cells[row, 2] = order.Customer.NationalHealthInsurance;
                    orderSheet.Cells[row, 3] = order.Customer.Name;
                    orderSheet.Cells[row, 4] = order.Price;
                    orderSheet.Cells[row, 5] = order.StartDate.ToString();
                    orderSheet.Cells[row, 6] = order.EndDate.ToString();
                    orderSheet.Cells[row, 7] = order.Note;

                    string filesName = string.Join("; ", order.Files.Select(f => f.Name));
                    orderSheet.Cells[row, 8] = filesName;

                    SetHyperLinkToOrdersCustomers(row, order.Customer.NationalHealthInsurance, customerSheet, orderSheet);

                    row++;
                    progressState.UpdateProgress();
                }

                fileSheet.Cells[1, 1] = AppRes.FileName;
                fileSheet.Cells[1, 2] = AppRes.OrderNumber;
                fileSheet.Cells[1, 3] = AppRes.FilePath;

                row = 2;
                await foreach (var file in _databaseModel.GetAllFilesAsStream(cancellationToken))
                {
                    fileSheet.Cells[row, 1] = file.Name;
                    fileSheet.Cells[row, 2] = file.Order.OrderNumber;

                    if (file.Content != null)
                    {
                        var newFile = Path.Combine(filePath, file.Name);
                        Directory.CreateDirectory(Path.GetDirectoryName(newFile));
                        File.WriteAllBytes(newFile, file.Content);
                        fileSheet.Cells[row, 3] = newFile;

                        MsExcel.Range cell = (MsExcel.Range)fileSheet.Cells[row, 3];
                        cell.Hyperlinks.Add(cell, newFile, Type.Missing, Type.Missing, Type.Missing);
                    }
                    else
                    {
                        fileSheet.Cells[row, 3] = "FileError";
                    }

                    SetHyperLinkToFiles(row, file.Order.OrderNumber, fileSheet, orderSheet);

                    row++;
                    progressState.UpdateProgress();
                }

                // Mentés és bezárás
                workbook.SaveAs(outputPath);
                workbook.Close(true, Type.Missing, Type.Missing);

                Marshal.ReleaseComObject(customerSheet);
                Marshal.ReleaseComObject(orderSheet);
                Marshal.ReleaseComObject(fileSheet);
                Marshal.ReleaseComObject(sheets);
                Marshal.ReleaseComObject(workbook);
                Marshal.ReleaseComObject(workbooks);
            }
            catch (Exception ex)
            {

                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                excelApp.Quit();
                Marshal.ReleaseComObject(excelApp);
            }
        }

        private void SetHyperLinkToFiles(int row, string orderId, MsExcel.Worksheet fileSheet, MsExcel.Worksheet orderSheet)
        {
            int maxRow = orderSheet.UsedRange.Rows.Count;

            for (int i = 2; i <= maxRow; i++)
            {
                string cellValue = Convert.ToString(orderSheet.Cells[i, 1].ToString());
                if (cellValue == orderId)
                {
                    string hyperlinkAddress = $"'{orderSheet.Name}'!A{i}";
                    MsExcel.Range cell = (MsExcel.Range)fileSheet.Cells[row, 2];
                    cell.Hyperlinks.Add(cell, "", hyperlinkAddress, Type.Missing, Type.Missing);
                    break;
                }
            }
        }

        private void SetHyperLinkToOrdersCustomers(int row, string nhi, MsExcel.Worksheet customerSheet, MsExcel.Worksheet orderSheet)
        {
            int maxRow = customerSheet.UsedRange.Rows.Count;

            for (int i = 2; i <= maxRow; i++)
            {
                string cellValue = Convert.ToString(customerSheet.Cells[i, 5].ToString());
                if (cellValue == nhi)
                {
                    string hyperlinkAddress = $"'{customerSheet.Name}'!A{i}";
                    MsExcel.Range cell = (MsExcel.Range)orderSheet.Cells[row, 2];
                    cell.Hyperlinks.Add(cell, "", hyperlinkAddress, Type.Missing, Type.Missing);
                    break;
                }
            }
        }
    }
}

