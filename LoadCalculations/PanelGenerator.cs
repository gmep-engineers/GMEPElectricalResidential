using GMEPElectricalResidential.LoadCalculations.Unit;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace GMEPElectricalResidential.LoadCalculations
{
  public partial class PanelGenerator : Form
  {
    private FlowLayoutPanel flowLayoutPanel;
    private List<UnitInformation> selectedUnits;

    public PanelGenerator(List<UnitInformation> units)
    {
      InitializeComponent();
      selectedUnits = units;
      SetupCustomLoadsPanel();
      SetupFlowLayoutPanel();
      PopulateFlowLayoutPanel();
    }

    private void SetupCustomLoadsPanel()
    {
      PANEL_CUSTOM_LOADS.AutoScroll = true;
      PANEL_CUSTOM_LOADS.HorizontalScroll.Enabled = false;
      PANEL_CUSTOM_LOADS.HorizontalScroll.Visible = false;
      PANEL_CUSTOM_LOADS.HorizontalScroll.Maximum = 0;
      PANEL_CUSTOM_LOADS.AutoScrollMinSize = new Size(0, 0);
    }

    private void SetupFlowLayoutPanel()
    {
      flowLayoutPanel = new FlowLayoutPanel
      {
        Dock = DockStyle.Top,
        FlowDirection = FlowDirection.TopDown,
        WrapContents = false,
        AutoSize = true,
        AutoSizeMode = AutoSizeMode.GrowAndShrink
      };
      PANEL_CUSTOM_LOADS.Controls.Add(flowLayoutPanel);
    }

    private void PopulateFlowLayoutPanel()
    {
      foreach (var unit in selectedUnits)
      {
        foreach (var customLoad in unit.CustomLoads)
        {
          var itemText = $"{unit.Name} - {customLoad.Name}";
          var itemControl = new ItemSelectionControl
          {
            ItemText = itemText,
            Width = PANEL_CUSTOM_LOADS.ClientSize.Width - SystemInformation.VerticalScrollBarWidth - 5,
            Margin = new Padding(0, 0, 0, 5)  // Add some vertical spacing between items
          };
          flowLayoutPanel.Controls.Add(itemControl);
        }
      }
    }

    protected override void OnResize(EventArgs e)
    {
      base.OnResize(e);
      ResizeItemControls();
    }

    private void ResizeItemControls()
    {
      if (flowLayoutPanel != null)
      {
        foreach (Control control in flowLayoutPanel.Controls)
        {
          control.Width = PANEL_CUSTOM_LOADS.ClientSize.Width - SystemInformation.VerticalScrollBarWidth - 5;
        }
      }
    }
  }
}