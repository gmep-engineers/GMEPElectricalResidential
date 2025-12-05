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

    public void ProcessWallReceptacles(
      List<ObjectId> wallLines,
      List<string> labels,
      double minDistance,
      double maxDistance
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

      (List<ObjectId> wallLines, List<IntPoint> wallPoints) =
        AutoCADHelper.DefineMultiSidedPerimeter("E-WALL", "wall");
      if (selectedOption == "KitchenCountertop")
      {
        ProcessWallReceptacles(wallLines, new List<string>() { "+42\"", "GFI" }, 24, 48);
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
        ProcessWallReceptacles(wallLines, new List<string>() { }, 24, 144);
      }
    }
  }
}
