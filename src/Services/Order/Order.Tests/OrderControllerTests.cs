using System.Runtime.Serialization;
using System.Security.Claims;
using AutoMapper;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Order.API.Contracts.Orders;
using Order.API.Controllers;
using Order.Application.Orders.Commands;
using Order.Domain.Orders;
using Xunit;

namespace Order.Tests;

public class OrderControllerTests
{
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<CreateOrder.IHandler> _createOrderHandlerMock = new();
    private readonly Mock<GetOrder.IHandler> _getOrderHandlerMock = new();
    private readonly Mock<ListOrder.IHandler> _listOrderHandlerMock = new();

    private readonly Mock<IValidator<CreateOrder.Command>> _createValidatorMock = new();
    private readonly Mock<IValidator<GetOrder.Command>> _getValidatorMock = new();

    private OrdersController CreateController()
    {
        var controller = new OrdersController(
            _mapperMock.Object,
            _createOrderHandlerMock.Object,
            _getOrderHandlerMock.Object,
            _listOrderHandlerMock.Object,
            _createValidatorMock.Object,
            _getValidatorMock.Object
        );

        var userId = Guid.NewGuid().ToString();
        var claims = new[]
        {
            new Claim("sub", userId),
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var user = new ClaimsPrincipal(identity);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = user
            }
        };

