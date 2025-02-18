using Gringotts.Api.Features.Client.Services;
using Gringotts.Api.Features.ClientAuthentication.Models;
using Gringotts.Api.Features.ClientAuthentication.Services;
using Gringotts.Api.Features.Interactions.Models;
using Gringotts.Api.Features.User.Models;
using Gringotts.Api.Shared.Core;
using Gringotts.Api.Shared.Utilities;
using Newtonsoft.Json;

namespace Gringotts.Api.Features.Populator;

public class ClientPopulatorService(
    AppDbContext dbContext,
    ClientService clientService,
    ClientSecretService clientSecretService,
    HashingService service
) {
    private const string ApiUrl = "https://randomuser.me/api/?results=5";

    public async Task PopulateDatabaseAsync()
    {
        var adminClient = (await clientService.CreateUserAsync(
            "Andrei John", "Sumilang", "Paleracio"
        )).Value!;
    
        var adminClientSecret = (await clientSecretService.CreateSecretAsync(
            adminClient.Id, "slangdrei.07", "Sl@ngDrei07"
        ));
    }

    public async Task PopulateDatabaseAsync1()
    {
        var apiData = await FetchApiDataAsync();
        if (apiData?.Results is not { Count: > 0 })
        {
            throw new InvalidOperationException("Failed to fetch client data from the API.");
        }

        var clients = new List<Client.Models.Client>();
        var clientSecrets = new List<ClientSecret>();

        var locations = new List<Gringotts.Api.Features.Reader.Models.Location>();
        var readers = new List<Reader.Models.Reader>();

        var departments = new List<Department>();
        var users = new List<Gringotts.Api.Features.User.Models.User>();

        var interactionLogs = new List<InteractionLog>();
        var sessions = new List<Session>();

        foreach (var result in apiData.Results)
        {
            var client = new Client.Models.Client
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                FirstName = result.Name.First,
                LastName = result.Name.Last,
                MiddleName = null // API does not provide middle names
            };

            var clientSecret = new ClientSecret
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                ClientId = client.Id,
                Username = result.Login.Username,
                Password = service.Hash(result.Login.Password)
            };

            var location = new Gringotts.Api.Features.Reader.Models.Location
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                BuildingName = result.Location.Country,
                RoomName = result.Location.State,
            };

            var reader = new Reader.Models.Reader
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                Name = result.Location.City,
                LocationId = location.Id
            };

            var department = new Department
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                Name = result.Location.Street.Name,
            };


            var user = new Gringotts.Api.Features.User.Models.User
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                AccessExpiry = DateTime.UtcNow.AddDays(7),
                CardId = result.Location.Postcode,
                SchoolId = result.Location.Postcode,
                FirstName = result.Name.First,
                MiddleName = null,
                LastName = result.Name.Last,
                Affiliation = (byte)Random.Shared.Next(0, 4),
                Sex = (byte)Random.Shared.Next(0, 2),
                DepartmentId = department.Id
            };

            foreach (var i in Enumerable.Range(1, 10))
            {
                interactionLogs.AddRange(Enumerable.Range(20, 55)
                .Select(_ => new InteractionLog
                {
                    Id = Guid.NewGuid(),
                    CreatedAt = DateTime.UtcNow,
                    LogReaderId = reader.Id,
                    LogUserId = user.Id,
                    CardId = user.CardId,
                    DateTime = DateTime.UtcNow.Subtract(TimeSpan.FromDays(i)),
                    InteractionType = (InteractionType)(byte)Random.Shared.Next(0, 4),
                }));

                var sessionLog = new Session
                {
                    Id = Guid.NewGuid(),
                    CreatedAt = DateTime.UtcNow,
                    LogReaderId = reader.Id,
                    LogUserId = user.Id,
                    StartDate = DateTime.UtcNow.Subtract(TimeSpan.FromDays(i)),
                    EndDate = DateTime.UtcNow.Subtract(TimeSpan.FromDays(i)).AddHours(1)
                };

                sessions.Add(sessionLog);
            }

            clients.Add(client);
            clientSecrets.Add(clientSecret);
            locations.Add(location);
            readers.Add(reader);
            departments.Add(department);
            users.Add(user);
        }

        await dbContext.Clients.AddRangeAsync(clients);
        await dbContext.ClientSecrets.AddRangeAsync(clientSecrets);
        await dbContext.Locations.AddRangeAsync(locations);
        await dbContext.Readers.AddRangeAsync(readers);
        await dbContext.Departments.AddRangeAsync(departments);
        await dbContext.LogUsers.AddRangeAsync(users);
        await dbContext.InteractionLogs.AddRangeAsync(interactionLogs);
        await dbContext.Sessions.AddRangeAsync(sessions);

        await dbContext.SaveChangesAsync();
    }

    private async Task<UserResponse?> FetchApiDataAsync()
    {
        var client = new HttpClient();
        var response = await client.GetStringAsync(ApiUrl);
        return JsonConvert.DeserializeObject<UserResponse>(response);
    }
}

