using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementBLL.AttachmentService
{
    public class AttachmentService : IAttachmentService
    {

        private readonly string[] Extensions = { ".jpg", ".jpeg", ".png" };

        private readonly long MaxFileSize = 5 * 1024 * 1024; // 5MB
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AttachmentService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<string?> UploadAsync(string FolderName, IFormFile File)
        {
            try
            {
                if (FolderName is null || File is null || File.Length == 0) return null;

                if (File.Length > MaxFileSize) return null;

                var FileExtension = Path.GetExtension(File.FileName).ToLower();

                if (!Extensions.Contains(FileExtension)) return null;

                var FolderPath = Path.Combine(_webHostEnvironment.WebRootPath, "images", FolderName);

                if (!Directory.Exists(FolderPath))
                    Directory.CreateDirectory(FolderPath);

                var FileName = Guid.NewGuid().ToString() + FileExtension;

                var FilePath = Path.Combine(FolderPath, FileName);

                using var FileStream = new FileStream(FilePath, FileMode.Create);

                await File.CopyToAsync(FileStream);

                return FileName;
            }
            catch(Exception ex) 
            {
                Console.WriteLine($"Failed To Upload File To Folder = {FolderName} : {ex}");
                return null;
            }
            


        }

        public bool Delete(string FileName, string FolderName)
        {
            try
            {
                if(FileName is null || FolderName is null) return false;

                var FilePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", FolderName, FileName);

                if(Path.Exists(FilePath))
                { 
                    File.Delete(FilePath);
                    return true;
                }

                return false;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Failed To Delete File with Name = {FileName} : {ex}");
                return false;
            }
        }
    }
}
