using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace QRKiller.Pages
{
    public class LookupModel : PageModel
    {
        public void OnGet()
        {
        }

        [BindProperty]
        public string ImageId { get; set; }  // Bind ImageId from form

        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrWhiteSpace(ImageId))
            {
                ErrorMessage = "Please provide an image ID.";
                return Page();
            }

            // Redirect to the new page with the provided ImageId
            return RedirectToPage("Code", new { id = ImageId });
        }
    }
}
