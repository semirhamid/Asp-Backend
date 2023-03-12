using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using EthioTelQuizBotBusinessLogic.Models.DTO;
using EthioTelQuizBotBusinessLogic.Models.Entity;
using Microsoft.AspNetCore.Identity;

namespace EthioTelQuizBotBusinessLogic.Interface
{
    public interface IAuthManager
    {

        Task<List<Claim>> GetValidClaims(AppUser user);
        Task<AuthResult> VerifyToken(TokenRequest tokenRequest);
        Task<AuthResult> GenerateJwtToken(AppUser user);
    }
}
