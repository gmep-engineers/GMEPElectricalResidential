using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using GMEPElectricalResidential.LoadCalculations.Unit;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace GMEPElectricalResidential.LoadCalculations
{
  public struct Panel
  {
    public bool Is3Ph { get; set; }
    public int NumBreakers { get; set; }
    public List<string> Descriptions { get; set; }
    public List<int> Loads { get; set; }

    public Panel(bool is3Ph, int numBreakers)
    {
      Is3Ph = is3Ph;
      NumBreakers = numBreakers;
      Descriptions = new List<string>();
      Loads = new List<int>();
    }
  }
  public partial class PanelGenerator : Form
  {
    private FlowLayoutPanel flowLayoutPanel;
    private List<UnitInformation> selectedUnits;
    private ToolTip _toolTip;
    private Document acDoc;
    public PanelGenerator(List<UnitInformation> units)
    {
      this.acDoc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
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
        FlowDirection = System.Windows.Forms.FlowDirection.TopDown,
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
        FlowDirection = System.Windows.Forms.FlowDirection.TopDown,
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

    public void StoreDataInJsonFile(List<Dictionary<string, object>> saveData)
    {
      string acDocPath = Path.GetDirectoryName(this.acDoc.Name);
      string savesDirectory = Path.Combine(acDocPath, "Saves");
      string panelSavesDirectory = Path.Combine(savesDirectory, "Panel");

      // Create the "Saves" directory if it doesn't exist
      if (!Directory.Exists(savesDirectory))
      {
        Directory.CreateDirectory(savesDirectory);
      }

      // Create the "Saves/Panel" directory if it doesn't exist
      if (!Directory.Exists(panelSavesDirectory))
      {
        Directory.CreateDirectory(panelSavesDirectory);
      }

      // Create a JSON file name based on the AutoCAD file name and the current timestamp
      string acDocFileName = Path.GetFileNameWithoutExtension(acDoc.Name);
      string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
      string jsonFileName = acDocFileName + "_" + timestamp + ".json";
      string jsonFilePath = Path.Combine(panelSavesDirectory, jsonFileName);

      // Serialize all the panel data to JSON
      string jsonData = JsonConvert.SerializeObject(saveData, Formatting.Indented);

      // Write the JSON data to the file
      File.WriteAllText(jsonFilePath, jsonData);
    }

    private string GetNextPhase(string currentPhase, int poles, bool is3Ph = false)
    {
      currentPhase = currentPhase.ToUpper();
      if (is3Ph)
      {
        if (poles == 2)
        {
          if (currentPhase == "A")
          {
            return "C";
          }
          if (currentPhase == "B")
          {
            return "A";
          }
          if (currentPhase == "C")
          {
            return "B";
          }
        }
        if (poles == 1)
        {
          if (currentPhase == "A")
          {
            return "B";
          }
          if (currentPhase == "B")
          {
            return "C";
          }
          if (currentPhase == "C")
          {
            return "A";
          }
        }
      }
      else
      {
        if (poles == 1)
        {
          if (currentPhase == "A")
          {
            return "B";
          }
          if (currentPhase == "B")
          {
            return "A";
          }
        }
      }
      return currentPhase;
    }

    private string GetBreakerSize(int load)
    {
      if (load / 120 * 1.25 > 20)
      {
        double mocp = Math.Round(load / 120 * 1.25, 0);
        switch (mocp)
        {
          case var _ when mocp <= 25:
            return "25";
          case var _ when mocp <= 30:
            return "30";
          case var _ when mocp <= 35:
            return "35";
          case var _ when mocp <= 40:
            return "40";
          case var _ when mocp <= 45:
            return "45";
          case var _ when mocp <= 50:
            return "50";
          case var _ when mocp <= 60:
            return "60";
          case var _ when mocp <= 70:
            return "70";
          case var _ when mocp <= 80:
            return "80";
          case var _ when mocp <= 90:
            return "90";
          case var _ when mocp <= 100:
            return "100";
          case var _ when mocp <= 110:
            return "110";
          case var _ when mocp <= 125:
            return "125";
          case var _ when mocp <= 150:
            return "150";
          case var _ when mocp <= 175:
            return "175";
          case var _ when mocp <= 200:
            return "200";
          case var _ when mocp <= 225:
            return "225";
          case var _ when mocp <= 250:
            return "250";
          case var _ when mocp <= 300:
            return "300";
          case var _ when mocp <= 350:
            return "350";
          case var _ when mocp <= 400:
            return "400";
          case var _ when mocp <= 450:
            return "450";
          case var _ when mocp <= 500:
            return "500";
          case var _ when mocp <= 600:
            return "600";
        }
      }
      return "20";
    }

    private void GENERATE_PANEL_Click(object sender, EventArgs e)
    {
      // bug: setting 42 breaker panel created panel with 36 breakers for ADU

      foreach (var unit in selectedUnits)
      {
        int currentPanel = -1;
        int numSubpanels = SUBPANELS.Items.Count;
        int numBreakers = 0;
        Int32.TryParse(MP_BREAKERS.Text, out numBreakers);
        Panel mainPanel = new Panel(false, numBreakers);
        List<Panel> subpanels = new List<Panel>();
        for (int i = 0; i < numSubpanels; i++) {
          Panel subpanel = new Panel();
          subpanels.Add(subpanel);
        }

        int breakerIndex = 0;
        mainPanel.NumBreakers = numBreakers;
        List < UnitLoad > loads = new List<UnitLoad>();


        // add unit.GeneralLoads.Lighting to panel
        foreach (var generalLoad in unit.GeneralLoads.Customs)
        {
          loads.Add(generalLoad);
        }
        foreach (var customLoad in unit.CustomLoads)
        {
          loads.Add(customLoad);
        }
        foreach (var load in loads)
        {
          if (currentPanel == -1)
          {
            if (breakerIndex >= mainPanel.NumBreakers - (subpanels.Count * 2) - (load.Total > 1800 ? 1 : 0))
            {
              currentPanel++;
              breakerIndex = 0;
            }
          }
          if (currentPanel == -1)
          {
            // add to main panel
            mainPanel.Descriptions.Add(load.Name);
            mainPanel.Loads.Add(load.Total);
            if (load.Total > 1800)
            {
              breakerIndex += 2;
            }
            else
            {
              breakerIndex += 1;
            }
          }
          else
          {
            // add to subpanel at index currentPanel
            if (breakerIndex >= subpanels[currentPanel].NumBreakers - (load.Total > 1800 ? 1 : 0))
            {
              currentPanel++;
              breakerIndex = 0;
            }
            subpanels[currentPanel].Descriptions.Add(load.Name);
            subpanels[currentPanel].Loads.Add(load.Total);
          }
        }
        // iterate through panels and separate by right and left
        List<string> descLeft = new List<string>();
        List<string> descRight = new List<string>();

        List<int> loadsLeft = new List<int>();
        List<int> loadsRight = new List<int>();

        breakerIndex = 0;
        for (int i = 0; i < mainPanel.Loads.Count; i++) 
        {
          {
            if (mainPanel.Loads[i] > 1800) // add 2-pole equip
            {
              if (breakerIndex >= (mainPanel.NumBreakers / 2) - 1)
              {
                // right side
                descRight.Add(mainPanel.Descriptions[i]);
                loadsRight.Add(mainPanel.Loads[i]);
              }
              else
              {
                // left side
                descLeft.Add(mainPanel.Descriptions[i]);
                loadsLeft.Add(mainPanel.Loads[i]);
              }
              breakerIndex += 2;
            }
          }
        }
        for (int i = 0; i < mainPanel.Loads.Count; i++)
        {
          {
            if (mainPanel.Loads[i] <= 1800) // add 1-pole equip
            {
              if (breakerIndex >= (mainPanel.NumBreakers / 2))
              {
                // right side
                descRight.Add(mainPanel.Descriptions[i]);
                loadsRight.Add(mainPanel.Loads[i]);
              }
              else
              {
                // left side
                descLeft.Add(mainPanel.Descriptions[i]);
                loadsLeft.Add(mainPanel.Loads[i]);
              }
              breakerIndex++;
            }
          }
        }
        // create json
        List<Dictionary<string, object>> panelStorage = new List<Dictionary<string, object>>();
        Dictionary<string, object> panel = new Dictionary<string, object>();
        panel.Add("main", "100 AMP");
        panel.Add("panel", "'MP'");
        panel.Add("location", "STORAGE");
        panel.Add("voltage1", "120");
        panel.Add("voltage2", "240");
        panel.Add("phase", "1");
        panel.Add("wire", "3");
        panel.Add("mounting", "RECESSED");
        panel.Add("existing", "EXISTING");
        panel.Add("lcl_override", false);
        panel.Add("lml_override", false);
        panel.Add("subtotal_a", "0");
        panel.Add("subtotal_b", "0");
        panel.Add("subtotal_c", "0");
        panel.Add("total_va", "0");
        panel.Add("lcl", 0.0);
        panel.Add("lcl125", 0.0);
        panel.Add("lml", 0.0);
        panel.Add("lml125", 0.0);
        panel.Add("kva", "0");
        panel.Add("feeder_amps", "0");
        panel.Add("custom_title", "");
        panel.Add("bus_rating", "100A");
        List<bool> description_left_highlights = new List<bool>();
        List<bool> description_right_highlights = new List<bool>();
        List<bool> breaker_left_highlights = new List<bool>();
        List<bool> breaker_right_highlights = new List<bool>();
        List<string> description_left = new List<string>();
        List<string> description_right = new List<string>();
        List<string> phase_a_left = new List<string>();
        List<string> phase_b_left = new List<string>();
        List<string> phase_a_right = new List<string>();
        List<string> phase_b_right = new List<string>();
        List<string> phase_c_left = new List<string>();
        List<string> phase_c_right = new List<string>();
        List<string> breaker_left = new List<string>();
        List<string> breaker_right = new List<string>();
        List<string> circuit_left = new List<string>();
        List<string> circuit_right = new List<string>();
        List<string> notes = new List<string>();
        for (int i = 0; i < numBreakers; i++)
        {
          description_left_highlights.Add(false);
          description_right_highlights.Add(false);
          breaker_left_highlights.Add(false);
          breaker_right_highlights.Add(false);
          if (i % 2 == 0)
          {
            circuit_left.Add((i + 1).ToString());
            circuit_right.Add((i + 2).ToString());
          }
          else
          {
            circuit_left.Add("");
            circuit_right.Add("");
          }
        }
        string currentPhase = "A";
        for (int i = 0; i < loadsLeft.Count; i++)
        {
          description_left.Add(descLeft[i]);
          description_left.Add("SPACE");
          if (loadsLeft[i] > 1800)
          {
            description_left.Add("---");
            description_left.Add("SPACE");
            if (currentPhase == "A")
            {
              breaker_left.Add(GetBreakerSize(loadsLeft[i]));
              breaker_left.Add("");
              breaker_left.Add("2");
              breaker_left.Add("");

              phase_a_left.Add(loadsLeft[i].ToString());
              phase_a_left.Add("0");
              phase_a_left.Add("0");
              phase_a_left.Add("0");

              phase_b_left.Add("0");
              phase_b_left.Add("0");
              phase_b_left.Add(loadsLeft[i].ToString());
              phase_b_left.Add("0");
              if (mainPanel.Is3Ph)
              {
                phase_c_left.Add("0");
                phase_c_left.Add("0");
                phase_c_left.Add("0");
                phase_c_left.Add("0");
              }
            }
            if (currentPhase == "B")
            {
              breaker_left.Add(GetBreakerSize(loadsLeft[i]));
              breaker_left.Add("");
              breaker_left.Add("2");
              breaker_left.Add("");

              phase_b_left.Add(loadsLeft[i].ToString());
              phase_b_left.Add("0");
              phase_b_left.Add("0");
              phase_b_left.Add("0");
              if (mainPanel.Is3Ph)
              {
                phase_c_left.Add("0");
                phase_c_left.Add("0");
                phase_c_left.Add(loadsLeft[i].ToString());
                phase_c_left.Add("0");

                phase_b_left.Add("0");
                phase_b_left.Add("0");
                phase_b_left.Add("0");
                phase_b_left.Add("0");
              }
              else
              {
                phase_a_right.Add("0");
                phase_a_right.Add("0");
                phase_a_left.Add(loadsLeft[i].ToString());
                phase_a_left.Add("0");
              }
              
            }
            if (currentPhase == "C")
            {
              phase_c_left.Add(loadsLeft[i].ToString());
              phase_c_left.Add("0");
              phase_c_left.Add("0");
              phase_c_left.Add("0");

              phase_a_left.Add("0");
              phase_a_left.Add("0");
              phase_a_left.Add(loadsLeft[i].ToString());
              phase_a_left.Add("0");

              phase_b_left.Add("0");
              phase_b_left.Add("0");
              phase_b_left.Add("0");
              phase_b_left.Add("0");
            }
            currentPhase = GetNextPhase(currentPhase, 2, mainPanel.Is3Ph);
          }
          else
          {
            if (currentPhase == "A")
            {
              breaker_left.Add(GetBreakerSize(loadsLeft[i]));
              breaker_left.Add("");

              phase_a_left.Add(loadsLeft[i].ToString());
              phase_a_left.Add("0");

              phase_b_left.Add("0");
              phase_b_left.Add("0");
            }
            if (currentPhase == "B")
            {
              breaker_left.Add(GetBreakerSize(loadsLeft[i]));
              breaker_left.Add("");

              phase_b_left.Add(loadsLeft[i].ToString());
              phase_b_left.Add("0");

              phase_a_left.Add("0");
              phase_a_left.Add("0");
            }
            if (currentPhase == "C")
            {
              phase_c_left.Add(loadsLeft[i].ToString());
              phase_c_left.Add("0");
            }
            currentPhase = GetNextPhase(currentPhase, 1, mainPanel.Is3Ph);
          }
        }
        currentPhase = "A";
        for (int i = 0; i < loadsRight.Count; i++)
        {
          description_right.Add(descRight[i]);
          description_right.Add("SPACE");
          if (loadsRight[i] > 1800)
          {
            description_right.Add("---");
            description_right.Add("SPACE");
            if (currentPhase == "A")
            {
              breaker_right.Add(GetBreakerSize(loadsRight[i]));
              breaker_right.Add("");
              breaker_right.Add("2");
              breaker_right.Add("");

              phase_a_right.Add(loadsRight[i].ToString());
              phase_a_right.Add("0");
              phase_a_right.Add("0");
              phase_a_right.Add("0");

              phase_b_right.Add("0");
              phase_b_right.Add("0");
              phase_b_right.Add(descRight[i]);
              phase_b_right.Add("0");
            }
            if (currentPhase == "B")
            {
              breaker_right.Add(GetBreakerSize(loadsRight[i]));
              breaker_right.Add("");
              breaker_right.Add("2");
              breaker_right.Add("");

              phase_b_right.Add(loadsRight[i].ToString());
              phase_b_right.Add("0");
              phase_b_right.Add("0");
              phase_b_right.Add("0");
              if (mainPanel.Is3Ph)
              {
                phase_c_right.Add("0");
                phase_c_right.Add("0");
                phase_c_right.Add(loadsRight[i].ToString());
                phase_c_right.Add("0");
              }
              else
              {
                phase_a_right.Add("0");
                phase_a_right.Add("0");
                phase_a_right.Add(loadsRight[i].ToString());
                phase_a_right.Add("0");
              }
            }
            if (currentPhase == "C")
            {
              phase_c_right.Add(loadsRight[i].ToString());
              phase_c_right.Add("0");
              phase_c_right.Add("0");
              phase_c_right.Add("0");

              phase_a_right.Add("0");
              phase_a_right.Add("0");
              phase_a_right.Add(loadsRight[i].ToString());
              phase_a_right.Add("0");
            }
            currentPhase = GetNextPhase(currentPhase, 2, mainPanel.Is3Ph);
          }
          else
          {
            if (currentPhase == "A")
            {
              breaker_right.Add(GetBreakerSize(loadsRight[i]));
              breaker_right.Add("");

              phase_a_right.Add(loadsRight[i].ToString());
              phase_a_right.Add("0");

              phase_b_right.Add("0");
              phase_b_right.Add("0"); 
              if (mainPanel.Is3Ph)
              {
                phase_a_right.Add("0");
                phase_a_right.Add("0");

                phase_b_right.Add("0");
                phase_b_right.Add("0");
              }
            }
            if (currentPhase == "B")
            {
              breaker_right.Add(GetBreakerSize(loadsRight[i]));
              breaker_right.Add("");

              phase_b_right.Add(loadsRight[i].ToString());
              phase_b_right.Add("0");
              if (mainPanel.Is3Ph)
              {
                phase_c_right.Add("0");
                phase_c_right.Add("0");
              } else {
                phase_a_right.Add("0");
                phase_a_right.Add("0");
              }
            }
            if (currentPhase == "C")
            {
              breaker_right.Add(GetBreakerSize(loadsRight[i]));
              breaker_right.Add("");

              phase_c_right.Add(loadsRight[i].ToString());
              phase_c_right.Add("0");

              phase_a_right.Add("0");
              phase_a_right.Add("0");

              phase_b_right.Add("0");
              phase_b_right.Add("0");
            }
            currentPhase = GetNextPhase(currentPhase, 1, mainPanel.Is3Ph);
          }
        }
        if (mainPanel.NumBreakers > description_left.Count / 2)
        {
          for (int i = 0; i < description_left.Count / 2 - mainPanel.NumBreakers; i++)
          {
            // add remaining lines to the panel, otherwise there won't be enough spaces.
          }
        }
        List<string> phase_a_left_tag = new List<string>();
        List<string> phase_b_left_tag = new List<string>();
        List<string> phase_a_right_tag = new List<string>();
        List<string> phase_b_right_tag = new List<string>();
        List<string> phase_c_left_tag = new List<string>();
        List<string> phase_c_right_tag = new List<string>();
        List<string> description_left_tags = new List<string>();
        List<string> description_right_tags = new List<string>();
        panel.Add("description_left", description_left);
        panel.Add("description_right", description_right);
        panel.Add("phase_a_left", phase_a_left);
        panel.Add("phase_a_right", phase_a_right);
        panel.Add("phase_b_left", phase_b_left);
        panel.Add("phase_b_right", phase_b_right);
        if (mainPanel.Is3Ph)
        {
          panel.Add("phase_c_left", phase_c_left);
          panel.Add("phase_c_right", phase_c_right);
          panel.Add("phase_c_left_tag", phase_c_left_tag);
          panel.Add("phase_c_right_tag", phase_c_right_tag);
        }
        panel.Add("phase_a_left_tag", phase_a_left_tag);
        panel.Add("phase_b_left_tag", phase_b_left_tag);
        panel.Add("phase_a_right_tag", phase_a_right_tag);
        panel.Add("phase_b_right_tag", phase_b_right_tag);
        panel.Add("description_left_tags", description_left_tags);
        panel.Add("description_right_tags", description_right_tags);
        panel.Add("description_left_highlights", description_left_highlights);
        panel.Add("description_right_highlights", description_right_highlights);
        panel.Add("breaker_left_highlights", description_left_highlights);
        panel.Add("breaker_right_highlights", description_right_highlights);
        panel.Add("circuit_left", circuit_left);
        panel.Add("circuit_right", circuit_right);
        panel.Add("breaker_left", breaker_left);
        panel.Add("breaker_right", breaker_right);
        panel.Add("notes", notes);

        foreach (Panel subpanel in subpanels)
        {
          Dictionary<string, object> sp = new Dictionary<string, object>();
        }
        panelStorage.Add(panel);
        if (this.acDoc != null)
        {
          using (DocumentLock docLock = this.acDoc.LockDocument())
          {
            StoreDataInJsonFile(panelStorage);
          }
        }
      }
    }
  }
}
/*
 * 
 */