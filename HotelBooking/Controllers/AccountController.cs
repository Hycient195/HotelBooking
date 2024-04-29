using AutoMapper;
using HotelBooking.Data;
using HotelBooking.Core.DTOs;
using HotelBooking.Core.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApiUser> _userManager;
        private readonly ILogger<AccountController> _logger;
        private readonly IMapper _mapper;
        private readonly IAuthManager _authManager;

        // Controller Dependency Injection
        public AccountController(
            UserManager<ApiUser> userManager,
            ILogger<AccountController> logger,
            IMapper mapper,
            IAuthManager authManager
        )
        {
            _userManager = userManager;
            _logger = logger;
            _mapper = mapper;
            _authManager = authManager;
        }

        [HttpPost]
        [Route("sign-up")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Register([FromBody] UserDTO userDTO)
        {
            _logger.LogInformation($"Registration attempt for {userDTO.Email}");
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            try
            {
                var user = _mapper.Map<ApiUser>(userDTO);
                user.UserName = userDTO.Email;
                var result = await _userManager.CreateAsync(user, userDTO.Password);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(error.Code, error.Description);
                    }
                    return BadRequest(ModelState);
                }

                await _userManager.AddToRolesAsync(user, userDTO.Roles);
                return Accepted(); 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something went wrong in {nameof(Register)}");
                return Problem($"Something went wrong in {nameof(Register)}", statusCode: 500);
            }
        }

        [HttpPost]
        [Route("sign-in")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> SignIn([FromBody] LoginUserDTO userDTO)
        {
            _logger.LogInformation($"Sign in attempt for user {userDTO.Email}");
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            try
            {
                if (!await _authManager.ValidateUser(userDTO))
                {
                    return Unauthorized();
                }

                return Accepted(new { Token = await _authManager.CreateToken() });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something went wrong in {nameof(SignIn)}");
                return Problem($"Something went wrong in {nameof(SignIn)}", statusCode: 500);
            }
        }

        /*[HttpPost]
        [Route("sign-in")]
        public async Task<IActionResult> Login([FromBody] LoginUserDTO userDTO)
        {
            _logger.LogInformation($"Login attempt for {userDTO.Email}");
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _signInManager.PasswordSignInAsync(userDTO.Email, userDTO.Password, false, false);
                if (!result.Succeeded)
                {
                    return Unauthorized("User credentials are incorrect");
                }
                return Accepted("Sign in sucessful");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something went wrong in {nameof(Login)}");
                return Problem($"Error in user sign in", statusCode: 400);
            }
        }*/
    }
}
