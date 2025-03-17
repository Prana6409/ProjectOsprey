using MobileBackendTest1;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using MobileBackendTest1.Services;

public class Program
{
    public static void Main(string[] args)
    {
        // Create WebApplication builder
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddLogging();

        // Load configuration from appsettings.json (or appsettings.js, as you mentioned)
        var configuration = builder.Configuration;

        // Register MongoDB Client and Database in DI container
        builder.Services.AddSingleton<IMongoClient>(sp =>
        {
            var connectionString = configuration["MongoDB:ConnectionString"];
            return new MongoClient(connectionString);  // Create MongoClient instance with connection string
        });

        builder.Services.AddSingleton(sp =>
        {
            var client = sp.GetRequiredService<IMongoClient>();
            var databaseName = configuration["MongoDB:DatabaseName"]; // Access database name from config
            return client.GetDatabase(databaseName);  // Get MongoDB database
        });

        // Register other services to DI container
        builder.Services.AddSingleton<UserService>();
        builder.Services.AddSingleton<CounterService>();
        builder.Services.AddSingleton<MongoDbHelper>();
        builder.Services.AddSingleton<MembershipService>();
        builder.Services.AddSingleton<ContentService>();
        builder.Services.AddSingleton<LogInService>();
        builder.Services.AddSingleton<PartnershipService>();
        builder.Services.AddSingleton<BusinessOwnerService>();
        builder.Services.AddSingleton<SportsmanService>();
        builder.Services.AddSingleton<EntertainerService>();
        builder.Services.AddSingleton<IFileStorageService, FileStorageService>();
        builder.Services.AddSingleton<FunctionService>();
        
        

        builder.Services.AddEndpointsApiExplorer();  // For endpoint discovery
        builder.Services.AddSwaggerGen();            // Add Swagger generator

        // Enable API Controllers
        builder.Services.AddControllers();

    
        // Add CORS configuration
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", builder =>
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader());
        });

        // Build the app
        var app = builder.Build();

 

        // Configure the HTTP request pipeline
        if (app.Environment.IsDevelopment())
        {
           
            app.UseSwagger();
            app.UseSwaggerUI();
        }


        app.UseStaticFiles();

        app.UseRouting();

        // Apply CORS policy
        app.UseCors("AllowAll");


  

        // HTTPS redirection and authorization
        app.UseHttpsRedirection();


        app.UseAuthorization();

        // Map controllers to endpoints
        app.MapControllers();

        // Run the application
        app.Run();
    }
}
