using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using GMEPElectricalResidential.HelperFiles;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GMEPElectricalResidential.LoadCalculations.Unit
{
  public class LoadCalculation
  {
    public static void CreateUnitLoadCalculationTable(UnitInformation unitInfo, Point3d placementPoint, bool placeTheBlocks = true)
    {
      double HEADER_HEIGHT = 0.75;
      double currentHeight = HEADER_HEIGHT;
      string newBlockName = $"Unit {unitInfo.Name}" + $" ID{unitInfo.ID}";

      var acCurDb = Application.DocumentManager.MdiActiveDocument.Database;

      using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
      {
        Point3d point = new Point3d(0, 0, 0);

        BlockTable acBlkTbl;
        acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead) as BlockTable;

        BlockTableRecord acBlkTblRec;

        var existingBlock = acBlkTbl.Cast<ObjectId>()
            .Select(id => acTrans.GetObject(id, OpenMode.ForRead) as BlockTableRecord)
            .FirstOrDefault(btr => btr.Name.Contains($"ID{unitInfo.ID}"));

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

        ObjectData headerData = GetCopyPasteData("UnitLoadCalculationHeader");
        ObjectData bodyData = GetCopyPasteData("UnitLoadCalculationBody");

        headerData = UpdateHeaderData(headerData, unitInfo);

        ObjectData dwellingBodyData = ShiftData(bodyData, -currentHeight);
        dwellingBodyData = UpdateDwellingData(dwellingBodyData, unitInfo);
        double dwellingSectionHeight = CreateUnitLoadCalculationRectangle(point, -currentHeight, dwellingBodyData.NumberOfRows, acBlkTblRec);

        currentHeight += dwellingSectionHeight;

        ObjectData generalBodyData = ShiftData(bodyData, -currentHeight);
        generalBodyData = UpdateGeneralData(generalBodyData, unitInfo);
        double generalSectionHeight = CreateUnitLoadCalculationRectangle(point, -currentHeight, generalBodyData.NumberOfRows, acBlkTblRec);

        currentHeight += generalSectionHeight;

        ObjectData generalBodyCalcData = ShiftData(bodyData, -currentHeight);
        generalBodyCalcData = UpdateGeneralCalculationData(generalBodyCalcData, unitInfo);
        double generalCalcSectionHeight = CreateUnitLoadCalculationRectangle(point, -currentHeight, generalBodyCalcData.NumberOfRows, acBlkTblRec);

        currentHeight += generalCalcSectionHeight;

        ObjectData airConditioningBodyData = ShiftData(bodyData, -currentHeight);
        airConditioningBodyData = UpdateAirConditioningData(airConditioningBodyData, unitInfo);
        double airConditioningSectionHeight = CreateUnitLoadCalculationRectangle(point, -currentHeight, airConditioningBodyData.NumberOfRows, acBlkTblRec);

        currentHeight += airConditioningSectionHeight;

        ObjectData customBodyData = ShiftData(bodyData, -currentHeight);
        customBodyData = UpdateCustomData(customBodyData, unitInfo);
        double customSectionHeight = CreateUnitLoadCalculationRectangle(point, -currentHeight, customBodyData.NumberOfRows, acBlkTblRec);

        currentHeight += customSectionHeight;

        ObjectData serviceBodyData = ShiftData(bodyData, -currentHeight);
        serviceBodyData = UpdateServiceData(serviceBodyData, unitInfo);
        double _ = CreateUnitLoadCalculationRectangle(point, -currentHeight, serviceBodyData.NumberOfRows, acBlkTblRec);

        string modifiedHeaderData = JsonConvert.SerializeObject(headerData);
        string modifiedDwellingBodyData = JsonConvert.SerializeObject(dwellingBodyData);
        string modifiedGeneralBodyData = JsonConvert.SerializeObject(generalBodyData);
        string modifiedGeneralBodyCalcData = JsonConvert.SerializeObject(generalBodyCalcData);
        string modifiedAirConditioningBodyData = JsonConvert.SerializeObject(airConditioningBodyData);
        string modifiedCustomBodyData = JsonConvert.SerializeObject(customBodyData);
        string modifiedServiceBodyData = JsonConvert.SerializeObject(serviceBodyData);

        CADObjectCommands.CreateObjectFromData(modifiedHeaderData, point, acBlkTblRec);
        CADObjectCommands.CreateObjectFromData(modifiedDwellingBodyData, point, acBlkTblRec);
        CADObjectCommands.CreateObjectFromData(modifiedGeneralBodyData, point, acBlkTblRec);
        CADObjectCommands.CreateObjectFromData(modifiedGeneralBodyCalcData, point, acBlkTblRec);
        CADObjectCommands.CreateObjectFromData(modifiedAirConditioningBodyData, point, acBlkTblRec);
        CADObjectCommands.CreateObjectFromData(modifiedCustomBodyData, point, acBlkTblRec);
        CADObjectCommands.CreateObjectFromData(modifiedServiceBodyData, point, acBlkTblRec);

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

    private static ObjectData UpdateHeaderData(ObjectData headerData, UnitInformation unitInfo)
    {
      var serviceLoadCalculationMText = headerData.MTexts.FirstOrDefault(mText => mText.Contents.Contains("SERVICE LOAD CALCULATION"));
      serviceLoadCalculationMText.Contents = serviceLoadCalculationMText.Contents.Replace("SERVICE LOAD CALCULATION", $"SERVICE LOAD CALCULATION - UNIT {unitInfo.Name}");
      return headerData;
    }

    private static ObjectData UpdateServiceData(ObjectData serviceBodyData, UnitInformation unitInfo)
    {
      int startingRows = 4;
      var headers = serviceBodyData.MTexts.FirstOrDefault(mText => mText.Contents.Contains("Title"));
      if (headers != null)
      {
        headers.Contents = "";
        string serviceSubtitles = "Calculated Load for Service:".Underline().NewLine();

        serviceSubtitles += $"({unitInfo.Totals.SubtotalGeneralLoad}VA+{unitInfo.Totals.TotalACLoad}VA+{unitInfo.Totals.CustomLoad}VA)/{unitInfo.Voltage}={unitInfo.Totals.ServiceLoad}A (Service Rating)".NewLine().NewLine();

        serviceSubtitles += "Provided Service Rating:";

        headers.Contents = serviceSubtitles.SetFont("Arial");
      }

      var values = serviceBodyData.MTexts.FirstOrDefault(mText => mText.Contents.Contains("Subtitle VA"));
      if (values != null)
      {
        values.Contents = "";
        string serviceValues = "".NewLine().NewLine().NewLine();

        serviceValues += $"{unitInfo.Totals.ServiceRating()}A";

        values.Contents = serviceValues.SetFont("Arial");
      }

      serviceBodyData.NumberOfRows = startingRows;

      return serviceBodyData;
    }

    private static ObjectData UpdateCustomData(ObjectData customBodyData, UnitInformation unitInfo)
    {
      int startingRows = 0;
      var headers = customBodyData.MTexts.FirstOrDefault(mText => mText.Contents.Contains("Title"));
      if (headers != null)
      {
        headers.Contents = "";
        string customSubtitles = "";

        if (unitInfo.CustomLoads.Count > 0)
        {
          customSubtitles = "Additional Load:".Underline().NewLine();
          startingRows++;
        }

        unitInfo.CustomLoads.ForEach(customLoad =>
        {
          customSubtitles += $"{customLoad.Name}{((customLoad.Multiplier <= 1) ? ":" : $" ({customLoad.Multiplier}):")}".NewLine();
          startingRows++;
        });

        headers.Contents = customSubtitles.SetFont("Arial");
      }

      var values = customBodyData.MTexts.FirstOrDefault(mText => mText.Contents.Contains("Subtitle VA"));
      if (values != null)
      {
        values.Contents = "";
        string customValues = "";

        if (unitInfo.CustomLoads.Count > 0)
        {
          customValues = "".NewLine();
        }

        unitInfo.CustomLoads.ForEach(customLoad =>
        {
          customValues += $"{customLoad.VA * customLoad.Multiplier}VA".NewLine();
        });

        values.Contents = customValues.SetFont("Arial");
      }

      customBodyData.NumberOfRows = startingRows;

      return customBodyData;
    }

    private static ObjectData UpdateAirConditioningData(ObjectData airConditioningBodyData, UnitInformation unitInfo)
    {
      int startingRows = 2;
      var headers = airConditioningBodyData.MTexts.FirstOrDefault(mText => mText.Contents.Contains("Title"));
      if (headers != null)
      {
        headers.Contents = "";
        string dwellingSubtitles = "AC Load:".Underline().NewLine();
        if (unitInfo.ACLoads.Condenser > 0)
        {
          dwellingSubtitles += "Outdoor Condensing Unit:".NewLine();
          startingRows++;
        }
        if (unitInfo.ACLoads.FanCoil > 0)
        {
          dwellingSubtitles += "Indoor Fan Coil Unit:".NewLine();
          startingRows++;
        }
        if (unitInfo.ACLoads.HeatingUnit.Heating > 0)
        {
          dwellingSubtitles += "Heating Unit" + $" {(unitInfo.ACLoads.HeatingUnit.NumberOfUnits > 1 ? $"({unitInfo.ACLoads.HeatingUnit.NumberOfUnits}):" : ":")}".NewLine();
          startingRows++;
        }

        dwellingSubtitles += $"Total AC Load (CEC {unitInfo.ACLoads.ElectricalCode}):".NewLine();

        headers.Contents = dwellingSubtitles.SetFont("Arial");
      }

      var values = airConditioningBodyData.MTexts.FirstOrDefault(mText => mText.Contents.Contains("Subtitle VA"));
      if (values != null)
      {
        values.Contents = "";
        string dwellingValues = "".NewLine();
        if (unitInfo.ACLoads.Condenser > 0)
        {
          dwellingValues += $"{unitInfo.ACLoads.Condenser}VA".NewLine();
        }
        if (unitInfo.ACLoads.FanCoil > 0)
        {
          dwellingValues += $"{unitInfo.ACLoads.FanCoil}VA".NewLine();
        }
        if (unitInfo.ACLoads.HeatingUnit.Heating > 0)
        {
          dwellingValues += $"{unitInfo.ACLoads.HeatingUnit.Heating}VA".NewLine();
        }

        dwellingValues += $"{unitInfo.Totals.TotalACLoad}VA".NewLine();

        values.Contents = dwellingValues.SetFont("Arial");
      }

      airConditioningBodyData.NumberOfRows = startingRows;

      return airConditioningBodyData;
    }

    private static ObjectData UpdateGeneralCalculationData(ObjectData generalBodyCalcData, UnitInformation unitInfo)
    {
      var headers = generalBodyCalcData.MTexts.FirstOrDefault(mText => mText.Contents.Contains("Title"));
      if (headers != null)
      {
        headers.Contents = "";
        string dwellingSubtitles = "Total General Load:".NewLine() +
                                   "First 10 KVA @ 100% (CEC  220.82(B)):".NewLine() +
                                   $"Remainder @ 40% ({unitInfo.Totals.AmountOver10KVA()}VA x 0.4) (CEC 220.82(B)):".NewLine() +
                                   "General Calculated Load (CEC  220.82(B)):".NewLine();

        headers.Contents = dwellingSubtitles.SetFont("Arial");
      }

      var values = generalBodyCalcData.MTexts.FirstOrDefault(mText => mText.Contents.Contains("Subtitle VA"));
      if (values != null)
      {
        values.Contents = "";
        string dwellingValues = $"{unitInfo.Totals.TotalGeneralLoad}VA".NewLine() +
                                $"{unitInfo.Totals.First10KVA()}VA".NewLine() +
                                $"{unitInfo.Totals.RemainderAt40Percent()}VA".NewLine() +
                                $"{unitInfo.Totals.SubtotalGeneralLoad}VA".NewLine();

        values.Contents = dwellingValues.SetFont("Arial");
      }

      generalBodyCalcData.NumberOfRows = 4;

      return generalBodyCalcData;
    }

    private static ObjectData UpdateGeneralData(ObjectData generalBodyData, UnitInformation unitInfo)
    {
      int startingRows = 15;
      List<string> contents;
      var headers = generalBodyData.MTexts.FirstOrDefault(mText => mText.Contents.Contains("Title"));
      if (headers != null)
      {
        contents = new List<string>
        {
            "%%uGeneral Load:",
            $"General Lighting (Floor Area x 3VA/ft²) (CEC {UnitGeneralLoadContainer.LightingCode}):",
            $"Small Appliance (3-20ACK by CEC 210.11){((unitInfo.GeneralLoads.SmallAppliance.Multiplier <= 1) ? ":" : $" ({unitInfo.GeneralLoads.SmallAppliance.Multiplier}):")}",
            $"Laundry (1-20ACKT by CEC 210.11){((unitInfo.GeneralLoads.Laundry.Multiplier <= 1) ? ":" : $" ({unitInfo.GeneralLoads.Laundry.Multiplier}):")}",
            $"Bathroom (1-20ACKT by CEC 210.11){((unitInfo.GeneralLoads.Bathroom.Multiplier <= 1) ? ":" : $" ({unitInfo.GeneralLoads.Bathroom.Multiplier}):")}",
            $"Dishwasher{((unitInfo.GeneralLoads.Dishwasher.Multiplier <= 1) ? ":" : $" ({unitInfo.GeneralLoads.Dishwasher.Multiplier}):")}",
            $"Microwave{((unitInfo.GeneralLoads.MicrowaveOven.Multiplier <= 1) ? ":" : $" ({unitInfo.GeneralLoads.MicrowaveOven.Multiplier}):")}",
            $"Garbage Disposal{((unitInfo.GeneralLoads.GarbageDisposal.Multiplier <= 1) ? ":" : $" ({unitInfo.GeneralLoads.GarbageDisposal.Multiplier}):")}",
            $"Bathroom Fans{((unitInfo.GeneralLoads.BathroomFans.Multiplier <= 1) ? ":" : $" ({unitInfo.GeneralLoads.BathroomFans.Multiplier}):")}",
            $"Garage Door Opener{((unitInfo.GeneralLoads.GarageDoorOpener.Multiplier <= 1) ? ":" : $" ({unitInfo.GeneralLoads.GarageDoorOpener.Multiplier}):")}",
            $"Dryer{((unitInfo.GeneralLoads.Dryer.Multiplier <= 1) ? ":" : $" ({unitInfo.GeneralLoads.Dryer.Multiplier}):")}",
            $"Range{((unitInfo.GeneralLoads.Range.Multiplier <= 1) ? ":" : $" ({unitInfo.GeneralLoads.Range.Multiplier}):")}",
            $"Refrigerator{((unitInfo.GeneralLoads.Refrigerator.Multiplier <= 1) ? ":" : $" ({unitInfo.GeneralLoads.Refrigerator.Multiplier}):")}",
            $"Oven{((unitInfo.GeneralLoads.Oven.Multiplier <= 1) ? ":" : $" ({unitInfo.GeneralLoads.Oven.Multiplier}):")}",
            $"Cooktop{((unitInfo.GeneralLoads.Cooktop.Multiplier <= 1) ? ":" : $" ({unitInfo.GeneralLoads.Cooktop.Multiplier}):")}"
        };

        var waterHeaterExtraRow = $"Water Heater{((unitInfo.GeneralLoads.WaterHeater.Multiplier <= 1) ? ":" : $" ({unitInfo.GeneralLoads.WaterHeater.Multiplier}):")}";

        if (!unitInfo.CustomLoads.Any(load => load.Name == "Water Heater"))
        {
          contents.Add(waterHeaterExtraRow);
          startingRows++;
        }

        unitInfo.GeneralLoads.Customs.ForEach(customLoad =>
        {
          contents.Add($"{customLoad.Name}{((customLoad.Multiplier <= 1) ? ":" : $" ({customLoad.Multiplier}):")}");
          startingRows++;
        });

        startingRows += InsertTitleLightingBreakdown(2, unitInfo, contents);

        AddTextObjectsToObjectData(generalBodyData, contents, headers, 0.25, 0.16);

        headers.Contents = "";
      }

      var values = generalBodyData.MTexts.FirstOrDefault(mText => mText.Contents.Contains("Subtitle VA"));
      if (values != null)
      {
        List<string> generalValues = new List<string>
        {
            "",
            $"{unitInfo.GeneralLoads.Lighting.VA * unitInfo.GeneralLoads.Lighting.Multiplier}VA",
            $"{unitInfo.GeneralLoads.SmallAppliance.VA * unitInfo.GeneralLoads.SmallAppliance.Multiplier}VA",
            $"{unitInfo.GeneralLoads.Laundry.VA * unitInfo.GeneralLoads.Laundry.Multiplier}VA",
            $"{unitInfo.GeneralLoads.Bathroom.VA * unitInfo.GeneralLoads.Bathroom.Multiplier}VA",
            $"{unitInfo.GeneralLoads.Dishwasher.VA * unitInfo.GeneralLoads.Dishwasher.Multiplier}VA",
            $"{unitInfo.GeneralLoads.MicrowaveOven.VA * unitInfo.GeneralLoads.MicrowaveOven.Multiplier}VA",
            $"{unitInfo.GeneralLoads.GarbageDisposal.VA * unitInfo.GeneralLoads.GarbageDisposal.Multiplier}VA",
            $"{unitInfo.GeneralLoads.BathroomFans.VA * unitInfo.GeneralLoads.BathroomFans.Multiplier}VA",
            $"{unitInfo.GeneralLoads.GarageDoorOpener.VA * unitInfo.GeneralLoads.GarageDoorOpener.Multiplier}VA",
            $"{unitInfo.GeneralLoads.Dryer.VA * unitInfo.GeneralLoads.Dryer.Multiplier}VA",
            $"{unitInfo.GeneralLoads.Range.VA * unitInfo.GeneralLoads.Range.Multiplier}VA",
            $"{unitInfo.GeneralLoads.Refrigerator.VA * unitInfo.GeneralLoads.Refrigerator.Multiplier}VA",
            $"{unitInfo.GeneralLoads.Oven.VA * unitInfo.GeneralLoads.Oven.Multiplier}VA",
            $"{unitInfo.GeneralLoads.Cooktop.VA * unitInfo.GeneralLoads.Cooktop.Multiplier}VA"
        };

        var waterHeaterExtraValue = $"{unitInfo.GeneralLoads.WaterHeater.VA * unitInfo.GeneralLoads.WaterHeater.Multiplier}VA";

        if (!unitInfo.CustomLoads.Any(load => load.Name == "Water Heater"))
        {
          generalValues.Add(waterHeaterExtraValue);
        }

        unitInfo.GeneralLoads.Customs.ForEach(customLoad =>
        {
          generalValues.Add($"{customLoad.VA}VA");
        });

        InsertValueLightingBreakdown(2, unitInfo, generalValues);

        AddTextObjectsToObjectData(generalBodyData, generalValues, values, 0.25, 0.16);

        values.Contents = "";
      }

      generalBodyData.NumberOfRows = startingRows;

      return generalBodyData;
    }

    private static void InsertValueLightingBreakdown(int index, UnitInformation unitInfo, List<string> generalValues)
    {
      int lightingVA = unitInfo.GeneralLoads.Lighting.VA * unitInfo.GeneralLoads.Lighting.Multiplier;

      if (unitInfo.GeneralLoads.LightingOccupancyType == LightingOccupancyType.Dwelling)
      {
        InsertValueLightingBreakdownDwelling(index, unitInfo, generalValues, lightingVA);
      }
      else if (unitInfo.GeneralLoads.LightingOccupancyType == LightingOccupancyType.HotelAndMotel)
      {
        InsertValueLightingBreakdownHotelMotel(index, unitInfo, generalValues, lightingVA);
      }
      else if (unitInfo.GeneralLoads.LightingOccupancyType == LightingOccupancyType.Warehouse)
      {
        InsertValueLightingBreakdownWarehouse(index, unitInfo, generalValues, lightingVA);
      }
    }

    private static void InsertValueLightingBreakdownDwelling(int index, UnitInformation unitInfo, List<string> generalValues, int lightingVA)
    {
      var firstValue = Math.Min(lightingVA, 3000);
      var secondValue = Math.Min(Math.Max(lightingVA - 3000, 0), 117000) * 0.35;
      var thirdValue = Math.Max(lightingVA - 120000, 0) * 0.25;

      var total = firstValue + secondValue + thirdValue;

      generalValues.Insert(index, $"{firstValue}VA");
      generalValues.Insert(index + 1, $"{secondValue}VA");
      generalValues.Insert(index + 2, $"{thirdValue}VA");
      generalValues.Insert(index + 3, $"{total}VA");
    }

    private static void InsertValueLightingBreakdownHotelMotel(int index, UnitInformation unitInfo, List<string> generalValues, int lightingVA)
    {
      var firstValue = Math.Min(lightingVA, 20000) * 0.6;
      var secondValue = Math.Min(Math.Max(lightingVA - 20000, 0), 80000) * 0.5;
      var thirdValue = Math.Max(lightingVA - 100000, 0) * 0.35;
      var total = firstValue + secondValue + thirdValue;

      generalValues.Insert(index, $"{firstValue}VA");
      generalValues.Insert(index + 1, $"{secondValue}VA");
      generalValues.Insert(index + 2, $"{thirdValue}VA");
      generalValues.Insert(index + 3, $"{total}VA");
    }

    private static void InsertValueLightingBreakdownWarehouse(int index, UnitInformation unitInfo, List<string> generalValues, int lightingVA)
    {
      var firstValue = Math.Min(lightingVA, 12500);
      var secondValue = Math.Max(lightingVA - 12500, 0) * 0.5;
      var total = firstValue + secondValue;

      generalValues.Insert(index, $"{firstValue}VA");
      generalValues.Insert(index + 1, $"{secondValue}VA");
      generalValues.Insert(index + 2, $"{total}VA");
    }

    private static int InsertTitleLightingBreakdown(int index, UnitInformation unitInfo, List<string> contents)
    {
      int additionalRows = 0;
      if (unitInfo.GeneralLoads.LightingOccupancyType == LightingOccupancyType.Dwelling)
      {
        additionalRows = InsertTitleLightingBreakdownDwelling(index, unitInfo, contents);
      }
      else if (unitInfo.GeneralLoads.LightingOccupancyType == LightingOccupancyType.HotelAndMotel)
      {
        additionalRows = InsertTitleLightingBreakdownHotelMotel(index, unitInfo, contents);
      }
      else if (unitInfo.GeneralLoads.LightingOccupancyType == LightingOccupancyType.Warehouse)
      {
        additionalRows = InsertTitleLightingBreakdownWarehouse(index, unitInfo, contents);
      }
      return additionalRows;
    }

    private static int InsertTitleLightingBreakdownDwelling(int index, UnitInformation unitInfo, List<string> contents)
    {
      contents.Insert(index, "   0-3kVA @ 100%:");
      contents.Insert(index + 1, "   3-120kVA @ 35%:");
      contents.Insert(index + 2, "   120+kVA @ 25%:");
      contents.Insert(index + 3, "   Lighting Subtotal:");

      return 4;
    }

    private static int InsertTitleLightingBreakdownHotelMotel(int index, UnitInformation unitInfo, List<string> contents)
    {
      contents.Insert(index, "   0-20kVA @ 60%:");
      contents.Insert(index + 1, "   20-100kVA @ 50%:");
      contents.Insert(index + 2, "   100+kVA @ 35%:");
      contents.Insert(index + 3, "   Lighting Subtotal:");

      return 4;
    }

    private static int InsertTitleLightingBreakdownWarehouse(int index, UnitInformation unitInfo, List<string> contents)
    {
      contents.Insert(index, "   0-12.5kVA @ 100%:");
      contents.Insert(index + 1, "   12.5+kVA @ 50%:");
      contents.Insert(index + 2, "   Lighting Subtotal:");

      return 3;
    }

    private static void AddTextObjectsToObjectData(ObjectData objectData, List<string> lines, MTextData mText, double spacing, double marginTop)
    {
      List<TextData> textData = new List<TextData>();
      for (int i = 0; i < lines.Count; i++)
      {
        TextData text = new TextData
        {
          Contents = lines[i],
          Location = new SimpleVector3d(mText.Location.X, mText.Location.Y - marginTop - (spacing * i), 0),
          Height = mText.TextHeight,
          Layer = mText.Layer,
          Rotation = mText.Rotation,
          Style = mText.Style,
          HorizontalMode = mText.Justification.Replace("Top", "")
        };
        textData.Add(text);
      }

      objectData.Texts.AddRange(textData);
    }

    private static ObjectData UpdateDwellingData(ObjectData dwellingBodyData, UnitInformation unitInfo)
    {
      var headers = dwellingBodyData.MTexts.FirstOrDefault(mText => mText.Contents.Contains("Title"));
      if (headers != null)
      {
        headers.Contents = "";
        string dwellingTitle = "Dwelling Information:".Underline().NewLine();
        string dwellingSubtitles = "Floor Area:".NewLine() +
                                   "Heater:".NewLine() +
                                   "Dryer:".NewLine() +
                                   "Oven:".NewLine() +
                                   "Cooktop:";
        string dwellingTitleAndSubtitles = dwellingTitle + dwellingSubtitles;
        headers.Contents = dwellingTitleAndSubtitles.SetFont("Arial");
      }

      var values = dwellingBodyData.MTexts.FirstOrDefault(mText => mText.Contents.Contains("Subtitle VA"));
      if (values != null)
      {
        values.Contents = "";
        string dwellingValues = "".NewLine() +
                                $"{unitInfo.DwellingArea.FloorArea}ft\u00B2".NewLine() +
                                $"{unitInfo.DwellingArea.Heater}".NewLine() +
                                $"{unitInfo.DwellingArea.Dryer}".NewLine() +
                                $"{unitInfo.DwellingArea.Oven}".NewLine() +
                                $"{unitInfo.DwellingArea.Cooktop}";
        values.Contents = dwellingValues.SetFont("Arial");
      }

      dwellingBodyData.NumberOfRows = 6;

      return dwellingBodyData;
    }

    private static double CreateUnitLoadCalculationRectangle(Point3d point, double shiftY, int numberOfRows, BlockTableRecord block)
    {
      if (numberOfRows == 0)
      {
        return 0;
      }

      double MARGIN_TOP = 0.16;
      double MARGIN_BOT = 0.08;
      double ROW_HEIGHT = 0.25;
      double WIDTH = 7.0;

      Point3d topRight = new Point3d(point.X, point.Y + shiftY, point.Z);
      Point3d topLeft = new Point3d(point.X - WIDTH, point.Y + shiftY, point.Z);

      double height = MARGIN_TOP + MARGIN_BOT + (ROW_HEIGHT * numberOfRows);

      Point3d bottomLeft = new Point3d(point.X - WIDTH, point.Y + shiftY - height, point.Z);
      Point3d bottomRight = new Point3d(point.X, point.Y + shiftY - height, point.Z);

      List<Point3d> points = new List<Point3d> { topRight, topLeft, bottomLeft, bottomRight };

      CreateClosedPolylineGivenPoints(points, block);

      return height;
    }

    private static void CreateClosedPolylineGivenPoints(List<Point3d> points, BlockTableRecord block)
    {
      Document acDoc = Application.DocumentManager.MdiActiveDocument;
      Database acCurDb = acDoc.Database;

      using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
      {
        BlockTableRecord acBlkTblRec = block;

        Polyline acPoly = new Polyline();
        for (int i = 0; i < points.Count; i++)
        {
          acPoly.AddVertexAt(i, new Point2d(points[i].X, points[i].Y), 0, 0, 0);
        }

        acPoly.Closed = true;
        acPoly.Layer = "E-TEXT";

        acBlkTblRec.AppendEntity(acPoly);
        acTrans.AddNewlyCreatedDBObject(acPoly, true);

        acTrans.Commit();
      }
    }

    private static ObjectData ShiftData(ObjectData bodyData, double shiftHeight)
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

    private static ObjectData GetCopyPasteData(string fileName)
    {
      string assemblyLocation = Assembly.GetExecutingAssembly().Location;
      string assemblyDirectory = Path.GetDirectoryName(assemblyLocation);
      string relativeFilePath = Path.Combine(assemblyDirectory, $"LoadCalculations\\Unit\\BlockData\\{fileName}.json");

      string jsonData = File.ReadAllText(relativeFilePath);
      ObjectData objectData = JsonConvert.DeserializeObject<ObjectData>(jsonData);
      return objectData;
    }
  }

  public static class StringExtensions
  {
    public static string fontName = "Arial";

    public static string Underline(this string text)
    {
      return "\\L" + text + "\\l";
    }

    public static string MakeBold(this string text)
    {
      return "{\\b1;" + text + "}";
    }

    public static string NewLine(this string text)
    {
      return text + "\\P";
    }

    public static string SetFont(this string text, string fontName)
    {
      return $"{{\\F{fontName};" + text + "}";
    }
  }
}