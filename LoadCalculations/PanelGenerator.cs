using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace GMEPElectricalResidential.LoadCalculations
{
  public partial class PanelGenerator : Form
  {
    private FlowLayoutPanel flowLayoutPanel;

    private List<string> items = new List<string>
    {
        "Exterior Lighting",
        "Exterior Receptacle",
        "Gas Receptacle",
        "Car Wash",
        "EV Charger",
        "Sauna"
    };

    public PanelGenerator()
    {
      InitializeComponent();
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
      foreach (string item in items)
      {
        var itemControl = new ItemSelectionControl
        {
          ItemText = item,
          Width = PANEL_CUSTOM_LOADS.ClientSize.Width - SystemInformation.VerticalScrollBarWidth - 5,
          Margin = new Padding(0, 0, 0, 5)  // Add some vertical spacing between items
        };
        flowLayoutPanel.Controls.Add(itemControl);
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