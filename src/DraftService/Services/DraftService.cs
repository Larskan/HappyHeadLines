using DraftService.Models;
using DraftService.Interfaces;
using DraftService.Data;
using Microsoft.EntityFrameworkCore;
using Shared;
using System.Diagnostics;
using Serilog;



namespace DraftService.Services;


public class DraftService(DraftDbContext context, Serilog.ILogger serilogger) : IDraftService
{
    private readonly DraftDbContext _context = context;
    // private readonly ILogger<DraftService> _logger;
    private readonly Serilog.ILogger _serilogger = serilogger;

    public async Task<DraftCreateDto> CreateDraftAsync(DraftCreateDto draftCreateDto)
    {
        // manual span for demo
        using var activity = new Activity("CreateDraft").Start();
        var draft = new Draft
        {
            Title = draftCreateDto.Title,
            Body = draftCreateDto.Body,
            AuthorId = draftCreateDto.AuthorId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Drafts.Add(draft);
        await _context.SaveChangesAsync();

        _serilogger.Information("Created draft {@Draft}", draft);
        _serilogger.Information("Lars ran this piece of code");

        activity?.SetTag("draft.id", draft.Id);
        activity?.SetTag("draft.authorId", draft.AuthorId);

        return ToDtoCreate(draft);
    }

    public async Task<DraftDto?> GetDraftByIdAsync(int id)
    {
        var draft = await _context.Drafts.FindAsync(id);
        _serilogger.Information("Retrieved draft {@Draft}", draft);
        _serilogger.Information("Lars ran this piece of code");
        return draft == null ? null : ToDto(draft);
    }

    public async Task<List<DraftDto>> GetAllDraftsAsync(int authorId)
    {
        var drafts = await _context.Drafts.ToListAsync();
        _serilogger.Information("Retrieved {Count} drafts for author {AuthorId}", drafts.Count, authorId);
        return drafts.Select(ToDto).ToList();
    }

    public async Task<bool> UpdateDraftAsync(int id, DraftUpdateDto draftUpdateDto)
    {
        var draft = await _context.Drafts.FindAsync(id);
        if (draft == null) return false;

        draft.Title = draftUpdateDto.Title;
        draft.Body = draftUpdateDto.Body;
        draft.UpdatedAt = DateTime.UtcNow;

        _serilogger.Information("Updating draft {@Draft}", draft);

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteDraftAsync(int id)
    {
        var draft = await _context.Drafts.FindAsync(id);
        if (draft == null) return false;

        _context.Drafts.Remove(draft);
        await _context.SaveChangesAsync();
        return true;
    }

    private static DraftDto ToDto(Draft d) =>
        new DraftDto(d.Id, d.AuthorId, d.Title, d.Body,  d.CreatedAt, d.UpdatedAt);
    
    private static DraftCreateDto ToDtoCreate(Draft d) =>
        new DraftCreateDto(d.Title, d.Body, d.AuthorId);
}