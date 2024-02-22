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
    public ItemTab()
    {
      InitializeComponent();
      TYPE.SelectedIndex = 0;
      BREAKER_SIZES.SelectedIndex = 0;
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
  }
}