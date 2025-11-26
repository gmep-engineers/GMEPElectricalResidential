using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;

namespace GMEPElectricalResidential
{
  public struct IntPoint
  {
    public int x;
    public int y;
    public double angle;

    public IntPoint(int x, int y, double angle = 0)
    {
      this.x = x;
      this.y = y;
      this.angle = angle;
    }
  }

  public class AutoCADHelper
  {
    public static void EnsureLayerExists(string layerName, bool isPlottable = true)
    {
      Document doc = Autodesk
        .AutoCAD
        .ApplicationServices
        .Application
        .DocumentManager
        .MdiActiveDocument;
      Database db = doc.Database;
      Editor ed = doc.Editor;
      using (Transaction tr = db.TransactionManager.StartTransaction())
      {
        LayerTable layerTable = tr.GetObject(db.LayerTableId, OpenMode.ForRead) as LayerTable;
        if (layerTable.Has(layerName) == false)
        {
          using (LayerTableRecord layerTableRecord = new LayerTableRecord())
          {
            layerTableRecord.Color = Autodesk.AutoCAD.Colors.Color.FromColorIndex(
              Autodesk.AutoCAD.Colors.ColorMethod.ByAci,
              8
            );
            layerTableRecord.Name = layerName;
            layerTableRecord.IsPlottable = isPlottable;

            tr.GetObject(db.LayerTableId, OpenMode.ForWrite);
            layerTable.Add(layerTableRecord);
            tr.AddNewlyCreatedDBObject(layerTableRecord, true);
          }
        }
        tr.Commit();
      }
    }

    public static Point3d DefinePerimeterSide(
      string layerName,
      string perimeterName,
      List<ObjectId> lineIds,
      List<IntPoint> intPoints,
      Point3d previousEndPoint
    )
    {
      var doc = Application.DocumentManager.MdiActiveDocument;
      var db = doc.Database;
      var ed = doc.Editor;
      PromptPointOptions ppo = new PromptPointOptions(
        $"\nDefine the outline of the {perimeterName}"
      );
      ppo.AllowNone = false;
      Point3d startPoint;
      if (previousEndPoint.X == 0 && previousEndPoint.Y == 0)
      {
        PromptPointResult ppr = ed.GetPoint(ppo);
        if (ppr.Status != PromptStatus.OK)
        {
          return new Point3d();
        }
        startPoint = ppr.Value;
      }
      else
      {
        startPoint = previousEndPoint;
      }
      AutoCADHelper.EnsureLayerExists(layerName, false);
      LineJig jig = new LineJig(
        startPoint,
        layerName,
        $"\nDefine the outline of the {perimeterName}"
      );
      PromptResult jigResult = ed.Drag(jig);
      if (jigResult.Status != PromptStatus.OK)
      {
        return new Point3d();
      }

      Point3d endPoint = jig.line.EndPoint;

      double deltaX = (endPoint.X - startPoint.X);
      double deltaY = (endPoint.Y - startPoint.Y);

      double angle = Math.Atan2(deltaY, deltaX);

      intPoints.Add(
        new IntPoint((int)Math.Round(startPoint.X), (int)Math.Round(startPoint.Y), angle)
      );

      double realInches = Math.Sqrt(
        Math.Pow(endPoint.X - startPoint.X, 2) + Math.Pow(endPoint.Y - startPoint.Y, 2)
      );
      double inches = (int)Math.Round(realInches);
      for (int j = 0; j < inches; j++)
      {
        double unitVectorX = (endPoint.X - startPoint.X) / realInches;
        double unitVectorY = (endPoint.Y - startPoint.Y) / realInches;
        int x = (int)Math.Round(startPoint.X + (unitVectorX * j));
        int y = (int)Math.Round(startPoint.Y + (unitVectorY * j));

        intPoints.Add(new IntPoint(x, y, angle));
      }

      intPoints.Add(new IntPoint((int)Math.Round(endPoint.X), (int)Math.Round(endPoint.Y), angle));

      Line line = new Line();
      using (Transaction tr = db.TransactionManager.StartTransaction())
      {
        BlockTable bt = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForWrite);
        BlockTableRecord btr = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);

        line.Layer = layerName;
        btr.AppendEntity(line);

        line.StartPoint = startPoint;
        line.EndPoint = endPoint;
        tr.AddNewlyCreatedDBObject(line, true);
        ObjectId lineId = line.ObjectId;
        lineIds.Add(lineId);
        tr.Commit();
      }
      return endPoint;
    }

    public static (List<ObjectId>, List<IntPoint>) DefineMultiSidedPerimeter(
      string layerName,
      string perimeterName
    )
    {
      List<ObjectId> lineIds = new List<ObjectId>();
      List<IntPoint> intPoints = new List<IntPoint>();
      Point3d previousEndPoint = new Point3d();
      while (true)
      {
        previousEndPoint = DefinePerimeterSide(
          layerName,
          perimeterName,
          lineIds,
          intPoints,
          previousEndPoint
        );
        if (previousEndPoint.X == 0 && previousEndPoint.Y == 0)
        {
          break;
        }
      }
      return (lineIds, intPoints);
    }

    public static (List<ObjectId>, List<IntPoint>) DefineFourSidedPerimeter(
      string layerName,
      string perimeterName
    )
    {
      List<ObjectId> lineIds = new List<ObjectId>();
      List<IntPoint> intPoints = new List<IntPoint>();
      Point3d previousEndPoint = new Point3d();
      for (var i = 0; i < 4; i++)
      {
        previousEndPoint = DefinePerimeterSide(
          layerName,
          perimeterName,
          lineIds,
          intPoints,
          previousEndPoint
        );
        if (previousEndPoint.X == 0 && previousEndPoint.Y == 0)
        {
          break;
        }
      }
      return (lineIds, intPoints);
    }
  }
}
