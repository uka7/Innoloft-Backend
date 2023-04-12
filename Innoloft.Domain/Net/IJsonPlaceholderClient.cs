using Innoloft.Domain.Users.Models;

namespace Innoloft.Domain.Net;

public interface IJsonPlaceholderClient
{
    Task<User> GetUserByIdAsync(int id);
}