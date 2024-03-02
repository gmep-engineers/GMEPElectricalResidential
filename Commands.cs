using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
        _loadCalculationForm = new LoadCalculationForm(this);
      }

      _loadCalculationForm.Show();
      _loadCalculationForm.BringToFront();
    }

    [CommandMethod("GetObjectData")]
    public static void GetObjectData()
    {
      var data = new ObjectData();

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
              else if (obj is Autodesk.AutoCAD.DatabaseServices.DBText)
              {
                data = HandleText(obj as Autodesk.AutoCAD.DatabaseServices.DBText, data, origin);
              }

              transaction.Commit();
            }
          }
        }
      }

      SaveDataToJsonFile(data, "data.json");
    }

    public static void CreateObjectFromData(string jsonData, Point3d basePoint, BlockTableRecord block)
    {
      ObjectData objectData = JsonConvert.DeserializeObject<ObjectData>(jsonData);

      Document acDoc = Application.DocumentManager.MdiActiveDocument;
      Database acCurDb = acDoc.Database;

      using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
      {
        BlockTableRecord acBlkTblRec = block;

        foreach (var polyline in objectData.Polylines)
        {
          basePoint = CreatePolyline(basePoint, acTrans, acBlkTblRec, polyline);
        }

        foreach (var line in objectData.Lines)
        {
          basePoint = CreateLine(basePoint, acTrans, acBlkTblRec, line);
        }

        foreach (var arc in objectData.Arcs)
        {
          basePoint = CreateArc(basePoint, acTrans, acBlkTblRec, arc);
        }

        foreach (var circle in objectData.Circles)
        {
          basePoint = CreateCircle(basePoint, acTrans, acBlkTblRec, circle);
        }

        foreach (var ellipse in objectData.Ellipses)
        {
          basePoint = CreateEllipse(basePoint, acTrans, acBlkTblRec, ellipse);
        }

        foreach (var mText in objectData.MTexts)
        {
          basePoint = CreateMText(basePoint, acTrans, acBlkTblRec, mText);
        }

        foreach (var text in objectData.Texts)
        {
          basePoint = CreateText(basePoint, acTrans, acBlkTblRec, text);
        }

        foreach (var solid in objectData.Solids)
        {
          basePoint = CreateSolid(basePoint, acTrans, acBlkTblRec, solid);
        }

        acTrans.Commit();
      }
    }

    public static void CreateUnitLoadCalculationTable(UnitInformation unitInfo, Point3d placementPoint)
    {
      double HEADER_HEIGHT = 0.75;
      double currentHeight = HEADER_HEIGHT;
      string blockName = $"Unit {unitInfo.Name}" + $" ID{unitInfo.ID}"; ;

      var acCurDb = Application.DocumentManager.MdiActiveDocument.Database;

      using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
      {
        Point3d point = new Point3d(0, 0, 0);

        BlockTable acBlkTbl;
        acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead) as BlockTable;

        BlockTableRecord acBlkTblRec;

        if (acBlkTbl.Has(blockName))
        {
          acBlkTblRec = acTrans.GetObject(acBlkTbl[blockName], OpenMode.ForWrite) as BlockTableRecord;
          WipeExistingBlockContent(acBlkTblRec);
        }
        else
        {
          acBlkTbl.UpgradeOpen();
          acBlkTblRec = new BlockTableRecord();
          acBlkTblRec.Name = blockName;
          acBlkTbl.Add(acBlkTblRec);
          acTrans.AddNewlyCreatedDBObject(acBlkTblRec, true);
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

        CreateObjectFromData(modifiedHeaderData, point, acBlkTblRec);
        CreateObjectFromData(modifiedDwellingBodyData, point, acBlkTblRec);
        CreateObjectFromData(modifiedGeneralBodyData, point, acBlkTblRec);
        CreateObjectFromData(modifiedGeneralBodyCalcData, point, acBlkTblRec);
        CreateObjectFromData(modifiedAirConditioningBodyData, point, acBlkTblRec);
        CreateObjectFromData(modifiedCustomBodyData, point, acBlkTblRec);
        CreateObjectFromData(modifiedServiceBodyData, point, acBlkTblRec);

        acTrans.Commit();
      }

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

        if (acBlkTbl.Has(blockName))
        {
          BlockReference acBlkRef = new BlockReference(placementPoint, acBlkTbl[blockName]);

          acBlkTblRec.AppendEntity(acBlkRef);

          acTrans.AddNewlyCreatedDBObject(acBlkRef, true);
        }

        UpdateAllBlockReferences(blockName);

        acTrans.Commit();
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

        unitInfo.CustomLoads.ForEach(customLoad =>
        {
          customValues += $"{customLoad.VA}VA".NewLine();
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
        string dwellingSubtitles = "Air Conditioning Calculation:".Underline().NewLine();
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
                                   "First 10 KVA at 100%:".NewLine() +
                                   $"Remainder at 40% ({unitInfo.Totals.AmountOver10KVA()}VA x 0.4):".NewLine() +
                                   "Subtotal General Load:".NewLine();

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
      int startingRows = 16;
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
            $"Microwave Oven{((unitInfo.GeneralLoads.MicrowaveOven.Multiplier <= 1) ? ":" : $" ({unitInfo.GeneralLoads.MicrowaveOven.Multiplier}):")}",
            $"Garbage Disposal{((unitInfo.GeneralLoads.GarbageDisposal.Multiplier <= 1) ? ":" : $" ({unitInfo.GeneralLoads.GarbageDisposal.Multiplier}):")}",
            $"Bathroom Fans{((unitInfo.GeneralLoads.BathroomFans.Multiplier <= 1) ? ":" : $" ({unitInfo.GeneralLoads.BathroomFans.Multiplier}):")}",
            $"Garage Door Opener{((unitInfo.GeneralLoads.GarageDoorOpener.Multiplier <= 1) ? ":" : $" ({unitInfo.GeneralLoads.GarageDoorOpener.Multiplier}):")}",
            $"Dryer{((unitInfo.GeneralLoads.Dryer.Multiplier <= 1) ? ":" : $" ({unitInfo.GeneralLoads.Dryer.Multiplier}):")}",
            $"Range{((unitInfo.GeneralLoads.Range.Multiplier <= 1) ? ":" : $" ({unitInfo.GeneralLoads.Range.Multiplier}):")}",
            $"Refrigerator{((unitInfo.GeneralLoads.Refrigerator.Multiplier <= 1) ? ":" : $" ({unitInfo.GeneralLoads.Refrigerator.Multiplier}):")}",
            $"Oven{((unitInfo.GeneralLoads.Oven.Multiplier <= 1) ? ":" : $" ({unitInfo.GeneralLoads.Oven.Multiplier}):")}",
            $"Water Heater{((unitInfo.GeneralLoads.WaterHeater.Multiplier <= 1) ? ":" : $" ({unitInfo.GeneralLoads.WaterHeater.Multiplier}):")}",
            $"Cooktop{((unitInfo.GeneralLoads.Cooktop.Multiplier <= 1) ? ":" : $" ({unitInfo.GeneralLoads.Cooktop.Multiplier}):")}"
        };

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
            $"{unitInfo.GeneralLoads.WaterHeater.VA * unitInfo.GeneralLoads.WaterHeater.Multiplier}VA",
            $"{unitInfo.GeneralLoads.Cooktop.VA * unitInfo.GeneralLoads.Cooktop.Multiplier}VA"
        };

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

      if (unitInfo.GeneralLoads.LightingOccupancyType == "Dwelling")
      {
        InsertValueLightingBreakdownDwelling(index, unitInfo, generalValues, lightingVA);
      }
      else if (unitInfo.GeneralLoads.LightingOccupancyType == "Hotel and Motel")
      {
        InsertValueLightingBreakdownHotelMotel(index, unitInfo, generalValues, lightingVA);
      }
      else if (unitInfo.GeneralLoads.LightingOccupancyType == "Warehouse")
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
      if (unitInfo.GeneralLoads.LightingOccupancyType == "Dwelling")
      {
        additionalRows = InsertTitleLightingBreakdownDwelling(index, unitInfo, contents);
      }
      else if (unitInfo.GeneralLoads.LightingOccupancyType == "Hotel and Motel")
      {
        additionalRows = InsertTitleLightingBreakdownHotelMotel(index, unitInfo, contents);
      }
      else if (unitInfo.GeneralLoads.LightingOccupancyType == "Warehouse")
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
      string relativeFilePath = $"LoadCalculations\\CopyPaste\\{fileName}.json";
      string jsonData = File.ReadAllText(relativeFilePath);
      ObjectData objectData = JsonConvert.DeserializeObject<ObjectData>(jsonData);
      return objectData;
    }

    private static Point3d CreateText(Point3d basePoint, Transaction acTrans, BlockTableRecord acBlkTblRec, TextData text)
    {
      DBText dbText = new DBText();
      var textStyleObject = new TextStyle(0.0, 1.0, "Arial.ttf");
      textStyleObject.CreateStyleIfNotExisting("Load Calcs");

      Document acDoc = Application.DocumentManager.MdiActiveDocument;
      Database acCurDb = acDoc.Database;

      using (Transaction trans = acCurDb.TransactionManager.StartTransaction())
      {
        dbText.Layer = text.Layer;
        dbText.TextString = text.Contents;
        dbText.Height = text.Height;
        dbText.Rotation = text.Rotation;
        SetTextStyleByName(dbText, "Load Calcs");

        dbText.HorizontalMode = (text.HorizontalMode != "Left") ? TextHorizontalMode.TextRight : TextHorizontalMode.TextLeft;

        dbText.Position = new Point3d(basePoint.X + text.Location.X, basePoint.Y + text.Location.Y, basePoint.Z + text.Location.Z);

        if (dbText.HorizontalMode == TextHorizontalMode.TextRight)
        {
          dbText.AlignmentPoint = dbText.Position;
        }

        acBlkTblRec.AppendEntity(dbText);
        trans.AddNewlyCreatedDBObject(dbText, true);

        trans.Commit();
      }

      return basePoint;
    }

    private static void SetTextStyleByName(Entity textEntity, string styleName)
    {
      if (!(textEntity is MText || textEntity is DBText))
      {
        throw new ArgumentException("The textEntity must be of type MText or DBText.");
      }

      Database db = HostApplicationServices.WorkingDatabase;
      using (Transaction tr = db.TransactionManager.StartTransaction())
      {
        TextStyleTable textStyleTable = tr.GetObject(db.TextStyleTableId, OpenMode.ForRead) as TextStyleTable;
        if (textStyleTable.Has(styleName))
        {
          TextStyleTableRecord textStyle = tr.GetObject(textStyleTable[styleName], OpenMode.ForRead) as TextStyleTableRecord;
          if (textEntity is MText mTextEntity)
          {
            mTextEntity.TextStyleId = textStyle.ObjectId;
          }
          else if (textEntity is DBText dbTextEntity)
          {
            dbTextEntity.TextStyleId = textStyle.ObjectId;
          }
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
      SetTextStyleByName(mText, mTextData.Style);
      mText.Attachment = (AttachmentPoint)Enum.Parse(typeof(AttachmentPoint), mTextData.Justification);
      mText.Contents = mTextData.Contents;
      mText.Location = new Point3d(basePoint.X + mTextData.Location.X, basePoint.Y + mTextData.Location.Y, basePoint.Z + mTextData.Location.Z);
      //mText.LineSpacingStyle = mTextData.LineSpacingStyle;
      //mText.LineSpacingFactor = mTextData.LineSpaceFactor;
      mText.TextHeight = mTextData.TextHeight;
      mText.Width = mTextData.Width;
      //mText.Rotation = mTextData.Rotation;
      //mText.Direction = mTextData.Direction;

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

    private static ObjectData HandlePolyline(Polyline polyline, ObjectData data, Point3d origin)
    {
      var polylineData = new PolylineData
      {
        Layer = polyline.Layer,
        Vectors = new List<SimpleVector3d>(),
        LineType = polyline.Linetype,
        Closed = polyline.Closed,
      };

      for (int i = 0; i < polyline.NumberOfVertices; i++)
      {
        Point3d point = polyline.GetPoint3dAt(i);
        Vector3d vector = point - origin;
        polylineData.Vectors.Add(new SimpleVector3d { X = vector.X, Y = vector.Y, Z = vector.Z });
      }

      data.Polylines.Add(polylineData);

      return data;
    }

    private static ObjectData HandleArc(Arc arc, ObjectData data, Point3d origin)
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
        EndAngle = arc.EndAngle,
      };

      data.Arcs.Add(arcData);

      return data;
    }

    private static ObjectData HandleCircle(Circle circle, ObjectData data, Point3d origin)
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
        Radius = circle.Radius,
      };

      data.Circles.Add(circleData);

      return data;
    }

    private static ObjectData HandleEllipse(Ellipse ellipse, ObjectData data, Point3d origin)
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
        EndAngle = ellipse.EndAngle,
      };

      data.Ellipses.Add(ellipseData);

      return data;
    }

    private static ObjectData HandleMText(MText mText, ObjectData data, Point3d origin)
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
        LineSpaceFactor = mText.LineSpacingFactor,
        LineSpacingStyle = mText.LineSpacingStyle,
        TextHeight = mText.TextHeight,
        Width = mText.Width,
        Rotation = mText.Rotation,
        Direction = mText.Direction
      };

      data.MTexts.Add(mTextData);

      return data;
    }

    private static ObjectData HandleSolid(Solid solid, ObjectData data, Point3d origin)
    {
      var solidData = new SolidData
      {
        Layer = solid.Layer,
        Vertices = new List<SimpleVector3d>(),
      };

      for (short i = 0; i < 4; i++)
      {
        Point3d point = solid.GetPointAt(i);
        Vector3d vector = point - origin;
        solidData.Vertices.Add(new SimpleVector3d { X = vector.X, Y = vector.Y, Z = vector.Z });
      }

      data.Solids.Add(solidData);

      return data;
    }

    private static ObjectData HandleLine(Line line, ObjectData data, Point3d origin)
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
        },
      };

      data.Lines.Add(lineData);

      return data;
    }

    private static ObjectData HandleText(DBText text, ObjectData data, Point3d origin)
    {
      var textData = new TextData
      {
        Layer = text.Layer,
        Style = text.TextStyleName,
        Contents = text.TextString,
        Location = new SimpleVector3d
        {
          X = text.Position.X - origin.X,
          Y = text.Position.Y - origin.Y,
          Z = text.Position.Z - origin.Z
        },
        LineSpaceDistance = text.WidthFactor,
        Height = text.Height,
        Rotation = text.Rotation,
        AlignmentPoint = new SimpleVector3d
        {
          X = text.AlignmentPoint.X - origin.X,
          Y = text.AlignmentPoint.Y - origin.Y,
          Z = text.AlignmentPoint.Z - origin.Z
        },
        IsMirroredInX = text.IsMirroredInX,
        IsMirroredInY = text.IsMirroredInY
      };

      data.Texts.Add(textData);

      return data;
    }

    public static void SaveDataToJsonFile(object data, string fileName)
    {
      string jsonData = JsonConvert.SerializeObject(data, Formatting.Indented);
      string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
      string fullPath = Path.Combine(desktopPath, fileName);
      File.WriteAllText(fullPath, jsonData);
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

  internal class TextStyle
  {
    public double height { get; set; }
    public double widthFactor { get; set; }
    public string fontName { get; set; }

    public TextStyle(double height, double widthFactor, string fontName)
    {
      this.height = height;
      this.widthFactor = widthFactor;
      this.fontName = fontName;
    }

    public void CreateStyleIfNotExisting(string name)
    {
      Document acDoc = Application.DocumentManager.MdiActiveDocument;
      Database acCurDb = acDoc.Database;

      using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
      {
        TextStyleTable acTextStyleTable;
        acTextStyleTable = acTrans.GetObject(acCurDb.TextStyleTableId, OpenMode.ForRead) as TextStyleTable;

        if (acTextStyleTable.Has(name) == false)
        {
          acTextStyleTable.UpgradeOpen();

          TextStyleTableRecord acTextStyleTableRec;
          acTextStyleTableRec = new TextStyleTableRecord();

          acTextStyleTableRec.Name = name;
          acTextStyleTableRec.FileName = this.fontName;
          acTextStyleTableRec.TextSize = this.height;
          acTextStyleTableRec.XScale = this.widthFactor;

          acTextStyleTable.Add(acTextStyleTableRec);
          acTrans.AddNewlyCreatedDBObject(acTextStyleTableRec, true);
        }

        acTrans.Commit();
      }
    }

    public List<string> GetStyles()
    {
      List<string> styles = new List<string>();

      Document acDoc = Application.DocumentManager.MdiActiveDocument;
      Database acCurDb = acDoc.Database;

      using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
      {
        TextStyleTable acTextStyleTable;
        acTextStyleTable = acTrans.GetObject(acCurDb.TextStyleTableId, OpenMode.ForRead) as TextStyleTable;

        foreach (var styleId in acTextStyleTable)
        {
          TextStyleTableRecord acTextStyleTableRec;
          acTextStyleTableRec = acTrans.GetObject(styleId, OpenMode.ForRead) as TextStyleTableRecord;

          styles.Add(acTextStyleTableRec.Name);
        }

        acTrans.Commit();
      }

      return styles;
    }
  }

  internal class ObjectData
  {
    public List<PolylineData> Polylines { get; set; }
    public List<LineData> Lines { get; set; }
    public List<ArcData> Arcs { get; set; }
    public List<CircleData> Circles { get; set; }
    public List<EllipseData> Ellipses { get; set; }
    public List<MTextData> MTexts { get; set; }
    public List<TextData> Texts { get; set; }
    public List<SolidData> Solids { get; set; }
    public int NumberOfRows { get; set; }

    public ObjectData()
    {
      Polylines = new List<PolylineData>();
      Lines = new List<LineData>();
      Arcs = new List<ArcData>();
      Circles = new List<CircleData>();
      Ellipses = new List<EllipseData>();
      MTexts = new List<MTextData>();
      Texts = new List<TextData>();
      Solids = new List<SolidData>();
    }
  }

  internal class TextData : BaseData
  {
    public string Style { get; set; }
    public string Justification { get; set; }
    public string Contents { get; set; }
    public SimpleVector3d Location { get; set; }
    public double LineSpaceDistance { get; set; }
    public double Height { get; set; }
    public double Rotation { get; set; }
    public SimpleVector3d AlignmentPoint { get; set; }
    public string HorizontalMode { get; set; }
    public bool IsMirroredInX { get; set; }
    public bool IsMirroredInY { get; set; }
  }

  internal class PolylineData : BaseData
  {
    public List<SimpleVector3d> Vectors { get; set; }
    public string LineType { get; set; }
    public bool Closed { get; set; }
  }

  internal class LineData : BaseData
  {
    public SimpleVector3d StartPoint { get; set; }
    public SimpleVector3d EndPoint { get; set; }
  }

  internal class ArcData : BaseData
  {
    public SimpleVector3d Center { get; set; }
    public double Radius { get; set; }
    public double StartAngle { get; set; }
    public double EndAngle { get; set; }
  }

  internal class CircleData : BaseData
  {
    public SimpleVector3d Center { get; set; }
    public double Radius { get; set; }
  }

  internal class EllipseData : BaseData
  {
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

  internal class MTextData : BaseData
  {
    public string Style { get; set; }
    public string Justification { get; set; }
    public string Contents { get; set; }
    public Vector3d Direction { get; set; }
    public SimpleVector3d Location { get; set; }
    public double LineSpaceDistance { get; set; }
    public double LineSpaceFactor { get; set; }
    public LineSpacingStyle LineSpacingStyle { get; set; }
    public double TextHeight { get; set; }
    public double Width { get; set; }
    public double Rotation { get; set; }
  }

  internal class SolidData : BaseData
  {
    public List<SimpleVector3d> Vertices { get; set; }
  }

  public class BaseData
  {
    public string Layer { get; set; }
  }

  public class SimpleVector3d
  {
    public SimpleVector3d(double X = 0, double Y = 0, double Z = 0)
    {
      this.X = X;
      this.Y = Y;
      this.Z = Z;
    }

    public double X { get; set; }
    public double Y { get; set; }
    public double Z { get; set; }
  }
}