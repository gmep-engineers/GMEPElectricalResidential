using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Data;

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

    public static void CreateObjectFromData(string jsonData, Point3d basePoint)
    {
      ObjectData objectData = JsonConvert.DeserializeObject<ObjectData>(jsonData);

      Document acDoc = Application.DocumentManager.MdiActiveDocument;
      Database acCurDb = acDoc.Database;

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

    public static void CreateUnitLoadCalculationTable(UnitInformation unitInfo, Point3d point)
    {
      double HEADER_HEIGHT = 0.75;
      double currentHeight = HEADER_HEIGHT;

      ObjectData headerData = GetCopyPasteData("UnitLoadCalculationHeader");
      ObjectData bodyData = GetCopyPasteData("UnitLoadCalculationBody");

      ObjectData dwellingBodyData = ShiftData(bodyData, -currentHeight);
      dwellingBodyData = UpdateDwellingData(dwellingBodyData, unitInfo);
      double dwellingSectionHeight = CreateUnitLoadCalculationRectangle(point, -currentHeight, dwellingBodyData.NumberOfRows);

      currentHeight += dwellingSectionHeight;

      ObjectData generalBodyData = ShiftData(bodyData, -currentHeight);
      generalBodyData = UpdateGeneralData(generalBodyData, unitInfo);
      double generalSectionHeight = CreateUnitLoadCalculationRectangle(point, -currentHeight, generalBodyData.NumberOfRows);

      currentHeight += generalSectionHeight;

      ObjectData generalBodyCalcData = ShiftData(bodyData, -currentHeight);
      generalBodyCalcData = UpdateGeneralCalculationData(generalBodyCalcData, unitInfo);
      double generalCalcSectionHeight = CreateUnitLoadCalculationRectangle(point, -currentHeight, generalBodyCalcData.NumberOfRows);

      currentHeight += generalCalcSectionHeight;

      ObjectData airConditioningBodyData = ShiftData(bodyData, -currentHeight);
      airConditioningBodyData = UpdateAirConditioningData(airConditioningBodyData, unitInfo);
      double airConditioningSectionHeight = CreateUnitLoadCalculationRectangle(point, -currentHeight, airConditioningBodyData.NumberOfRows);

      string modifiedHeaderData = JsonConvert.SerializeObject(headerData);
      string modifiedDwellingBodyData = JsonConvert.SerializeObject(dwellingBodyData);
      string modifiedGeneralBodyData = JsonConvert.SerializeObject(generalBodyData);
      string modifiedGeneralBodyCalcData = JsonConvert.SerializeObject(generalBodyCalcData);
      string modifiedAirConditioningBodyData = JsonConvert.SerializeObject(airConditioningBodyData);

      CreateObjectFromData(modifiedHeaderData, point);
      CreateObjectFromData(modifiedDwellingBodyData, point);
      CreateObjectFromData(modifiedGeneralBodyData, point);
      CreateObjectFromData(modifiedGeneralBodyCalcData, point);
      CreateObjectFromData(modifiedAirConditioningBodyData, point);
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
      var headers = generalBodyData.MTexts.FirstOrDefault(mText => mText.Contents.Contains("Title"));
      if (headers != null)
      {
        headers.Contents = "";
        string dwellingTitle = "General Load:".Underline().NewLine();
        string dwellingSubtitles = $"General Lighting (Floor Area x 3VA/ft²) (CEC {UnitGeneralLoadContainer.LightingCode}):".NewLine() +
                           $"Small Appliance (3-20ACK by CEC 210.11){((unitInfo.GeneralLoads.SmallAppliance.Multiplier <= 1) ? ":" : $" ({unitInfo.GeneralLoads.SmallAppliance.Multiplier}):")}".NewLine() +
                           $"Laundry (1-20ACKT by CEC 210.11){((unitInfo.GeneralLoads.Laundry.Multiplier <= 1) ? ":" : $" ({unitInfo.GeneralLoads.Laundry.Multiplier}):")}".NewLine() +
                           $"Bathroom (1-20ACKT by CEC 210.11){((unitInfo.GeneralLoads.Bathroom.Multiplier <= 1) ? ":" : $" ({unitInfo.GeneralLoads.Bathroom.Multiplier}):")}".NewLine() +
                           $"Dishwasher{((unitInfo.GeneralLoads.Dishwasher.Multiplier <= 1) ? ":" : $" ({unitInfo.GeneralLoads.Dishwasher.Multiplier}):")}".NewLine() +
                           $"Microwave Oven{((unitInfo.GeneralLoads.MicrowaveOven.Multiplier <= 1) ? ":" : $" ({unitInfo.GeneralLoads.MicrowaveOven.Multiplier}):")}".NewLine() +
                           $"Garbage Disposal{((unitInfo.GeneralLoads.GarbageDisposal.Multiplier <= 1) ? ":" : $" ({unitInfo.GeneralLoads.GarbageDisposal.Multiplier}):")}".NewLine() +
                           $"Bathroom Fans{((unitInfo.GeneralLoads.BathroomFans.Multiplier <= 1) ? ":" : $" ({unitInfo.GeneralLoads.BathroomFans.Multiplier}):")}".NewLine() +
                           $"Garage Door Opener{((unitInfo.GeneralLoads.GarageDoorOpener.Multiplier <= 1) ? ":" : $" ({unitInfo.GeneralLoads.GarageDoorOpener.Multiplier}):")}".NewLine() +
                           $"Dryer{((unitInfo.GeneralLoads.Dryer.Multiplier <= 1) ? ":" : $" ({unitInfo.GeneralLoads.Dryer.Multiplier}):")}".NewLine() +
                           $"Range{((unitInfo.GeneralLoads.Range.Multiplier <= 1) ? ":" : $" ({unitInfo.GeneralLoads.Range.Multiplier}):")}".NewLine() +
                           $"Refrigerator{((unitInfo.GeneralLoads.Refrigerator.Multiplier <= 1) ? ":" : $" ({unitInfo.GeneralLoads.Refrigerator.Multiplier}):")}".NewLine() +
                           $"Oven{((unitInfo.GeneralLoads.Oven.Multiplier <= 1) ? ":" : $" ({unitInfo.GeneralLoads.Oven.Multiplier}):")}".NewLine() +
                           $"Water Heater{((unitInfo.GeneralLoads.WaterHeater.Multiplier <= 1) ? ":" : $" ({unitInfo.GeneralLoads.WaterHeater.Multiplier}):")}".NewLine() +
                           $"Cooktop{((unitInfo.GeneralLoads.Cooktop.Multiplier <= 1) ? ":" : $" ({unitInfo.GeneralLoads.Cooktop.Multiplier}):")}".NewLine();

        unitInfo.GeneralLoads.Customs.ForEach(customLoad =>
        {
          dwellingSubtitles += $"{customLoad.Name}{((customLoad.Multiplier <= 1) ? ":" : $" ({customLoad.Multiplier}):")}".NewLine();
          startingRows++;
        });

        string dwellingTitleAndSubtitles = dwellingTitle + dwellingSubtitles;

        headers.Contents = dwellingTitleAndSubtitles.SetFont("Arial");
      }

      var values = generalBodyData.MTexts.FirstOrDefault(mText => mText.Contents.Contains("Subtitle VA"));
      if (values != null)
      {
        values.Contents = "";
        string generalValues = "".NewLine() +
                       $"{unitInfo.GeneralLoads.Lighting.VA * unitInfo.GeneralLoads.Lighting.Multiplier}VA".NewLine() +
                       $"{unitInfo.GeneralLoads.SmallAppliance.VA * unitInfo.GeneralLoads.SmallAppliance.Multiplier}VA".NewLine() +
                       $"{unitInfo.GeneralLoads.Laundry.VA * unitInfo.GeneralLoads.Laundry.Multiplier}VA".NewLine() +
                       $"{unitInfo.GeneralLoads.Bathroom.VA * unitInfo.GeneralLoads.Bathroom.Multiplier}VA".NewLine() +
                       $"{unitInfo.GeneralLoads.Dishwasher.VA * unitInfo.GeneralLoads.Dishwasher.Multiplier}VA".NewLine() +
                       $"{unitInfo.GeneralLoads.MicrowaveOven.VA * unitInfo.GeneralLoads.MicrowaveOven.Multiplier}VA".NewLine() +
                       $"{unitInfo.GeneralLoads.GarbageDisposal.VA * unitInfo.GeneralLoads.GarbageDisposal.Multiplier}VA".NewLine() +
                       $"{unitInfo.GeneralLoads.BathroomFans.VA * unitInfo.GeneralLoads.BathroomFans.Multiplier}VA".NewLine() +
                       $"{unitInfo.GeneralLoads.GarageDoorOpener.VA * unitInfo.GeneralLoads.GarageDoorOpener.Multiplier}VA".NewLine() +
                       $"{unitInfo.GeneralLoads.Dryer.VA * unitInfo.GeneralLoads.Dryer.Multiplier}VA".NewLine() +
                       $"{unitInfo.GeneralLoads.Range.VA * unitInfo.GeneralLoads.Range.Multiplier}VA".NewLine() +
                       $"{unitInfo.GeneralLoads.Refrigerator.VA * unitInfo.GeneralLoads.Refrigerator.Multiplier}VA".NewLine() +
                       $"{unitInfo.GeneralLoads.Oven.VA * unitInfo.GeneralLoads.Oven.Multiplier}VA".NewLine() +
                       $"{unitInfo.GeneralLoads.WaterHeater.VA * unitInfo.GeneralLoads.WaterHeater.Multiplier}VA".NewLine() +
                       $"{unitInfo.GeneralLoads.Cooktop.VA * unitInfo.GeneralLoads.Cooktop.Multiplier}VA".NewLine();

        unitInfo.GeneralLoads.Customs.ForEach(customLoad =>
        {
          generalValues += $"{customLoad.VA}VA".NewLine();
        });

        values.Contents = generalValues.SetFont("Arial");
      }

      generalBodyData.NumberOfRows = startingRows;

      return generalBodyData;
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

    private static double CreateUnitLoadCalculationRectangle(Point3d point, double shiftY, int numberOfRows)
    {
      double MARGIN_TOP_BOT = 0.16;
      double ROW_HEIGHT = 0.245;
      double WIDTH = 7.0;

      Point3d topRight = new Point3d(point.X, point.Y + shiftY, point.Z);
      Point3d topLeft = new Point3d(point.X - WIDTH, point.Y + shiftY, point.Z);

      double height = MARGIN_TOP_BOT * 2 + (ROW_HEIGHT * numberOfRows);

      Point3d bottomLeft = new Point3d(point.X - WIDTH, point.Y + shiftY - height, point.Z);
      Point3d bottomRight = new Point3d(point.X, point.Y + shiftY - height, point.Z);

      List<Point3d> points = new List<Point3d> { topRight, topLeft, bottomLeft, bottomRight };

      CreateClosedPolylineGivenPoints(points);

      return height;
    }

    private static void CreateClosedPolylineGivenPoints(List<Point3d> points)
    {
      Document acDoc = Application.DocumentManager.MdiActiveDocument;
      Database acCurDb = acDoc.Database;

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
      dbText.Layer = text.Layer;
      dbText.TextString = text.Contents;
      dbText.Position = new Point3d(basePoint.X + text.Location.X, basePoint.Y + text.Location.Y, basePoint.Z + text.Location.Z);
      dbText.Height = text.Height;
      dbText.Rotation = text.Rotation;
      dbText.WidthFactor = text.LineSpaceDistance;
      SetTextStyleByName(dbText, text.Style);

      acBlkTblRec.AppendEntity(dbText);
      acTrans.AddNewlyCreatedDBObject(dbText, true);
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
    public double X { get; set; }
    public double Y { get; set; }
    public double Z { get; set; }
  }
}