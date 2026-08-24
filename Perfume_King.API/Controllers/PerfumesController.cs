using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Perfume_King.Shared.Contracts.Requests;
using Perfume_King.API.DbContexts;
using Perfume_King.Shared.DTOs;
using Perfume_King.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Perfume_King.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PerfumesController(AppDbContext context) : ControllerBase
    {
        private static PerfumeDTO MapPerfume(Perfume product)
        {
            return new()
            {
                Id = product.Id,
                Category = (Shared.DTOs.Category)product.Category,
                IsAvailable = product.IsAvailable,
                SaleNumber = product.SaleNumber,
                ImageUri = product.ImageUri,
                Name = product.Name,
                Price = product.Price,
            };
        }

        [HttpGet]
        public async Task<IActionResult> GetPerfumes()
        {
            var products = await context.Perfumes
                .AsNoTracking()
                .ToArrayAsync();

            var productsDTO = new List<PerfumeDTO>(products.Length);
            foreach (var product in products)
                productsDTO.Add(MapPerfume(product));
            return Ok(productsDTO);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetPerfumes([FromRoute] int id)
        {
            var product = await context.Perfumes
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);
            if (product == null)
                return NotFound("Product was not found.");
            return Ok(MapPerfume(product));
        }

        [HttpGet("featured-perfumes")]
        public async Task<IActionResult> GetFeaturedPerfumes()
        {
            var featuredProducts = await context.Perfumes
                .AsNoTracking()
                .OrderByDescending(p => p.SaleNumber)
                .Take(8)
                .ToArrayAsync();

            var featuredProductsDTO = new List<PerfumeDTO>(featuredProducts.Length);
            foreach (var featuredProduct in featuredProducts)
                featuredProductsDTO.Add(MapPerfume(featuredProduct));

            return Ok(featuredProductsDTO);
        }
    }
}
