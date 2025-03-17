using MobileBackendTest1.Models;
using MongoDB.Driver;

public class LogInService
{
    private readonly IMongoCollection<User> _users;
    private readonly IMongoCollection<Sportsman> _sportsmen;
    private readonly IMongoCollection<BusinessOwner> _businessOwners;
    private readonly IMongoCollection<Entertainer> _entertainers;

    public LogInService(IMongoDatabase database)
    {
        _users = database.GetCollection<User>("Users");
        _sportsmen = database.GetCollection<Sportsman>("Sportsmen");
        _businessOwners = database.GetCollection<BusinessOwner>("BusinessOwners");
        _entertainers = database.GetCollection<Entertainer>("Entertainers");
    }

    public async Task<dynamic> AuthenticateAsync(string email, string password)
    {
        // Check each collection for a matching user
        var user = await _users.Find(u => u.email == email && u.password == password).FirstOrDefaultAsync();
        if (user != null)
        {
            return new { Id = user.Id, UqiD=user.UqID,Email = user.email, Role = "Users" };
        }

        var sportsman = await _sportsmen.Find(s => s.email == email && s.password == password).FirstOrDefaultAsync();
        if (sportsman != null)
        {
            return new { Id = sportsman.Id, UqiD = sportsman.UqID, Email = sportsman.email, Role = "Sportsman" };
        }

        var businessOwner = await _businessOwners.Find(b => b.email == email && b.password == password).FirstOrDefaultAsync();
        if (businessOwner != null)
        {
            return new { Id = businessOwner.Id, UqiD = businessOwner.UqID, Email = businessOwner.email, Role = "BusinessOwner" };
        }

        var entertainer = await _entertainers.Find(e => e.email == email && e.password == password).FirstOrDefaultAsync();
        if (entertainer != null)
        {
            return new { Id = entertainer.Id, UqiD = entertainer.UqID, Email = entertainer.email, Role = "Entertainer" };
        }

        // If no match is found, return null
        return null;
    }
}