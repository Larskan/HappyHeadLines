using DraftService.Models;

namespace DraftService.Interfaces;

public interface IDraftRepository
{
    Task<Draft> CreateDraftAsync(Draft draft);
    Task<Draft?> GetDraftByIdAsync(Guid id);
    Task<List<Draft>> GetAllDraftsAsync(string Author);
    Task<bool> UpdateDraftAsync(Draft draft);
    Task<bool> DeleteDraftAsync(Guid id);
}