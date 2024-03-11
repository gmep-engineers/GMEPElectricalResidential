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
using System.Windows.Media.Media3D;

namespace GMEPElectricalResidential.LoadCalculations.Building
{
  public class LoadCalculation
  {
    public static void CreateBuildingLoadCalculationTable(BuildingInformation buildingInfo, Point3d placementPoint, bool placeTheBlocks = true)
    {
      double HEADER_HEIGHT = 0.75;
      double COLUMN_WIDTH = 1.5;
      double WIDTH_NO_COLS = 6.7034;
      double currentHeight = HEADER_HEIGHT;
      string newBlockName = $"Building {buildingInfo.Name}" + $" ID{buildingInfo.ID}";

      placementPoint = GetStartingPoint(buildingInfo, placementPoint, COLUMN_WIDTH, WIDTH_NO_COLS);

      if (buildingInfo == null)
      {
        return;
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
            .FirstOrDefault(btr => btr.Name.Contains($"ID{buildingInfo.ID}"));

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
        ObjectData rowData = GetCopyPasteData("Row");

        titleData = UpdateBuildingTitleData(titleData, buildingInfo);

        string modifiedTitleData = JsonConvert.SerializeObject(titleData);

        CADObjectCommands.CreateObjectFromData(modifiedTitleData, point, acBlkTblRec);

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
      string relativeFilePath = Path.Combine(assemblyDirectory, $"LoadCalculations\\Building\\BlockData\\{fileName}.json");

      string jsonData = File.ReadAllText(relativeFilePath);
      ObjectData objectData = JsonConvert.DeserializeObject<ObjectData>(jsonData);
      return objectData;
    }

    private static ObjectData UpdateBuildingTitleData(ObjectData titleData, BuildingInformation buildingInfo)
    {
      var serviceLoadCalculationMText = titleData.MTexts.FirstOrDefault(mText => mText.Contents.Contains("SERVICE LOAD CALCULATION"));
      serviceLoadCalculationMText.Contents = serviceLoadCalculationMText.Contents.Replace("SERVICE LOAD CALCULATION", $"SERVICE LOAD CALCULATION - BUILDING {buildingInfo.Name}");
      return titleData;
    }

    private static ObjectData UpdateBuildingInformationData(ObjectData buildingInformationBodyData, BuildingInformation buildingInfo)
    {
      // Implement the logic to update the building information data
      // based on the buildingInfo object
      // Similar to UpdateDwellingData in the CreateUnitLoadCalculationTable method
      // ...

      return buildingInformationBodyData;
    }

    private static ObjectData UpdateUnitsData(ObjectData unitsBodyData, BuildingInformation buildingInfo)
    {
      // Implement the logic to update the units data
      // based on the buildingInfo object
      // Similar to UpdateGeneralData in the CreateUnitLoadCalculationTable method
      // ...

      return unitsBodyData;
    }

    private static ObjectData UpdateHouseLoadData(ObjectData houseLoadBodyData, BuildingInformation buildingInfo)
    {
      // Implement the logic to update the house load data
      // based on the buildingInfo object
      // Similar to UpdateAirConditioningData in the CreateUnitLoadCalculationTable method
      // ...

      return houseLoadBodyData;
    }

    private static ObjectData UpdateCalculationData(ObjectData calculationBodyData, BuildingInformation buildingInfo)
    {
      // Implement the logic to update the calculation data
      // based on the buildingInfo object
      // Similar to UpdateGeneralCalculationData in the CreateUnitLoadCalculationTable method
      // ...

      return calculationBodyData;
    }

    private static ObjectData UpdateServiceData(ObjectData serviceBodyData, BuildingInformation buildingInfo)
    {
      // Implement the logic to update the service data
      // based on the buildingInfo object
      // Similar to UpdateServiceData in the CreateUnitLoadCalculationTable method
      // ...

      return serviceBodyData;
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
}