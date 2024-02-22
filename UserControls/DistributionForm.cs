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
  public partial class DistributionForm : UserControl
  {
    private string distributionWaterMark = "Enter dist name...";
    private int tabCount = 0;

    public DistributionForm(Point location)
    {
      Location = location;
      InitializeComponent();
      SetWatermarkText();
      CreateTab();

      CONFIGURATION.SelectedIndex = 0;
      KAIC.SelectedIndex = 1;
      SIZE.SelectedIndex = 0;

      DISTRIBUTION_NAME.Enter += PANEL_NAME_Enter;
      DISTRIBUTION_NAME.Leave += PANEL_NAME_Leave;
    }

    private void CreateTab()
    {
      tabCount++;

      TabPage tab = new TabPage
      {
        Text = "Item" + tabCount.ToString()
      };

      var tabUserControl = new ItemTab();

      tab.Controls.Add(tabUserControl);
      TABS.TabPages.Add(tab);
    }

    private void SetWatermarkText()
    {
      DISTRIBUTION_NAME.ForeColor = Color.Gray;
      DISTRIBUTION_NAME.Text = distributionWaterMark;
    }

    // TextBox Enter event handler
    private void PANEL_NAME_Enter(object sender, EventArgs e)
    {
      if (DISTRIBUTION_NAME.Text == distributionWaterMark)
      {
        DISTRIBUTION_NAME.Text = "";
        DISTRIBUTION_NAME.ForeColor = Color.Black;
      }
    }

    // TextBox Leave event handler
    private void PANEL_NAME_Leave(object sender, EventArgs e)
    {
      if (string.IsNullOrWhiteSpace(DISTRIBUTION_NAME.Text))
      {
        SetWatermarkText();
      }
    }
  }
}