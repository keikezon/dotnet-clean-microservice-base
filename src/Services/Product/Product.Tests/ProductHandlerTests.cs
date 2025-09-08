using FluentAssertions;
using MassTransit;
using Moq;
using Product.Application.Contracts.Events;
using Product.Application.Products.Abstractions;
using Product.Application.Products.Commands;
using Product.Domain.Products;
using Xunit;

public class ProductHandlerTests
{
    private readonly Mock<IProductRepository> _repoMock = new();
    private readonly Mock<IPublishEndpoint> _busMock = new();

    #region Create
    [Fact]
    public async Task Handle_ShouldCreateProductAndPublishEvent()
    {
        // Arrange
        var cmd = new CreateProduct.Command(
            "Keyboard",
            "Mechanical keyboard",
            199.90m,
            10
        );

        ProductModel? addedProduct = null;

        _repoMock
            .Setup(r => r.AddAsync(It.IsAny<ProductModel>(), It.IsAny<CancellationToken>()))
            .Callback<ProductModel, CancellationToken>((p, _) => addedProduct = p)
            .Returns(Task.CompletedTask);

        _repoMock
            .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = new CreateProduct.Handler(_repoMock.Object, _busMock.Object);

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().NotBe(Guid.Empty);
        result.Name.Should().Be(cmd.Name);
        result.Description.Should().Be(cmd.Description);
        result.Price.Should().Be(cmd.Price);
        result.Stock.Should().Be(cmd.Stock);

        addedProduct.Should().NotBeNull();
        addedProduct!.Name.Should().Be(cmd.Name);
        addedProduct.Description.Should().Be(cmd.Description);
        addedProduct.Price.Should().Be(cmd.Price);
        addedProduct.Stock.Should().Be(cmd.Stock);

        _repoMock.Verify(r => r.AddAsync(It.IsAny<ProductModel>(), It.IsAny<CancellationToken>()), Times.Once);
        _repoMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _busMock.Verify(b => b.Publish(It.IsAny<ProductCreatedIntegrationEvent>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData("", "Desc", 10.0, 1)]      // Nome vazio
    [InlineData("Name", "Desc", -1.0, 1)]  // Preço inválido
    [InlineData("Name", "Desc", 10.0, -1)] // Stock inválido
    public async Task Handle_CreateShouldThrowArgumentException_WhenDataIsInvalid(string name, string description, decimal price, int stock)
    {
        // Arrange
        var cmd = new CreateProduct.Command(name, description, price, stock);
        var handler = new CreateProduct.Handler(_repoMock.Object, _busMock.Object);

        // Act
        var act = async () => await handler.Handle(cmd, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }
    #endregion

    #region GetById
    [Fact]
    public async Task Handle_ShouldGetProduct()
    {
        // Arrange
        var productId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var cmd = new GetProduct.Command(productId);

        var expected = new ProductModel(
            productId,
            "Mouse",
            "Wireless",
            99.99m,
            5
        );

        _repoMock
            .Setup(r => r.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var handler = new GetProduct.Handler(_repoMock.Object);

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(productId);
        result.Name.Should().Be("Mouse");
        result.Description.Should().Be("Wireless");
        result.Price.Should().Be(99.99m);
        result.Stock.Should().Be(5);

        _repoMock.Verify(r => r.GetByIdAsync(productId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData("")]  // Id vazio
    [InlineData("11111111-1111-1111-1111-111111111100")] // Id inválido
    [InlineData("11111111-1111-1111-1111-111111111111")] // Id válido
    public async Task Handle_GetShouldThrowArgumentException_WhenDataIsInvalid(string idString)
    {
        // Arrange
        if (!Guid.TryParse(idString, out var id) || id == Guid.Empty)
        {
            await FluentActions.Invoking(async () =>
            {
                var cmd = new GetProduct.Command(Guid.Empty);
                var handler = new GetProduct.Handler(_repoMock.Object);
                await handler.Handle(cmd, CancellationToken.None);
            }).Should().ThrowAsync<ArgumentException>();

            return;
        }

        // Id válido, mock retorna null para simular produto não encontrado
        _repoMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProductModel?)null);

        var cmdValid = new GetProduct.Command(id);
        var handlerValid = new GetProduct.Handler(_repoMock.Object);

        // Act
        var result = await handlerValid.Handle(cmdValid, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }
    #endregion
}