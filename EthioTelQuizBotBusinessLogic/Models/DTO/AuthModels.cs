using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace EthioTelQuizBotBusinessLogic.Models.DTO
{
    public class AuthResult
    {
        public string Token { get; set; }
        public bool Result { get; set; }
        public bool Success { get; set; }
        public List<string> Errors { get; set; }
        public string RefreshToken { get; set; }
        public bool? IsTemporaryPassword { get; set; }
        public string[] Roles { get; set; }
        public string[] Permissions { get; set; }
    }

    public class RegistrationResponse : AuthResult
    {

    }

    public class UserRegistrationRequestDto
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string MiddleName { get; set; }
        [Required]
        public string LastName { get; set; }
        public string? Phone { get; set; }
        [Required]
        public DateTime BirthDate { get; set; }

        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Gender { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Roles { get; set; }
    }

    public class UserLoginRequest
    {
        public string? UserName { get; set; }
        public string? Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
    public class ChangePasswordRequest
    {
        public string? UserName { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }



    public class TokenRequest
    {
        [Required]
        public string Token { get; set; }
        [Required]
        public string RefreshToken { get; set; }
    }


}