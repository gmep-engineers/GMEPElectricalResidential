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
  public partial class ItemTab : UserControl
  {
    private string waterMark = "Enter name...";

    public ItemTab()
    {
      InitializeComponent();
      SetWatermarkText();

      TYPE.SelectedIndex = 2;
      BREAKER_SIZES.SelectedIndex = 8;
      XFMR.SelectedIndex = 0;

      NAME.Enter += NAME_Enter;
      NAME.Leave += NAME_Leave;
    }

    private void SetWatermarkText()
    {
      NAME.ForeColor = Color.Gray;
      NAME.Text = waterMark;
    }

    private void TYPE_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (TYPE.SelectedIndex == 0)
      {
        NAME.Enabled = false;
      }
      else
      {
        NAME.Enabled = true;
      }
    }

    private void NEW_Click(object sender, EventArgs e)
    {
    }

    private void NAME_Enter(object sender, EventArgs e)
    {
      if (NAME.Text == waterMark)
      {
        NAME.Text = "";
        NAME.ForeColor = Color.Black;
      }
    }

    private void NAME_Leave(object sender, EventArgs e)
    {
      if (NAME.Text == "")
      {
        SetWatermarkText();
      }
    }
  }
}