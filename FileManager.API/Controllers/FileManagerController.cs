using FileManager.API.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace FileManager.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [EnableCors("CorsPolicy")]
    public class FileManagerController : ControllerBase
    {
        [HttpPost("upload")]
        public async Task<ActionResult> Upload([FromForm] IFormFile file)
        {
            try
            {
                var fileManagerService = new FileManagerService(file);
                var json = await fileManagerService.TryReadFileAsync();

                fileManagerService.RemoveTemporalFiles();

                return Ok(json);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
