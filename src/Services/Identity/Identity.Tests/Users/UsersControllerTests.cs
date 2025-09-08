using Common.Enum;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Identity.API.Contracts.Users;
using Identity.API.Controllers;
using Identity.Application.Users.Commands;
using Identity.Domain.Users;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

public class UsersControllerTests
{
    private readonly Mock<CreateUser.IHandler> _createUserHandlerMock = new();
    private readonly Mock<GetUser.IHandler> _getUserHandlerMock = new();
    private readonly Mock<IValidator<CreateUser.Command>> _createUserValidatorMock = new();
    private readonly Mock<IValidator<GetUser.Command>> _getUserValidatorMock = new();

    private UsersController CreateController()
    {
        return new UsersController(
            _createUserHandlerMock.Object,
            _createUserValidatorMock.Object,
            _getUserHandlerMock.Object,
            _getUserValidatorMock.Object
        );
    }

    #region Create Endpoint

    [Fact]
    public async Task Create_ShouldReturnCreated_WhenRequestIsValid()
    {
        // Arrange
        var request = new CreateUserRequest(
            "John Doe",
            "john@example.com",
            "securePass",
            UserProfile.Seller
        );

        var expectedId = Guid.NewGuid();

        _createUserValidatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<CreateUser.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _createUserHandlerMock
            .Setup(h => h.Handle(It.IsAny<CreateUser.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedId);

        var controller = CreateController();

        // Act
        var result = await controller.Create(request, CancellationToken.None);

        // Assert
        var createdResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;

        createdResult.ActionName.Should().Be(nameof(UsersController.GetById));

        var response = createdResult.Value.Should().BeOfType<UserResponse>().Subject;
        response.Id.Should().Be(expectedId);
        response.Name.Should().Be(request.Name);
        response.Email.Should().Be(request.Email);
        response.Profile.Should().Be(UserProfile.Seller);
    }

    [Fact]
    public async Task Create_ShouldReturnBadRequest_WhenValidationFails()
    {
        // Arrange
        var request = new CreateUserRequest(
            "",
            "invalid-email",
            "",
            UserProfile.Seller
        );

        var validationFailures = new List<ValidationFailure>
        {
            new ValidationFailure("Email", "Invalid email"),
            new ValidationFailure("Password", "Password is required")
        };

        _createUserValidatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<CreateUser.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(validationFailures));

        var controller = CreateController();

        // Act
        var result = await controller.Create(request, CancellationToken.None);

        // Assert
        var badRequest = result.Should().BeOfType<BadRequestObjectResult>().Subject;

        var errors = badRequest.Value.Should().BeAssignableTo<Dictionary<string, string[]>>().Subject;
        errors.Should().ContainKey("Email");
        errors.Should().ContainKey("Password");
    }

    #endregion

    #region GetById Endpoint

    [Fact]
    public async Task GetById_ShouldReturnOk_WhenUserExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var mockCreateUser = new Mock<CreateUser.IHandler>();
        var mockValidatorCreate = new Mock<IValidator<CreateUser.Command>>();
        var mockGetUser = new Mock<GetUser.IHandler>();
        var mockValidatorGet = new Mock<IValidator<GetUser.Command>>();

        // Configura o validador para sempre retornar válido
        mockValidatorGet
            .Setup(v => v.ValidateAsync(It.IsAny<GetUser.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        // Cria um usuário de domínio
        var domainUser = new User
        (
            userId,
            "Jane Doe",
            "jane@example.com",
            "hash",
            UserProfile.Admin.ToString()
        );

        // Configura o mock para retornar esse usuário
        mockGetUser
            .Setup(h => h.Handle(It.IsAny<GetUser.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(domainUser);

        var controller = new UsersController(
            mockCreateUser.Object,
            mockValidatorCreate.Object,
            mockGetUser.Object,
            mockValidatorGet.Object
        );

        // Act
        var result = await controller.GetById(userId, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<UserResponse>(okResult.Value);

        Assert.Equal(userId, response.Id);
        Assert.Equal("Jane Doe", response.Name);
        Assert.Equal("jane@example.com", response.Email);
        Assert.Equal(UserProfile.Admin, response.Profile);
    }

    [Fact]
    public async Task GetById_ShouldReturnBadRequest_WhenValidationFails()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var validationFailures = new List<ValidationFailure>
        {
            new ValidationFailure("Id", "Invalid user Id")
        };

        _getUserValidatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<GetUser.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(validationFailures));

        var controller = CreateController();

        // Act
        var result = await controller.GetById(userId, CancellationToken.None);

        // Assert
        var badRequest = result.Should().BeOfType<BadRequestObjectResult>().Subject;

        var errors = badRequest.Value.Should().BeAssignableTo<Dictionary<string, string[]>>().Subject;
        errors.Should().ContainKey("Id");
    }

    [Fact]
    public async Task GetById_ShouldReturnNotFound_WhenUserDoesNotExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var mockCreateUser = new Mock<CreateUser.IHandler>();
        var mockValidatorCreate = new Mock<IValidator<CreateUser.Command>>();
        var mockGetUser = new Mock<GetUser.IHandler>();
        var mockValidatorGet = new Mock<IValidator<GetUser.Command>>();

        // Validador sempre retorna válido
        mockValidatorGet
            .Setup(v => v.ValidateAsync(It.IsAny<GetUser.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        // Mock retorna null, simulando usuário não encontrado
        mockGetUser
            .Setup(h => h.Handle(It.IsAny<GetUser.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var controller = new UsersController(
            mockCreateUser.Object,
            mockValidatorCreate.Object,
            mockGetUser.Object,
            mockValidatorGet.Object
        );

        // Act
        var result = await controller.GetById(userId, CancellationToken.None);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    #endregion
}