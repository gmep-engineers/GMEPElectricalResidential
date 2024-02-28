﻿using GMEPUtilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GMEPElectricalResidential
{
  public partial class LoadCalculationForm : Form
  {
    private int _tabID = 0;

    public LoadCalculationForm()
    {
      InitializeComponent();
      bool createdTab = LoadSavedUnitLoadCalculations();
      if (!createdTab)
      {
        AddNewTab();
      }

      this.FormClosing += LoadCalculationForm_FormClosing;
    }

    private bool LoadSavedUnitLoadCalculations()
    {
      var createdTabFlag = false;
      var doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
      string dwgDirectory = Path.GetDirectoryName(doc.Database.Filename);
      string baseSaveDirectory = Path.Combine(dwgDirectory, "Load Calculation Saves");

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

            var unitInformation = JsonConvert.DeserializeObject<UnitInformation>(json);

            AddNewTab(unitInformation);

            createdTabFlag = true;
          }
        }
      }
      return createdTabFlag;
    }

    private void LoadCalculationForm_FormClosing(object sender, FormClosingEventArgs e)
    {
      SaveLoadCalculationForm();
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

    public void AddNewTab(UnitInformation unitInformation = null)
    {
      TabPage tabPage;
      if (unitInformation != null)
      {
        tabPage = new TabPage("Unit " + unitInformation.Name);
      }
      else
      {
        tabPage = new TabPage("Unit");
      }

      UnitLoadCalculation unitLoadCalculation = new UnitLoadCalculation(_tabID, unitInformation);
      tabPage.Tag = _tabID;
      tabPage.Controls.Add(unitLoadCalculation);
      TAB_CONTROL.TabPages.Add(tabPage);

      _tabID++;
    }

    public void RemoveCurrentTab()
    {
      if (TAB_CONTROL.TabCount > 0)
      {
        DialogResult result = MessageBox.Show("Are you sure you want to remove this tab?", "Confirmation", MessageBoxButtons.YesNo);

        if (result == DialogResult.Yes)
        {
          TAB_CONTROL.TabPages.Remove(TAB_CONTROL.SelectedTab);
        }
      }
    }

    private void SaveLoadCalculationForm()
    {
      var doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
      string dwgDirectory = Path.GetDirectoryName(doc.Database.Filename);
      string baseSaveDirectory = Path.Combine(dwgDirectory, "Load Calculation Saves");

      for (int i = 0; i < TAB_CONTROL.TabCount; i++)
      {
        var tabPage = TAB_CONTROL.TabPages[i];
        var unitLoadCalculation = tabPage.Controls.OfType<UnitLoadCalculation>().FirstOrDefault();

        if (unitLoadCalculation != null)
        {
          var unitInformation = unitLoadCalculation.RetrieveUnitInformation();

          string unitDirectory = $"Unit {tabPage.Text.Replace("Unit", "").Trim()} - ID{tabPage.Tag}";
          string saveDirectory = Path.Combine(baseSaveDirectory, unitDirectory);

          if (!Directory.Exists(saveDirectory))
          {
            var existingDirectory = Directory.GetDirectories(baseSaveDirectory)
                .FirstOrDefault(dir => dir.Contains($"ID{tabPage.Tag}"));

            if (existingDirectory != null)
            {
              Directory.Move(existingDirectory, saveDirectory);
            }
            else
            {
              Directory.CreateDirectory(saveDirectory);
            }
          }

          string json = JsonConvert.SerializeObject(unitInformation, Formatting.Indented);

          string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
          string savePath = Path.Combine(saveDirectory, $"{timestamp}.json");
          File.WriteAllText(savePath, json);
        }
      }
    }

    public void RemoveUnitTypeData(int tabID)
    {
      var doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
      string dwgDirectory = Path.GetDirectoryName(doc.Database.Filename);
      string baseSaveDirectory = Path.Combine(dwgDirectory, "Load Calculation Saves");

      var unitDirectory = Directory.GetDirectories(baseSaveDirectory)
          .FirstOrDefault(dir => dir.Contains($"ID{tabID}"));

      if (unitDirectory != null)
      {
        Directory.Delete(unitDirectory, true);
      }
    }

    private void CREATE_UNIT_BUTTON_Click(object sender, EventArgs e)
    {
      AddNewTab();
    }

    private void DELETE_UNIT_BUTTON_Click(object sender, EventArgs e)
    {
      if (TAB_CONTROL.SelectedTab != null)
      {
        if (TAB_CONTROL.SelectedTab.Tag is int tabId ||
            int.TryParse(TAB_CONTROL.SelectedTab.Tag.ToString(), out tabId))
        {
          WriteMessageToAutoCADConsole(tabId, "Tab ID: ");
          RemoveUnitTypeData(tabId);
        }

        RemoveCurrentTab();
      }
    }

    private void SAVE_BUTTON_Click(object sender, EventArgs e)
    {
      SaveLoadCalculationForm();
    }

    private void WriteMessageToAutoCADConsole(object thing, string preMessage = "")
    {
      var settings = new JsonSerializerSettings
      {
        ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
        PreserveReferencesHandling = PreserveReferencesHandling.Objects
      };

      var message = JsonConvert.SerializeObject(thing, Formatting.Indented, settings);
      var doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
      doc.Editor.WriteMessage(preMessage);
      doc.Editor.WriteMessage(message);
    }
  }
}