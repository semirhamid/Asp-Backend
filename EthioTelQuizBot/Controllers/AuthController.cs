using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EthioTelQuizBotBusinessLogic.Interface;
using EthioTelQuizBotBusinessLogic.Models.DTO;
using EthioTelQuizBotBusinessLogic.Models.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace EthioTelQuizBot.Controller
{

    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IAuthManager _authManager;

        public AuthController(
            UserManager<AppUser> userManager,
            IAuthManager authManager
        )
        {
            _userManager = userManager;
            _authManager = authManager;
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationRequestDto user)
        {

            // check i the user with the same email exist
            var existingUser = await _userManager.FindByNameAsync(user.UserName);

            if (existingUser != null)
            {
                return BadRequest(new RegistrationResponse()
                {
                    Result = false,
                    Errors = new List<string>(){
                                            "Username already exists"
                                        }
                });
            }

            var newUser = new AppUser()
            {
                FirstName = user.FirstName,
                MiddleName = user.MiddleName,
                LastName = user.LastName,
                Email = user.Email,
                BirthDate = user.BirthDate,
                UserName = user.UserName,
                Gender = user.Gender,
                Phone = user.Phone,
                PhoneNumber = user.Phone
            };
            var isCreated = await _userManager.CreateAsync(newUser, user.Password);
            if (isCreated.Succeeded)
            {
                var result = await _userManager.AddToRoleAsync(newUser, user.Roles);
                if (result.Succeeded)
                {
                    return Ok(await _authManager.GenerateJwtToken(newUser));
                }
                else
                {
                    return StatusCode(400, "Role is not assigned");
                }

            }
            return new JsonResult(new RegistrationResponse()
            {
                Result = false,
                Errors = isCreated.Errors.Select(x => x.Description).ToList()
            }
                    )
            { StatusCode = 500 };

        }


        [HttpPost]
        [Route("RefreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenRequest tokenRequest)
        {
            if (ModelState.IsValid)
            {
                var res = await _authManager.VerifyToken(tokenRequest);

                if (res == null)
                {
                    return BadRequest(new RegistrationResponse()
                    {
                        Errors = new List<string>() {
                    "Invalid tokens"
                },
                        Success = false
                    });
                }

                return Ok(res);
            }

            return BadRequest(new RegistrationResponse()
            {
                Errors = new List<string>() {
                "Invalid payload"
            },
                Success = false
            });
        }
        private bool CheckTemporaryPassword(string password)
        {
            var temp = password.Split("#").ElementAt(0);
            if (temp == "Temp")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private async Task<AppUser> GetCurrentUser()
        {

            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            // Console.WriteLine(userId);
            AppUser user = await _userManager.FindByNameAsync(userId);

            return user;
        }

        [HttpPost]
        [Route("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest userRequest)
        {
            AppUser user;
            if (userRequest.UserName == null)
            {
                user = await this.GetCurrentUser();
                if (user == null)
                {
                    return StatusCode(400, "User not found");
                }
            }
            else{
                Console.WriteLine("I am here");
                user = await _userManager.FindByNameAsync(userRequest.UserName);
                
                if (user == null)
                {
                    return StatusCode(400, "User not found");
                }

            }

            var result = await _userManager.ChangePasswordAsync(user, userRequest.OldPassword, userRequest.NewPassword);

            if (result.Succeeded)
            {
                return StatusCode(200, "Password is changed");
            }
            else
            {
                return StatusCode(400, "Password change request is not successful.");
            }


        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest user)
        {
            AppUser existingUser;
            if (user.Email == null && user.UserName == null)
            {
                return BadRequest(new RegistrationResponse()
                {
                    Result = false,
                    Errors = new List<string>(){
                                        "Username or Email is required"
                                    }
                });
            }
            if (user.Email != null)
            {
                existingUser = await _userManager.FindByEmailAsync(user.Email);
            }
            else
            {
                existingUser = await _userManager.FindByNameAsync(user.UserName);
            }



            if (existingUser == null)
            {
                // We dont want to give to much information on why the request has failed for security reasons
                return BadRequest(new RegistrationResponse()
                {
                    Result = false,
                    Errors = new List<string>(){
                                        "Invalid authentication request"
                                    }
                });
            }


            // Now we need to check if the user has inputed the right password
            var isCorrect = await _userManager.CheckPasswordAsync(existingUser, user.Password);

            if (isCorrect)
            {
                var loginToken = await _authManager.GenerateJwtToken(existingUser);
                if (this.CheckTemporaryPassword(user.Password))
                {
                    loginToken.IsTemporaryPassword = true;
                    return Ok(loginToken);
                }
                else
                {
                    loginToken.IsTemporaryPassword = false;
                    return Ok(loginToken);
                }


            }
            else
            {
                // We dont want to give to much information on why the request has failed for security reasons
                return BadRequest(new RegistrationResponse()
                {
                    Result = false,
                    Errors = new List<string>(){
                                         "Invalid authentication request"
                                    }
                });
            }


        }

    }

}