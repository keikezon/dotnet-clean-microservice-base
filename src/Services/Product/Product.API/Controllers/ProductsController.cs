using AutoMapper;
using Common.Enum;
using FluentValidation;
using Identity.API.Contracts.Products;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Product.API.Contracts.Products;
using Product.Application.Products.Commands;

namespace Product.API.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController(IMapper mapper, 
    CreateProduct.IHandler createProduct,
    UpdateProduct.IHandler updateProduct, 
    UpdateStock.IHandler updateStock, 
    DecreaseStock.IHandler decreaseStock, 
    GetProduct.IHandler getProduct,
    DeleteProduct.IHandler deleteProduct,
    ListProduct.IHandler listProduct, 
    IValidator<CreateProduct.Command> validator, 
    IValidator<UpdateStock.Command> validatorStock,
    IValidator<DecreaseStock.Command> validatorDecreaseStock, 
    IValidator<UpdateProduct.Command> validatorUpdate, 
    IValidator<GetProduct.Command> validatorGet,
    IValidator<DeleteProduct.Command> validatorDelete) : Controller
{
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var cmd = new GetProduct.Command(id);
        var validation = await validatorGet.ValidateAsync(cmd, ct);
        if (!validation.IsValid)
            return BadRequest(validation.ToDictionary());

        var product = await getProduct.Handle(cmd, ct);
        if (product == null)
            return new NotFoundResult();
        var response = mapper.Map<ProductResponse>(product);
        return Ok(response);
    }
    
    [HttpGet]
    [ProducesResponseType(typeof(List<ProductResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> List(CancellationToken ct)
    {
        var cmd = new ListProduct.Command();
        var list = await listProduct.Handle(cmd, ct);
        if (list == null)
            return new NotFoundResult();
        var response = mapper.Map<List<ProductResponse>>(list);
        return Ok(response);
    }
    
    [HttpPost]
    [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status201Created)]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateProductRequest request, CancellationToken ct)
    {
        var cmd = new CreateProduct.Command(request.Name, request.Description, request.Price, request.Stock);
        var validation = await validator.ValidateAsync(cmd, ct);
        if (!validation.IsValid)
            return BadRequest(validation.ToDictionary());

        var product = await createProduct.Handle(cmd, ct);
        var response = mapper.Map<ProductResponse>(product);
        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }
    
    [HttpPut]
    [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status200OK)]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update([FromBody] UpdateProductRequest request, CancellationToken ct)
    {
        var cmd = new UpdateProduct.Command(request.Id,request.Name, request.Description, request.Price, request.Stock);
        var validation = await validatorUpdate.ValidateAsync(cmd, ct);
        if (!validation.IsValid)
            return BadRequest(validation.ToDictionary());

        var product = await updateProduct.Handle(cmd, ct);
        var response = mapper.Map<ProductResponse>(product);
        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }
    
    [HttpPut("increase-stock")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> IncreaseStock([FromBody] UpdateStockRequest request, CancellationToken ct)
    {
        var cmd = new UpdateStock.Command(request.Id, request.Quantity, request.Invoice);
        var validation = await validatorStock.ValidateAsync(cmd, ct);
        if (!validation.IsValid)
            return BadRequest(validation.ToDictionary());

        await updateStock.Handle(cmd, ct);
        return NoContent();
    }
    
    [HttpPut("decrease-stock")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DecreaseStock([FromBody] DecreaseStockRequest request, CancellationToken ct)
    {
        var cmd = new DecreaseStock.Command(request.Id, request.Quantity);
        var validation = await validatorDecreaseStock.ValidateAsync(cmd, ct);
        if (!validation.IsValid)
            return BadRequest(validation.ToDictionary());

        await decreaseStock.Handle(cmd, ct);
        return NoContent();
    }
    
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteById(Guid id, CancellationToken ct)
    {
        var cmd = new DeleteProduct.Command(id);
        var validation = await validatorDelete.ValidateAsync(cmd, ct);
        if (!validation.IsValid)
            return BadRequest(validation.ToDictionary());

        var deleted = await deleteProduct.Handle(cmd, ct);
        var response = mapper.Map<bool>(deleted);
        return Ok(response);
    }
}