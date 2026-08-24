using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Perfume_King.API.DbContexts;
using Perfume_King.API.Models;
using Perfume_King.Shared.DTOs;

namespace Perfume_King.API.Controllers
{
    [Authorize(Roles = "admin")]
    [ApiController]
    [Route("api/admin/users")]
    public class UsersAdminController(AppDbContext dbContext) : ControllerBase
    {
        private async Task<UserDTO> MapUserToDTOAsync(User user)
        {
            return new()
            {
                Address = user.Address,
                City = user.City,
                Email = user.Email,
                FullName = user.FullName,
                Phone = user.Phone,
            };
        }

        private async Task<List<UserDTO>> MapUsersToDTOsAsync(List<User> users)
        {
            List<UserDTO> userDTOs = new(users.Count);
            foreach (var user in users)
                userDTOs.Add(await MapUserToDTOAsync(user));
            return userDTOs;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await dbContext.Users
                .AsNoTracking()
                .ToListAsync();

            var usersDTO = await MapUsersToDTOsAsync(users);
            return Ok(usersDTO);
        }
    }
}
