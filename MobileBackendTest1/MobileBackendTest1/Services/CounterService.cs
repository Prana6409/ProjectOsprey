using MongoDB.Bson;
using MongoDB.Driver;
using System.Threading.Tasks;

public class CounterService
{
    private readonly IMongoCollection<BsonDocument> _counterCollection;

    public CounterService(MongoDbHelper dbHelper)
    {
        _counterCollection = dbHelper.GetDatabase().GetCollection<BsonDocument>("Counter");
    }

    // Get next ID for a specific collection (user, post, etc.)
    public async Task<int> GetNextIdAsync(string counterName)
    {
        var filter = Builders<BsonDocument>.Filter.Eq("_id", counterName);
        var update = Builders<BsonDocument>.Update.Inc("seq", 1);
        var options = new FindOneAndUpdateOptions<BsonDocument>
        {
            ReturnDocument = ReturnDocument.After, // Get updated document
            IsUpsert = true // Create if not exists
        };

        var counterDoc = await _counterCollection.FindOneAndUpdateAsync(filter, update, options);
        return counterDoc["seq"].AsInt32;
    }
}
