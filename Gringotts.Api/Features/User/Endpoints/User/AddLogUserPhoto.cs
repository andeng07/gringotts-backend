using Gringotts.Api.Shared.Core;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;

namespace Gringotts.Api.Features.User.Endpoints.User;

public class AddLogUserPhoto : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("users/{userId:guid}/photo", async (
                [FromRoute] Guid userId,
                HttpRequest request
            ) =>
            {
                try
                {
                    using var memoryStream = new MemoryStream();
                    await request.Body.CopyToAsync(memoryStream);
                    memoryStream.Seek(0, SeekOrigin.Begin); // ✅ Ensure correct stream position

                    using var image = Image.Load(memoryStream); // ✅ Properly load image

                    // Define the directory and file path
                    var userDirectory = Path.Combine("wwwroot", "users");
                    if (!Directory.Exists(userDirectory))
                    {
                        Directory.CreateDirectory(userDirectory);
                    }

                    var filePath = Path.Combine(userDirectory, $"{userId}.png");

                    // ✅ Convert and save as PNG
                    await image.SaveAsync(filePath, new PngEncoder());

                    return Results.Ok(new { FilePath = filePath, UserId = userId });
                }
                catch (Exception ex)
                {
                    return Results.BadRequest($"Invalid image format: {ex.Message}");
                }
            })
            .Accepts<object>("application/octet-stream"); // ✅ Expect raw binary data
    }
}