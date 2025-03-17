using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;
using MobileBackendTest1.Models;
using System.Text.RegularExpressions;

namespace MobileBackendTest1.Services
{
    public class EntertainerService
    {
        private readonly CounterService _counterService;
        private readonly IMongoCollection<Entertainer> _entertainers;
        private readonly IMongoCollection<User> _usersCollection;
        private readonly IFileStorageService _fileStorageService;
        private readonly IMongoCollection<Sportsman> _sportsmanCollection;
        private readonly IMongoCollection<BusinessOwner> _businessOwnerCollection;

        public EntertainerService(IMongoDatabase database, CounterService counterService, IFileStorageService fileStorageService)
        {
            _counterService = counterService;
            _fileStorageService = fileStorageService;
            _entertainers = database.GetCollection<Entertainer>("Entertainer");
            _usersCollection = database.GetCollection<User>("Users");
            _sportsmanCollection = database.GetCollection<Sportsman>("Sportsman");
            _businessOwnerCollection = database.GetCollection<BusinessOwner>("BusinessOwner");
        }

        // Get all entertainers
        public async Task<List<Entertainer>> GetEntertainersAsync()
        {
            return await _entertainers.Find(_ => true).ToListAsync();
        }
        //Get user by ID
        public async Task<Entertainer> GetUserByUserID(string userid)
        {
            var filter = Builders<Entertainer>.Filter.Eq(u => u.UqID, userid);
            return await _entertainers.Find(filter).FirstOrDefaultAsync();
        }

        // Get an entertainer by username
        public async Task<Entertainer> GetEntertainerByUsernameAsync(string username)
        {
            return await _entertainers.Find(e => e.Username == username).FirstOrDefaultAsync();
        }

        // Get an entertainer by email
        public async Task<Entertainer> GetEntertainerByEmailAsync(string email)
        {
            return await _entertainers.Find(e => e.email == email).FirstOrDefaultAsync();
        }

        // Check if username exists in Users, Sportsman, or BusinessOwner collections
        public async Task<bool> IsUsernameExistsInOtherCollectionsAsync(string username)
        {
            var userTask = _usersCollection.Find(u => u.Username == username).FirstOrDefaultAsync();
            var sportsmanTask = _sportsmanCollection.Find(s => s.Username == username).FirstOrDefaultAsync();
            var businessOwnerTask = _businessOwnerCollection.Find(b => b.Username == username).FirstOrDefaultAsync();

            await Task.WhenAll(userTask, sportsmanTask, businessOwnerTask);

            return userTask.Result != null || sportsmanTask.Result != null || businessOwnerTask.Result != null;
        }

        // Check if email exists in Users, Sportsman, or BusinessOwner collections
        public async Task<bool> IsEmailExistsInOtherCollectionsAsync(string email)
        {
            var userTask = _usersCollection.Find(u => u.email == email).FirstOrDefaultAsync();
            var sportsmanTask = _sportsmanCollection.Find(s => s.email == email).FirstOrDefaultAsync();
            var businessOwnerTask = _businessOwnerCollection.Find(b => b.email == email).FirstOrDefaultAsync();

            await Task.WhenAll(userTask, sportsmanTask, businessOwnerTask);

            return userTask.Result != null || sportsmanTask.Result != null || businessOwnerTask.Result != null;
        }

        // Create a new entertainer
        public async Task CreateEntertainerAsync(Entertainer entertainer)
        {

            //Validate the plaintext password before hashing
            if (!IsPasswordValid(entertainer.password))
            {
                throw new ArgumentException("Password does not meet the required format.");
            }

            // Check if the username exists in other collections
            var usernameExists = await IsUsernameExistsInOtherCollectionsAsync(entertainer.Username);
            if (usernameExists)
            {
                throw new InvalidOperationException("Username is already taken in another collection.");
            }

            // Check if the email exists in other collections
            var emailExists = await IsEmailExistsInOtherCollectionsAsync(entertainer.email);
            if (emailExists)
            {
                throw new InvalidOperationException("Email is already registered in another collection.");
            }
            int newUserId = await _counterService.GetNextIdAsync("EntertainerId");
            entertainer.EntertainerId = newUserId;
            // Hash the password before saving
            entertainer.password = HashPassword(entertainer.password);

            // Insert the entertainer into the database
            await _entertainers.InsertOneAsync(entertainer);
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

        // Update entertainer data
        public async Task UpdateUserAsync(string id, Entertainer updatedUser, IFormFile profilePictureFile)
        {
            if (updatedUser == null)
                throw new ArgumentNullException(nameof(updatedUser));

            // Hash the password if it's being updated
            if (!string.IsNullOrEmpty(updatedUser.password))
            {
                updatedUser.password = HashPassword(updatedUser.password);
            }
            // Update profile picture if a new file is provided
            if (profilePictureFile != null && profilePictureFile.Length > 0)
            {
                var profilePictureUrl = await _fileStorageService.UploadProfilePictureAsync(id.ToString(), profilePictureFile);
                updatedUser.ProfilePictureUrl = profilePictureUrl;
            }

            var filter = Builders<Entertainer>.Filter.Eq(u => u.Id, id);
            var update = Builders<Entertainer>.Update
                .Set(u => u.Username, updatedUser.Username)
                .Set(u => u.email, updatedUser.email)
                .Set(u => u.Address, updatedUser.Address)
                .Set(u => u.stagename, updatedUser.stagename)
                .Set(u => u.Country, updatedUser.Country)
                .Set(u => u.ContactNumber, updatedUser.ContactNumber)
                .Set(u => u.Talent, updatedUser.Talent)
                .Set(u => u.DateOfBirth, updatedUser.DateOfBirth)
                .Set(u => u.password, updatedUser.password)
                .Set(u => u.ProfilePictureUrl, updatedUser.ProfilePictureUrl);

            var result = await _entertainers.UpdateOneAsync(filter, update);

            if (result.MatchedCount == 0)
                throw new InvalidOperationException("User not found.");
        }
        // Upload or Update Profile Picture
        public async Task<string> UploadOrUpdateProfilePictureAsync(string userId, IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("Invalid file.");

            // Upload the profile picture using IFileStorageService
            var profilePicturePath = await _fileStorageService.UploadProfilePictureAsync(userId, file);

            // Update the user's ProfilePicturePath in the database
            var filter = Builders<Entertainer>.Filter.Eq(u => u.UqID, userId);
            var update = Builders<Entertainer>.Update.Set(u => u.ProfilePictureUrl, profilePicturePath);

            var result = await _entertainers.UpdateOneAsync(filter, update);

            if (result.MatchedCount == 0)
                throw new InvalidOperationException("User not found.");

            return profilePicturePath;
        }

        // Get Profile Picture
        public async Task<byte[]> GetProfilePictureAsync(string userId)
        {
            // Retrieve the profile picture path from the user document
            var filter = Builders<Entertainer>.Filter.Eq(u => u.UqID, userId);
            var user = await _entertainers.Find(filter).FirstOrDefaultAsync();

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
                var filter = Builders<Entertainer>.Filter.Eq(u => u.UqID, userId);
                var update = Builders<Entertainer>.Update.Set(u => u.ProfilePictureUrl, null);

                var result = await _entertainers.UpdateOneAsync(filter, update);

                if (result.MatchedCount == 0)
                    throw new InvalidOperationException("User not found.");
            }

            return isDeleted;

        }
    }
}