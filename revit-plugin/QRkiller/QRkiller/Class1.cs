using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace QRkiller
{
    [Transaction(TransactionMode.Manual)]
    public class Class1 : IExternalApplication
    {
        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
;        }

        public Result OnStartup(UIControlledApplication application)
        {
            return Result.Succeeded;
        }
    }

    public class Class2 : IExternalCommand
    {
        //Fill in with code
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // MyForm form = new MyForm();
            //form.ShowDialog();

            TaskDialog.Show("My Message", "Hello, this is a message from your Revit test add-in.");
            return Result.Succeeded;

        }
    }
}
