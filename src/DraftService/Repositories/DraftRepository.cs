using DraftService.Models;
using Microsoft.EntityFrameworkCore;
using DraftService.Data;
using DraftService.Interfaces;

namespace DraftService.Repositories;

public class DraftRepository : IDraftRepository
{
    private readonly DraftDbContext _dbContext;

    public DraftRepository(DraftDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Draft> CreateDraftAsync(Draft draft)
    {
        _dbContext.Drafts.Add(draft);
        await _dbContext.SaveChangesAsync();
        return draft;
    }

    public async Task<List<Draft>> GetAllDraftsAsync(Guid authorId) =>
        await _dbContext.Drafts.Where(d => d.AuthorId == authorId).ToListAsync();


    public async Task<Draft?> UpdateDraftAsync(Draft draft)
    {
        var existingDraft = await _dbContext.Drafts.FindAsync(draft.Id);
        if (existingDraft is null) return null;

        existingDraft.Title = draft.Title;
        existingDraft.Body = draft.Body;
        existingDraft.AuthorId = draft.AuthorId;
        existingDraft.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();
        return existingDraft;
    }

    public async Task<bool> DeleteDraftAsync(Guid id)
    {
        var draft = await _dbContext.Drafts.FindAsync(id);
        if (draft is null) return false;
        _dbContext.Drafts.Remove(draft);
        return await _dbContext.SaveChangesAsync() > 0;
    }
}