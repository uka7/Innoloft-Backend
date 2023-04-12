using Innoloft.Domain.Entities;
using Innoloft.Domain.Repositories;
using Innoloft.Shared.Dtos;
using Microsoft.EntityFrameworkCore;

namespace Innoloft.EntityFramework.Repositories;

public class EventParticipantRepository : IEventParticipantRepository
{
    private readonly AppDbContext _context;

    public EventParticipantRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<EventParticipant> GetByIdAsync(int id)
    {
        return await _context.EventParticipants.FindAsync(id);
    }

    public async Task<IEnumerable<EventParticipant>> GetAllByEventIdAsync(int eventId)
    {
        return await _context.EventParticipants
            .Where(ep => ep.EventId == eventId)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<EventParticipant>> GetEventParticipantsAsync(int eventId, PaginationParameters paginationParameters)
    {
        return await _context.EventParticipants
            .Where(ep => ep.EventId == eventId)
            .Skip((paginationParameters.PageNumber - 1) * paginationParameters.PageSize)
            .Take(paginationParameters.PageSize)
            .ToListAsync();
    }
    
    public async Task<EventParticipant?> GetParticipantByEventIdAsync(int eventId, int userId)
    {
        return await _context.EventParticipants
            .FirstOrDefaultAsync(ep => ep.EventId == eventId && ep.UserId == userId)!;
    }


    public async Task AddAsync(EventParticipant eventParticipant)
    {
        await _context.EventParticipants.AddAsync(eventParticipant);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(EventParticipant eventParticipant)
    {
        _context.Entry(eventParticipant).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var eventParticipant = await _context.EventParticipants.FindAsync(id);
        if (eventParticipant != null)
        {
            _context.EventParticipants.Remove(eventParticipant);
            await _context.SaveChangesAsync();
        }
    }
}