using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QRKiller.Core.Services;

namespace QRKiller.Pages
{
    public class LookupModel : PageModel
    {
        ImageService _imageService { get; set; }

        public LookupModel(ImageService imagesService)
        {
            _imageService = imagesService;
        }
        public void OnGet()
        {
        }

        [BindProperty]
        public IFormFile ImageFile { get; set; }  // Image file to upload

        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {

            if (ImageFile == null || ImageFile.Length == 0)
            {
                ErrorMessage = "Please select a valid image file to upload.";
                return Page();
            }

            // Convert the file stream to a byte array
            byte[] imageBytes;
            using (var memoryStream = new MemoryStream())
            {
                await ImageFile.OpenReadStream().CopyToAsync(memoryStream);
                imageBytes = memoryStream.ToArray();
            }

            string? imageId = string.Empty;
            // Now pass the byte array to the embedding service
            imageId = await _imageService.PostImageToEmbeddingService(imageBytes);

            if (imageId == "ee908473-e374-4c88-837e-420c4bb0374b")
                return RedirectToPage("Code", new { id = imageId });

            var externalLink = _imageService.GetImagePairing(imageId);
            return Redirect(externalLink!.Value.Item2);
        }
    }
}
