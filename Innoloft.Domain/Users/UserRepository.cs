using Innoloft.Cache.Redis;
using Innoloft.Domain.Net;
using Innoloft.Domain.Users.Models;

namespace Innoloft.Domain.Users;

public class UserRepository : IUserRepository
{
    private readonly IJsonPlaceholderClient _jsonPlaceholderClient;
    private readonly ICacheRepository _cacheRepository;

    public UserRepository(IJsonPlaceholderClient jsonPlaceholderClient,
                          ICacheRepository cacheRepository)
    {
        _jsonPlaceholderClient = jsonPlaceholderClient;
        _cacheRepository = cacheRepository;
    }
    
    public async Task<User> GetUserAsync(int userId)
    {
        // Try to get user from cache
        User user = await _cacheRepository.GetAsync<User>($"User_{userId}");

        if (user == null)
        {
            // Get user details from JsonPlaceholder API
            user = await _jsonPlaceholderClient.GetUserByIdAsync(userId);

            if (user == null)
            {
                return null;
            }

            // Save user details in cache
            await _cacheRepository.SetAsync($"User_{userId}", user,TimeSpan.FromMinutes(30));
        }

        return user;
    }
}