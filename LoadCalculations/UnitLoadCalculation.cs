using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
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
      DetectEnterPresses();
    }

    private void DetectEnterPresses()
    {
      // Subscribe to KeyDown event for GENERAL_CUSTOM controls
      GENERAL_CUSTOM_NAME.KeyDown += TextBox_KeyDown;
      GENERAL_CUSTOM_VA.KeyDown += TextBox_KeyDown;
      GENERAL_CUSTOM_MULTIPLIER.KeyDown += TextBox_KeyDown;

      // Subscribe to KeyDown event for CUSTOM controls
      CUSTOM_NAME.KeyDown += TextBox_KeyDown;
      CUSTOM_VA.KeyDown += TextBox_KeyDown;
      CUSTOM_MULTIPLIER.KeyDown += TextBox_KeyDown;
    }

    private void TextBox_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
      {
        if (sender == GENERAL_CUSTOM_NAME || sender == GENERAL_CUSTOM_VA || sender == GENERAL_CUSTOM_MULTIPLIER)
        {
          AddEntry(GENERAL_CUSTOM_NAME, GENERAL_CUSTOM_VA, GENERAL_CUSTOM_MULTIPLIER, GENERAL_CUSTOM_LOAD_BOX);
        }
        else if (sender == CUSTOM_NAME || sender == CUSTOM_VA || sender == CUSTOM_MULTIPLIER)
        {
          AddEntry(CUSTOM_NAME, CUSTOM_VA, CUSTOM_MULTIPLIER, CUSTOM_LOAD_BOX);
        }
        e.SuppressKeyPress = true;
      }
    }

    private void DetectIncorrectInputs()
    {
      SubscribeVAsToOnlyDigits(this.Controls);
      SubscribeMultipliersToOnlyDigitInputs(this.Controls);
    }

    private void SubscribeMultipliersToOnlyDigitInputs(Control.ControlCollection controls)
    {
      foreach (Control control in controls)
      {
        if (control is ComboBox comboBox && comboBox.Name.EndsWith("MULTIPLIER"))
        {
          control.KeyPress += OnlyDigitInputs;
        }
      }
    }

    private void SubscribeVAsToOnlyDigits(Control.ControlCollection controls)
    {
      foreach (Control control in controls)
      {
        if (control is TextBox textBox && textBox.Name.EndsWith("VA"))
        {
          textBox.KeyPress += OnlyDigitInputs;
        }

        if (control.HasChildren)
        {
          SubscribeVAsToOnlyDigits(control.Controls);
        }
      }
    }

    private void OnlyDigitInputs(object sender, KeyPressEventArgs e)
    {
      if (sender is TextBox textBox)
      {
        HandleDigitInput(textBox, e);
      }
      else if (sender is ComboBox comboBox && comboBox.DropDownStyle == ComboBoxStyle.DropDown)
      {
        HandleDigitInput(comboBox, e);
      }
    }

    private void HandleDigitInput(Control control, KeyPressEventArgs e)
    {
      if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
      {
        e.Handled = true;
        _toolTip.Show("You must enter a digit.", control, 0, -20, 2000);
      }
      else
      {
        _toolTip.Hide(control);
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

    private bool isGreaterThanZero(string multiplier)
    {
      if (string.IsNullOrEmpty(multiplier))
      {
        return false;
      }

      if (decimal.TryParse(multiplier, out decimal result))
      {
        return result > 0;
      }

      return false;
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

    private void AREA_TextChanged(object sender, EventArgs e)
    {
      var textBox = sender as TextBox;
      if (textBox != null && int.TryParse(textBox.Text, out int floorArea))
      {
        GENERAL_LIGHTING_VA.Text = (floorArea * 3).ToString();
      }
    }

    private void AddEntry(TextBox nameTextBox, TextBox vaTextBox, ComboBox multiplierComboBox, ListBox listBox)
    {
      string name = nameTextBox.Text;
      string va = vaTextBox.Text;
      string multiplier = multiplierComboBox.Text;

      if (string.IsNullOrEmpty(name) || name == _NameWatermark)
      {
        _toolTip.Show("You must enter a name.", nameTextBox, 0, -20, 2000);
        return;
      }
      else
      {
        _toolTip.Hide(nameTextBox);
      }

      if (string.IsNullOrEmpty(va) || va == _VAWatermark)
      {
        _toolTip.Show("You must enter a VA.", vaTextBox, 0, -20, 2000);
        return;
      }
      else
      {
        _toolTip.Hide(vaTextBox);
      }

      bool isMultiplierGreaterThanZero = isGreaterThanZero(multiplier);
      WriteMessageToAutoCADConsole(isMultiplierGreaterThanZero);

      if (string.IsNullOrEmpty(multiplier) || !isMultiplierGreaterThanZero)
      {
        _toolTip.Show("You must enter a multiplier that is greater than 0.", multiplierComboBox, 0, -20, 2000);
        return;
      }
      else
      {
        _toolTip.Hide(multiplierComboBox);
      }

      TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
      name = textInfo.ToTitleCase(name.ToLower());

      string newEntry = $"{name}, {va}, {multiplier}";

      listBox.Items.Add(newEntry);

      nameTextBox.Text = "";
      vaTextBox.Text = "";
      multiplierComboBox.Text = "1";

      nameTextBox.Focus();
    }

    private void ADD_ENTRY_Click(object sender, EventArgs e)
    {
      AddEntry(GENERAL_CUSTOM_NAME, GENERAL_CUSTOM_VA, GENERAL_CUSTOM_MULTIPLIER, GENERAL_CUSTOM_LOAD_BOX);
    }

    private void ADD_ENTRY_CUSTOM_Click(object sender, EventArgs e)
    {
      AddEntry(CUSTOM_NAME, CUSTOM_VA, CUSTOM_MULTIPLIER, CUSTOM_LOAD_BOX);
    }

    private void RemoveEntry(ListBox listBox)
    {
      if (listBox.Items.Count > 0)
      {
        if (listBox.SelectedIndex != -1)
        {
          listBox.Items.RemoveAt(listBox.SelectedIndex);
        }
        else
        {
          listBox.Items.RemoveAt(listBox.Items.Count - 1);
        }
      }
    }

    private void REMOVE_ENTRY_Click(object sender, EventArgs e)
    {
      RemoveEntry(GENERAL_CUSTOM_LOAD_BOX);
    }

    private void REMOVE_ENTRY_CUSTOM_Click(object sender, EventArgs e)
    {
      RemoveEntry(CUSTOM_LOAD_BOX);
    }
  }

  public class UnitInformation
  {
    public string Name { get; set; }
    public string Voltage { get; set; }
    public UnitDwellingArea DwellingArea { get; set; }
    public UnitGeneralLoads GeneralLoads { get; set; }
    public UnitCustomLoads CustomLoads { get; set; }
    public UnitACLoads ACLoads { get; set; }
    public UnitTotals Totals { get; set; }
  }

  public class UnitDwellingArea
  {
    public string FloorArea { get; set; }
    public bool ElectricHeater { get; set; }
    public bool ElectricDryer { get; set; }
    public bool ElectricOven { get; set; }
    public bool ElectricCooktop { get; set; }
  }

  public class UnitGeneralLoads
  {
    public string Lighting { get; set; }
    public string SmallAppliance { get; set; }
    public string Laundry { get; set; }
    public string Bathroom { get; set; }
    public string Dishwasher { get; set; }
    public string MicrowaveOven { get; set; }
    public string GarbageDisposal { get; set; }
    public string BathroomFans { get; set; }
    public string GarageDoorOpener { get; set; }
    public string Dryer { get; set; }
    public string Range { get; set; }
    public string Refrigerator { get; set; }
    public string Oven { get; set; }
    public string WaterHeater { get; set; }
    public string Cooktop { get; set; }

    public List<string> Customs { get; set; }
  }

  public class UnitCustomLoads
  {
    public List<string> Customs { get; set; }
  }

  public class UnitACLoads
  {
    public string Condenser { get; set; }
    public string FanCoil { get; set; }
  }

  public class UnitTotals
  {
    public string TotalGeneralLoad { get; set; }
    public string TotalACLoad { get; set; }
    public string SubtotalGeneralLoad { get; set; }
    public string ServiceLoad { get; set; }
  }
}