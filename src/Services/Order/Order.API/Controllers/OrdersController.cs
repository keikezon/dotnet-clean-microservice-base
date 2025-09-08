using System.Security.Claims;
using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Order.API.Contracts.Orders;
using Order.Application.Orders.Commands;
using Order.Domain.Orders;

namespace Order.API.Controllers;

[ApiController]
[Route("api/orders")]
public class OrdersController(IMapper mapper, 
    CreateOrder.IHandler createOrder,
    GetOrder.IHandler getOrder,
    ListOrder.IHandler listProduct, 
    IValidator<CreateOrder.Command> validator, 
    IValidator<GetOrder.Command> validatorGet) : Controller
{
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var cmd = new GetOrder.Command(id);
        var validation = await validatorGet.ValidateAsync(cmd, ct);
        if (!validation.IsValid)
            return BadRequest(validation.ToDictionary());

        var product = await getOrder.Handle(cmd, ct);
        if (product == null)
            return new NotFoundResult();
        var response = mapper.Map<OrderResponse>(product);
        return Ok(response);
    }
    
    [HttpGet]
    [ProducesResponseType(typeof(List<OrderResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> List(CancellationToken ct)
    {
        var cmd = new ListOrder.Command();
        var list = await listProduct.Handle(cmd, ct);
        if (list == null)
            return new NotFoundResult();
        var response = mapper.Map<List<OrderResponse>>(list);
        return Ok(response);
    }
    
    [HttpPost]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status201Created)]
    [Authorize(Roles = "Seller")]
    public async Task<IActionResult> Create([FromBody] CreateOrderRequest request, CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);;
        
        var requestMap = mapper.Map<OrderModel>(request);
        requestMap.UserId = Guid.Parse(userId);
        
        var cmd = new CreateOrder.Command(requestMap);
        var validation = await validator.ValidateAsync(cmd, ct);
        if (!validation.IsValid)
            return BadRequest(validation.ToDictionary());

        var product = await createOrder.Handle(cmd, ct);
        var response = mapper.Map<OrderResponse>(product);
        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }
}