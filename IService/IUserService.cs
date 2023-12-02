using Microsoft.AspNetCore.Identity;
using Shared.DTO;
using Shared.Request;
using Shared.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IService;

public interface IUserService
{
    Task<IdentityResult> CreateUser(NewUserRequestDTO userDto);
    Task<UserDTO> GetUserById(int userId);
    Task<TableResponse<UserDTO>> GetAllUsersWithPagination(LookupDTO filter);
    Task<TokenDTO> ValidateUserAndCreateToken(LoginDTO loginDTO);
    Task<TokenDTO> RefreshToken(TokenDTO tokenDto);
}
