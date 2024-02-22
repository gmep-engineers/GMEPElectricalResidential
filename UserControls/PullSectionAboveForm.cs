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
  public partial class PullSectionAboveForm : UserControl
  {
    public PullSectionAboveForm(Point location)
    {
      Location = location;
      InitializeComponent();
    }
  }
}