using KORCore.Modules.Database.Models;
using KORCore.Modules.Database.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KORConnect.Controllers
{
    [ApiController]
    [Route("api/order/")]
    public class OrderController : ControllerBase
    {
        private readonly IDatabaseModel _database;

        public OrderController(IDatabaseModel database)
        {
            _database = database;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] OrderModel order)
        {
            int result = await _database.CreateOrder(order);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(Guid id)
        {
            OrderModel order = await _database.GetOrderById(id);
            return Ok(order);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOrders([FromQuery] int page = int.MinValue)
        {
            List<OrderModel> orders = await _database.GetAllOrders(page);
            return Ok(orders);
        }

        [HttpGet("stream")]
        public async IAsyncEnumerable<OrderModel> GetAllOrdersAsStream(CancellationToken cancellationToken)
        {
            await foreach (var order in _database.GetAllOrdersAsStream(cancellationToken))
            {
                yield return order;
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateOrder([FromBody] OrderModel order)
        {
            int result = await _database.UpdateOrder(order);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(Guid id)
        {
            int result = await _database.DeleteOrder(id);
            return Ok(result);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchOrders([FromQuery] string search, [FromQuery] int page = int.MinValue)
        {
            List<OrderModel> orders = await _database.SearchOrders(search, page);
            return Ok(orders);
        }

        [HttpGet("search/stream")]
        public async IAsyncEnumerable<OrderModel> SearchOrdersAsStream([FromQuery] string search, CancellationToken cancellationToken)
        {
            await foreach (var order in _database.SearchOrdersAsStream(search, cancellationToken))
            {
                yield return order;
            }
        }

        [HttpGet("count")]
        public async Task<IActionResult> CountOrders()
        {
            int count = await _database.CountOrders();
            return Ok(count);
        }
    }

}
