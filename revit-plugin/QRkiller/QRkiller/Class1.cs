using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace Lab1PlaceGroup
{
    [Transaction(TransactionMode.Manual)]
    public class Class1 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //Get application and document objects  
            UIApplication uiapp = commandData.Application;
            Document doc = uiapp.ActiveUIDocument.Document;

            //Define a reference Object to accept the pick result  
            Reference pickedref;

            //Pick a group  
            Selection sel = uiapp.ActiveUIDocument.Selection;
            pickedref = sel.PickObject(ObjectType.Element, "Please select a group");

            Element elem = doc.GetElement(pickedref);
            Group? group = elem as Group;

            //Pick point  
            XYZ point = sel.PickPoint("Please pick a point to place group");

            //Place the group  
            Transaction trans = new Transaction(doc);
            trans.Start("Lab");
            doc.Create.PlaceGroup(point, group?.GroupType);
            trans.Commit();

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
