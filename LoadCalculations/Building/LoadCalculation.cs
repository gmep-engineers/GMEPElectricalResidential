using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using GMEPElectricalResidential.HelperFiles;
using GMEPElectricalResidential.LoadCalculations.Unit;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media.Media3D;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;

namespace GMEPElectricalResidential.LoadCalculations.Building
{
  public class LoadCalculation
  {
    public static double CreateBuildingLoadCalculationTable(BuildingInformation buildingInfo, List<UnitInformation> allUnitInformation, Point3d placementPoint, bool placeTheBlocks = true)
    {
      double HEADER_HEIGHT = 0.75;
      double SUBTITLE_HEIGHT = 0.5;
      double ROW_HEIGHT = 0.25;
      double COLUMN_WIDTH = 1.5;
      double WIDTH_NO_COLS = 6.7034;
      double INITIAL_WIDTH = 8.2033907256843577;
      double shiftHeight = 0;
      double currentHeight = HEADER_HEIGHT;
      string newBlockName = $"Building {buildingInfo.Name}" + $" ID{buildingInfo.ID}";
      var buildingUnitInfo = buildingInfo.GetListOfBuildingUnitTypes(allUnitInformation);
      var columnCount = buildingUnitInfo.Count;
      double additionalWidth = CalculateAdditionalWidth(buildingUnitInfo, COLUMN_WIDTH);

      placementPoint = GetStartingPoint(buildingInfo, placementPoint, COLUMN_WIDTH, WIDTH_NO_COLS);

      if (buildingInfo == null)
      {
        return 0;
      }

      if (buildingInfo.Name == null)
      {
        buildingInfo.Name = "";
      }

      var acCurDb = Application.DocumentManager.MdiActiveDocument.Database;

      using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
      {
        Point3d point = new Point3d(0, 0, 0);

        BlockTable acBlkTbl;
        acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead) as BlockTable;

        BlockTableRecord acBlkTblRec;

        var existingBlock = acBlkTbl.Cast<ObjectId>()
            .Select(id => acTrans.GetObject(id, OpenMode.ForRead) as BlockTableRecord)
            .FirstOrDefault(btr => btr.Name.Contains($"ID{buildingInfo.ID}") && btr.Name.Contains("Building"));

        if (existingBlock != null)
        {
          if (existingBlock.Name != newBlockName)
          {
            existingBlock.UpgradeOpen();
            existingBlock.Name = newBlockName;
            existingBlock.DowngradeOpen();
          }
          acBlkTblRec = existingBlock;
          existingBlock.UpgradeOpen();
          WipeExistingBlockContent(acBlkTblRec);
          existingBlock.DowngradeOpen();
        }
        else
        {
          acBlkTbl.UpgradeOpen();
          acBlkTblRec = new BlockTableRecord();
          acBlkTblRec.Name = newBlockName;
          acBlkTbl.Add(acBlkTblRec);
          acTrans.AddNewlyCreatedDBObject(acBlkTblRec, true);
          acBlkTbl.DowngradeOpen();
        }

        ObjectData titleData = GetCopyPasteData("Title");
        ObjectData rowHeaderData = GetCopyPasteData("RowHeader");
        ObjectData rowEntryData = GetCopyPasteData("RowEntry");
        ObjectData subtitleData = GetCopyPasteData("Subtitle");

        // Title
        titleData = UpdateBuildingTitleData(titleData, buildingInfo, additionalWidth);
        string modifiedTitleData = JsonConvert.SerializeObject(titleData);
        CADObjectCommands.CreateObjectFromData(modifiedTitleData, point, acBlkTblRec);
        shiftHeight -= HEADER_HEIGHT;

        // Dwelling Area
        // Create Subtitle
        CreateSubtitle(subtitleData, shiftHeight, additionalWidth, point, acBlkTblRec, "Dwelling Information");
        shiftHeight -= SUBTITLE_HEIGHT;

        // Create Rows
        List<string> dwellingRowHeaders = RowHeaders.Dwelling;
        CreateRow(rowHeaderData, rowEntryData, shiftHeight, buildingUnitInfo, additionalWidth, columnCount, point, acBlkTblRec, dwellingRowHeaders);
        shiftHeight -= ROW_HEIGHT * dwellingRowHeaders.Count;

        // General Load Area
        // Create Subtitle
        CreateSubtitle(subtitleData, shiftHeight, additionalWidth, point, acBlkTblRec, "General Load");
        shiftHeight -= SUBTITLE_HEIGHT;

        // Create Rows
        List<string> generalLoadRowHeaders = RowHeaders.GeneralLoad;
        CreateRow(rowHeaderData, rowEntryData, shiftHeight, buildingUnitInfo, additionalWidth, columnCount, point, acBlkTblRec, generalLoadRowHeaders);
        shiftHeight -= ROW_HEIGHT * generalLoadRowHeaders.Count;

        // Optional Water Heater
        if (!IsWHCustomLoadForAllUnits(allUnitInformation))
        {
          // Create Rows
          List<string> optionalWaterHeaterRowHeaders = new List<string> { RowHeaders.OptionalWaterHeater };
          CreateRow(rowHeaderData, rowEntryData, shiftHeight, buildingUnitInfo, additionalWidth, columnCount, point, acBlkTblRec, optionalWaterHeaterRowHeaders);
          shiftHeight -= ROW_HEIGHT * optionalWaterHeaterRowHeaders.Count;
        }

        UpdateAllBlockReferences(newBlockName);

        acTrans.Commit();
      }

