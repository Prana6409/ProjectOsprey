using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using MobileBackendTest1.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MobileBackendTest1.Services
{
    public class PartnershipService
    {
        private readonly IMongoCollection<Partnership> _partnerships;
        private readonly CounterService _counterService;
        private readonly ILogger<PartnershipService> _logger;
        private readonly IFileStorageService _fileStorageService; // Injecting the IFileStorageService interface
        private readonly IMongoDatabase _database;

        public PartnershipService(IMongoDatabase database, CounterService counterService, ILogger<PartnershipService> logger, IFileStorageService fileStorageService)
        {
            _database = database; // Database is injected directly
            _partnerships = _database.GetCollection<Partnership>("Partnerships"); // Initialize collection
            _counterService = counterService;
            _logger = logger;
            _fileStorageService = fileStorageService; // Initialize the file storage service
        }

        // Get all partnerships
        public async Task<List<Partnership>> GetAllPartnershipsAsync()
        {
            return await _partnerships.Find(_ => true).ToListAsync();
        }

        // Get partnership by ID
        public async Task<Partnership> GetPartnershipByIdAsync(string id)
        {
            var filter = Builders<Partnership>.Filter.Eq(p => p.Id, id);
            return await _partnerships.Find(filter).FirstOrDefaultAsync();
        }

        // Create a new partnership with file upload
        public async Task CreatePartnershipAsync(Partnership partnership, IFormFile offerFile)
        {
            if (partnership == null)
                throw new ArgumentNullException(nameof(partnership));

            // Handle file upload if file is provided
            if (offerFile != null)
            {
                var filePath = await _fileStorageService.UploadFileAsync(offerFile); // Using the file storage service for file upload
                partnership.OfferDetails = filePath;  // Store the file path or URL in OfferDetails
            }

            // Generate PartnershipId via counter service
            partnership.PartnershipId = await _counterService.GetNextIdAsync("PartnershipId");
            partnership.CreatedAt = DateTime.UtcNow;
            partnership.UpdatedAt = DateTime.UtcNow;

            await _partnerships.InsertOneAsync(partnership);
            _logger.LogInformation($"Partnership created with ID {partnership.Id}.");
        }

        // Update an existing partnership with file upload
        public async Task UpdatePartnershipAsync(string id, Partnership updatedPartnership, IFormFile offerFile)
        {
            if (updatedPartnership == null)
                throw new ArgumentNullException(nameof(updatedPartnership));

            // Handle file upload if file is provided
            if (offerFile != null)
            {
                var filePath = await _fileStorageService.UploadFileAsync(offerFile); // Using the file storage service for file upload
                updatedPartnership.OfferDetails = filePath;  // Store the file path or URL in OfferDetails
            }

            var filter = Builders<Partnership>.Filter.Eq(p => p.Id, id);
            var update = Builders<Partnership>.Update
                .Set(p => p.BrandName, updatedPartnership.BrandName)
                .Set(p => p.Description, updatedPartnership.Description)
                .Set(p => p.OfferDetails, updatedPartnership.OfferDetails)  // Updating OfferDetails with new file path
                .Set(p => p.UpdatedAt, DateTime.UtcNow);

            var result = await _partnerships.UpdateOneAsync(filter, update);

            if (result.MatchedCount == 0)
                throw new InvalidOperationException("Partnership not found.");

            _logger.LogInformation($"Partnership with ID {id} updated.");
        }

        // Delete a partnership
        public async Task DeletePartnershipAsync(string id)
        {
            var filter = Builders<Partnership>.Filter.Eq(p => p.Id, id);
            var result = await _partnerships.DeleteOneAsync(filter);

            if (result.DeletedCount == 0)
                throw new InvalidOperationException("Partnership not found.");

            _logger.LogInformation($"Partnership with ID {id} deleted.");
        }
    }
}
