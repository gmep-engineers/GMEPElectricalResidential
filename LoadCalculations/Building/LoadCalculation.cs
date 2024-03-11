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
      HelperFiles.HelperClass.SaveDataToJsonFileOnDesktop(buildingInfo, "building.json");
      HelperFiles.HelperClass.SaveDataToJsonFileOnDesktop(point, "point.json");

      var COLUMN_WIDTH = 1.5;
      var initialWidth = 8.2034;
      var widthNoCols = initialWidth - COLUMN_WIDTH;

      var numberOfUnitTypes = GetNumberOfUnitTypes(buildingInfo);
      var totalWidth = widthNoCols + (COLUMN_WIDTH * numberOfUnitTypes);

      var newPoint = new Point3d(point.X - totalWidth, point.Y, point.Z);
    }

    private static int GetNumberOfUnitTypes(BuildingInformation buildingInfo)
    {
      var counters = buildingInfo.Counters;
      var numberOfUnitTypes = counters.Count(c => c.Count > 0);

      return numberOfUnitTypes;
    }
  }
}