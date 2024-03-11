using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GMEPElectricalResidential.LoadCalculations.Building
{
  public class LoadCalculation
  {
    public static void CreateBuildingLoadCalculationTable(BuildingInformation buildingInfo, Point3d point)
    {
      HelperFiles.HelperClass.WriteMessageToAutoCADConsole(buildingInfo, "Building Information: ");
      HelperFiles.HelperClass.WriteMessageToAutoCADConsole(point, "Point: ");
    }
  }
}