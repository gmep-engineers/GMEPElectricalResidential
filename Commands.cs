using System;
using System.Collections.Generic;
using System.Net;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.DatabaseServices.Filters;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.GraphicsInterface;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Windows;
using Autodesk.AutoCAD.Windows.ToolPalette;
using GMEPElectricalResidential.LoadCalculations;
using GMEPElectricalResidential.SingleLineDiagram;

namespace GMEPElectricalResidential
{
  public class Commands
  {
    private static LOAD_CALCULATION_FORM _loadCalculationForm;
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
        _loadCalculationForm = new LOAD_CALCULATION_FORM(this);
      }

      _loadCalculationForm.Show();
      _loadCalculationForm.BringToFront();
    }

    [CommandMethod("Receptacle")]
    public void Receptacle()
    {
      List<string> roomTypes = new List<string>()
      {
        "KitchenCountertop",
        "KitchenIsland",
        "FamilyRoom",
        "DiningRoom",
        "LivingRoom",
        "Parlor",
        "Library",
        "Den",
        "Sunroom",
        "Bedroom",
        "RecreationRoom",
        "Bathroom",
        "OutdoorGrade-LevelEntrance/Exit",
        "OutdoorOne-andTwo-FamilyDwelling",
        "Balcony",
        "Laundry",
        "Basement",
        "Garage",
        "Hallway",
        "Foyer",
      };
      var doc = Application.DocumentManager.MdiActiveDocument;
      if (doc == null)
        return;

      var db = doc.Database;
      var ed = doc.Editor;

      PromptKeywordOptions keywordOptions = new PromptKeywordOptions("");
      PromptResult keywordResult;
      keywordOptions.Message = "\nSelect room type:";
      foreach (var roomType in roomTypes)
      {
        keywordOptions.Keywords.Add(roomType);
      }

      keywordOptions.AllowNone = true;
      keywordResult = ed.GetKeywords(keywordOptions);
      if (keywordResult.Status != PromptStatus.OK)
      {
        ed.WriteMessage("\nCommand cancelled.");
        return;
      }
      string selectedOption = keywordResult.StringResult;

      if (selectedOption == "KitchenCountertop")
      {
        // HERE follow same line creation method as phr
        // create a poly line on a layer titled SINK
        (List<ObjectId> sinkLines, List<IntPoint> sinkPoints) =
          AutoCADHelper.DefineFourSidedPerimeter("E-SINK", "sink");
        (List<ObjectId> rangeLines, List<IntPoint> rangePoints) =
          AutoCADHelper.DefineFourSidedPerimeter("E-RANGE", "range");
        (List<ObjectId> wallLines, List<IntPoint> wallPoints) =
          AutoCADHelper.DefineMultiSidedPerimeter("E-WALL", "wall");

        foreach (var l in wallLines)
        {
          bool wallDone = false;
          int coveredLength = 0;
          while (!wallDone)
          {
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
              Line line = (Line)tr.GetObject(l, OpenMode.ForWrite);
              if (line.Length < 24)
              {
                wallDone = true;
                break;
              }
              if (line.Length < coveredLength)
              {
                wallDone = true;
                break;
              }
              Point3d startPoint;
              Point3d endPoint;
              if (line.StartPoint.X >= line.EndPoint.X)
              {
                startPoint = line.EndPoint;
                endPoint = line.StartPoint;
              }
              else
              {
                startPoint = line.StartPoint;
                endPoint = line.EndPoint;
              }
              if (line.StartPoint.Y >= line.EndPoint.Y)
              {
                startPoint = line.EndPoint;
                endPoint = line.StartPoint;
              }
              else
              {
                startPoint = line.StartPoint;
                endPoint = line.EndPoint;
              }
              Vector3d dir = (endPoint - startPoint).GetNormal();
              double angle = dir.AngleOnPlane(new Plane(Point3d.Origin, Vector3d.ZAxis));

              BlockTable bt = (BlockTable)
                tr.GetObject(line.Database.BlockTableId, OpenMode.ForRead);
              string blockName = "GMEP REC";
              if (!bt.Has(blockName))
              {
                ed.WriteMessage($"\nBlock '{blockName}' not found in drawing.");
                return;
              }
              ObjectId blockDefId = bt[blockName];
              BlockTableRecord btr = (BlockTableRecord)
                tr.GetObject(line.OwnerId, OpenMode.ForWrite);
              LineBlockJig lineBlockJig = new LineBlockJig(line, blockDefId, 1, angle);
              PromptResult jigResult = ed.Drag(lineBlockJig);
              if (jigResult.Status != PromptStatus.OK)
              {
                break;
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

              coveredLength += 144;
            }
          }
        }

        // create a poly line on a layer titled RANGE
        // create a poly line on a layer titled WALL
        // follow same line procedure as the arrows
      }
      else if (selectedOption == "KitchenIsland") { }
      else if (selectedOption == "Bathroom") { }
      else if (selectedOption == "OutdoorGrade-LevelEntrance/Exit") { }
      else if (selectedOption == "OutdoorOne-andTwo-FamilyDwelling") { }
      else if (selectedOption == "Balcony") { }
      else if (selectedOption == "Laundry") { }
      else if (selectedOption == "Basement") { }
      else if (selectedOption == "Garage") { }
      else if (selectedOption == "Hallway") { }
      else if (selectedOption == "Foyer") { }
      else
      {
        ed.WriteMessage("\nPoop.");
      }
    }
  }
}
