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

namespace GMEPElectricalResidential.LoadCalculations.Unit
{
  public partial class LoadCalculationForm : UserControl
  {
    private string _NameWatermark = "Enter name...";
    private string _VAWatermark = "Enter VA...";
    private ToolTip _toolTip;
    private UnitInformation _unitInformation;
    private LOAD_CALCULATION_FORM _parent;
    private bool _isLoaded = false;
    private bool _unitNullFlag = false;

    public LoadCalculationForm(LOAD_CALCULATION_FORM parent, int tabId, UnitInformation unitInformation = null)
    {
      InitializeComponent();
      SetDefaultValues();
      AddWaterMarks();
      DetectIncorrectInputs();
      DetectEnterPresses();
      SubscribeTextBoxesToTextChangedEvent(this.Controls);
      SubscribeComboBoxesToTextChangedEvent(this.Controls);
      SubscribeTextBoxesToTextEnterEvent(this.Controls);

      _parent = parent;
      _toolTip = new ToolTip();

      if (unitInformation != null)
      {
        _unitInformation = unitInformation;
      }
      else
      {
        _unitNullFlag = true;
        _unitInformation = new UnitInformation(tabId);
      }

      this.Load += new EventHandler(UnitLoadCalculation_Load);
    }

    private void UnitLoadCalculation_Load(object sender, EventArgs e)
    {
      if (!_unitNullFlag)
      {
        PopulateUserControlWithUnitInformation(_unitInformation);
      }
      _isLoaded = true;
      UpdateDataAndLoads();
    }

