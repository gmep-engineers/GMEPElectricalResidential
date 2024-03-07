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

      SetDefaultValues();
    }

    public BuildingInformation RetrieveBuildingInformation()
    {
      return _buildingInformation;
    }

    private void SetDefaultValues()
    {
      VOLTAGE.SelectedIndex = 0;
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
  }

  public class BuildingInformation
  {
    public string Name { get; set; }
    public string Voltage { get; set; }
    public int ID { get; }
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
  }

  public class UnitCounter
  {
    public int UnitID { get; set; }
    public int Count { get; set; }
  }
}