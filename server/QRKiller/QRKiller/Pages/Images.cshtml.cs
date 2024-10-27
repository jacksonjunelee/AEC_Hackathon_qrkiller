using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QRKiller.Core.Services;

namespace QRKiller.Pages
{
    public class ImagesModel : PageModel
    {
        ImageService _imageService { get; set; }
        public List<string> Images = new List<string>();

        public ImagesModel(ImageService imageService)
        {
            _imageService = imageService;
        }

        public void OnGet()
        {
            Images = _imageService._savedImages;
        }


        public void Refresh()
        {

        }
    }
}
