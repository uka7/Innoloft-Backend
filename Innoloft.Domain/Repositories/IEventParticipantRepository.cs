using Innoloft.Domain.Entities;
using Innoloft.Shared.Dtos;

namespace Innoloft.Domain.Repositories;

public interface IEventParticipantRepository
{
    Task<EventParticipant> GetByIdAsync(int id);
    Task<IEnumerable<EventParticipant>> GetAllByEventIdAsync(int eventId);
    Task AddAsync(EventParticipant eventParticipant);
    Task UpdateAsync(EventParticipant eventParticipant);
    Task DeleteAsync(int id);
    Task<EventParticipant?> GetParticipantByEventIdAsync(int eventId, int userId);
    Task<IEnumerable<EventParticipant>> GetEventParticipantsAsync(int eventId, PaginationParameters paginationParameters);
    Task<List<EventParticipant>> GetReceivedInvitations(int userId);
    Task<List<EventParticipant>> GetSentInvitations(int userId);
}