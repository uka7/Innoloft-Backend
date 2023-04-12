using System.Net;
using System.Net.Http.Json;
using Innoloft.Domain.Users.Models;

namespace Innoloft.Domain.Net.Implementation;

public class JsonPlaceholderClient : IJsonPlaceholderClient
{
    private readonly HttpClient _httpClient;

    public JsonPlaceholderClient(HttpClient httpClient)
    { 
        // Disable SSL certificate validation
        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };

        _httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://jsonplaceholder.typicode.com")
        };
    }
    
    public async Task<User> GetUserByIdAsync(int id)
    {
        var response = await _httpClient.GetAsync($"/users/{id}");

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"Error fetching user data with ID {id}. Status code: {response.StatusCode}");
        }
        var user = await response.Content.ReadFromJsonAsync<User>();
        return user;
    }
}