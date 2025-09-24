using Shared;

namespace DraftService.Interfaces;

public interface IDraftService
{
    Task<DraftCreateDto> CreateDraftAsync(DraftCreateDto draftCreateDto);
    Task<DraftDto?> GetDraftByIdAsync(int id);
    Task<List<DraftDto>> GetAllDraftsAsync(int authorId);
    Task<bool> UpdateDraftAsync(int id, DraftUpdateDto draftUpdateDto);
    Task<bool> DeleteDraftAsync(int id);
}