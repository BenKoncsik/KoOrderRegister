﻿using Microsoft.Win32;
using KoOrderRegister.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Infrastructure;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using KoOrderRegister.I18N;

namespace KoOrderRegister.Services
{

    public class FileService
    {
        private static OrderService _orderService = new OrderService();

        public static bool deleteFile(FileBLOB file)
        {
            try
            {
                // Töröljük a kapcsolatot a 'product_files' táblában
                using (var deleteLinkCommand = new SQLiteCommand("DELETE FROM product_files WHERE fileId = @FileId", OrderService.OpenConnection()))
                {
                    deleteLinkCommand.Parameters.AddWithValue("@FileId", file.ID);
                    deleteLinkCommand.ExecuteNonQuery();
                }

                // Töröljük a fájlt a 'files' táblából
                using (var deleteFileCommand = new SQLiteCommand("DELETE FROM files WHERE fileId = @FileId", OrderService.OpenConnection()))
                {
                    deleteFileCommand.Parameters.AddWithValue("@FileId", file.ID);
                    deleteFileCommand.ExecuteNonQuery();
                }

                return true;
            }
            catch (Exception ex)
            {
                // Kezelje le a kivételt, például naplózás vagy hibaüzenet megjelenítése
                Console.WriteLine($"{Resources.ErrorFileDelete} {Resources.ErrorMsgLabel} {ex.Message}");
                return false;
            }
        }

        public static bool deleteFilesWithOrder(long productID)
        {
            if (deleteProduct_files(productID))
            {
                return deleteFiles(productID);
            }
            return false;
        }
        private static bool deleteProduct_files(long productID)
        {
            string sql = $"DELETE FROM product_files WHERE orderId = @ProductId";

            using (var command = new SQLiteCommand(sql, OrderService.OpenConnection()))
            {
                command.Parameters.AddWithValue("@id", productID);
                command.ExecuteNonQuery();
                return true;
            }
        }
        private static bool deleteFiles(long productID)
        {
            string sql = $"DELETE FROM product_files WHERE orderId = @ProductId";

            using (var command = new SQLiteCommand(sql, OrderService.OpenConnection()))
            {
                command.Parameters.AddWithValue("@id", productID);
                command.ExecuteNonQuery();
                return true;
            }
        }

        //kellene egz olyam metodus ami a FIleBLOB ot vár és frissíti a name értékét true ha sikerü akkor false
        public static bool renameFile(FileBLOB file)
        {
            try
            {
                using (SQLiteConnection connection = OrderService.OpenConnection())
                {
                    using (SQLiteCommand command = new SQLiteCommand(connection))
                    {
                        command.CommandText = "UPDATE files SET fileName = @newName WHERE fileId = @id";
                        command.Parameters.AddWithValue("@newName", file.Name);
                        command.Parameters.AddWithValue("@id", file.ID);

                        int rowsUpdated = command.ExecuteNonQuery();

                        return rowsUpdated > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{Resources.ErrorFileRename} {Resources.ErrorMsgLabel} {ex.Message}");
                return false;
            }
        }

        public static bool save(FileBLOB file)
        {
            try
            {
                long fileId;
                using (var connection = OrderService.OpenConnection())
                {
                    using (var command = new SQLiteCommand(connection))
                    {
                        command.CommandText = @"INSERT INTO files (fileName, fileBlob) VALUES (@FileName, @FileBlob)";
                        command.Parameters.AddWithValue("@FileName", file.Name);
                        command.Parameters.AddWithValue("@FileBlob", file.Data);

                        command.ExecuteNonQuery();

                        // Get the ID of the inserted file
                        fileId = (long)new SQLiteCommand("SELECT last_insert_rowid()", connection).ExecuteScalar();
                    }

                    using (var transaction = connection.BeginTransaction())
                    {
                        // Now, we will insert the relation into the product_files table
                        using (var command = new SQLiteCommand(connection))
                        {
                            command.CommandText = @"INSERT INTO product_files (productId, fileId) VALUES (@OrderId, @FileId)";
                            command.Parameters.AddWithValue("@OrderId", file.ProductID);
                            command.Parameters.AddWithValue("@FileId", fileId);

                            command.ExecuteNonQuery();
                        }
                        transaction.Commit();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{Resources.ErrorSavingFile}   {Resources.ErrorMsgLabel} {ex.Message}");
                return false;
            }
        }

        public static FileBLOB getFileBLOB(string fileName, string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"{Resources.NotFoundFile}", filePath);
            }

            return new FileBLOB(fileName, File.ReadAllBytes(filePath), new BitmapImage(new Uri(filePath)));
        }

        public static List<FileBLOB> getDataBaseFileBlobProductID(long productID)
        {
            List<FileBLOB> fileBlobs = new List<FileBLOB>();
                string query = @"
                            SELECT f.fileId, f.fileName, f.fileBlob 
                            FROM files f 
                            INNER JOIN product_files pf ON f.fileId = pf.fileId 
                            WHERE pf.productId = @ProductID";

                using (SQLiteCommand command = new SQLiteCommand(query, OrderService.OpenConnection()))
                {
                    command.Parameters.AddWithValue("@ProductID", productID);

                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            FileBLOB fileBlob = new FileBLOB
                            {
                                ID = (int)reader.GetInt64(0),
                                Name = reader.GetString(1),
                                Data = (byte[])reader[2],
                            };
                            fileBlobs.Add(fileBlob);
                        }
                    }
                return fileBlobs;
            }
        }


        public static void OpenFileFromDatabase(FileBLOB fileBLOB)
        {
            string fileName = fileBLOB.Name;
            // Ideiglenes fájl létrehozása a rendszeren
            string tempFilePath = Path.Combine(Path.GetTempPath(), fileName);
            File.WriteAllBytes(tempFilePath, fileBLOB.Data);

            // Megnyitja a fájlt az alapértelmezett alkalmazással
            System.Diagnostics.Process.Start(tempFilePath);
        }
       public static ObservableCollection<FileBLOB> getFilesObservable(long producId)
        {
            ObservableCollection<FileBLOB> images = new ObservableCollection<FileBLOB>();
            foreach (FileBLOB file in getDataBaseFileBlobProductID(producId))
            {
                using(MemoryStream ms = new MemoryStream(file.Data))
                {
                    BitmapImage image = new BitmapImage();
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.StreamSource = ms;
                    image.EndInit();
                    file.BitmapImage = image;
                }
                images.Add(file);
            }
            return images;
        }
    }
}
