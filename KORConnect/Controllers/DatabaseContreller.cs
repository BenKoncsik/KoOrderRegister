using KORCore.Modules.Database.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KORConnect.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DatabaseContreller : ControllerBase
    {
        private readonly IDatabaseModel _databaseModel;

        public DatabaseContreller(IDatabaseModel databaseModel)
        {
            _databaseModel = databaseModel;
        }


        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Database API");
        }

        [HttpGet("export")]
        public async Task<IActionResult> ExportDatabase()
        {
            try
            {
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "database.json");
                await _databaseModel.ExportDatabaseToJson(filePath, CancellationToken.None);
                return Ok("Database exported successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("import")]
        public async Task<IActionResult> ImportDatabase(IFormFile file)
        {
            try
            {
                using (var stream = file.OpenReadStream())
                {
                    await _databaseModel.ImportDatabaseFromJson(stream);
                }
                return Ok("Database imported successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
