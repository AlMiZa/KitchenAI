using System.Globalization;
using KitchenAI.Application.Auth;
using KitchenAI.Application.Exceptions;
using KitchenAI.Application.Services;
using KitchenAI.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace KitchenAI.Tests.Localization;

/// <summary>Verifies that handler error messages are returned in the correct language.</summary>
public class LocalizationHandlerTests
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
    public async Task RegisterHandler_DuplicateEmail_EnglishCulture_ReturnsEnglishErrorMessage()
    {
        await using var db = CreateDb();
        var handler = new RegisterHandler(db, TokenServiceMock().Object);
        var command = new RegisterCommand("dup@example.com", "Password1!", "Dup");
        await handler.Handle(command, CancellationToken.None);

        var previous = CultureInfo.CurrentUICulture;
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        try
        {
            var ex = await Assert.ThrowsAsync<ValidationException>(() =>
                handler.Handle(command, CancellationToken.None));
            Assert.Equal("Email already in use", ex.Message);
        }
        finally
        {
            CultureInfo.CurrentUICulture = previous;
        }
    }

    [Fact]
    public async Task RegisterHandler_DuplicateEmail_PolishCulture_ReturnsPolishErrorMessage()
    {
        await using var db = CreateDb();
        var handler = new RegisterHandler(db, TokenServiceMock().Object);
        var command = new RegisterCommand("dup2@example.com", "Password1!", "Dup");
        await handler.Handle(command, CancellationToken.None);

        var previous = CultureInfo.CurrentUICulture;
        CultureInfo.CurrentUICulture = new CultureInfo("pl-PL");
        try
        {
            var ex = await Assert.ThrowsAsync<ValidationException>(() =>
                handler.Handle(command, CancellationToken.None));
            Assert.Equal("Email jest już w użyciu", ex.Message);
        }
        finally
        {
            CultureInfo.CurrentUICulture = previous;
        }
    }
}
