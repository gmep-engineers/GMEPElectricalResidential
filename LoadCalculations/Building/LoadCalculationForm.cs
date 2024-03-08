using GMEPElectricalResidential.LoadCalculations.Unit;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
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
      VOLTAGE.Text = buildingInformation.Voltage;
      HOUSE_LOAD.Text = buildingInformation.HouseLoad.ToString();
      HOUSE_LOAD_COPY.Text = buildingInformation.HouseLoad.ToString();
      TOTAL_NUMBER_OF_UNITS.Text = buildingInformation.TotalUnitCount().ToString();
      SUBTOTAL_RESIDENTIAL_LOAD.Text = buildingInformation.TotalSubtotalLoad().ToString();
      DEMAND_FACTOR.Text = buildingInformation.DemandFactor().ToString();
      TOTAL_DEMAND_LOAD.Text = buildingInformation.TotalDemandLoad().ToString();
      TOTAL_DEMAND_HOUSE_LOAD.Text = buildingInformation.TotalDemandHouseLoad().ToString();
      TOTAL_AMPERAGE.Text = buildingInformation.TotalAmperage().ToString();
      SERVICE_RATING.Text = buildingInformation.ServiceRating().ToString();

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
      HOUSE_LOAD.KeyPress += UpdateHouseLoad;
      NUMBER_OF_UNITS.KeyPress += OnlyDigitInputs;
      NUMBER_OF_UNITS.KeyPress += OnlyWhenLoadBoxSelected;
      NUMBER_OF_UNITS.KeyPress += UpdateUnitCountInformation;
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
      _buildingInformation.Voltage = VOLTAGE.Text;
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
    }

    private void UpdateHouseLoad(object sender, KeyPressEventArgs e)
    {
      string houseLoadText = HOUSE_LOAD.Text;

      if (char.IsDigit(e.KeyChar))
      {
        if (HOUSE_LOAD.SelectionLength > 0)
        {
          houseLoadText = houseLoadText.Remove(HOUSE_LOAD.SelectionStart, HOUSE_LOAD.SelectionLength);
        }
        houseLoadText += e.KeyChar;
        if (int.TryParse(houseLoadText, out int houseLoad))
        {
          WriteMessageToAutoCADConsole($"House Load: {houseLoad}\n");
          _buildingInformation.HouseLoad = houseLoad;
          HOUSE_LOAD_COPY.Text = _buildingInformation.HouseLoad.ToString();
        }
      }
      else if (e.KeyChar == '\b')
      {
        int selectionStart = HOUSE_LOAD.SelectionStart;
        int selectionLength = HOUSE_LOAD.SelectionLength;
        if (selectionLength > 1)
        {
          houseLoadText = houseLoadText.Remove(selectionStart, selectionLength);
        }
        else if (selectionStart > 0)
        {
          houseLoadText = houseLoadText.Remove(selectionStart - 1, 1);
        }
        if (int.TryParse(houseLoadText, out int houseLoad))
        {
          _buildingInformation.HouseLoad = houseLoad;
          HOUSE_LOAD_COPY.Text = _buildingInformation.HouseLoad.ToString();
        }
      }
      UpdateBuildingInformation();
    }

    private void UpdateBuildingInformation()
    {
      TOTAL_NUMBER_OF_UNITS.Text = _buildingInformation.TotalUnitCount().ToString();
      SUBTOTAL_RESIDENTIAL_LOAD.Text = _buildingInformation.TotalSubtotalLoad().ToString();
      DEMAND_FACTOR.Text = _buildingInformation.DemandFactor().ToString();
      TOTAL_DEMAND_LOAD.Text = _buildingInformation.TotalDemandLoad().ToString();
      TOTAL_DEMAND_HOUSE_LOAD.Text = _buildingInformation.TotalDemandHouseLoad().ToString();
      TOTAL_AMPERAGE.Text = _buildingInformation.TotalAmperage().ToString();
      SERVICE_RATING.Text = _buildingInformation.ServiceRating().ToString();
    }

    private void UpdateUnitCountInformation(object sender, KeyPressEventArgs e)
    {
      var allUnitInfo = _parentForm.AllUnitInformation();
      var selectedUnit = allUnitInfo.FirstOrDefault(unit => unit.FormattedName() == UNIT_TYPES.Text);
      var numberOfUnitsText = NUMBER_OF_UNITS.Text;
      numberOfUnitsText = HandleNumberOfUnitsKeyPresses(e, numberOfUnitsText);

      if (string.IsNullOrEmpty(HOUSE_LOAD.Text))
      {
        HOUSE_LOAD.Text = "0";
      }
    }

    private string HandleNumberOfUnitsKeyPresses(KeyPressEventArgs e, string numberOfUnitsText)
    {
      if (char.IsDigit(e.KeyChar))
      {
        if (NUMBER_OF_UNITS.SelectionLength > 0)
        {
          numberOfUnitsText = numberOfUnitsText.Remove(NUMBER_OF_UNITS.SelectionStart, NUMBER_OF_UNITS.SelectionLength);
        }
        numberOfUnitsText += e.KeyChar;
        UpdateSubtotalOfUnitLoads(numberOfUnitsText);
      }
      else if (e.KeyChar == '\b')
      {
        int selectionStart = NUMBER_OF_UNITS.SelectionStart;
        int selectionLength = NUMBER_OF_UNITS.SelectionLength;
        if (selectionLength > 1)
        {
          numberOfUnitsText = numberOfUnitsText.Remove(selectionStart, selectionLength);
        }
        else if (selectionStart > 0)
        {
          numberOfUnitsText = numberOfUnitsText.Remove(selectionStart - 1, 1);
        }
        UpdateSubtotalOfUnitLoads(numberOfUnitsText);
      }
      else if (e.KeyChar == '\r')
      {
        SelectNextItem();
      }

      return numberOfUnitsText;
    }

    private void UpdateSubtotalOfUnitLoads(string numberOfUnitsText)
    {
      var allUnitInfo = _parentForm.AllUnitInformation();
      var selectedUnit = allUnitInfo.FirstOrDefault(unit => unit.FormattedName() == UNIT_TYPES.Text);
      if (selectedUnit != null)
      {
        int count;
        if (int.TryParse(numberOfUnitsText, out count))
        {
          var subtotal = count * selectedUnit.Totals.TotalGeneralLoad;
          _buildingInformation.UpdateCounter(selectedUnit, count, subtotal);
          SUBTOTAL_UNIT_LOADS.Text = subtotal.ToString();
        }
        else
        {
          _buildingInformation.UpdateCounter(selectedUnit, 0, 0);
          SUBTOTAL_UNIT_LOADS.Text = "0";
        }
        UpdateBuildingInformation();
      }
    }

    private void UNIT_TYPES_SelectedIndexChanged(object sender, EventArgs e)
    {
      NUMBER_OF_UNITS.Enabled = true;
      NUMBER_OF_UNITS.Text = "";
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
  }

  public class BuildingInformation
  {
    public string Name { get; set; }
    public string Voltage { get; set; }
    public int ID { get; }
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

    public int TotalUnitCount()
    {
      int total = 0;
      foreach (var counter in Counters)
      {
        total += counter.Count;
      }
      return total;
    }

    public int TotalSubtotalLoad()
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
      return $"Building {Name} - ID{ID}";
    }

    public double TotalDemandLoad()
    {
      return Math.Round(TotalSubtotalLoad() * DemandFactor(), 1);
    }

    public double DemandFactor()
    {
      var dwellingUnits = TotalUnitCount();
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

    public double TotalDemandHouseLoad()
    {
      return (HouseLoad ?? 0) + TotalDemandLoad();
    }

    public double TotalAmperage()
    {
      int voltage = int.Parse(Voltage.TrimEnd('V'));
      return Math.Ceiling(TotalDemandHouseLoad() / voltage);
    }

    public int ServiceRating()
    {
      int[] possibleValues = { 30, 60, 100, 125, 150, 200, 400, 600, 800, 1000, 1200, 1600, 2000, 2500, 3000 };
      int totalAmperage = (int)Math.Ceiling(TotalAmperage());
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