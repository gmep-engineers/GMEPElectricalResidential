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
      string newBlockName = unitInfo.FilteredFormattedName();

      if (unitInfo == null)
      {
        return;
      }

      if (unitInfo.Name == null)
      {
        unitInfo.Name = "";
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
            .FirstOrDefault(btr => btr.Name == newBlockName);

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
      serviceLoadCalculationMText.Contents = serviceLoadCalculationMText.Contents.Replace("\\Farial|c0", "\\fArial Rounded MT Bold|b1|i0|c0|p34");
      return headerData;
    }

    private static ObjectData UpdateDwellingData(ObjectData dwellingBodyData, UnitInformation unitInfo)
    {
      var headers = dwellingBodyData.MTexts.FirstOrDefault(mText => mText.Contents.Contains("Title"));

      if (headers != null)
      {
        headers.Contents = "";
        string dwellingTitle = "Dwelling Information:".Underline().BoldItalic().NewLine();
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
        int area1 = int.Parse(unitInfo.DwellingArea.FloorArea);
        string heater = unitInfo.DwellingArea.Heater.ToString();
        string dryer = unitInfo.DwellingArea.Dryer.ToString();
        string oven = unitInfo.DwellingArea.Oven.ToString();
        string cooktop = unitInfo.DwellingArea.Cooktop.ToString();

        values.Contents = "";
        string dwellingValues = "".NewLine() +
                                $"{area1}ft\u00B2".NewLine() +
                                $"{heater}".NewLine() +
                                $"{dryer}".NewLine() +
                                $"{oven}".NewLine() +
                                $"{cooktop}";
        values.Contents = dwellingValues.SetFont("Arial");
      }

      dwellingBodyData.NumberOfRows = 6;

      return dwellingBodyData;
    }

    private static ObjectData UpdateServiceData(ObjectData serviceBodyData, UnitInformation unitInfo)
    {
      int startingRows = 4;
      int totalServiceRating = unitInfo.Totals.ServiceLoad;
      var combinedUnitTotals = new UnitTotalContainer();
      combinedUnitTotals.ServiceLoad = totalServiceRating;
      var headers = serviceBodyData.MTexts.FirstOrDefault(mText => mText.Contents.Contains("Title"));
      if (headers != null)
      {
        headers.Contents = "";
        string serviceSubtitles = "Calculated Load for Service:".Underline().BoldItalic().NewLine();

        int totalSubtotalGeneralLoad = unitInfo.Totals.SubtotalGeneralLoad;
        int totalTotalACLoad = unitInfo.Totals.TotalACLoad;
        int totalCustomLoad = unitInfo.Totals.CustomLoad;

        serviceSubtitles += $"({totalSubtotalGeneralLoad}VA+{totalTotalACLoad}VA+{totalCustomLoad}VA)/{unitInfo.Voltage}={totalServiceRating}A (Service Rating)".NewLine().NewLine();

        serviceSubtitles += "Provided Service Rating:";

        headers.Contents = serviceSubtitles.SetFont("Arial");
      }

      var values = serviceBodyData.MTexts.FirstOrDefault(mText => mText.Contents.Contains("Subtitle VA"));
      if (values != null)
      {
        values.Contents = "";
        string serviceValues = "".NewLine().NewLine().NewLine();

        serviceValues += $"{combinedUnitTotals.ServiceRating()}A";

        values.Contents = serviceValues.SetFont("Arial");
      }

      serviceBodyData.NumberOfRows = startingRows;

      return serviceBodyData;
    }

    private static ObjectData UpdateCustomData(ObjectData customBodyData, UnitInformation unitInfo)
    {
      ObjectData customBodyDataCopy = Newtonsoft.Json.JsonConvert.DeserializeObject<ObjectData>(Newtonsoft.Json.JsonConvert.SerializeObject(customBodyData));

      int startingRows = 0;
      var headers = customBodyDataCopy.MTexts.FirstOrDefault(mText => mText.Contents.Contains("Title"));
      if (headers != null)
      {
        headers.Contents = "";
        string customSubtitles = "";

        var combinedCustomLoads = new Dictionary<string, UnitLoad>();
        foreach (var customLoad in unitInfo.CustomLoads)
        {
          combinedCustomLoads[customLoad.Name] = new UnitLoad(customLoad.Name, customLoad.Total.ToString(), customLoad.Multiplier.ToString(), customLoad.IsCookingAppliance);
        }

        if (combinedCustomLoads.Count > 0)
        {
          customSubtitles = "Additional Load:".Underline().BoldItalic().NewLine();
          startingRows++;
        }

        foreach (var customLoad in combinedCustomLoads.Values)
        {
          customSubtitles += $"{customLoad.Name}{((customLoad.Multiplier <= 1) ? ":" : $" ({customLoad.Multiplier}):")}".NewLine();
          startingRows++;
        }

        headers.Contents = customSubtitles.SetFont("Arial");

        var values = customBodyDataCopy.MTexts.FirstOrDefault(mText => mText.Contents.Contains("Subtitle VA"));
        if (values != null)
        {
          values.Contents = "";
          string customValues = "";

          if (combinedCustomLoads.Count > 0)
          {
            customValues = "".NewLine();
          }

          foreach (var customLoad in combinedCustomLoads.Values)
          {
            customValues += $"{customLoad.GetTotal()}VA".NewLine();
          }

          values.Contents = customValues.SetFont("Arial");
        }
      }

      customBodyDataCopy.NumberOfRows = startingRows;

      return customBodyDataCopy;
    }

    private static ObjectData UpdateAirConditioningData(ObjectData airConditioningBodyData, UnitInformation unitInfo)
    {
      int startingRows = 2;
      var headers = airConditioningBodyData.MTexts.FirstOrDefault(mText => mText.Contents.Contains("Title"));
      if (headers != null)
      {
        headers.Contents = "";
        string dwellingSubtitles = "AC Load:".Underline().BoldItalic().NewLine();
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
        var unitTotal = new UnitTotalContainer();
        unitTotal.TotalGeneralLoad = unitInfo.Totals.TotalGeneralLoad;
        unitTotal.SubtotalGeneralLoad = unitInfo.Totals.SubtotalGeneralLoad;
        values.Contents = "";
        string dwellingValues = $"{unitTotal.TotalGeneralLoad}VA".NewLine() +
                                $"{unitTotal.First10KVA()}VA".NewLine() +
                                $"{unitTotal.RemainderAt40Percent()}VA".NewLine() +
                                $"{unitTotal.SubtotalGeneralLoad}VA".NewLine();

        values.Contents = dwellingValues.SetFont("Arial");
      }

      generalBodyCalcData.NumberOfRows = 4;

      return generalBodyCalcData;
    }

    private static ObjectData UpdateGeneralData(ObjectData generalBodyData, UnitInformation unitInfo)
    {
      int startingRows = 2;
      List<string> contents;
      var mTextObj = generalBodyData.MTexts.FirstOrDefault(mText => mText.Contents.Contains("Title"));
      if (mTextObj != null)
      {
        contents = new List<string>
        {
            $"General Lighting (Floor Area x 3VA/ft²) (CEC {UnitGeneralLoadContainer.LightingCode}):",
        };

        unitInfo.GeneralLoads.Customs.ForEach(customLoad =>
        {
          if (!customLoad.IsCookingAppliance)
          {
            contents.Add($"{customLoad.Name}{((customLoad.Multiplier <= 1) ? ":" : $" ({customLoad.Multiplier}):")}");
            startingRows++;
          }
        });

        startingRows += InsertTitleLightingBreakdown(1, unitInfo, contents);
        startingRows += InsertTitleCookingApplianceBreakdown(unitInfo, contents);

        AddTextObjectsToObjectData(generalBodyData, contents, mTextObj, 0.25, 0.16);

        mTextObj.Contents = "General Load:".Underline().BoldItalic();
      }

      var values = generalBodyData.MTexts.FirstOrDefault(mText => mText.Contents.Contains("Subtitle VA"));
      if (values != null)
      {
        List<string> generalValues = new List<string>
        {
            $"{unitInfo.GeneralLoads.Lighting.GetTotal()}VA",
        };

        unitInfo.GeneralLoads.Customs.ForEach(customLoad =>
        {
          if (!customLoad.IsCookingAppliance)
          {
            generalValues.Add($"{customLoad.GetTotal()}VA");
          }
        });

        InsertValueLightingBreakdown(1, unitInfo, generalValues);
        InsertValueCookingApplianceBreakdown(unitInfo, generalValues);

        AddTextObjectsToObjectData(generalBodyData, generalValues, values, 0.25, 0.16);

        values.Contents = "";
      }

      generalBodyData.NumberOfRows = startingRows;

      return generalBodyData;
    }

    private static void InsertValueCookingApplianceBreakdown(UnitInformation unitInfo, List<string> generalValues)
    {
      var cookingAppInfo = unitInfo.GeneralLoads.GetCookingApplianceInfo();
      var numberOfApps = cookingAppInfo.NumberOfCookingAppliancesUnder8750 + cookingAppInfo.NumberOfCookingAppliancesOver8750;
      if (numberOfApps == 0) return;

      if (numberOfApps == 1)
      {
        generalValues.Add($"{cookingAppInfo.CookingAppliances[0].GetTotal()}VA");
      }
      else
      {
        var appliancesUnder8750 = cookingAppInfo.CookingAppliancesUnder8750;
        var appliancesOver8750 = cookingAppInfo.CookingAppliancesOver8750;

        generalValues.Add("");

        if (cookingAppInfo.NumberOfCookingAppliancesUnder8750 >= 1 && cookingAppInfo.NumberOfCookingAppliancesOver8750 >= 1)
        {
          for (int i = 0; i < appliancesUnder8750.Count; i++)
          {
            generalValues.Add("");
          }
          for (int i = 0; i < appliancesOver8750.Count; i++)
          {
            generalValues.Add("");
          }
        }
        else if (cookingAppInfo.NumberOfCookingAppliancesUnder8750 >= 1)
        {
          for (int i = 0; i < appliancesUnder8750.Count; i++)
          {
            generalValues.Add("");
          }
        }
        else if (cookingAppInfo.NumberOfCookingAppliancesOver8750 >= 1)
        {
          for (int i = 0; i < appliancesOver8750.Count; i++)
          {
            generalValues.Add("");
          }
        }

        generalValues.Add($"{cookingAppInfo.TotalDemand}VA");
      }
    }

    private static int InsertTitleCookingApplianceBreakdown(UnitInformation unitInfo, List<string> contents)
    {
      var cookingAppInfo = unitInfo.GeneralLoads.GetCookingApplianceInfo();
      var numberOfApps = cookingAppInfo.NumberOfCookingAppliancesUnder8750 + cookingAppInfo.NumberOfCookingAppliancesOver8750;
      if (numberOfApps == 0) return 0;
      if (numberOfApps == 1)
      {
        var appliances = cookingAppInfo.CookingAppliances;
        var customLoad = appliances[0];
        contents.Add($"{customLoad.Name}{((customLoad.Multiplier <= 1) ? ":" : $" ({customLoad.Multiplier}):")}");
        return 1;
      }

      var appliancesUnder8750 = cookingAppInfo.CookingAppliancesUnder8750;
      var appliancesOver8750 = cookingAppInfo.CookingAppliancesOver8750;
      var demandFactorsUnder8750 = new double[2];
      demandFactorsUnder8750[0] = cookingAppInfo.DemandFactor1750to3500;
      demandFactorsUnder8750[1] = cookingAppInfo.DemandFactor3500to8750;

      int additionalRows = 1;
      string title = "Cooking Appliances (CEC Table 220.55)";
      contents.Add(title);

      if (cookingAppInfo.NumberOfCookingAppliancesUnder8750 >= 1 && cookingAppInfo.NumberOfCookingAppliancesOver8750 >= 1)
      {
        additionalRows = UpdateContentsWithAppliances(contents, appliancesUnder8750, additionalRows, demandFactorsUnder8750);
        additionalRows = UpdateContentsWithAppliances(contents, appliancesOver8750, additionalRows);
      }
      else if (cookingAppInfo.NumberOfCookingAppliancesUnder8750 >= 1)
      {
        additionalRows = UpdateContentsWithAppliances(contents, appliancesUnder8750, additionalRows, demandFactorsUnder8750);
      }
      else if (cookingAppInfo.NumberOfCookingAppliancesOver8750 >= 1)
      {
        additionalRows = UpdateContentsWithAppliances(contents, appliancesOver8750, additionalRows);
      }

      contents.Add("   Cooking Appliance Subtotal:");
      additionalRows++;

      return additionalRows;
    }

    private static int UpdateContentsWithAppliances(List<string> contents, List<UnitLoad> appliances, int additionalRows, double[] demandFactors = null)
    {
      appliances.ForEach(customLoad =>
      {
        customLoad.GetIndividual();
        if (demandFactors != null)
        {
          int load = customLoad.GetIndividual();
          double demandFactor = (load > 3500) ? demandFactors[1] : demandFactors[0];
          contents.Add($"   {customLoad.Name} {load}VA @ {demandFactor}% with Demand Applied ({customLoad.Multiplier})");
        }
        else
        {
          int load = customLoad.GetIndividual();
          contents.Add($"   {customLoad.Name} {load}VA with Demand Applied ({customLoad.Multiplier})");
        }
        additionalRows++;
      });
      return additionalRows;
    }

    private static void InsertValueLightingBreakdown(int index, UnitInformation unitInfo, List<string> generalValues)
    {
      int lightingVA = unitInfo.GeneralLoads.Lighting.GetTotal();

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
      var secondValue = (int)Math.Ceiling(Math.Min(Math.Max(lightingVA - 3000, 0), 117000) * 0.35);
      var thirdValue = (int)Math.Ceiling(Math.Max(lightingVA - 120000, 0) * 0.25);

      var total = firstValue + secondValue + thirdValue;

      generalValues.Insert(index, $"{firstValue}VA");
      generalValues.Insert(index + 1, $"{secondValue}VA");
      generalValues.Insert(index + 2, $"{thirdValue}VA");
      generalValues.Insert(index + 3, $"{total}VA");
    }

    private static void InsertValueLightingBreakdownHotelMotel(int index, UnitInformation unitInfo, List<string> generalValues, int lightingVA)
    {
      var firstValue = (int)Math.Ceiling(Math.Min(lightingVA, 20000) * 0.6);
      var secondValue = (int)Math.Ceiling(Math.Min(Math.Max(lightingVA - 20000, 0), 80000) * 0.5);
      var thirdValue = (int)Math.Ceiling(Math.Max(lightingVA - 100000, 0) * 0.35);
      var total = firstValue + secondValue + thirdValue;

      generalValues.Insert(index, $"{firstValue}VA");
      generalValues.Insert(index + 1, $"{secondValue}VA");
      generalValues.Insert(index + 2, $"{thirdValue}VA");
      generalValues.Insert(index + 3, $"{total}VA");
    }

    private static void InsertValueLightingBreakdownWarehouse(int index, UnitInformation unitInfo, List<string> generalValues, int lightingVA)
    {
      var firstValue = Math.Min(lightingVA, 12500);
      var secondValue = (int)Math.Ceiling(Math.Max(lightingVA - 12500, 0) * 0.5);
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
      contents.Insert(index, "   0-3KVA @ 100%:");
      contents.Insert(index + 1, "   3-120KVA @ 35%:");
      contents.Insert(index + 2, "   120+KVA @ 25%:");
      contents.Insert(index + 3, "   General Lighting Subtotal:");

      return 4;
    }

    private static int InsertTitleLightingBreakdownHotelMotel(int index, UnitInformation unitInfo, List<string> contents)
    {
      contents.Insert(index, "   0-20KVA @ 60%:");
      contents.Insert(index + 1, "   20-100KVA @ 50%:");
      contents.Insert(index + 2, "   100+KVA @ 35%:");
      contents.Insert(index + 3, "   Lighting Subtotal:");

      return 4;
    }

    private static int InsertTitleLightingBreakdownWarehouse(int index, UnitInformation unitInfo, List<string> contents)
    {
      contents.Insert(index, "   0-12.5KVA @ 100%:");
      contents.Insert(index + 1, "   12.5+KVA @ 50%:");
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
          Location = new SimpleVector3d(mText.Location.X, mText.Location.Y - marginTop - ((spacing * i) + 0.25), 0),
          Height = mText.TextHeight,
          Layer = mText.Layer,
          Rotation = mText.Rotation,
          Style = mText.Style,
          HorizontalMode = (mText.Justification.Contains("Left") ? TextHorizontalMode.TextLeft : TextHorizontalMode.TextRight),
        };
        textData.Add(text);
      }

      objectData.Texts.AddRange(textData);
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

    public static string BoldItalic(this string text)
    {
      return "{\\fArial Rounded MT Bold|b1|i1|c0|p34;" + text + "}";
    }

    public static string Bold(this string text)
    {
      return "{\\fArial Rounded MT Bold|b1|i0|c0|p34;" + text + "}";
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