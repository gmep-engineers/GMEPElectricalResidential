using GMEPElectricalResidential.HelperFiles;
using GMEPElectricalResidential.LoadCalculations.Unit;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static GMEPElectricalResidential.HelperFiles.HelperClass;

namespace GMEPElectricalResidential.LoadCalculations.Building
{
  public partial class LoadCalculationForm : UserControl
  {
    private int _tabID;
    private BuildingInformation _buildingInformation;
    public LOAD_CALCULATION_FORM _parentForm;
    private ToolTip _toolTip;
    private bool _isLoaded = false;
    private bool _buildingNullFlag = false;

    public LoadCalculationForm(LOAD_CALCULATION_FORM parent, int tabID, BuildingInformation buildingInformation = null)
    {
      InitializeComponent();
      _parentForm = parent;
      _toolTip = new ToolTip();
      _tabID = tabID;

      if (buildingInformation != null)
      {
        _buildingInformation = buildingInformation;
      }
      else
      {
        _buildingNullFlag = true;
        _buildingInformation = new BuildingInformation(tabID);
        SetDefaultValues();
      }

      DetectIncorrectInputs();

      this.Load += new EventHandler(UnitLoadCalculation_Load);
    }

    private void UnitLoadCalculation_Load(object sender, EventArgs e)
    {
      if (!_buildingNullFlag)
      {
        PopulateUserControlWithUnitInformation(_buildingInformation);
      }
      _isLoaded = true;
    }

    private void PopulateUserControlWithUnitInformation(BuildingInformation buildingInformation)
    {
      if (buildingInformation == null) return;

      BUILDING_NAME.Text = buildingInformation.Name;
      BUILDING_TITLE.Text = buildingInformation.Title;
      VOLTAGE.Text = buildingInformation.Voltage;
      PHASE_COMBO.Text = buildingInformation.Phase;
      HOUSE_LOAD.Text = buildingInformation.HouseLoad.ToString();
      HOUSE_LOAD_COPY.Text = buildingInformation.HouseLoad.ToString();
      TOTAL_NUMBER_OF_UNITS.Text = buildingInformation.TotalNumberOfUnits().ToString();
      SUBTOTAL_RESIDENTIAL_LOAD.Text = buildingInformation.TotalBuildingLoad().ToString();
      DEMAND_FACTOR.Text = buildingInformation.DemandFactor().ToString();
      TOTAL_DEMAND_LOAD.Text = buildingInformation.TotalBuildingLoadWithDemandFactor().ToString();
      TOTAL_DEMAND_HOUSE_LOAD.Text = buildingInformation.TotalBuildingLoadWithDemandFactorAndHouseLoad().ToString();
      TOTAL_AMPERAGE.Text = buildingInformation.TotalAmperage().ToString();
      SERVICE_RATING.Text = buildingInformation.RecommendedServiceSize().ToString();

      foreach (var counter in buildingInformation.Counters)
      {
        var unit = _parentForm.AllUnitInformation().FirstOrDefault(u => u.ID == counter.UnitID);
        if (unit != null)
        {
          UNIT_TYPES.Items.Add(unit.FormattedName());
        }
      }
    }

    private void DetectIncorrectInputs()
    {
      HOUSE_LOAD.KeyPress += OnlyDigitInputs;
      HOUSE_LOAD.KeyUp += UpdateHouseLoad;
      NUMBER_OF_UNITS.KeyPress += OnlyDigitInputs;
      NUMBER_OF_UNITS.KeyPress += OnlyWhenLoadBoxSelected;
      NUMBER_OF_UNITS.KeyUp += UpdateUnitCountInformation;
      NUMBER_OF_UNITS_BG.Click += InformUserHowToEnable;
    }

    private void InformUserHowToEnable(object sender, EventArgs e)
    {
      if (!NUMBER_OF_UNITS.Enabled)
      {
        _toolTip.SetToolTip(NUMBER_OF_UNITS, "Please select a unit type first");
        _toolTip.Show("Please select a unit type first", NUMBER_OF_UNITS, 0, -20, 2000);
      }
    }

    private void OnlyDigitInputs(object sender, KeyPressEventArgs e)
    {
      if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
      {
        e.Handled = true;
        _toolTip.Show("Input must be a digit", (Control)sender, 0, -20, 2000);
      }
    }

    private void OnlyWhenLoadBoxSelected(object sender, KeyPressEventArgs e)
    {
      if (UNIT_TYPES.SelectedIndex == -1)
      {
        e.Handled = true;
        _toolTip.Show("Select a unit type first", (Control)sender, 0, -20, 2000);
      }
    }

