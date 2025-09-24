using Shared;

namespace DraftService.Interfaces;

public interface IDraftService
{
    Task<DraftCreateDto> CreateDraftAsync(DraftCreateDto draftCreateDto);
    Task<DraftDto?> GetDraftByIdAsync(Guid id);
    Task<List<DraftDto>> GetAllDraftsAsync(Guid authorId);
    Task<bool> UpdateDraftAsync(Guid id, DraftUpdateDto draftUpdateDto);
    Task<bool> DeleteDraftAsync(Guid id);
}