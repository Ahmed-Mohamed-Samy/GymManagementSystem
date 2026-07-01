using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementBLL.AttachmentService
{
    public interface IAttachmentService
    {
        Task<string?> UploadAsync(string FolderName , IFormFile File);

        bool Delete(string FileName, string FolderName);
    }
}