      if (placeTheBlocks)
      {
        using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
        {
          BlockTable acBlkTbl;
          acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead) as BlockTable;
          BlockTableRecord acBlkTblRec;

          if (acCurDb.TileMode)
          {
            acBlkTblRec = acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
          }
          else
          {
            acBlkTblRec = acTrans.GetObject(acBlkTbl[BlockTableRecord.PaperSpace], OpenMode.ForWrite) as BlockTableRecord;
          }

          if (acBlkTbl.Has(newBlockName))
          {
            BlockReference acBlkRef = new BlockReference(placementPoint, acBlkTbl[newBlockName]);

            acBlkTblRec.AppendEntity(acBlkRef);

            acTrans.AddNewlyCreatedDBObject(acBlkRef, true);
          }

          acTrans.Commit();
        }
      }

      return INITIAL_WIDTH + additionalWidth;
    }

    private static bool IsWHCustomLoadForAllUnits(List<UnitInformation> allUnitInformation)
    {
      return allUnitInformation.All(unit => unit.CustomLoads.Any(customLoad => customLoad.Name == "Water Heater"));
    }

    private static void CreateSubtitle(ObjectData subtitleData, double shiftHeight, double additionalWidth, Point3d point, BlockTableRecord acBlkTblRec, string subtitle)
    {
      var copiedSubtitleData = JsonConvert.DeserializeObject<ObjectData>(JsonConvert.SerializeObject(subtitleData));

      copiedSubtitleData = ShiftDataVertically(copiedSubtitleData, shiftHeight);
      copiedSubtitleData = UpdateBuildingSubtitleData(copiedSubtitleData, additionalWidth, subtitle);

      string modifiedSubtitleData = JsonConvert.SerializeObject(copiedSubtitleData);
      CADObjectCommands.CreateObjectFromData(modifiedSubtitleData, point, acBlkTblRec);
    }

    private static void CreateRow(ObjectData rowHeaderData, ObjectData rowEntryData, double shiftHeight, List<UnitInformation> buildingUnitInfo, double additionalWidth, int columnCount, Point3d point, BlockTableRecord acBlkTblRec, List<string> dwellingRowHeaders)
    {
      double ROW_HEIGHT = 0.25;
      foreach (var header in dwellingRowHeaders)
      {
        var copiedRowHeaderData = JsonConvert.DeserializeObject<ObjectData>(JsonConvert.SerializeObject(rowHeaderData));
        var copiedRowEntryData = JsonConvert.DeserializeObject<ObjectData>(JsonConvert.SerializeObject(rowEntryData));

        copiedRowHeaderData = ShiftDataVertically(copiedRowHeaderData, shiftHeight);
        copiedRowEntryData = ShiftDataVertically(copiedRowEntryData, shiftHeight);

        var allRowData = UpdateRowData(copiedRowHeaderData, copiedRowEntryData, buildingUnitInfo, additionalWidth, columnCount, header);

        foreach (var rowData in allRowData)
        {
          string modifiedRowData = JsonConvert.SerializeObject(rowData);
          CADObjectCommands.CreateObjectFromData(modifiedRowData, point, acBlkTblRec);
        }

        shiftHeight -= ROW_HEIGHT;
      }
    }

    private static List<ObjectData> UpdateRowData(ObjectData rowHeaderData, ObjectData rowEntryData, List<UnitInformation> unitInfo, double additionalWidth, int colCount, string message)
    {
      var startPoint = 6.7033907256843577;
      var COLUMN_WIDTH = 1.5;

      List<ObjectData> rowData = new List<ObjectData>();

      var rowHeaderTextObj = rowHeaderData.Texts.FirstOrDefault(text => text.Contents.Contains("Unit"));
      rowHeaderTextObj.Contents = rowHeaderTextObj.Contents.Replace("Unit", message);
      rowData.Add(rowHeaderData);

      // Shift the rowEntryData to the right by the value of startPoint
      rowEntryData = ShiftDataHorizontally(rowEntryData, startPoint);

      // Create a copy of rowEntryData for each column
      for (int i = 0; i < colCount; i++)
      {
        var copiedRowEntryData = JsonConvert.DeserializeObject<ObjectData>(JsonConvert.SerializeObject(rowEntryData));

        // Shift the copied rowEntryData to the right by COLUMN_WIDTH * i
        copiedRowEntryData = ShiftDataHorizontally(copiedRowEntryData, COLUMN_WIDTH * i);

        // Update the value of the copied rowEntryData based on the UnitInformation
        if (i < unitInfo.Count)
        {
          string value = RowHeaders.GetValueFromUnitInfo(message, unitInfo[i]);
          var textObj = copiedRowEntryData.Texts.FirstOrDefault(text => text.Contents.Contains("A"));
          if (textObj != null)
          {
            textObj.Contents = textObj.Contents.Replace("A", value);
          }
        }

        rowData.Add(copiedRowEntryData);
      }

      return rowData;
    }

    private static ObjectData UpdateBuildingSubtitleData(ObjectData subtitleData, double additionalWidth, string message)
    {
      var textObj = subtitleData.MTexts.FirstOrDefault(mText => mText.Contents.Contains("Dwelling Information"));
      textObj.Contents = textObj.Contents.Replace("Dwelling Information", message);
      UpdateTitleOrSubtitleText(subtitleData, additionalWidth, true);

      return subtitleData;
    }

    private static ObjectData UpdateBuildingTitleData(ObjectData titleData, BuildingInformation buildingInfo, double additionalWidth)
    {
      var serviceLoadCalculationMText = titleData.MTexts.FirstOrDefault(mText => mText.Contents.Contains("SERVICE LOAD CALCULATION"));
      serviceLoadCalculationMText.Contents = serviceLoadCalculationMText.Contents.Replace("SERVICE LOAD CALCULATION", $"SERVICE LOAD CALCULATION - BUILDING {buildingInfo.Name}");
      UpdateTitleOrSubtitleText(titleData, additionalWidth);

      return titleData;
    }

    private static void UpdateAllBlockReferences(string blockName)
    {
      var acCurDb = Application.DocumentManager.MdiActiveDocument.Database;

      using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
      {
        BlockTable acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead) as BlockTable;

        if (!acBlkTbl.Has(blockName))
          return;

        ObjectId blockId = acBlkTbl[blockName];

        foreach (ObjectId btrId in acBlkTbl)
        {
          BlockTableRecord btr = (BlockTableRecord)acTrans.GetObject(btrId, OpenMode.ForRead);
          foreach (ObjectId entId in btr)
          {
            Entity ent = acTrans.GetObject(entId, OpenMode.ForRead) as Entity;
            if (ent is BlockReference br && br.BlockTableRecord == blockId)
            {
              br.UpgradeOpen();
              br.RecordGraphicsModified(true);
            }
          }
        }

        acTrans.Commit();
      }
    }

    private static void WipeExistingBlockContent(BlockTableRecord acBlkTblRec)
    {
      foreach (ObjectId id in acBlkTblRec)
      {
        DBObject obj = id.GetObject(OpenMode.ForWrite);
        obj.Erase();
      }
    }

    private static ObjectData ShiftDataVertically(ObjectData bodyData, double shiftHeight)
    {
      bodyData = JsonConvert.DeserializeObject<ObjectData>(JsonConvert.SerializeObject(bodyData));

      foreach (var polyline in bodyData.Polylines)
      {
        for (int i = 0; i < polyline.Vectors.Count; i++)
        {
          polyline.Vectors[i].Y += shiftHeight;
        }
      }

      foreach (var line in bodyData.Lines)
      {
        line.StartPoint.Y += shiftHeight;
        line.EndPoint.Y += shiftHeight;
      }

      foreach (var arc in bodyData.Arcs)
      {
        arc.Center.Y += shiftHeight;
      }

      foreach (var circle in bodyData.Circles)
      {
        circle.Center.Y += shiftHeight;
      }

      foreach (var ellipse in bodyData.Ellipses)
      {
        ellipse.Center.Y += shiftHeight;
      }

      foreach (var mText in bodyData.MTexts)
      {
        mText.Location.Y += shiftHeight;
      }

      foreach (var text in bodyData.Texts)
      {
        text.Location.Y += shiftHeight;
        text.AlignmentPoint.Y += shiftHeight;
      }

      foreach (var solid in bodyData.Solids)
      {
        for (int i = 0; i < solid.Vertices.Count; i++)
        {
          solid.Vertices[i].Y += shiftHeight;
        }
      }

      return bodyData;
    }

    private static ObjectData ShiftDataHorizontally(ObjectData bodyData, double shiftWidth)
    {
      bodyData = JsonConvert.DeserializeObject<ObjectData>(JsonConvert.SerializeObject(bodyData));

      foreach (var polyline in bodyData.Polylines)
      {
        for (int i = 0; i < polyline.Vectors.Count; i++)
        {
          polyline.Vectors[i].X += shiftWidth;
        }
      }

      foreach (var line in bodyData.Lines)
      {
        line.StartPoint.X += shiftWidth;
        line.EndPoint.X += shiftWidth;
      }

      foreach (var arc in bodyData.Arcs)
      {
        arc.Center.X += shiftWidth;
      }

      foreach (var circle in bodyData.Circles)
      {
        circle.Center.X += shiftWidth;
      }

      foreach (var ellipse in bodyData.Ellipses)
      {
        ellipse.Center.X += shiftWidth;
      }

      foreach (var mText in bodyData.MTexts)
      {
        mText.Location.X += shiftWidth;
      }

      foreach (var text in bodyData.Texts)
      {
        text.Location.X += shiftWidth;
        text.AlignmentPoint.X += shiftWidth;
      }

      foreach (var solid in bodyData.Solids)
      {
        for (int i = 0; i < solid.Vertices.Count; i++)
        {
          solid.Vertices[i].X += shiftWidth;
        }
      }

      return bodyData;
    }

    private static ObjectData GetCopyPasteData(string fileName)
    {
      string assemblyLocation = Assembly.GetExecutingAssembly().Location;
      string assemblyDirectory = Path.GetDirectoryName(assemblyLocation);
      string relativeFilePath = Path.Combine(assemblyDirectory, $"LoadCalculations\\Building\\BlockData\\{fileName}.json");

      string jsonData = File.ReadAllText(relativeFilePath);
      ObjectData objectData = JsonConvert.DeserializeObject<ObjectData>(jsonData);
      return objectData;
    }

    private static double CalculateAdditionalWidth(List<Unit.UnitInformation> buildingUnitInfo, double COLUMN_WIDTH)
    {
      if (buildingUnitInfo.Count > 1)
      {
        double additionalWidth = (buildingUnitInfo.Count - 1) * COLUMN_WIDTH;
        return additionalWidth;
      }
      return 0;
    }

    private static void UpdateTitleOrSubtitleText(ObjectData titleData, double additionalWidth, bool isSubtitle = false)
    {
      var INITIAL_WIDTH = 8.2033907256843577;

      foreach (var polyline in titleData.Polylines)
      {
        for (int i = 0; i < polyline.Vectors.Count; i++)
        {
          if (polyline.Vectors[i].X == INITIAL_WIDTH)
          {
            polyline.Vectors[i].X += additionalWidth;
          }
        }
      }

      if (!isSubtitle)
      {
        foreach (var mText in titleData.MTexts)
        {
          mText.Location.X += additionalWidth / 2;
        }
      }
    }

    private static Point3d GetStartingPoint(BuildingInformation buildingInfo, Point3d point, double COLUMN_WIDTH, double widthNoCols)
    {
      var numberOfUnitTypes = GetNumberOfUnitTypes(buildingInfo);
      var totalWidth = widthNoCols + (COLUMN_WIDTH * numberOfUnitTypes);

      var newPoint = new Point3d(point.X - totalWidth, point.Y, point.Z);
      return newPoint;
    }

    private static int GetNumberOfUnitTypes(BuildingInformation buildingInfo)
    {
      var counters = buildingInfo.Counters;
      var numberOfUnitTypes = counters.Count(c => c.Count > 0);

      return numberOfUnitTypes;
    }
  }

  public static class RowHeaders
  {
    public static List<string> Dwelling = new List<string> {
      "Unit",
      "Floor Area"
    };

    public static List<string> GeneralLoad = new List<string> {
      "General Lighting Subtotal (Floor Area x 3VA/ft²) (CEC 220.42)",
      "Small Appliance (3-20ACK by CEC 210.11)",
      "Laundry (1-20ACKT by CEC 210.11)",
      "Lighting And Appliance Load Total",
      "Bathroom (1-20ACKT by CEC 210.11)",
      "Dishwasher",
      "Microwave",
      "Garbage Disposal",
      "Bathroom Fans",
      "Garage Door Opener",
      "Dryer",
      "Range",
      "Refrigerator",
      "Oven",
      "Cooktop",
    };

    public static string OptionalWaterHeater = "Water Heater";

    public static List<string> GeneralLoadCalculations = new List<string>
    {
      "Total General Load",
      "First 10 KVA @ 100% (CEC 220.82(B))",
      "Remaining @ 40% (CEC 220.82(B))",
      "General Calculated Load (CEC 220.82(B))"
    };

    public static string GetValueFromUnitInfo(string message, UnitInformation unitInfo)
    {
      switch (message)
      {
        case "Unit":
          return unitInfo.Name;

        case "Floor Area":
          return unitInfo.DwellingArea.FloorArea + "ft²";

        case "General Lighting Subtotal (Floor Area x 3VA/ft²) (CEC 220.42)":
          return unitInfo.GeneralLoads.Lighting.GetLoad().ToString() + "VA";

        case "Small Appliance (3-20ACK by CEC 210.11)":
          return unitInfo.GeneralLoads.SmallAppliance.GetLoad().ToString() + "VA";

        case "Laundry (1-20ACKT by CEC 210.11)":
          return unitInfo.GeneralLoads.Laundry.GetLoad().ToString() + "VA";

        case "Lighting And Appliance Load Total":
          int lightingAndApplianceTotal = unitInfo.GeneralLoads.Lighting.GetLoad() +
                                          unitInfo.GeneralLoads.SmallAppliance.GetLoad() +
                                          unitInfo.GeneralLoads.Laundry.GetLoad();
          return lightingAndApplianceTotal.ToString() + "VA";

        case "Bathroom (1-20ACKT by CEC 210.11)":
          return unitInfo.GeneralLoads.Bathroom.GetLoad().ToString() + "VA";

        case "Dishwasher":
          return unitInfo.GeneralLoads.Dishwasher.GetLoad().ToString() + "VA";

        case "Microwave":
          return unitInfo.GeneralLoads.MicrowaveOven.GetLoad().ToString() + "VA";

        case "Garbage Disposal":
          return unitInfo.GeneralLoads.GarbageDisposal.GetLoad().ToString() + "VA";

        case "Bathroom Fans":
          return unitInfo.GeneralLoads.BathroomFans.GetLoad().ToString() + "VA";

        case "Garage Door Opener":
          return unitInfo.GeneralLoads.GarageDoorOpener.GetLoad().ToString() + "VA";

        case "Dryer":
          return unitInfo.GeneralLoads.Dryer.GetLoad().ToString() + "VA";

        case "Range":
          return unitInfo.GeneralLoads.Range.GetLoad().ToString() + "VA";

        case "Refrigerator":
          return unitInfo.GeneralLoads.Refrigerator.GetLoad().ToString() + "VA";

        case "Oven":
          return unitInfo.GeneralLoads.Oven.GetLoad().ToString() + "VA";

        case "Cooktop":
          return unitInfo.GeneralLoads.Cooktop.GetLoad().ToString() + "VA";

        case "Water Heater":
          return unitInfo.GeneralLoads.WaterHeater.GetLoad().ToString() + "VA";

        case "Total General Load":
          return unitInfo.Totals.TotalGeneralLoad.ToString() + "VA";

        case "First 10 KVA @ 100% (CEC 220.82(B))":
          return unitInfo.Totals.First10KVA().ToString() + "VA";

        case "Remaining @ 40% (CEC 220.82(B))":
          return unitInfo.Totals.RemainderAt40Percent().ToString() + "VA";

        case "General Calculated Load (CEC 220.82(B))":
          int generalCalculatedLoad = unitInfo.Totals.First10KVA() + unitInfo.Totals.RemainderAt40Percent();
          return generalCalculatedLoad.ToString() + "VA";

        default:
          return "";
      }
    }
  }
}