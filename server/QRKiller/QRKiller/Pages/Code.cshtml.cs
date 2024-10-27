using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QRKiller.Core.Services;

namespace QRKiller.Pages
{
    public class CodeModel : PageModel
    {
        private readonly ImageService _imageService;

        public (string, string) imagePairing { get; set; } = (string.Empty, string.Empty);

        public CodeModel(ImageService imageService)
        {
            _imageService = imageService;
        }

        [BindProperty(SupportsGet = true)]
        public string Id { get; set; }

        public void OnGet()
        {
            (string, string)? tempData;
            tempData = _imageService.GetImagePairing(Id);
            if( tempData != null)
                imagePairing = _imageService.GetImagePairing(Id).Value;
        }
    }
}
