using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;

namespace GMEPElectricalResidential
{
  public class Commands
  {
    [CommandMethod("SLD")]
    public void SLD()
    {
      SINGLE_LINE_DIAGRAM sld = new SINGLE_LINE_DIAGRAM();
      sld.Show();
    }

    [CommandMethod("LoadCalculation")]
    public void LoadCalculation()
    {
    }
  }
}