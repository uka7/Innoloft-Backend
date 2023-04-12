using Innoloft.Domain.Entities;
using Innoloft.Shared.Dtos;

namespace Innoloft.Domain.Repositories;

public interface IEventRepository
{
    Task<Event> GetByIdAsync(int id);
    Task<Event> AddAsync(Event eventEntity);
    Task UpdateAsync(Event eventEntity);
    Task DeleteAsync(int id);
    Task<PagedResult<Event>> GetAllAsync(EventFilterParameters filterParameters, int currentUserId);
}