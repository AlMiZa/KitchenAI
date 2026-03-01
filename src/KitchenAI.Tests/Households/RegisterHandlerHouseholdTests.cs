using KitchenAI.Application.Auth;
using KitchenAI.Application.Services;
using KitchenAI.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace KitchenAI.Tests.Households;

public class RegisterHandlerHouseholdTests
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
    public async Task UserLinkedToHouseholdOnRegistration()
    {
        // Arrange
        await using var db = CreateDb();
        var handler = new RegisterHandler(db, TokenServiceMock().Object);
        var command = new RegisterCommand("charlie@example.com", "Password1!", "Charlie");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert – user is linked to their new household via HouseholdMember
        var user = await db.Users.FirstAsync();
        var household = await db.Households.FirstAsync();
        var member = await db.HouseholdMembers.FirstAsync();

        Assert.Equal(user.Id, household.OwnerUserId);
        Assert.Equal(user.Id, member.UserId);
        Assert.Equal(household.Id, member.HouseholdId);
        Assert.Equal("owner", member.Role);
        Assert.Equal(result.HouseholdId, household.Id);
    }
}
