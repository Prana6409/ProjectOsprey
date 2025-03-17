using MongoDB.Driver;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;
using MobileBackendTest1.Models;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace MobileBackendTest1.Services
{
    public class UserService
    {
        private readonly IMongoDatabase _database;
        private readonly CounterService _counterService;
        private readonly IMongoCollection<User> _users;
        private readonly IFileStorageService _fileStorageService;
        private readonly IMongoCollection<Sportsman> _sportsmanCollection;
        private readonly IMongoCollection<BusinessOwner> _businessOwnerCollection;
        private readonly IMongoCollection<Entertainer> _entertainerCollection;

        public UserService(IMongoDatabase database, CounterService counterService, IFileStorageService fileStorageService)
        {
            _database = database;
            _counterService = counterService;
            _users = _database.GetCollection<User>("Users"); // Ensure this matches your collection name
            _sportsmanCollection = _database.GetCollection<Sportsman>("Sportsman");
            _businessOwnerCollection = _database.GetCollection<BusinessOwner>("BusinessOwner");
            _entertainerCollection = _database.GetCollection<Entertainer>("Entertainer");
            _fileStorageService = fileStorageService;
        }
        //Get user by ID
        public async Task<User> GetUserByUserID(string userid)
        {
            var filter = Builders<User>.Filter.Eq(u => u.UqID, userid);
            return await _users.Find(filter).FirstOrDefaultAsync();
        }
        // Get user by Name
        public async Task<User> GetUserByNameAsync(string name)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Username, name); // Query filter by name
            return await _users.Find(filter).FirstOrDefaultAsync(); // Return the first user matching the name, or null if not found
        }

        // Get user by Email
        public async Task<User> GetUserByEmailAsync(string email)
        {
            var filter = Builders<User>.Filter.Eq(u => u.email, email); // Query filter by email
            return await _users.Find(filter).FirstOrDefaultAsync(); // Return the first user matching the email, or null if not found
        }

        // Check if name exists in Sportsman, BusinessOwner, or Entertainer collections
        public async Task<bool> IsNameExistsInOtherCollectionsAsync(string name)
        {
            var sportsmanFilter = Builders<Sportsman>.Filter.Eq(s => s.Username, name);
            var businessOwnerFilter = Builders<BusinessOwner>.Filter.Eq(b => b.Username, name);
            var entertainerFilter = Builders<Entertainer>.Filter.Eq(e => e.Username, name);

            var sportsmanTask = _sportsmanCollection.Find(sportsmanFilter).FirstOrDefaultAsync();
            var businessOwnerTask = _businessOwnerCollection.Find(businessOwnerFilter).FirstOrDefaultAsync();
            var entertainerTask = _entertainerCollection.Find(entertainerFilter).FirstOrDefaultAsync();

            await Task.WhenAll(sportsmanTask, businessOwnerTask, entertainerTask);

            return sportsmanTask.Result != null || businessOwnerTask.Result != null || entertainerTask.Result != null;
        }

        // Check if email exists in Sportsman, BusinessOwner, or Entertainer collections
        public async Task<bool> IsEmailExistsInOtherCollectionsAsync(string email)
        {
            var sportsmanFilter = Builders<Sportsman>.Filter.Eq(s => s.email, email);
            var businessOwnerFilter = Builders<BusinessOwner>.Filter.Eq(b => b.email, email);
            var entertainerFilter = Builders<Entertainer>.Filter.Eq(e => e.email, email);

            var sportsmanTask = _sportsmanCollection.Find(sportsmanFilter).FirstOrDefaultAsync();
            var businessOwnerTask = _businessOwnerCollection.Find(businessOwnerFilter).FirstOrDefaultAsync();
            var entertainerTask = _entertainerCollection.Find(entertainerFilter).FirstOrDefaultAsync();

            await Task.WhenAll(sportsmanTask, businessOwnerTask, entertainerTask);

            return sportsmanTask.Result != null || businessOwnerTask.Result != null || entertainerTask.Result != null;
        }

        // Get all users from 'Users' collection
        public async Task<List<User>> GetUsersAsync()
        {
            var collection = _database.GetCollection<User>("Users"); // Dynamically get 'Users' collection
            return await collection.Find(_ => true).ToListAsync();  // Get all users from 'Users' collection
        }

        // Hash Password Before Saving (SHA256)
        public string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2")); // Convert to hexadecimal
                }
                return builder.ToString();
            }
        }

        // Create a new user in the 'Users' collection
        public async Task CreateUserAsync(User user)
        {

            //Validate the plaintext password before hashing
            if (!IsPasswordValid(user.password))
            {
                throw new ArgumentException("Password does not meet the required format.");
            }
            // Hash the password before storing it
            user.password = HashPassword(user.password);
            int newUserId = await _counterService.GetNextIdAsync("UserID");

            // Setting the new user ID before inserting into the database
            user.UserID = newUserId;

            var collection = _database.GetCollection<User>("Users"); // Dynamically get 'Users' collection
            await collection.InsertOneAsync(user);  // Insert the new user into the 'Users' collection
        }
        // Validate password against regex rules
        private bool IsPasswordValid(string password)
        {
            // Regex pattern for password validation
            var passwordRegex = new Regex(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$");
            return passwordRegex.IsMatch(password);
        }

        // For dynamic collections, you can pass the collection name as a parameter
        public async Task<List<T>> GetCollectionDataAsync<T>(string collectionName)
        {
            var collection = _database.GetCollection<T>(collectionName); // Get the specified collection by name
            return await collection.Find(_ => true).ToListAsync(); // Get all documents in the collection
        }

        // Create a new document in any collection (based on collectionName)
        public async Task CreateDocumentAsync<T>(string collectionName, T document)
        {
            var collection = _database.GetCollection<T>(collectionName); // Get the specified collection by name
            await collection.InsertOneAsync(document); // Insert the document into the collection
        }

        // Update user data
        public async Task UpdateUserAsync(string Id, User updatedUser, IFormFile profilePictureFile)
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
                var profilePictureUrl = await _fileStorageService.UploadProfilePictureAsync(Id.ToString(), profilePictureFile);
                updatedUser.ProfilePictureUrl = profilePictureUrl;
            }

            var filter = Builders<User>.Filter.Eq(u => u.Id, Id);
            var update = Builders<User>.Update
                .Set(u => u.Username, updatedUser.Username)
                .Set(u => u.email, updatedUser.email)
                .Set(u => u.address, updatedUser.address)
                .Set(u => u.dateofbirth, updatedUser.dateofbirth)
                .Set(u => u.country, updatedUser.country)
                .Set(u => u.number, updatedUser.number)
                .Set(u => u.password, updatedUser.password)
                .Set(u => u.ProfilePictureUrl, updatedUser.ProfilePictureUrl);
            // Update profile picture path

            var result = await _users.UpdateOneAsync(filter, update);

            if (result.MatchedCount == 0)
                throw new InvalidOperationException("User not found.");
        }
        // Upload or Update Profile Picture
        public async Task<string> UploadOrUpdateProfilePictureAsync(int userId, IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("Invalid file.");

            // Upload the profile picture using IFileStorageService
            var profilePicturePath = await _fileStorageService.UploadProfilePictureAsync(userId.ToString(), file);

            // Update the user's ProfilePicturePath in the database
            var filter = Builders<User>.Filter.Eq(u => u.UserID, userId);
            var update = Builders<User>.Update.Set(u => u.ProfilePictureUrl, profilePicturePath);

            var result = await _users.UpdateOneAsync(filter, update);

            if (result.MatchedCount == 0)
                throw new InvalidOperationException("User not found.");

            return profilePicturePath;
        }

        // Get Profile Picture
        public async Task<byte[]> GetProfilePictureAsync(int userId)
        {
            // Retrieve the profile picture path from the user document
            var filter = Builders<User>.Filter.Eq(u => u.UserID, userId);
            var user = await _users.Find(filter).FirstOrDefaultAsync();

            if (user == null || string.IsNullOrEmpty(user.ProfilePictureUrl))
            {
                throw new InvalidOperationException("User or profile picture not found.");
            }

            // Retrieve the profile picture file using IFileStorageService
            return await _fileStorageService.GetProfilePictureAsync(userId.ToString());
        }

        // Delete Profile Picture
        public async Task<bool> DeleteProfilePictureAsync(int userId)
        {
            // Delete the profile picture file using IFileStorageService
            var isDeleted = await _fileStorageService.DeleteProfilePictureAsync(userId.ToString());

            if (isDeleted)
            {
                // Clear the ProfilePicturePath in the user document
                var filter = Builders<User>.Filter.Eq(u => u.UserID, userId);
                var update = Builders<User>.Update.Set(u => u.ProfilePictureUrl, null);

                var result = await _users.UpdateOneAsync(filter, update);

                if (result.MatchedCount == 0)
                    throw new InvalidOperationException("User not found.");
            }

            return isDeleted;
        }

    }
}