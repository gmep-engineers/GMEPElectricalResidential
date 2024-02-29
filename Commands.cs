using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using GMEPUtilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace GMEPElectricalResidential
{
  public class Commands
  {
    private static LoadCalculationForm _loadCalculationForm;
    private static SINGLE_LINE_DIAGRAM _sld;

    [CommandMethod("SLD")]
    public void SLD()
    {
      if (_sld == null || _sld.IsDisposed)
      {
        _sld = new SINGLE_LINE_DIAGRAM();
      }

      _sld.Show();
      _sld.BringToFront();
    }

    [CommandMethod("LoadCalculation")]
    public void LoadCalculation()
    {
      if (_loadCalculationForm == null || _loadCalculationForm.IsDisposed)
      {
        _loadCalculationForm = new LoadCalculationForm();
      }

      _loadCalculationForm.Show();
      _loadCalculationForm.BringToFront();
    }

    [CommandMethod("GetObjectData")]
    public static void GetObjectData()
    {
      var data = new List<object>();

      Autodesk.AutoCAD.EditorInput.Editor ed = Autodesk
          .AutoCAD
          .ApplicationServices
          .Application
          .DocumentManager
          .MdiActiveDocument
          .Editor;
      Autodesk.AutoCAD.EditorInput.PromptSelectionResult selectionResult = ed.GetSelection();
      if (selectionResult.Status == Autodesk.AutoCAD.EditorInput.PromptStatus.OK)
      {
        Autodesk.AutoCAD.EditorInput.SelectionSet selectionSet = selectionResult.Value;

        // Prompt the user to select an origin point
        Autodesk.AutoCAD.EditorInput.PromptPointOptions originOptions =
            new Autodesk.AutoCAD.EditorInput.PromptPointOptions("Select an origin point: ");
        Autodesk.AutoCAD.EditorInput.PromptPointResult originResult = ed.GetPoint(
            originOptions
        );
        if (originResult.Status == Autodesk.AutoCAD.EditorInput.PromptStatus.OK)
        {
          Point3d origin = originResult.Value;

          foreach (
              Autodesk.AutoCAD.DatabaseServices.ObjectId objectId in selectionSet.GetObjectIds()
          )
          {
            using (
                Transaction transaction =
                    objectId.Database.TransactionManager.StartTransaction()
            )
            {
              Autodesk.AutoCAD.DatabaseServices.DBObject obj = transaction.GetObject(
                  objectId,
                  Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead
              );

              ed.WriteMessage(obj.GetType().ToString());

              // Check the type of the selected object
              if (obj is Autodesk.AutoCAD.DatabaseServices.Polyline)
              {
                // Handle polyline
                data = HandlePolyline(
                    obj as Autodesk.AutoCAD.DatabaseServices.Polyline,
                    data,
                    origin
                );
              }

              // Commit the transaction
              transaction.Commit();
            }
          }
        }
      }

      HelperMethods.SaveDataToJsonFile(data, "data.json");
    }

    [CommandMethod("CreatePolyline")]
    public static void CreatePolyline()
    {
      // Get the path to the desktop
      string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

      // Get the path to the data.json file on the desktop
      string filePath = Path.Combine(desktopPath, "data.json");

      // Read the JSON data from the file
      string jsonData = File.ReadAllText(filePath);

      HelperMethods.SaveDataToJsonFile(jsonData, "jsonData.json");

      // Deserialize the JSON data into a List<PolylineData>
      List<PolylineData> polylineDataList = JsonConvert.DeserializeObject<List<PolylineData>>(jsonData);

      // Get the first item in the list
      PolylineData polylineData = polylineDataList[0];

      // Get the current document and database
      Document acDoc = Application.DocumentManager.MdiActiveDocument;
      Database acCurDb = acDoc.Database;
      Editor ed = acDoc.Editor;

      // Prompt the user to select a point
      PromptPointOptions ppo = new PromptPointOptions("Select a point: ");
      PromptPointResult ppr = ed.GetPoint(ppo);

      if (ppr.Status != PromptStatus.OK) return;

      Point3d basePoint = ppr.Value;

      // Create a new polyline with the data
      Polyline polyline = new Polyline();
      polyline.Layer = polylineData.Layer;
      polyline.Linetype = polylineData.LineType;
      polyline.Closed = polylineData.Closed;

      HelperMethods.SaveDataToJsonFile(polylineDataList, "polylineData.json");

      // Add the points to the polyline
      for (int i = 0; i < polylineData.Vectors.Count; i++)
      {
        SimpleVector3d vector = polylineData.Vectors[i];
        polyline.AddVertexAt(i, new Point2d(basePoint.X + vector.X, basePoint.Y + vector.Y), 0, 0, 0);
      }

      // Start a transaction
      using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
      {
        // Open the Block table for read
        BlockTable acBlkTbl;
        acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead) as BlockTable;

        // Open the Block table record Model space for write
        BlockTableRecord acBlkTblRec;
        acBlkTblRec = acTrans.GetObject(acBlkTbl[BlockTableRecord.PaperSpace], OpenMode.ForWrite) as BlockTableRecord;

        // Add the new object to the block table record and the transaction
        acBlkTblRec.AppendEntity(polyline);
        acTrans.AddNewlyCreatedDBObject(polyline, true);

        // Commit the changes and dispose of the transaction
        acTrans.Commit();
      }
    }

    private static List<object> HandlePolyline(Polyline polyline, List<object> data, Point3d origin)
    {
      var polylineData = new PolylineData
      {
        Layer = polyline.Layer,
        Vectors = new List<SimpleVector3d>(),
        LineType = polyline.Linetype,
        Closed = polyline.Closed
      };

      for (int i = 0; i < polyline.NumberOfVertices; i++)
      {
        Point3d point = polyline.GetPoint3dAt(i);
        Vector3d vector = point - origin;
        polylineData.Vectors.Add(new SimpleVector3d { X = vector.X, Y = vector.Y, Z = vector.Z });
      }

      data.Add(polylineData);

      return data;
    }
  }

  internal class PolylineData
  {
    public string Layer { get; set; }
    public List<SimpleVector3d> Vectors { get; set; }
    public string LineType { get; set; }
    public bool Closed { get; set; }
  }

  public class SimpleVector3d
  {
    public double X { get; set; }
    public double Y { get; set; }
    public double Z { get; set; }
  }
}