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

    public int SelectedOption
    {
      get { return rbOption1.Checked ? 1 : 2; }
      set
      {
        if (value == 1)
          rbOption1.Checked = true;
        else if (value == 2)
          rbOption2.Checked = true;
      }
    }

    public ItemSelectionControl()
    {
      InitializeComponent();
    }
  }
}