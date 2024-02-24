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
  public partial class PullSectionBelowForm : UserControl
  {
    private string amperageWaterMark = "Enter amperage...";

    public PullSectionBelowForm(Point location)
    {
      Location = location;
      InitializeComponent();
      SetWatermarkText();
      AMPERAGE.Enter += AMPERAGE_Enter;
      AMPERAGE.Leave += AMPERAGE_Leave;
    }

    private void SetWatermarkText()
    {
      AMPERAGE.ForeColor = Color.Gray;
      AMPERAGE.Text = amperageWaterMark;
    }

    // TextBox Enter event handler
    private void AMPERAGE_Enter(object sender, EventArgs e)
    {
      if (AMPERAGE.Text == amperageWaterMark)
      {
        AMPERAGE.Text = "";
        AMPERAGE.ForeColor = Color.Black;
      }
    }

    // TextBox Leave event handler
    private void AMPERAGE_Leave(object sender, EventArgs e)
    {
      if (string.IsNullOrWhiteSpace(AMPERAGE.Text))
      {
        AMPERAGE.Text = amperageWaterMark;
        AMPERAGE.ForeColor = Color.Gray;
      }
    }
  }
}