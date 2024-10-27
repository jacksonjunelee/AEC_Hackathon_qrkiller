using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QRKiller.Core.Services;

namespace QRKiller.Pages
{
    public class ImagesModel : PageModel
    {
        ImageService _imageService { get; set; }
        public List<(string, string)> Images = new List<(string, string)>();

        public ImagesModel(ImageService imageService)
        {
            _imageService = imageService;
        }

        public void OnGet()
        {
            foreach (var item in _imageService._savedImages)
            {
                Images.Add(_imageService.GetImagePairing(item).Value);
            }
        }

    }
}
