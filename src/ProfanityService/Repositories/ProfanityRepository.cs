using ProfanityService.Models;
using ProfanityService.Data;
using Microsoft.EntityFrameworkCore;
using ProfanityService.Interfaces;

namespace ProfanityService.Repositories;
public class ProfanityRepository : IProfanityRepository
{
    private readonly ProfanityDbContext _dbContext;

    public ProfanityRepository(ProfanityDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<string>> GetAllBlockedWordsAsync()
    {
        return await _dbContext.BlockedWords.Select(bw => bw.Word).Select(word => word.ToLower()).ToListAsync();
    }

    public async Task<Profanity> AddBlockedWordAsync(string word)
    {
        var blockedWord = new Profanity { Word = word.ToLowerInvariant() };
        _dbContext.BlockedWords.Add(blockedWord);
        await _dbContext.SaveChangesAsync();
        return blockedWord;
    }

    public async Task<bool> RemoveBlockedWordAsync(string word)
    {
        var blockedWord = await _dbContext.BlockedWords.FirstOrDefaultAsync(bw => bw.Word == word);
        if (blockedWord is null) return false;

        _dbContext.BlockedWords.Remove(blockedWord);
        return await _dbContext.SaveChangesAsync() > 0;
    }
}
