using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRepository;

public interface IUserRepository
{
    void CreateRecord(User user);
    void UpdateRecord(User user);
    void DeleteRecord(User user);
    Task<User> GetRecordByIdAsync(int id);

    Task SaveAsync();
}
