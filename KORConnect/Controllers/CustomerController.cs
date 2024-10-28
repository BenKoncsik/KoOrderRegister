using KORCore.Modules.Database.Models;
using KORCore.Modules.Database.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KORConnect.Controllers
{
    [ApiController]
    [Route("api/customer/")]
    public class CustomerController : ControllerBase
    {
        private readonly IDatabaseModel _database;

        public CustomerController(IDatabaseModel database)
        {
            _database = database;
        }

        [HttpPost]
        public async Task<IActionResult> CreateCustomer([FromBody] CustomerModel customer)
        {
            int result = await _database.CreateCustomer(customer);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCustomerById(Guid id)
        {
            CustomerModel customer = await _database.GetCustomerById(id);
            return Ok(customer);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCustomers([FromQuery] int page = int.MinValue)
        {
            List<CustomerModel> customers = await _database.GetAllCustomers(page);
            return Ok(customers);
        }

        [HttpGet("stream")]
        public async IAsyncEnumerable<CustomerModel> GetAllCustomersAsStream(CancellationToken cancellationToken)
        {
            await foreach (var customer in _database.GetAllCustomersAsStream(cancellationToken))
            {
                yield return customer;
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateCustomer([FromBody] CustomerModel customer)
        {
            int result = await _database.UpdateCustomer(customer);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(Guid id)
        {
            int result = await _database.DeleteCustomer(id);
            return Ok(result);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchCustomer([FromQuery] string search, [FromQuery] int page = int.MinValue)
        {
            List<CustomerModel> customers = await _database.SearchCustomer(search, page);
            return Ok(customers);
        }

        [HttpGet("search/stream")]
        public async IAsyncEnumerable<CustomerModel> SearchCustomerAsStream([FromQuery] string search, CancellationToken cancellationToken)
        {
            await foreach (var customer in _database.SearchCustomerAsStream(search, cancellationToken))
            {
                yield return customer;
            }
        }

        [HttpGet("count")]
        public async Task<IActionResult> CountCustomers()
        {
            int count = await _database.CountCustomers();
            return Ok(count);
        }
    }

}
