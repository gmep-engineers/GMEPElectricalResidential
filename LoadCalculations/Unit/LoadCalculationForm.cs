using GMEPElectricalResidential.HelperFiles;
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
using System.Xml.Linq;

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
    private int _dragIndex = -1;

    public LoadCalculationForm(LOAD_CALCULATION_FORM parent, int tabId, UnitInformation unitInformation = null)
    {
      InitializeComponent();
      SetDefaultValues();
      AddWaterMarks();
      DetectIncorrectInputs();
      DetectEnterPresses();
      SubscribeTextBoxesToTextChangedEvent(this.Controls);
      SubscribeComboBoxesToTextChangedEvent(this.Controls);

      _parent = parent;

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
      GENERAL_CUSTOM_LOAD_BOX.DrawMode = DrawMode.OwnerDrawFixed;
      GENERAL_CUSTOM_LOAD_BOX.DrawItem += GENERAL_CUSTOM_LOAD_BOX_DrawItem;
    }

    private void PopulateBoxFormItems(ListBox box, TextBox name, ComboBox multiplier, TextBox total)
    {
      if (box.SelectedIndex != -1)
      {
        var selectedItem = box.SelectedItem.ToString().Split(',');
        name.Text = selectedItem[0].Trim();
        total.Text = selectedItem[1].Trim();
        multiplier.Text = selectedItem[2].Trim();

        name.ForeColor = Color.Black;
        total.ForeColor = Color.Black;
        multiplier.ForeColor = Color.Black;
      }
    }

    private void UnitLoadCalculation_Load(object sender, EventArgs e)
    {
      if (!_unitNullFlag)
      {
        PopulateUserControlWithUnitInformation(_unitInformation);
      }
      else
      {
        var defaultValues = DefaultGeneralValuesProvider.GetDefaultGeneralValues();
        foreach (var value in defaultValues)
        {
          GENERAL_CUSTOM_LOAD_BOX.Items.Add(value);
        }
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

      if (unitInformation.GeneralLoads.Customs != null)
      {
        foreach (var load in unitInformation.GeneralLoads.Customs)
        {
          GENERAL_CUSTOM_LOAD_BOX.Items.Add(load.FormattedName());
        }
      }

      if (unitInformation.CustomLoads != null)
      {
        foreach (var load in unitInformation.CustomLoads)
        {
          CUSTOM_LOAD_BOX.Items.Add(load.FormattedName());
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
      OUTDOOR_HEATER_UNIT.Text = unitInformation.ACLoads.HeatingUnit?.Heating.ToString() ?? "0";
      OUTDOOR_HEATER_UNIT_AMOUNT.Text = unitInformation.ACLoads.HeatingUnit?.NumberOfUnits.ToString() ?? "1";

      // Set totals
      TOTAL_GENERAL_LOAD_CALCULATION.Text = unitInformation.Totals.TotalGeneralLoad.ToString();
      TOTAL_AC_LOAD_CALCULATION.Text = unitInformation.Totals.TotalACLoad.ToString();
      SUBTOTAL_GENERAL_LOAD_CALCULATION.Text = unitInformation.Totals.SubtotalGeneralLoad.ToString();
      TOTAL_CUSTOM_LOAD_CALCULATION.Text = unitInformation.Totals.CustomLoad.ToString();
      CALCULATED_LOAD_FOR_SERVICE.Text = unitInformation.Totals.ServiceLoad.ToString();

      // Set Checkbox
      DEMAND_40PC.Checked = unitInformation.Demand40Percent;
    }

    public UnitInformation RetrieveUnitInformation()
    {
      return _unitInformation;
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
      _unitInformation.Demand40Percent = DEMAND_40PC.Checked;
    }

    private void UpdateTotalCustomLoadCalculation()
    {
      var totalLoad = 0;
      foreach (var customLoad in _unitInformation.CustomLoads)
      {
        totalLoad += customLoad.GetTotal();
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

        var unitCustomLoad = new UnitLoad(split[0], split[1], split[2]);
        customs.Add(unitCustomLoad);
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
      unitGeneralLoadContainer.Lighting = new UnitLoad("General Lighting", lightingLoad.ToString(), "1");

      var previousCustoms = _unitInformation.GeneralLoads?.Customs;
      List<UnitLoad> customs = new List<UnitLoad>();

      foreach (var item in GENERAL_CUSTOM_LOAD_BOX.Items)
      {
        var index = GENERAL_CUSTOM_LOAD_BOX.Items.IndexOf(item);
        bool isCookingAppliance = false;
        if (previousCustoms != null && previousCustoms.Count > index)
        {
          isCookingAppliance = previousCustoms[index].IsCookingAppliance;
        }
        var split = item.ToString().Trim().Split(',');

        var unitGeneralCustomLoad = new UnitLoad(split[0], split[1], split[2], isCookingAppliance);
        customs.Add(unitGeneralCustomLoad);
      }

      // Find the removed item and update the isCookingAppliance flag for subsequent items
      if (previousCustoms != null && previousCustoms.Count > customs.Count)
      {
        for (int i = 0; i < previousCustoms.Count; i++)
        {
          if (i >= customs.Count || previousCustoms[i].Name != customs[i].Name)
          {
            // Found the removed item
            for (int j = i; j < customs.Count; j++)
            {
              customs[j].IsCookingAppliance = previousCustoms[j + 1].IsCookingAppliance;
            }
            break;
          }
        }
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

      var isUsing40Percent = _unitInformation.Demand40Percent;
      var generalLoads = _unitInformation.GeneralLoads;
      int totalLoad = generalLoads.OccupancyLighting();

      foreach (var customLoad in generalLoads.Customs)
      {
        if (!customLoad.IsCookingAppliance)
        {
          totalLoad += customLoad.GetTotal();
        }
      }

      var cookingApplianceInfo = generalLoads.GetCookingApplianceInfo();
      var numberOfApps = cookingApplianceInfo.NumberOfCookingAppliancesUnder8750 + cookingApplianceInfo.NumberOfCookingAppliancesOver8750;

      if (numberOfApps == 1)
      {
        totalLoad += cookingApplianceInfo.CookingAppliances[0].GetTotal();
      }
      else if (numberOfApps > 1)
      {
        totalLoad += (int)cookingApplianceInfo.TotalDemand;
      }

      _unitInformation.Totals.TotalGeneralLoad = totalLoad;
      TOTAL_GENERAL_LOAD_CALCULATION.Text = totalLoad.ToString();

      int subtotalLoad;
      if (isUsing40Percent && totalLoad > 10000)
      {
        double subtotal = 10000 + 0.4 * (totalLoad - 10000);
        subtotalLoad = (int)Math.Ceiling(subtotal);
      }
      else
      {
        subtotalLoad = totalLoad;
      }

      SUBTOTAL_GENERAL_LOAD_CALCULATION.Text = subtotalLoad.ToString();
      _unitInformation.Totals.SubtotalGeneralLoad = subtotalLoad;
    }

    private void DetectEnterPresses()
    {
      GENERAL_CUSTOM_NAME.KeyDown += TextBox_KeyDown;
      GENERAL_CUSTOM_MULTIPLIER.KeyDown += TextBox_KeyDown;
      GENERAL_CUSTOM_TOTAL.KeyDown += TextBox_KeyDown;

      CUSTOM_NAME.KeyDown += TextBox_KeyDown;
      CUSTOM_MULTIPLIER.KeyDown += TextBox_KeyDown;
      CUSTOM_TOTAL.KeyDown += TextBox_KeyDown;
    }

    private void TextBox_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Enter)
      {
        bool isEnterPressed = !e.Shift;

        if (sender == GENERAL_CUSTOM_TOTAL || sender == CUSTOM_TOTAL)
        {
          PerformMultiplication(GENERAL_CUSTOM_TOTAL, GENERAL_CUSTOM_MULTIPLIER);
          PerformMultiplication(CUSTOM_TOTAL, CUSTOM_MULTIPLIER);
        }
        if (sender == GENERAL_CUSTOM_NAME || sender == GENERAL_CUSTOM_MULTIPLIER || sender == GENERAL_CUSTOM_TOTAL)
        {
          AddEntry(GENERAL_CUSTOM_NAME, GENERAL_CUSTOM_MULTIPLIER, GENERAL_CUSTOM_TOTAL, GENERAL_CUSTOM_LOAD_BOX, isEnterPressed);
        }
        else if (sender == CUSTOM_NAME || sender == CUSTOM_MULTIPLIER || sender == CUSTOM_TOTAL)
        {
          AddEntry(CUSTOM_NAME, CUSTOM_MULTIPLIER, CUSTOM_TOTAL, CUSTOM_LOAD_BOX, isEnterPressed);
        }
        e.SuppressKeyPress = true;
      }
    }

    private void AddEntry(TextBox nameTextBox, ComboBox multiplierComboBox, TextBox totalBox, ListBox listBox, bool isEnterPressed = true)
    {
      string name = nameTextBox.Text;
      string total = totalBox.Text;
      string multiplier = multiplierComboBox.Text;

      bool proceed = HandleAddEntryToolTips(nameTextBox, totalBox, multiplierComboBox);

      if (!proceed) return;

      string newEntry = $"{name}, {total}, {multiplier}";

      bool added = AddOrUpdateEntryToListBox(listBox, name, newEntry);

      if (isEnterPressed)
      {
        SelectNextItem(listBox);
      }
      else
      {
        SelectPreviousItem(listBox);
      }

      if (added)
      {
        ResetFields(nameTextBox, multiplierComboBox, totalBox);
      }
      else
      {
        totalBox.Focus();
        totalBox.SelectAll();
      }

      if (_isLoaded) UpdateDataAndLoads();
    }

    private void ResetFields(TextBox nameTextBox, ComboBox multiplierComboBox, TextBox totalBox)
    {
      nameTextBox.Text = "";
      multiplierComboBox.Text = "1";
      totalBox.Text = "";
    }

    private void PerformMultiplication(TextBox totalBox, ComboBox multiplierBox)
    {
      if (totalBox.Text.Contains("*"))
      {
        var split = totalBox.Text.Split('*');
        if (split.Length == 2)
        {
          if (decimal.TryParse(split[0], out decimal total) && decimal.TryParse(split[1], out decimal multiplier))
          {
            totalBox.Text = (total * multiplier).ToString();
            multiplierBox.Text = multiplier.ToString();
          }
        }
      }
    }

    private static void SelectPreviousItem(ListBox listBox)
    {
      if (listBox.Items.Count > 0)
      {
        var selectedIndex = listBox.SelectedIndex;
        if (selectedIndex == 0)
        {
          listBox.SelectedIndex = listBox.Items.Count - 1;
        }
        else
        {
          listBox.SelectedIndex = selectedIndex - 1;
        }
      }
    }

    private static void SelectNextItem(ListBox listBox)
    {
      if (listBox.Items.Count > 0)
      {
        var selectedIndex = listBox.SelectedIndex;
        if (selectedIndex == listBox.Items.Count - 1)
        {
          listBox.SelectedIndex = 0;
        }
        else
        {
          listBox.SelectedIndex = selectedIndex + 1;
        }
      }
    }

    private static bool AddOrUpdateEntryToListBox(ListBox listBox, string name, string newEntry)
    {
      int existingIndex = -1;
      for (int i = 0; i < listBox.Items.Count; i++)
      {
        string item = listBox.Items[i].ToString();
        string[] parts = item.Split(new[] { ", " }, StringSplitOptions.None);
        if (parts.Length > 0 && parts[0] == name)
        {
          existingIndex = i;
          break;
        }
      }

      bool added = false;
      if (existingIndex >= 0)
      {
        listBox.Items[existingIndex] = newEntry;
      }
      else
      {
        listBox.Items.Add(newEntry);
        added = true;
      }

      return added;
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

      GENERAL_CUSTOM_TOTAL.Text = _VAWatermark;
      GENERAL_CUSTOM_TOTAL.ForeColor = Color.LightGray;
      GENERAL_CUSTOM_TOTAL.Enter += RemoveWatermark;
      GENERAL_CUSTOM_TOTAL.Leave += AddWatermark;

      CUSTOM_NAME.Text = _NameWatermark;
      CUSTOM_NAME.ForeColor = Color.LightGray;
      CUSTOM_NAME.Enter += RemoveWatermark;
      CUSTOM_NAME.Leave += AddWatermark;

      CUSTOM_TOTAL.Text += _VAWatermark;
      CUSTOM_TOTAL.ForeColor = Color.LightGray;
      CUSTOM_TOTAL.Enter += RemoveWatermark;
      CUSTOM_TOTAL.Leave += AddWatermark;
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
          if (textBox == GENERAL_CUSTOM_NAME || textBox == CUSTOM_NAME)
          {
            textBox.Text = _NameWatermark;
          }
          else if (textBox == GENERAL_CUSTOM_TOTAL || textBox == CUSTOM_TOTAL)
          {
            textBox.Text = _VAWatermark;
          }
          textBox.ForeColor = Color.LightGray;
        }
      }
    }

    private void SetDefaultValues()
    {
      string multiplierMessage = "To update the total and the amount at the same time, use the format: (amount in VA for 1 item) * (number of items) then press enter";

      _toolTip = new ToolTip();
      _toolTip.SetToolTip(TOTAL_GENERAL_LABEL, multiplierMessage);
      _toolTip.SetToolTip(TOTAL_CUSTOM_LABEL, multiplierMessage);

      VOLTAGE.SelectedIndex = 0;

      GENERAL_CUSTOM_LOAD_BOX.AllowDrop = true;
      CUSTOM_LOAD_BOX.AllowDrop = true;

      GENERAL_CUSTOM_LOAD_BOX.MouseDown += ListBox_MouseDown;
      CUSTOM_LOAD_BOX.MouseDown += ListBox_MouseDown;

      GENERAL_CUSTOM_LOAD_BOX.DragDrop += ListBox_DragDrop;
      CUSTOM_LOAD_BOX.DragDrop += ListBox_DragDrop;

      GENERAL_CUSTOM_LOAD_BOX.DragOver += ListBox_DragOver;
      CUSTOM_LOAD_BOX.DragOver += ListBox_DragOver;

      GENERAL_CUSTOM_LOAD_BOX.SelectedIndexChanged += ListBox_SelectedIndexChanged;
      CUSTOM_LOAD_BOX.SelectedIndexChanged += ListBox_SelectedIndexChanged;

      GENERAL_CUSTOM_LOAD_BOX.KeyDown += ListBox_KeyDown;
      CUSTOM_LOAD_BOX.KeyDown += ListBox_KeyDown;

      var parentTab = this.Parent as TabPage;
      if (parentTab != null)
      {
        parentTab.Text = _unitInformation.Name;
      }
    }

    private void ListBox_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.Delete)
      {
        ListBox listBox = (ListBox)sender;
        if (listBox.SelectedIndex != -1)
        {
          listBox.Items.RemoveAt(listBox.SelectedIndex);
          ClearListBoxInputs(listBox);
          if (_isLoaded) UpdateDataAndLoads();
        }
      }
    }

    private void ListBox_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (sender == GENERAL_CUSTOM_LOAD_BOX)
      {
        PopulateBoxFormItems(GENERAL_CUSTOM_LOAD_BOX, GENERAL_CUSTOM_NAME, GENERAL_CUSTOM_MULTIPLIER, GENERAL_CUSTOM_TOTAL);
      }
      else if (sender == CUSTOM_LOAD_BOX)
      {
        PopulateBoxFormItems(CUSTOM_LOAD_BOX, CUSTOM_NAME, CUSTOM_MULTIPLIER, CUSTOM_TOTAL);
      }
    }

    private void ListBox_DragOver(object sender, DragEventArgs e)
    {
      e.Effect = DragDropEffects.Move;
    }

    private void ListBox_DragDrop(object sender, DragEventArgs e)
    {
      ListBox listBox = (ListBox)sender;
      int dropIndex = listBox.IndexFromPoint(listBox.PointToClient(new Point(e.X, e.Y)));
      if (dropIndex >= 0 && dropIndex != _dragIndex)
      {
        object dragItem = listBox.Items[_dragIndex];
        listBox.Items.RemoveAt(_dragIndex);
        listBox.Items.Insert(dropIndex, dragItem);
        listBox.SelectedIndex = dropIndex;
      }
    }

    private void ListBox_MouseDown(object sender, MouseEventArgs e)
    {
      ListBox listBox = (ListBox)sender;
      if (e.Button == MouseButtons.Left && listBox.SelectedItem != null)
      {
        _dragIndex = listBox.IndexFromPoint(e.X, e.Y);
        if (_dragIndex >= 0)
        {
          var index = listBox.SelectedIndex;
          listBox.SelectedIndex = -1;
          listBox.SelectedIndex = index;
          listBox.DoDragDrop(listBox.Items[_dragIndex], DragDropEffects.Move);
        }
      }
    }

    private bool isNonNegative(string multiplier)
    {
      if (string.IsNullOrEmpty(multiplier))
      {
        return false;
      }

      if (decimal.TryParse(multiplier, out decimal result))
      {
        return result >= 0;
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
          parentTab.Text = _unitInformation.Name;
        }
      }
    }

    private void ELECTRIC_HEATER_CheckedChanged(object sender, EventArgs e)
    {
      var radioButton = sender as RadioButton;
      if (!radioButton.Checked) return;

      _unitInformation.DwellingArea.Heater = ApplianceType.Electric;
    }

    private void GAS_HEATER_CheckedChanged(object sender, EventArgs e)
    {
      var radioButton = sender as RadioButton;
      if (!radioButton.Checked) return;

      _unitInformation.DwellingArea.Heater = ApplianceType.Gas;
    }

    private void NA_HEATER_CheckedChanged(object sender, EventArgs e)
    {
      var radioButton = sender as RadioButton;
      if (!radioButton.Checked) return;

      _unitInformation.DwellingArea.Heater = ApplianceType.NA;
    }

    private void ELECTRIC_DRYER_CheckedChanged(object sender, EventArgs e)
    {
      var radioButton = sender as RadioButton;
      if (!radioButton.Checked) return;

      _unitInformation.DwellingArea.Dryer = ApplianceType.Electric;
    }

    private void GAS_DRYER_CheckedChanged(object sender, EventArgs e)
    {
      var radioButton = sender as RadioButton;
      if (!radioButton.Checked) return;

      _unitInformation.DwellingArea.Dryer = ApplianceType.Gas;
    }

    private void NA_DRYER_CheckedChanged(object sender, EventArgs e)
    {
      var radioButton = sender as RadioButton;
      if (!radioButton.Checked) return;

      _unitInformation.DwellingArea.Dryer = ApplianceType.NA;
    }

    private void ELECTRIC_OVEN_CheckedChanged(object sender, EventArgs e)
    {
      var radioButton = sender as RadioButton;
      if (!radioButton.Checked) return;

      _unitInformation.DwellingArea.Oven = ApplianceType.Electric;
    }

    private void GAS_OVEN_CheckedChanged(object sender, EventArgs e)
    {
      var radioButton = sender as RadioButton;
      if (!radioButton.Checked) return;

      _unitInformation.DwellingArea.Oven = ApplianceType.Gas;
    }

    private void NA_OVEN_CheckedChanged(object sender, EventArgs e)
    {
      var radioButton = sender as RadioButton;
      if (!radioButton.Checked) return;

      _unitInformation.DwellingArea.Oven = ApplianceType.NA;
    }

    private void ELECTRIC_COOKTOP_CheckedChanged(object sender, EventArgs e)
    {
      var radioButton = sender as RadioButton;
      if (!radioButton.Checked) return;

      _unitInformation.DwellingArea.Cooktop = ApplianceType.Electric;
    }

    private void GAS_COOKTOP_CheckedChanged(object sender, EventArgs e)
    {
      var radioButton = sender as RadioButton;
      if (!radioButton.Checked) return;

      _unitInformation.DwellingArea.Cooktop = ApplianceType.Gas;
    }

    private void NA_COOKTOP_CheckedChanged(object sender, EventArgs e)
    {
      var radioButton = sender as RadioButton;
      if (!radioButton.Checked) return;

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

    private bool HandleAddEntryToolTips(TextBox nameTextBox, TextBox totalBox, ComboBox multiplierComboBox)
    {
      string name = nameTextBox.Text;
      string total = totalBox.Text;
      string multiplier = multiplierComboBox.Text;

      if (string.IsNullOrEmpty(name) || name == _NameWatermark)
      {
        _toolTip.Show("You must enter a name.", nameTextBox, 0, -20, 2000);
        return false;
      }
      else
      {
        _toolTip.Hide(nameTextBox);
      }

      if (string.IsNullOrEmpty(total) || total == _VAWatermark)
      {
        _toolTip.Show("You must enter a VA.", totalBox, 0, -20, 2000);
        return false;
      }
      else
      {
        _toolTip.Hide(totalBox);
      }

      if (string.IsNullOrEmpty(multiplier) || !isNonNegative(multiplier))
      {
        _toolTip.Show("You must enter a multiplier that is a positive integer or 0.", multiplierComboBox, 0, -20, 2000);
        return false;
      }
      else
      {
        _toolTip.Hide(multiplierComboBox);
      }

      return true;
    }

    private void ADD_ENTRY_Click(object sender, EventArgs e)
    {
      AddEntry(GENERAL_CUSTOM_NAME, GENERAL_CUSTOM_MULTIPLIER, GENERAL_CUSTOM_TOTAL, GENERAL_CUSTOM_LOAD_BOX);
    }

    private void ADD_ENTRY_CUSTOM_Click(object sender, EventArgs e)
    {
      AddEntry(CUSTOM_NAME, CUSTOM_MULTIPLIER, CUSTOM_TOTAL, CUSTOM_LOAD_BOX);
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

        ClearListBoxInputs(listBox);
      }

      if (_isLoaded) UpdateDataAndLoads();
    }

    private void ClearListBoxInputs(ListBox listBox)
    {
      if (listBox == GENERAL_CUSTOM_LOAD_BOX)
      {
        GENERAL_CUSTOM_NAME.Text = "";
        GENERAL_CUSTOM_MULTIPLIER.Text = "1";
        GENERAL_CUSTOM_TOTAL.Text = "";
      }
      else if (listBox == CUSTOM_LOAD_BOX)
      {
        CUSTOM_NAME.Text = "";
        CUSTOM_MULTIPLIER.Text = "1";
        CUSTOM_TOTAL.Text = "";
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
        { 24, 648 },
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

    private void COOKING_APPLIANCE_Click(object sender, EventArgs e)
    {
      var selectedItem = GENERAL_CUSTOM_LOAD_BOX.Text;
      var unitLoad = _unitInformation.GeneralLoads.Customs.FirstOrDefault(load => load.FormattedName() == selectedItem);

      if (unitLoad != null)
      {
        unitLoad.IsCookingAppliance = !unitLoad.IsCookingAppliance;
      }

      if (_isLoaded) UpdateDataAndLoads();

      GENERAL_CUSTOM_LOAD_BOX.Invalidate();
    }

    private void GENERAL_CUSTOM_LOAD_BOX_DrawItem(object sender, DrawItemEventArgs e)
    {
      ColorTheCookingAppliances(e);
    }

    private void ColorTheCookingAppliances(DrawItemEventArgs e)
    {
      if (e.Index >= 0 && e.Index < GENERAL_CUSTOM_LOAD_BOX.Items.Count &&
          e.Index < _unitInformation.GeneralLoads.Customs.Count)
      {
        string item = GENERAL_CUSTOM_LOAD_BOX.Items[e.Index].ToString();
        bool isCookingAppliance = _unitInformation.GeneralLoads.Customs[e.Index].IsCookingAppliance;

        if (isCookingAppliance)
        {
          e.Graphics.FillRectangle(Brushes.LightPink, e.Bounds);
        }
        else
        {
          e.Graphics.FillRectangle(SystemBrushes.Window, e.Bounds);
        }

        e.Graphics.DrawString(item, e.Font, Brushes.Black, e.Bounds, StringFormat.GenericDefault);

        if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
        {
          using (Pen pen = new Pen(Color.Black, 1))
          {
            Rectangle rect = e.Bounds;
            rect.Inflate(-1, -1);
            e.Graphics.DrawRectangle(pen, rect);
          }
        }
      }

      e.DrawFocusRectangle();
    }

    private void DEMAND_40PC_CheckedChanged(object sender, EventArgs e)
    {
      if (DEMAND_40PC.Checked)
      {
        GENERAL_LIGHTING_TITLE.Visible = false;
        GENERAL_LIGHTING_GROUP_BOX.Visible = false;
        GENERAL_LIGHTING_TOTAL.Visible = false;
        GENERAL_LIGHTING_TOTAL_VA_LABEL.Visible = false;
        COOKING_APPLIANCE.Visible = false;
        LIGHTING_OTHER.Checked = true;

        SetAllGeneralLoadsAsNonCookingAppliances();
      }
      else
      {
        GENERAL_LIGHTING_TITLE.Visible = true;
        GENERAL_LIGHTING_GROUP_BOX.Visible = true;
        GENERAL_LIGHTING_TOTAL.Visible = true;
        GENERAL_LIGHTING_TOTAL_VA_LABEL.Visible = true;
        COOKING_APPLIANCE.Visible = true;

        UpdateDataAndLoads();
      }
    }

    private void SetAllGeneralLoadsAsNonCookingAppliances()
    {
      foreach (var unitLoad in _unitInformation.GeneralLoads.Customs)
      {
        unitLoad.IsCookingAppliance = false;
      }
      if (_isLoaded) UpdateDataAndLoads();
      GENERAL_CUSTOM_LOAD_BOX.Invalidate();
    }
  }

  public static class DefaultGeneralValuesProvider
  {
    public static List<string> GetDefaultGeneralValues()
    {
      return new List<string>()
        {
            "Small Appliance, 3000, 1",
            "Laundry, 1500, 1",
            "Bathroom, 0, 1",
            "Dishwasher, 1200, 1",
            "Microwave, 1500, 1",
            "Garbage Disposal, 1200, 1",
            "Bathroom Fans, 200, 1",
            "Garage Door Opener, 1200, 1",
            "Dryer, 5000, 1",
            "Oven, 8000, 1",
            "Refrigerator, 1000, 1",
            "Water Heater, 5000, 1"
        };
    }
  }

  public class UnitInformation
  {
    public string Name { get; set; }
    public string Voltage { get; set; }
    public int ID { get; set; }
    public bool Demand40Percent { get; set; } = true;
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

    public string FilteredFormattedName()
    {
      if (string.IsNullOrEmpty(Name))
      {
        return $"Unit - ID{ID}";
      }

      string filteredName = new string(Name.Where(c => char.IsLetterOrDigit(c)).ToArray());
      return $"Unit {filteredName} - ID{ID}";
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
      var firstValue = Math.Min(Lighting.GetTotal(), 3000);
      var secondValue = Math.Min(Math.Max(Lighting.GetTotal() - 3000, 0), 117000) * 0.35;
      var thirdValue = Math.Max(Lighting.GetTotal() - 120000, 0) * 0.25;
      return (int)Math.Ceiling(firstValue + secondValue + thirdValue);
    }

    private int HotelAndMotelLoad()
    {
      var firstValue = Math.Min(Lighting.GetTotal(), 20000) * 0.6;
      var secondValue = Math.Min(Math.Max(Lighting.GetTotal() - 20000, 0), 80000) * 0.5;
      var thirdValue = Math.Max(Lighting.GetTotal() - 100000, 0) * 0.35;
      return (int)Math.Ceiling(firstValue + secondValue + thirdValue);
    }

    private int WarehouseLoad()
    {
      var firstValue = Math.Min(Lighting.GetTotal(), 12500);
      var secondValue = Math.Max(Lighting.GetTotal() - 12500, 0) * 0.5;
      return (int)Math.Ceiling(firstValue + secondValue);
    }

    private int OtherLoad()
    {
      return Lighting.GetTotal();
    }

    public List<UnitLoad> GetCookingAppliances()
    {
      return Customs.Where(custom => custom.IsCookingAppliance && custom.GetIndividual() >= 1750 && custom.GetIndividual() <= 27000).ToList();
    }

    public List<UnitLoad> GetCookingAppliancesOver8750()
    {
      return Customs.Where(custom => custom.IsCookingAppliance && custom.GetIndividual() > 8750 && custom.GetIndividual() <= 27000).ToList();
    }

    public List<UnitLoad> GetCookingAppliancesUnder8750()
    {
      return Customs.Where(custom => custom.IsCookingAppliance && custom.GetIndividual() <= 8750 && custom.GetIndividual() >= 1750).ToList();
    }

    public CookingApplianceInfo GetCookingApplianceInfo()
    {
      var cookingApps = GetCookingAppliances();
      var cookingAppBuckets = GetCookingApplianceBuckets(cookingApps); // index 0, number of appliances from 1750VA - 8750VA | index 1, number of appliances from 8750VA - 27000VA
      var demandFactorBuckets = GetDemandFactorBuckets(cookingAppBuckets); // index 0, demand factor for appliances from 1750VA - 3500VA | index1, demand factor for appliances from 3500VA - 8750VA
      var maximumDemandOver8750 = GetMaximumDemand(cookingAppBuckets[1], GetAverageLoadFor8750to27000());
      var maximumDemandUnder8750 = GetDemand(demandFactorBuckets[0], demandFactorBuckets[1], cookingApps);
      var maximumDemand = maximumDemandUnder8750 + maximumDemandOver8750;

      var cookingApplianceInfo = new CookingApplianceInfo()
      {
        CookingAppliances = cookingApps,
        CookingAppliancesUnder8750 = GetCookingAppliancesUnder8750(),
        CookingAppliancesOver8750 = GetCookingAppliancesOver8750(),
        NumberOfCookingAppliancesUnder8750 = cookingAppBuckets[0],
        NumberOfCookingAppliancesOver8750 = cookingAppBuckets[1],
        DemandFactor1750to3500 = demandFactorBuckets[0],
        DemandFactor3500to8750 = demandFactorBuckets[1],
        TotalDemandOver8750 = maximumDemandOver8750,
        TotalDemandUnder8750 = maximumDemandUnder8750,
        TotalDemand = maximumDemand,
      };

      return cookingApplianceInfo;
    }

    private int GetDemand(double demandFactor1750to3500, double demandFactor3500to8750, List<UnitLoad> cookingApps)
    {
      var totalVA = 0;

      // Calculate totalVA for cookingApps with GetIndividual() value of 1750-3500
      var cookingApps1750to3500 = cookingApps.Where(app => app.GetIndividual() >= 1750 && app.GetIndividual() <= 3500);
      foreach (var app in cookingApps1750to3500)
      {
        totalVA += (int)Math.Round(app.GetTotal() * demandFactor1750to3500 / 100.0);
      }

      // Calculate totalVA for cookingApps with GetIndividual() value of 3500-8750
      var cookingApps3500to8750 = cookingApps.Where(app => app.GetIndividual() > 3500 && app.GetIndividual() <= 8750);
      foreach (var app in cookingApps3500to8750)
      {
        totalVA += (int)Math.Round(app.GetTotal() * demandFactor3500to8750 / 100.0);
      }

      return totalVA;
    }

    private double[] GetDemandFactorBuckets(int[] cookingAppBuckets)
    {
      var numOfApps1750to8750 = cookingAppBuckets[0];

      var demandFactorBuckets = new double[3];
      demandFactorBuckets[0] = GetDemandFactor(numOfApps1750to8750, "Column A");
      demandFactorBuckets[1] = GetDemandFactor(numOfApps1750to8750, "Column B");

      return demandFactorBuckets;
    }

    private int GetAverageLoadFor8750to27000()
    {
      var cookingApps8750to27000 = GetCookingAppliancesOver8750();
      int totalLoad = 0;
      int count = 0;

      foreach (var appliance in cookingApps8750to27000)
      {
        totalLoad += appliance.GetTotal();
        count += appliance.Multiplier;
      }

      if (count > 0)
      {
        return totalLoad / count;
      }
      else
      {
        return 0;
      }
    }

    private int[] GetCookingApplianceBuckets(List<UnitLoad> cookingApps)
    {
      var cookingApplianceBuckets = new int[2];
      foreach (var appliance in cookingApps)
      {
        int total = appliance.GetIndividual();
        if (total >= 1750 && total <= 8750)
        {
          cookingApplianceBuckets[0] += appliance.Multiplier;
        }
        else if (total > 8750 && total <= 27000)
        {
          cookingApplianceBuckets[1] += appliance.Multiplier;
        }
      }
      return cookingApplianceBuckets;
    }

    private double GetDemandFactor(int count, string column)
    {
      double[,] demandFactors = {
        {80, 80},
        {75, 65},
        {70, 55},
        {66, 50},
        {62, 45},
        {59, 43},
        {56, 40},
        {53, 36},
        {51, 35},
        {49, 34},
        {47, 32},
        {45, 32},
        {43, 32},
        {41, 32},
        {40, 32},
        {39, 28},
        {38, 28},
        {37, 28},
        {36, 28},
        {35, 28},
        {34, 28},
        {33, 26},
        {32, 26},
        {31, 26},
        {30, 26},
        {30, 24},
        {30, 24},
        {30, 24},
        {30, 24},
        {30, 24},
        {30, 22},
        {30, 22},
        {30, 22},
        {30, 22},
        {30, 22},
        {30, 22},
        {30, 22},
        {30, 22},
        {30, 22},
        {30, 22},
        {30, 20},
        {30, 20},
        {30, 20},
        {30, 20},
        {30, 20},
        {30, 20},
        {30, 20},
        {30, 20},
        {30, 20},
        {30, 20},
        {30, 18},
        {30, 18},
        {30, 18},
        {30, 18},
        {30, 18},
        {30, 18},
        {30, 18},
        {30, 18},
        {30, 18},
        {30, 18},
        {30, 16}
    };

      int rowIndex = count - 1;

      if (rowIndex > demandFactors.GetLength(0))
      {
        rowIndex = demandFactors.GetLength(0) - 1;
      }

      int columnIndex = column == "Column A" ? 0 : 1;

      if (rowIndex >= 0 && rowIndex < demandFactors.GetLength(0))
      {
        return demandFactors[rowIndex, columnIndex];
      }
      else
      {
        return 100;
      }
    }

    private int GetMaximumDemand(int count, int averageVA)
    {
      int averageKVA = (int)Math.Ceiling((double)averageVA / 1000);
      double[] maximumDemands = { 8, 11, 14, 17, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40 };

      int KVAover12 = averageKVA - 12;
      if (KVAover12 < 0)
      {
        KVAover12 = 0;
      }

      int percentIncrease = KVAover12 * 5;

      if (count > maximumDemands.GetLength(0) && count < 41)
      {
        return 15 + count;
      }
      else if (count > 40)
      {
        return 25 + 3 * count / 4;
      }

      if (count >= 1 && count <= maximumDemands.Length)
      {
        return (int)Math.Round(maximumDemands[count - 1] * (1.0 + percentIncrease / 100) * 1000);
      }
      else
      {
        return 0;
      }
    }
  }

  public class CookingApplianceInfo
  {
    public List<UnitLoad> CookingAppliances { get; set; }
    public List<UnitLoad> CookingAppliancesUnder8750 { get; set; }
    public List<UnitLoad> CookingAppliancesOver8750 { get; set; }

    public int NumberOfCookingAppliancesOver8750 { get; set; }
    public int NumberOfCookingAppliancesUnder8750 { get; set; }

    public double DemandFactor1750to3500 { get; set; }
    public double DemandFactor3500to8750 { get; set; }

    public double TotalDemandOver8750 { get; set; }
    public double TotalDemandUnder8750 { get; set; }
    public double TotalDemand { get; set; }
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
    public int Multiplier { get; set; }
    public int Total { get; set; }
    public string Name { get; set; }
    public bool IsCookingAppliance { get; set; }

    public UnitLoad(string name, string total, string multiplier, bool isCookingAppliance = false)
    {
      Multiplier = string.IsNullOrEmpty(multiplier) ? 0 : int.TryParse(multiplier, out int multiplierResult) ? multiplierResult : 0;
      Total = string.IsNullOrEmpty(total) ? 0 : int.TryParse(total, out int totalResult) ? totalResult : 0;
      Name = name;
      IsCookingAppliance = isCookingAppliance;
    }

    public int GetTotal()
    {
      return Multiplier == 0 ? 0 : Total;
    }

    public int GetIndividual()
    {
      if (Multiplier == 0)
      {
        return 0;
      }
      return Total / Multiplier;
    }

    public string FormattedName()
    {
      return $"{Name}, {Total}, {Multiplier}";
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

    public int SubtotalOfUnitType()
    {
      return TotalACLoad + TotalGeneralLoad + CustomLoad;
    }
  }

  public class HeatingUnit
  {
    public int Heating { get; set; }
    public int NumberOfUnits { get; set; }
  }
}