using AutoMapper;
using Entities;
using Exceptions;
using IRepository;
using IService;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Shared.Configuration;
using Shared.DTO;
using Shared.Request;
using Shared.Response;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Service;

public class UserService : IUserService
{
    private readonly IMapper _mapper;
    private readonly IUserRepository _userRepository;
    private readonly UserManager<User> _userManager;
    private readonly IOptions<JwtConfiguration> _configuration;
    private readonly JwtConfiguration _jwtConfiguration;
    private readonly SignInManager<User> _signInManager;

    public UserService(
        IMapper mapper,
        IUserRepository userRepository,
        UserManager<User> userManager,
        IOptions<JwtConfiguration> configuration,
        SignInManager<User> signInManager
        )
    {
      
        _mapper = mapper;
        _userRepository = userRepository;
        _userManager = userManager;
        _configuration = configuration;
        _jwtConfiguration = configuration.Value;
        _signInManager=signInManager;
    }

    public async Task<IdentityResult> CreateUser(NewUserRequestDTO userDto)
    {
        try
        {
            var user = _mapper.Map<User>(userDto);
            user.UserName = userDto.Email;


            var result = await _userManager.CreateAsync(user, userDto.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "User");
            }

            return result;
        }
        catch (Exception ex)
        {
            throw new BadRequestException(ex.Message);
        }
    }


    public async Task<UserDTO> GetUserById(int userId)
    {
        try
        {
            var existingUser = await GetUserAndCheckIfExistsAsync(userId);

            var userById = _mapper.Map<UserDTO>(existingUser);

            return userById;
        }
        catch (Exception ex)
        {
            throw new BadRequestException(ex.Message);
        }
    }
    public Task<TableResponse<UserDTO>> GetAllUsersWithPagination(LookupDTO filter)
    {
        throw new NotImplementedException();
    }
    public async Task<TokenDTO> RefreshToken(TokenDTO tokenDto)
    {
        try
        {
            var principal = GetPrincipalFromExpiredToken(tokenDto.AccessToken);

            var currentUserEmail = principal.Claims.Where(x => x.Type == "Email").FirstOrDefault();
            if (currentUserEmail is null)
                throw new BadRequestException("Invalid client request. The token has some invalid values.");

            var user = await _userManager.FindByEmailAsync(currentUserEmail.Value);
            if (user == null || user.RefreshToken != tokenDto.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
                throw new BadRequestException("Invalid client request. The token has some invalid values.");

            return await CreateToken(user, false);
        }
        catch (Exception ex)
        {
            throw new BadRequestException(ex.Message);
        }
    }

    public async Task<TokenDTO> ValidateUserAndCreateToken(LoginDTO loginDTO)
    {
        try
        {
            User currentUser = await _userManager.FindByNameAsync(loginDTO.Email);

            if (currentUser is null)
                throw new BadRequestException($"User with email {loginDTO.Email} doesnt exist");


            var validateUser = await _signInManager.PasswordSignInAsync(currentUser, loginDTO.Password, false, lockoutOnFailure: true);


            if (!validateUser.Succeeded)
            {
                throw new BadRequestException("E-mail or password is incorrect!");

            }
            else
            {
                var tokenDto = await CreateToken(currentUser, true);
                return tokenDto;
            }

        }
        catch (Exception ex)
        {
            throw new BadRequestException(ex.Message);
        }
    }


    private async Task<User> GetUserAndCheckIfExistsAsync(int userId)
    {
        var existingUser = await _userRepository.GetRecordByIdAsync(userId);
        if (existingUser is null)
            throw new NotFoundException(string.Format("User with Id: {0} doesnt exist!", userId));

        return existingUser;
    }

    private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        int refreshTokenExpire = Convert.ToInt32(_jwtConfiguration.RefreshTokenExpire);
        int tokenExpire = Convert.ToInt32(_jwtConfiguration.Expires);

        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateLifetime = false,
            ClockSkew = TimeSpan.FromMinutes(refreshTokenExpire - tokenExpire),
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfiguration.SecretKey)),
            ValidIssuer = _jwtConfiguration.ValidIssuer,
            ValidAudience = _jwtConfiguration.ValidAudience
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        SecurityToken securityToken;
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);

        var jwtSecurityToken = securityToken as JwtSecurityToken;
        if (jwtSecurityToken == null)
            throw new SecurityTokenException("Token is uncorrect");

        return principal;
    }

    private async Task<TokenDTO> CreateToken(User? currentUser, bool populateExp)
    {
        try
        {
            if (currentUser is not null)
            {
                var signingCredentials = GetSigningCredentials();
                var claims = await GetClaims(currentUser);

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = claims,
                    NotBefore = DateTime.UtcNow,
                    IssuedAt = DateTime.UtcNow,
                    Expires = DateTime.Now.AddMinutes(Convert.ToDouble(_jwtConfiguration.Expires)),
                    SigningCredentials = signingCredentials,
                    Audience = _jwtConfiguration.ValidAudience,
                    Issuer = _jwtConfiguration.ValidIssuer
                };

                var refreshToken = GenerateRefreshToken();
                currentUser.RefreshToken = refreshToken;

                if (populateExp)
                    currentUser.RefreshTokenExpiryTime = DateTime.Now.AddMinutes(Convert.ToDouble(_jwtConfiguration.RefreshTokenExpire));

                await _userManager.UpdateAsync(currentUser);

                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var accessToken = tokenHandler.WriteToken(token);

                return new TokenDTO(accessToken, refreshToken);
            }
            return new TokenDTO("", "");
        }
        catch (Exception ex)
        {
            throw new BadRequestException(ex.Message);
        }
    }

    private async Task<ClaimsIdentity> GetClaims(User currentUser)
    {
        var claims = new List<Claim>
             {
                new Claim("Id", currentUser.Id.ToString()),
                new Claim("FirstName", currentUser.FirstName),
                new Claim("LastName", currentUser.LastName),
                new Claim("Email", currentUser.Email),
            };

        var roles = await _userManager.GetRolesAsync(currentUser);
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        return new ClaimsIdentity(claims);
    }

    private SigningCredentials GetSigningCredentials()
    {
        var key = Encoding.UTF8.GetBytes(_jwtConfiguration.SecretKey);
        var secret = new SymmetricSecurityKey(key);

        return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256Signature);
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}
