using Microsoft.OpenApi.Services;
using MobileBackendTest1.Models;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MobileBackendTest1.Services
{
    public class FunctionService
    {
        private readonly IMongoCollection<Content> _contents;
        private readonly IMongoCollection<BusinessOwner> _businessOwners;
        private readonly IMongoCollection<User> _usersCollection;
        private readonly IMongoCollection<Sportsman> _sportsmanCollection;
        private readonly IMongoCollection<Entertainer> _entertainerCollection;
        private readonly IMongoCollection<Event> _events;

        public FunctionService(IMongoDatabase database)
        {
            _events = database.GetCollection<Event>("Events");
            _contents = database.GetCollection<Content>("Content");
            _businessOwners = database.GetCollection<BusinessOwner>("BusinessOwner");
            _usersCollection = database.GetCollection<User>("Users");
            _sportsmanCollection = database.GetCollection<Sportsman>("Sportsman");
            _entertainerCollection = database.GetCollection<Entertainer>("Entertainer");
        }

        public class BusinessOwnerDto
        {
            public string BusinessName { get; set; }
            public string BusinessType { get; set; }
        }

        public class SportsmanDto
        {
            public string Sport { get; set; }
            public string Team { get; set; }
        }

        public class EntertainerDto
        {
            public string ArtForm { get; set; }
            public string StageName { get; set; }
        }

        public async Task<List<object>> SearchUsernamesAsync(string query, bool exactMatch)
        {
            if (exactMatch)
            {
                // Exact username search
                var userTask = _usersCollection.Find(u => u.Username == query).ToListAsync();
                var sportsmanTask = _sportsmanCollection.Find(s => s.Username == query).ToListAsync();
                var entertainerTask = _entertainerCollection.Find(e => e.Username == query).ToListAsync();
                var businessOwnerTask = _businessOwners.Find(b => b.Username == query).ToListAsync();

                await Task.WhenAll(userTask, sportsmanTask, entertainerTask, businessOwnerTask);

                var results = new List<object>();

                if (userTask.Result.Any())
                {
                    results.AddRange(userTask.Result.Select(u => new { Collection = "Users", Data = u }));
                }
                if (sportsmanTask.Result.Any())
                {
                    results.AddRange(sportsmanTask.Result.Select(s => new { Collection = "Sportsman", Data = s }));
                }
                if (entertainerTask.Result.Any())
                {
                    results.AddRange(entertainerTask.Result.Select(e => new { Collection = "Entertainer", Data = e }));
                }
                if (businessOwnerTask.Result.Any())
                {
                    results.AddRange(businessOwnerTask.Result.Select(b => new { Collection = "BusinessOwner", Data = b }));
                }
                foreach (var result in results)
                {
                    Console.WriteLine($"Collection: {result.GetType().GetProperty("Collection")?.GetValue(result)}, Data: {result.GetType().GetProperty("Data")?.GetValue(result)}");
                }

                return results;
            }
            else
            {
                // Partial username search (search-as-you-type)
                var userTask = _usersCollection.Find(u => u.Username.StartsWith(query)).ToListAsync();
                var sportsmanTask = _sportsmanCollection.Find(s => s.Username.StartsWith(query)).ToListAsync();
                var entertainerTask = _entertainerCollection.Find(e => e.Username.StartsWith(query)).ToListAsync();
                var businessOwnerTask = _businessOwners.Find(b => b.Username.StartsWith(query)).ToListAsync();

                await Task.WhenAll(userTask, sportsmanTask, entertainerTask, businessOwnerTask);

                var results = new List<object>();

                if (userTask.Result.Any())
                {
                    results.AddRange(userTask.Result.Select(u => new { Collection = "Users", Data = u }));
                }
                if (sportsmanTask.Result.Any())
                {
                    results.AddRange(sportsmanTask.Result.Select(s => new { Collection = "Sportsman", Data = s }));
                }
                if (entertainerTask.Result.Any())
                {
                    results.AddRange(entertainerTask.Result.Select(e => new { Collection = "Entertainer", Data = e }));
                }
                if (businessOwnerTask.Result.Any())
                {
                    results.AddRange(businessOwnerTask.Result.Select(b => new { Collection = "BusinessOwners", Data = b }));
                }

                return results;
            }
        }

        private async Task<object> GetUserByIdAndRoleAsync(string uId, string role)
        {
            switch (role)
            {
                case "BusinessOwner":
                    var businessOwner = await _businessOwners.Find(b => b.UqID == uId).FirstOrDefaultAsync();
                    if (businessOwner != null)
                    {
                        return new BusinessOwnerDto
                        {
                            BusinessName = businessOwner.CompanyName,
                            BusinessType = businessOwner.sponsorshipInterest
                        };
                    }
                    break;

                case "Sportsman":
                    var sportsman = await _sportsmanCollection.Find(s => s.UqID == uId).FirstOrDefaultAsync();
                    if (sportsman != null)
                    {
                        return new SportsmanDto
                        {
                            Sport = sportsman.Sport,
                            Team = sportsman.Team
                        };
                    }
                    break;

                case "Entertainer":
                    var entertainer = await _entertainerCollection.Find(e => e.UqID == uId).FirstOrDefaultAsync();
                    if (entertainer != null)
                    {
                        return new EntertainerDto
                        {
                            ArtForm = entertainer.Talent,
                            StageName = entertainer.stagename
                        };
                    }
                    break;

                case "User":
                    var user = await _usersCollection.Find(u => u.UqID == uId).FirstOrDefaultAsync();
                    if (user != null)
                    {
                        return new
                        {
                            Username = user.Username,
                            ProfilePicture = user.ProfilePictureUrl,
                            DateOfBirth = user.dateofbirth,
                            Country = user.country,
                            Role = user.Role
                        };
                    }
                    break;

                default:
                    return null; // Invalid role
            }

            return null; // User not found
        }

        public async Task<(string uId, string Role)> GetUserIdAndRoleAsync(string username)
        {
            // Use the existing SearchUsernamesAsync method to find the user
            var searchResults = await SearchUsernamesAsync(username, exactMatch: true);

            if (searchResults.Any())
            {
                var user = searchResults.First();

                // Extract the actual user object from the anonymous object
                var dataProperty = user.GetType().GetProperty("Data");
                if (dataProperty != null)
                {
                    var userData = dataProperty.GetValue(user);

                    // Use reflection to access the UqID and Role properties of the user object
                    var idProperty = userData.GetType().GetProperty("UqID") ?? userData.GetType().GetProperty("UqId"); // Handle case sensitivity
                    var roleProperty = userData.GetType().GetProperty("Role");

                    if (idProperty != null && roleProperty != null)
                    {
                        string uId = idProperty.GetValue(userData)?.ToString();
                        string role = roleProperty.GetValue(userData)?.ToString();

                        if (!string.IsNullOrEmpty(uId) && !string.IsNullOrEmpty(role))
                        {
                            return (uId, role); // Return the uId and role of the user
                        }
                    }
                }
            }

            return (null, null); // User not found or properties are missing
        }

        public async Task<Function> GetUserProfileAsync(string username)
        {
            // Fetch the uId and role of the user using the helper function
            var (uId, role) = await GetUserIdAndRoleAsync(username);
            if (uId == null || role == null)
            {
                return null; // User not found
            }

            // Fetch user details
            var userDetails = await GetUserByIdAndRoleAsync(uId, role);
            if (userDetails == null)
            {
                return null; // User details not found
            }

            // Fetch user's contents and events using the uqid field
            var contents = await _contents.Find(c => c.UqId == uId).ToListAsync();
            var events = await _events.Find(e => e.UqId == uId).ToListAsync();

            // Combine all data into a single response
            return new Function
            {
                Collection = "UserProfile",
                Data = new
                {
                    UserInfo = userDetails, // User details (role-specific)
                    Contents = contents,
                    Events = events
                }
            };
        }
    }
}