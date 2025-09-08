using Common.Enum;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Identity.API.Contracts.Users;
using Identity.API.Controllers;
using Identity.Application.Users.Commands;
using Identity.Domain.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

public class LoginControllerTests
{
    private readonly Mock<IConfiguration> _configMock = new();
    private readonly Mock<LoginUser.IHandler> _loginUserMock = new();
    private readonly Mock<IValidator<LoginUser.Command>> _validatorMock = new();
    private readonly Pbkdf2PasswordHasher _hasher = new();

    private LoginController LoginController() =>
        new LoginController(_configMock.Object, _loginUserMock.Object, _validatorMock.Object);

    [Fact]
    public async Task Login_ShouldReturnOk_WithToken_WhenValidCredentials()
    {
        // Arrange
        var hasher = new Pbkdf2PasswordHasher();
        var password = "secret123";
        var user = new User(
            Guid.NewGuid(),
            "John Doe",
            "john@example.com",
            "Admin",
            hasher.Hash(password));

        _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<LoginUser.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        var fakeToken = "fake-jwt-token";
        _loginUserMock.Setup(h => h.Handle(It.IsAny<LoginUser.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fakeToken);

        _configMock.Setup(c => c.GetSection("Jwt")["Key"]).Returns("m5UMQ+t/NN8XIi4dO/wCpQMJtBLkRbT6/R4AnEsvU58=");
        _configMock.Setup(c => c.GetSection("Jwt")["Issuer"]).Returns("api");
        _configMock.Setup(c => c.GetSection("Jwt")["Audience"]).Returns("api-service");
        _configMock.Setup(c => c.GetSection("Jwt")["ExpireMinutes"]).Returns("60");

        var controller = LoginController();
        var request = new LoginRequest(user.Email, password);

        // Act
        var result = await controller.Login(request, CancellationToken.None);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var loginResponse = okResult.Value.Should().BeOfType<LoginResponse>().Subject;
        loginResponse.token.Should().NotBeNullOrEmpty();
        loginResponse.token.Should().Be(fakeToken);
    }

    [Fact]
    public async Task Login_ShouldReturnBadRequest_WhenValidationFails()
    {
        // Arrange
        _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<LoginUser.Command>(), It.IsAny<CancellationToken>()))
                      .ReturnsAsync(new FluentValidation.Results.ValidationResult(new[]
                      {
                          new FluentValidation.Results.ValidationFailure("Email", "Required")
                      }));

        var controller = LoginController();
        var request = new LoginRequest("", "password");

        // Act
        var result = await controller.Login(request, CancellationToken.None);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Login_ShouldReturnUnauthorized_WhenUserDoesNotExist()
    {
        // Arrange
        _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<LoginUser.Command>(), It.IsAny<CancellationToken>()))
                      .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        _loginUserMock.Setup(h => h.Handle(It.IsAny<LoginUser.Command>(), It.IsAny<CancellationToken>()))
                      .ReturnsAsync((string?)null);

        var controller = LoginController();
        var request = new LoginRequest("notfound@example.com", "password");

        // Act
        var result = await controller.Login(request, CancellationToken.None);

        // Assert
        var unauthorizedResult = result.Should().BeOfType<UnauthorizedObjectResult>().Subject;
        unauthorizedResult.Value.Should().Be("Invalid credentials.");
    }
}