    private void PopulateUserControlWithUnitInformation(UnitInformation unitInformation)
    {
      if (unitInformation == null) return;

      // Set TextBox and ComboBox values with the saved data
      UNIT_NAME.Text = unitInformation.Name;
      VOLTAGE.Text = unitInformation.Voltage;

      // Set Radio Buttons
      ELECTRIC_HEATER.Checked = unitInformation.DwellingArea.Heater == ApplianceType.Electric;
      GAS_HEATER.Checked = unitInformation.DwellingArea.Heater == ApplianceType.Gas;
      NA_HEATER.Checked = unitInformation.DwellingArea.Heater == ApplianceType.NA;

      ELECTRIC_DRYER.Checked = unitInformation.DwellingArea.Dryer == ApplianceType.Electric;
      GAS_DRYER.Checked = unitInformation.DwellingArea.Dryer == ApplianceType.Gas;
      NA_DRYER.Checked = unitInformation.DwellingArea.Dryer == ApplianceType.NA;

      ELECTRIC_OVEN.Checked = unitInformation.DwellingArea.Oven == ApplianceType.Electric;
      GAS_OVEN.Checked = unitInformation.DwellingArea.Oven == ApplianceType.Gas;
      NA_OVEN.Checked = unitInformation.DwellingArea.Oven == ApplianceType.NA;

      ELECTRIC_COOKTOP.Checked = unitInformation.DwellingArea.Cooktop == ApplianceType.Electric;
      GAS_COOKTOP.Checked = unitInformation.DwellingArea.Cooktop == ApplianceType.Gas;
      NA_COOKTOP.Checked = unitInformation.DwellingArea.Cooktop == ApplianceType.NA;

      // Set area
      AREA.Text = unitInformation.DwellingArea.FloorArea.ToString();

      // Set general loads
      SMALL_APPLIANCE_VA.Text = unitInformation.GeneralLoads.SmallAppliance.VA.ToString();
      SMALL_APPLIANCE_MULTIPLIER.Text = unitInformation.GeneralLoads.SmallAppliance.Multiplier.ToString();
      LAUNDRY_VA.Text = unitInformation.GeneralLoads.Laundry.VA.ToString();
      LAUNDRY_MULTIPLIER.Text = unitInformation.GeneralLoads.Laundry.Multiplier.ToString();
      BATHROOM_VA.Text = unitInformation.GeneralLoads.Bathroom.VA.ToString();
      BATHROOM_MULTIPLIER.Text = unitInformation.GeneralLoads.Bathroom.Multiplier.ToString();
      DISHWASHER_VA.Text = unitInformation.GeneralLoads.Dishwasher.VA.ToString();
      DISHWASHER_MULTIPLIER.Text = unitInformation.GeneralLoads.Dishwasher.Multiplier.ToString();
      MICROWAVE_OVEN_VA.Text = unitInformation.GeneralLoads.MicrowaveOven.VA.ToString();
      MICROWAVE_OVEN_MULTIPLIER.Text = unitInformation.GeneralLoads.MicrowaveOven.Multiplier.ToString();
      GARBAGE_DISPOSAL_VA.Text = unitInformation.GeneralLoads.GarbageDisposal.VA.ToString();
      GARBAGE_DISPOSAL_MULTIPLIER.Text = unitInformation.GeneralLoads.GarbageDisposal.Multiplier.ToString();
      BATHROOM_FANS_VA.Text = unitInformation.GeneralLoads.BathroomFans.VA.ToString();
      BATHROOM_FANS_MULTIPLIER.Text = unitInformation.GeneralLoads.BathroomFans.Multiplier.ToString();
      GARAGE_DOOR_OPENER_VA.Text = unitInformation.GeneralLoads.GarageDoorOpener.VA.ToString();
      GARAGE_DOOR_OPENER_MULTIPLIER.Text = unitInformation.GeneralLoads.GarageDoorOpener.Multiplier.ToString();
      DRYER_VA.Text = unitInformation.GeneralLoads.Dryer.VA.ToString();
      DRYER_MULTIPLIER.Text = unitInformation.GeneralLoads.Dryer.Multiplier.ToString();
      RANGE_VA.Text = unitInformation.GeneralLoads.Range.VA.ToString();
      RANGE_MULTIPLIER.Text = unitInformation.GeneralLoads.Range.Multiplier.ToString();
      REFRIGERATOR_VA.Text = unitInformation.GeneralLoads.Refrigerator.VA.ToString();
      REFRIGERATOR_MULTIPLIER.Text = unitInformation.GeneralLoads.Refrigerator.Multiplier.ToString();
      OVEN_VA.Text = unitInformation.GeneralLoads.Oven.VA.ToString();
      OVEN_MULTIPLIER.Text = unitInformation.GeneralLoads.Oven.Multiplier.ToString();
      WATER_HEATER_VA.Text = unitInformation.GeneralLoads.WaterHeater.VA.ToString();
      WATER_HEATER_MULTIPLIER.Text = unitInformation.GeneralLoads.WaterHeater.Multiplier.ToString();
      COOKTOP_VA.Text = unitInformation.GeneralLoads.Cooktop.VA.ToString();
      COOKTOP_MULTIPLIER.Text = unitInformation.GeneralLoads.Cooktop.Multiplier.ToString();

      foreach (var load in unitInformation.GeneralLoads.Customs)
      {
        var entry = $"{load.Name}, {load.VA}, {load.Multiplier}";
        GENERAL_CUSTOM_LOAD_BOX.Items.Add(entry);
      }

      foreach (var load in unitInformation.CustomLoads)
      {
        if (load.Name != "Water Heater")
        {
          var entry = $"{load.Name}, {load.VA}, {load.Multiplier}";
          CUSTOM_LOAD_BOX.Items.Add(entry);
        }
      }

      foreach (var load in unitInformation.CustomLoads)
      {
        if (load.Name == "Water Heater")
        {
          WATER_HEATER_CHECK.Checked = false;
          WATER_HEATER_VA.Text = load.VA.ToString();
          WATER_HEATER_MULTIPLIER.Text = load.Multiplier.ToString();
          break;
        }
      }

      if (unitInformation.GeneralLoads.LightingOccupancyType == LightingOccupancyType.Dwelling)
      {
        LIGHTING_DWELLING.Checked = true;
      }
      else if (unitInformation.GeneralLoads.LightingOccupancyType == LightingOccupancyType.HotelAndMotel)
      {
        LIGHTING_HOTEL_MOTEL.Checked = true;
      }
      else if (unitInformation.GeneralLoads.LightingOccupancyType == LightingOccupancyType.Warehouse)
      {
        LIGHTING_WAREHOUSE.Checked = true;
      }
      else if (unitInformation.GeneralLoads.LightingOccupancyType == LightingOccupancyType.Other)
      {
        LIGHTING_OTHER.Checked = true;
      }

      // Set AC loads
      OUTDOOR_CONDENSER_VA.Text = unitInformation.ACLoads.Condenser.ToString();
      INDOOR_FAN_COIL_VA.Text = unitInformation.ACLoads.FanCoil.ToString();
      OUTDOOR_HEATER_UNIT.Text = unitInformation.ACLoads.HeatingUnit.Heating.ToString();
      OUTDOOR_HEATER_UNIT_AMOUNT.Text = unitInformation.ACLoads.HeatingUnit.NumberOfUnits.ToString();

      // Set totals
      TOTAL_GENERAL_LOAD_CALCULATION.Text = unitInformation.Totals.TotalGeneralLoad.ToString();
      TOTAL_AC_LOAD_CALCULATION.Text = unitInformation.Totals.TotalACLoad.ToString();
      SUBTOTAL_GENERAL_LOAD_CALCULATION.Text = unitInformation.Totals.SubtotalGeneralLoad.ToString();
      TOTAL_CUSTOM_LOAD_CALCULATION.Text = unitInformation.Totals.CustomLoad.ToString();
      CALCULATED_LOAD_FOR_SERVICE.Text = unitInformation.Totals.ServiceLoad.ToString();
    }

