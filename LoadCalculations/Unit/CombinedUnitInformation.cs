using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GMEPElectricalResidential.LoadCalculations.Unit
{
  public class CombinedUnitInformation
  {
    public static UnitInformation CreateCombinedCopyOfUnitInfo(UnitInformation unitInfo1, UnitInformation unitInfo2)
    {
      UnitInformation combinedUnitInfo = new UnitInformation(unitInfo1.ID);

      combinedUnitInfo.Name = $"{unitInfo1.Name} & {unitInfo2.Name}";
      combinedUnitInfo.Voltage = unitInfo1.Voltage;
      combinedUnitInfo.DwellingArea = CreateCombinedDwellingArea(unitInfo1.DwellingArea, unitInfo2.DwellingArea);
      combinedUnitInfo.GeneralLoads = CreateCombinedGeneralLoads(unitInfo1.GeneralLoads, unitInfo2.GeneralLoads);
      combinedUnitInfo.CustomLoads = CreateCombinedCustomLoads(unitInfo1.CustomLoads, unitInfo2.CustomLoads);
      combinedUnitInfo.ACLoads = CreateCombinedACLoads(unitInfo1.ACLoads, unitInfo2.ACLoads);
      combinedUnitInfo.Totals = CreateCombinedTotals(unitInfo1.Totals, unitInfo2.Totals);

      return combinedUnitInfo;
    }

    public static UnitGeneralLoadContainer CreateCombinedGeneralLoads(UnitGeneralLoadContainer generalLoads1, UnitGeneralLoadContainer generalLoads2)
    {
      UnitGeneralLoadContainer combinedGeneralLoads = new UnitGeneralLoadContainer();

      combinedGeneralLoads.Lighting = new UnitLoad(generalLoads1.Lighting.Name, (generalLoads1.Lighting.Total + generalLoads2.Lighting.Total).ToString(), "1");

      combinedGeneralLoads.Customs = CreateCombinedCustomLoads(generalLoads1.Customs, generalLoads2.Customs);

      combinedGeneralLoads.LightingOccupancyType = generalLoads1.LightingOccupancyType;

      return combinedGeneralLoads;
    }

    public static List<UnitLoad> CreateCombinedCustomLoads(List<UnitLoad> customLoads1, List<UnitLoad> customLoads2)
    {
      Dictionary<string, UnitLoad> combinedCustomLoads = new Dictionary<string, UnitLoad>();

      foreach (UnitLoad customLoad in customLoads1)
      {
        combinedCustomLoads[customLoad.Name] = new UnitLoad(customLoad.Name, customLoad.Total.ToString(), customLoad.Multiplier.ToString(), customLoad.IsCookingAppliance);
      }

      foreach (UnitLoad customLoad in customLoads2)
      {
        if (combinedCustomLoads.ContainsKey(customLoad.Name))
        {
          UnitLoad existingLoad = combinedCustomLoads[customLoad.Name];
          existingLoad.Total += customLoad.Total;
          existingLoad.Multiplier += customLoad.Multiplier;
          existingLoad.IsCookingAppliance = existingLoad.IsCookingAppliance || customLoad.IsCookingAppliance;
        }
        else
        {
          combinedCustomLoads[customLoad.Name] = new UnitLoad(customLoad.Name, customLoad.Total.ToString(), customLoad.Multiplier.ToString(), customLoad.IsCookingAppliance);
        }
      }

      return combinedCustomLoads.Values.ToList();
    }

    public static UnitDwellingArea CreateCombinedDwellingArea(UnitDwellingArea dwellingArea1, UnitDwellingArea dwellingArea2)
    {
      UnitDwellingArea combinedDwellingArea = new UnitDwellingArea();

      combinedDwellingArea.FloorArea = (int.Parse(dwellingArea1.FloorArea) + int.Parse(dwellingArea2.FloorArea)).ToString();
      combinedDwellingArea.Heater = dwellingArea1.Heater;
      combinedDwellingArea.Dryer = dwellingArea1.Dryer;
      combinedDwellingArea.Oven = dwellingArea1.Oven;
      combinedDwellingArea.Cooktop = dwellingArea1.Cooktop;

      return combinedDwellingArea;
    }

    public static UnitACLoadContainer CreateCombinedACLoads(UnitACLoadContainer acLoads1, UnitACLoadContainer acLoads2)
    {
      UnitACLoadContainer combinedACLoads = new UnitACLoadContainer();

      combinedACLoads.Condenser = acLoads1.Condenser + acLoads2.Condenser;
      combinedACLoads.FanCoil = acLoads1.FanCoil + acLoads2.FanCoil;

      HeatingUnit combinedHeatingUnit = new HeatingUnit();
      combinedHeatingUnit.Heating = acLoads1.HeatingUnit.Heating + acLoads2.HeatingUnit.Heating;
      combinedHeatingUnit.NumberOfUnits = acLoads1.HeatingUnit.NumberOfUnits + acLoads2.HeatingUnit.NumberOfUnits;
      combinedACLoads.HeatingUnit = combinedHeatingUnit;

      combinedACLoads.ElectricalCode = acLoads1.ElectricalCode;

      return combinedACLoads;
    }

    public static UnitTotalContainer CreateCombinedTotals(UnitTotalContainer totals1, UnitTotalContainer totals2)
    {
      UnitTotalContainer combinedTotals = new UnitTotalContainer();

      combinedTotals.TotalGeneralLoad = totals1.TotalGeneralLoad + totals2.TotalGeneralLoad;
      combinedTotals.TotalACLoad = totals1.TotalACLoad + totals2.TotalACLoad;
      combinedTotals.SubtotalGeneralLoad = totals1.SubtotalGeneralLoad + totals2.SubtotalGeneralLoad;
      combinedTotals.CustomLoad = totals1.CustomLoad + totals2.CustomLoad;
      combinedTotals.ServiceLoad = totals1.ServiceLoad + totals2.ServiceLoad;

      return combinedTotals;
    }
  }
}