using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using static System.Collections.Specialized.BitVector32;
using System;
using System.Drawing;
using System.Windows;

namespace QRKiller.Revit
{
    [Transaction(TransactionMode.Manual)]
    public class Class1 : IExternalCommand
    {
        private static int instanceCount = 0;
       // public class ImageDisplayForm : Form
     //   {
      //      private PictureBox pictureBox;
      //      public ImageDisplayForm(string imagePath)
      //      {
       //         this.Text = "View Image";
       //         this.Size = new System.Drawing.Size(800, 600);
        //        pictureBox = new PictureBox
       //         {
       //             Image = new Bitmap(imagePath),
        //            SizeMode = PictureBoxSizeMode.Zoom,
         //           Dock = DockStyle.Fill
        //        };
        //        this.Controls.Add(pictureBox);
        //    }
     //   }

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = commandData.Application.ActiveUIDocument.Document;
          //  SetPlanView(doc, uidoc);

            View activeView = doc.ActiveView;

            // Inside the Execute method or as a separate method
            string imagePath = ExportActiveViewAsImage(doc, activeView);
           //  ShowViewImage(doc, activeView);
            instanceCount++;

            TaskDialog.Show("view test names", "req view: " + activeView.Name + "\n" + imagePath);

            return Result.Succeeded;
        }
        public static void ExportViewImage(Document doc, View view, string filePath)
        {
            if (view == null || !view.CanBePrinted)
            {
                TaskDialog.Show("Error", "Selected view cannot be exported.");
                return;
            }
            // Configure image export options
            ImageExportOptions options = new ImageExportOptions
            {
                FilePath = filePath,
                FitDirection = FitDirectionType.Horizontal,
                HLRandWFViewsFileType = ImageFileType.PNG,
                ImageResolution = ImageResolution.DPI_150,
                PixelSize = 1024, // Set the size as needed
                ExportRange = ExportRange.CurrentView,
                ShadowViewsFileType = ImageFileType.PNG
            };
            // Set the name for the output file
            options.SetViewsAndSheets(new ElementId[] { view.Id });
            // Export image
            doc.ExportImage(options);
            TaskDialog.Show("Export", $"Image exported to {filePath}");
        }

        public void ShowViewImage(Document doc, View view)
        {
            string filePath = @"C:\Users\samahaa\Downloads\aec\AEC_Hackathon_qrkiller\revit-plugin\ImageDrop\"; // Set desired file path
            ExportViewImage(doc, view, filePath);
            //ImageDisplayForm displayForm = new ImageDisplayForm(filePath);
            // displayForm.ShowDialog(); // Display the form with the exported image
            return;
        }

        private string ExportActiveViewAsImage(Document doc, View view)
        {
            // Define the file path for the image
            string tempImagePath = @"C:\Users\samahaa\Downloads\aec\AEC_Hackathon_qrkiller\revit-plugin\ImageDrop\" + "Document-" + GetInstanceCount() + ".png"; // Set desired file path

            // Set up image export options
            ImageExportOptions options = new ImageExportOptions
            {
                FilePath = tempImagePath,
                FitDirection = FitDirectionType.Horizontal,
                HLRandWFViewsFileType = ImageFileType.PNG,
                ImageResolution = ImageResolution.DPI_150,
                ExportRange = ExportRange.CurrentView,
                ZoomType = ZoomFitType.FitToPage,
                ShadowViewsFileType = ImageFileType.PNG
            };

            // Start a transaction (required by the API even for read operations)
            using (Transaction tx = new Transaction(doc, "Export Active View"))
            {
                tx.Start();

                // Export the current view as an image
                doc.ExportImage(options);

                tx.Commit();
            }

            return tempImagePath;
        }
         
        public static int GetInstanceCount()
        {
            return instanceCount;
        }


    }
}
