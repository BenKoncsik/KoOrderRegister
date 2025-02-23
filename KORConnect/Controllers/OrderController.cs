using KORCore.Modules.Database.Models;
using KORCore.Modules.Database.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;

namespace KORConnect.Controllers
{
    [ApiController]
    [Route("api/orders")]
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
            var result = await _database.CreateOrder(order);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(Guid id)
        {
            var order = await _database.GetOrderById(id);
            return order != null ? Ok(order) : NotFound();
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOrders(int page = 0)
        {
            var orders = await _database.GetAllOrders(page);
            return Ok(orders);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateOrder([FromBody] OrderModel order)
        {
            var result = await _database.UpdateOrder(order);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(Guid id)
        {
            var result = await _database.DeleteOrder(id);
            return Ok(result);
        }

        [HttpGet("order/{orderId}/stream")]
        public async IAsyncEnumerable<FileModel> GetFilesByOrderIdAsStream(Guid orderId, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            await foreach (var file in _database.GetAllFilesByOrderIdAsStream(orderId, cancellationToken))
            {
                yield return file;
            }
        }

        [HttpGet("search")]
        public IAsyncEnumerable<OrderModel> SearchOrdersAsStream(string search, CancellationToken cancellationToken)
       => _database.SearchOrdersAsStream(search, cancellationToken);

        [HttpGet("count")]
        public Task<int> CountOrders() => _database.CountOrders();
    }

}
