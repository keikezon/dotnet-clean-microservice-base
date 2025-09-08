using System.Net.Http.Json;
using Order.Application.Orders.Abstractions;
using Order.Application.Orders.Contracts;

namespace Order.Infrastructure.Clients;

public sealed class ProductClient(HttpClient httpClient) : IProductClient
{
    public async Task<bool> DecreaseStockAsync(Guid productId, int quantity, CancellationToken ct)
    {
        try
        {
            var request = new { Id = productId, Quantity = quantity };

            var response = await httpClient.PutAsJsonAsync($"/api/products/decrease-stock", request, ct);

            if (response.IsSuccessStatusCode)
                return true;

            var content = await response.Content.ReadAsStringAsync(ct);
            
            throw new ApplicationException(
                $"Failed to decrease stock. StatusCode: {response.StatusCode}, Response: {content}");
        }
        catch (Exception ex)
        {
            throw new ApplicationException(ex.Message, ex);
        }
    }
    
    public async Task<ProductResponse?> GetByIdAsync(Guid productId, CancellationToken ct)
    {
        try
        {
            var response = await httpClient.GetFromJsonAsync<ProductResponse?>($"/api/products/{productId}",ct);

            if (response != null)
                return response;
        }
        catch (Exception ex)
        {
            throw new ApplicationException(ex.Message, ex);
        }

        return null;
    }
}