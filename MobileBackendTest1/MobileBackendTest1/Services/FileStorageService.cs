using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace MobileBackendTest1.Services
{
    public class FileStorageService : IFileStorageService
    {
        private readonly string _storagePath; // For offer letters
        private readonly string _profilePicturePath; // For profile pictures
        private readonly ILogger<FileStorageService> _logger;

        public FileStorageService(IConfiguration configuration, ILogger<FileStorageService> logger)
        {
            _storagePath = configuration["FileStorage:Path"] ?? throw new ArgumentNullException(nameof(_storagePath));
            _profilePicturePath = configuration["FileStorage:ProfilePicturePath"] ?? throw new ArgumentNullException(nameof(_profilePicturePath));
            _logger = logger;

            // Ensure storage directories exist
            if (!Directory.Exists(_storagePath))
            {
                Directory.CreateDirectory(_storagePath);
            }
            if (!Directory.Exists(_profilePicturePath))
            {
                Directory.CreateDirectory(_profilePicturePath);
            }
        }

        // Upload offer letter
        public async Task<string> UploadFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("Invalid file.");

            try
            {
                string fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                string fullPath = Path.Combine(_storagePath, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                _logger.LogInformation($"File uploaded successfully: {fullPath}");

                // Return relative path for frontend use
                return $"/uploads/{fileName}";
            }
            catch (Exception ex)
            {
                _logger.LogError($"File upload failed: {ex.Message}");
                throw;
            }
        }
        public async Task<string> UpdateFileAsync(string oldFilePath, IFormFile newFile)
        {
            if (newFile == null || newFile.Length == 0)
                throw new ArgumentException("Invalid file.");

            const long maxFileSize = 10 * 1024 * 1024; // 10 MB
            if (newFile.Length > maxFileSize)
            {
                throw new ArgumentException("File size exceeds the maximum allowed limit of 10 MB.");
            }

            try
            {
                // Delete the old file if it exists
                if (!string.IsNullOrEmpty(oldFilePath))
                {
                    await DeleteFileAsync(oldFilePath);
                }

                // Upload the new file
                return await UploadFileAsync(newFile);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating file: {ex.Message}");
                throw;
            }
        }

        // Delete offer letter
        public async Task<bool> DeleteFileAsync(string filePath)
        {
            try
            {
                string fullPath = Path.Combine(_storagePath, Path.GetFileName(filePath));

                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                    _logger.LogInformation($"File deleted: {filePath}");
                    return true;
                }

                _logger.LogWarning($"File not found: {filePath}");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting file: {ex.Message}");
                return false;
            }
        }

        public async Task<string> UploadProfilePictureAsync(string userId, IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("Invalid file.");

            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (fileExtension != ".jpg" && fileExtension != ".jpeg" && fileExtension != ".png")
            {
                throw new ArgumentException("Only JPG, JPEG, and PNG files are allowed.");
            }

            try
            {
                var sanitizedUserId = Path.GetFileName(userId); // Prevent directory traversal
                var filePath = Path.Combine(_profilePicturePath, $"{sanitizedUserId}{fileExtension}");

                using var stream = new FileStream(filePath, FileMode.Create);
                await file.CopyToAsync(stream);

                _logger.LogInformation($"Profile picture uploaded successfully: {filePath}");
                return $"/profile-pictures/{sanitizedUserId}{fileExtension}";
            }
            catch (Exception ex)
            {
                _logger.LogError($"Profile picture upload failed: {ex.Message}");
                throw;
            }
        }
        public async Task<string> UpdateProfilePictureAsync(string userId, IFormFile newFile)
        {
            if (newFile == null || newFile.Length == 0)
                throw new ArgumentException("Invalid file.");

            var fileExtension = Path.GetExtension(newFile.FileName).ToLowerInvariant();
            if (fileExtension != ".jpg" && fileExtension != ".jpeg" && fileExtension != ".png")
            {
                throw new ArgumentException("Only JPG, JPEG, and PNG files are allowed.");
            }

            try
            {
                // Delete the old profile picture if it exists
                await DeleteProfilePictureAsync(userId);

                // Upload the new profile picture
                return await UploadProfilePictureAsync(userId, newFile);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating profile picture: {ex.Message}");
                throw;
            }
        }
        public async Task<byte[]> GetProfilePictureAsync(string userId)
        {
            try
            {
                var sanitizedUserId = Path.GetFileName(userId);
                var filePath = Path.Combine(_profilePicturePath, $"{sanitizedUserId}.jpg");

                if (!File.Exists(filePath))
                {
                    _logger.LogWarning($"Profile picture not found: {filePath}");
                    return null;
                }

                return await File.ReadAllBytesAsync(filePath);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving profile picture: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> DeleteProfilePictureAsync(string userId)
        {
            try
            {
                var sanitizedUserId = Path.GetFileName(userId);
                var filePath = Path.Combine(_profilePicturePath, $"{sanitizedUserId}.jpg");

                if (File.Exists(filePath))
                {
                    await Task.Run(() => File.Delete(filePath));
                    _logger.LogInformation($"Profile picture deleted: {filePath}");
                    return true;
                }

                _logger.LogWarning($"Profile picture not found: {filePath}");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting profile picture: {ex.Message}");
                return false;
            }
        }
    }
}
