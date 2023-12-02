using Dapper;
using Entities;
using IRepository;
using Microsoft.EntityFrameworkCore;
using Shared.Request;
using Shared.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository;

public class NoteRepository : INoteRepository
{

    protected RepositoryContext RepositoryContext;

    public NoteRepository(RepositoryContext repositoryContext) => RepositoryContext = repositoryContext;

    public void CreateRecord(Note note)=>RepositoryContext.Set<Note>().Add(note);

    public void DeleteRecord(Note note) => RepositoryContext.Set<Note>().Remove(note);

    public Task<Note> GetRecordByIdAsync(int id) => RepositoryContext.Set<Note>().Where(c => c.Id.Equals(id)).SingleOrDefaultAsync();

    public void UpdateRecord(Note note) => RepositoryContext.Set<Note>().Update(note);



    public async Task SaveAsync() => await RepositoryContext.SaveChangesAsync();

    public async Task<PagedResult<Note>> GetAllNotesWithPagination(LookupDTO lookupDto)
    {

        var connection = RepositoryContext.Database.GetDbConnection();
        await connection.OpenAsync();

        try
        {
            var q = new PagedQueryExecutor<Note>(connection,"Select id,title,content,createdAt,updatedAt from Note");
            return await q.GetPagedResultAsync(lookupDto.PageNumber,lookupDto.PageSize,lookupDto.Filters);
        }
        finally
        {
            connection.Close();
        }
        return null;
    }
}
