using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GMEPElectricalResidential
{
  public partial class UnitLoadCalculation : UserControl
  {
    public UnitLoadCalculation()
    {
      InitializeComponent();
      SetDefaultValues();
    }

    private void SetDefaultValues()
    {
      VOLTAGE.SelectedIndex = 0;
    }

    private void UNIT_NAME_TextChanged(object sender, EventArgs e)
    {
      var textBox = sender as TextBox;
      if (textBox != null)
      {
        var parentTab = this.Parent as TabPage;
        if (parentTab != null)
        {
          parentTab.Text = $"Unit {textBox.Text}";
        }
      }
    }

    private void ELECTRIC_HEATER_CheckedChanged(object sender, EventArgs e)
    {
      var radioButton = sender as RadioButton;
      if (radioButton != null && radioButton.Checked && WATER_HEATER_VA.Text == "180")
      {
        WATER_HEATER_VA.Text = "5000";
      }
    }

    private void GAS_HEATER_CheckedChanged(object sender, EventArgs e)
    {
      var radioButton = sender as RadioButton;
      if (radioButton != null && radioButton.Checked)
      {
        WATER_HEATER_VA.Text = "180";
      }
    }

    private void ELECTRIC_DRYER_CheckedChanged(object sender, EventArgs e)
    {
      var radioButton = sender as RadioButton;
      if (radioButton != null && radioButton.Checked && DRYER_VA.Text == "180")
      {
        DRYER_VA.Text = "5000";
      }
    }

    private void GAS_DRYER_CheckedChanged(object sender, EventArgs e)
    {
      var radioButton = sender as RadioButton;
      if (radioButton != null && radioButton.Checked)
      {
        DRYER_VA.Text = "180";
      }
    }

    private void ELECTRIC_OVEN_CheckedChanged(object sender, EventArgs e)
    {
      var radioButton = sender as RadioButton;
      if (radioButton != null && radioButton.Checked && OVEN_VA.Text == "180")
      {
        OVEN_VA.Text = "8000";
      }
    }

    private void GAS_OVEN_CheckedChanged(object sender, EventArgs e)
    {
      var radioButton = sender as RadioButton;
      if (radioButton != null && radioButton.Checked)
      {
        OVEN_VA.Text = "180";
      }
    }

    private void ELECTRIC_COOKTOP_CheckedChanged(object sender, EventArgs e)
    {
      var radioButton = sender as RadioButton;
      if (radioButton != null && radioButton.Checked && COOKTOP_VA.Text == "180")
      {
        COOKTOP_VA.Text = "3000";
      }
    }

    private void GAS_COOKTOP_CheckedChanged(object sender, EventArgs e)
    {
      var radioButton = sender as RadioButton;
      if (radioButton != null && radioButton.Checked)
      {
        COOKTOP_VA.Text = "180";
      }
    }

    private void WriteMessageToAutoCADConsole(object thing, string preMessage = "")
    {
      var message = JsonConvert.SerializeObject(thing, Formatting.Indented);
      var doc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
      doc.Editor.WriteMessage(preMessage);
      doc.Editor.WriteMessage(message);
    }

    private void AREA_TextChanged(object sender, EventArgs e)
    {
      var textBox = sender as TextBox;
      if (textBox != null && int.TryParse(textBox.Text, out int floorArea))
      {
        GENERAL_LIGHTING_VA.Text = (floorArea * 3).ToString();
      }
    }
  }
}