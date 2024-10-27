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
            SetPlanView(doc, uidoc);

            //View activeView = doc.ActiveView;
            //Selection selection = uidoc.Selection;
            //ICollection<ElementId> selectedIds = selection.GetElementIds();
            //FilteredElementCollector collector = new FilteredElementCollector(doc)
            //.OfClass(typeof(Viewport))
            //.WhereElementIsNotElementType();
            return Result.Succeeded;
        }

        private void SetPlanView(Document doc, UIDocument uidoc)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc).OfClass(typeof(ViewPlan));
            ViewPlan viewPlan = null;
            string reqView = "";
            string viewNames = "";
            foreach (ViewPlan v in collector)
            {
                viewNames += v.Name + ", ";
          
            }

            TaskDialog.Show("view test names", "req view: "  + "\n" + viewNames);
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
        }


    }
}
