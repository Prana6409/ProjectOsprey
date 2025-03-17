using MongoDB.Driver;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;
using MobileBackendTest1.Models;
using Microsoft.Extensions.Logging;

namespace MobileBackendTest1.Services
{
    public class MembershipService
    {
        private readonly IMongoCollection<Membership> _memberships;
        private readonly CounterService _counterService;
        private readonly ILogger<MembershipService> _logger;

        public MembershipService(IMongoDatabase database, CounterService counterService, ILogger<MembershipService> logger)
        {
            _memberships = database.GetCollection<Membership>("Memberships"); // Ensure collection name matches
            _counterService = counterService;
            _logger = logger;

            //Ensure MembershipId 
            CreateIndexes();
        }
        private void CreateIndexes()
        {
            var indexKeysDefinition = Builders<Membership>.IndexKeys.Ascending(m => m.MembershipId);
            var indexOptions = new CreateIndexOptions { Unique = true };  // Ensure it's unique to avoid duplicate MembershipId
            var indexModel = new CreateIndexModel<Membership>(indexKeysDefinition, indexOptions);

            _memberships.Indexes.CreateOne(indexModel); // Create the index on the MembershipId field
        }

        // Get all memberships
        public async Task<List<Membership>> GetAllMembershipsAsync()
        {
            return await _memberships.Find(_ => true).ToListAsync();
        }

        // Get membership by ID
        public async Task<Membership> GetMembershipByIdAsync(string id)
        {
            var filter = Builders<Membership>.Filter.Eq(m => m.MembershipId, id);
            return await _memberships.Find(filter).FirstOrDefaultAsync();
        }

        // Create a new membership
        public async Task CreateMembershipAsync(Membership membership)
        {
            int newMembershipId = await _counterService.GetNextIdAsync("MembershipID");

            membership.MembershipId = newMembershipId.ToString();
            membership.CreatedAt = DateTime.UtcNow;
            membership.UpdatedAt = DateTime.UtcNow;

            await _memberships.InsertOneAsync(membership);
        }

        // Update a membership
        public async Task UpdateMembershipAsync(string id, Membership updatedMembership)
        {
            updatedMembership.UpdatedAt = DateTime.UtcNow;
            await _memberships.ReplaceOneAsync(m => m.MembershipId == id, updatedMembership);
        }

        // Delete a membership
        public async Task DeleteMembershipAsync(string id)
        {
            await _memberships.DeleteOneAsync(m => m.MembershipId == id);
        }
    }
}
