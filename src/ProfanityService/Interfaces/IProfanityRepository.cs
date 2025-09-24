
using ProfanityService.Models;

namespace ProfanityService.Interfaces
{
    public interface IProfanityRepository
    {
        Task<List<string>> GetAllBlockedWordsAsync();
        Task<Profanity> AddBlockedWordAsync(string word);
        Task<bool> RemoveBlockedWordAsync(string word);
    }
}