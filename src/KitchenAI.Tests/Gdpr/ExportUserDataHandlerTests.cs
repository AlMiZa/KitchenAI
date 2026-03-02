using KitchenAI.Application.Gdpr;
using KitchenAI.Domain.Entities;
using KitchenAI.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace KitchenAI.Tests.Gdpr;

public class ExportUserDataHandlerTests
{
    private static AppDbContext CreateDb() =>
        new(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    [Fact]
    public async Task ValidUser_ReturnsJsonContainingEmail()
    {
        await using var db = CreateDb();

        var userId = Guid.NewGuid();
        db.Users.Add(new User
        {
            Id = userId,
            Email = "frank@example.com",
            PasswordHash = "hash",
            DisplayName = "Frank",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });
        await db.SaveChangesAsync();

        var handler = new ExportUserDataHandler(db);
        var result = await handler.Handle(new ExportUserDataQuery(userId), CancellationToken.None);

        Assert.Contains("frank@example.com", result);
    }

    [Fact]
    public async Task UnknownUser_ThrowsKeyNotFoundException()
    {
        await using var db = CreateDb();

        var handler = new ExportUserDataHandler(db);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            handler.Handle(new ExportUserDataQuery(Guid.NewGuid()), CancellationToken.None));
    }
}
