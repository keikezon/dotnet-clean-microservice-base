using System.Net.Http.Json;
using Order.Application.Orders.Abstractions;
using Order.Application.Orders.Contracts;

namespace Order.Infrastructure.Clients;

public class IdentityServiceClient(HttpClient httpClient) : IIdentityClient
{
    public async Task<UserResponse?> GetByIdAsync(Guid userId, CancellationToken ct)
    {
        try
        {
            var response = await httpClient.GetFromJsonAsync<UserResponse?>($"/api/users/{userId}",ct);
 
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