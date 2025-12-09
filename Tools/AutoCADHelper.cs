using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;

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
    public static double Scale = -1;

    [CommandMethod("SetScale")]
    public static void SetScale()
    {
      var doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
      var ed = doc.Editor;

      var promptStringOptions = new PromptStringOptions(
        "\nEnter the scale value (e.g., 1/4, 3/16, 1/8): "
      );
      var promptStringResult = ed.GetString(promptStringOptions);

      if (promptStringResult.Status == PromptStatus.OK)
      {
        string scaleString = promptStringResult.StringResult;
        string[] scaleParts = scaleString.Split('/');

        if (
          scaleParts.Length == 2
          && double.TryParse(scaleParts[0], out double numerator)
          && double.TryParse(scaleParts[1], out double denominator)
        )
        {
          Scale = numerator / denominator;
          ed.WriteMessage($"\nScale set to {scaleString} ({Scale})");
        }
        else
        {
          ed.WriteMessage(
            $"\nInvalid scale format. Please enter the scale in the format 'numerator/denominator' (e.g., 1/4, 3/16, 1/8)."
          );
        }
      }
    }

    public static ObjectId GetTextStyleId(string styleName)
    {
      Document doc = Autodesk
        .AutoCAD
        .ApplicationServices
        .Application
        .DocumentManager
        .MdiActiveDocument;
      Database db = doc.Database;
      var textStyleTable = (TextStyleTable)db.TextStyleTableId.GetObject(OpenMode.ForRead);

      if (textStyleTable.Has(styleName))
      {
        return textStyleTable[styleName];
      }
      else
      {
        // Return the ObjectId of the "Standard" style
        return textStyleTable["Standard"];
      }
    }

    private static ObjectId CreateText(
      string content,
      string style,
      TextHorizontalMode horizontalMode,
      TextVerticalMode verticalMode,
      double height,
      double widthFactor,
      Autodesk.AutoCAD.Colors.Color color,
      string layer,
      AttachmentPoint justify = AttachmentPoint.BaseLeft
    )
    {
      Document doc = Autodesk
        .AutoCAD
        .ApplicationServices
        .Application
        .DocumentManager
        .MdiActiveDocument;
      Database db = doc.Database;

      // Check if the layer exists
      using (var tr = db.TransactionManager.StartTransaction())
      {
        var layerTable = (LayerTable)tr.GetObject(db.LayerTableId, OpenMode.ForRead);

        if (!layerTable.Has(layer))
        {
          // Layer doesn't exist, create it
          var newLayer = new LayerTableRecord();
          newLayer.Name = layer;

          layerTable.UpgradeOpen();
          layerTable.Add(newLayer);
          tr.AddNewlyCreatedDBObject(newLayer, true);
        }

        tr.Commit();
      }
      using (var tr = doc.TransactionManager.StartTransaction())
      {
        var textStyleId = GetTextStyleId(style);
        var textStyle = (TextStyleTableRecord)tr.GetObject(textStyleId, OpenMode.ForRead);
        if (
          textStyle.FileName.ToLower().Contains("architxt")
          || textStyle.FileName.ToLower().Contains("a2")
        )
        {
          if (widthFactor > 0.85)
          {
            widthFactor = 0.85;
          }
          content = content.Replace("\u03A6", "\u0081");
        }

        var text = new DBText
        {
          TextString = content,
          Height = height,
          WidthFactor = widthFactor,
          Color = color,
          Layer = layer,
          TextStyleId = textStyleId,
          HorizontalMode = horizontalMode,
          VerticalMode = verticalMode,
          Justify = justify,
        };

        var currentSpace = (BlockTableRecord)tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);
        currentSpace.AppendEntity(text);
        tr.AddNewlyCreatedDBObject(text, true);

        tr.Commit();

        return text.ObjectId;
      }
    }

    public static ObjectId CreateAndPositionText(
      Transaction tr,
      string content,
      string style,
      double height,
      double widthFactor,
      int colorIndex,
      string layerName,
      Point3d position,
      TextHorizontalMode horizontalMode = TextHorizontalMode.TextLeft,
      TextVerticalMode verticalMode = TextVerticalMode.TextBase,
      AttachmentPoint justify = AttachmentPoint.BaseLeft
    )
    {
      var color = Autodesk.AutoCAD.Colors.Color.FromColorIndex(
        Autodesk.AutoCAD.Colors.ColorMethod.ByLayer,
        (short)colorIndex
      );
      var textId = CreateText(
        content,
        style,
        horizontalMode,
        verticalMode,
        height,
        widthFactor,
        color,
        layerName,
        justify
      );
      var text = (DBText)tr.GetObject(textId, OpenMode.ForWrite);
      if (justify == AttachmentPoint.BaseLeft)
      {
        text.Position = position;
      }
      else
      {
        text.AlignmentPoint = position;
      }
      return textId;
    }

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

    public static (double, double, bool) PlaceBlockOnLine(
      double coveredLength,
      double remainingLength,
      ObjectId l,
      string blockName,
      double minDistance,
      double maxDistance,
      double initialDistance,
      List<string> labels
    )
    {
      Point3d blockRefPosition = new Point3d();
      double lineBlockJigRotation = 0;

      var doc = Application.DocumentManager.MdiActiveDocument;
      if (doc == null)
        return (0, 0, false);

      var db = doc.Database;
      var ed = doc.Editor;

      using (Transaction tr = db.TransactionManager.StartTransaction())
      {
        Line line = (Line)tr.GetObject(l, OpenMode.ForWrite);
        if (line.Length < minDistance)
        {
          return (0, 0, false);
        }
        maxDistance = maxDistance + coveredLength;

        if (coveredLength == 0 && remainingLength == 0)
        {
          maxDistance = initialDistance;
        }
        else if (coveredLength == 0 && remainingLength > 0)
        {
          maxDistance = maxDistance - remainingLength;
        }

        Point3d startPoint;
        Point3d endPoint;
        if (line.StartPoint.X > line.EndPoint.X)
        {
          startPoint = line.EndPoint;
          endPoint = line.StartPoint;
        }
        else
        {
          startPoint = line.StartPoint;
          endPoint = line.EndPoint;
        }
        if (line.StartPoint.Y > line.EndPoint.Y && line.StartPoint.X <= line.EndPoint.X)
        {
          startPoint = line.EndPoint;
          endPoint = line.StartPoint;
        }
        Vector3d dir = (endPoint - startPoint).GetNormal();
        double angle = dir.AngleOnPlane(new Plane(Point3d.Origin, Vector3d.ZAxis));

        BlockTable bt = (BlockTable)tr.GetObject(line.Database.BlockTableId, OpenMode.ForRead);
        if (!bt.Has(blockName))
        {
          ed.WriteMessage($"\nBlock '{blockName}' not found in drawing.");

          return (0, 0, false);
        }
        ObjectId blockDefId = bt[blockName];
        BlockTableRecord btr = (BlockTableRecord)tr.GetObject(line.OwnerId, OpenMode.ForWrite);
        LineBlockJig lineBlockJig = new LineBlockJig(line, blockDefId, 1, angle, maxDistance);
        PromptResult jigResult = ed.Drag(lineBlockJig);
        if (jigResult.Status != PromptStatus.OK)
        {
          return (0, remainingLength, false);
        }
        Point3d blockPos = lineBlockJig.InsertionPoint;
        string layer = "E-SYM1";
        BlockReference blockRef = new BlockReference(blockPos, blockDefId)
        {
          Rotation = lineBlockJig.rotation,
          Layer = layer,
        };
        btr.AppendEntity(blockRef);
        tr.AddNewlyCreatedDBObject(blockRef, true);
        tr.Commit();
        blockRefPosition = blockRef.Position;
        lineBlockJigRotation = lineBlockJig.rotation;
        remainingLength = line.EndPoint.DistanceTo(blockPos);
        coveredLength = line.StartPoint.DistanceTo(blockPos);
      }
      using (Transaction tr = db.TransactionManager.StartTransaction())
      {
        if (AutoCADHelper.Scale < 0)
        {
          AutoCADHelper.SetScale();
        }
        TextHorizontalMode horizontalMode = TextHorizontalMode.TextLeft;
        AttachmentPoint attachmentPoint = AttachmentPoint.BaseLeft;

        if (lineBlockJigRotation > 1.5 && lineBlockJigRotation < 1.6)
        {
          blockRefPosition = new Point3d(blockRefPosition.X + 1, blockRefPosition.Y, 0);
        }

        if (lineBlockJigRotation > 4.7 && lineBlockJigRotation < 4.8)
        {
          horizontalMode = TextHorizontalMode.TextRight;
          attachmentPoint = AttachmentPoint.BaseRight;
          blockRefPosition = new Point3d(blockRefPosition.X - 1, blockRefPosition.Y, 0);
        }

        if (lineBlockJigRotation > 3.14 && lineBlockJigRotation < 3.15)
        {
          blockRefPosition = new Point3d(
            blockRefPosition.X + 1.125 / AutoCADHelper.Scale,
            blockRefPosition.Y - 1.125 / AutoCADHelper.Scale,
            0
          );
        }

        if (lineBlockJigRotation == 0)
        {
          blockRefPosition = new Point3d(
            blockRefPosition.X + 1.125 / AutoCADHelper.Scale,
            blockRefPosition.Y + 1.375 / AutoCADHelper.Scale,
            0
          );
        }

        double yTransform = 1.375;
        for (int i = 0; i < labels.Count; i++)
        {
          AutoCADHelper.CreateAndPositionText(
            tr,
            labels[i],
            "RPM",
            1.125 / AutoCADHelper.Scale,
            0.85,
            2,
            "E-TXT1",
            new Point3d(
              blockRefPosition.X,
              blockRefPosition.Y - (yTransform * i) / AutoCADHelper.Scale,
              0
            ),
            horizontalMode,
            TextVerticalMode.TextBase,
            attachmentPoint
          );
        }

        tr.Commit();
      }
      return (coveredLength, remainingLength, true);
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
