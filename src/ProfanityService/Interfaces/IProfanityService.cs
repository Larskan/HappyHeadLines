using ProfanityService.Models;
using System.Text.RegularExpressions;

namespace ProfanityService.Interfaces
{
    public interface IProfanityService
    {
        Task<List<string>> GetAllBlockedWordsAsync();
        Task<Profanity> AddBlockedWordAsync(string word);
        Task<string> FilterProfanityAsync(string text);
    }
}