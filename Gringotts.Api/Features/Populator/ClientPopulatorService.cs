using Gringotts.Api.Features.Client.Services;
using Gringotts.Api.Features.ClientAuthentication.Models;
using Gringotts.Api.Features.ClientAuthentication.Services;
using Gringotts.Api.Features.Interactions.Models;
using Gringotts.Api.Features.User.Models;
using Gringotts.Api.Shared.Core;
using Gringotts.Api.Shared.Utilities;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;

namespace Gringotts.Api.Features.Populator;

public class ClientPopulatorService(
    AppDbContext dbContext,
    ClientService clientService,
    ClientSecretService clientSecretService,
    HashingService service
) {
    private const string ApiUrl = "https://randomuser.me/api/?results=10";

    public async Task PopulateDatabaseAsync()
    {
    }

    public async Task PopulateDatabaseAsync1()
    {
        var location = new Reader.Models.Location()
        {
            Id = Guid.NewGuid(),
            BuildingName = "Library",
            RoomName = "Main",
            CreatedAt = DateTime.UtcNow
        };
        
        dbContext.Locations.Add(location);
        
        var reader = new Reader.Models.Reader
        {
            Id = Guid.NewGuid(),
            LocationId = location.Id,
            Name = "Library Main",
            CreatedAt = DateTime.UtcNow
        };
        
        dbContext.Readers.Add(reader);
        
        var department = new Department
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            Name = "SHS"
        };
        
        dbContext.Departments.Add(department);

        var user = new Gringotts.Api.Features.User.Models.User
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            AccessExpiry = DateTime.UtcNow.AddDays(7),
            CardId = "3871879588",
            SchoolId = "123456",
            FirstName = "Lindsay Angeline",
            MiddleName = "L.",
            LastName = "Abelar",
            Affiliation = 1,
            Sex = 1,
            DepartmentId = department.Id
        };
        
        dbContext.LogUsers.Add(user);
        
        
        // var adminClient = (await clientService.CreateUserAsync(
        //     "Admin Library", "Main"
        // )).Value!;
        //
        // var adminClientSecret = (await clientSecretService.CreateSecretAsync(
        //     adminClient.Id, "admin123", "admin12345"
        // ));
        // //
        // var location = new Reader.Models.Location()
        // {
        //     Id = Guid.NewGuid(),
        //     BuildingName = "Library",
        //     RoomName = "Main",
        //     CreatedAt = DateTime.UtcNow
        // };
        //
        // dbContext.Locations.Add(location);
        //
        // var reader = new Reader.Models.Reader
        // {
        //     Id = Guid.NewGuid(),
        //     LocationId = location.Id,
        //     Name = "Library Main",
        //     CreatedAt = DateTime.UtcNow
        // };
        //
        // dbContext.Readers.Add(reader);
        //
        //
        // var location2 = new Reader.Models.Location()
        // {
        //     Id = Guid.NewGuid(),
        //     BuildingName = "Library",
        //     RoomName = "E-Lib",
        //     CreatedAt = DateTime.UtcNow
        // };
        //
        // dbContext.Locations.Add(location2);
        //
        // var reader2 = new Reader.Models.Reader
        // {
        //     Id = Guid.NewGuid(),
        //     LocationId = location.Id,
        //     Name = "E-Library",
        //     CreatedAt = DateTime.UtcNow
        // };
        //
        // dbContext.Readers.Add(reader2);
        //
        // var department = new Department
        // {
        //     Id = Guid.NewGuid(),
        //     CreatedAt = DateTime.UtcNow,
        //     Name = "SHS"
        // };
        //
        // dbContext.Departments.Add(department);
        //
        // var user = new Gringotts.Api.Features.User.Models.User
        // {
        //     Id = Guid.NewGuid(),
        //     CreatedAt = DateTime.UtcNow,
        //     AccessExpiry = DateTime.UtcNow.AddDays(7),
        //     CardId = "3871941748",
        //     SchoolId = "123456",
        //     FirstName = "Lemuel John",
        //     MiddleName = "L.",
        //     LastName = "Rondulla",
        //     Affiliation = 0,
        //     Sex = 0,
        //     DepartmentId = department.Id
        // };
        //
        // dbContext.LogUsers.Add(user);
        //
        // var user2 = new Gringotts.Api.Features.User.Models.User
        // {
        //     Id = Guid.NewGuid(),
        //     CreatedAt = DateTime.UtcNow,
        //     AccessExpiry = DateTime.UtcNow.AddDays(7),
        //     CardId = "3871883748",
        //     SchoolId = "234567",
        //     FirstName = "John Erick",
        //     MiddleName = "M.",
        //     LastName = "Reyes",
        //     Affiliation = 0,
        //     Sex = 0,
        //     DepartmentId = department.Id
        // };
        //
        // dbContext.LogUsers.Add(user2);
        //
        // var user3 = new Gringotts.Api.Features.User.Models.User
        // {
        //     Id = Guid.NewGuid(),
        //     CreatedAt = DateTime.UtcNow,
        //     AccessExpiry = DateTime.UtcNow.AddDays(7),
        //     CardId = "3870052644",
        //     SchoolId = "345678",
        //     FirstName = "Peter Jairus",
        //     MiddleName = "N.",
        //     LastName = "Tolentino",
        //     Affiliation = 0,
        //     Sex = 0,
        //     DepartmentId = department.Id
        // };
        //
        // dbContext.LogUsers.Add(user3);
        //
        // var user4 = new Gringotts.Api.Features.User.Models.User
        // {
        //     Id = Guid.NewGuid(),
        //     CreatedAt = DateTime.UtcNow,
        //     AccessExpiry = DateTime.UtcNow.AddDays(7),
        //     CardId = "3870612740",
        //     SchoolId = "456789",
        //     FirstName = "Andrei John",
        //     MiddleName = "P.",
        //     LastName = "Sumilang",
        //     Affiliation = 0,
        //     Sex = 0,
        //     DepartmentId = department.Id
        // };
        //
        // dbContext.LogUsers.Add(user4);

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