        return controller;
    }

    private static OrderModel CreateAnyOrderInstance()
    {
        // Avoids depending on specific constructors; we only need a non-null instance for the controller flow.
        return (OrderModel)FormatterServices.GetUninitializedObject(typeof(OrderModel));
    }

    #region Create Endpoint

    [Fact]
    public async Task Create_ShouldReturnCreated_WhenRequestIsValid()
    {
        // Arrange
        var request = new CreateOrderRequest(
            new List<CreateOrderItemRequest>
            {
                new CreateOrderItemRequest(Guid.NewGuid(), 2)
            },
            "12345678900"
        );

        var domainResult = CreateAnyOrderInstance();

        var expectedResponse = new OrderResponse
        (
            Guid.NewGuid(),
            Guid.Parse("11111111-1111-1111-1111-111111111111"),
            "Admin",
            request.ClientDocument,
            new List<OrderItemResponse>
            {
                new OrderItemResponse(Guid.NewGuid(), request.Items[0].ProductId, "Name",2, 10.5m)
            },
            21m
        );

        _mapperMock
            .Setup(m => m.Map<OrderModel>(It.IsAny<CreateOrderRequest>()))
            .Returns(CreateAnyOrderInstance());

        _createValidatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<CreateOrder.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _createOrderHandlerMock
            .Setup(h => h.Handle(It.IsAny<CreateOrder.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(domainResult);

        _mapperMock
            .Setup(m => m.Map<OrderResponse>(domainResult))
            .Returns(expectedResponse);

        var controller = CreateController();

        // Act
        var result = await controller.Create(request, CancellationToken.None);

        // Assert
        var created = result.Should().BeOfType<CreatedAtActionResult>().Subject;
        created.ActionName.Should().Be(nameof(OrdersController.GetById));

        var response = created.Value.Should().BeOfType<OrderResponse>().Subject;
        response.Id.Should().Be(expectedResponse.Id);
        response.ClientDocument.Should().Be(request.ClientDocument);
        response.Items.Should().HaveCount(1);
        response.Items![0].ProductId.Should().Be(request.Items[0].ProductId);
        response.Items![0].Quantity.Should().Be(2);
        response.Items![0].UnitPrice.Should().Be(10.5m);
    }

    [Fact]
    public async Task Create_ShouldReturnBadRequest_WhenValidationFails()
    {
        // Arrange
        var request = new CreateOrderRequest(
            new List<CreateOrderItemRequest>(),
            ""
        );

        var failures = new List<ValidationFailure>
        {
            new ValidationFailure("Items", "At least one item is required"),
            new ValidationFailure("ClientDocument", "ClientDocument is required")
        };

        _mapperMock
            .Setup(m => m.Map<OrderModel>(It.IsAny<CreateOrderRequest>()))
            .Returns(CreateAnyOrderInstance());

        _createValidatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<CreateOrder.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(failures));

        var controller = CreateController();

        // Act
        var result = await controller.Create(request, CancellationToken.None);

        // Assert
        var badRequest = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        var errors = badRequest.Value.Should().BeAssignableTo<Dictionary<string, string[]>>().Subject;
        errors.Should().ContainKey("Items");
        errors.Should().ContainKey("ClientDocument");
    }

    #endregion

    #region GetById Endpoint

    [Fact]
    public async Task GetById_ShouldReturnOk_WhenOrderExists()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var domainOrder = CreateAnyOrderInstance();

        var mappedResponse = new OrderResponse
        (
            orderId,
            Guid.Parse("11111111-1111-1111-1111-111111111111"),
            "Admin",
            "12345678900",
            new List<OrderItemResponse>
            {
                new OrderItemResponse(Guid.NewGuid(), Guid.Parse("11111111-1111-1111-1111-111111111111"), "Name",1, 50.0m )
            },
            50.0m
        );

        _getValidatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<GetOrder.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _getOrderHandlerMock
            .Setup(h => h.Handle(It.IsAny<GetOrder.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(domainOrder);

        _mapperMock
            .Setup(m => m.Map<OrderResponse>(domainOrder))
            .Returns(mappedResponse);

        var controller = CreateController();

        // Act
        var result = await controller.GetById(orderId, CancellationToken.None);

        // Assert
        var ok = result.Should().BeOfType<OkObjectResult>().Subject;
        var response = ok.Value.Should().BeOfType<OrderResponse>().Subject;

        response.Id.Should().Be(orderId);
        response.ClientDocument.Should().Be("12345678900");
        response.Items.Should().HaveCount(1);
        response.Items![0].Quantity.Should().Be(1);
        response.Items![0].UnitPrice.Should().Be(50.0m);
    }

    [Fact]
    public async Task GetById_ShouldReturnBadRequest_WhenValidationFails()
    {
        // Arrange
        var orderId = Guid.NewGuid();

        var failures = new List<ValidationFailure>
        {
            new ValidationFailure("Id", "Invalid order Id")
        };

        _getValidatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<GetOrder.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(failures));

        var controller = CreateController();

        // Act
        var result = await controller.GetById(orderId, CancellationToken.None);

        // Assert
        var badRequest = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        var errors = badRequest.Value.Should().BeAssignableTo<Dictionary<string, string[]>>().Subject;
        errors.Should().ContainKey("Id");
    }

    [Fact]
    public async Task GetById_ShouldReturnNotFound_WhenOrderDoesNotExist()
    {
        // Arrange
        var orderId = Guid.NewGuid();

        _getValidatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<GetOrder.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _getOrderHandlerMock
            .Setup(h => h.Handle(It.IsAny<GetOrder.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((OrderModel?)null);

        var controller = CreateController();

        // Act
        var result = await controller.GetById(orderId, CancellationToken.None);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    #endregion

    #region List Endpoint

    [Fact]
    public async Task List_ShouldReturnOk_WhenOrdersExist()
    {
        // Arrange
        var domainList = new List<OrderModel> { CreateAnyOrderInstance(), CreateAnyOrderInstance() };
        var mappedList = new List<OrderResponse>
        {
            new OrderResponse
            (
                Guid.NewGuid(),
                Guid.Parse("11111111-1111-1111-1111-111111111111"),
                "Admin",
                "A",
                new List<OrderItemResponse>
                {
                    new OrderItemResponse(Guid.NewGuid(), Guid.Parse("11111111-1111-1111-1111-111111111111"), "Name",1, 50.0m )
                },
                50.0m
            ),
            new OrderResponse
            (
                Guid.NewGuid(),
                Guid.Parse("11111111-1111-1111-1111-111111111111"),
                "Admin",
                "B",
                new List<OrderItemResponse>
                {
                    new OrderItemResponse(Guid.NewGuid(), Guid.Parse("11111111-1111-1111-1111-111111111111"), "Name",1, 50.0m )
                },
                50.0m
            )
        };

        _listOrderHandlerMock
            .Setup(h => h.Handle(It.IsAny<ListOrder.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(domainList);

        _mapperMock
            .Setup(m => m.Map<List<OrderResponse>>(domainList))
            .Returns(mappedList);

        var controller = CreateController();

        // Act
        var result = await controller.List(CancellationToken.None);

        // Assert
        var ok = result.Should().BeOfType<OkObjectResult>().Subject;
        var response = ok.Value.Should().BeAssignableTo<List<OrderResponse>>().Subject;
        response.Should().HaveCount(2);
    }

    [Fact]
    public async Task List_ShouldReturnNotFound_WhenNoOrders()
    {
        // Arrange
        _listOrderHandlerMock
            .Setup(h => h.Handle(It.IsAny<ListOrder.Command>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((List<OrderModel>?)null);

        var controller = CreateController();

        // Act
        var result = await controller.List(CancellationToken.None);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    #endregion
}