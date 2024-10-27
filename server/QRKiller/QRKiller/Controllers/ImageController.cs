using Microsoft.AspNetCore.Mvc;
using QRKiller.Core.Services;
using System.Threading.Tasks;

namespace QRKiller.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class ImageController : ControllerBase
    {
        private readonly ImageService _imageService;

        public ImageController(ImageService imageService)
        {
            _imageService = imageService;
        }

        [HttpGet("{imageName}")]
        public async Task<IActionResult> GetImage(string imageName)
        {
            try
            {
                byte[] imageBytes = _imageService.GetImage(imageName);
                return File(imageBytes, "image/png");
            }
            catch (Exception ex)
            {
                return NotFound($"Image '{imageName}' not found: {ex.Message}");
            }
        }



        [HttpPost("/images/")]
        public async Task<IActionResult> PostImage([FromHeader(Name = "Image-Name")] string imageName, [FromHeader(Name = "Content")] string content)
        {
            try
            {
                using (var ms = new MemoryStream())
                {
                    await Request.Body.CopyToAsync(ms);
                    byte[] imageBytes = ms.ToArray();
                    _imageService.StoreImage(imageName, imageBytes, content);  // Store image using the image service
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }

}
