using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;
using MobileBackendTest1.Models;
using Microsoft.AspNetCore.Http;
using System.Text.RegularExpressions;

namespace MobileBackendTest1.Services
{
    public class BusinessOwnerService
    {
        private readonly CounterService _counterService;
        private readonly IFileStorageService _fileStorageService;
        private readonly IMongoCollection<BusinessOwner> _businessOwners;
        private readonly IMongoCollection<User> _usersCollection;
        private readonly IMongoCollection<Sportsman> _sportsmanCollection;
        private readonly IMongoCollection<Entertainer> _entertainerCollection;

        public BusinessOwnerService(IMongoDatabase database, CounterService counterService, IFileStorageService fileStorageService)
        {
            _counterService = counterService;
            _fileStorageService = fileStorageService;
            _businessOwners = database.GetCollection<BusinessOwner>("BusinessOwner");
            _usersCollection = database.GetCollection<User>("Users");
            _sportsmanCollection = database.GetCollection<Sportsman>("Sportsman");
            _entertainerCollection = database.GetCollection<Entertainer>("Entertainer");
        }

        // Get all business owners
        public async Task<List<BusinessOwner>> GetBusinessOwnersAsync()
        {
            return await _businessOwners.Find(_ => true).ToListAsync();
        }
        //Get user by ID
        public async Task<BusinessOwner> GetUserByUserID(string userid)
        {
            var filter = Builders<BusinessOwner>.Filter.Eq(u => u.UqID, userid);
            return await _businessOwners.Find(filter).FirstOrDefaultAsync();
        }
        public async Task<BusinessOwner> GetBusinessOwnerByIdAsync(int BusinessOwnerID)
        {
            var filter = Builders<BusinessOwner>.Filter.Eq(b => b.BusinessOwnerID, BusinessOwnerID);
            return await _businessOwners.Find(filter).FirstOrDefaultAsync();
        }

        // Get a business owner by username
        public async Task<BusinessOwner> GetBusinessOwnerByUsernameAsync(string username)
        {
            return await _businessOwners.Find(u => u.Username == username).FirstOrDefaultAsync();
        }

        // Get a business owner by email
        public async Task<BusinessOwner> GetBusinessOwnerByEmailAsync(string email)
        {
            return await _businessOwners.Find(u => u.email == email).FirstOrDefaultAsync();
        }

        // Check if username exists in Users, Sportsman, or Entertainer collections
        public async Task<bool> IsUsernameExistsInOtherCollectionsAsync(string username)
        {
            var userTask = _usersCollection.Find(u => u.Username == username).FirstOrDefaultAsync();
            var sportsmanTask = _sportsmanCollection.Find(s => s.Username == username).FirstOrDefaultAsync();
            var entertainerTask = _entertainerCollection.Find(e => e.Username == username).FirstOrDefaultAsync();

            await Task.WhenAll(userTask, sportsmanTask, entertainerTask);

            return userTask.Result != null || sportsmanTask.Result != null || entertainerTask.Result != null;
        }

        // Check if email exists in Users, Sportsman, or Entertainer collections
        public async Task<bool> IsEmailExistsInOtherCollectionsAsync(string email)
        {
            var userTask = _usersCollection.Find(u => u.email == email).FirstOrDefaultAsync();
            var sportsmanTask = _sportsmanCollection.Find(s => s.email == email).FirstOrDefaultAsync();
            var entertainerTask = _entertainerCollection.Find(e => e.email == email).FirstOrDefaultAsync();

            await Task.WhenAll(userTask, sportsmanTask, entertainerTask);

            return userTask.Result != null || sportsmanTask.Result != null || entertainerTask.Result != null;
        }

        // Create a new business owner
        public async Task CreateBusinessOwnerAsync(BusinessOwner businessOwner)
        {

            //Validate the plaintext password before hashing
            if (!IsPasswordValid(businessOwner.password))
            {
                throw new ArgumentException("Password does not meet the required format.");
            }
            // Check if the username exists in other collections
            var usernameExists = await IsUsernameExistsInOtherCollectionsAsync(businessOwner.Username);
            if (usernameExists)
            {
                throw new InvalidOperationException("Username is already taken in another collection.");
            }

            // Check if the email exists in other collections
            var emailExists = await IsEmailExistsInOtherCollectionsAsync(businessOwner.email);
            if (emailExists)
            {
                throw new InvalidOperationException("Email is already registered in another collection.");
            }

            // Generate a new ID for the business owner
            int newUserId = await _counterService.GetNextIdAsync("BusinessOwnerID");
            businessOwner.BusinessOwnerID = newUserId;

            // Hash the password before saving
            businessOwner.password = HashPassword(businessOwner.password);

            // Insert the business owner into the database
            await _businessOwners.InsertOneAsync(businessOwner);
        }

