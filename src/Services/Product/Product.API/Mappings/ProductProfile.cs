using AutoMapper;
using Product.Domain;
using Product.API.Contracts.Products;
using Product.Domain.Products;

namespace Product.API.Mappings;

public class ProductProfile : Profile
{
    public ProductProfile()
    {
        CreateMap<ProductModel, ProductResponse>();

        // CreateMap<CreateProductRequest, ProductModel>()
        //     .ConstructUsing(src => new ProductModel(
        //         Guid.NewGuid(),
        //         src.Name,
        //         src.Description,
        //         src.Price,
        //         src.Stock
        //     ));
    }
}