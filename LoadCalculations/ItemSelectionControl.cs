using System;
using System.Windows.Forms;

namespace GMEPElectricalResidential.LoadCalculations
{
  public partial class ItemSelectionControl : UserControl
  {
    public string ItemText
    {
      get { return lblItem.Text; }
      set { lblItem.Text = value; }
    }

    public ItemSelectionControl()
    {
      InitializeComponent();
    }
  }
}