    public UnitInformation RetrieveUnitInformation()
    {
      return _unitInformation;
    }

    private void SubscribeTextBoxesToTextEnterEvent(Control.ControlCollection controls)
    {
      foreach (Control control in controls)
      {
        if (control is TextBox)
        {
          ((TextBox)control).MouseUp += TextBox_MouseUp;
        }
        else
        {
          SubscribeTextBoxesToTextEnterEvent(control.Controls);
        }
      }
    }

    private void TextBox_MouseUp(object sender, MouseEventArgs e)
    {
      if (sender is TextBox textBox && !textBox.ReadOnly)
      {
        textBox.SelectAll();
      }
    }

    protected override void OnVisibleChanged(EventArgs e)
    {
      base.OnVisibleChanged(e);

      if (this.Visible)
      {
        UNIT_NAME.Select();
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
      if (_isLoaded) UpdateDataAndLoads();
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

      UpdateServiceLoadDataAndCalculation();
      _parent.UpdateBuildingData(_unitInformation);
    }

    private void UpdateServiceLoadDataAndCalculation()
    {
      if (int.TryParse(SUBTOTAL_GENERAL_LOAD_CALCULATION.Text, out int subtotalGeneralLoad) &&
          int.TryParse(TOTAL_AC_LOAD_CALCULATION.Text, out int totalACLoad) &&
          int.TryParse(TOTAL_CUSTOM_LOAD_CALCULATION.Text, out int totalCustomLoad))
      {
        string voltageText = VOLTAGE.Text;
        if (voltageText.Length > 0 && voltageText[voltageText.Length - 1] == 'V')
        {
          voltageText = voltageText.Substring(0, voltageText.Length - 1);
        }

        if (int.TryParse(voltageText, out int voltage) && voltage != 0) // Avoid division by zero
        {
          var amperage = (int)Math.Ceiling((double)(subtotalGeneralLoad + totalACLoad + totalCustomLoad) / voltage);
          CALCULATED_LOAD_FOR_SERVICE.Text = amperage.ToString();
          _unitInformation.Totals.ServiceLoad = amperage;
        }
      }
    }

    private void UpdateDwellingData()
    {
      _unitInformation.DwellingArea.FloorArea = AREA.Text;
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
      _unitInformation.Totals.CustomLoad = totalLoad;
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
      _unitInformation.Totals.TotalACLoad = loadValue;
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

      int lightingLoad = 0;
      if (!string.IsNullOrEmpty(AREA.Text))
      {
        lightingLoad = int.Parse(AREA.Text) * 3;
      }

      unitGeneralLoadContainer.Lighting = new UnitLoad("General Lighting", lightingLoad, "1");
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

      if (LIGHTING_DWELLING.Checked)
      {
        _unitInformation.GeneralLoads.LightingOccupancyType = LightingOccupancyType.Dwelling;
      }
      else if (LIGHTING_HOTEL_MOTEL.Checked)
      {
        _unitInformation.GeneralLoads.LightingOccupancyType = LightingOccupancyType.HotelAndMotel;
      }
      else if (LIGHTING_WAREHOUSE.Checked)
      {
        _unitInformation.GeneralLoads.LightingOccupancyType = LightingOccupancyType.Warehouse;
      }
      else if (LIGHTING_OTHER.Checked)
      {
        _unitInformation.GeneralLoads.LightingOccupancyType = LightingOccupancyType.Other;
      }
    }

    private void UpdateTotalGeneralLoadCalculation()
    {
      if (_unitInformation == null || _unitInformation.GeneralLoads == null)
      {
        return;
      }

      var generalLoads = _unitInformation.GeneralLoads;

      int totalLoad = generalLoads.OccupancyLighting()
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

      _unitInformation.Totals.TotalGeneralLoad = totalLoad;
      TOTAL_GENERAL_LOAD_CALCULATION.Text = totalLoad.ToString();

      if (totalLoad <= 10000)
      {
        SUBTOTAL_GENERAL_LOAD_CALCULATION.Text = totalLoad.ToString();
        _unitInformation.Totals.SubtotalGeneralLoad = totalLoad;
      }
      else
      {
        double subtotal = 10000 + 0.4 * (totalLoad - 10000);
        int roundedSubtotal = (int)Math.Ceiling(subtotal);
        SUBTOTAL_GENERAL_LOAD_CALCULATION.Text = roundedSubtotal.ToString();
        _unitInformation.Totals.SubtotalGeneralLoad = roundedSubtotal;
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
      var parentTab = this.Parent as TabPage;
      if (parentTab != null)
      {
        parentTab.Text = _unitInformation.FormattedName();
      }
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

    private void UNIT_NAME_TextChanged(object sender, EventArgs e)
    {
      var textBox = sender as TextBox;
      if (textBox != null)
      {
        var parentTab = this.Parent as TabPage;
        if (parentTab != null)
        {
          _unitInformation.Name = textBox.Text;
          parentTab.Text = _unitInformation.FormattedName();
        }
      }
    }

    private void ELECTRIC_HEATER_CheckedChanged(object sender, EventArgs e)
    {
      var radioButton = sender as RadioButton;
      if (!radioButton.Checked) return;

      WATER_HEATER_VA.Text = "5000";

      _unitInformation.DwellingArea.Heater = ApplianceType.Electric;
    }

    private void GAS_HEATER_CheckedChanged(object sender, EventArgs e)
    {
      var radioButton = sender as RadioButton;
      if (!radioButton.Checked) return;

      WATER_HEATER_VA.Text = "180";

      _unitInformation.DwellingArea.Heater = ApplianceType.Gas;
    }

    private void NA_HEATER_CheckedChanged(object sender, EventArgs e)
    {
      var radioButton = sender as RadioButton;
      if (!radioButton.Checked) return;

      WATER_HEATER_VA.Text = "0";

      _unitInformation.DwellingArea.Heater = ApplianceType.NA;
    }

    private void ELECTRIC_DRYER_CheckedChanged(object sender, EventArgs e)
    {
      var radioButton = sender as RadioButton;
      if (!radioButton.Checked) return;

      DRYER_VA.Text = "5000";

      _unitInformation.DwellingArea.Dryer = ApplianceType.Electric;
    }

    private void GAS_DRYER_CheckedChanged(object sender, EventArgs e)
    {
      var radioButton = sender as RadioButton;
      if (!radioButton.Checked) return;

      DRYER_VA.Text = "180";

      _unitInformation.DwellingArea.Dryer = ApplianceType.Gas;
    }

    private void NA_DRYER_CheckedChanged(object sender, EventArgs e)
    {
      var radioButton = sender as RadioButton;
      if (!radioButton.Checked) return;

      DRYER_VA.Text = "0";

      _unitInformation.DwellingArea.Dryer = ApplianceType.NA;
    }

    private void ELECTRIC_OVEN_CheckedChanged(object sender, EventArgs e)
    {
      var radioButton = sender as RadioButton;
      if (!radioButton.Checked) return;

      OVEN_VA.Text = "8000";

      _unitInformation.DwellingArea.Oven = ApplianceType.Electric;
    }

    private void GAS_OVEN_CheckedChanged(object sender, EventArgs e)
    {
      var radioButton = sender as RadioButton;
      if (!radioButton.Checked) return;

      OVEN_VA.Text = "180";

      _unitInformation.DwellingArea.Oven = ApplianceType.Gas;
    }

    private void NA_OVEN_CheckedChanged(object sender, EventArgs e)
    {
      var radioButton = sender as RadioButton;
      if (!radioButton.Checked) return;

      OVEN_VA.Text = "0";

      _unitInformation.DwellingArea.Oven = ApplianceType.NA;
    }

    private void ELECTRIC_COOKTOP_CheckedChanged(object sender, EventArgs e)
    {
      var radioButton = sender as RadioButton;
      if (!radioButton.Checked) return;

      COOKTOP_VA.Text = "8000";

      _unitInformation.DwellingArea.Cooktop = ApplianceType.Electric;
    }

    private void GAS_COOKTOP_CheckedChanged(object sender, EventArgs e)
    {
      var radioButton = sender as RadioButton;
      if (!radioButton.Checked) return;

      COOKTOP_VA.Text = "180";

      _unitInformation.DwellingArea.Cooktop = ApplianceType.Gas;
    }

    private void NA_COOKTOP_CheckedChanged(object sender, EventArgs e)
    {
      var radioButton = sender as RadioButton;
      if (!radioButton.Checked) return;

      COOKTOP_VA.Text = "0";

      _unitInformation.DwellingArea.Cooktop = ApplianceType.NA;
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

      if (_isLoaded) UpdateDataAndLoads();
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

      if (_isLoaded) UpdateDataAndLoads();
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

      if (_isLoaded) UpdateDataAndLoads();
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
      if (_isLoaded) UpdateDataAndLoads();
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

    private void ESTIMATE_CONDENSER_Click(object sender, EventArgs e)
    {
      List<int> units;
      if (int.TryParse(AREA.Text, out int area))
      {
        int kBTU = (int)Math.Ceiling((area / 500.0) * 12);

        if (kBTU < 18)
        {
          kBTU = 18;
        }
        else if (kBTU % 6 != 0)
        {
          kBTU = ((int)(kBTU / 6) + 1) * 6;
        }

        units = BreakUpkBTU(kBTU);
      }
      else
      {
        units = new List<int> { 0 };
      }

      int condenserVA = GetCondenserVATotalFromUnits(units);

      OUTDOOR_CONDENSER_VA.Text = condenserVA.ToString();
    }

    private int GetCondenserVATotalFromUnits(List<int> units)
    {
      Dictionary<int, int> map = new Dictionary<int, int>
    {
        { 18, 2714 },
        { 24, 4048 },
        { 30, 3864 },
        { 36, 4945 },
        { 42, 5405 },
        { 48, 6026 },
        { 60, 7866 }
    };

      return units.Sum(unit => map.ContainsKey(unit) ? map[unit] : 0);
    }

    private int GetFanCoilVATotalFromUnits(List<int> units)
    {
      Dictionary<int, int> map = new Dictionary<int, int>
    {
        { 18, 648 },
        { 24, 848 },
        { 30, 840 },
        { 36, 816 },
        { 42, 984 },
        { 48, 1200 },
        { 60, 1632 }
    };

      return units.Sum(unit => map.ContainsKey(unit) ? map[unit] : 0);
    }

    private List<int> BreakUpkBTU(int kBTU)
    {
      var units = new List<int> { 60, 48, 42, 36, 30, 24, 18 };
      var bestCombination = new List<int>();
      var startingCombination = new List<int>();
      while (kBTU >= 300)
      {
        startingCombination.Add(60);
        kBTU -= 60;
      }

      void FindCombination(int remainingBTU, List<int> currentCombination, int startIndex)
      {
        if (remainingBTU == 0)
        {
          if (!bestCombination.Any() || currentCombination.Count < bestCombination.Count)
          {
            bestCombination = new List<int>(currentCombination);
          }
          return;
        }

        for (int i = startIndex; i < units.Count; i++)
        {
          if (units[i] <= remainingBTU)
          {
            var nextCombination = new List<int>(currentCombination) { units[i] };
            FindCombination(remainingBTU - units[i], nextCombination, i);
          }
        }
      }

      FindCombination(kBTU, startingCombination, 0);
      return bestCombination;
    }

    private void ESTIMATE_FAN_COIL_Click(object sender, EventArgs e)
    {
      List<int> units;
      if (int.TryParse(AREA.Text, out int area))
      {
        int kBTU = (int)Math.Ceiling((area / 500.0) * 12);

        if (kBTU < 18)
        {
          kBTU = 18;
        }
        else if (kBTU % 6 != 0)
        {
          kBTU = ((int)(kBTU / 6) + 1) * 6;
        }

        units = BreakUpkBTU(kBTU);
      }
      else
      {
        units = new List<int> { 0 };
      }

      int fanCoilVA = GetFanCoilVATotalFromUnits(units);

      INDOOR_FAN_COIL_VA.Text = fanCoilVA.ToString();
    }
  }

  public class UnitInformation
  {
    public string Name { get; set; }
    public string Voltage { get; set; }
    public int ID { get; set; }
    public UnitDwellingArea DwellingArea { get; set; }
    public UnitGeneralLoadContainer GeneralLoads { get; set; }
    public List<UnitLoad> CustomLoads { get; set; }
    public UnitACLoadContainer ACLoads { get; set; }
    public UnitTotalContainer Totals { get; set; }

    public UnitInformation(int id)
    {
      ID = id;
      DwellingArea = new UnitDwellingArea();
      GeneralLoads = new UnitGeneralLoadContainer();
      CustomLoads = new List<UnitLoad>();
      ACLoads = new UnitACLoadContainer();
      Totals = new UnitTotalContainer();
    }

    public string FormattedName()
    {
      return $"Unit {Name} - ID{ID}";
    }
  }

  public enum ApplianceType
  {
    Electric,
    Gas,
    NA
  }

  public class UnitDwellingArea
  {
    public string FloorArea { get; set; }
    public ApplianceType Heater { get; set; }
    public ApplianceType Dryer { get; set; }
    public ApplianceType Oven { get; set; }
    public ApplianceType Cooktop { get; set; }
  }

  public class UnitGeneralLoadContainer
  {
    public static string LightingCode = "220.42";
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
    public LightingOccupancyType LightingOccupancyType { get; set; }

    public int OccupancyLighting()
    {
      switch (LightingOccupancyType)
      {
        case LightingOccupancyType.Dwelling:
          return DwellingLoad();

        case LightingOccupancyType.HotelAndMotel:
          return HotelAndMotelLoad();

        case LightingOccupancyType.Warehouse:
          return WarehouseLoad();

        case LightingOccupancyType.Other:
          return OtherLoad();

        default:
          return 0;
      }
    }

    private int DwellingLoad()
    {
      var firstValue = Math.Min(Lighting.VA, 3000);
      var secondValue = Math.Min(Math.Max(Lighting.VA - 3000, 0), 117000) * 0.35;
      var thirdValue = Math.Max(Lighting.VA - 120000, 0) * 0.25;
      return (int)Math.Ceiling(firstValue + secondValue + thirdValue);
    }

    private int HotelAndMotelLoad()
    {
      var firstValue = Math.Min(Lighting.VA, 20000) * 0.6;
      var secondValue = Math.Min(Math.Max(Lighting.VA - 20000, 0), 80000) * 0.5;
      var thirdValue = Math.Max(Lighting.VA - 100000, 0) * 0.35;
      return (int)Math.Ceiling(firstValue + secondValue + thirdValue);
    }

    private int WarehouseLoad()
    {
      var firstValue = Math.Min(Lighting.VA, 12500);
      var secondValue = Math.Max(Lighting.VA - 12500, 0) * 0.5;
      return (int)Math.Ceiling(firstValue + secondValue);
    }

    private int OtherLoad()
    {
      return Lighting.VA;
    }
  }

  public enum LightingOccupancyType
  {
    Dwelling,
    HotelAndMotel,
    Warehouse,
    Other
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
    public int TotalGeneralLoad { get; set; }
    public int TotalACLoad { get; set; }
    public int SubtotalGeneralLoad { get; set; }
    public int CustomLoad { get; set; }
    public int ServiceLoad { get; set; }

    public int First10KVA()
    {
      return TotalGeneralLoad > 10000 ? 10000 : TotalGeneralLoad;
    }

    public int RemainderAt40Percent()
    {
      return TotalGeneralLoad < 10000 ? 0 : (int)Math.Ceiling((TotalGeneralLoad - 10000) * 0.4);
    }

    public int AmountOver10KVA()
    {
      return TotalGeneralLoad < 10000 ? 0 : TotalGeneralLoad - 10000;
    }

    public int ServiceRating()
    {
      int[] possibleValues = { 30, 60, 100, 125, 150, 200, 400, 600, 800, 1000, 1200, 1600, 2000, 2500, 3000 };
      int totalAmperage = ServiceLoad;
      int? serviceRating = possibleValues.FirstOrDefault(value => value >= totalAmperage);

      if (serviceRating == 0)
      {
        serviceRating = 3000;
        int increment = 1000;
        while (serviceRating <= totalAmperage)
        {
          serviceRating += increment;
        }
      }

      return serviceRating ?? 0;
    }
  }

  public class HeatingUnit
  {
    public int Heating { get; set; }
    public int NumberOfUnits { get; set; }
  }
}