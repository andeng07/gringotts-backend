using Gringotts.Api.Features.Client.Services;
using Gringotts.Api.Features.ClientAuthentication.Services;
using Gringotts.Api.Features.Reader.Models;
using Gringotts.Api.Features.User.Models;
using Gringotts.Api.Shared.Core;

namespace Gringotts.Api.Features.Populator;

public class PopulatorService(ClientService clientService, ClientSecretService clientSecretService, AppDbContext dbContext)
{
    // pre-defines the necessary Admin, Department, Location, Readers
    public async Task PopulateDatabaseAsync()
    {
        // ADMIN ACCOUNT
        var client = await clientService.CreateUserAsync(
            "University Library", "Southern Luzon State University"
        );

        var clientSecret = await clientSecretService.CreateSecretAsync(
            client.Value!.Id, "slsu.lib.admin", "slsul!b@dmin"
        );

        // DEPARTMENTS
        string[] departments =
        [
            "Office of the President",
            "Legal Counsel",
            "MIS-ICT Office",
            "Information and Communications Office",
            "University and Board Secretary",
            "International Affairs and External Linkages Office",
            "Internal Audit Office",
            "Quality Assurance Office",
            "Planning Office",
            "Project Management Office",
            "External Resource Linkages Office",

            "Office of the Vice President for AA",
            "Office of Instruction",
            "Office of Student Affairs Services",
            "Guidance Office",
            "Student Admission Office",
            "University Registrar's Office",
            "University Library",
            "National Service Training Program",

            "Graduate School",
            "College of Medicine",
            "College of Administration, Business, Hospitality, and Accountancy",
            "College of Agriculture",
            "College of Allied Medicine",
            "College of Arts and Sciences",
            "College of Engineering",
            "College of Industrial Technology",
            "College of Teacher Education",
            "Institute of Human Kinetics",
            "Laboratory School",

            "Office of the Vice President for AFA",
            "Chief Administrative Officer",
            "Human Resource Management Office",
            "Faculty and Staff Development Program",
            "Records Management Office",
            "General Services Office",
            "Procurement Office",
            "Supply & Property Office",
            "University Health Services",
            "Civil Safety and Security Unit",
            "Disaster Risk Reduction & Management Coordinating Office",
            "Accounting Office",
            "Budget Office",
            "Cashier's Office",
            "Business Affairs Office",
            "SLSU Hotel"
        ];

        var departmentModels = departments.Select(department => new Department
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                Name = department
            }
        );
        
        dbContext.Departments.AddRange(departmentModels);
        
        string[] locations =
        [
            "General Circulation Section",
            "Technical Section",
            "Learning Commons",
            "Periodical and General Reference Section",
            "E-Library",
            "Thesis Section",
            "College of Medicine LRC"
        ];

        var locationModels = locations.Select(location => new Location
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                BuildingName = "University Library",
                RoomName = location
            }
        ).ToArray();
        
        dbContext.Locations.AddRange(locationModels);

        var readers = locationModels.Select((location, index) => new Reader.Models.Reader
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            Name = $"Reader {index}",
            LocationId = location.Id
        });
        
        dbContext.Readers.AddRange(readers);

        await dbContext.SaveChangesAsync();
    }
}