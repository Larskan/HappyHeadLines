using Shared;

namespace DraftService.Interfaces;

public interface IDraftService
{
    Task<DraftDto> CreateDraftAsync(DraftCreateDto draftCreateDto);
    Task<DraftDto?> GetDraftByIdAsync(Guid id);
    Task<List<DraftDto>> GetAllDraftsAsync(string Author);
    Task<bool> UpdateDraftAsync(DraftUpdateDto draftUpdateDto);
    Task<bool> DeleteDraftAsync(Guid id);
}