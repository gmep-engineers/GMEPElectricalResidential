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
      UnitLoadCalculation unitLoadCalculation = new UnitLoadCalculation();
      TabPage tabPage = new TabPage("Unit");
      tabPage.Controls.Add(unitLoadCalculation);
      TAB_CONTROL.TabPages.Add(tabPage);
    }

    public void RemoveCurrentTab()
    {
      if (TAB_CONTROL.TabCount > 0)
      {
        DialogResult result = MessageBox.Show("Are you sure you want to remove this tab?", "Confirmation", MessageBoxButtons.YesNo);

        if (result == DialogResult.Yes)
        {
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