using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GMEPElectricalResidential
{
  public partial class LoadCalculationForm : Form
  {
    public LoadCalculationForm()
    {
      InitializeComponent();
      AddNewTab();
    }

    public void AddNewTab()
    {
      // get the unit load calculation control
      UnitLoadCalculation unitLoadCalculation = new UnitLoadCalculation();

      // create a new tab page named "Unit 1" if it the first tab, "Unit 2" if it is the second tab, and so on
      TabPage tabPage = new TabPage("Unit " + (TAB_CONTROL.TabCount + 1).ToString());

      // add the unit load calculation control to the tab page
      tabPage.Controls.Add(unitLoadCalculation);

      // add the tab to the tab control
      TAB_CONTROL.TabPages.Add(tabPage);
    }

    public void RemoveCurrentTab()
    {
      // Check if there are any tabs to remove
      if (TAB_CONTROL.TabCount > 0)
      {
        // Ask the user for confirmation
        DialogResult result = MessageBox.Show("Are you sure you want to remove this tab?", "Confirmation", MessageBoxButtons.YesNo);

        if (result == DialogResult.Yes)
        {
          // If the user clicked 'Yes', remove the current tab
          TAB_CONTROL.TabPages.Remove(TAB_CONTROL.SelectedTab);
        }
      }
    }

    private void CREATE_UNIT_BUTTON_Click(object sender, EventArgs e)
    {
      AddNewTab();
    }

    private void DELETE_UNIT_BUTTON_Click(object sender, EventArgs e)
    {
      RemoveCurrentTab();
    }
  }
}