// API Response Structure
public class UserResponse
{
    [JsonProperty("results")] public List<User> Results { get; set; }

    [JsonProperty("info")] public Info Info { get; set; }
}

public class User
{
    [JsonProperty("gender")] public string Gender { get; set; }

    [JsonProperty("name")] public Name Name { get; set; }

    [JsonProperty("location")] public Location Location { get; set; }

    [JsonProperty("email")] public string Email { get; set; }

    [JsonProperty("login")] public Login Login { get; set; }

    [JsonProperty("dob")] public Dob Dob { get; set; }

    [JsonProperty("registered")] public Registered Registered { get; set; }

    [JsonProperty("phone")] public string Phone { get; set; }

    [JsonProperty("cell")] public string Cell { get; set; }

    [JsonProperty("id")] public Id Id { get; set; }

    [JsonProperty("picture")] public Picture Picture { get; set; }

    [JsonProperty("nat")] public string Nat { get; set; }
}

public class Name
{
    [JsonProperty("title")] public string Title { get; set; }

    [JsonProperty("first")] public string First { get; set; }

    [JsonProperty("last")] public string Last { get; set; }
}

public class Location
{
    [JsonProperty("street")] public Street Street { get; set; }

    [JsonProperty("city")] public string City { get; set; }

    [JsonProperty("state")] public string State { get; set; }

    [JsonProperty("country")] public string Country { get; set; }

    [JsonProperty("postcode")] public string Postcode { get; set; }

    [JsonProperty("coordinates")] public Coordinates Coordinates { get; set; }

    [JsonProperty("timezone")] public Timezone Timezone { get; set; }
}

public class Street
{
    [JsonProperty("number")] public int Number { get; set; }

    [JsonProperty("name")] public string Name { get; set; }
}

public class Coordinates
{
    [JsonProperty("latitude")] public string Latitude { get; set; }

    [JsonProperty("longitude")] public string Longitude { get; set; }
}

public class Timezone
{
    [JsonProperty("offset")] public string Offset { get; set; }

    [JsonProperty("description")] public string Description { get; set; }
}

public class Login
{
    [JsonProperty("uuid")] public string Uuid { get; set; }

    [JsonProperty("username")] public string Username { get; set; }

    [JsonProperty("password")] public string Password { get; set; }

    [JsonProperty("salt")] public string Salt { get; set; }

    [JsonProperty("md5")] public string Md5 { get; set; }

    [JsonProperty("sha1")] public string Sha1 { get; set; }

    [JsonProperty("sha256")] public string Sha256 { get; set; }
}

public class Dob
{
    [JsonProperty("date")] public DateTime Date { get; set; }

    [JsonProperty("age")] public int Age { get; set; }
}

public class Registered
{
    [JsonProperty("date")] public DateTime Date { get; set; }

    [JsonProperty("age")] public int Age { get; set; }
}

public class Id
{
    [JsonProperty("name")] public string Name { get; set; }

    [JsonProperty("value")] public string Value { get; set; }
}

public class Picture
{
    [JsonProperty("large")] public string Large { get; set; }

    [JsonProperty("medium")] public string Medium { get; set; }

    [JsonProperty("thumbnail")] public string Thumbnail { get; set; }
}

public class Info
{
    [JsonProperty("seed")] public string Seed { get; set; }

    [JsonProperty("results")] public int Results { get; set; }

    [JsonProperty("page")] public int Page { get; set; }

    [JsonProperty("version")] public string Version { get; set; }
}