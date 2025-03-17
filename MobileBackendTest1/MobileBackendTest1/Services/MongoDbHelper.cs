using MongoDB.Driver;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;

public class MongoDbHelper
{
    private readonly IMongoDatabase _database;
    private readonly ILogger<MongoDbHelper> _logger;

    // Constructor to initialize connection and handle connection errors
    public MongoDbHelper(IConfiguration configuration, ILogger<MongoDbHelper> logger)
    {
        _logger = logger;
        try
        {
            // Retrieve MongoDB connection settings from appsettings.json
            var connectionString = configuration.GetValue<string>("MongoDB:ConnectionString");
            var databaseName = configuration.GetValue<string>("MongoDB:DatabaseName");

            // Establish connection to MongoDB
            var client = new MongoClient(connectionString);
            _database = client.GetDatabase(databaseName);
        }
        catch (Exception ex)
        {
            // Log the error using the logger
            _logger.LogError($"Error connecting to MongoDB: {ex.Message}");

            // Optionally, rethrow the exception to handle it upstream if needed
            throw new Exception("MongoDB connection failed. Please check your connection settings.", ex);
        }
    }

    // Method to get the database instance
    public IMongoDatabase GetDatabase() => _database;
}