    public BuildingInformation RetrieveBuildingInformation()
    {
      return _buildingInformation;
    }

    private void SetDefaultValues()
    {
      VOLTAGE.SelectedIndex = 0;
      PHASE_COMBO.SelectedIndex = 0;
      BUILDING_TITLE.Text = "ELECTRICAL RESIDENTIAL LOAD CALCULATIONS";
      _buildingInformation.Phase = PHASE_COMBO.Text;
      _buildingInformation.Voltage = VOLTAGE.Text;
      _buildingInformation.Title = BUILDING_TITLE.Text;
      SetLoadBoxValues();
    }

    public void SetLoadBoxValues()
    {
      var allUnitInfo = _parentForm.AllUnitInformation();
      UNIT_TYPES.Items.Clear();
      foreach (var unit in allUnitInfo)
      {
        UNIT_TYPES.Items.Add(unit.FormattedName());
      }
    }

    public void DisableNumberOfUnits()
    {
      NUMBER_OF_UNITS.Enabled = false;
      NUMBER_OF_UNITS.Text = "";
      SUBTOTAL_UNIT_LOADS.Text = "0";
    }

    private void BUILDING_NAME_TextChanged(object sender, EventArgs e)
    {
      var textBox = sender as TextBox;
      if (textBox != null)
      {
        var parentTab = this.Parent as TabPage;
        if (parentTab != null)
        {
          _buildingInformation.Name = textBox.Text;
          parentTab.Text = _buildingInformation.FormattedName();
        }
      }
    }

    private void VOLTAGE_SelectedIndexChanged(object sender, EventArgs e)
    {
      _buildingInformation.Voltage = VOLTAGE.Text;
      UpdateBuildingFormInformation();
    }

    private void UpdateHouseLoad(object sender, KeyEventArgs e)
    {
      string houseLoadText = HOUSE_LOAD.Text;

      if (string.IsNullOrEmpty(houseLoadText))
      {
        houseLoadText = "0";
      }

      _buildingInformation.HouseLoad = int.Parse(houseLoadText);
      HOUSE_LOAD_COPY.Text = _buildingInformation.HouseLoad.ToString();

      UpdateBuildingFormInformation();
    }

    private void UpdateBuildingFormInformation()
    {
      TOTAL_NUMBER_OF_UNITS.Text = _buildingInformation.TotalNumberOfUnits().ToString();
      SUBTOTAL_RESIDENTIAL_LOAD.Text = _buildingInformation.TotalBuildingLoad().ToString();
      DEMAND_FACTOR.Text = _buildingInformation.DemandFactor().ToString();
      TOTAL_DEMAND_LOAD.Text = _buildingInformation.TotalBuildingLoadWithDemandFactor().ToString();
      TOTAL_DEMAND_HOUSE_LOAD.Text = _buildingInformation.TotalBuildingLoadWithDemandFactorAndHouseLoad().ToString();
      TOTAL_AMPERAGE.Text = _buildingInformation.TotalAmperage().ToString();
      SERVICE_RATING.Text = _buildingInformation.RecommendedServiceSize().ToString();
    }

    private void UpdateUnitCountInformation(object sender, KeyEventArgs e)
    {
      UpdateSubtotalOfUnitLoads();
      if (e.KeyValue == '\r') SelectNextItem();
    }

    private void UpdateSubtotalOfUnitLoads()
    {
      UnitInformation selectedUnit = GetSelectedUnit();
      if (selectedUnit == null) return;

      if (int.TryParse(NUMBER_OF_UNITS.Text, out int count))
      {
        var subtotalUnitLoads = count * selectedUnit.Totals.SubtotalOfUnitType();
        _buildingInformation.UpdateCounter(selectedUnit, count, subtotalUnitLoads);
        SUBTOTAL_UNIT_LOADS.Text = subtotalUnitLoads.ToString();
      }
      UpdateBuildingFormInformation();
    }

    private UnitInformation GetSelectedUnit()
    {
      var allUnitInfo = _parentForm.AllUnitInformation();
      var selectedUnit = allUnitInfo.FirstOrDefault(unit => unit.FormattedName() == UNIT_TYPES.Text);
      return selectedUnit;
    }

