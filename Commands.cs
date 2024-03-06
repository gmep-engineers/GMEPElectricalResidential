using Autodesk.AutoCAD.Runtime;
using GMEPElectricalResidential.LoadCalculations;
using GMEPElectricalResidential.SingleLineDiagram;

namespace GMEPElectricalResidential
{
  public class Commands
  {
    private static LOAD_CALCULATION_FORM _loadCalculationForm;
    private static SINGLE_LINE_DIAGRAM _sld;

    [CommandMethod("SLD")]
    public void SLD()
    {
      if (_sld == null || _sld.IsDisposed)
      {
        _sld = new SINGLE_LINE_DIAGRAM();
      }

      _sld.Show();
      _sld.BringToFront();
    }

    [CommandMethod("LoadCalculation")]
    public void LoadCalculation()
    {
      if (_loadCalculationForm == null || _loadCalculationForm.IsDisposed)
      {
        _loadCalculationForm = new LOAD_CALCULATION_FORM(this);
      }

      _loadCalculationForm.Show();
      _loadCalculationForm.BringToFront();
    }
  }
}