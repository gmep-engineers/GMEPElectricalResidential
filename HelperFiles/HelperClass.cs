using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GMEPElectricalResidential.HelperFiles
{
  public class HelperClass
  {
    public static void WriteMessageToAutoCADConsole(object thing, string preMessage = "")
    {
      var settings = new JsonSerializerSettings
      {
        ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
        PreserveReferencesHandling = PreserveReferencesHandling.Objects
      };

      var message = JsonConvert.SerializeObject(thing, Formatting.Indented, settings);
      var doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
      doc.Editor.WriteMessage(preMessage + " ");
      doc.Editor.WriteMessage(message + "\n");
    }

    public static void SaveDataToJsonFileOnDesktop(object data, string fileName, bool noOverride = false)
    {
      string jsonData = JsonConvert.SerializeObject(data, Formatting.Indented);
      string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
      string fullPath = Path.Combine(desktopPath, fileName);

      if (noOverride && File.Exists(fullPath))
      {
        int fileNumber = 1;
        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
        string fileExtension = Path.GetExtension(fileName);

        while (File.Exists(fullPath))
        {
          string newFileName = $"{fileNameWithoutExtension} ({fileNumber}){fileExtension}";
          fullPath = Path.Combine(desktopPath, newFileName);
          fileNumber++;
        }
      }

      File.WriteAllText(fullPath, jsonData);
    }

    public static void SaveDataToJsonFile(object data, string filePath)
    {
      string jsonData = JsonConvert.SerializeObject(data, Formatting.Indented);
      File.WriteAllText(filePath, jsonData);
    }

    public static Point3d UserClick()
    {
      var doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
      var ed = doc.Editor;

      PromptPointResult pPtRes;
      PromptPointOptions pPtOpts = new PromptPointOptions("");

      pPtOpts.Message = "\nClick a point to place the unit load calculation tables: ";
      pPtRes = ed.GetPoint(pPtOpts);

      if (pPtRes.Status == PromptStatus.OK)
      {
        return pPtRes.Value;
      }
      else
      {
        return new Point3d();
      }
    }
  }
}