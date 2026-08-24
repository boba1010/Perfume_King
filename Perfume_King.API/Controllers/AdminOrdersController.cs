using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Perfume_King.API.DbContexts;
using Perfume_King.Shared.Contracts.Requests;
using Perfume_King.Shared.DTOs;

namespace Perfume_King.API.Controllers
{
    [Authorize(Roles = "admin")]
    [ApiController]
    [Route("api/admin/orders")]
    public class AdminOrdersController(AppDbContext dbContext) : ControllerBase
    {
        private async Task<OrderRequestDTO?> GetOrderAsync(string orderNum)
        {
            var order = await dbContext.OrderRequests
                .AsNoTracking()
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.OrderNumber == orderNum);

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
                var order = await GetOrderAsync(orderNum);
                if (order == null)
                    return NotFound("Order was not found.");

                return Ok(order);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized("Invalid or missing context");
            }
        }

        private async Task<List<OrderRequestDTO>> GerOrdersAsync()
        {
            var orders = await dbContext.OrderRequests
                .AsNoTracking()
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
                    PaymentMethod = (Shared.DTOs.PaymentMethod)order.PaymentMethod,
                });
            }

            return orderRequestsDTO;
        }

        [HttpGet]
        public async Task<IActionResult> GetOrders()
        {
            var orders = await GerOrdersAsync();
            if (orders.Count <= 0)
                return NotFound("There's no orders requests.");

            return Ok(orders);
        }

        [HttpPost("update/state")]
        public async Task<IActionResult> UpdateOrderState([FromBody] RequestOrderStateUpdate request)
        {
            var order = await dbContext.OrderRequests
                .FirstOrDefaultAsync(o => o.Id == request.OrderId);
            if (order == null)
                return NotFound("Order was not found");

            order.Status = (Models.OrderStatus)request.OrderStatus;

            await dbContext.SaveChangesAsync();
            
            return Ok("Status updated successfully");
        }
    }
}
