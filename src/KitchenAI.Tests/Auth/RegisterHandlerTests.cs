using KitchenAI.Application.Auth;
using KitchenAI.Application.Exceptions;
using KitchenAI.Application.Services;
using KitchenAI.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace KitchenAI.Tests.Auth;

public class RegisterHandlerTests
{
    private static AppDbContext CreateDb() =>
        new(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    private static Mock<ITokenService> TokenServiceMock()
    {
        var mock = new Mock<ITokenService>();
        mock.Setup(s => s.GenerateToken(It.IsAny<KitchenAI.Domain.Entities.User>(), It.IsAny<Guid>()))
            .Returns("test-token");
        return mock;
    }

    [Fact]
    public async Task ValidRegistration_CreatesUserAndHousehold()
    {
        // Arrange
        await using var db = CreateDb();
        var handler = new RegisterHandler(db, TokenServiceMock().Object);
        var command = new RegisterCommand("alice@example.com", "Password1!", "Alice");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal("test-token", result.Token);
        Assert.Equal("alice@example.com", result.Email);
        Assert.NotNull(result.HouseholdId);

        Assert.Equal(1, await db.Users.CountAsync());
        Assert.Equal(1, await db.Households.CountAsync());
        Assert.Equal(1, await db.HouseholdMembers.CountAsync());

        var member = await db.HouseholdMembers.FirstAsync();
        Assert.Equal("owner", member.Role);
    }

    [Fact]
    public async Task DuplicateEmail_ThrowsValidationException()
    {
        // Arrange
        await using var db = CreateDb();
        var handler = new RegisterHandler(db, TokenServiceMock().Object);
        var command = new RegisterCommand("bob@example.com", "Password1!", "Bob");

        await handler.Handle(command, CancellationToken.None);

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() =>
            handler.Handle(command, CancellationToken.None));
    }
}
