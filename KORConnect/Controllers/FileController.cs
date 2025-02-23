using KORCore.Modules.Database.Models;
using KORCore.Modules.Database.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;

namespace KORConnect.Controllers
{
    [ApiController]
    [Route("api/files")]
    public class FileController : ControllerBase
    {
        private readonly IDatabaseModel _database;

        public FileController(IDatabaseModel database)
        {
            _database = database;
        }

        [HttpPost]
        public async Task<IActionResult> CreateFile([FromBody] FileModel file)
        {
            var result = await _database.CreateFile(file);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetFileById(Guid id)
        {
            var file = await _database.GetFileById(id);
            return file != null ? Ok(file) : NotFound();
        }

        [HttpGet("order/{orderId}")]
        public async Task<IActionResult> GetFilesByOrderId(Guid orderId)
        {
            var files = await _database.GetAllFilesByOrderId(orderId);
            return Ok(files);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateFile([FromBody] FileModel file)
        {
            var result = await _database.UpdateFile(file);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFile(Guid id)
        {
            var result = await _database.DeleteFile(id);
            return Ok(result);
        }

        [HttpGet("stream")]
        public async IAsyncEnumerable<OrderModel> GetAllOrdersAsStream([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            await foreach (var order in _database.GetAllOrdersAsStream(cancellationToken))
            {
                yield return order;
            }
        }

        [HttpGet("order/{id}")]
        public Task<List<FileModel>> GetAllFilesByOrderId(Guid id) => _database.GetAllFilesByOrderId(id);

        [HttpGet("order/{id}/stream")]
        public IAsyncEnumerable<FileModel> GetAllFilesByOrderIdAsStream(Guid id, CancellationToken cancellationToken)
            => _database.GetAllFilesByOrderIdAsStream(id, cancellationToken);

        [HttpGet("count")]
        public Task<int> CountFiles() => _database.CountFiles();
    }
}
