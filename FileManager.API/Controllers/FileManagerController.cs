using FileManager.API.Abstractions;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace FileManager.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [EnableCors("CorsPolicy")]
    public class FileManagerController : ControllerBase
    {
        private readonly IFileManagerService _fileManagerService;

        public FileManagerController(IFileManagerService fileManagerService)
        {
            _fileManagerService = fileManagerService;
        }

        [HttpPost("upload")]
        public async Task<ActionResult> Upload([FromForm] IFormFile file)
        {
            try
            {
                var json = await _fileManagerService.TryReadFileAsync(file);

                return Ok(json);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
