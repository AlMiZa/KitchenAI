using KitchenAI.Application.Auth;
using KitchenAI.Application.Services;
using KitchenAI.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace KitchenAI.Tests.Auth;

public class PasswordlessRequestHandlerTests
{
    private static AppDbContext CreateDb() =>
        new(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    [Fact]
    public async Task ValidEmail_StoresTokenAndTriggersEmail()
    {
        await using var db = CreateDb();
        var emailMock = new Mock<IEmailService>();
        var handler = new PasswordlessRequestHandler(db, emailMock.Object, NullLogger<PasswordlessRequestHandler>.Instance);
        var command = new PasswordlessRequestCommand("diana@example.com");

        await handler.Handle(command, CancellationToken.None);

        Assert.Equal(1, await db.MagicLinkTokens.CountAsync());

        var stored = await db.MagicLinkTokens.FirstAsync();
        Assert.Equal("diana@example.com", stored.Email);
        Assert.False(stored.IsUsed);
        Assert.True(stored.ExpiresAt > DateTime.UtcNow);

        emailMock.Verify(
            s => s.SendMagicLinkAsync("diana@example.com", stored.Token, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task EmptyEmail_ThrowsArgumentException()
    {
        await using var db = CreateDb();
        var handler = new PasswordlessRequestHandler(db, new Mock<IEmailService>().Object, NullLogger<PasswordlessRequestHandler>.Instance);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            handler.Handle(new PasswordlessRequestCommand("   "), CancellationToken.None));
    }
}
