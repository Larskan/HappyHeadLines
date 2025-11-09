using DraftService.Models;

namespace DraftService.Interfaces;

public interface IDraftRepository
{
    Task<Draft> CreateDraftAsync(Draft draft);
    Task<List<Draft>> GetAllDraftsAsync(int authorId); //Find all drafts for a specific author
    Task<Draft?> UpdateDraftAsync(Draft draft);
    Task<bool> DeleteDraftAsync(int id);
}