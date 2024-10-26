using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using QRKiller.Core.Services;

namespace QRKiller.Pages
{
    public class CaptureModel : PageModel
    {
        private readonly ImageService _imageService;

        [BindProperty]
        public string CameraCapture { get; set; }  // Base64 encoded image data
        public bool UploadSuccess { get; set; }
        public string ErrorMessage { get; set; }
        public string UploadedFileName { get; set; }

        public CaptureModel(ImageService imageService)
        {
            _imageService = imageService;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrEmpty(CameraCapture))
            {
                ErrorMessage = "No image captured.";
                UploadSuccess = false;
                return Page();
            }

            // Decode the Base64 image data
            var data = new Regex(@"^data:image\/[a-z]+;base64,").Replace(CameraCapture, "");
            var imageBytes = Convert.FromBase64String(data);

            var fileName = Guid.NewGuid() + ".png";

            _imageService.StoreImage(fileName, imageBytes);

            UploadedFileName = fileName;
            UploadSuccess = true;
            return Page();
        }
    }
}
