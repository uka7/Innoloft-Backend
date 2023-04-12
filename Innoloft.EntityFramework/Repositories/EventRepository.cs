using Innoloft.Domain.Entities;
using Innoloft.Domain.Extensions;
using Innoloft.Domain.Repositories;
using Innoloft.Shared.Dtos;
using Microsoft.EntityFrameworkCore;

namespace Innoloft.EntityFramework.Repositories;

public class EventRepository : IEventRepository
{
    private readonly AppDbContext _context;

    public EventRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<Event>> GetAllAsync(EventFilterParameters filterParameters, int currentUserId)
    {
        var events = _context.Events.AsQueryable();
        List<int> userEventsIds = new List<int>();
        if (filterParameters.OnlyEventsCurrentUserIsParticipantInFilter)
        {
            userEventsIds = await _context.EventParticipants.AsQueryable()
                .Where(e => e.UserId == currentUserId)
                .Select(e => e.EventId).Distinct().ToListAsync();
        }

        events = events.WhereIf(filterParameters.IdFilter.HasValue, e => e.Id == filterParameters.IdFilter.Value)
            .WhereIf(!string.IsNullOrWhiteSpace(filterParameters.TitleFilter), e => e.Title.Contains(filterParameters.TitleFilter))
            .WhereIf(filterParameters.StartDateFilter.HasValue, e => e.StartDate >= filterParameters.StartDateFilter.Value)
            .WhereIf(filterParameters.EndDateFilter.HasValue, e => e.EndDate <= filterParameters.EndDateFilter.Value)
            .WhereIf(!string.IsNullOrWhiteSpace(filterParameters.LocationFilter), e => e.Location.Contains(filterParameters.LocationFilter))
            .WhereIf(!string.IsNullOrWhiteSpace(filterParameters.DescriptionFilter), e => e.Description.Contains(filterParameters.DescriptionFilter))
            .WhereIf(filterParameters.CreatorUserIdFilter.HasValue, e => e.CreatorUserId == filterParameters.CreatorUserIdFilter)
            .WhereIf(filterParameters.OnlyEventsCreatedByCurrentUserFilter, e => e.CreatorUserId == currentUserId)
            .WhereIf(filterParameters.OnlyEventsCurrentUserIsParticipantInFilter, e => userEventsIds.Contains(e.Id));

        var totalCount = await events.CountAsync();
        var items = await events.Skip((filterParameters.PageNumber - 1) * filterParameters.PageSize)
            .Take(filterParameters.PageSize)
            .ToListAsync();

        return new PagedResult<Event>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = filterParameters.PageNumber,
            PageSize = filterParameters.PageSize
        };
    }

    public async Task<Event> GetByIdAsync(int id)
    {
        return await _context.Events.FindAsync(id);
    }

    public async Task<Event> AddAsync(Event eventEntity)
    {
        await _context.Events.AddAsync(eventEntity);
        await _context.SaveChangesAsync();
        return eventEntity;
    }

    public async Task UpdateAsync(Event eventEntity)
    {
        var existingEntity = await _context.Events.FindAsync(eventEntity.Id);
        if (existingEntity != null)
        {
            _context.Entry(existingEntity).State = EntityState.Detached;
        }

        _context.Entry(eventEntity).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var eventEntity = await _context.Events.FindAsync(id);
        if (eventEntity != null)
        {
            _context.Events.Remove(eventEntity);
            await _context.SaveChangesAsync();
        }
    }
}