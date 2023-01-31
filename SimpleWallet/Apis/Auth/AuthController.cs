using System;
using System.Security.Authentication;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SimpleWallet.Models;

namespace SimpleWallet.Apis.Auth
{
    [ApiController]
    [Route("api/v{version:ApiVersion}/auth")]
    [ApiVersion("1.0")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        
        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] RegisterParam registerParam)
        {
            AccountModel accountModel = await _authService.Register(registerParam);
            
            return Ok(accountModel);
        }
    }
}