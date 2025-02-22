using CommunityToolkit.Maui.Storage;
using KoOrderRegister.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using KORCore.Modules.Database.Models;
using KORCore.Utility;

namespace KoOrderRegister.Modules.Order.Services
{

    public class FileService : IFileService
    {
        private const string CANCLE_FOLDER = "CANCLE_FOLDER";
        public async Task<bool> SaveFileToLocal(FileModel file)
        {
            if (file.Content == null)
            {
                return false;
            }
            try
            {
                CancellationToken cancellationToken = new CancellationToken();
                string folderPath = await PickFolderAsync(cancellationToken);
                if (folderPath.Equals(CANCLE_FOLDER))
                {
                    throw new FileSaveException(CANCLE_FOLDER);
                }
                string filePath = Path.Combine(folderPath, file.Name);
                await File.WriteAllBytesAsync(filePath, file.Content);
                return true;

            }
            catch (FileSaveException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"An error occurred while picking the folder: {ex.Message}");
                return false;
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
                if (!string.IsNullOrEmpty(file.HashCode) && file.HashCode.Equals(existingFileHash))
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
            return await ThreadManager.Run(() =>
            {
                using (var sha256 = SHA256.Create())
                {
                    byte[] hashBytes = sha256.ComputeHash(content);
                    return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
                }
            }, ThreadManager.Priority.High);
        }


        private async Task<string> PickFolderAsync(CancellationToken cancellationToken)
        {
            try
            {
                var folderResult = await FolderPicker.PickAsync(cancellationToken);

                if (folderResult != null)
                {
                    if (folderResult.Folder == null || folderResult.Folder.Path == null)
                    {
                        return CANCLE_FOLDER;
                    }
                    else
                    {
                        return folderResult.Folder.Path;
                    }

                }
                else
                {
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"An error occurred while picking the folder: {ex.Message}");
                return string.Empty;
            }
        }


    }
}
