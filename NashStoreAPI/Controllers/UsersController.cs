using NashPhaseOne.BusinessObjects.Models;
using DTO.Models.Authen;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NashPhaseOne.DTO.Models.Authen;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DTO.Models;
using DAO.Interfaces;
using AutoMapper;
using NashPhaseOne.API.Statics;
using NashPhaseOne.API.Filters;
using Microsoft.AspNetCore.Authorization;
using NashPhaseOne.DTO.Models;

namespace NashStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UsersController(IMapper mapper, UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, IUserRepository userRepository)
        {
            _mapper = mapper;
            _userRepository = userRepository;
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        [TypeFilter(typeof(CustomAuthorizeFilter))]
        public async Task<ViewListDTO<UserInfo>> GetUsers([FromQuery]int pageIndex = 1)
        {
            var users = _userManager.Users;
            var listusers = await _userRepository.PagingAsync(users, pageIndex);
            if(listusers == null)
            {
                return new ViewListDTO<UserInfo>();
            }
            var castedList = _mapper.Map<List<UserInfo>>(listusers.ModelDatas);
            return new ViewListDTO<UserInfo> { ModelDatas = castedList, PageIndex = listusers.PageIndex, MaxPage = listusers.MaxPage };
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                if (user.IsBanned)
                {
                    return Unauthorized("Your account has been banned!");
                }
                var userRoles = await _userManager.GetRolesAsync(user);
                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                var tokenClaims = GetToken(authClaims);
                var token = new JwtSecurityTokenHandler().WriteToken(tokenClaims);
                ListOfActiveTokens.ActiveTokens.Add(token);
                return Ok(new Token
                {
                    TokenString = token,
                    Expiration = tokenClaims.ValidTo,
                    UserInfo = new UserDTO { Id = user.Id , Roles = (List<string>)userRoles, UserName = user.NormalizedUserName},
                });
            }
            return Unauthorized();
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            var userExists = await _userManager.FindByNameAsync(model.Username);
            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User already exists!" });

            User user = new()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if(result.Errors.Count() > 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = result.Errors.First().Code });
            }
            bool customerRoleExits = await _roleManager.RoleExistsAsync(UserRoles.Customer);
            if (!customerRoleExits)
            {
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.Customer));
            }
            await _userManager.AddToRoleAsync(user, UserRoles.Customer);
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User creation failed! Please check user details and try again." });

            return Ok(new Response { Status = "Success", Message = "User created successfully!" });
        }

        [HttpPost]
        [Route("register-admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterModel model)
        {
            var userExists = await _userManager.FindByNameAsync(model.Username);
            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User already exists!" });

            User user = new()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User creation failed! Please check user details and try again." });

            if (!await _roleManager.RoleExistsAsync(UserRoles.Admin))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
            if (!await _roleManager.RoleExistsAsync(UserRoles.Customer))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.Customer));

            if (await _roleManager.RoleExistsAsync(UserRoles.Admin))
            {
                await _userManager.AddToRoleAsync(user, UserRoles.Admin);
            }
            if (await _roleManager.RoleExistsAsync(UserRoles.Admin))
            {
                await _userManager.AddToRoleAsync(user, UserRoles.Customer);
            }
            return Ok(new Response { Status = "Success", Message = "User created successfully!" });
        }

        [HttpDelete]
        [Route("logout")]
        [Authorize]
        [TypeFilter(typeof(CustomAuthorizeFilter))]
        public async Task<IActionResult> Logout()
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString().Split(" ")[1];
            if(token == null)
            {
                return BadRequest();
            }
            else
            {
                ListOfActiveTokens.ActiveTokens.Remove(token);
            }
            return Ok();
        }

        [HttpPut("toggle")]
        [Authorize(Roles = "Admin")]
        [TypeFilter(typeof(CustomAuthorizeFilter))]
        public async Task<IActionResult> ToggleUserStatus(IdString idString)
        {
            var user = await _userManager.FindByIdAsync(idString.Id);
            if(user == null)
            {
                return BadRequest(new { message = "Can't find" });
            }
            else
            {
                user.IsBanned = !user.IsBanned;
                await _userManager.UpdateAsync(user);
            }
            return Ok();
        }

        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            return token;
        }
    }
}
