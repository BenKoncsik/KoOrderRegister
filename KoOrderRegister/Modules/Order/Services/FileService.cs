using KoOrderRegister.Modules.Database.Models;
using KoOrderRegister.Modules.Order.List.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace KoOrderRegister.Modules.Database.Services
{
 
    public class FileService : IFileService
    {
        public async Task<string> SaveFileToLocal(FileModel file)
        {
            var documentsPath = FileSystem.AppDataDirectory;
            var filePath = Path.Combine(documentsPath, file.Name);

            try
            {
                File.WriteAllBytes(filePath, file.Content);
                return filePath;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error saving file: {ex.Message}");
                return null;
            }
        }

        public async Task<string> SaveFileToTmp(FileModel file)
        {
            var tempPath = FileSystem.CacheDirectory;
            var filePath = Path.Combine(tempPath, file.Name);

            if (File.Exists(filePath))
            {
                var existingFileContent = await File.ReadAllBytesAsync(filePath);
                var existingFileHash = await CalculateHashAsync(existingFileContent);
                if(!string.IsNullOrEmpty(file.HashCode) && file.HashCode.Equals(existingFileHash))
                {
                    return filePath;
                }
            }
            try
            {
                File.WriteAllBytes(filePath, file.Content);
                return filePath;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error saving file: {ex.Message}");
                return null;
            }
        }


        public async Task<string> CalculateHashAsync(byte[] content)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] hashBytes = await Task.Run(() => sha256.ComputeHash(content));
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
            }
        }
    }
}
