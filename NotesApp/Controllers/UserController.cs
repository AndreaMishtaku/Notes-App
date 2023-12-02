using IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.DTO;
using Shared.Request;
using Shared.Response;

namespace NotesApp.Controllers;

[Route("api/users")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserService _service;

    public UserController(IUserService service)
    {
        _service = service;
    }

    [HttpPost()]
    public async Task<IActionResult> CreateUser([FromBody] NewUserRequestDTO userDto)
    {
        var result = await _service.CreateUser(userDto);
        return Ok(result);
    }

  

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(int id)
    {
        var result = await _service.GetUserById(id);
        return Ok(result);
    }



    [HttpPost("get-all")]
    public async Task<IActionResult> GetAllUsersWithPagination([FromBody] LookupDTO filter)
    {
        var result = await _service.GetAllUsersWithPagination(filter);
        return Ok(result);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] NewUserRequestDTO userDto)
    {
        var result = await _service.CreateUser(userDto);
        var baseResponse = new BaseResponse
        {
            Message = "User registered!",
            StatusCode = 200
        };
        return Ok(baseResponse);


    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDTO loginDto)
    {
        var tokenDto = await _service.ValidateUserAndCreateToken(loginDto);

        return Ok(tokenDto);

    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] TokenDTO tokenDto)
    {
        var tokenDtoToReturn = await _service.RefreshToken(tokenDto);
        return Ok(tokenDtoToReturn);
    }
}

