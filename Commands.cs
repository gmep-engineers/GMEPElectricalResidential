using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
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
using GMEPElectricalResidential.HelperFiles;
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

    public void ProcessWallReceptacles(
      List<ObjectId> wallLines,
      List<string> labels,
      double minDistance,
      double maxDistance,
      double initialDistance
    )
    {
      double remainingLength = 0;
      foreach (var l in wallLines)
      {
        bool wallDone = false;
        double coveredLength = 0;
        while (!wallDone)
        {
          bool cont;
          (coveredLength, remainingLength, cont) = AutoCADHelper.PlaceBlockOnLine(
            coveredLength,
            remainingLength,
            l,
            "GMEP REC",
            minDistance,
            maxDistance,
            initialDistance,
            labels
          );
          if (!cont)
          {
            wallDone = true;
          }
        }
      }
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
      var doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
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
        (List<ObjectId> wallLines, List<IntPoint> wallPoints) =
          AutoCADHelper.DefineMultiSidedPerimeter("E-WALL", "wall");
        ProcessWallReceptacles(wallLines, new List<string>() { "+42\"", "GFI" }, 24, 48, 24);
      }
      else if (selectedOption == "KitchenIsland")
      {
        // Create a closed polyline - follow DefineLightingLocation command
        PromptPointOptions ppo = new PromptPointOptions("\nSpecify start point: ");
        PromptPointResult ppr = ed.GetPoint(ppo);
        if (ppr.Status != PromptStatus.OK)
          return;

        Point3d startPoint = ppr.Value;

        PolyLineJig jig = new PolyLineJig(startPoint);
        while (true)
        {
          PromptResult res = ed.Drag(jig);
          if (res.Status == PromptStatus.OK)
          {
            jig.AddVertex(jig.CurrentPoint);
          }
          if (res.Status == PromptStatus.Cancel || res.StringResult == "Close")
          {
            break;
          }
        }
        Autodesk.AutoCAD.DatabaseServices.Polyline polyline;
        double area = 0;
        using (Transaction tr = db.TransactionManager.StartTransaction())
        {
          BlockTable bt = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
          BlockTableRecord btr =
            tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
          polyline = jig.GetPolyline();

          if (polyline != null)
          {
            if (!polyline.Closed)
            {
              MessageBox.Show("Polyline is not closed. You must create a closed polyline.");
            }
            else
            {
              btr.AppendEntity(polyline);
              tr.AddNewlyCreatedDBObject(polyline, true);
              tr.Commit();
              area = polyline.Area / 144;
            }
          }
        }
        if (area == 0)
        {
          return;
        }

        int numRecs = 1;
        area -= 9;
        while (area > 0)
        {
          numRecs++;
          area -= 18;
        }
        int objectIdIdx = 0;
        for (int i = 0; i < numRecs; i++)
        {
          ed.WriteMessage(
            "\nPlace " + (i + 1).ToString() + "/" + numRecs + " for '" + "Kitchen Island" + "'"
          );
          ObjectId blockId;
          try
          {
            Point3d point;
            double rotation = 0;
            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
              BlockTable bt = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);
              BlockTableRecord duplex = (BlockTableRecord)
                tr.GetObject(bt["GMEP DUPLEX"], OpenMode.ForRead);
              BlockTableRecord duplexData = (BlockTableRecord)
                tr.GetObject(bt["GMEP DUPLEXDATA"], OpenMode.ForRead);
              BlockTableRecord floorDuplex = (BlockTableRecord)
                tr.GetObject(bt["GMEP FLOOR DUPLEX"], OpenMode.ForRead);
              BlockTableRecord floorDuplexData = (BlockTableRecord)
                tr.GetObject(bt["GMEP FLOOR DUPLEXDATA"], OpenMode.ForRead);
              BlockTableRecord quad = (BlockTableRecord)
                tr.GetObject(bt["GMEP QUAD"], OpenMode.ForRead);
              BlockTableRecord quadData = (BlockTableRecord)
                tr.GetObject(bt["GMEP QUADDATA"], OpenMode.ForRead);
              BlockTableRecord floorQuad = (BlockTableRecord)
                tr.GetObject(bt["GMEP FLOOR QUAD"], OpenMode.ForRead);
              BlockTableRecord floorQuadData = (BlockTableRecord)
                tr.GetObject(bt["GMEP FLOOR QUADDATA"], OpenMode.ForRead);

              ConvenienceRecJig blockJig = new ConvenienceRecJig();

              List<ObjectId> objectIdList;

              if (numRecs - i > 2)
              {
                objectIdList = new List<ObjectId>()
                {
                  duplex.ObjectId,
                  duplexData.ObjectId,
                  floorDuplex.ObjectId,
                  floorDuplexData.ObjectId,
                  quad.ObjectId,
                  quadData.ObjectId,
                  floorQuad.ObjectId,
                  floorQuadData.ObjectId,
                };
              }
              else
              {
                objectIdList = new List<ObjectId>()
                {
                  duplex.ObjectId,
                  duplexData.ObjectId,
                  floorDuplex.ObjectId,
                  floorDuplexData.ObjectId,
                };
              }

              if (objectIdIdx >= objectIdList.Count)
              {
                objectIdIdx = 0;
              }

              (PromptResult, ObjectId, int) res = blockJig.DragMe(
                objectIdList[objectIdIdx],
                objectIdList,
                objectIdIdx,
                out point
              );
              if (res.Item1.Status == PromptStatus.OK)
              {
                objectIdIdx = res.Item3;

                BlockTableRecord curSpace = (BlockTableRecord)
                  tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite);

                BlockReference br = new BlockReference(point, res.Item2);
                RotateJig rotateJig = new RotateJig(br);
                PromptResult rotatePromptResult = ed.Drag(rotateJig);

                if (rotatePromptResult.Status != PromptStatus.OK)
                {
                  return;
                }
                rotation = br.Rotation;

                curSpace.AppendEntity(br);

                tr.AddNewlyCreatedDBObject(br, true);
                blockId = br.Id;
                double circuitOffsetX = 0;
                double circuitOffsetY = 0;
                switch (rotation)
                {
                  case var _ when rotation > 5.49:
                    circuitOffsetY = 8.5;
                    circuitOffsetX = 4.5;
                    break;
                  case var _ when rotation > 4.71:
                    circuitOffsetY = 4.5;
                    break;
                  case var _ when rotation > 2.35:
                    circuitOffsetY = 1.5;
                    circuitOffsetX = -4.5;
                    break;
                  case var _ when rotation > 1.57:
                    circuitOffsetY = -4.5;
                    break;
                  default:
                    circuitOffsetY = -5;
                    break;
                }
              }
              else
              {
                return;
              }
              tr.Commit();
            }
          }
          catch (System.Exception ex)
          {
            Console.WriteLine(ex.ToString());
          }
        }

        // Standard block jig rotation jig to place appropriate # of recs
      }
      else if (selectedOption == "Bathroom")
      {
        // Standard block jig with height and gfi
      }
      else if (selectedOption == "OutdoorGrade-LevelEntrance/Exit") { }
      else if (selectedOption == "OutdoorOne-andTwo-FamilyDwelling") { }
      else if (selectedOption == "Balcony") { }
      else if (selectedOption == "Laundry") { }
      else if (selectedOption == "Basement") { }
      else if (selectedOption == "Garage") { }
      else if (selectedOption == "Hallway")
      {
        (List<ObjectId> wallLines, List<IntPoint> wallPoints) =
          AutoCADHelper.DefineMultiSidedPerimeter("E-WALL", "wall");
        ProcessWallReceptacles(wallLines, new List<string>() { }, 24, 240, 240);
      }
      else if (selectedOption == "Foyer") { }
      else
      {
        (List<ObjectId> wallLines, List<IntPoint> wallPoints) =
          AutoCADHelper.DefineMultiSidedPerimeter("E-WALL", "wall");
        ProcessWallReceptacles(wallLines, new List<string>() { }, 24, 144, 72);
      }
    }
  }
}
