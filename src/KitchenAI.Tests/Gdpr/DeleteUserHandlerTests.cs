using KitchenAI.Application.Gdpr;
using KitchenAI.Domain.Entities;
using KitchenAI.Infrastructure.Persistence;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace KitchenAI.Tests.Gdpr;

public class DeleteUserHandlerTests : IDisposable
{
    private readonly SqliteConnection _connection;

    public DeleteUserHandlerTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();
        using var ctx = CreateDb();
        ctx.Database.EnsureCreated();
    }

    public void Dispose() => _connection.Dispose();

    private AppDbContext CreateDb() =>
        new(new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options);

    [Fact]
    public async Task ValidUser_DeletesUserAndHousehold()
    {
        await using var db = CreateDb();

        var userId = Guid.NewGuid();
        var householdId = Guid.NewGuid();

        db.Users.Add(new User
        {
            Id = userId,
            Email = "eve@example.com",
            PasswordHash = "hash",
            DisplayName = "Eve",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });
        db.Households.Add(new Household
        {
            Id = householdId,
            Name = "Eve's Home",
            OwnerUserId = userId,
            CreatedAt = DateTime.UtcNow
        });
        await db.SaveChangesAsync();

        var handler = new DeleteUserHandler(db, NullLogger<DeleteUserHandler>.Instance);

        await handler.Handle(new DeleteUserCommand(userId), CancellationToken.None);

        Assert.Equal(0, await db.Users.CountAsync());
        Assert.Equal(0, await db.Households.CountAsync());
    }

    [Fact]
    public async Task UnknownUser_ThrowsKeyNotFoundException()
    {
        await using var db = CreateDb();

        var handler = new DeleteUserHandler(db, NullLogger<DeleteUserHandler>.Instance);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            handler.Handle(new DeleteUserCommand(Guid.NewGuid()), CancellationToken.None));
    }
}
