using IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NotesApp.Utility;
using Shared.DTO;
using Shared.Request;

namespace NotesApp.Controllers;

[Route("api/notes")]
[ApiController]
[Authorize]
public class NoteController : ControllerBase
{

    private readonly INoteService _noteService;

    public NoteController(INoteService noteService)
    {
        _noteService = noteService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateNote([FromBody] NoteRequestDTO noteDto)
    {
        var result = await _noteService.CreateNote(noteDto);
        return Ok(result);
    }

    [HttpPost("get-all")]
    public async Task<IActionResult> GetAllNotesPagination([FromBody] LookupDTO lookupDto)
    {
        var result = await _noteService.GetAllNotesWithPagination(lookupDto);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetNoteById(int id)
    {
        var result = await _noteService.GetNoteById(id);
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateNote(int id, [FromBody] NoteRequestDTO noteDto)
    {
       
        var result = await _noteService.UpdateNote(id, noteDto);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteNote(int id)
    {
        var result = await _noteService.DeleteNote(id);
        return Ok(result);
    }
}
