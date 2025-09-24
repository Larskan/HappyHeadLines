using ProfanityService.Models;
using ProfanityService.Repositories;
using System.Text.RegularExpressions;
using ProfanityService.Interfaces;

namespace ProfanityService.Services
{
    public class ProfanityService : IProfanityService
    {
        private readonly IProfanityRepository _profanityRepository;

        public ProfanityService(IProfanityRepository profanityRepository)
        {
            _profanityRepository = profanityRepository;
        }

        public async Task<List<string>> GetAllBlockedWordsAsync() => await _profanityRepository.GetAllBlockedWordsAsync();

        public async Task<Profanity> AddBlockedWordAsync(string word) => await _profanityRepository.AddBlockedWordAsync(word);

        public async Task<string> FilterProfanityAsync(string text)
        {
            var blockedWords = await _profanityRepository.GetAllBlockedWordsAsync();
            var body = text;
            foreach (var bw in blockedWords)
            {
                body = Regex.Replace(body, $@"\b{Regex.Escape(bw)}\b", new string('*', bw.Length), RegexOptions.IgnoreCase);
            }
            return body;
        }
    }
}