        // Validate password against regex rules
        private bool IsPasswordValid(string password)
        {
            // Regex pattern for password validation
            var passwordRegex = new Regex(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$");
            return passwordRegex.IsMatch(password);
        }

        // Hash password using SHA256
        public string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                var builder = new StringBuilder();
                foreach (var b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }

        // Update business owner data
        public async Task UpdateUserAsync(string id, BusinessOwner updatedUser, IFormFile profilePictureFile)
        {
            if (updatedUser == null)
                throw new ArgumentNullException(nameof(updatedUser));

            // Hash the password if it's being updated
            if (!string.IsNullOrEmpty(updatedUser.password))
            {
                updatedUser.password = HashPassword(updatedUser.password);
            }
             // Update profile picture if a new file is provided
            if (profilePictureFile!= null && profilePictureFile.Length > 0)
            {
                var profilePictureUrl = await _fileStorageService.UploadProfilePictureAsync(id.ToString(), profilePictureFile);
                updatedUser.ProfilePictureUrl = profilePictureUrl;
            }
            var filter = Builders<BusinessOwner>.Filter.Eq(u => u.UqID, id);
            var update = Builders<BusinessOwner>.Update
                .Set(u => u.Username, updatedUser.Username)
                .Set(u => u.email, updatedUser.email)
                .Set(u => u.Address, updatedUser.Address)
                .Set(u => u.CompanyName, updatedUser.CompanyName)
                .Set(u => u.HeadCountry, updatedUser.HeadCountry)
                .Set(u => u.ContactNumber, updatedUser.ContactNumber)
                .Set(u => u.website, updatedUser.website)
                .Set(u => u.password, updatedUser.password)
                .Set(u => u.ProfilePictureUrl, updatedUser.ProfilePictureUrl);

            var result = await _businessOwners.UpdateOneAsync(filter, update);

            if (result.MatchedCount == 0)
                throw new InvalidOperationException("User not found.");
        }
        // Upload or Update Profile Picture
        public async Task<string> UploadOrUpdateProfilePictureAsync(string userId, IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("Invalid file.");

            // Upload the profile picture using IFileStorageService
            var profilePicturePath = await _fileStorageService.UploadProfilePictureAsync(userId.ToString(), file);

            // Update the user's ProfilePicturePath in the database
            var filter = Builders<BusinessOwner>.Filter.Eq(u => u.UqID, userId);
            var update = Builders<BusinessOwner>.Update.Set(u => u.ProfilePictureUrl, profilePicturePath);

            var result = await _businessOwners.UpdateOneAsync(filter, update);

            if (result.MatchedCount == 0)
                throw new InvalidOperationException("User not found.");

            return profilePicturePath;
        }

        // Get Profile Picture
        public async Task<byte[]> GetProfilePictureAsync(string userId)
        {
            // Retrieve the profile picture path from the user document
            var filter = Builders<BusinessOwner>.Filter.Eq(u => u.UqID, userId);
            var user = await _businessOwners.Find(filter).FirstOrDefaultAsync();

            if (user == null || string.IsNullOrEmpty(user.ProfilePictureUrl))
            {
                throw new InvalidOperationException("User or profile picture not found.");
            }

            // Retrieve the profile picture file using IFileStorageService
            return await _fileStorageService.GetProfilePictureAsync(userId.ToString());
        }

        // Delete Profile Picture
        public async Task<bool> DeleteProfilePictureAsync(string userId)
        {
            // Delete the profile picture file using IFileStorageService
            var isDeleted = await _fileStorageService.DeleteProfilePictureAsync(userId.ToString());

            if (isDeleted)
            {
                // Clear the ProfilePicturePath in the user document
                var filter = Builders<BusinessOwner>.Filter.Eq(u => u.UqID, userId);
                var update = Builders<BusinessOwner>.Update.Set(u => u.UqID, null);

                var result = await _businessOwners.UpdateOneAsync(filter, update);

                if (result.MatchedCount == 0)
                    throw new InvalidOperationException("User not found.");
            }

            return isDeleted;

        }
    }
}