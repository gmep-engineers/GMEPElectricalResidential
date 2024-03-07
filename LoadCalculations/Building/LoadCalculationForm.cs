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

    private void BUILDING_NAME_TextChanged(object sender, EventArgs e)
    {
      var textBox = sender as TextBox;
      if (textBox != null)
      {
        var parentTab = this.Parent as TabPage;
        if (parentTab != null)
        {
          parentTab.Text = _buildingInformation.FormattedName();
        }
      }
      _buildingInformation.Name = textBox.Text;
    }

    private void VOLTAGE_SelectedIndexChanged(object sender, EventArgs e)
    {
      _buildingInformation.Voltage = VOLTAGE.Text;
    }

    private void HOUSE_LOAD_TextChanged(object sender, EventArgs e)
    {
      _buildingInformation.HouseLoad = int.Parse(HOUSE_LOAD.Text);
    }

    private void NUMBER_OF_UNITS_TextChanged(object sender, EventArgs e)
    {
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

    public void UpdateCounters(List<Unit.UnitInformation> unitInformation)
    {
      Counters = new List<UnitCounter>();
      foreach (var unit in unitInformation)
      {
        Counters.Add(new UnitCounter
        {
          UnitID = unit.ID,
          Count = 0
        });
      }
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