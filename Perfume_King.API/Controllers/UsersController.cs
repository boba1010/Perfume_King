using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Perfume_King.API.DbContexts;
using Perfume_King.API.Models;
using Perfume_King.API.Services;
using Perfume_King.Shared.Contracts.Requests;
using Perfume_King.Shared.Contracts.Responses;
using Perfume_King.Shared.DTOs;
using System.Security.Claims;
using System.Security.Cryptography;

namespace Perfume_King.API.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UsersController(AppDbContext dbContext, IAuthService authService) : ControllerBase
    {
        [HttpPost("auth/login")]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest request)
        {
            var savedUser = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (savedUser == null)
                return NotFound("User was not found.");

            var isVerified = authService.VerifyPassword(request.Password, savedUser.HashPassword, savedUser.Salt);
            if (!isVerified)
                return Unauthorized("Wrong passwrod.");

            var accessToken = authService.GenerateJwtToken(savedUser.Id, "user");
            var refreshToken = new RefreshToken
            {
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(30),
                IsRevoked = false,
                UserId = savedUser.Id,
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64))
            };

            dbContext.RefreshTokens.Add(refreshToken);
            await dbContext.SaveChangesAsync();

            return Ok(new UserLoginResponse
            {
                Token = accessToken,
                RefreshToken = refreshToken.Token,
            });
        }

        [HttpPost("auth/signup")]
        public async Task<IActionResult> Signup([FromBody] UserSignupRequest request)
        {
            var userTest = dbContext.Users.FirstOrDefault(u => u.Email == request.Email);
            if (userTest != null)
                return BadRequest("Email is already used.");

            var salt = authService.GenerateSalt();
            var hashedPassword = authService.HashPassword(request.Password, salt);

            User user = new()
            {
                FullName = request.FullName,
                HashPassword = hashedPassword,
                Email = request.Email,
                Address = "",
                Phone = "",
                City = "",
                Salt = salt,
                Cart = new() { CartItems = [] },
            };
            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();

            var savedUser = dbContext.Users.FirstOrDefault(u => u.Email == request.Email);
            var token = authService.GenerateJwtToken(savedUser.Id, "user");
            var refreshToken = new RefreshToken
            {
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(30),
                IsRevoked = false,
                UserId = savedUser.Id,
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64))
            };

            dbContext.RefreshTokens.Add(refreshToken);
            await dbContext.SaveChangesAsync();

            return Ok(new UserSignupResponse
            {
                Token = token,
                RefreshToken = refreshToken.Token,
            });
        }

        [HttpPost("auth/refresh")]
        public async Task<IActionResult> RefreshSession([FromBody] RequestRefreshToken request)
        {
            var refreshToken = dbContext.RefreshTokens.FirstOrDefault(x => x.Token == request.Token);
            if (refreshToken == null)
                return NotFound();

            if (refreshToken.ExpiresAt <= DateTime.UtcNow
                || refreshToken.IsRevoked)
                return Unauthorized();

            int id = request.Role == "admin" ?
                await dbContext.AdminUsers
                    .Select(u => u.Id)
                    .FirstOrDefaultAsync(id => id == refreshToken.UserId)
                : await dbContext.Users
                    .Select(u => u.Id)
                    .FirstOrDefaultAsync(id => id == refreshToken.UserId);

                if (id == 0)
                    return NotFound();
            var accessToken = authService.GenerateJwtToken(id, request.Role);
            var newRefreshToken = new RefreshToken()
            {
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(30),
                IsRevoked = false,
                UserId = id,
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64))
            };
            dbContext.RefreshTokens.Update(refreshToken);
            dbContext.RefreshTokens.Add(newRefreshToken);
            refreshToken.IsRevoked = true;
            await dbContext.SaveChangesAsync();

            return Ok(new RefreshTokenResponse()
            {
                AccessToken = accessToken,
                RefreshToken = newRefreshToken.Token
            });
        }

        private int GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
                throw new UnauthorizedAccessException();
            return userId;
        }

        [Authorize(Roles = "user")]
        [HttpGet("auth/verify")]
        public async Task<IActionResult> VerifyUser()
        {
            var user = dbContext.Users.FirstOrDefault(u => u.Id == GetUserId());
            if (user == null)
                return NotFound("User was not found.");

            return Ok(true);
        }

        public async Task<UserProfileDTO?> GetProfileAsync(int userId)
        {
            var user = await dbContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId);
            
            if (user == null)
                return null;

            var orders = await dbContext.OrderRequests
                .AsNoTracking()
                .Where(o => o.UserId == userId)
                .Include(o => o.Items)
                .ToListAsync();

            var orderSummaries = new List<ProfileOrderSummaryDTO>(orders.Count);
            for (int i = 0; i < orders.Count; i++)
            {
                var order = orders[i];
                var orderItems = order.Items;

                var orderItemSummaries = new List<ProfileOrderItemSummaryDTO>(orderItems.Count);
                for (int j = 0; j < orderItems.Count; j++)
                {
                    var orderItem = orderItems[j];
                    orderItemSummaries.Add(new()
                    {
                        Name = orderItem.Name,
                        Quantity = orderItem.Quantity,
                    });
                }

                orderSummaries.Add(new()
                {
                    Id = order.Id,
                    CreatedAt = order.CreatedAt,
                    OrderNumber = order.OrderNumber,
                    Status = (Shared.DTOs.OrderStatus)order.Status,
                    TotalPrice = order.TotalPrice,
                    TotalItemsCount = orderItemSummaries.Count,
                    Items = orderItemSummaries
                });
            }

            return new UserProfileDTO
            {
                Id = user.Id,
                Address = user.Address,
                City = user.City,
                Email = user.Email,
                FullName = user.FullName,
                Phone = user.Phone,
                Orders = orderSummaries,
            };
        }


        [Authorize(Roles = "user")]
        [HttpGet("profile")]
        public async Task<IActionResult> GetUserProfile()
        {
            try
            {
                int userId = GetUserId();

                var profile = await GetProfileAsync(userId);
                if (profile == null)
                    return NotFound("User profile was not found.");

                return Ok(profile);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized("Invalid or missing user context.");
            }
        }

        [Authorize(Roles = "user")]
        [HttpPost("profile/update")]
        public async Task<IActionResult> UpdateProfile([FromBody] RequestUserProfileUpdate request)
        {
            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == GetUserId());
            if (user == null)
                return NotFound("User profile not found.");

            if (!string.IsNullOrEmpty(request.Phone))
                user.Phone = request.Phone?.Trim()!;
            else if (!string.IsNullOrEmpty(request.Address))
                user.Address = request.Address?.Trim()!;
            else if (!string.IsNullOrEmpty(request.City))
                user.City = request.City?.Trim()!;

            await dbContext.SaveChangesAsync();

            return Ok("Successfully updated the profile.");
        }
    }
}
