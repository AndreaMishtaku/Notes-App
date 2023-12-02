using Entities;
using Shared.Request;
using Shared.Response;

namespace IRepository;

public interface INoteRepository
{

    void CreateRecord(Note note);
    void UpdateRecord(Note note);
    void DeleteRecord(Note note);
    Task<Note> GetRecordByIdAsync(int id);

    Task<PagedResult<Note>> GetAllNotesWithPagination(LookupDTO lookupDTO);

    Task SaveAsync();
}
