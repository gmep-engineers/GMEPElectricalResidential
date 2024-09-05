using GMEPElectricalResidential.LoadCalculations.Unit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace GMEPElectricalResidential.LoadCalculations
{
  public partial class PanelGeneratorUnitSelection : Form
  {
    private List<UnitInformation> units = new List<UnitInformation>();

    public PanelGeneratorUnitSelection(List<UnitInformation> unitInfo)
    {
      InitializeComponent();
      this.UNIT_TYPES.SelectedIndexChanged += new System.EventHandler(this.UNIT_TYPES_SelectedIndexChanged);
      this.PROCEED_BUTTON.Click += new System.EventHandler(this.PROCEED_BUTTON_Click);

      units = unitInfo;
      ConfigureListBox();
      PopulateUnitTypes();
    }

    private void ConfigureListBox()
    {
      UNIT_TYPES.SelectionMode = SelectionMode.MultiSimple;
    }

    private void PopulateUnitTypes()
    {
      UNIT_TYPES.Items.Clear();
      foreach (var unit in units)
      {
        UNIT_TYPES.Items.Add(unit.FilteredFormattedName());
      }
    }

    private void UNIT_TYPES_SelectedIndexChanged(object sender, EventArgs e)
    {
      UpdateSelectedUnitsInfo();
    }

    private void UpdateSelectedUnitsInfo()
    {
      var selectedUnits = GetSelectedUnits();
      // Update any UI elements based on the selection if needed
    }

    private List<UnitInformation> GetSelectedUnits()
    {
      return UNIT_TYPES.SelectedIndices.Cast<int>()
          .Select(index => units[index])
          .ToList();
    }

    private void PROCEED_BUTTON_Click(object sender, EventArgs e)
    {
      var selectedUnits = GetSelectedUnits();

      if (selectedUnits.Count > 0)
      {
        CreatePanelGenerator(selectedUnits);
      }
      else
      {
        MessageBox.Show("Please select at least one unit before proceeding.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
      }
    }

    private void CreatePanelGenerator(List<UnitInformation> selectedUnits)
    {
      var panelGenerator = new PanelGenerator(selectedUnits);
      panelGenerator.Show();
      this.Hide(); // Optionally hide this form
    }
  }
}