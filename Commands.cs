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

      Autodesk.AutoCAD.EditorInput.Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
      Autodesk.AutoCAD.EditorInput.PromptSelectionResult selectionResult = ed.GetSelection();
      if (selectionResult.Status == Autodesk.AutoCAD.EditorInput.PromptStatus.OK)
      {
        Autodesk.AutoCAD.EditorInput.SelectionSet selectionSet = selectionResult.Value;

        Autodesk.AutoCAD.EditorInput.PromptPointOptions originOptions = new Autodesk.AutoCAD.EditorInput.PromptPointOptions("Select an origin point: ");
        Autodesk.AutoCAD.EditorInput.PromptPointResult originResult = ed.GetPoint(originOptions);
        if (originResult.Status == Autodesk.AutoCAD.EditorInput.PromptStatus.OK)
        {
          Point3d origin = originResult.Value;

          foreach (Autodesk.AutoCAD.DatabaseServices.ObjectId objectId in selectionSet.GetObjectIds())
          {
            using (Transaction transaction = objectId.Database.TransactionManager.StartTransaction())
            {
              Autodesk.AutoCAD.DatabaseServices.DBObject obj = transaction.GetObject(objectId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead);

              ed.WriteMessage(obj.GetType().ToString());

              if (obj is Autodesk.AutoCAD.DatabaseServices.Polyline)
              {
                data = HandlePolyline(obj as Autodesk.AutoCAD.DatabaseServices.Polyline, data, origin);
              }
              else if (obj is Autodesk.AutoCAD.DatabaseServices.Arc)
              {
                data = HandleArc(obj as Autodesk.AutoCAD.DatabaseServices.Arc, data, origin);
              }
              else if (obj is Autodesk.AutoCAD.DatabaseServices.Circle)
              {
                data = HandleCircle(obj as Autodesk.AutoCAD.DatabaseServices.Circle, data, origin);
              }
              else if (obj is Autodesk.AutoCAD.DatabaseServices.Ellipse)
              {
                data = HandleEllipse(obj as Autodesk.AutoCAD.DatabaseServices.Ellipse, data, origin);
              }
              else if (obj is Autodesk.AutoCAD.DatabaseServices.MText)
              {
                data = HandleMText(obj as Autodesk.AutoCAD.DatabaseServices.MText, data, origin);
              }
              else if (obj is Autodesk.AutoCAD.DatabaseServices.Solid)
              {
                data = HandleSolid(obj as Autodesk.AutoCAD.DatabaseServices.Solid, data, origin);
              }
              else if (obj is Autodesk.AutoCAD.DatabaseServices.Line)
              {
                data = HandleLine(obj as Autodesk.AutoCAD.DatabaseServices.Line, data, origin);
              }

              transaction.Commit();
            }
          }
        }
      }

      HelperMethods.SaveDataToJsonFile(data, "data.json");
    }

    [CommandMethod("CreateObjectFromData")]
    public static void CreateObjectFromData()
    {
      string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
      string filePath = Path.Combine(desktopPath, "data.json");
      string jsonData = File.ReadAllText(filePath);

      List<object> objectDataList = JsonConvert.DeserializeObject<List<object>>(jsonData);

      Document acDoc = Application.DocumentManager.MdiActiveDocument;
      Database acCurDb = acDoc.Database;
      Editor ed = acDoc.Editor;

      PromptPointOptions ppo = new PromptPointOptions("Select a point: ");
      PromptPointResult ppr = ed.GetPoint(ppo);

      if (ppr.Status != PromptStatus.OK) return;

      Point3d basePoint = ppr.Value;

      using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
      {
        BlockTable acBlkTbl;
        acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead) as BlockTable;
        BlockTableRecord acBlkTblRec;
        acBlkTblRec = acTrans.GetObject(acBlkTbl[BlockTableRecord.PaperSpace], OpenMode.ForWrite) as BlockTableRecord;

        foreach (var objectData in objectDataList)
        {
          if (objectData is PolylineData polylineData)
          {
            basePoint = CreatePolyline(basePoint, acTrans, acBlkTblRec, polylineData);
          }
          else if (objectData is LineData lineData)
          {
            basePoint = CreateLine(basePoint, acTrans, acBlkTblRec, lineData);
          }
          else if (objectData is ArcData arcData)
          {
            basePoint = CreateArc(basePoint, acTrans, acBlkTblRec, arcData);
          }
          else if (objectData is CircleData circleData)
          {
            basePoint = CreateCircle(basePoint, acTrans, acBlkTblRec, circleData);
          }
          else if (objectData is EllipseData ellipseData)
          {
            basePoint = CreateEllipse(basePoint, acTrans, acBlkTblRec, ellipseData);
          }
          else if (objectData is MTextData mTextData)
          {
            basePoint = CreateMText(basePoint, acTrans, acBlkTblRec, mTextData);
          }
          else if (objectData is SolidData solidData)
          {
            basePoint = CreateSolid(basePoint, acTrans, acBlkTblRec, solidData);
          }
        }

        acTrans.Commit();
      }
    }

    private static void SetMTextStyleByName(MText mtext, string styleName)
    {
      Database db = HostApplicationServices.WorkingDatabase;
      using (Transaction tr = db.TransactionManager.StartTransaction())
      {
        TextStyleTable textStyleTable = tr.GetObject(db.TextStyleTableId, OpenMode.ForRead) as TextStyleTable;
        if (textStyleTable.Has(styleName))
        {
          TextStyleTableRecord textStyle = tr.GetObject(textStyleTable[styleName], OpenMode.ForRead) as TextStyleTableRecord;
          mtext.TextStyleId = textStyle.ObjectId;
        }
        tr.Commit();
      }
    }

    private static Point3d CreateSolid(Point3d basePoint, Transaction acTrans, BlockTableRecord acBlkTblRec, SolidData solidData)
    {
      Solid solid = new Solid();
      solid.Layer = solidData.Layer;
      for (short i = 0; i < solidData.Vertices.Count; i++)
      {
        SimpleVector3d vector = solidData.Vertices[i];
        solid.SetPointAt(i, new Point3d(basePoint.X + vector.X, basePoint.Y + vector.Y, basePoint.Z + vector.Z));
      }

      acBlkTblRec.AppendEntity(solid);
      acTrans.AddNewlyCreatedDBObject(solid, true);
      return basePoint;
    }

    private static Point3d CreateMText(Point3d basePoint, Transaction acTrans, BlockTableRecord acBlkTblRec, MTextData mTextData)
    {
      MText mText = new MText();
      mText.Layer = mTextData.Layer;
      SetMTextStyleByName(mText, mTextData.Style);
      mText.Attachment = (AttachmentPoint)Enum.Parse(typeof(AttachmentPoint), mTextData.Justification);
      mText.Contents = mTextData.Contents;
      mText.Location = new Point3d(basePoint.X + mTextData.Location.X, basePoint.Y + mTextData.Location.Y, basePoint.Z + mTextData.Location.Z);
      mText.LineSpaceDistance = mTextData.LineSpaceDistance;
      mText.Height = mTextData.Height;
      mText.Width = mTextData.Width;
      mText.Rotation = mTextData.Rotation;

      acBlkTblRec.AppendEntity(mText);
      acTrans.AddNewlyCreatedDBObject(mText, true);
      return basePoint;
    }

    private static Point3d CreateEllipse(Point3d basePoint, Transaction acTrans, BlockTableRecord acBlkTblRec, EllipseData ellipseData)
    {
      Ellipse ellipse = new Ellipse();
      ellipse.Layer = ellipseData.Layer;
      Point3d center = new Point3d(basePoint.X + ellipseData.Center.X, basePoint.Y + ellipseData.Center.Y, basePoint.Z + ellipseData.Center.Z);
      Vector3d majorAxis = new Vector3d(ellipseData.MajorAxis.X, ellipseData.MajorAxis.Y, ellipseData.MajorAxis.Z);
      double radiusRatio = ellipseData.RadiusRatio();
      double startAngle = ellipseData.StartAngle;
      double endAngle = ellipseData.EndAngle;
      Vector3d unitNormal = new Vector3d(0, 0, 1);

      ellipse.Set(center, unitNormal, majorAxis, radiusRatio, startAngle, endAngle);

      acBlkTblRec.AppendEntity(ellipse);
      acTrans.AddNewlyCreatedDBObject(ellipse, true);
      return basePoint;
    }

    private static Point3d CreateCircle(Point3d basePoint, Transaction acTrans, BlockTableRecord acBlkTblRec, CircleData circleData)
    {
      Circle circle = new Circle();
      circle.Layer = circleData.Layer;
      circle.Center = new Point3d(basePoint.X + circleData.Center.X, basePoint.Y + circleData.Center.Y, basePoint.Z + circleData.Center.Z);
      circle.Radius = circleData.Radius;

      acBlkTblRec.AppendEntity(circle);
      acTrans.AddNewlyCreatedDBObject(circle, true);
      return basePoint;
    }

    private static Point3d CreateArc(Point3d basePoint, Transaction acTrans, BlockTableRecord acBlkTblRec, ArcData arcData)
    {
      Arc arc = new Arc();
      arc.Layer = arcData.Layer;
      arc.Center = new Point3d(basePoint.X + arcData.Center.X, basePoint.Y + arcData.Center.Y, basePoint.Z + arcData.Center.Z);
      arc.Radius = arcData.Radius;
      arc.StartAngle = arcData.StartAngle;
      arc.EndAngle = arcData.EndAngle;

      acBlkTblRec.AppendEntity(arc);
      acTrans.AddNewlyCreatedDBObject(arc, true);
      return basePoint;
    }

    private static Point3d CreateLine(Point3d basePoint, Transaction acTrans, BlockTableRecord acBlkTblRec, LineData lineData)
    {
      Line line = new Line();
      line.Layer = lineData.Layer;
      line.StartPoint = new Point3d(basePoint.X + lineData.StartPoint.X, basePoint.Y + lineData.StartPoint.Y, basePoint.Z + lineData.StartPoint.Z);
      line.EndPoint = new Point3d(basePoint.X + lineData.EndPoint.X, basePoint.Y + lineData.EndPoint.Y, basePoint.Z + lineData.EndPoint.Z);

      acBlkTblRec.AppendEntity(line);
      acTrans.AddNewlyCreatedDBObject(line, true);
      return basePoint;
    }

    private static Point3d CreatePolyline(Point3d basePoint, Transaction acTrans, BlockTableRecord acBlkTblRec, PolylineData polylineData)
    {
      Polyline polyline = new Polyline();
      polyline.Layer = polylineData.Layer;
      polyline.Linetype = polylineData.LineType;
      polyline.Closed = polylineData.Closed;

      for (int i = 0; i < polylineData.Vectors.Count; i++)
      {
        SimpleVector3d vector = polylineData.Vectors[i];
        polyline.AddVertexAt(i, new Point2d(basePoint.X + vector.X, basePoint.Y + vector.Y), 0, 0, 0);
      }

      acBlkTblRec.AppendEntity(polyline);
      acTrans.AddNewlyCreatedDBObject(polyline, true);
      return basePoint;
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

    private static List<object> HandleArc(Arc arc, List<object> data, Point3d origin)
    {
      var arcData = new ArcData
      {
        Layer = arc.Layer,
        Center = new SimpleVector3d
        {
          X = arc.Center.X - origin.X,
          Y = arc.Center.Y - origin.Y,
          Z = arc.Center.Z - origin.Z
        },
        Radius = arc.Radius,
        StartAngle = arc.StartAngle,
        EndAngle = arc.EndAngle
      };

      data.Add(arcData);

      return data;
    }

    private static List<object> HandleCircle(Circle circle, List<object> data, Point3d origin)
    {
      var circleData = new CircleData
      {
        Layer = circle.Layer,
        Center = new SimpleVector3d
        {
          X = circle.Center.X - origin.X,
          Y = circle.Center.Y - origin.Y,
          Z = circle.Center.Z - origin.Z
        },
        Radius = circle.Radius
      };

      data.Add(circleData);

      return data;
    }

    private static List<object> HandleEllipse(Ellipse ellipse, List<object> data, Point3d origin)
    {
      var ellipseData = new EllipseData
      {
        Layer = ellipse.Layer,
        UnitNormal = new SimpleVector3d
        {
          X = ellipse.Normal.X,
          Y = ellipse.Normal.Y,
          Z = ellipse.Normal.Z
        },
        Center = new SimpleVector3d
        {
          X = ellipse.Center.X - origin.X,
          Y = ellipse.Center.Y - origin.Y,
          Z = ellipse.Center.Z - origin.Z
        },
        MajorAxis = new SimpleVector3d
        {
          X = ellipse.MajorAxis.X,
          Y = ellipse.MajorAxis.Y,
          Z = ellipse.MajorAxis.Z
        },
        MajorRadius = ellipse.MajorRadius,
        MinorRadius = ellipse.MinorRadius,
        StartAngle = ellipse.StartAngle,
        EndAngle = ellipse.EndAngle
      };

      data.Add(ellipseData);

      return data;
    }

    private static List<object> HandleMText(MText mText, List<object> data, Point3d origin)
    {
      var mTextData = new MTextData
      {
        Layer = mText.Layer,
        Style = mText.TextStyleName,
        Justification = mText.Attachment.ToString(),
        Contents = mText.Contents,
        Location = new SimpleVector3d
        {
          X = mText.Location.X - origin.X,
          Y = mText.Location.Y - origin.Y,
          Z = mText.Location.Z - origin.Z
        },
        LineSpaceDistance = mText.LineSpaceDistance,
        Height = mText.ActualHeight,
        Width = mText.ActualWidth,
        Rotation = mText.Rotation
      };

      data.Add(mTextData);

      return data;
    }

    private static List<object> HandleSolid(Solid solid, List<object> data, Point3d origin)
    {
      var solidData = new SolidData
      {
        Layer = solid.Layer,
        Vertices = new List<SimpleVector3d>()
      };

      for (short i = 0; i < 4; i++)
      {
        Point3d point = solid.GetPointAt(i);
        Vector3d vector = point - origin;
        solidData.Vertices.Add(new SimpleVector3d { X = vector.X, Y = vector.Y, Z = vector.Z });
      }

      data.Add(solidData);

      return data;
    }

    private static List<object> HandleLine(Line line, List<object> data, Point3d origin)
    {
      var lineData = new LineData
      {
        Layer = line.Layer,
        StartPoint = new SimpleVector3d
        {
          X = line.StartPoint.X - origin.X,
          Y = line.StartPoint.Y - origin.Y,
          Z = line.StartPoint.Z - origin.Z
        },
        EndPoint = new SimpleVector3d
        {
          X = line.EndPoint.X - origin.X,
          Y = line.EndPoint.Y - origin.Y,
          Z = line.EndPoint.Z - origin.Z
        }
      };

      data.Add(lineData);

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

  internal class LineData
  {
    public string Layer { get; set; }
    public SimpleVector3d StartPoint { get; set; }
    public SimpleVector3d EndPoint { get; set; }
  }

  internal class ArcData
  {
    public string Layer { get; set; }
    public SimpleVector3d Center { get; set; }
    public double Radius { get; set; }
    public double StartAngle { get; set; }
    public double EndAngle { get; set; }
  }

  internal class CircleData
  {
    public string Layer { get; set; }
    public SimpleVector3d Center { get; set; }
    public double Radius { get; set; }
  }

  internal class EllipseData
  {
    public string Layer { get; set; }
    public SimpleVector3d UnitNormal { get; set; }
    public SimpleVector3d Center { get; set; }
    public SimpleVector3d MajorAxis { get; set; }
    public double MajorRadius { get; set; }
    public double MinorRadius { get; set; }
    public double StartAngle { get; set; }
    public double EndAngle { get; set; }

    public double RadiusRatio()
    {
      if (MinorRadius != 0 && MajorRadius != 0)
      {
        return MinorRadius / MajorRadius;
      }
      else
      {
        return 0;
      }
    }
  }

  internal class MTextData
  {
    public string Layer { get; set; }
    public string Style { get; set; }
    public string Justification { get; set; }
    public string Contents { get; set; }
    public SimpleVector3d Location { get; set; }
    public double LineSpaceDistance { get; set; }
    public double Height { get; set; }
    public double Width { get; set; }
    public double Rotation { get; set; }
  }

  internal class SolidData
  {
    public string Layer { get; set; }
    public List<SimpleVector3d> Vertices { get; set; }
  }

  public class SimpleVector3d
  {
    public double X { get; set; }
    public double Y { get; set; }
    public double Z { get; set; }
  }
}