using AutoMapper;
using Entities;
using Exceptions;
using IRepository;
using IService;
using Shared.DTO;
using Shared.Request;
using Shared.Response;
using Shared.Configuration;
using Shared.Response.Columns;
using Microsoft.Extensions.Logging;

namespace Service;

public class NoteService : INoteService
{

    private readonly IMapper _mapper;
    private readonly INoteRepository _noteRepository;
    private readonly IClaimsUtility _claimsUtility;
    private readonly ILogger<NoteService> _logger;

    public NoteService(IMapper mapper, INoteRepository noteRepository,IClaimsUtility claimsUtility, ILogger<NoteService> logger)
    {

        _mapper = mapper;
        _noteRepository = noteRepository;
        _claimsUtility = claimsUtility;
        _logger = logger;


    }

 

    public async Task<BaseResponse> CreateNote(NoteRequestDTO noteDTO)
    {
        try
        {
            var note = _mapper.Map<Note>(noteDTO);

            var userId = _claimsUtility.ReadCurrentUserId();

            if (userId == 0)
            {
                throw new Exception("Not authorized!!!");
            }
            note.UserId = userId;
            note.CreatedAt = DateTime.Now;
            _noteRepository.CreateRecord(note);
            await _noteRepository.SaveAsync();

            return new BaseResponse
            {
             
                Message = "New note is added with success!",
                StatusCode = 200
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("Note creation failed!!!");
            throw new BadRequestException(ex.Message);
        }
    }

    public async Task<BaseResponse> DeleteNote(int id)
    {
     
        var existingNote = await _noteRepository.GetRecordByIdAsync(id);


        var userId = _claimsUtility.ReadCurrentUserId();

        if (userId == 0)
        {
            throw new Exception("Not authorized!!!");
        }


        if (existingNote is null && existingNote.User.Id!=userId)
        {
            throw new NotFoundException(string.Format("Note with Id: {0} doesnt exist!", id));
        }

        _noteRepository.DeleteRecord(existingNote);
        return new BaseResponse()
        {
            Message = "Note deleted successfully!",
            StatusCode = 200
        };
        
    }    
    
    public async Task<NoteDTO> GetNoteById(int id)
    {
        var existingNote = await _noteRepository.GetRecordByIdAsync(id);

        var userId = _claimsUtility.ReadCurrentUserId();

        if (userId == 0)
        {
            throw new Exception("Not authorized!!!");
        }

        if (existingNote is null && existingNote.User.Id!=userId)
        {
            throw new NotFoundException(string.Format("Note with Id: {0} doesnt exist!", id));
        }

        var serviceResponse = _mapper.Map<NoteDTO>(existingNote);
        return serviceResponse;
    }

    public async Task<TableResponse<NoteDTO>> GetAllNotesWithPagination(LookupDTO lookupDto)
    {
        try
        {


            PagedResult<Note> notes =await _noteRepository.GetAllNotesWithPagination(lookupDto);

            var userId = _claimsUtility.ReadCurrentUserId();

            if (userId == 0)
            {
                throw new Exception("Not authorized!!!");
            }

            var columns = GenerateDataTableColumn<NoteColumn>.GetTableColumns();
            var response = new TableResponse<NoteDTO>
            {
                RowCount = notes.RowCount,
                Page = notes.Page,
                PageSize = notes.PageSize,
                Columns =columns,
                Rows = _mapper.Map<IEnumerable<NoteDTO>>(notes.Rows),
                HasNext = notes.HasNext,
                HasPrevious = notes.HasPrevious,
                TotalPages = notes.TotalPages,
            };

            return response;
        }
        catch (Exception ex)
        {
            throw new Exception("Error retrieving notes: " + ex.Message, ex);
        }
    }



    public async Task<BaseResponse> UpdateNote(int id, NoteRequestDTO noteDTO)
    {
        try
        {
            var existingNote = await _noteRepository.GetRecordByIdAsync(id);

            var userId = _claimsUtility.ReadCurrentUserId();

            if (userId == 0)
            {
                throw new Exception("Not authorized!!!");
            }


            if (existingNote is null&&existingNote.Id!=userId)
            {
                throw new NotFoundException(string.Format("Note with Id: {0} doesnt exist!", id));
            }
          

            _mapper.Map(noteDTO, existingNote);
            existingNote.UpdatedAt = DateTime.Now;
            _noteRepository.UpdateRecord(existingNote);

            await _noteRepository.SaveAsync();

            return new BaseResponse
            {
                Message = "Note is updated with success",
                StatusCode = 200
            };
            
        }
        catch (Exception ex)
        {
            _logger.LogError("Note update failed!!!");
            throw new BadRequestException(ex.Message);

        }
    }
}
