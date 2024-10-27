using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using QRKiller.Core.Services;
using System.Collections.Generic;
using System.IO;

namespace QRKiller.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IWebHostEnvironment _environment;
        private readonly ImageService _imageService;

        public IndexModel(ILogger<IndexModel> logger, ImageService imageService, IWebHostEnvironment environment)
        {
            _logger = logger;
            _imageService = imageService;
            _environment = environment;
            ConfigureTestData();
        }

        public IActionResult OnGet()
        {
            return RedirectToPage("/Lookup");
        }

        private void ConfigureTestData()
        {
            // List of image file names (add your image file names here)
            string[] imageFiles = new string[] { "train2.jpg", "train4.jpg", "train6.png" };
            string[] imageGuids = new string[] {
                "ee908473-e374-4c88-837e-420c4bb0374b",
                "78c7ac50-dd8a-4151-871b-a599cbb649d0",
                "29c825e1-1a62-4ded-b087-fa860cfc5743"
            };

            string[] imageContents = new string[] {
                "ee908473-e374-4c88-837e-420c4bb0374b",
                @"https://api2.enscape3d.com/v3/view/d8852e9f-f4ec-4ae5-87ba-cfe4784326e2",
                @" https://aec.viktor.ai/public/qrkiller"

            };


            for (var i = 0; i < imageFiles.Length; i++)
            {
                // Use the web root path (wwwroot) and specify the folder within it
                string filePath = Path.Combine(_environment.WebRootPath, "images", imageFiles[i]);

                // Check if the file exists to avoid errors
                if (System.IO.File.Exists(filePath))
                {
                    try
                    {
                        // Read the file as a byte array
                        byte[] image = System.IO.File.ReadAllBytes(filePath);

                        var imagebytes = _imageService.GetImage(imageGuids[i]);
                        if (imagebytes != null)
                            return;
                        // Store the image in the image service with the file name as the ID
                        _imageService.StoreImage(imageGuids[i], image, imageContents[i]);
                        _logger.LogInformation($"Successfully stored image: {imageGuids[i]}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Error reading file {filePath}: {ex.Message}");
                    }
                }
                else
                {
                    _logger.LogWarning($"File not found: {filePath}");
                }
            }
        }
    }
}
