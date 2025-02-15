using Gringotts.Api.Features.ClientAuthentication.Models;
using Gringotts.Api.Shared.Core;
using Gringotts.Api.Shared.Utilities;
using Newtonsoft.Json;

namespace Gringotts.Api.Features.Populator;

public class ClientPopulatorService(AppDbContext dbContext, HashingService service)
{
    public async Task PopulateDatabaseAsync()
    {
        var fakeClients = new List<Client.Models.Client>();

        // Fetch data from API
        var response = await new HttpClient().GetStringAsync("https://randomuser.me/api/?results=10");
        var apiData = JsonConvert.DeserializeObject<ApiResponse>(response); // Deserialize API response

        // Use API data to populate the database
        foreach (var result in apiData.Results)
        {
            var client = new Client.Models.Client
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                FirstName = result.Name.First,
                LastName = result.Name.Last,
                MiddleName = null // API does not provide middle name, you can leave it null or generate one
            };

            fakeClients.Add(client);
        }

        await dbContext.Clients.AddRangeAsync(fakeClients);
        await dbContext.SaveChangesAsync();

        // Now, populate ClientSecrets for each client
        foreach (var client in fakeClients)
        {
            var clientSecret = new ClientSecret
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                ClientId = client.Id,
                Username = apiData.Results[0].Login.Username, // You may want to use a unique username per client
                Password = service.Hash(apiData.Results[0].Login.Password) // You may want to hash this password
            };

            await dbContext.ClientSecrets.AddAsync(clientSecret);
        }

        await dbContext.SaveChangesAsync();
    }
}

// API Response Structure
public class ApiResponse
{
    [JsonProperty("results")]
    public List<ApiResult> Results { get; set; }
}

public class ApiResult
{
    [JsonProperty("name")]
    public Name Name { get; set; }
    
    [JsonProperty("login")]
    public Login Login { get; set; }
}

public class Name
{
    [JsonProperty("first")]
    public string First { get; set; }

    [JsonProperty("last")]
    public string Last { get; set; }
}

public class Login
{
    [JsonProperty("username")]
    public string Username { get; set; }

    [JsonProperty("password")]
    public string Password { get; set; }
}