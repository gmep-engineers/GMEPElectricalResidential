using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GMEPElectricalResidential
{
  public class Commands : GenericCommands
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

    [CommandMethod("GetObjectData")]
    public void GetObjectData()
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

      HelperClass.SaveDataToJsonFile(data, "data.json");
    }

    [CommandMethod("SETUPXREFS")]
    public void SetupXrefs()
    {
      Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
      Database currentDb = Application.DocumentManager.MdiActiveDocument.Database;

      // Prompt user to select DWG files
      System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog
      {
        Multiselect = true,
        Filter = "DWG files (*.dwg)|*.dwg",
        Title = "Select DWG Files"
      };

      if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
      {
        HashSet<string> allXrefFileNames = new HashSet<string>();

        foreach (string file in ofd.FileNames)
        {
          allXrefFileNames.Add(file);

          Database db = new Database(false, true);
          try
          {
            db.ReadDwgFile(file, FileShare.ReadWrite, true, "");

            string[] xrefFileNames = GetXrefsOfXrefFile(db);

            foreach (string xrefFile in xrefFileNames)
            {
              allXrefFileNames.Add(xrefFile);
            }

            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
              // Create a new layer named "0-GMEP" and set its color to 8
              LayerTable layerTable = (LayerTable)tr.GetObject(db.LayerTableId, OpenMode.ForRead);
              if (!layerTable.Has("0-GMEP"))
              {
                layerTable.UpgradeOpen();
                LayerTableRecord layerRecord = new LayerTableRecord
                {
                  Name = "0-GMEP",
                  Color = Autodesk.AutoCAD.Colors.Color.FromColorIndex(Autodesk.AutoCAD.Colors.ColorMethod.ByAci, 8)
                };
                layerTable.Add(layerRecord);
                tr.AddNewlyCreatedDBObject(layerRecord, true);
              }

              // Get the ObjectId of the "0" layer and the "0-GMEP" layer
              ObjectId zeroLayerId = layerTable["0"];
              ObjectId gmepLayerId = layerTable["0-GMEP"];

              BlockTable bt = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);
              BlockTableRecord btr = (BlockTableRecord)tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);

              foreach (ObjectId objId in btr)
              {
                Entity ent = tr.GetObject(objId, OpenMode.ForWrite) as Entity;
                if (ent != null && ent.LayerId == zeroLayerId)
                {
                  // Move the entity from the "0" layer to the "0-GMEP" layer
                  ent.LayerId = gmepLayerId;
                }
                SetEntityColorToByLayer(ent, tr, 4);
              }

              tr.Commit();
            }

            db.SaveAs(file, DwgVersion.Current);
          }
          catch (Autodesk.AutoCAD.Runtime.Exception ex)
          {
            ed.WriteMessage($"Error processing file {file}: {ex.Message}");
          }
          finally
          {
            db.Dispose();
          }
        }

        // Convert allXrefFileNames to an array
        string[] allXrefFileNamesArray = allXrefFileNames.ToArray();

        // Call the AddDwgAsXref method with the selected files, the editor, and the database
        AddDwgAsXref(ofd.FileNames, ed, currentDb);

        // Call the GrayXref method with the selected files
        GrayXref(allXrefFileNamesArray);

        // Call the MagentaElectricalLayers method with the selected files
        MagentaElectricalLayers(allXrefFileNamesArray);

        ed.WriteMessage("Processing complete.");
      }
    }

    private string[] GetXrefsOfXrefFile(Database db)
    {
      List<string> xrefFileNames = new List<string>();

      // Get the xref graph of the database
      XrefGraph xrefGraph = db.GetHostDwgXrefGraph(true);

      // Traverse the xref graph
      for (int i = 0; i < xrefGraph.NumNodes; i++)
      {
        XrefGraphNode xrefGraphNode = xrefGraph.GetXrefNode(i);

        // Check if the node is an xref (not the main drawing)
        if (xrefGraphNode.XrefStatus == XrefStatus.Resolved)
        {
          // Get the file path of the xref
          string xrefFileName = xrefGraphNode.Name;

          // Add the file path to xrefFileNames
          xrefFileNames.Add(xrefFileName);
        }
      }

      return xrefFileNames.ToArray();
    }

    public void GrayXref(string[] xrefFileNames)
    {
      Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
      Database currentDb = Application.DocumentManager.MdiActiveDocument.Database;

      using (Transaction tr = currentDb.TransactionManager.StartTransaction())
      {
        // Get the LayerTable from the database
        LayerTable layerTable = (LayerTable)tr.GetObject(currentDb.LayerTableId, OpenMode.ForRead);

        // Iterate over all layers in the LayerTable
        foreach (ObjectId layerId in layerTable)
        {
          LayerTableRecord layerRecord = (LayerTableRecord)tr.GetObject(layerId, OpenMode.ForWrite);

          // Extract just the file name without the path and extension from each string in the xrefFileNames array
          string[] xrefFileNamesWithoutPathAndExtension = xrefFileNames.Select(Path.GetFileNameWithoutExtension).ToArray();

          // Check if the layer is from one of the xref files
          if (xrefFileNamesWithoutPathAndExtension.Any(xrefFileName => layerRecord.Name.StartsWith(xrefFileName + "|")))
          {
            // Set the color of the layer to index 8
            layerRecord.Color = Autodesk.AutoCAD.Colors.Color.FromColorIndex(Autodesk.AutoCAD.Colors.ColorMethod.ByAci, 8);
          }
        }

        tr.Commit();
      }

      ed.WriteMessage("Processing complete.");
    }

    public void MagentaElectricalLayers(string[] xrefFileNames)
    {
      Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
      Database currentDb = Application.DocumentManager.MdiActiveDocument.Database;

      using (Transaction tr = currentDb.TransactionManager.StartTransaction())
      {
        // Get the LayerTable from the database
        LayerTable layerTable = (LayerTable)tr.GetObject(currentDb.LayerTableId, OpenMode.ForRead);

        // Extract just the file name without the path and extension from each string in the xrefFileNames array
        string[] xrefFileNamesWithoutPathAndExtension = xrefFileNames.Select(Path.GetFileNameWithoutExtension).ToArray();

        // Iterate over all layers in the LayerTable
        foreach (ObjectId layerId in layerTable)
        {
          LayerTableRecord layerRecord = (LayerTableRecord)tr.GetObject(layerId, OpenMode.ForWrite);

          // Check if the layer is from one of the xref files and contains "LITE", "POWER", or "LIGHT"
          if (xrefFileNamesWithoutPathAndExtension.Any(xrefFileName => layerRecord.Name.StartsWith(xrefFileName + "|")) &&
              (layerRecord.Name.Contains("LITE") || layerRecord.Name.Contains("POWER") || layerRecord.Name.Contains("LIGHT")))
          {
            // Set the color of the layer to index 6
            layerRecord.Color = Autodesk.AutoCAD.Colors.Color.FromColorIndex(Autodesk.AutoCAD.Colors.ColorMethod.ByAci, 6);
          }
        }

        tr.Commit();
      }

      ed.WriteMessage("Processing complete.");
    }

    public void AddDwgAsXref(string[] files, Editor ed, Database currentDb)
    {
      foreach (string file in files)
      {
        using (Transaction tr = currentDb.TransactionManager.StartTransaction())
        {
          BlockTable bt = (BlockTable)tr.GetObject(currentDb.BlockTableId, OpenMode.ForRead);
          BlockTableRecord modelSpace = (BlockTableRecord)tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);

          // Attach the DWG file as an Xref
          ObjectId xrefId = currentDb.AttachXref(file, Path.GetFileNameWithoutExtension(file));

          if (!xrefId.IsNull)
          {
            // Get the bounding box of the xref
            using (Database xrefDb = new Database(false, true))
            {
              xrefDb.ReadDwgFile(file, FileOpenMode.OpenForReadAndAllShare, true, null);
              using (Transaction xrefTr = xrefDb.TransactionManager.StartTransaction())
              {
                BlockTableRecord xrefBtr = (BlockTableRecord)xrefTr.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(xrefDb), OpenMode.ForRead);
                Extents3d? extents = null;
                foreach (ObjectId id in xrefBtr)
                {
                  Entity ent = (Entity)xrefTr.GetObject(id, OpenMode.ForRead);
                  if (ent.Bounds.HasValue)
                  {
                    if (extents.HasValue)
                    {
                      extents.Value.AddExtents(ent.Bounds.Value);
                    }
                    else
                    {
                      extents = ent.Bounds.Value;
                    }
                  }
                }

                Point3d center = extents.HasValue ? new Point3d(
                    -(extents.Value.MinPoint.X + (extents.Value.MaxPoint.X - extents.Value.MinPoint.X) / 2),
                    -(extents.Value.MinPoint.Y + (extents.Value.MaxPoint.Y - extents.Value.MinPoint.Y) / 2),
                    -(extents.Value.MinPoint.Z + (extents.Value.MaxPoint.Z - extents.Value.MinPoint.Z) / 2)) : Point3d.Origin;

                // Add the Xref to the model space at the center of its bounding box
                BlockReference xrefReference = new BlockReference(center, xrefId);
                modelSpace.AppendEntity(xrefReference);
                tr.AddNewlyCreatedDBObject(xrefReference, true);
              }
            }
          }

          tr.Commit();
        }
      }
    }

    private void SetEntityColorToByLayer(Entity ent, Transaction tr, int depth)
    {
      if (ent != null)
      {
        ent.Color = Autodesk.AutoCAD.Colors.Color.FromColorIndex(Autodesk.AutoCAD.Colors.ColorMethod.ByLayer, 256);
      }

      if (depth <= 0)
      {
        return;
      }

      BlockReference blockRef = ent as BlockReference;
      if (blockRef != null)
      {
        blockRef.Color = Autodesk.AutoCAD.Colors.Color.FromColorIndex(Autodesk.AutoCAD.Colors.ColorMethod.ByLayer, 256);

        // Iterate over the entities in the block
        BlockTableRecord blockDef = (BlockTableRecord)tr.GetObject(blockRef.BlockTableRecord, OpenMode.ForRead);
        foreach (ObjectId entId in blockDef)
        {
          Entity blockEnt = tr.GetObject(entId, OpenMode.ForWrite) as Entity;
          SetEntityColorToByLayer(blockEnt, tr, depth - 1);
        }
      }
    }

    public static void CreateUnitLoadCalculationTable(UnitInformation unitInfo, Point3d placementPoint)
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
          WipeExistingBlockContent(acBlkTblRec);
        }
        else
        {
          acBlkTbl.UpgradeOpen();
          acBlkTblRec = new BlockTableRecord();
          acBlkTblRec.Name = newBlockName;
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

        if (acBlkTbl.Has(newBlockName))
        {
          BlockReference acBlkRef = new BlockReference(placementPoint, acBlkTbl[newBlockName]);

          acBlkTblRec.AppendEntity(acBlkRef);

          acTrans.AddNewlyCreatedDBObject(acBlkRef, true);
        }

        UpdateAllBlockReferences(newBlockName);

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
            $"Microwave Oven{((unitInfo.GeneralLoads.MicrowaveOven.Multiplier <= 1) ? ":" : $" ({unitInfo.GeneralLoads.MicrowaveOven.Multiplier}):")}",
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