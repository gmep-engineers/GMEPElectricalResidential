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
    private string _NameWatermark = "Enter name...";
    private string _VAWatermark = "Enter VA...";
    private ToolTip _toolTip = new ToolTip();

    public UnitLoadCalculation()
    {
      InitializeComponent();
      SetDefaultValues();
      AddWaterMarks();
      DetectIncorrectInputs();
    }

    private void DetectIncorrectInputs()
    {
      GENERAL_CUSTOM_VA.KeyPress += GENERAL_CUSTOM_VA_KeyPress;
      CUSTOM_VA.KeyPress += GENERAL_CUSTOM_VA_KeyPress;
    }

    private void GENERAL_CUSTOM_VA_KeyPress(object sender, KeyPressEventArgs e)
    {
      var textBox = sender as TextBox;
      if (textBox != null)
      {
        if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
        {
          e.Handled = true;
          _toolTip.Show("You must enter a digit.", textBox, 0, -20, 2000);
        }
        else
        {
          _toolTip.Hide(textBox);
        }
      }
    }

    private void AddWaterMarks()
    {
      GENERAL_CUSTOM_NAME.Text = _NameWatermark;
      GENERAL_CUSTOM_NAME.ForeColor = Color.LightGray;
      GENERAL_CUSTOM_NAME.Enter += RemoveWatermark;
      GENERAL_CUSTOM_NAME.Leave += AddWatermark;

      GENERAL_CUSTOM_VA.Text = _VAWatermark;
      GENERAL_CUSTOM_VA.ForeColor = Color.LightGray;
      GENERAL_CUSTOM_VA.Enter += RemoveWatermark;
      GENERAL_CUSTOM_VA.Leave += AddWatermark;

      CUSTOM_NAME.Text = _NameWatermark;
      CUSTOM_NAME.ForeColor = Color.LightGray;
      CUSTOM_NAME.Enter += RemoveWatermark;
      CUSTOM_NAME.Leave += AddWatermark;

      CUSTOM_VA.Text += _VAWatermark;
      CUSTOM_VA.ForeColor = Color.LightGray;
      CUSTOM_VA.Enter += RemoveWatermark;
      CUSTOM_VA.Leave += AddWatermark;
    }

    private void RemoveWatermark(object sender, EventArgs e)
    {
      var textBox = sender as TextBox;
      if (textBox != null)
      {
        if (textBox.Text == _NameWatermark || textBox.Text == _VAWatermark)
        {
          textBox.Text = "";
          textBox.ForeColor = Color.Black;
        }
      }
    }

    private void AddWatermark(object sender, EventArgs e)
    {
      var textBox = sender as TextBox;
      if (textBox != null)
      {
        if (string.IsNullOrEmpty(textBox.Text))
        {
          textBox.Text = textBox == GENERAL_CUSTOM_NAME ? _NameWatermark : _VAWatermark;
          textBox.ForeColor = Color.LightGray;
        }
      }
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

    private void GENERAL_CUSTOM_VA_TextChanged(object sender, EventArgs e)
    {
    }
  }
}