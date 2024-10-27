using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using static System.Collections.Specialized.BitVector32;

namespace QRKiller.Revit
{
    [Transaction(TransactionMode.Manual)]
    public class Class1 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = commandData.Application.ActiveUIDocument.Document;
            SetPlanView(doc, uidoc);

            View activeView = doc.ActiveView;
            Selection selection = uidoc.Selection;
            ICollection<ElementId> selectedIds = selection.GetElementIds();
            FilteredElementCollector collector = new FilteredElementCollector(doc)
            .OfClass(typeof(Viewport))
            .WhereElementIsNotElementType();

            foreach (Viewport viewport in collector)
            {
                View view = doc.GetElement(viewport.ViewId) as View;
                TaskDialog.Show("view names", "req view: " + view.Name + "\n" + view.Name);
                // Process the embedded viewport, such as focusing or exporting
            }

            //DocumentPreviewSettings settings = doc.GetDocumentPreviewSettings();
            ////
            //string info = "";
            //foreach (View3D v in new FilteredElementCollector(doc).OfClass(typeof(View3D)))
            //{
            //    if (v.Name == "{3D}")
            //    {
            //        uidoc.ActiveView = v;
            //    }
            //}
            ////
            //Selection selection = uidoc.Selection;
            //ICollection<ElementId> selectedIds = uidoc.Selection.GetElementIds();
            //bool t = false;
            //if (selectedIds.Count == 0)
            //{
            //    TaskDialog.Show("Revit", "You haven't selected any elements at all.");
            //}
            // else
            //  {
            //      string s2 = ProcessSelection(doc, uidoc, selectedIds);
            //   TaskDialog.Show("Revit", "data copied to clipboard / write json to file\n" + s2);
            //   t = true;
            //}
            // if (t)
            // {
            // drawInRhino();
            //  }
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
                if (v.Name.Contains("Level 1"))
                {
                    reqView = v.Name;
                    viewPlan = v;
                    uidoc.ActiveView = viewPlan;
                    break;
                }
            }

            TaskDialog.Show("view names", "req view: " + reqView + "\n" + viewNames);
        }

        public void set3dView(Autodesk.Revit.DB.Document doc, UIDocument uidoc)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc).OfClass(typeof(View3D));
            View3D view3d = null;
            string reqView = "";
            string viewNames = "";
            foreach (View3D v in collector)
            {
                viewNames += v.Name + ", ";
                if (v.Name.Contains("{3D}"))
                {
                    reqView = v.Name;
                    view3d = v;
                    uidoc.ActiveView = view3d;
                    break;
                }
            }

            TaskDialog.Show("view names", "req view: " + reqView + "\n" + viewNames);
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
