using Common.Enum;
using FluentAssertions;
using Identity.Application.Users.Abstractions;
using Identity.Application.Users.Commands;
using Identity.Contracts.Events;
using Identity.Domain.Users;
using MassTransit;
using Moq;
using Xunit;

public class UserHandlerTests
{
    private readonly Mock<IUserRepository> _repoMock = new();
    private readonly Mock<IPasswordHasher> _hasherMock = new();
    private readonly Mock<IPublishEndpoint> _busMock = new();

    #region Create
    [Fact]
    public async Task Handle_ShouldCreateUserAndPublishEvent()
    {
        // Arrange
        var cmd = new CreateUser.Command("John Doe", "john@example.com", "123456", UserProfile.Seller);

        _hasherMock
            .Setup(h => h.Hash(It.IsAny<string>()))
            .Returns("hashed-password");

        var handler = new CreateUser.Handler(_repoMock.Object, _hasherMock.Object, _busMock.Object);

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        result.Should().NotBeEmpty();

        _repoMock.Verify(r => r.AddAsync(It.Is<User>(u =>
            u.Name == "John Doe" &&
            u.Email == "john@example.com" &&
            u.PasswordHash == "hashed-password" &&
            u.Profile == UserProfile.Seller.ToString()
        ), It.IsAny<CancellationToken>()), Times.Once);

        _repoMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _busMock.Verify(b => b.Publish(It.IsAny<UserCreatedIntegrationEvent>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData("", "john@example.com", "123456")]  // Nome vazio
    [InlineData("John", "invalid-email", "123456")] // Email inválido
    [InlineData("John", "john@example.com", "")]    // Senha vazia
    public async Task Handle_CreateShouldThrowArgumentException_WhenDataIsInvalid(string name, string email, string password)
    {
        // Arrange
        var cmd = new CreateUser.Command(name, email, password, UserProfile.Seller);

        _hasherMock
            .Setup(h => h.Hash(It.IsAny<string>()))
            .Returns("hashed-password");

        var handler = new CreateUser.Handler(_repoMock.Object, _hasherMock.Object, _busMock.Object);

        // Act
        var act = async () => await handler.Handle(cmd, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }
    
    #endregion
    
    #region GetById
    [Fact]
    public async Task Handle_ShouldGetUser()
    {
        var userId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var cmd = new GetUser.Command(userId);

        var expectedUser = new User(
            userId,
            "Jane Doe",
            "jane@example.com",
            "hashed-password",
            UserProfile.Admin.ToString()
        );

        _repoMock.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedUser);
        
        var handler = new GetUser.Handler(_repoMock.Object);

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(userId);
        result.Name.Should().Be("Jane Doe");
        result.Email.Should().Be("jane@example.com");
        result.Profile.Should().Be(UserProfile.Admin.ToString());

        _repoMock.Verify(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData("")]  // Id vazio
    [InlineData("11111111-1111-1111-1111-111111111100")] // Id inválido
    [InlineData("11111111-1111-1111-1111-111111111111")] // Id válido
    public async Task Handle_GetShouldThrowArgumentException_WhenDataIsInvalid2(string idString)
    {
        // Arrange
        Guid id;

        // Tenta converter, lança ArgumentException se inválido
        if (!Guid.TryParse(idString, out id))
        {
            await FluentActions.Invoking(async () =>
            {
                var cmd = new GetUser.Command(Guid.Empty);
                var handler = new GetUser.Handler(_repoMock.Object);
                await handler.Handle(cmd, CancellationToken.None);
            }).Should().ThrowAsync<ArgumentException>();

            return;
        }

        // Id válido, mock retorna null para simular usuário não encontrado
        _repoMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var cmdValid = new GetUser.Command(id);
        var handlerValid = new GetUser.Handler(_repoMock.Object);

        // Act
        var result = await handlerValid.Handle(cmdValid, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    #endregion
}