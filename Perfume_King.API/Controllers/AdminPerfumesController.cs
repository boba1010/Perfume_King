using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Perfume_King.API.DbContexts;
using Perfume_King.API.Models;
using Perfume_King.API.Services;
using Perfume_King.Shared.Contracts.Requests;

namespace Perfume_King.API.Controllers
{
    [Authorize(Roles = "admin")]
    [ApiController]
    [Route("api/admin/perfumes")]
    public class AdminPerfumesController(AppDbContext dbContext, CloudinaryClient cloudinaryClient) : ControllerBase
    {
        [HttpPost("create")]
        public async Task<IActionResult> CreatePerfume([FromForm] CreatePerfumeRequest request)
        {
            //var imgUri = await cloudinaryClient.UploadImageAsync(request.Image);
            //if (imgUri == null)
            //    return BadRequest("Cloud upload failed");

            Perfume perfume = new()
            {
                Name = request.Name,
                Price = request.Price,
                Category = (Models.Category)request.Category,
                IsAvailable = request.IsAvailable,
                ImageUri = "imgUri",
            };

            dbContext.Perfumes.Add(perfume);
            await dbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdatePerfume([FromForm] UpdatePerfumeRequest request)
        {
            try
            {
                var savedPerfume = await dbContext.Perfumes.FirstOrDefaultAsync(p => p.Id == request.Id);
                if (savedPerfume == null)
                    return NotFound("Perfume not found");

                if (!string.IsNullOrWhiteSpace(request.Name))
                    savedPerfume.Name = request.Name.Trim();

                if (request.Price.HasValue)
                    savedPerfume.Price = request.Price.Value;

                if (request.IsAvailable.HasValue)
                    savedPerfume.IsAvailable = request.IsAvailable.Value;

                if (request.Category.HasValue)
                    savedPerfume.Category = (Category)request.Category.Value;

                if (request.Image != null)
                {
                    //var imgUri = await cloudinaryClient.UploadImageAsync(request.Image);
                    //if (imgUri == null)
                    //    return BadRequest("Cloud upload failed");

                    //savedPerfume.ImageUri = imgUri;
                }

                await dbContext.SaveChangesAsync();
                return Ok("Perfume updated successfully");
            }
            catch (Exception)
            {
                return StatusCode(500, "An internal error occurred.");
            }
        }


        [HttpPost("delete")]
        public async Task<IActionResult> DeletePerfume([FromBody] int perfumeId)
        {
            try
            {
                var perfume = await dbContext.Perfumes.FirstOrDefaultAsync(p => p.Id == perfumeId);
                if (perfume == null)
                    return NotFound("Perfume was not found");

                if (!string.IsNullOrEmpty(perfume.ImageUri))
                {
                    // Extract the public ID from your URI and delete it from Cloudinary
                    // await cloudinaryClient.DeleteImageAsync(perfume.ImageUri);
                }

                var linkedCartItems = dbContext.Set<CartItem>().Where(ci => ci.PerfumeId == perfumeId);
                dbContext.RemoveRange(linkedCartItems);

                dbContext.Remove(perfume);

                await dbContext.SaveChangesAsync();
                return Ok("Perfume completely removed.");
            }
            catch (Exception)
            {
                return StatusCode(500, "An internal error occurred while deleting the product.");
            }
        }

    }
}
