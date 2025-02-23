using KORCore.Modules.Database.Models;
using KORCore.Modules.Database.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

[ApiController]
[Route("api/customers")]
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
        var result = await _database.CreateCustomer(customer);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCustomerById(Guid id)
    {
        var customer = await _database.GetCustomerById(id);
        return customer != null ? Ok(customer) : NotFound();
    }

    [HttpGet]
    public async Task<IActionResult> GetAllCustomers(int page = 0)
    {
        var customers = await _database.GetAllCustomers(page);
        return Ok(customers);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateCustomer([FromBody] CustomerModel customer)
    {
        var result = await _database.UpdateCustomer(customer);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCustomer(Guid id)
    {
        var result = await _database.DeleteCustomer(id);
        return Ok(result);
    }

    [HttpGet("stream")]
    public async IAsyncEnumerable<CustomerModel> GetAllCustomersAsStream([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await foreach (var customer in _database.GetAllCustomersAsStream(cancellationToken))
        {
            yield return customer;
        }
    }

    [HttpGet("search")]
    public IAsyncEnumerable<CustomerModel> SearchCustomerAsStream(string search, CancellationToken cancellationToken)
      => _database.SearchCustomerAsStream(search, cancellationToken);

    [HttpGet("count")]
    public Task<int> CountCustomers() => _database.CountCustomers();

    [HttpGet("search")]
    public Task<List<CustomerModel>> SearchCustomer(string search, int page = int.MinValue)
    => _database.SearchCustomer(search, page);

}
