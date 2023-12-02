using Shared.DTO;
using Shared.Request;
using Shared.Response;


namespace IService;

public interface INoteService
{
    Task<BaseResponse> CreateNote(NoteRequestDTO noteDTO);

    Task<NoteDTO> GetNoteById(int id);

    Task<TableResponse<NoteDTO>> GetAllNotesWithPagination(LookupDTO lookupDTO);

    Task<BaseResponse> UpdateNote(int id, NoteRequestDTO noteDTO);

    Task<BaseResponse> DeleteNote(int id);
}
