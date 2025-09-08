using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Product.API.Contracts.Products;
using Product.API.Controllers;
using Product.Application.Products.Commands;
using AutoMapper;
using Identity.API.Contracts.Products;
using Product.Domain.Products;
using Xunit;

public class ProductControllerTests
{
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<CreateProduct.IHandler> _createProductHandlerMock = new();
    private readonly Mock<UpdateProduct.IHandler> _updateProductHandlerMock = new();
    private readonly Mock<UpdateStock.IHandler> _updateStockHandlerMock = new();
    private readonly Mock<DecreaseStock.IHandler> _decreaseStockHandlerMock = new();
    private readonly Mock<GetProduct.IHandler> _getProductHandlerMock = new();
    private readonly Mock<DeleteProduct.IHandler> _deleteProductHandlerMock = new();
    private readonly Mock<ListProduct.IHandler> _listProductHandlerMock = new();

    private readonly Mock<IValidator<CreateProduct.Command>> _createValidatorMock = new();
    private readonly Mock<IValidator<UpdateStock.Command>> _updateStockValidatorMock = new();
    private readonly Mock<IValidator<DecreaseStock.Command>> _decreaseStockValidatorMock = new();
    private readonly Mock<IValidator<UpdateProduct.Command>> _updateProductValidatorMock = new();
    private readonly Mock<IValidator<GetProduct.Command>> _getProductValidatorMock = new();
    private readonly Mock<IValidator<DeleteProduct.Command>> _deleteProductValidatorMock = new();

    private ProductsController CreateController()
    {
        return new ProductsController(
            _mapperMock.Object,
            _createProductHandlerMock.Object,
            _updateProductHandlerMock.Object,
            _updateStockHandlerMock.Object,
            _decreaseStockHandlerMock.Object,
            _getProductHandlerMock.Object,
            _deleteProductHandlerMock.Object,
            _listProductHandlerMock.Object,
            _createValidatorMock.Object,
            _updateStockValidatorMock.Object,
            _decreaseStockValidatorMock.Object,
            _updateProductValidatorMock.Object,
            _getProductValidatorMock.Object,
            _deleteProductValidatorMock.Object
        );
    }

    #region Create Endpoint

    [Fact]
    public async Task Create_ShouldReturnCreated_WhenRequestIsValid()
    {
        // Arrange
        var request = new CreateProductRequest(
            "Keyboard",
            "Mechanical keyboard",
            199.90m,
            10
        );

        var domainProduct = new ProductModel();
        var expectedResponse = new ProductResponse
        (Guid.NewGuid(),
            request.Name,
            request.Description,
            request.Price,
            request.Stock);

        _createValidatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<CreateProduct.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _createProductHandlerMock
            .Setup(h => h.Handle(It.IsAny<CreateProduct.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(domainProduct);

        _mapperMock
            .Setup(m => m.Map<ProductResponse>(domainProduct))
            .Returns(expectedResponse);

        var controller = CreateController();

        // Act
        var result = await controller.Create(request, CancellationToken.None);

        // Assert
        var created = result.Should().BeOfType<CreatedAtActionResult>().Subject;
        created.ActionName.Should().Be(nameof(ProductsController.GetById));

        var response = created.Value.Should().BeOfType<ProductResponse>().Subject;
        response.Id.Should().Be(expectedResponse.Id);
        response.Name.Should().Be(request.Name);
        response.Description.Should().Be(request.Description);
        response.Price.Should().Be(request.Price);
        response.Stock.Should().Be(request.Stock);
    }

    [Fact]
    public async Task Create_ShouldReturnBadRequest_WhenValidationFails()
    {
        // Arrange
        var request = new CreateProductRequest(
            "",
            "",
            -10m,
            -1
        );

        var failures = new List<ValidationFailure>
        {
            new ValidationFailure("Name", "Name is required"),
            new ValidationFailure("Price", "Price must be greater than zero"),
            new ValidationFailure("Stock", "Stock must be non-negative")
        };

        _createValidatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<CreateProduct.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(failures));

        var controller = CreateController();

        // Act
        var result = await controller.Create(request, CancellationToken.None);

        // Assert
        var badRequest = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        var errors = badRequest.Value.Should().BeAssignableTo<Dictionary<string, string[]>>().Subject;
        errors.Should().ContainKey("Name");
        errors.Should().ContainKey("Price");
        errors.Should().ContainKey("Stock");
    }

    #endregion

    #region GetById Endpoint

    [Fact]
    public async Task GetById_ShouldReturnOk_WhenProductExists()
    {
        // Arrange
        var productId = Guid.NewGuid();

        var domainProduct = new ProductModel();
        var mappedResponse = new ProductResponse(productId,
            "Mouse",
            "Wireless",
            99.99m,
            5);

        _getProductValidatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<GetProduct.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _getProductHandlerMock
            .Setup(h => h.Handle(It.IsAny<GetProduct.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(domainProduct);

        _mapperMock
            .Setup(m => m.Map<ProductResponse>(domainProduct))
            .Returns(mappedResponse);

        var controller = CreateController();

        // Act
        var result = await controller.GetById(productId, CancellationToken.None);

        // Assert
        var ok = result.Should().BeOfType<OkObjectResult>().Subject;
        var response = ok.Value.Should().BeOfType<ProductResponse>().Subject;

        response.Id.Should().Be(productId);
        response.Name.Should().Be("Mouse");
        response.Description.Should().Be("Wireless");
        response.Price.Should().Be(99.99m);
        response.Stock.Should().Be(5);
    }

    [Fact]
    public async Task GetById_ShouldReturnBadRequest_WhenValidationFails()
    {
        // Arrange
        var productId = Guid.NewGuid();

        var failures = new List<ValidationFailure>
        {
            new ValidationFailure("Id", "Invalid product Id")
        };

        _getProductValidatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<GetProduct.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(failures));

        var controller = CreateController();

        // Act
        var result = await controller.GetById(productId, CancellationToken.None);

        // Assert
        var badRequest = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        var errors = badRequest.Value.Should().BeAssignableTo<Dictionary<string, string[]>>().Subject;
        errors.Should().ContainKey("Id");
    }

    [Fact]
    public async Task GetById_ShouldReturnNotFound_WhenProductDoesNotExist()
    {
        // Arrange
        var productId = Guid.NewGuid();

        _getProductValidatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<GetProduct.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _getProductHandlerMock
            .Setup(h => h.Handle(It.IsAny<GetProduct.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProductModel?)null);

        var controller = CreateController();

        // Act
        var result = await controller.GetById(productId, CancellationToken.None);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    #endregion
}