    private void UNIT_TYPES_SelectedIndexChanged(object sender, EventArgs e)
    {
      NUMBER_OF_UNITS.Enabled = true;
      NUMBER_OF_UNITS.Text = "";
      NUMBER_OF_UNITS.Focus();

      var allUnitInfo = _parentForm.AllUnitInformation();
      var selectedUnit = allUnitInfo.FirstOrDefault(unit => unit.FormattedName() == UNIT_TYPES.Text);
      if (selectedUnit != null)
      {
        var counter = _buildingInformation.Counters.FirstOrDefault(c => c.UnitID == selectedUnit.ID);
        if (counter != null)
        {
          NUMBER_OF_UNITS.Text = counter.Count.ToString();
          SUBTOTAL_UNIT_LOADS.Text = counter.SubtotalLoad.ToString();
        }
        else
        {
          SUBTOTAL_UNIT_LOADS.Text = "0";
        }
        NUMBER_OF_UNITS.SelectAll();
      }
    }

    private void NEXT_Click(object sender, EventArgs e)
    {
      NUMBER_OF_UNITS.Text = "";
      SelectNextItem();
    }

    private void SelectNextItem()
    {
      if (UNIT_TYPES.Items.Count > 0)
      {
        if (UNIT_TYPES.SelectedIndex == -1)
        {
          UNIT_TYPES.SelectedIndex = 0;
        }
        else if (UNIT_TYPES.SelectedIndex == UNIT_TYPES.Items.Count - 1)
        {
          UNIT_TYPES.SelectedIndex = 0;
        }
        else
        {
          UNIT_TYPES.SelectedIndex += 1;
        }
      }
    }

    public void UpdateUnitData(Unit.UnitInformation unitInformation)
    {
      if (_buildingInformation == null) return;
      if (_buildingInformation.Counters == null) return;
      if (unitInformation == null) return;

      var counter = _buildingInformation.Counters.FirstOrDefault(c => c.UnitID == unitInformation.ID);
      if (counter == null) return;

      var existingNumberOfUnits = counter.Count;
      var newSubtotal = unitInformation.Totals.SubtotalOfUnitType() * existingNumberOfUnits;
      _buildingInformation.UpdateCounter(unitInformation, existingNumberOfUnits, newSubtotal);
      UpdateBuildingFormInformation();
    }

    private void PHASE_COMBO_SelectedIndexChanged(object sender, EventArgs e)
    {
      _buildingInformation.Phase = PHASE_COMBO.Text;
      UpdateBuildingFormInformation();
    }

    private void BUILDING_TITLE_TextChanged(object sender, EventArgs e)
    {
      _buildingInformation.Title = BUILDING_TITLE.Text;
    }
  }

  public static class IntExtensions
  {
    public static double ConvertToKVA(this int value)
    {
      return Math.Round(value / 1000.0, 1);
    }
  }

  public class BuildingInformation
  {
    public string Name { get; set; }
    public string Title { get; set; } = "ELECTRICAL RESIDENTIAL LOAD CALCULATIONS";
    public string Voltage { get; set; }
    public string Phase { get; set; }
    public int ID { get; set; }
    public int? HouseLoad { get; set; }
    public List<UnitCounter> Counters { get; set; }

    public BuildingInformation(int id)
    {
      ID = id;
      HouseLoad = 0;
      Counters = new List<UnitCounter>();
    }

    public void UpdateCounter(Unit.UnitInformation unit, int count, int subtotal)
    {
      var existingCounter = Counters.FirstOrDefault(c => c.UnitID == unit.ID);
      if (existingCounter != null)
      {
        Counters.Remove(existingCounter);
      }

      Counters.Add(new UnitCounter
      {
        UnitID = unit.ID,
        Count = count,
        SubtotalLoad = subtotal
      });
    }

    public List<Unit.UnitInformation> GetListOfBuildingUnitTypes(List<Unit.UnitInformation> allUnitInformation)
    {
      var buildingUnitTypes = new List<Unit.UnitInformation>();

      foreach (var counter in Counters)
      {
        if (counter.Count > 0)
        {
          var unitInformation = allUnitInformation.FirstOrDefault(u => u.ID == counter.UnitID);
          if (unitInformation != null)
          {
            buildingUnitTypes.Add(unitInformation);
          }
        }
      }

      return buildingUnitTypes;
    }

    public int TotalNumberOfUnits()
    {
      int total = 0;
      foreach (var counter in Counters)
      {
        total += counter.Count;
      }
      return total;
    }

    public int TotalBuildingLoad()
    {
      int total = 0;
      foreach (var counter in Counters)
      {
        total += counter.SubtotalLoad;
      }
      return total;
    }

