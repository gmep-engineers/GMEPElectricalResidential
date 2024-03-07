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
    private bool _unitNullFlag = false;

    public LoadCalculationForm(LOAD_CALCULATION_FORM parent, int tabID, BuildingInformation buildingInformation = null)
    {
      InitializeComponent();
      _parentForm = parent;
      _buildingInformation = new BuildingInformation(tabID);
      _toolTip = new ToolTip();
      SetDefaultValues();
      DetectIncorrectInputs();
    }

    private void DetectIncorrectInputs()
    {
      HOUSE_LOAD.KeyPress += OnlyDigitInputs;
      NUMBER_OF_UNITS.KeyPress += OnlyDigitInputs;
      NUMBER_OF_UNITS.KeyPress += OnlyWhenLoadBoxSelected;
      NUMBER_OF_UNITS.KeyPress += UpdateUnitCountInformation;
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

    private void HOUSE_LOAD_TextChanged(object sender, EventArgs e)
    {
      _buildingInformation.HouseLoad = int.Parse(HOUSE_LOAD.Text);
    }

    private void UpdateUnitCountInformation(object sender, KeyPressEventArgs e)
    {
      var allUnitInfo = _parentForm.AllUnitInformation();
      var selectedUnit = allUnitInfo.FirstOrDefault(unit => unit.FormattedName() == UNIT_TYPES.Text);
      var numberOfUnitsText = NUMBER_OF_UNITS.Text;
      numberOfUnitsText = HandleNumberOfUnitsKeyPresses(e, numberOfUnitsText);
      if (int.TryParse(numberOfUnitsText, out int numberOfUnits))
      {
        _buildingInformation.UpdateCounter(selectedUnit, numberOfUnits);
      }
      else
      {
        _buildingInformation.UpdateCounter(selectedUnit, 0);
      }
    }

    private string HandleNumberOfUnitsKeyPresses(KeyPressEventArgs e, string numberOfUnitsText)
    {
      if (char.IsDigit(e.KeyChar))
      {
        numberOfUnitsText += e.KeyChar;
      }
      else if (e.KeyChar == '\b')
      {
        if (numberOfUnitsText.Length > 0)
        {
          int selectionStart = NUMBER_OF_UNITS.SelectionStart;
          int selectionLength = NUMBER_OF_UNITS.SelectionLength;
          numberOfUnitsText = numberOfUnitsText.Remove(selectionStart, selectionLength);
        }
      }

      return numberOfUnitsText;
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
        }
      }
    }

    private void NEXT_Click(object sender, EventArgs e)
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
    public int HouseLoad { get; set; }
    public List<UnitCounter> Counters { get; set; }

    public BuildingInformation(int id)
    {
      ID = id;
      Counters = new List<UnitCounter>();
    }

    public void UpdateCounter(Unit.UnitInformation unit, int count)
    {
      var existingCounter = Counters.FirstOrDefault(c => c.UnitID == unit.ID);
      if (existingCounter != null)
      {
        Counters.Remove(existingCounter);
      }

      Counters.Add(new UnitCounter
      {
        UnitID = unit.ID,
        Count = count
      });
    }

    public string FormattedName()
    {
      return $"Building {Name} - ID:{ID}";
    }
  }

  public class UnitCounter
  {
    public int UnitID { get; set; }
    public int Count { get; set; }
  }
}