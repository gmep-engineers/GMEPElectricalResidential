﻿using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using GMEPElectricalResidential.HelperFiles;
using GMEPElectricalResidential.LoadCalculations.Building;
using GMEPElectricalResidential.LoadCalculations.Unit;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.PerformanceData;
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
    private ToolTip _toolTip;
    private List<int> _unitCannotBeIDs;
    private List<int> _buildingCannotBeIDs;

    public Commands Commands { get; }
    public bool FormatCheckboxChecked { get; set; }

    private string _initialDocumentPath;

    public LOAD_CALCULATION_FORM(Commands commands)
    {
      Commands = commands;
      InitializeComponent();

      _unitCannotBeIDs = new List<int>();
      _buildingCannotBeIDs = new List<int>();
      _toolTip = new ToolTip();

      var doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
      _initialDocumentPath = doc.Database.Filename;

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
      Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.BeginDocumentClose += Document_BeginDocumentClose;
    }

    private void Document_BeginDocumentClose(object sender, Autodesk.AutoCAD.ApplicationServices.DocumentBeginCloseEventArgs e)
    {
      SaveLoadCalculationForm();
      Close();
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
            var id = (int)UNIT_TAB_CONTROL.SelectedTab.Tag;
            UNIT_TAB_CONTROL.TabPages.Remove(UNIT_TAB_CONTROL.SelectedTab);
            RemoveUnitTypeFromBuildings(id);
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

    private void RemoveUnitTypeFromBuildings(int id)
    {
      var allBuildingInformation = AllBuildingInformation();

      foreach (var buildingInformation in allBuildingInformation)
      {
        var counterToRemove = buildingInformation.Counters.FirstOrDefault(counter => counter.UnitID == id);
        if (counterToRemove != null)
        {
          buildingInformation.Counters = buildingInformation.Counters
              .Where(counter => counter.UnitID != id)
              .ToList();
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

    private void SaveLoadCalculationForm()
    {
      string dwgDirectory = Path.GetDirectoryName(_initialDocumentPath);
      string baseSaveDirectory = Path.Combine(dwgDirectory, "Saves", "Load Calculations");

      // Create the Saves directory if it doesn't exist
      string savesDirectory = Path.Combine(dwgDirectory, "Saves");
      Directory.CreateDirectory(savesDirectory);

      // Create the Load Calculations directory if it doesn't exist
      Directory.CreateDirectory(baseSaveDirectory);

      // Create the Unit directory if it doesn't exist
      string unitDirectory = Path.Combine(baseSaveDirectory, "Unit");
      Directory.CreateDirectory(unitDirectory);

      // Create the Building directory if it doesn't exist
      string buildingDirectory = Path.Combine(baseSaveDirectory, "Building");
      Directory.CreateDirectory(buildingDirectory);

      List<Unit.UnitInformation> allUnitInformation = AllUnitInformation();
      HandleUnitDataSaving(unitDirectory, allUnitInformation);

      List<Building.BuildingInformation> allBuildingInformation = AllBuildingInformation();
      HandleBuildingDataSaving(buildingDirectory, allBuildingInformation);
    }

    private static void HandleUnitDataSaving(string unitDirectory, List<Unit.UnitInformation> allUnitInformation)
    {
      foreach (var unitInformation in allUnitInformation)
      {
        string saveDirectory = Path.Combine(unitDirectory, unitInformation.FilteredFormattedName());
        Directory.CreateDirectory(saveDirectory);

        string json = JsonConvert.SerializeObject(unitInformation, Formatting.Indented);
        string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
        string savePath = Path.Combine(saveDirectory, $"{timestamp}.json");
        File.WriteAllText(savePath, json);
      }
    }

    private static void HandleBuildingDataSaving(string buildingDirectory, List<Building.BuildingInformation> allBuildingInformation)
    {
      foreach (var buildingInformation in allBuildingInformation)
      {
        string saveDirectory = Path.Combine(buildingDirectory, buildingInformation.FilteredFormattedName());
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

    private void CreateOrUpdateLoadCalculations(bool isCreate)
    {
      var allUnitInfo = AllUnitInformation();
      var allBuildingInfo = AllBuildingInformation();

      Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Focus();

      using (DocumentLock docLock = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.LockDocument())
      {
        Point3d point = new Point3d(0, 0, 0);

        if (isCreate)
        {
          point = HelperClass.UserClick();
        }

        foreach (var unitInfo in allUnitInfo)
        {
          Unit.LoadCalculation.CreateUnitLoadCalculationTable(unitInfo, point, isCreate);
          point = new Point3d(point.X - 7, point.Y, point.Z);
        }

        int numberOfBuildingsWithThreeOrMoreUnits = allBuildingInfo.Count(building => building.TotalNumberOfUnits() >= 3);

        foreach (var buildingInfo in allBuildingInfo)
        {
          var numberOfUnits = buildingInfo.TotalNumberOfUnits();

          if (numberOfUnits != 2) continue;

          point = CreateComboUnitLoadCalculationTable(isCreate, allUnitInfo, point, buildingInfo);
        }

        foreach (var buildingInfo in allBuildingInfo)
        {
          var numberOfUnits = buildingInfo.TotalNumberOfUnits();

          if (numberOfUnits < 3) continue;

          double width = Building.LoadCalculation.CreateBuildingLoadCalculationTable(buildingInfo, allUnitInfo, point, isCreate);
          point = new Point3d(point.X - width, point.Y, point.Z);
        }
      }
    }

    private static Point3d CreateComboUnitLoadCalculationTable(bool isCreate, List<UnitInformation> allUnitInfo, Point3d point, BuildingInformation buildingInfo)
    {
      var unitInfo1 = allUnitInfo.FirstOrDefault(unit => buildingInfo.Counters.Any(counter => counter.Count == 2));

      if (unitInfo1 != null)
      {
        var unitInfo2 = unitInfo1;
        unitInfo1 = CombinedUnitInformation.CreateCombinedCopyOfUnitInfo(unitInfo1, unitInfo2);
        Unit.LoadCalculation.CreateUnitLoadCalculationTable(unitInfo1, point, isCreate);
        point = new Point3d(point.X - 7, point.Y, point.Z);
      }
      else
      {
        var units = allUnitInfo.Where(unit => buildingInfo.Counters.Count(counter => counter.UnitID == unit.ID) == 1).ToList();

        if (units.Count == 2)
        {
          unitInfo1 = units[0];
          var unitInfo2 = units[1];

          if (unitInfo1.ID > unitInfo2.ID)
          {
            var temp = unitInfo1;
            unitInfo1 = unitInfo2;
            unitInfo2 = temp;
          }

          unitInfo1 = CombinedUnitInformation.CreateCombinedCopyOfUnitInfo(unitInfo1, unitInfo2);
          Unit.LoadCalculation.CreateUnitLoadCalculationTable(unitInfo1, point, isCreate);
          point = new Point3d(point.X - 7, point.Y, point.Z);
        }
      }

      return point;
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

      Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.BeginDocumentClose -= Document_BeginDocumentClose;
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
      CreateOrUpdateLoadCalculations(true);
    }

    private void SAVE_BUTTON_Click(object sender, EventArgs e)
    {
      SaveLoadCalculationForm();
    }

    private void UPDATE_Click(object sender, EventArgs e)
    {
      CreateOrUpdateLoadCalculations(false);
    }

    private void DUPLICATE_Click(object sender, EventArgs e)
    {
      if (TAB_CONTROL.SelectedTab == UNIT_TAB)
      {
        if (UNIT_TAB_CONTROL.SelectedTab != null)
        {
          var selectedTabPage = UNIT_TAB_CONTROL.SelectedTab;
          var selectedLoadCalculationForm = selectedTabPage.Controls.OfType<Unit.LoadCalculationForm>().FirstOrDefault();
          if (selectedLoadCalculationForm != null)
          {
            var unitInformation = selectedLoadCalculationForm.RetrieveUnitInformation();

            // Generate a new unit ID
            int newUnitID = _UnitTabID + 1;
            while (_unitCannotBeIDs.Contains(newUnitID))
            {
              newUnitID++;
            }

            var serializedUnitInformation = JsonConvert.SerializeObject(unitInformation);
            var newUnitInformation = JsonConvert.DeserializeObject<Unit.UnitInformation>(serializedUnitInformation);

            // Update the unit information with the new ID
            newUnitInformation.ID = newUnitID;

            // Add the new ID to the list of taken IDs
            _unitCannotBeIDs.Add(newUnitID);

            // Create a new tab with the updated unit information
            TabPage newTabPage = new TabPage(newUnitInformation.FormattedName());
            newTabPage.Tag = newUnitID;
            Unit.LoadCalculationForm unitLoadCalculation = new Unit.LoadCalculationForm(this, newUnitID, newUnitInformation);
            newTabPage.Controls.Add(unitLoadCalculation);
            UNIT_TAB_CONTROL.TabPages.Add(newTabPage);

            UNIT_TAB_CONTROL.SelectedIndex = UNIT_TAB_CONTROL.TabCount - 1;
          }
        }
      }
      else if (TAB_CONTROL.SelectedTab == BUILDING_TAB)
      {
        if (BUILDING_TAB_CONTROL.SelectedTab != null)
        {
          var selectedTabPage = BUILDING_TAB_CONTROL.SelectedTab;
          var selectedLoadCalculationForm = selectedTabPage.Controls.OfType<Building.LoadCalculationForm>().FirstOrDefault();
          if (selectedLoadCalculationForm != null)
          {
            var buildingInformation = selectedLoadCalculationForm.RetrieveBuildingInformation();

            // Generate a new building ID
            int newBuildingID = _BuildingTabID + 1;
            while (_buildingCannotBeIDs.Contains(newBuildingID))
            {
              newBuildingID++;
            }

            var serializedBuildingInformation = JsonConvert.SerializeObject(buildingInformation);
            var newBuildingInformation = JsonConvert.DeserializeObject<Building.BuildingInformation>(serializedBuildingInformation);

            // Update the building information with the new ID
            newBuildingInformation.ID = newBuildingID;

            // Add the new ID to the list of taken IDs
            _buildingCannotBeIDs.Add(newBuildingID);

            // Create a new tab with the updated building information
            TabPage newTabPage = new TabPage("Building " + newBuildingInformation.Name);
            newTabPage.Tag = newBuildingID;
            Building.LoadCalculationForm buildingLoadCalculation = new Building.LoadCalculationForm(this, newBuildingID, newBuildingInformation);
            newTabPage.Controls.Add(buildingLoadCalculation);
            BUILDING_TAB_CONTROL.TabPages.Add(newTabPage);

            BUILDING_TAB_CONTROL.SelectedIndex = BUILDING_TAB_CONTROL.TabCount - 1;
          }
        }
      }
    }
  }
}