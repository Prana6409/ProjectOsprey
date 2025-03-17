using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;
using MobileBackendTest1.Models;
using System.Text.RegularExpressions;

namespace MobileBackendTest1.Services
{
    public class SportsmanService
    {
        private readonly CounterService _counterService;
        private readonly IMongoCollection<Sportsman> _sportsmen;
        private readonly IFileStorageService _fileStorageService;
        private readonly IMongoCollection<User> _usersCollection;
        private readonly IMongoCollection<BusinessOwner> _businessOwnerCollection;
        private readonly IMongoCollection<Entertainer> _entertainerCollection;

        public SportsmanService(IMongoDatabase database, CounterService counterService, IFileStorageService fileStorageService)
        {
            _counterService = counterService;
            _sportsmen = database.GetCollection<Sportsman>("Sportsman");
            _usersCollection = database.GetCollection<User>("Users");
            _businessOwnerCollection = database.GetCollection<BusinessOwner>("BusinessOwner");
            _entertainerCollection = database.GetCollection<Entertainer>("Entertainer");
            _fileStorageService = fileStorageService;
        }

        // Get all sportsmen
        public async Task<List<Sportsman>> GetSportsmenAsync()
        {
            return await _sportsmen.Find(_ => true).ToListAsync();
        }

        //Get user by ID
        public async Task<Sportsman> GetUserByUserID(string userid)
        {
            var filter = Builders<Sportsman>.Filter.Eq(u => u.UqID, userid);
            return await _sportsmen.Find(filter).FirstOrDefaultAsync();
        }
        // Get a sportsman by username
        public async Task<Sportsman> GetSportsmanByUsernameAsync(string username)
        {
            return await _sportsmen.Find(s => s.Username == username).FirstOrDefaultAsync();
        }

        // Get a sportsman by email
        public async Task<Sportsman> GetSportsmanByEmailAsync(string email)
        {
            return await _sportsmen.Find(s => s.email == email).FirstOrDefaultAsync();
        }

        // Check if username exists in Users, BusinessOwner, or Entertainer collections
        public async Task<bool> IsUsernameExistsInOtherCollectionsAsync(string username)
        {
            var userTask = _usersCollection.Find(u => u.Username == username).FirstOrDefaultAsync();
            var businessOwnerTask = _businessOwnerCollection.Find(b => b.Username == username).FirstOrDefaultAsync();
            var entertainerTask = _entertainerCollection.Find(e => e.Username == username).FirstOrDefaultAsync();

            await Task.WhenAll(userTask, businessOwnerTask, entertainerTask);

            return userTask.Result != null || businessOwnerTask.Result != null || entertainerTask.Result != null;
        }

        // Check if email exists in Users, BusinessOwner, or Entertainer collections
        public async Task<bool> IsEmailExistsInOtherCollectionsAsync(string email)
        {
            var userTask = _usersCollection.Find(u => u.email == email).FirstOrDefaultAsync();
            var businessOwnerTask = _businessOwnerCollection.Find(b => b.email == email).FirstOrDefaultAsync();
            var entertainerTask = _entertainerCollection.Find(e => e.email == email).FirstOrDefaultAsync();

            await Task.WhenAll(userTask, businessOwnerTask, entertainerTask);

            return userTask.Result != null || businessOwnerTask.Result != null || entertainerTask.Result != null;
        }

        // Create a new sportsman
        public async Task CreateSportsmanAsync(Sportsman sportsman)
        {

            //Validate the plaintext password before hashing
            if (!IsPasswordValid(sportsman.password))
            {
                throw new ArgumentException("Password does not meet the required format.");
            }
            // Check if the username exists in other collections
            var usernameExists = await IsUsernameExistsInOtherCollectionsAsync(sportsman.Username);
            if (usernameExists)
            {
                throw new InvalidOperationException("Username is already taken in another collection.");
            }

            // Check if the email exists in other collections
            var emailExists = await IsEmailExistsInOtherCollectionsAsync(sportsman.email);
            if (emailExists)
            {
                throw new InvalidOperationException("Email is already registered in another collection.");
            }

            // Generate a new ID for the sportsman
            int newUserId = await _counterService.GetNextIdAsync("sportsmanID");
            sportsman.sportsmanID = newUserId;

            // Hash the password before saving
            sportsman.password = HashPassword(sportsman.password);

            // Insert the sportsman into the database
            await _sportsmen.InsertOneAsync(sportsman);
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

        // Update sportsman data
        public async Task UpdateUserAsync(string id, Sportsman updatedUser, IFormFile profilePictureFile)
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

            var filter = Builders<Sportsman>.Filter.Eq(u => u.UqID, id);
            var update = Builders<Sportsman>.Update
                .Set(u => u.Username, updatedUser.Username)
                .Set(u => u.email, updatedUser.email)
                .Set(u => u.Address, updatedUser.Address)
                .Set(u => u.Sport, updatedUser.Sport)
                .Set(u => u.Country, updatedUser.Country)
                .Set(u => u.ContactNumber, updatedUser.ContactNumber)
                .Set(u => u.Team, updatedUser.Team)
                .Set(u => u.DateOfBirth, updatedUser.DateOfBirth)
                .Set(u => u.password, updatedUser.password)
                .Set(u => u.ProfilePictureUrl, updatedUser.ProfilePictureUrl);

            var result = await _sportsmen.UpdateOneAsync(filter, update);

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
            var filter = Builders<Sportsman>.Filter.Eq(u => u.UqID, userId);
            var update = Builders<Sportsman>.Update.Set(u => u.ProfilePictureUrl, profilePicturePath);

            var result = await _sportsmen.UpdateOneAsync(filter, update);

            if (result.MatchedCount == 0)
                throw new InvalidOperationException("User not found.");

            return profilePicturePath;
        }

        // Get Profile Picture
        public async Task<byte[]> GetProfilePictureAsync(string userId)
        {
            // Retrieve the profile picture path from the user document
            var filter = Builders<Sportsman>.Filter.Eq(u => u.UqID, userId);
            var user = await _sportsmen.Find(filter).FirstOrDefaultAsync();

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
            var isDeleted = await _fileStorageService.DeleteProfilePictureAsync(userId);

            if (isDeleted)
            {
                // Clear the ProfilePicturePath in the user document
                var filter = Builders<Sportsman>.Filter.Eq(u => u.UqID, userId);
                var update = Builders<Sportsman>.Update.Set(u => u.ProfilePictureUrl, null);

                var result = await _sportsmen.UpdateOneAsync(filter, update);

                if (result.MatchedCount == 0)
                    throw new InvalidOperationException("User not found.");
            }

            return isDeleted;
        }
    }
}