    public string FormattedName()
    {
      return $"{Name} - ID{ID}";
    }

    public string FilteredFormattedName()
    {
      if (string.IsNullOrEmpty(Name))
      {
        return $" - ID{ID}";
      }

      string filteredName = new string(Name.Where(c => char.IsLetterOrDigit(c)).ToArray());
      return $"{filteredName} - ID{ID}";
    }

    public int TotalBuildingLoadWithDemandFactor()
    {
      return (int)Math.Round(TotalBuildingLoad() * DemandFactor());
    }

    public double DemandFactor()
    {
      var dwellingUnits = TotalNumberOfUnits();
      if (dwellingUnits >= 3 && dwellingUnits <= 5)
        return 45.0 / 100.0;
      else if (dwellingUnits >= 6 && dwellingUnits <= 7)
        return 44.0 / 100.0;
      else if (dwellingUnits >= 8 && dwellingUnits <= 10)
        return 43.0 / 100.0;
      else if (dwellingUnits == 11)
        return 42.0 / 100.0;
      else if (dwellingUnits >= 12 && dwellingUnits <= 13)
        return 41.0 / 100.0;
      else if (dwellingUnits >= 14 && dwellingUnits <= 15)
        return 40.0 / 100.0;
      else if (dwellingUnits >= 16 && dwellingUnits <= 17)
        return 39.0 / 100.0;
      else if (dwellingUnits >= 18 && dwellingUnits <= 20)
        return 38.0 / 100.0;
      else if (dwellingUnits == 21)
        return 37.0 / 100.0;
      else if (dwellingUnits >= 22 && dwellingUnits <= 23)
        return 36.0 / 100.0;
      else if (dwellingUnits >= 24 && dwellingUnits <= 25)
        return 35.0 / 100.0;
      else if (dwellingUnits >= 26 && dwellingUnits <= 27)
        return 34.0 / 100.0;
      else if (dwellingUnits >= 28 && dwellingUnits <= 30)
        return 33.0 / 100.0;
      else if (dwellingUnits == 31)
        return 32.0 / 100.0;
      else if (dwellingUnits >= 32 && dwellingUnits <= 33)
        return 31.0 / 100.0;
      else if (dwellingUnits >= 34 && dwellingUnits <= 36)
        return 30.0 / 100.0;
      else if (dwellingUnits >= 37 && dwellingUnits <= 38)
        return 29.0 / 100.0;
      else if (dwellingUnits >= 39 && dwellingUnits <= 42)
        return 28.0 / 100.0;
      else if (dwellingUnits >= 43 && dwellingUnits <= 45)
        return 27.0 / 100.0;
      else if (dwellingUnits >= 46 && dwellingUnits <= 50)
        return 26.0 / 100.0;
      else if (dwellingUnits >= 51 && dwellingUnits <= 55)
        return 25.0 / 100.0;
      else if (dwellingUnits >= 56 && dwellingUnits <= 61)
        return 24.0 / 100.0;
      else if (dwellingUnits >= 62)
        return 23.0 / 100.0;
      else
        return 1.0;
    }

    public int TotalBuildingLoadWithDemandFactorAndHouseLoad()
    {
      return (HouseLoad ?? 0) + TotalBuildingLoadWithDemandFactor();
    }

    public int TotalAmperage()
    {
      int voltage = int.Parse(Voltage.TrimEnd('V'));
      double totalDemandHouseLoad = TotalBuildingLoadWithDemandFactorAndHouseLoad();
      if (Phase == "1 PH")
      {
        return (int)Math.Round(totalDemandHouseLoad / voltage);
      }
      else
      {
        return (int)Math.Round(totalDemandHouseLoad / 360);
      }
    }

    public int RecommendedServiceSize()
    {
      int[] possibleValues = { 30, 60, 100, 125, 150, 200, 400, 600, 800, 1000, 1200, 1600, 2000, 2500, 3000 };
      int totalAmperage = TotalAmperage();
      int? serviceRating = possibleValues.FirstOrDefault(value => value >= totalAmperage);

      if (serviceRating == 0)
      {
        serviceRating = 3000;
        int increment = 1000;
        while (serviceRating <= totalAmperage)
        {
          serviceRating += increment;
        }
      }

      return serviceRating ?? 0;
    }
  }

  public class UnitCounter
  {
    public int UnitID { get; set; }
    public int Count { get; set; }
    public int SubtotalLoad { get; set; }
  }
}