using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Perfume_King.API.DbContexts;
using Perfume_King.API.Models;
using Perfume_King.Shared.Contracts.Requests;
using Perfume_King.Shared.DTOs;
using System.Security.Claims;

namespace Perfume_King.API.Controllers
{
    [Authorize(Roles = "user")]
    [ApiController]
    [Route("api/cart")]
    public class CartController(AppDbContext dbContext) : ControllerBase
    {
        private int GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
                throw new UnauthorizedAccessException();
            return userId;
        }

        private async Task<CartDTO?> GetCartAsync(int userId)
        {
            var user = await dbContext.Users
                .Include(u => u.Cart)
                    .ThenInclude(c => c.CartItems)
                .FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                return null;

            var cart = user.Cart;
            var cartItems = cart.CartItems;

            HashSet<int> perfumeIds = [];
            for (int i = 0; i < cartItems.Count; i++)
            {
                int id = cartItems[i].PerfumeId;
                perfumeIds.Add(id);
            }

            var perfumesList = await dbContext.Perfumes
                .AsNoTracking()
                .Where(p => perfumeIds.Contains(p.Id))
                .ToListAsync();

            Dictionary<int, Perfume> perfumeLookup = [];
            for (int i = 0; i < perfumesList.Count; i++)
            {
                var p = perfumesList[i];
                perfumeLookup[p.Id] = p;
            }

            List<CartItemDTO> cartItemsDTOs = new(cartItems.Count);

            for (int i = 0; i < cartItems.Count; i++)
            {
                var item = cartItems[i];

                if (perfumeLookup.TryGetValue(item.PerfumeId, out var perfume))
                {
                    cartItemsDTOs.Add(new CartItemDTO
                    {
                        Id = item.Id,
                        Quantity = item.Quantity,
                        PerfumeId = item.PerfumeId,
                        PerfumeName = perfume.Name,
                        UnitPrice = perfume.Price,
                        ImageUri = perfume.ImageUri
                    });
                }
            }

            return new CartDTO
            {
                Id = cart.Id,
                CartItems = cartItemsDTOs,
                UpdatedAt = cart.UpdatedAt,
            };
        }

        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            try
            {
                int id = GetUserId();

                var cart = await GetCartAsync(id);
                if (cart == null)
                    return NotFound("Cart was not found.");

                return Ok(cart);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized("Invalid or missing user context.");
            }
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddToCart([FromBody] int perfumeId)
        {
            try
            {
                var userId = GetUserId();

                var cart = await dbContext.Set<Cart>()
                    .Include(c => c.CartItems.Where(ci => ci.PerfumeId == perfumeId))
                    .FirstOrDefaultAsync(c => c.UserId == userId);

                if (cart == null)
                    return NotFound("Cart was not found for this user.");

                var existingItem = cart.CartItems.FirstOrDefault();
                if (existingItem != null)
                {
                    return Ok("Item is already in the cart");
                }

                cart.CartItems.Add(new CartItem
                {
                    PerfumeId = perfumeId,
                    Quantity = 1
                });

                cart.UpdatedAt = DateTime.UtcNow;

                await dbContext.SaveChangesAsync();
                return Ok("Item added to the cart");
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized("Invalid or missing user context.");
            }
            catch (Exception)
            {
                return StatusCode(500, "An internal error occurred.");
            }
        }


        [HttpPost("remove")]
        public async Task<IActionResult> RemoveFromCart([FromBody] int id)
        {
            try
            {
                var userId = GetUserId();

                var cart = await dbContext.Set<Cart>()
                    .Include(c => c.CartItems.Where(ci => ci.Id == id))
                    .FirstOrDefaultAsync(c => c.UserId == userId);

                if (cart == null)
                    return NotFound("Cart was not found for this user.");

                var cartItem = cart.CartItems.FirstOrDefault();

                if (cartItem == null)
                    return NotFound("Cart item was not found in this user's cart.");

                dbContext.Remove(cartItem);

                cart.UpdatedAt = DateTime.UtcNow;

                await dbContext.SaveChangesAsync();

                return Ok("Item removed from the cart");
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized("Invalid or missing user context.");
            }
            catch (Exception)
            {
                return StatusCode(500, "An internal error occurred.");
            }
        }


        [HttpPost("change-item-qty")]
        public async Task<IActionResult> ChangeItemQty([FromBody] RequestCartItemQtyChange request)
        {
            try
            {
                var userId = GetUserId();

                var cart = await dbContext.Set<Cart>()
                    .Include(c => c.CartItems.Where(ci => ci.Id == request.CartItemId))
                    .FirstOrDefaultAsync(c => c.UserId == userId);

                if (cart == null)
                    return NotFound("Cart was not found for this user.");

                var cartItem = cart.CartItems.FirstOrDefault();

                if (cartItem == null)
                    return NotFound("Cart item was not found in this user's cart.");

                int newQuantity = request.CartItemQty;

                if (newQuantity <= 0)
                {
                    dbContext.Remove(cartItem);
                    cart.UpdatedAt = DateTime.UtcNow;
                    await dbContext.SaveChangesAsync();
                    return Ok("Item removed from cart because quantity reached zero.");
                }

                cartItem.Quantity = newQuantity;
                cart.UpdatedAt = DateTime.UtcNow;

                await dbContext.SaveChangesAsync();

                string statusMessage = request.CartItemQty > 0 ? "increased" : "decreased";
                return Ok($"Item quantity {statusMessage} successfully.");
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized("Invalid or missing user context.");
            }
            catch (Exception)
            {
                return StatusCode(500, "An internal error occurred.");
            }
        }
    }
}
