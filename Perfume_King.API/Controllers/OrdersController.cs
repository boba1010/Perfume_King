using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Perfume_King.API.DbContexts;
using Perfume_King.API.Models;
using Perfume_King.Shared.DTOs;
using System.Security.Claims;

namespace Perfume_King.API.Controllers
{
    [Authorize(Roles = "user")]
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController(AppDbContext dbContext) : ControllerBase
    {
        private int GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
                throw new UnauthorizedAccessException();
            return userId;
        }

        private async Task<List<OrderRequestDTO>> GerOrdersAsync(int userId)
        {
            var orders = await dbContext.OrderRequests
                .AsNoTracking()
                .Where(o => o.UserId == userId)
                .Include(o => o.Items)
                .ToListAsync();

            var orderRequestsDTO = new List<OrderRequestDTO>(orders.Count);
            for (int i = 0; i < orders.Count; i++)
            {
                var order = orders[i];

                var orderItems = order.Items;

                List<OrderItemDTO> orderItemsDTO = new(orderItems.Count);
                for (int j = 0; j < orderItems.Count; j++)
                {
                    var orderItem = orderItems[j];
                    orderItemsDTO.Add(new()
                    {
                        Id = orderItem.Id,
                        Name = orderItem.Name,
                        OrderRequestId = orderItem.OrderRequestId,
                        PerfumeId = orderItem.PerfumeId,
                        Price = orderItem.Price,
                        Quantity = orderItem.Quantity,
                    });
                }

                orderRequestsDTO.Add(new()
                {
                    Id = order.Id,
                    Address = order.Address,
                    City = order.City,
                    CreatedAt = order.CreatedAt,
                    CustomerName = order.CustomerName,
                    Items = orderItemsDTO,
                    OrderNumber = order.OrderNumber,
                    Phone = order.Phone,
                    Status = (Shared.DTOs.OrderStatus)order.Status,
                });
            }

            return orderRequestsDTO;
        }

        [HttpGet]
        public async Task<IActionResult> GetOrders()
        {
            try
            {
                var orders = await GerOrdersAsync(GetUserId());
                if (orders.Count <= 0)
                    return NotFound("There's no orders requests.");

                return Ok(orders);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized("Invalid or missing context");
            }
        }

        private async Task<OrderRequestDTO?> GetOrderAsync(int userId, string orderNum)
        {
            var order = await dbContext.OrderRequests
                .AsNoTracking()
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.OrderNumber == orderNum && o.UserId == userId);

            if (order == null)
                return null;

            var orderItems = order.Items;
            List<OrderItemDTO> orderItemsDTO = new(orderItems.Count);
            for (int i = 0; i < order.Items.Count; i++)
            {
                var orderItem = orderItems[i];
                orderItemsDTO.Add(new()
                {
                    Id = orderItem.Id,
                    Name = orderItem.Name,
                    OrderRequestId = orderItem.OrderRequestId,
                    PerfumeId = orderItem.PerfumeId,
                    Price = orderItem.Price,
                    Quantity = orderItem.Quantity,
                });
            }

            return new()
            {
                Id = order.Id,
                Status = (Shared.DTOs.OrderStatus)order.Status,
                Address = order.Address,
                City = order.City,
                CreatedAt = order.CreatedAt,
                CustomerName = order.CustomerName,
                Items = orderItemsDTO,
                OrderNumber = order.OrderNumber,
                Phone = order.Phone,
            };
        }

        [HttpGet("{orderNum}")]
        public async Task<IActionResult> GetOrder([FromRoute] string orderNum)
        {
            try
            {
                var order = await GetOrderAsync(GetUserId(), orderNum);
                if (order == null)
                    return NotFound("Order was not found.");

                return Ok(order);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized("Invalid or missing context");
            }
        }

        [HttpPost("request-order")]
        public async Task<IActionResult> RequestOrder([FromBody] Shared.DTOs.PaymentMethod paymentMethod)
        {
            try
            {
                var userId = GetUserId();

                var user = await dbContext.Users
                    .Include(u => u.Cart)
                        .ThenInclude(c => c.CartItems)
                    .FirstOrDefaultAsync(c => c.Id == userId);
                if (user == null)
                    return NotFound("User was not found");

                var cart = user.Cart;

                HashSet<int> perfumeIds = [];
                for (int i = 0; i < cart.CartItems.Count; i++)
                {
                    int id = cart.CartItems[i].PerfumeId;
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

                List<OrderItem> orderItems = new(cart.CartItems.Count);

                foreach (var item in cart.CartItems)
                {
                    if (perfumeLookup.TryGetValue(item.PerfumeId, out var perfume))
                    {
                        orderItems.Add(new()
                        {
                            Name = perfume.Name,
                            PerfumeId = perfume.Id,
                            Price = perfume.Price,
                            Quantity = item.Quantity,
                        });
                    }
                }

                var orderRequest = new OrderRequest
                {
                    UserId = userId,
                    Address = user.Address,
                    City = user.City,
                    CustomerName = user.FullName,
                    Phone = user.Phone,
                    CreatedAt = DateTime.UtcNow,
                    Items = orderItems,
                    Status = Models.OrderStatus.Pending,
                    PaymentMethod = (Models.PaymentMethod)paymentMethod,
                    OrderNumber = "PK-",
                };

                cart.CartItems.Clear();
                cart.UpdatedAt = DateTime.UtcNow;

                dbContext.OrderRequests.Add(orderRequest);
                await dbContext.SaveChangesAsync();

                orderRequest.OrderNumber = $"PK-{1000 + orderRequest.Id}";
                await dbContext.SaveChangesAsync();

                return Ok();
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized("Invalid or missing context");
            }
        }
    }
}
