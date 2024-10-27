using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QRKiller.Core.Services;

namespace QRKiller.Pages
{
    public class UploadModel : PageModel
    {
        private readonly ImageService _imageService;

        public UploadModel(ImageService imageService)
        {
            _imageService = imageService;
        }

        [BindProperty]
        public string Content { get; set; }  // ID for the image

        [BindProperty]
        public IFormFile ImageFile { get; set; }  // Image file to upload

        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrWhiteSpace(Content))
            {
                ErrorMessage = "Please provide an image ID.";
                return Page();
            }

            if (ImageFile == null || ImageFile.Length == 0)
            {
                ErrorMessage = "Please select an image file to upload.";
                return Page();
            }

            var fileName = Guid.NewGuid() + ".png";
            using (var ms = new MemoryStream())
            {
                await ImageFile.CopyToAsync(ms);
                var imageBytes = ms.ToArray();

                // Store image using ImageService with the provided Content ID
                _imageService.StoreImage(fileName, imageBytes, Content);
            }

            // Redirect to the display page to show the uploaded image
            return RedirectToPage("Code", new { id = fileName });
        }
    }
}
