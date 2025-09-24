using DraftService.Models;
using DraftService.Interfaces;
using DraftService.Data;
using Microsoft.EntityFrameworkCore;
using Shared;



namespace DraftService.Services;


public class DraftService : IDraftService
{
    private readonly DraftDbContext _context;

    public DraftService(DraftDbContext context)
    {
        _context = context;
    }

    public async Task<DraftCreateDto> CreateDraftAsync(DraftCreateDto draftCreateDto)
    {
        var draft = new Draft
        {
            Id = Guid.NewGuid(),
            Title = draftCreateDto.Title,
            Body = draftCreateDto.Body,
            AuthorId = draftCreateDto.AuthorId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Drafts.Add(draft);
        await _context.SaveChangesAsync();

        return ToDtoCreate(draft);
    }

    public async Task<DraftDto?> GetDraftByIdAsync(Guid id)
    {
        var draft = await _context.Drafts.FindAsync(id);
        return draft == null ? null : ToDto(draft);
    }

    public async Task<List<DraftDto>> GetAllDraftsAsync(Guid authorId)
    {
        var drafts = await _context.Drafts.ToListAsync();
        return drafts.Select(ToDto).ToList();
    }

    public async Task<bool> UpdateDraftAsync(Guid id, DraftUpdateDto draftUpdateDto)
    {
        var draft = await _context.Drafts.FindAsync(id);
        if (draft == null) return false;

        draft.Title = draftUpdateDto.Title;
        draft.Body = draftUpdateDto.Body;
        draft.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteDraftAsync(Guid id)
    {
        var draft = await _context.Drafts.FindAsync(id);
        if (draft == null) return false;

        _context.Drafts.Remove(draft);
        await _context.SaveChangesAsync();
        return true;
    }

    private static DraftDto ToDto(Draft d) =>
        new DraftDto(d.Id, d.AuthorId, d.Title, d.Body,  d.CreatedAt, d.UpdatedAt);
    
    private static DraftCreateDto ToDtoCreate(Draft d) =>
        new DraftCreateDto(d.Title, d.Body, d.AuthorId);
}