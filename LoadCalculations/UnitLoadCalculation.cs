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
  public partial class UnitLoadCalculation : UserControl
  {
    public UnitLoadCalculation()
    {
      InitializeComponent();
      SetDefaultValues();
    }

    private void SetDefaultValues()
    {
      VOLTAGE.SelectedIndex = 0;
      var parentTabControl = this.Parent as TabControl;
      if (parentTabControl != null)
      {
        UNIT_NAME.Text = parentTabControl.TabCount.ToString();
      }
    }

    private void UNIT_NAME_TextChanged(object sender, EventArgs e)
    {
      var textBox = sender as TextBox;
      if (textBox != null)
      {
        var parentTab = this.Parent as TabPage;
        if (parentTab != null)
        {
          parentTab.Text = $"Unit {textBox.Text}";
        }
      }
    }
  }
}