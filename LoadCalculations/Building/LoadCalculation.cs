using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Internal.DatabaseServices;
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
        ObjectData spacerData = GetCopyPasteData("Spacer");

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
        CreateRow(rowHeaderData, rowEntryData, shiftHeight, buildingUnitInfo, columnCount, point, acBlkTblRec, dwellingRowHeaders);
        shiftHeight -= ROW_HEIGHT * dwellingRowHeaders.Count;

        // General Load Area
        // Create Subtitle
        CreateSubtitle(subtitleData, shiftHeight, additionalWidth, point, acBlkTblRec, "General Load");
        shiftHeight -= SUBTITLE_HEIGHT;

        // Create Rows
        List<string> generalLoadRowHeaders = RowHeaders.GeneralLoad;
        CreateRow(rowHeaderData, rowEntryData, shiftHeight, buildingUnitInfo, columnCount, point, acBlkTblRec, generalLoadRowHeaders);
        shiftHeight -= ROW_HEIGHT * generalLoadRowHeaders.Count;

        // Custom General Loads
        // Create Rows
        List<string> customGeneralLoadRowHeaders = buildingUnitInfo.SelectMany(unit => unit.GeneralLoads.Customs.Select(customLoad => customLoad.Name)).Distinct().ToList();
        CreateRow(rowHeaderData, rowEntryData, shiftHeight, buildingUnitInfo, columnCount, point, acBlkTblRec, customGeneralLoadRowHeaders);
        shiftHeight -= ROW_HEIGHT * customGeneralLoadRowHeaders.Count;

        // General Load Calculations
        // Create Spacer
        CreateSpacer(spacerData, shiftHeight, additionalWidth, point, acBlkTblRec);
        shiftHeight -= ROW_HEIGHT;

        // Create Rows
        List<string> generalLoadCalculationsRowHeaders = RowHeaders.GeneralLoadCalculations;
        CreateRow(rowHeaderData, rowEntryData, shiftHeight, buildingUnitInfo, columnCount, point, acBlkTblRec, generalLoadCalculationsRowHeaders);
        shiftHeight -= ROW_HEIGHT * generalLoadCalculationsRowHeaders.Count;

        // AC Load Area
        // Create Subtitle
        CreateSubtitle(subtitleData, shiftHeight, additionalWidth, point, acBlkTblRec, "AC Load");
        shiftHeight -= SUBTITLE_HEIGHT;

        // Create Rows
        List<string> acLoadRowHeaders = RowHeaders.ACLoad;
        CreateRow(rowHeaderData, rowEntryData, shiftHeight, buildingUnitInfo, columnCount, point, acBlkTblRec, acLoadRowHeaders);
        shiftHeight -= ROW_HEIGHT * acLoadRowHeaders.Count;

        // Custom Loads if they exist
        if (buildingUnitInfo.Any(unit => unit.CustomLoads.Count > 0))
        {
          // Create Subtitle
          CreateSubtitle(subtitleData, shiftHeight, additionalWidth, point, acBlkTblRec, "Additional Load");
          shiftHeight -= SUBTITLE_HEIGHT;

          // Create Rows
          List<string> customLoadRowHeaders = buildingUnitInfo.SelectMany(unit => unit.CustomLoads.Select(customLoad => customLoad.Name)).Distinct().ToList();
          CreateRow(rowHeaderData, rowEntryData, shiftHeight, buildingUnitInfo, columnCount, point, acBlkTblRec, customLoadRowHeaders, true);
          shiftHeight -= ROW_HEIGHT * customLoadRowHeaders.Count;
        }

        // Service Sizing Area
        // Create Subtitle
        CreateSubtitle(subtitleData, shiftHeight, additionalWidth, point, acBlkTblRec, "Calculated Load for Service");
        shiftHeight -= SUBTITLE_HEIGHT;

        // Create Rows
        List<string> serviceSizingRowHeaders = RowHeaders.ServiceSizingUnits;
        CreateRow(rowHeaderData, rowEntryData, shiftHeight, buildingUnitInfo, columnCount, point, acBlkTblRec, serviceSizingRowHeaders, false, buildingInfo);
        shiftHeight -= ROW_HEIGHT * serviceSizingRowHeaders.Count;

        // Create Spacer
        CreateSpacer(spacerData, shiftHeight, additionalWidth, point, acBlkTblRec);
        shiftHeight -= ROW_HEIGHT;

        // Create Rows
        List<string> serviceSizingBuildingRowHeaders = RowHeaders.ServiceSizingBuilding;
        HelperClass.SaveDataToJsonFileOnDesktop(serviceSizingBuildingRowHeaders, "serviceSizingBuildingRowHeaders.json");
        UpdateBuildingCalculationVoltage(serviceSizingBuildingRowHeaders, buildingInfo);
        HelperClass.SaveDataToJsonFileOnDesktop(serviceSizingBuildingRowHeaders, "serviceSizingBuildingRowHeadersAfter.json");
        CreateRow(rowHeaderData, rowEntryData, shiftHeight, buildingUnitInfo, columnCount, point, acBlkTblRec, serviceSizingBuildingRowHeaders, false, buildingInfo, true);
        shiftHeight -= ROW_HEIGHT * serviceSizingBuildingRowHeaders.Count;

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

      if (columnCount == 1)
      {
        return INITIAL_WIDTH;
      }
      else
      {
        return INITIAL_WIDTH + additionalWidth;
      }
    }

    private static void CreateSpacer(ObjectData spacerData, double shiftHeight, double additionalWidth, Point3d point, BlockTableRecord acBlkTblRec)
    {
      double INITIAL_WIDTH = 8.2033907256843577;

      var copiedSpacerData = JsonConvert.DeserializeObject<ObjectData>(JsonConvert.SerializeObject(spacerData));

      copiedSpacerData = ShiftDataVertically(copiedSpacerData, shiftHeight);

      foreach (var polyline in copiedSpacerData.Polylines)
      {
        for (int i = 0; i < polyline.Vectors.Count; i++)
        {
          if (Math.Abs(polyline.Vectors[i].X - INITIAL_WIDTH) < 0.001)
          {
            polyline.Vectors[i].X = INITIAL_WIDTH + additionalWidth;
          }
        }
      }

      string modifiedSpacerData = JsonConvert.SerializeObject(copiedSpacerData);
      CADObjectCommands.CreateObjectFromData(modifiedSpacerData, point, acBlkTblRec);
    }

    private static void CreateSubtitle(ObjectData subtitleData, double shiftHeight, double additionalWidth, Point3d point, BlockTableRecord acBlkTblRec, string subtitle)
    {
      var copiedSubtitleData = JsonConvert.DeserializeObject<ObjectData>(JsonConvert.SerializeObject(subtitleData));

      copiedSubtitleData = ShiftDataVertically(copiedSubtitleData, shiftHeight);
      copiedSubtitleData = UpdateBuildingSubtitleData(copiedSubtitleData, additionalWidth, subtitle);

      string modifiedSubtitleData = JsonConvert.SerializeObject(copiedSubtitleData);
      CADObjectCommands.CreateObjectFromData(modifiedSubtitleData, point, acBlkTblRec);
    }

    private static void CreateRow(ObjectData rowHeaderData, ObjectData rowEntryData, double shiftHeight, List<UnitInformation> buildingUnitInfo, int columnCount, Point3d point, BlockTableRecord acBlkTblRec, List<string> rowHeaders, bool isCustom = false, BuildingInformation buildingInfo = null, bool isBuildingData = false)
    {
      double ROW_HEIGHT = 0.25;
      foreach (var header in rowHeaders)
      {
        var copiedRowHeaderData = JsonConvert.DeserializeObject<ObjectData>(JsonConvert.SerializeObject(rowHeaderData));
        var copiedRowEntryData = JsonConvert.DeserializeObject<ObjectData>(JsonConvert.SerializeObject(rowEntryData));

        copiedRowHeaderData = ShiftDataVertically(copiedRowHeaderData, shiftHeight);
        copiedRowEntryData = ShiftDataVertically(copiedRowEntryData, shiftHeight);

        var allRowData = UpdateRowData(copiedRowHeaderData, copiedRowEntryData, buildingUnitInfo, columnCount, header, isCustom, buildingInfo, isBuildingData);

        foreach (var rowData in allRowData)
        {
          string modifiedRowData = JsonConvert.SerializeObject(rowData);
          CADObjectCommands.CreateObjectFromData(modifiedRowData, point, acBlkTblRec);
        }

        shiftHeight -= ROW_HEIGHT;
      }
    }

    private static List<ObjectData> UpdateRowData(ObjectData rowHeaderData, ObjectData rowEntryData, List<UnitInformation> unitInfo, int colCount, string message, bool isCustom = false, BuildingInformation buildingInfo = null, bool isBuildingData = false)
    {
      var startPoint = 6.7033907256843577;
      var COLUMN_WIDTH = 1.5;

      List<ObjectData> rowData = new List<ObjectData>();

      var rowHeaderTextObj = rowHeaderData.Texts.FirstOrDefault(text => text.Contents.Contains("Unit"));
      UpdateHeaderText(message, rowHeaderTextObj);
      rowData.Add(rowHeaderData);

      rowEntryData = ShiftDataHorizontally(rowEntryData, startPoint);

      unitInfo = unitInfo.OrderBy(u => u.ID).ToList();

      if (isBuildingData && buildingInfo.Counters.Count != 0)
      {
        var copiedRowEntryData = JsonConvert.DeserializeObject<ObjectData>(JsonConvert.SerializeObject(rowEntryData));

        copiedRowEntryData = ExtendRectangleHorizontally(copiedRowEntryData, COLUMN_WIDTH * (colCount - 1));
        copiedRowEntryData = ShiftTextHorizontally(copiedRowEntryData, COLUMN_WIDTH * (colCount - 1));

        if (buildingInfo != null)
        {
          string value = RowHeaders.GetValueFromBuildingInfo(message, buildingInfo);
          var textObj = copiedRowEntryData.Texts.FirstOrDefault(text => text.Contents.Contains("A"));
          if (textObj != null)
          {
            textObj.Contents = textObj.Contents.Replace("A", value);
          }
        }

        rowData.Add(copiedRowEntryData);
      }
      else
      {
        for (int i = 0; i < colCount; i++)
        {
          var copiedRowEntryData = JsonConvert.DeserializeObject<ObjectData>(JsonConvert.SerializeObject(rowEntryData));

          copiedRowEntryData = ShiftDataHorizontally(copiedRowEntryData, COLUMN_WIDTH * i);

          if (i < unitInfo.Count)
          {
            string value = "";
            if (!isCustom)
            {
              value = RowHeaders.GetValueFromUnitInfo(message, unitInfo[i], buildingInfo);
            }
            else
            {
              value = RowHeaders.GetValueFromCustomUnitInfo(message, unitInfo[i]);
            }
            var textObj = copiedRowEntryData.Texts.FirstOrDefault(text => text.Contents.Contains("A"));
            if (textObj != null)
            {
              textObj.Contents = textObj.Contents.Replace("A", value);
            }
          }

          rowData.Add(copiedRowEntryData);
        }
      }

      return rowData;
    }

    private static void UpdateHeaderText(string message, TextData rowHeaderTextObj)
    {
      if (message == "Laundry")
      {
        message = "Laundry (1-20ACKT by CEC 210.11)";
      }
      else if (message == "Bathroom")
      {
        message = "Bathroom (1-20ACKT by CEC 210.11)";
      }
      else if (message == "Small Appliance")
      {
        message = "Small Appliance (3-20ACK by CEC 210.11)";
      }
      rowHeaderTextObj.Contents = rowHeaderTextObj.Contents.Replace("Unit", message);
    }

    private static ObjectData ShiftTextHorizontally(ObjectData copiedRowEntryData, double shiftDistance)
    {
      foreach (var text in copiedRowEntryData.Texts)
      {
        text.Location.X += shiftDistance / 2;
        text.AlignmentPoint.X += shiftDistance / 2;
      }

      return copiedRowEntryData;
    }

    private static ObjectData ExtendRectangleHorizontally(ObjectData copiedRowEntryData, double extendDistance)
    {
      foreach (var polyline in copiedRowEntryData.Polylines)
      {
        for (int i = 0; i < polyline.Vectors.Count; i++)
        {
          if (polyline.Vectors[i].X == 8.2033907256843577)
          {
            polyline.Vectors[i].X += extendDistance;
          }
        }
      }

      return copiedRowEntryData;
    }

    private static ObjectData UpdateBuildingSubtitleData(ObjectData subtitleData, double additionalWidth, string message)
    {
      var textObj = subtitleData.MTexts.FirstOrDefault(mText => mText.Contents.Contains("Dwelling Information"));
      textObj.Contents = textObj.Contents.Replace("Dwelling Information", message);
      textObj.Contents = textObj.Contents.Replace("\\Farial|c0", "\\fArial Rounded MT Bold|b1|i1|c0|p34");
      UpdateTitleOrSubtitleText(subtitleData, additionalWidth, true);

      return subtitleData;
    }

    private static ObjectData UpdateBuildingTitleData(ObjectData titleData, BuildingInformation buildingInfo, double additionalWidth)
    {
      var serviceLoadCalculationMText = titleData.MTexts.FirstOrDefault(mText => mText.Contents.Contains("SERVICE LOAD CALCULATION"));
      serviceLoadCalculationMText.Contents = serviceLoadCalculationMText.Contents.Replace("SERVICE LOAD CALCULATION", $"SERVICE LOAD CALCULATION - BUILDING {buildingInfo.Name}");
      serviceLoadCalculationMText.Contents = serviceLoadCalculationMText.Contents.Replace("\\Farial|c0", "\\fArial Rounded MT Bold|b1|i0|c0|p34");
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
      else if (buildingUnitInfo.Count == 1)
      {
        return 0;
      }
      else
      {
        return -COLUMN_WIDTH;
      }
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

    private static void UpdateBuildingCalculationVoltage(List<string> serviceSizingBuildingRowHeaders, BuildingInformation buildingInfo)
    {
      for (int i = 0; i < serviceSizingBuildingRowHeaders.Count; i++)
      {
        if (serviceSizingBuildingRowHeaders[i].Contains("Total Amperage"))
        {
          serviceSizingBuildingRowHeaders[i] = $"Total Amperage @120/{buildingInfo.Voltage} {buildingInfo.Phase}";
        }
      }
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
    };

    public static List<string> GeneralLoadCalculations = new List<string>
    {
      "Total General Load",
      "First 10 KVA @ 100% (CEC 220.82(B))",
      "Remaining @ 40% (CEC 220.82(B))",
      "General Calculated Load (CEC 220.82(B))"
    };

    public static List<string> ACLoad = new List<string>
    {
      "Outdoor Condensing Unit Load",
      "Indoor Fan Coil Unit Load",
      "Space Heating Unit Load",
      "Total AC Load (CEC 220.82(C))"
    };

    public static List<string> ServiceSizingUnits = new List<string>
    {
      "Number of Units Per Unit Type",
      "Load Per Unit Type ((Total General + AC + Additional) x # of Units)"
    };

    public static List<string> ServiceSizingBuilding = new List<string>
    {
      "Total Number Of Units",
      "Total Building Load (Sum of Load Per Unit Types)",
      "Demand Factor (CEC 220.84)",
      "Total Building Load with Demand Factor Applied",
      "House Load",
      "Total Building Load with Demand Factor Applied & House Load",
      "Total Amperage @120/208 1 PH",
      "Recommended Service Size"
    };

    public static string GetValueFromUnitInfo(string message, UnitInformation unitInfo, BuildingInformation buildingInfo = null)
    {
      UnitCounter unitType;

      switch (message)
      {
        case "Unit":
          return unitInfo.Name;

        case "Floor Area":
          return unitInfo.DwellingArea.FloorArea + "ft²";

        case "General Lighting Subtotal (Floor Area x 3VA/ft²) (CEC 220.42)":
          return unitInfo.GeneralLoads.Lighting.GetTotal().ToString() + "VA";

        case "Total General Load":
          return unitInfo.Totals.TotalGeneralLoad.ToString() + "VA";

        case "First 10 KVA @ 100% (CEC 220.82(B))":
          return unitInfo.Totals.First10KVA().ToString() + "VA";

        case "Remaining @ 40% (CEC 220.82(B))":
          return unitInfo.Totals.RemainderAt40Percent().ToString() + "VA";

        case "General Calculated Load (CEC 220.82(B))":
          int generalCalculatedLoad = unitInfo.Totals.First10KVA() + unitInfo.Totals.RemainderAt40Percent();
          return generalCalculatedLoad.ToString() + "VA";

        case "Outdoor Condensing Unit Load":
          return unitInfo.ACLoads.Condenser.ToString() + "VA";

        case "Indoor Fan Coil Unit Load":
          return unitInfo.ACLoads.FanCoil.ToString() + "VA";

        case "Space Heating Unit Load":
          return unitInfo.ACLoads.HeatingUnit.Heating.ToString() + "VA";

        case "Total AC Load (CEC 220.82(C))":
          return unitInfo.Totals.TotalACLoad.ToString() + "VA";

        case "Number of Units Per Unit Type":
          unitType = buildingInfo.Counters.FirstOrDefault(c => c.UnitID == unitInfo.ID);
          return unitType != null ? unitType.Count.ToString() : "0";

        case "Load Per Unit Type ((Total General + AC + Additional) x # of Units)":
          unitType = buildingInfo.Counters.FirstOrDefault(c => c.UnitID == unitInfo.ID);
          return unitType != null ? unitType.SubtotalLoad.ToString() + "VA" : "0VA";

        default:
          return TryGeneralCustomLoads(message, unitInfo);
      }
    }

    public static string GetValueFromBuildingInfo(string message, BuildingInformation buildingInfo)
    {
      switch (message)
      {
        case "Total Number Of Units":
          return buildingInfo.TotalNumberOfUnits().ToString();

        case "Total Building Load (Sum of Load Per Unit Types)":
          return buildingInfo.TotalBuildingLoad().ConvertToKVA().ToString("0.0") + "KVA";

        case "Demand Factor (CEC 220.84)":
          return buildingInfo.DemandFactor().ToString();

        case "Total Building Load with Demand Factor Applied":
          return buildingInfo.TotalBuildingLoadWithDemandFactor().ConvertToKVA().ToString("0.0") + "KVA";

        case "House Load":
          int houseLoad = (buildingInfo.HouseLoad ?? 0);
          return houseLoad.ConvertToKVA().ToString("0.0") + "KVA";

        case "Total Building Load with Demand Factor Applied & House Load":
          return buildingInfo.TotalBuildingLoadWithDemandFactorAndHouseLoad().ConvertToKVA().ToString("0.0") + "KVA";

        case "Total Amperage @120/208V 1 PH":
          return buildingInfo.TotalAmperage().ToString() + "A";

        case "Total Amperage @120/240V 1 PH":
          return buildingInfo.TotalAmperage().ToString() + "A";

        case "Total Amperage @120/208V 3 PH":
          return buildingInfo.TotalAmperage().ToString() + "A";

        case "Total Amperage @120/240V 3 PH":
          return buildingInfo.TotalAmperage().ToString() + "A";

        case "Recommended Service Size":
          return buildingInfo.RecommendedServiceSize().ToString() + "A";

        default:
          return "0VA";
      }
    }

    public static string GetValueFromCustomUnitInfo(string message, UnitInformation unitInfo)
    {
      foreach (var customLoad in unitInfo.CustomLoads)
      {
        if (customLoad.Name == message)
        {
          return customLoad.GetTotal().ToString() + "VA";
        }
      }

      return "0VA";
    }

    private static string TryGeneralCustomLoads(string message, UnitInformation unitInfo)
    {
      foreach (var generalCustomLoad in unitInfo.GeneralLoads.Customs)
      {
        if (generalCustomLoad.Name == message)
        {
          return generalCustomLoad.GetTotal().ToString() + "VA";
        }
      }

      return "0VA";
    }
  }
}