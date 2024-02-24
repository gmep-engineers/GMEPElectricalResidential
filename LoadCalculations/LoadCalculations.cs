using MaterialSkin;
using MaterialSkin.Controls;
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
  public partial class LoadCalculations : MaterialForm
  {
    public LoadCalculations()
    {
      InitializeComponent();

      // Initialize MaterialSkinManager
      var materialSkinManager = MaterialSkinManager.Instance;
      materialSkinManager.AddFormToManage(this);
      materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;

      // Choose a color scheme
      materialSkinManager.ColorScheme = new ColorScheme(
          Primary.Blue400, Primary.Blue500,
          Primary.Blue500, Accent.LightBlue200,
          TextShade.WHITE);
    }
  }
}