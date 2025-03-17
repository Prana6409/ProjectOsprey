using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace MobileBackendTest1.Services
{
    public interface IFileStorageService
    {
        Task<string> UploadFileAsync(IFormFile file);
        Task<bool> DeleteFileAsync(string filePath);
        Task<string> UpdateFileAsync(string oldFilePath, IFormFile newFile); // Add this

        Task<string> UploadProfilePictureAsync(string userId, IFormFile file);
        Task<byte[]> GetProfilePictureAsync(string userId);
        Task<bool> DeleteProfilePictureAsync(string userId);
        Task<string> UpdateProfilePictureAsync(string userId, IFormFile newFile); // Add this
    }
}
