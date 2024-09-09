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
    private ToolTip _toolTip;
    public PanelGenerator(List<UnitInformation> units)
    {
      InitializeComponent();
      _toolTip = new ToolTip();
      MP_BREAKERS.KeyPress += OnlyDigitInputs;
      MP_BREAKERS.KeyUp += UpdateBreakersAvailableEvent;
      MP_BREAKERS.SelectedIndexChanged += UpdateBreakersAvailableEvent;
      SUB_BREAKERS.KeyPress += OnlyDigitInputs;
      SUB_BREAKERS.KeyPress += UpdateBreakersAvailableEvent;
      SUB_BREAKERS.SelectedIndexChanged += UpdateBreakersAvailableEvent;
      selectedUnits = units;
      SetupCustomLoadsPanel();
      SetupFlowLayoutPanel();
      PopulateFlowLayoutPanel();
      PopulatePanelBreakers(0);
    }

    private void PopulatePanelBreakers(int breakersAvailable)
    {
      PANEL_BREAKERS.AutoScroll = true;
      PANEL_BREAKERS.HorizontalScroll.Enabled = false;
      PANEL_BREAKERS.HorizontalScroll.Visible = false;
      PANEL_BREAKERS.HorizontalScroll.Maximum = 0;
      PANEL_BREAKERS.AutoScrollMinSize = new Size(0, 0);

      for (int i = 0; PANEL_BREAKERS.Controls.Count > 0; i++)
      {
        PANEL_BREAKERS.Controls.RemoveAt(i);
      }

      var flowLayoutPanel = new FlowLayoutPanel
      {
        Dock = DockStyle.Top,
        FlowDirection = FlowDirection.TopDown,
        WrapContents = false,
        AutoSize = true,
        AutoSizeMode = AutoSizeMode.GrowAndShrink
      };
      PANEL_BREAKERS.Controls.Add(flowLayoutPanel);

      foreach (var unit in selectedUnits)
      {
        var label = new Label
        {
          Text = $"{unit.Name} - 0/{breakersAvailable}",
          AutoSize = true,
          Margin = new Padding(0, 0, 0, 5)
        };
        flowLayoutPanel.Controls.Add(label);
      }
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
      ResizePanelBreakerLabels();
    }

    private void ResizePanelBreakerLabels()
    {
      if (PANEL_BREAKERS.Controls.Count > 0 && PANEL_BREAKERS.Controls[0] is FlowLayoutPanel flowPanel)
      {
        foreach (Control control in flowPanel.Controls)
        {
          control.Width = PANEL_BREAKERS.ClientSize.Width - SystemInformation.VerticalScrollBarWidth - 5;
        }
      }
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

		private void OnlyDigitInputs(object sender, KeyPressEventArgs e)
		{
			if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
			{
				e.Handled = true;
				_toolTip.Show("Input must be a digit", (Control)sender, 0, -20, 2000);
			}
		}

    private void UpdateBreakersAvailableEvent(object sender, EventArgs e)
    {
      UpdateBreakersAvailable(SUBPANELS.Items.Count);
    }

    private void UpdateBreakersAvailable(int numSubpanels)
    {
      int numBreakersMainPanel = 0;
      int breakersAvailable = 0;
      Int32.TryParse(MP_BREAKERS.Text, out numBreakersMainPanel);
      breakersAvailable = numBreakersMainPanel - (2 * numSubpanels);
      for (int i = 0; i < numSubpanels; i++)
      {
        int numSubPanelBreakers = 0;
        if (Int32.TryParse(SUBPANELS.Items[i].ToString(), out numSubPanelBreakers))
        {
          breakersAvailable += numSubPanelBreakers;
        }
      }

      PopulatePanelBreakers(breakersAvailable);
    }

    private void ADD_BUTTON_Click(object sender, EventArgs e)
    {
      int subBreakers = 0;
      if (Int32.TryParse(SUB_BREAKERS.Text, out subBreakers) && subBreakers > 0)
      {
        SUBPANELS.Items.Add(subBreakers);
        UpdateBreakersAvailable(SUBPANELS.Items.Count);
      }
    }

    private void REMOVE_BUTTON_Click(object sender, EventArgs e)
    {
      if (SUBPANELS.SelectedIndex != -1)
      {
        SUBPANELS.Items.RemoveAt(SUBPANELS.SelectedIndex);
        UpdateBreakersAvailable(SUBPANELS.Items.Count);
      }
    }
  }
}