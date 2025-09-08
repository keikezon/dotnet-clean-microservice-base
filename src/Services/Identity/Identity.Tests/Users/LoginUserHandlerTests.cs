using Common.Enum;
using FluentAssertions;
using Identity.Application.Users.Abstractions;
using Identity.Application.Users.Commands;
using Identity.Contracts.Events;
using Identity.Domain.Users;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

public class LoginUserHandlerTests
{
    private readonly Mock<IUserRepository> _repoMock = new();
    private readonly Mock<IPasswordHasher> _hasherMock = new();
    private readonly Mock<IPublishEndpoint> _busMock = new();
    private readonly Mock<IConfiguration> _configMock = new();

    [Fact]
    public async Task Handle_ShouldLoginUserAndPublishEvent()
    {
        // Arrange
        var cmd = new LoginUser.Command("admin@email.com", "admin@123");
    
        var expectedUser = new User(
            Guid.Parse("11111111-1111-1111-1111-111111111111"),
            "Admin",
            "admin@email.com",
            "pbkdf2-256|mPafq1CXaNWHpPW3GGnhcg==|pU7K+StmlgzreQOEh1tf2AMTNH0GLj3OYTW8OTET1Go=",
            UserProfile.Admin.ToString()
        );

        // Mock do repositório retorna o usuário esperado
        _repoMock.Setup(r => r.LoginAsync(It.IsAny<Login>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedUser);


        // Mock do hasher para simular verificação de senha
        _hasherMock.Setup(h => h.Hash(It.IsAny<string>()))
            .Returns(expectedUser.PasswordHash);


        // Mock da configuração JWT
        _configMock.Setup(c => c["Jwt:Key"]).Returns("m5UMQ+t/NN8XIi4dO/wCpQMJtBLkRbT6/R4AnEsvU58=");
        _configMock.Setup(c => c["Jwt:Issuer"]).Returns("api");
        _configMock.Setup(c => c["Jwt:Audience"]).Returns("api-service");
        _configMock.Setup(c => c["Jwt:ExpireMinutes"]).Returns("60");

        var handler = new LoginUser.Handler(_repoMock.Object, _hasherMock.Object, _busMock.Object, _configMock.Object);

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        result.Should().NotBeNullOrEmpty(); // Token gerado não pode ser nulo ou vazio
        result.Should().Contain("."); // Opcional: verificar se parece com JWT

        // Verifica se o evento foi publicado
        _busMock.Verify(b => b.Publish(It.IsAny<UserLoginIntegrationEvent>(), It.IsAny<CancellationToken>()), Times.Once);
    }


    [Theory]
    [InlineData("admin@email.com", "123")]  // Senha inválida
    [InlineData( "admin@email.com", "")]    // Senha vazia
    public async Task Handle_ShouldThrowArgumentException_WhenDataIsInvalid(string email, string password)
    {
        // Arrange
        var cmd = new LoginUser.Command(email, password);
        var userId = Guid.Parse("11111111-1111-1111-1111-111111111111");

        var expectedUser = new User(
            userId,
            "Admin",
            "admin@email.com",
            "hashed-password",
            UserProfile.Admin.ToString()
        );

        _repoMock.Setup(r => r.LoginAsync(It.Is<Login>(l =>
                l.Email == cmd.Email &&
                l.Password == cmd.Password
            ), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedUser);


        var handler = new LoginUser.Handler(_repoMock.Object, _hasherMock.Object, _busMock.Object, _configMock.Object);

        // Act
        var result = async () => await handler.Handle(cmd, CancellationToken.None);

        // Assert
        await result.Should().ThrowAsync<ArgumentException>();
    }
}