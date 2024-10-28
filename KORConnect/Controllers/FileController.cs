using KORCore.Modules.Database.Models;
using KORCore.Modules.Database.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KORConnect.Controllers
{
    [ApiController]
    [Route("api/file/")]
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
            int result = await _database.CreateFile(file);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetFileById(Guid id)
        {
            FileModel file = await _database.GetFileById(id);
            return Ok(file);
        }

        [HttpGet("order/{orderId}")]
        public async Task<IActionResult> GetAllFilesByOrderId(Guid orderId)
        {
            List<FileModel> files = await _database.GetAllFilesByOrderId(orderId);
            return Ok(files);
        }

        [HttpGet("order/{orderId}/stream")]
        public async IAsyncEnumerable<FileModel> GetAllFilesByOrderIdAsStream(Guid orderId, CancellationToken cancellationToken)
        {
            await foreach (var file in _database.GetAllFilesByOrderIdAsStream(orderId, cancellationToken))
            {
                yield return file;
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllFiles()
        {
            List<FileModel> files = await _database.GetAllFiles();
            return Ok(files);
        }

        [HttpGet("stream")]
        public async IAsyncEnumerable<FileModel> GetAllFilesAsStream(CancellationToken cancellationToken)
        {
            await foreach (var file in _database.GetAllFilesAsStream(cancellationToken))
            {
                yield return file;
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateFile([FromBody] FileModel file)
        {
            int result = await _database.UpdateFile(file);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFile(Guid id)
        {
            int result = await _database.DeleteFile(id);
            return Ok(result);
        }

        [HttpGet("order/{orderId}/withoutcontent")]
        public async Task<IActionResult> GetFilesByOrderIdWithOutContent(Guid orderId)
        {
            List<FileModel> files = await _database.GetFilesByOrderIdWithOutContent(orderId);
            return Ok(files);
        }

        [HttpGet("count")]
        public async Task<IActionResult> CountFiles()
        {
            int count = await _database.CountFiles();
            return Ok(count);
        }
    }
}
