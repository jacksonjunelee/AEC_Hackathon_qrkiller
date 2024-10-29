using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using System.Windows.Forms;
using System.IO;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

public class ImprintCommand : IExternalCommand
{
    public Result Execute(ExternalCommandData commandData, ElementSet elements)
    {
        // Get the active document and view
        UIDocument uiDoc = commandData.GetUIDocument();
        Document doc = uiDoc.Document;
        View activeView = doc.ActiveView;

        // Define the path to save the image
        string imagePath = Path.Combine(Path.GetTempPath(), $"{activeView.Name}.png");

        // Save the active view as a PNG file
        SaveActiveViewAsPng(doc, activeView, imagePath);

        // Show a form to input metadata
        Form inputForm = new Form();
        // (Add controls for user input here)
        // ... input form setup ...

        if (inputForm.ShowDialog() == DialogResult.OK)
        {
            string metadata = inputForm.Controls["metadataTextBox"].Text; // Example control for metadata
            string uuid = UploadImageToImprint(imagePath, metadata).Result;

            MessageBox.Show($"Image uploaded successfully! UUID: {uuid}");
        }

        return Result.Succeeded;
    }

    private void SaveActiveViewAsPng(Document doc, View view, string imagePath)
    {
        // Logic to capture the active view and save it as a PNG
        // Use RenderedView to save the image
        using (Image img = view.GetImage())
        {
            img.Save(imagePath, ImageFormat.Png);
        }
    }

    private async Task<string> UploadImageToImprint(string imagePath, string metadata)
    {
        using (HttpClient client = new HttpClient())
        {
            // Prepare the content for the POST request
            var content = new MultipartFormDataContent();
            content.Add(new ByteArrayContent(File.ReadAllBytes(imagePath)), "file", Path.GetFileName(imagePath));
            content.Add(new StringContent(metadata), "metadata");

            // Send the request to the Imprint backend API
            HttpResponseMessage response = await client.PostAsync("https://your-imprint-api-url.com/upload", content);
            response.EnsureSuccessStatusCode();

            // Read the UUID returned from the response
            string responseBody = await response.Content.ReadAsStringAsync();
            var responseData = JsonConvert.DeserializeObject<ResponseData>(responseBody);
            return responseData.Uuid;
        }
    }

    private class ResponseData
    {
        public string Uuid { get; set; }
    }
}
