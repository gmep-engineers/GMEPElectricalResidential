using Autodesk.AutoCAD.ApplicationServices;
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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GMEPElectricalResidential.LoadCalculations
{
  public partial class LOAD_CALCULATION_FORM : Form
  {
    private int _maxUnitID = 0;
    private int _maxBuildingID = 0;
    private ToolTip _toolTip;

    public Commands Commands { get; }
    public bool FormatCheckboxChecked { get; set; }

    private string _initialDocumentPath;

    public LOAD_CALCULATION_FORM(Commands commands)
    {
      Commands = commands;
      InitializeComponent();

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
        tabPage = new TabPage(buildingInformation.Name);
        _maxBuildingID = Math.Max(_maxBuildingID, buildingInformation.ID);
      }
      else
      {
        tabPage = new TabPage("");
        _maxBuildingID++;
      }

      Building.LoadCalculationForm buildingLoadCalculation = new Building.LoadCalculationForm(this, _maxBuildingID, buildingInformation);
      tabPage.Tag = _maxBuildingID;
      tabPage.Controls.Add(buildingLoadCalculation);

      BUILDING_TAB_CONTROL.TabPages.Add(tabPage);
    }

    public void AddNewUnitTab(Unit.UnitInformation unitInformation = null)
    {
      TabPage tabPage;
      if (unitInformation != null)
      {
        tabPage = new TabPage(unitInformation.Name);
        _maxUnitID = Math.Max(_maxUnitID, unitInformation.ID);
      }
      else
      {
        tabPage = new TabPage("");
        _maxUnitID++;
      }

      Unit.LoadCalculationForm unitLoadCalculation = new Unit.LoadCalculationForm(this, _maxUnitID, unitInformation);
      tabPage.Tag = _maxUnitID;
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
        var unitDirectories = Directory.GetDirectories(baseSaveDirectory).OrderBy(d => Path.GetFileName(d));
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
              _maxUnitID = Math.Max(_maxUnitID, unitInformation.ID);
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
        var buildingDirectories = Directory.GetDirectories(baseSaveDirectory).OrderBy(d => Path.GetFileName(d));
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
              _maxBuildingID = Math.Max(_maxBuildingID, buildingInformation.ID);
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
            var selectedTab = UNIT_TAB_CONTROL.SelectedTab;
            var unitLoadCalculation = selectedTab.Controls.OfType<Unit.LoadCalculationForm>().FirstOrDefault();
            if (unitLoadCalculation != null)
            {
              var unitInformation = unitLoadCalculation.RetrieveUnitInformation();
              var id = unitInformation.ID;
              UNIT_TAB_CONTROL.TabPages.Remove(selectedTab);
              RemoveUnitTypeFromBuildings(id);
              DeleteUnitDirectory(unitInformation);
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
            var selectedTab = BUILDING_TAB_CONTROL.SelectedTab;
            var buildingLoadCalculation = selectedTab.Controls.OfType<Building.LoadCalculationForm>().FirstOrDefault();
            if (buildingLoadCalculation != null)
            {
              var buildingInformation = buildingLoadCalculation.RetrieveBuildingInformation();
              BUILDING_TAB_CONTROL.TabPages.Remove(selectedTab);
              DeleteBuildingDirectory(buildingInformation);
            }
          }
        }
      }
    }

    private void DeleteUnitDirectory(Unit.UnitInformation unitInfo)
    {
      if (unitInfo != null)
      {
        string dwgDirectory = Path.GetDirectoryName(_initialDocumentPath);
        string baseSaveDirectory = Path.Combine(dwgDirectory, "Saves", "Load Calculations");
        string unitDirectory = Path.Combine(baseSaveDirectory, "Unit");

        string directoryToDelete = Path.Combine(unitDirectory, unitInfo.FilteredFormattedName());
        if (Directory.Exists(directoryToDelete))
        {
          try
          {
            Directory.Delete(directoryToDelete, true);
          }
          catch (Autodesk.AutoCAD.Runtime.Exception ex)
          {
            MessageBox.Show($"Error deleting unit directory: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
          }
        }
      }
    }

    private void DeleteBuildingDirectory(Building.BuildingInformation buildingInfo)
    {
      if (buildingInfo != null)
      {
        string dwgDirectory = Path.GetDirectoryName(_initialDocumentPath);
        string baseSaveDirectory = Path.Combine(dwgDirectory, "Saves", "Load Calculations");
        string buildingDirectory = Path.Combine(baseSaveDirectory, "Building");

        string directoryToDelete = Path.Combine(buildingDirectory, buildingInfo.FilteredFormattedName());
        if (Directory.Exists(directoryToDelete))
        {
          try
          {
            Directory.Delete(directoryToDelete, true);
          }
          catch (Autodesk.AutoCAD.Runtime.Exception ex)
          {
            MessageBox.Show($"Error deleting building directory: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
          }
        }
      }
    }

    private void RemoveUnitTypeFromBuildings(int id)
    {
      for (int i = 0; i < BUILDING_TAB_CONTROL.TabCount; i++)
      {
        var tabPage = BUILDING_TAB_CONTROL.TabPages[i];
        var buildingLoadCalculation = tabPage.Controls.OfType<Building.LoadCalculationForm>().FirstOrDefault();
        if (buildingLoadCalculation != null)
        {
          var buildingInformation = buildingLoadCalculation.RetrieveBuildingInformation();
          var counterToRemove = buildingInformation.Counters.FirstOrDefault(counter => counter.UnitID == id);
          if (counterToRemove != null)
          {
            buildingInformation.Counters.Remove(counterToRemove);
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

      CleanupMismatchedDirectories(unitDirectory);

      // Create the Building directory if it doesn't exist
      string buildingDirectory = Path.Combine(baseSaveDirectory, "Building");
      Directory.CreateDirectory(buildingDirectory);

      CleanupMismatchedDirectories(buildingDirectory);

      List<Unit.UnitInformation> allUnitInformation = AllUnitInformation();
      HandleUnitDataSaving(unitDirectory, allUnitInformation);

      List<Building.BuildingInformation> allBuildingInformation = AllBuildingInformation();
      HandleBuildingDataSaving(buildingDirectory, allBuildingInformation);
    }

    public void CleanupMismatchedDirectories(string basePath, object informationObject = null)
    {
      foreach (var directory in Directory.GetDirectories(basePath))
      {
        var dirName = new DirectoryInfo(directory).Name;
        var match = Regex.Match(dirName, @"ID(\d+)$");
        if (!match.Success)
        {
          Console.WriteLine($"Directory {dirName} doesn't match the expected format. Skipping.");
          continue;
        }
        var dirId = int.Parse(match.Groups[1].Value);

        // Get all JSON files in the directory
        var jsonFiles = Directory.GetFiles(directory, "*.json");
        if (jsonFiles.Length == 0)
        {
          Console.WriteLine($"JSON file not found in directory {dirName}. Skipping.");
          continue;
        }

        // Find the most recently created JSON file
        var mostRecentJsonFile = jsonFiles
            .Select(f => new FileInfo(f))
            .OrderByDescending(f => f.CreationTime)
            .First();

        var jsonPath = mostRecentJsonFile.FullName;
        Console.WriteLine($"Using most recent JSON file: {mostRecentJsonFile.Name}");

        var jsonContent = File.ReadAllText(jsonPath);

        dynamic info;
        if (informationObject is Building.BuildingInformation)
        {
          info = JsonConvert.DeserializeObject<Building.BuildingInformation>(jsonContent);
        }
        else if (informationObject is Unit.UnitInformation)
        {
          info = JsonConvert.DeserializeObject<Unit.UnitInformation>(jsonContent);
        }
        else
        {
          // If no specific type is provided, assume UnitInformation as before
          info = JsonConvert.DeserializeObject<Unit.UnitInformation>(jsonContent);
        }

        if (info.ID != dirId)
        {
          Console.WriteLine($"Mismatch found in {dirName}. Directory ID: {dirId}, JSON ID: {info.ID}. Deleting directory.");
          Directory.Delete(directory, true);
        }
        else
        {
          Console.WriteLine($"Directory {dirName} is valid. Keeping it.");
        }
      }
    }

    private static void HandleUnitDataSaving(string unitDirectory, List<Unit.UnitInformation> allUnitInformation)
    {
      foreach (var oldDir in Directory.GetDirectories(unitDirectory, "Unit *"))
      {
        if (Directory.Exists(oldDir))
        {
          Directory.Delete(oldDir, true);
        }
      }

      if (!allUnitInformation.Any())
      {
        // Handle the case where allBuildingInformation is empty
        Console.WriteLine("No unit information to process.");
        return;
      }

      var duplicateIds = allUnitInformation
          .GroupBy(u => u.ID)
          .Where(g => g.Count() > 1)
          .Select(g => g.Key)
          .ToHashSet();

      int maximumUnitId = allUnitInformation.Max(u => u.ID);

      foreach (var unitInformation in allUnitInformation)
      {
        if (unitInformation.Name == null) continue;

        if (duplicateIds.Contains(unitInformation.ID))
        {
          unitInformation.ID = ++maximumUnitId;
        }

        string saveDirectory = Path.Combine(unitDirectory, unitInformation.FilteredFormattedName());
        Directory.CreateDirectory(saveDirectory);

        string json = JsonConvert.SerializeObject(unitInformation, Formatting.Indented);
        string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
        string savePath = Path.Combine(saveDirectory, $"{timestamp}.json");
        File.WriteAllText(savePath, json);
      }

      RemoveOldUnitDirectories(unitDirectory, duplicateIds);
    }

    private static void RemoveOldUnitDirectories(string unitDirectory, HashSet<int> duplicateIds)
    {
      var regex = new Regex(@"^.+ - ID(\d+)$");
      foreach (var subdirectory in Directory.GetDirectories(unitDirectory))
      {
        string dirName = Path.GetFileName(subdirectory);
        var match = regex.Match(dirName);
        if (match.Success && int.TryParse(match.Groups[1].Value, out int id) && duplicateIds.Contains(id))
        {
          try
          {
            Directory.Delete(subdirectory, true);
            Console.WriteLine($"Removed directory: {subdirectory}");
          }
          catch (Autodesk.AutoCAD.Runtime.Exception ex)
          {
            Console.WriteLine($"Error removing directory {subdirectory}: {ex.Message}");
          }
        }
      }
    }

    private static void HandleBuildingDataSaving(string buildingDirectory, List<Building.BuildingInformation> allBuildingInformation)
    {
      foreach (var oldDir in Directory.GetDirectories(buildingDirectory, "Building *"))
      {
        if (Directory.Exists(oldDir))
        {
          Directory.Delete(oldDir, true);
        }
      }

      if (!allBuildingInformation.Any())
      {
        // Handle the case where allBuildingInformation is empty
        Console.WriteLine("No building information to process.");
        return;
      }

      var duplicateIds = allBuildingInformation
        .GroupBy(b => b.ID)
        .Where(g => g.Count() > 1)
        .Select(g => g.Key)
        .ToHashSet();

      int maximumBuildingId = allBuildingInformation.Max(b => b.ID);

      foreach (var buildingInformation in allBuildingInformation)
      {
        if (buildingInformation.Name == null) continue;

        if (duplicateIds.Contains(buildingInformation.ID))
        {
          buildingInformation.ID = ++maximumBuildingId;
        }

        string saveDirectory = Path.Combine(buildingDirectory, buildingInformation.FilteredFormattedName());
        Directory.CreateDirectory(saveDirectory);

        string json = JsonConvert.SerializeObject(buildingInformation, Formatting.Indented);
        string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
        string savePath = Path.Combine(saveDirectory, $"{timestamp}.json");
        File.WriteAllText(savePath, json);
      }

      RemoveOldBuildingDirectories(buildingDirectory, duplicateIds);
    }

    private static void RemoveOldBuildingDirectories(string buildingDirectory, HashSet<int> duplicateIds)
    {
      var regex = new Regex(@"^.+ - ID(\d+)$");
      foreach (var subdirectory in Directory.GetDirectories(buildingDirectory))
      {
        string dirName = Path.GetFileName(subdirectory);
        var match = regex.Match(dirName);
        if (match.Success && int.TryParse(match.Groups[1].Value, out int id) && duplicateIds.Contains(id))
        {
          try
          {
            Directory.Delete(subdirectory, true);
            Console.WriteLine($"Removed directory: {subdirectory}");
          }
          catch (Autodesk.AutoCAD.Runtime.Exception ex)
          {
            Console.WriteLine($"Error removing directory {subdirectory}: {ex.Message}");
          }
        }
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

    public void CreateOrUpdateLoadCalculationSingle(bool isCreate, bool isUnit = false, int id = 0)
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

        if (isUnit)
        {
          // Only create/update the specified unit load calculation
          var unitInfo = allUnitInfo.FirstOrDefault(u => u.ID == id);
          if (unitInfo != null)
          {
            Unit.LoadCalculation.CreateUnitLoadCalculationTable(unitInfo, point, isCreate);
          }
        }
        else
        {
          // Create/update all building load calculations
          var buildingInfo = allBuildingInfo.FirstOrDefault(b => b.ID == id);

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

            _maxUnitID++;

            var serializedUnitInformation = JsonConvert.SerializeObject(unitInformation);
            var newUnitInformation = JsonConvert.DeserializeObject<Unit.UnitInformation>(serializedUnitInformation);

            newUnitInformation.ID = _maxUnitID;

            TabPage newTabPage = new TabPage(newUnitInformation.FormattedName());
            newTabPage.Tag = _maxUnitID;
            Unit.LoadCalculationForm unitLoadCalculation = new Unit.LoadCalculationForm(this, _maxUnitID, newUnitInformation);
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
            _maxBuildingID++;

            var serializedBuildingInformation = JsonConvert.SerializeObject(buildingInformation);
            var newBuildingInformation = JsonConvert.DeserializeObject<Building.BuildingInformation>(serializedBuildingInformation);

            // Update the building information with the new ID
            newBuildingInformation.ID = _maxBuildingID;

            // Create a new tab with the updated building information
            TabPage newTabPage = new TabPage(newBuildingInformation.Name);
            newTabPage.Tag = _maxBuildingID;
            Building.LoadCalculationForm buildingLoadCalculation = new Building.LoadCalculationForm(this, _maxBuildingID, newBuildingInformation);
            newTabPage.Controls.Add(buildingLoadCalculation);
            BUILDING_TAB_CONTROL.TabPages.Add(newTabPage);

            BUILDING_TAB_CONTROL.SelectedIndex = BUILDING_TAB_CONTROL.TabCount - 1;
          }
        }
      }
    }

    private void LOAD_BUTTON_Click(object sender, EventArgs e)
    {
      // Close the form (this will trigger a save)
      this.Close();

      // Use OpenFileDialog instead of FolderBrowserDialog
      using (var openFileDialog = new OpenFileDialog())
      {
        openFileDialog.Title = "Select the Load Calculations file";
        openFileDialog.Filter = "All Files (*.*)|*.*";
        openFileDialog.CheckFileExists = false;
        openFileDialog.CheckPathExists = true;
        openFileDialog.FileName = "Load Calculations";

        if (openFileDialog.ShowDialog() == DialogResult.OK)
        {
          string selectedPath = Path.GetDirectoryName(openFileDialog.FileName);
          string selectedFileName = Path.GetFileName(openFileDialog.FileName);

          if (selectedFileName == "Load Calculations")
          {
            CopyLoadCalculationData(selectedPath);
          }
          else
          {
            MessageBox.Show("Please select a file or directory named 'Load Calculations'.", "Invalid Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
          }
        }
      }
    }

    private void CopyLoadCalculationData(string sourcePath)
    {
      string targetPath = Path.Combine(Path.GetDirectoryName(_initialDocumentPath), "Saves", "Load Calculations");

      // Dictionary to store old and new Unit IDs
      Dictionary<int, int> unitIdMap = new Dictionary<int, int>();

      // Copy unit data
      CopyDirectoryData(Path.Combine(sourcePath, "Unit"), Path.Combine(targetPath, "Unit"),
          (sourceFile, targetDir) => CopyUnitData(sourceFile, targetDir, unitIdMap));

      // Copy building data
      CopyDirectoryData(Path.Combine(sourcePath, "Building"), Path.Combine(targetPath, "Building"),
          (sourceFile, targetDir) => CopyBuildingData(sourceFile, targetDir, unitIdMap));
    }

    private void CopyDirectoryData(string sourceDir, string targetDir, Action<string, string> copyAction)
    {
      if (Directory.Exists(sourceDir))
      {
        Directory.CreateDirectory(targetDir);

        foreach (var subDir in Directory.GetDirectories(sourceDir))
        {
          string dirName = Path.GetFileName(subDir);
          string newTargetSubDir = Path.Combine(targetDir, dirName);
          Directory.CreateDirectory(newTargetSubDir);

          var latestFile = Directory.GetFiles(subDir, "*.json")
                                    .OrderByDescending(f => f)
                                    .FirstOrDefault();

          if (latestFile != null)
          {
            copyAction(latestFile, newTargetSubDir);
          }
        }
      }
    }

    private void CopyUnitData(string sourceFile, string targetDir, Dictionary<int, int> unitIdMap)
    {
      string json = File.ReadAllText(sourceFile);
      var unitInfo = JsonConvert.DeserializeObject<Unit.UnitInformation>(json);

      _maxUnitID++;
      unitInfo.ID = _maxUnitID;

      string newJson = JsonConvert.SerializeObject(unitInfo, Formatting.Indented);
      string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
      string savePath = Path.Combine(targetDir, $"{timestamp}.json");
      File.WriteAllText(savePath, newJson);
    }

    private void CopyBuildingData(string sourceFile, string targetDir, Dictionary<int, int> unitIdMap)
    {
      string json = File.ReadAllText(sourceFile);
      var buildingInfo = JsonConvert.DeserializeObject<Building.BuildingInformation>(json);

      _maxBuildingID++;
      buildingInfo.ID = _maxBuildingID;

      // Update the Counters with new Unit IDs
      if (buildingInfo.Counters != null)
      {
        foreach (var counter in buildingInfo.Counters)
        {
          if (unitIdMap.TryGetValue(counter.UnitID, out int newUnitId))
          {
            counter.UnitID = newUnitId;
          }
          else
          {
            Console.WriteLine($"Warning: Unit ID {counter.UnitID} not found in the new mapping.");
          }
        }
      }

      string newJson = JsonConvert.SerializeObject(buildingInfo, Formatting.Indented);
      string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
      string savePath = Path.Combine(targetDir, $"{timestamp}.json");
      File.WriteAllText(savePath, newJson);
    }

    private void GENERATE_PANELS_Click(object sender, EventArgs e)
    {
      // get the unit information
      var unitInformation = AllUnitInformation();

      // show the PanelGenerator form
      var panelGeneratorUnitSelectionForm = new PanelGeneratorUnitSelection(unitInformation);
      panelGeneratorUnitSelectionForm.Show();
    }
  }

  internal class PanelGeneratorData
  {
    private UnitInformation _data;
    private const double GeneralLightingPercent = 0.70;
    private const double ExteriorLightingPercent = 0.10;
    private const double LivingRecPercent = 0.30;
    private const double ExteriorRecPercent = 0.10;
    private const int COAndSmokeWatt = 100;
    private const int GasRecWatt = 50;
    private const int LightingBreakerMaximumWatt = 1000;
    private bool hasExteriorLighting = false;
    private bool hasExteriorReceptacle = false;
    private bool hasGasReceptacle = false;

    public List<PanelBreaker> PanelBreakers { get; private set; }

    public PanelGeneratorData(UnitInformation data, bool hasExteriorLighting, bool hasExteriorReceptacle, bool hasGasReceptacle)
    {
      _data = data;
      this.hasExteriorLighting = hasExteriorLighting;
      this.hasExteriorReceptacle = hasExteriorReceptacle;
      this.hasGasReceptacle = hasGasReceptacle;
      PanelBreakers = new List<PanelBreaker>();
    }

    private List<PanelBreaker> DistributeBreakers(string name, int totalWatts, int maxBreakers = 10)
    {
      List<PanelBreaker> breakers = new List<PanelBreaker>();
      int numBreakers = Math.Min(maxBreakers, Math.Max(1, (int)Math.Ceiling(totalWatts / 1000.0)));
      int wattsPerBreaker = (int)Math.Ceiling((double)totalWatts / numBreakers);

      for (int i = 0; i < numBreakers; i++)
      {
        int breakerWatts = (i == numBreakers - 1) ? totalWatts : wattsPerBreaker;
        breakers.Add(new PanelBreaker($"{name} {i + 1}", breakerWatts, 1));
        totalWatts -= breakerWatts;
      }

      return breakers;
    }

    public void GenerateLightingBreakers()
    {
      int totalLightingWatts = _data.GeneralLoads.Lighting.Total;

      PanelBreakers.Add(new PanelBreaker("CO & Smoke", COAndSmokeWatt, 1));

      if (hasGasReceptacle)
      {
        totalLightingWatts -= GasRecWatt;
        PanelBreakers.Add(new PanelBreaker("Gas Receptacle", GasRecWatt, 1));
      }

      int remainingWatts = totalLightingWatts - COAndSmokeWatt;

      double generalLightingPercent = GeneralLightingPercent;
      if (hasExteriorLighting)
        generalLightingPercent -= ExteriorLightingPercent;
      if (hasExteriorReceptacle)
        generalLightingPercent -= ExteriorRecPercent;

      int livingRecWatts = (int)(remainingWatts * LivingRecPercent);
      int generalLightingWatts = (int)(remainingWatts * generalLightingPercent);

      PanelBreakers.AddRange(DistributeBreakers("Living Receptacle", livingRecWatts));

      if (hasExteriorLighting)
      {
        int exteriorLightingWatts = (int)(remainingWatts * ExteriorLightingPercent);
        PanelBreakers.AddRange(DistributeBreakers("Exterior Lighting", exteriorLightingWatts));
      }

      if (hasExteriorReceptacle)
      {
        int exteriorRecWatts = (int)(remainingWatts * ExteriorRecPercent);
        PanelBreakers.AddRange(DistributeBreakers("Exterior Receptacle", exteriorRecWatts));
      }

      PanelBreakers.AddRange(DistributeBreakers("General Lighting", generalLightingWatts));
    }
  }

  internal class PanelBreaker
  {
    public string Description { get; set; }
    public int Watts { get; set; }
    public int Poles { get; set; }

    public PanelBreaker(string description, int watts, int poles)
    {
      Description = description;
      Watts = watts;
      Poles = poles;
    }
  }
}