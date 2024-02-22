using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace GMEPElectricalResidential
{
  public partial class MeterComboForm : UserControl
  {
    private string panelWaterMark = "Enter panel name...";
    private string sizeWaterMark = "Enter panel size...";
    private string amperageWaterMark = "Enter amperage...";

    public MeterComboForm(Point location)
    {
      Location = location;
      InitializeComponent();
      SetWatermarkText();

      CONFIGURATION.SelectedIndex = 0;
      KAIC.SelectedIndex = 1;

      PANEL_NAME.Enter += PANEL_NAME_Enter;
      PANEL_NAME.Leave += PANEL_NAME_Leave;
      PANEL_SIZE.Enter += PANEL_SIZE_Enter;
      PANEL_SIZE.Leave += PANEL_SIZE_Leave;
      AMPERAGE.Enter += AMPERAGE_Enter;
      AMPERAGE.Leave += AMPERAGE_Leave;
    }

    private void SetWatermarkText()
    {
      PANEL_NAME.ForeColor = Color.Gray;
      PANEL_NAME.Text = panelWaterMark;
      PANEL_SIZE.ForeColor = Color.Gray;
      PANEL_SIZE.Text = sizeWaterMark;
      AMPERAGE.ForeColor = Color.Gray;
      AMPERAGE.Text = amperageWaterMark;
    }

    // TextBox Enter event handler
    private void PANEL_NAME_Enter(object sender, EventArgs e)
    {
      if (PANEL_NAME.Text == panelWaterMark)
      {
        PANEL_NAME.Text = "";
        PANEL_NAME.ForeColor = Color.Black;
      }
    }

    // TextBox Leave event handler
    private void PANEL_NAME_Leave(object sender, EventArgs e)
    {
      if (string.IsNullOrWhiteSpace(PANEL_NAME.Text))
      {
        SetWatermarkText();
      }
    }

    // TextBox Enter event handler
    private void PANEL_SIZE_Enter(object sender, EventArgs e)
    {
      if (PANEL_SIZE.Text == sizeWaterMark)
      {
        PANEL_SIZE.Text = "";
        PANEL_SIZE.ForeColor = Color.Black;
      }
    }

    // TextBox Leave event handler
    private void PANEL_SIZE_Leave(object sender, EventArgs e)
    {
      if (string.IsNullOrWhiteSpace(PANEL_SIZE.Text))
      {
        SetWatermarkText();
      }
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
        SetWatermarkText();
      }
    }
  }
}