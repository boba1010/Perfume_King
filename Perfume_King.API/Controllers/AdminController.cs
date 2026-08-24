using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Perfume_King.API.DbContexts;
using Perfume_King.API.Models;
using Perfume_King.API.Services;
using Perfume_King.Shared.Contracts.Requests;
using Perfume_King.Shared.Contracts.Responses;
using System.Security.Claims;
using System.Security.Cryptography;

namespace Perfume_King.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController(AppDbContext dbContext, IAuthService authService) : ControllerBase
    {
        [HttpPost("auth/login")]
        public async Task<IActionResult> Login([FromBody] RequestAdminLogin request)
        {
            var admin = dbContext.AdminUsers.FirstOrDefault(u => u.Username == request.Username);
            if (admin == null)
                return NotFound("Admin user is not found");

            bool isVerfied = authService.VerifyPassword(request.Password, admin.Password, admin.Salt);
            if (!isVerfied)
                return Unauthorized("Wrong password");

            string token = authService.GenerateJwtToken(admin.Id, "admin");
            var refreshToken = new RefreshToken
            {
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(30),
                IsRevoked = false,
                UserId = admin.Id,
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64))
            };

            dbContext.RefreshTokens.Add(refreshToken);
            await dbContext.SaveChangesAsync();

            return Ok(new AdminLoginResponse
            {
                Token = token,
                RefreshToken = refreshToken.Token,
            });
        }

        private int GetAdminId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
                throw new UnauthorizedAccessException();
            return userId;
        }

        [Authorize(Roles = "admin")]
        [HttpGet("auth/verify")]
        public async Task<IActionResult> VerifyUser()
        {
            var user = dbContext.AdminUsers.FirstOrDefault(u => u.Id == GetAdminId());
            if (user == null)
                return NotFound("User was not found.");

            return Ok(true);
        }
    }
}
