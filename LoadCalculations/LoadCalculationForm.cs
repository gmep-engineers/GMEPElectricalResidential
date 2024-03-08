using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using GMEPElectricalResidential.HelperFiles;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GMEPElectricalResidential.LoadCalculations
{
  public partial class LOAD_CALCULATION_FORM : Form
  {
    private int _UnitTabID = 0;
    private int _BuildingTabID = 0;
    private List<int> _unitCannotBeIDs;
    private List<int> _buildingCannotBeIDs;

    public Commands Commands { get; }

    public LOAD_CALCULATION_FORM(Commands commands)
    {
      Commands = commands;
      InitializeComponent();

      _unitCannotBeIDs = new List<int>();
      _buildingCannotBeIDs = new List<int>();

      bool createdUnitTab = LoadSavedUnitLoadCalculations();
      bool createdBuildingTab = LoadSavedBuildingLoadCalculations();

      if (!createdUnitTab)
      {
        AddNewUnitTab();
      }

      if (!createdBuildingTab)
      {
        AddNewBuildingTab();
      }

      this.FormClosing += LOAD_CALCULATION_FORM_FormClosing;
      this.TAB_CONTROL.SelectedIndexChanged += TAB_CONTROL_SelectedIndexChanged;
      this.BUILDING_TAB_CONTROL.SelectedIndexChanged += TAB_CONTROL_SelectedIndexChanged;
      this.UNIT_TAB_CONTROL.SelectedIndexChanged += TAB_CONTROL_SelectedIndexChanged;
    }

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
      if (keyData == (Keys.Control | Keys.S))
      {
        SaveLoadCalculationForm();
        return true;
      }

      return base.ProcessCmdKey(ref msg, keyData);
    }

    public void AddNewBuildingTab(Building.BuildingInformation buildingInformation = null)
    {
      TabPage tabPage;
      if (buildingInformation != null)
      {
        tabPage = new TabPage("Building " + buildingInformation.Name);
      }
      else
      {
        tabPage = new TabPage("Building");
      }

      while (_buildingCannotBeIDs.Contains(_BuildingTabID))
      {
        _BuildingTabID++;
      }

      _buildingCannotBeIDs.Add(_BuildingTabID);

      Building.LoadCalculationForm buildingLoadCalculation = new Building.LoadCalculationForm(this, _BuildingTabID, buildingInformation);
      tabPage.Tag = _BuildingTabID;
      tabPage.Controls.Add(buildingLoadCalculation);

      BUILDING_TAB_CONTROL.TabPages.Add(tabPage);
    }

    public void AddNewUnitTab(Unit.UnitInformation unitInformation = null)
    {
      TabPage tabPage;
      if (unitInformation != null)
      {
        tabPage = new TabPage(unitInformation.FormattedName());
      }
      else
      {
        tabPage = new TabPage($"Unit");
      }

      while (_unitCannotBeIDs.Contains(_UnitTabID))
      {
        _UnitTabID++;
      }

      _unitCannotBeIDs.Add(_UnitTabID);

      Unit.LoadCalculationForm unitLoadCalculation = new Unit.LoadCalculationForm(this, _UnitTabID, unitInformation);
      tabPage.Tag = _UnitTabID;
      tabPage.Controls.Add(unitLoadCalculation);

      UNIT_TAB_CONTROL.TabPages.Add(tabPage);
    }

    private bool LoadSavedUnitLoadCalculations()
    {
      var createdTabFlag = false;
      var doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
      string dwgDirectory = Path.GetDirectoryName(doc.Database.Filename);
      string baseSaveDirectory = Path.Combine(dwgDirectory, "Saves", "Load Calculations", "Unit");

      if (Directory.Exists(baseSaveDirectory))
      {
        var unitDirectories = Directory.GetDirectories(baseSaveDirectory);
        foreach (var unitDirectory in unitDirectories)
        {
          var jsonFiles = Directory.GetFiles(unitDirectory, "*.json");
          if (jsonFiles.Length > 0)
          {
            var latestJsonFile = jsonFiles.OrderByDescending(f => File.GetCreationTime(f)).First();
            var json = File.ReadAllText(latestJsonFile);
            var unitInformation = JsonConvert.DeserializeObject<Unit.UnitInformation>(json);

            if (unitInformation.Voltage != null)
            {
              AddNewUnitTab(unitInformation);
              _unitCannotBeIDs.Add(unitInformation.ID);
            }
            createdTabFlag = true;
          }
        }
      }
      return createdTabFlag;
    }

    private bool LoadSavedBuildingLoadCalculations()
    {
      var createdTabFlag = false;
      var doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
      string dwgDirectory = Path.GetDirectoryName(doc.Database.Filename);
      string baseSaveDirectory = Path.Combine(dwgDirectory, "Saves", "Load Calculations", "Building");

      if (Directory.Exists(baseSaveDirectory))
      {
        var buildingDirectories = Directory.GetDirectories(baseSaveDirectory);
        foreach (var buildingDirectory in buildingDirectories)
        {
          var jsonFiles = Directory.GetFiles(buildingDirectory, "*.json");
          if (jsonFiles.Length > 0)
          {
            var latestJsonFile = jsonFiles.OrderByDescending(f => File.GetCreationTime(f)).First();
            var json = File.ReadAllText(latestJsonFile);
            var buildingInformation = JsonConvert.DeserializeObject<Building.BuildingInformation>(json);

            if (buildingInformation.Name != null)
            {
              AddNewBuildingTab(buildingInformation);
              _buildingCannotBeIDs.Add(buildingInformation.ID);
            }
            createdTabFlag = true;
          }
        }
      }
      return createdTabFlag;
    }

    public void RemoveCurrentTab()
    {
      if (TAB_CONTROL.SelectedTab == UNIT_TAB)
      {
        if (UNIT_TAB_CONTROL.TabCount > 0)
        {
          DialogResult result = MessageBox.Show("Are you sure you want to remove this unit tab?", "Confirmation", MessageBoxButtons.YesNo);
          if (result == DialogResult.Yes)
          {
            UNIT_TAB_CONTROL.TabPages.Remove(UNIT_TAB_CONTROL.SelectedTab);

            var id = (int)UNIT_TAB_CONTROL.SelectedTab.Tag;
            var allBuildingInformation = AllBuildingInformation();
            foreach (var buildingInformation in allBuildingInformation)
            {
              var counterToRemove = buildingInformation.Counters.FirstOrDefault(counter => counter.UnitID == id);
              buildingInformation.Counters.Remove(counterToRemove);
            }
          }
        }
      }
      else if (TAB_CONTROL.SelectedTab == BUILDING_TAB)
      {
        if (BUILDING_TAB_CONTROL.TabCount > 0)
        {
          DialogResult result = MessageBox.Show("Are you sure you want to remove this building tab?", "Confirmation", MessageBoxButtons.YesNo);
          if (result == DialogResult.Yes)
          {
            BUILDING_TAB_CONTROL.TabPages.Remove(BUILDING_TAB_CONTROL.SelectedTab);
          }
        }
      }
    }

    public List<Unit.UnitInformation> AllUnitInformation()
    {
      List<Unit.UnitInformation> allUnitInformation = new List<Unit.UnitInformation>();

      for (int i = 0; i < UNIT_TAB_CONTROL.TabCount; i++)
      {
        var tabPage = UNIT_TAB_CONTROL.TabPages[i];
        var unitLoadCalculation = tabPage.Controls.OfType<Unit.LoadCalculationForm>().FirstOrDefault();

        if (unitLoadCalculation != null)
        {
          var unitInformation = unitLoadCalculation.RetrieveUnitInformation();
          allUnitInformation.Add(unitInformation);
        }
      }

      return allUnitInformation;
    }

    public List<Building.BuildingInformation> AllBuildingInformation()
    {
      List<Building.BuildingInformation> allBuildingInformation = new List<Building.BuildingInformation>();

      for (int i = 0; i < BUILDING_TAB_CONTROL.TabCount; i++)
      {
        var tabPage = BUILDING_TAB_CONTROL.TabPages[i];
        var buildingLoadCalculation = tabPage.Controls.OfType<Building.LoadCalculationForm>().FirstOrDefault();

        if (buildingLoadCalculation != null)
        {
          var buildingInformation = buildingLoadCalculation.RetrieveBuildingInformation();
          allBuildingInformation.Add(buildingInformation);
        }
      }

      return allBuildingInformation;
    }

    private void SaveLoadCalculationForm()
    {
      var doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
      string dwgDirectory = Path.GetDirectoryName(doc.Database.Filename);
      string baseSaveDirectory = Path.Combine(dwgDirectory, "Saves", "Load Calculations");

      // Delete the current Load Calculations directory if it exists
      if (Directory.Exists(baseSaveDirectory))
      {
        Directory.Delete(baseSaveDirectory, true);
      }

      // Create the Saves directory if it doesn't exist
      string savesDirectory = Path.Combine(dwgDirectory, "Saves");
      if (!Directory.Exists(savesDirectory))
      {
        Directory.CreateDirectory(savesDirectory);
      }

      // Create the Load Calculations directory inside the Saves directory
      Directory.CreateDirectory(baseSaveDirectory);

      // Create the Unit directory inside the Load Calculations directory
      string unitDirectory = Path.Combine(baseSaveDirectory, "Unit");
      Directory.CreateDirectory(unitDirectory);

      // Create the Building directory inside the Load Calculations directory
      string buildingDirectory = Path.Combine(baseSaveDirectory, "Building");
      Directory.CreateDirectory(buildingDirectory);

      List<Unit.UnitInformation> allUnitInformation = AllUnitInformation();
      HandleUnitDataSaving(unitDirectory, allUnitInformation);

      List<Building.BuildingInformation> allBuildingInformation = AllBuildingInformation();
      HandleBuildingDataSaving(buildingDirectory, allBuildingInformation);
    }

    private static void HandleUnitDataSaving(string unitDirectory, List<Unit.UnitInformation> allUnitInformation)
    {
      for (int i = 0; i < allUnitInformation.Count; i++)
      {
        var unitInformation = allUnitInformation[i];

        string saveDirectory = Path.Combine(unitDirectory, unitInformation.FormattedName());
        Directory.CreateDirectory(saveDirectory);

        string json = JsonConvert.SerializeObject(unitInformation, Formatting.Indented);
        string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
        string savePath = Path.Combine(saveDirectory, $"{timestamp}.json");
        File.WriteAllText(savePath, json);
      }
    }

    private static void HandleBuildingDataSaving(string buildingDirectory, List<Building.BuildingInformation> allBuildingInformation)
    {
      for (int i = 0; i < allBuildingInformation.Count; i++)
      {
        var buildingInformation = allBuildingInformation[i];

        string saveDirectory = Path.Combine(buildingDirectory, buildingInformation.FormattedName());
        Directory.CreateDirectory(saveDirectory);

        string json = JsonConvert.SerializeObject(buildingInformation, Formatting.Indented);
        string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
        string savePath = Path.Combine(saveDirectory, $"{timestamp}.json");
        File.WriteAllText(savePath, json);
      }
    }

    private void DisableNumberOfUnitsForAllTabs()
    {
      for (int i = 0; i < BUILDING_TAB_CONTROL.TabCount; i++)
      {
        var tabPage = BUILDING_TAB_CONTROL.TabPages[i];
        var buildingLoadCalculationForm = tabPage.Controls.OfType<Building.LoadCalculationForm>().FirstOrDefault();
        if (buildingLoadCalculationForm != null)
        {
          buildingLoadCalculationForm.DisableNumberOfUnits();
        }
      }
    }

    private void TAB_CONTROL_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (TAB_CONTROL.SelectedTab != null && TAB_CONTROL.SelectedTab.Text == "Unit")
      {
        DisableNumberOfUnitsForAllTabs();
      }

      for (int i = 0; i < BUILDING_TAB_CONTROL.TabCount; i++)
      {
        var tabPage = BUILDING_TAB_CONTROL.TabPages[i];
        var buildingLoadCalculationForm = tabPage.Controls.OfType<Building.LoadCalculationForm>().FirstOrDefault();
        if (buildingLoadCalculationForm != null)
        {
          buildingLoadCalculationForm.SetLoadBoxValues();
        }
      }
    }

    private void LOAD_CALCULATION_FORM_FormClosing(object sender, FormClosingEventArgs e)
    {
      SaveLoadCalculationForm();
    }

    private void CREATE_UNIT_BUTTON_Click(object sender, EventArgs e)
    {
      if (TAB_CONTROL.SelectedTab == UNIT_TAB)
      {
        AddNewUnitTab();
        UNIT_TAB_CONTROL.SelectedIndex = UNIT_TAB_CONTROL.TabCount - 1;
      }
      else if (TAB_CONTROL.SelectedTab == BUILDING_TAB)
      {
        AddNewBuildingTab();
        BUILDING_TAB_CONTROL.SelectedIndex = BUILDING_TAB_CONTROL.TabCount - 1;
      }
    }

    private void DELETE_UNIT_BUTTON_Click(object sender, EventArgs e)
    {
      if (TAB_CONTROL.SelectedTab == UNIT_TAB)
      {
        if (UNIT_TAB_CONTROL.SelectedTab != null)
        {
          RemoveCurrentTab();
        }
      }
      else if (TAB_CONTROL.SelectedTab == BUILDING_TAB)
      {
        if (BUILDING_TAB_CONTROL.SelectedTab != null)
        {
          RemoveCurrentTab();
        }
      }
    }

    private void CREATE_Click(object sender, EventArgs e)
    {
      var allUnitInfo = AllUnitInformation();

      Close();

      Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Focus();

      using (DocumentLock docLock = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.LockDocument())
      {
        Point3d point = HelperClass.UserClick();
        foreach (var unitInfo in allUnitInfo)
        {
          Unit.LoadCalculation.CreateUnitLoadCalculationTable(unitInfo, point);
          point = new Point3d(point.X - 7, point.Y, point.Z);
        }
      }
    }

    private void SAVE_BUTTON_Click(object sender, EventArgs e)
    {
      SaveLoadCalculationForm();
    }

    public void UpdateBuildingData(Unit.UnitInformation unitInformation)
    {
      for (int i = 0; i < BUILDING_TAB_CONTROL.TabCount; i++)
      {
        var tabPage = BUILDING_TAB_CONTROL.TabPages[i];
        var buildingLoadCalculationForm = tabPage.Controls.OfType<Building.LoadCalculationForm>().FirstOrDefault();
        if (buildingLoadCalculationForm != null)
        {
          buildingLoadCalculationForm.UpdateUnitData(unitInformation);
        }
      }
    }
  }
}