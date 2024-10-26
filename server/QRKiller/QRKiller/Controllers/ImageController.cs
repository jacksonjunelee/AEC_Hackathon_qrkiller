using Microsoft.AspNetCore.Mvc;
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
                return BadRequest(ex.Message);
            }
        }
    }

}
