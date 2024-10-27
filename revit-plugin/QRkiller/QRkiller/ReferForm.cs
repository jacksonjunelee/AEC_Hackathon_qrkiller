using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace Lab1PlaceGroup
{
    [Transaction(TransactionMode.Manual)]
    public class ReferForm : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = commandData.Application.ActiveUIDocument.Document;
            DocumentPreviewSettings settings = doc.GetDocumentPreviewSettings();
            //
            string info = "";
            foreach (View3D v in new FilteredElementCollector(doc).OfClass(typeof(View3D)))
            {
                if (v.Name == "{3D}")
                {
                    uidoc.ActiveView = v;
                }
            }
            //
            Selection selection = uidoc.Selection;
            ICollection<ElementId> selectedIds = uidoc.Selection.GetElementIds();
            bool t = false;
            if (selectedIds.Count == 0)
            {
                TaskDialog.Show("Revit", "You haven't selected any elements at all.");
            }
           // else
          //  {
          //      string s2 = ProcessSelection(doc, uidoc, selectedIds);
             //   TaskDialog.Show("Revit", "data copied to clipboard / write json to file\n" + s2);
             //   t = true;
            //}
            if (t)
            {
                // drawInRhino();
            }
            return Result.Succeeded;
        }
    }
}
//namespace QRkiller
//{
//    [Transaction(TransactionMode.Manual)]
//    public class Class1 : IExternalApplication
//    {
//        public Result OnShutdown(UIControlledApplication application)
//        {
//            return Result.Succeeded;
//;        }

//        public Result OnStartup(UIControlledApplication application)
//        {
//            return Result.Succeeded;
//        }
//    }

//    [Transaction(TransactionMode.Manual)]
//    public class Class2 : IExternalCommand
//    {
//        //Fill in with code
//        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
//        {
//            // MyForm form = new MyForm();
//            //form.ShowDialog();

//            TaskDialog.Show("My Message", "Hello, this is a message from your Revit test add-in.");
//            return Result.Succeeded;

//        }
//    }
//}
