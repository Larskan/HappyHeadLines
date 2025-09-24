using DraftService.Models;

namespace DraftService.Interfaces;

public interface IDraftRepository
{
    Task<Draft> CreateDraftAsync(Draft draft);
    Task<List<Draft>> GetAllDraftsAsync(Guid authorId);
    Task<Draft?> UpdateDraftAsync(Draft draft);
    Task<bool> DeleteDraftAsync(Guid id);
}