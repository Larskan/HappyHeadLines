using DraftService.Models;
using DraftService.Interfaces;
using DraftService.Data;
using Microsoft.EntityFrameworkCore;
using Shared;
using System.Diagnostics;

namespace DraftService.Services;


public class DraftService(DraftDbContext context, Serilog.ILogger serilogger) : IDraftService
{
    private readonly DraftDbContext _context = context;
    private readonly Serilog.ILogger _serilogger = serilogger;

    public async Task<DraftCreateDto> CreateDraftAsync(DraftCreateDto draftCreateDto)
    {
        // manual span for demo
        using var activity = new Activity("CreateDraft").Start();

        try
        {
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
            activity?.SetTag("draft.id", draft.Id);
            activity?.SetTag("draft.authorId", draft.AuthorId);
            return ToDtoCreate(draft);
        }
        catch (Exception ex)
        {
            _serilogger.Error(ex, "Error occurred while creating draft");
            throw;
        }
    }

    public async Task<DraftDto?> GetDraftByIdAsync(int id)
    {
        try
        {
            var draft = await _context.Drafts.FindAsync(id);
            if(draft == null) return null;
            _serilogger.Information("Retrieved draft {@Draft}", draft);
            return ToDto(draft);
        }
        catch (Exception ex)
        {
            _serilogger.Error(ex, "Error occurred while fetching draft with ID {DraftId}", id);
            throw;
        }
    }

    public async Task<List<DraftDto>> GetAllDraftsAsync(int authorId)
    {
        try
        {
            var drafts = await _context.Drafts.Where(d => d.AuthorId == authorId).ToListAsync();
            _serilogger.Information("Retrieved {Count} drafts for author {AuthorId}", drafts.Count, authorId);
            return drafts.Select(ToDto).ToList();
        }
        catch (Exception ex)
        {
            _serilogger.Error(ex, "Error occurred while fetching drafts for author {AuthorId}", authorId);
            throw;
        }
    }

    public async Task<bool> UpdateDraftAsync(int id, DraftUpdateDto draftUpdateDto)
    {
        try
        {
            var draft = await _context.Drafts.FindAsync(id);
            if (draft == null) return false;
            draft.Title = draftUpdateDto.Title;
            draft.Body = draftUpdateDto.Body;
            draft.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            _serilogger.Information("Updated draft {@Draft}", draft);
            return true;
        }
        catch (Exception ex)
        {
            _serilogger.Error(ex, "Error occurred while updating draft with ID {DraftId}", id);
            throw;
        }
    }

    public async Task<bool> DeleteDraftAsync(int id)
    {
        try
        {
            var draft = await _context.Drafts.FindAsync(id);
            if (draft == null) return false;
            _context.Drafts.Remove(draft);
            await _context.SaveChangesAsync();
            _serilogger.Information("Successfully deleted draft with ID {DraftId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _serilogger.Error(ex, "Error occurred while deleting draft with ID {DraftId}", id);
            throw;
        }
    }

    private static DraftDto ToDto(Draft d) =>
        new DraftDto(d.Id, d.AuthorId, d.Title, d.Body,  d.CreatedAt, d.UpdatedAt);
    
    private static DraftCreateDto ToDtoCreate(Draft d) =>
        new DraftCreateDto(d.Title, d.Body, d.AuthorId);
}