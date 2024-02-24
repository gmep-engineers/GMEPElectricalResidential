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
  public partial class MainForm : UserControl
  {
    private string amperageWaterMark = "Enter amperage...";

    public MainForm(Point location)
    {
      Location = location;
      InitializeComponent();
      SetWaterMarkText();
      CONFIGURATION.SelectedIndex = 0;
      KAIC.SelectedIndex = 1;
      AMPERAGE.Enter += new EventHandler(AMPERAGE_Enter);
      AMPERAGE.Leave += new EventHandler(AMPERAGE_Leave);
    }

    private void SetWaterMarkText()
    {
      AMPERAGE.Text = amperageWaterMark;
      AMPERAGE.ForeColor = Color.Gray;
    }

    private void AMPERAGE_Enter(object sender, EventArgs e)
    {
      if (AMPERAGE.Text == amperageWaterMark)
      {
        AMPERAGE.Text = "";
        AMPERAGE.ForeColor = Color.Black;
      }
    }

    private void AMPERAGE_Leave(object sender, EventArgs e)
    {
      if (AMPERAGE.Text.Length == 0)
      {
        SetWaterMarkText();
      }
    }
  }
}