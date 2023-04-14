using System.Net;
using System.Net.Http.Json;
using Innoloft.Domain.Users.Models;
using Microsoft.Extensions.Logging;

namespace Innoloft.Domain.Net.Implementation;

public class JsonPlaceholderClient : IJsonPlaceholderClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<JsonPlaceholderClient> _logger;

    public JsonPlaceholderClient(HttpClient httpClient,
        ILogger<JsonPlaceholderClient> logger)
    {
        _logger = logger;
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
        _logger.LogInformation($"Fetching user data with ID {id}");
        var response = await _httpClient.GetAsync($"/users/{id}");

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            _logger.LogInformation($"User data with ID {id} is not found");
            return null;
        }
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError($"Error fetching user data with ID {id}. Status code: {response.StatusCode}");
            return null;
        }
        var user = await response.Content.ReadFromJsonAsync<User>();
        _logger.LogInformation($"Retrieved user data with ID {id}");
        return user;
    }
}