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
  public partial class MeterAndMainForm : UserControl
  {
    public MeterAndMainForm(Point location)
    {
      Location = location;
      InitializeComponent();
      CONFIGURATION.SelectedIndex = 0;
      KAIC.SelectedIndex = 1;
    }
  }
}