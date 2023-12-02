using Entities;
using IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository;

public class UserRepository:IUserRepository
{
    protected RepositoryContext RepositoryContext;

    public UserRepository(RepositoryContext repositoryContext)=> RepositoryContext = repositoryContext;

    public void CreateRecord(User user)=> RepositoryContext.Set<User>().Add(user);

    public void DeleteRecord(User user)=>RepositoryContext.Set<User>().Remove(user);

    public Task<User> GetRecordByIdAsync(int id) => RepositoryContext.Set<User>().Where(c => c.Id.Equals(id)).SingleOrDefaultAsync();

    public void UpdateRecord(User user) => RepositoryContext.Set<User>().Update(user);

    public async Task SaveAsync() => await RepositoryContext.SaveChangesAsync();
    
}
