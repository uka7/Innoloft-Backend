using Innoloft.Domain.Users.Models;

namespace Innoloft.Domain.Users;

public interface IUserRepository
{
    Task<User> GetUserAsync(int userId);
}