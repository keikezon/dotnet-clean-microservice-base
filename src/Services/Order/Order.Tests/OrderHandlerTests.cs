using FluentAssertions;
using MassTransit;
using Moq;
using Order.API.Contracts.Orders;
using Order.Application.Orders.Abstractions;
using Order.Application.Orders.Commands;
using Order.Contracts.Events;
using Order.Domain.Orders;
using Xunit;

public class OrderHandlerTests
{
    private readonly Mock<IOrderRepository> _repoMock = new();
    private readonly Mock<IPublishEndpoint> _busMock = new();

    // ... existing code ...
    [Fact]
    public async Task Handle_ShouldCreateOrderAndPublishEvent()
    {
        // Arrange
        // Cria um "draft" de OrderModel usando o ctor não público para evitar depender de detalhes de implementação
        var draftOrder = (OrderModel)Activator.CreateInstance(typeof(OrderModel), nonPublic: true)!;

        OrderModel? addedOrder = null;

        _repoMock
            .Setup(r => r.AddAsync(It.IsAny<OrderModel>(), It.IsAny<CancellationToken>()))
            .Callback<OrderModel, CancellationToken>((o, _) => addedOrder = o)
            .Returns(Task.CompletedTask);

        _repoMock
            .Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Simula que após salvar, o repositório retorna a ordem persistida
        var persistedOrder = (OrderModel)Activator.CreateInstance(typeof(OrderModel), nonPublic: true)!;
        _repoMock
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(persistedOrder);

        var handler = new CreateOrder.Handler(_repoMock.Object, _busMock.Object);
        var items = new List<OrderItemModel>() {
            new OrderItemModel(Guid.NewGuid(), Guid.NewGuid(), Guid.Parse("11111111-1111-1111-1111-111111111111"), 1, 10)
        };
        var request = new OrderModel(Guid.NewGuid(), Guid.Parse("11111111-1111-1111-1111-111111111111"),"name", "Test Order", items, 10);
        
        var cmd = new CreateOrder.Command(request);

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeSameAs(persistedOrder);

        addedOrder.Should().NotBeNull();

        _repoMock.Verify(r => r.AddAsync(It.IsAny<OrderModel>(), It.IsAny<CancellationToken>()), Times.Once);
        _repoMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _repoMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
        _busMock.Verify(b => b.Publish(It.IsAny<OrderCreatedIntegrationEvent>(), It.IsAny<CancellationToken>()), Times.Once);
    }
    // ... existing code ...

    // ... existing code ...
    [Fact]
    public async Task Handle_CreateShouldThrowArgumentException_WhenDataIsInvalid()
    {
        // Arrange
        // Usa uma instância "vazia" para forçar validação falhar dentro de OrderModel.Create(...)
        var invalidDraft = (OrderModel)Activator.CreateInstance(typeof(OrderModel), nonPublic: true)!;
        var handler = new CreateOrder.Handler(_repoMock.Object, _busMock.Object);
        var cmd = new CreateOrder.Command(invalidDraft);

        // Act
        var act = async () => await handler.Handle(cmd, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }
    // ... existing code ...
}