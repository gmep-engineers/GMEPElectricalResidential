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
    private ToolTip _toolTip;
    private UnitInformation _unitInformation;

    public UnitLoadCalculation()
    {
      InitializeComponent();
      SetDefaultValues();
      AddWaterMarks();
      DetectIncorrectInputs();
      DetectEnterPresses();
      SubscribeTextBoxesToTextChangedEvent(this.Controls);
      SubscribeComboBoxesToTextChangedEvent(this.Controls);

      _toolTip = new ToolTip();
      _unitInformation = new UnitInformation();
      _unitInformation.GeneralLoads = new UnitGeneralLoadContainer();
      _unitInformation.Totals = new UnitTotalContainer();
      _unitInformation.ACLoads = new UnitACLoadContainer();
      _unitInformation.DwellingArea = new UnitDwellingArea();
      _unitInformation.GeneralLoads.LightingCode = "220.42";
    }

    protected override void OnVisibleChanged(EventArgs e)
    {
      base.OnVisibleChanged(e);

      if (this.Visible)
      {
        UpdateDataAndLoads();
      }
    }

    private void SubscribeComboBoxesToTextChangedEvent(Control.ControlCollection controls)
    {
      foreach (Control control in controls)
      {
        if (control is ComboBox comboBox)
        {
          comboBox.TextChanged += TextBox_TextChanged;
        }

        if (control.HasChildren)
        {
          SubscribeComboBoxesToTextChangedEvent(control.Controls);
        }
      }
    }

    private void SubscribeTextBoxesToTextChangedEvent(Control.ControlCollection controls)
    {
      foreach (Control control in controls)
      {
        if (control is TextBox textBox)
        {
          textBox.TextChanged += TextBox_TextChanged;
        }

        if (control.HasChildren)
        {
          SubscribeTextBoxesToTextChangedEvent(control.Controls);
        }
      }
    }

    private void TextBox_TextChanged(object sender, EventArgs e)
    {
      UpdateDataAndLoads();
    }

    private Tuple<int, string> GetLargestACInformation()
    {
      var numberOfUnits = _unitInformation.ACLoads.HeatingUnit.NumberOfUnits;
      var heaterVA = _unitInformation.ACLoads.HeatingUnit.Heating;
      var condenserVA = _unitInformation.ACLoads.Condenser;
      var fanCoilVA = _unitInformation.ACLoads.FanCoil;

      var totalACAndCooling = condenserVA + fanCoilVA;
      var heatingAt65pc = Math.Ceiling(heaterVA * 0.65);
      var heatingAt40pc = Math.Ceiling(heaterVA * 0.40);

      if (numberOfUnits < 4)
      {
        if (totalACAndCooling > heatingAt65pc)
        {
          return Tuple.Create(totalACAndCooling, "220.82(C)(1)");
        }
        else
        {
          return Tuple.Create((int)heatingAt65pc, "220.82(C)(4)");
        }
      }

      if (totalACAndCooling > heatingAt40pc)
      {
        return Tuple.Create(totalACAndCooling, "220.82(C)(1)");
      }
      else
      {
        return Tuple.Create((int)heatingAt40pc, "220.82(C)(5)");
      }
    }

    private void UpdateDataAndLoads()
    {
      UpdateGeneralData();
      UpdateDwellingData();

      UpdateGeneralLoadData();
      UpdateACLoadData();
      UpdateCustomLoadData();

      UpdateTotalGeneralLoadCalculation();
      UpdateTotalACLoadCalculation();
      UpdateTotalCustomLoadCalculation();
    }

    private void UpdateDwellingData()
    {
      var dwellingArea = new UnitDwellingArea();
      dwellingArea.FloorArea = AREA.Text;
      dwellingArea.ElectricHeater = ELECTRIC_HEATER.Checked;
      dwellingArea.ElectricDryer = ELECTRIC_DRYER.Checked;
      dwellingArea.ElectricCooktop = ELECTRIC_COOKTOP.Checked;
      dwellingArea.ElectricOven = ELECTRIC_OVEN.Checked;
      _unitInformation.DwellingArea = dwellingArea;
    }

    private void UpdateGeneralData()
    {
      _unitInformation.Name = UNIT_NAME.Text;
      _unitInformation.Voltage = VOLTAGE.Text;
    }

    private void UpdateTotalCustomLoadCalculation()
    {
      var totalLoad = 0;
      foreach (var customLoad in _unitInformation.CustomLoads)
      {
        totalLoad += customLoad.GetLoad();
      }
      _unitInformation.Totals.CustomLoad = totalLoad.ToString();
      TOTAL_CUSTOM_LOAD_CALCULATION.Text = totalLoad.ToString();
    }

    private void UpdateCustomLoadData()
    {
      List<UnitLoad> customs = new List<UnitLoad>();

      foreach (var item in CUSTOM_LOAD_BOX.Items)
      {
        var split = item.ToString().Trim().Split(',');
        var unitGeneralCustomLoad = new UnitLoad(split[0], split[1], split[2]);
        customs.Add(unitGeneralCustomLoad);
      }

      if (!WATER_HEATER_CHECK.Checked)
      {
        var name = "Water Heater";
        var load = WATER_HEATER_VA.Text;
        var multiplier = WATER_HEATER_MULTIPLIER.Text;
        var waterHeater = new UnitLoad(name, load, multiplier);
        customs.Add(waterHeater);
      }

      _unitInformation.CustomLoads = customs;
    }

    private void UpdateTotalACLoadCalculation()
    {
      var ACInformation = GetLargestACInformation();
      var loadValue = ACInformation.Item1;
      var loadCode = ACInformation.Item2;

      TOTAL_AC_LOAD_CALCULATION.Text = loadValue.ToString();
      _unitInformation.Totals.TotalACLoad = loadValue.ToString();

      _unitInformation.ACLoads.ElectricalCode = loadCode;
    }

    private void UpdateACLoadData()
    {
      UnitACLoadContainer unitACLoadContainer = new UnitACLoadContainer();

      int.TryParse(OUTDOOR_CONDENSER_VA.Text, out int condenser);
      int.TryParse(INDOOR_FAN_COIL_VA.Text, out int fanCoil);

      HeatingUnit heatingUnit = new HeatingUnit();

      int.TryParse(OUTDOOR_HEATER_UNIT.Text, out int heating);
      int.TryParse(OUTDOOR_HEATER_UNIT_AMOUNT.Text, out int numberOfUnits);

      heatingUnit.Heating = heating;
      heatingUnit.NumberOfUnits = numberOfUnits;

      unitACLoadContainer.Condenser = condenser;
      unitACLoadContainer.FanCoil = fanCoil;
      unitACLoadContainer.HeatingUnit = heatingUnit;

      _unitInformation.ACLoads = unitACLoadContainer;
    }

    private void UpdateGeneralLoadData()
    {
      UnitGeneralLoadContainer unitGeneralLoadContainer = new UnitGeneralLoadContainer();

      unitGeneralLoadContainer.Lighting = new UnitLoad("General Lighting", GENERAL_LIGHTING_TOTAL.Text, "1");
      unitGeneralLoadContainer.SmallAppliance = new UnitLoad("Small Appliance", SMALL_APPLIANCE_VA.Text, SMALL_APPLIANCE_MULTIPLIER.Text);
      unitGeneralLoadContainer.Laundry = new UnitLoad("Laundry", LAUNDRY_VA.Text, LAUNDRY_MULTIPLIER.Text);
      unitGeneralLoadContainer.Bathroom = new UnitLoad("Bathroom", BATHROOM_VA.Text, BATHROOM_MULTIPLIER.Text);
      unitGeneralLoadContainer.Dishwasher = new UnitLoad("Dishwasher", DISHWASHER_VA.Text, DISHWASHER_MULTIPLIER.Text);
      unitGeneralLoadContainer.MicrowaveOven = new UnitLoad("Microwave Oven", MICROWAVE_OVEN_VA.Text, MICROWAVE_OVEN_MULTIPLIER.Text);
      unitGeneralLoadContainer.GarbageDisposal = new UnitLoad("Garbage Disposal", GARBAGE_DISPOSAL_VA.Text, GARBAGE_DISPOSAL_MULTIPLIER.Text);
      unitGeneralLoadContainer.BathroomFans = new UnitLoad("Bathroom Fans", BATHROOM_FANS_VA.Text, BATHROOM_FANS_MULTIPLIER.Text);
      unitGeneralLoadContainer.GarageDoorOpener = new UnitLoad("Garage Door Opener", GARAGE_DOOR_OPENER_VA.Text, GARAGE_DOOR_OPENER_MULTIPLIER.Text);
      unitGeneralLoadContainer.Dryer = new UnitLoad("Dryer", DRYER_VA.Text, DRYER_MULTIPLIER.Text);
      unitGeneralLoadContainer.Range = new UnitLoad("Range", RANGE_VA.Text, RANGE_MULTIPLIER.Text);
      unitGeneralLoadContainer.Refrigerator = new UnitLoad("Refrigerator", REFRIGERATOR_VA.Text, REFRIGERATOR_MULTIPLIER.Text);
      unitGeneralLoadContainer.Oven = new UnitLoad("Oven", OVEN_VA.Text, OVEN_MULTIPLIER.Text);
      unitGeneralLoadContainer.Cooktop = new UnitLoad("Cooktop", COOKTOP_VA.Text, COOKTOP_MULTIPLIER.Text);

      if (WATER_HEATER_CHECK.Checked)
      {
        unitGeneralLoadContainer.WaterHeater = new UnitLoad("Water Heater", WATER_HEATER_VA.Text, WATER_HEATER_MULTIPLIER.Text);
      }
      else
      {
        unitGeneralLoadContainer.WaterHeater = new UnitLoad("Water Heater", "0", WATER_HEATER_MULTIPLIER.Text);
      }

      List<UnitLoad> customs = new List<UnitLoad>();

      foreach (var item in GENERAL_CUSTOM_LOAD_BOX.Items)
      {
        var split = item.ToString().Trim().Split(',');
        var unitGeneralCustomLoad = new UnitLoad(split[0], split[1], split[2]);
        customs.Add(unitGeneralCustomLoad);
      }

      unitGeneralLoadContainer.Customs = customs;

      _unitInformation.GeneralLoads = unitGeneralLoadContainer;
    }

    private void UpdateTotalGeneralLoadCalculation()
    {
      if (_unitInformation == null || _unitInformation.GeneralLoads == null)
      {
        return;
      }

      var generalLoads = _unitInformation.GeneralLoads;

      int totalLoad = generalLoads.Lighting.GetLoad()
                      + generalLoads.SmallAppliance.GetLoad()
                      + generalLoads.Laundry.GetLoad()
                      + generalLoads.Bathroom.GetLoad()
                      + generalLoads.Dishwasher.GetLoad()
                      + generalLoads.MicrowaveOven.GetLoad()
                      + generalLoads.GarbageDisposal.GetLoad()
                      + generalLoads.BathroomFans.GetLoad()
                      + generalLoads.GarageDoorOpener.GetLoad()
                      + generalLoads.Dryer.GetLoad()
                      + generalLoads.Range.GetLoad()
                      + generalLoads.Refrigerator.GetLoad()
                      + generalLoads.Oven.GetLoad()
                      + generalLoads.Cooktop.GetLoad()
                      + generalLoads.WaterHeater.GetLoad();

      foreach (var customLoad in generalLoads.Customs)
      {
        totalLoad += customLoad.GetLoad();
      }

      _unitInformation.Totals.TotalGeneralLoad = totalLoad.ToString();
      TOTAL_GENERAL_LOAD_CALCULATION.Text = totalLoad.ToString();

      if (totalLoad <= 10000)
      {
        SUBTOTAL_GENERAL_LOAD_CALCULATION.Text = totalLoad.ToString();
        _unitInformation.Totals.SubtotalGeneralLoad = totalLoad.ToString();
      }
      else
      {
        double subtotal = 10000 + 0.4 * (totalLoad - 10000);
        int roundedSubtotal = (int)Math.Ceiling(subtotal);
        SUBTOTAL_GENERAL_LOAD_CALCULATION.Text = roundedSubtotal.ToString();
        _unitInformation.Totals.SubtotalGeneralLoad = roundedSubtotal.ToString();
      }
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
        if (control is TextBox textBox && (textBox.Name.EndsWith("VA") || textBox.Name == "AREA"))
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
      UpdateGeneralLighting();
    }

    private void UpdateGeneralLighting()
    {
      bool isLightingHotelMotel = LIGHTING_HOTEL_MOTEL.Checked;
      bool isLightingWarehouse = LIGHTING_WAREHOUSE.Checked;
      bool isOther = LIGHTING_OTHER.Checked;

      var occType = "Dwelling";
      if (isLightingHotelMotel)
      {
        occType = "Hotel and Motel";
      }
      else if (isLightingWarehouse)
      {
        occType = "Warehouse";
      }
      else if (isOther)
      {
        occType = "Other";
      }

      List<int> lightingLoads;
      if (AREA != null && int.TryParse(AREA.Text, out int floorArea))
      {
        lightingLoads = CalculateLightingLoad(occType, floorArea * 3);
      }
      else
      {
        lightingLoads = CalculateLightingLoad(occType, 0);
      }

      var total = SumListInts(lightingLoads);
      GENERAL_LIGHTING_TOTAL.Text = total.ToString();

      UpdateDataAndLoads();
    }

    private int SumListInts(List<int> lightingLoads)
    {
      int total = 0;
      foreach (int i in lightingLoads)
      {
        total += i;
      }
      return total;
    }

    public List<int> CalculateLightingLoad(string occupancyType, int lightingLoad)
    {
      List<int> calculatedLoads = new List<int>();

      switch (occupancyType)
      {
        case "Dwelling":
          calculatedLoads.Add((int)Math.Ceiling(Math.Min(lightingLoad, 3000) * 1.0));
          calculatedLoads.Add((int)Math.Ceiling(Math.Min(Math.Max(lightingLoad - 3000, 0), 117000) * 0.35));
          calculatedLoads.Add((int)Math.Ceiling(Math.Max(lightingLoad - 120000, 0) * 0.25));
          break;

        case "Hotel and Motel":
          calculatedLoads.Add((int)Math.Ceiling(Math.Min(lightingLoad, 20000) * 0.60));
          calculatedLoads.Add((int)Math.Ceiling(Math.Min(Math.Max(lightingLoad - 20000, 0), 80000) * 0.50));
          calculatedLoads.Add((int)Math.Ceiling(Math.Max(lightingLoad - 100000, 0) * 0.35));
          break;

        case "Warehouse":
          calculatedLoads.Add((int)Math.Ceiling(Math.Min(lightingLoad, 12500) * 1.0));
          calculatedLoads.Add((int)Math.Ceiling(Math.Max(lightingLoad - 12500, 0) * 0.50));
          break;

        default:
          calculatedLoads.Add(lightingLoad);
          break;
      }

      return calculatedLoads;
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

      UpdateDataAndLoads();
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

      UpdateDataAndLoads();
    }

    private void REMOVE_ENTRY_Click(object sender, EventArgs e)
    {
      RemoveEntry(GENERAL_CUSTOM_LOAD_BOX);
    }

    private void REMOVE_ENTRY_CUSTOM_Click(object sender, EventArgs e)
    {
      RemoveEntry(CUSTOM_LOAD_BOX);
    }

    private void WATER_HEATER_CHECK_CheckedChanged(object sender, EventArgs e)
    {
      UpdateDataAndLoads();
    }

    private void LIGHTING_DWELLING_CheckedChanged(object sender, EventArgs e)
    {
      UpdateGeneralLighting();
    }

    private void LIGHTING_HOTEL_MOTEL_CheckedChanged(object sender, EventArgs e)
    {
      UpdateGeneralLighting();
    }

    private void LIGHTING_WAREHOUSE_CheckedChanged(object sender, EventArgs e)
    {
      UpdateGeneralLighting();
    }

    private void LIGHTING_OTHER_CheckedChanged(object sender, EventArgs e)
    {
      UpdateGeneralLighting();
    }
  }

  public class UnitInformation
  {
    public string Name { get; set; }
    public string Voltage { get; set; }
    public UnitDwellingArea DwellingArea { get; set; }
    public UnitGeneralLoadContainer GeneralLoads { get; set; }
    public List<UnitLoad> CustomLoads { get; set; }
    public UnitACLoadContainer ACLoads { get; set; }
    public UnitTotalContainer Totals { get; set; }
  }

  public class UnitDwellingArea
  {
    public string FloorArea { get; set; }
    public bool ElectricHeater { get; set; }
    public bool ElectricDryer { get; set; }
    public bool ElectricOven { get; set; }
    public bool ElectricCooktop { get; set; }
  }

  public class UnitGeneralLoadContainer
  {
    public UnitLoad Lighting { get; set; }
    public UnitLoad SmallAppliance { get; set; }
    public UnitLoad Laundry { get; set; }
    public UnitLoad Bathroom { get; set; }
    public UnitLoad Dishwasher { get; set; }
    public UnitLoad MicrowaveOven { get; set; }
    public UnitLoad GarbageDisposal { get; set; }
    public UnitLoad BathroomFans { get; set; }
    public UnitLoad GarageDoorOpener { get; set; }
    public UnitLoad Dryer { get; set; }
    public UnitLoad Range { get; set; }
    public UnitLoad Refrigerator { get; set; }
    public UnitLoad Oven { get; set; }
    public UnitLoad WaterHeater { get; set; }
    public UnitLoad Cooktop { get; set; }
    public List<UnitLoad> Customs { get; set; }
    public string LightingCode { get; set; }
  }

  public class UnitLoad
  {
    public int VA { get; set; }
    public int Multiplier { get; set; }
    public string Name { get; set; }

    public UnitLoad(string name, object va, object multiplier)
    {
      VA = va is int ? (int)va : int.TryParse(va.ToString(), out int vaResult) ? vaResult : 0;
      Multiplier = multiplier is int ? (int)multiplier : int.TryParse(multiplier.ToString(), out int multiplierResult) ? multiplierResult : 0;
      Name = name;
    }

    public int GetLoad()
    {
      int load = VA * Multiplier;
      return (load != 0) ? load : 0;
    }
  }

  public class UnitACLoadContainer
  {
    public int Condenser { get; set; }
    public int FanCoil { get; set; }
    public HeatingUnit HeatingUnit { get; set; }
    public string ElectricalCode { get; set; }
  }

  public class UnitTotalContainer
  {
    public string TotalGeneralLoad { get; set; }
    public string TotalACLoad { get; set; }
    public string SubtotalGeneralLoad { get; set; }
    public string CustomLoad { get; set; }
    public string ServiceLoad { get; set; }
  }

  public class HeatingUnit
  {
    public int Heating { get; set; }
    public int NumberOfUnits { get; set; }
  }
}