namespace GMEPElectricalResidential
{
  partial class DistributionForm
  {
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.label1 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.CONFIGURATION = new System.Windows.Forms.ComboBox();
      this.label3 = new System.Windows.Forms.Label();
      this.KAIC = new System.Windows.Forms.ComboBox();
      this.label4 = new System.Windows.Forms.Label();
      this.TABS = new System.Windows.Forms.TabControl();
      this.SIZE = new System.Windows.Forms.ComboBox();
      this.NEW = new System.Windows.Forms.RadioButton();
      this.EXISTING = new System.Windows.Forms.RadioButton();
      this.STATUS = new System.Windows.Forms.GroupBox();
      this.panel1 = new System.Windows.Forms.Panel();
      this.DISTRIBUTION_NAME = new System.Windows.Forms.TextBox();
      this.BREAKER_PANELS = new System.Windows.Forms.Button();
      this.BREAKER_LOAD_CALCULATIONS = new System.Windows.Forms.Button();
      this.DELETE_BREAKER = new System.Windows.Forms.Button();
      this.ADD_BREAKER = new System.Windows.Forms.Button();
      this.PARENT = new System.Windows.Forms.ComboBox();
      this.label5 = new System.Windows.Forms.Label();
      this.SET_DISTRIBUTION_LOCATION = new System.Windows.Forms.Button();
      this.BREAKER_EQUIPMENT = new System.Windows.Forms.Button();
      this.DISTANCE_FROM_PARENT_LABEL = new System.Windows.Forms.Label();
      this.DISTANCE_FROM_PARENT_PANEL = new System.Windows.Forms.Panel();
      this.DISTANCE_FROM_PARENT = new System.Windows.Forms.TextBox();
      this.STATUS.SuspendLayout();
      this.panel1.SuspendLayout();
      this.DISTANCE_FROM_PARENT_PANEL.SuspendLayout();
      this.SuspendLayout();
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(9, 84);
      this.label1.Margin = new System.Windows.Forms.Padding(0);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(27, 13);
      this.label1.TabIndex = 1;
      this.label1.Text = "Size";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(9, 126);
      this.label2.Margin = new System.Windows.Forms.Padding(2);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(69, 13);
      this.label2.TabIndex = 2;
      this.label2.Text = "Configuration";
      // 
      // CONFIGURATION
      // 
      this.CONFIGURATION.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.CONFIGURATION.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.CONFIGURATION.FormattingEnabled = true;
      this.CONFIGURATION.Items.AddRange(new object[] {
            "120/208V 1PH 3W",
            "120/240V 1PH 3W",
            "120/208V 3PH 4W",
            "120/240V 3PH 4W",
            "277/480V 3PH 4W"});
      this.CONFIGURATION.Location = new System.Drawing.Point(6, 141);
      this.CONFIGURATION.Margin = new System.Windows.Forms.Padding(2);
      this.CONFIGURATION.Name = "CONFIGURATION";
      this.CONFIGURATION.Size = new System.Drawing.Size(240, 21);
      this.CONFIGURATION.TabIndex = 4;
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(8, 166);
      this.label3.Margin = new System.Windows.Forms.Padding(2);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(31, 13);
      this.label3.TabIndex = 5;
      this.label3.Text = "KAIC";
      // 
      // KAIC
      // 
      this.KAIC.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.KAIC.FormattingEnabled = true;
      this.KAIC.Items.AddRange(new object[] {
            "N/A",
            "10",
            "22",
            "42",
            "65"});
      this.KAIC.Location = new System.Drawing.Point(6, 181);
      this.KAIC.Margin = new System.Windows.Forms.Padding(2);
      this.KAIC.Name = "KAIC";
      this.KAIC.Size = new System.Drawing.Size(240, 21);
      this.KAIC.TabIndex = 5;
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.BackColor = System.Drawing.Color.Transparent;
      this.label4.Location = new System.Drawing.Point(9, 6);
      this.label4.Margin = new System.Windows.Forms.Padding(0);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(35, 13);
      this.label4.TabIndex = 9;
      this.label4.Text = "Name";
      // 
      // TABS
      // 
      this.TABS.Location = new System.Drawing.Point(6, 314);
      this.TABS.Margin = new System.Windows.Forms.Padding(0);
      this.TABS.Name = "TABS";
      this.TABS.SelectedIndex = 0;
      this.TABS.Size = new System.Drawing.Size(240, 263);
      this.TABS.TabIndex = 8;
      // 
      // SIZE
      // 
      this.SIZE.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.SIZE.FormattingEnabled = true;
      this.SIZE.Items.AddRange(new object[] {
            "100A",
            "200A",
            "400A",
            "600A",
            "800A",
            "1000A",
            "1200A",
            "1600A",
            "2000A"});
      this.SIZE.Location = new System.Drawing.Point(6, 99);
      this.SIZE.Margin = new System.Windows.Forms.Padding(2);
      this.SIZE.Name = "SIZE";
      this.SIZE.Size = new System.Drawing.Size(240, 21);
      this.SIZE.TabIndex = 3;
      // 
      // NEW
      // 
      this.NEW.AutoSize = true;
      this.NEW.Checked = true;
      this.NEW.Location = new System.Drawing.Point(10, 17);
      this.NEW.Name = "NEW";
      this.NEW.Size = new System.Drawing.Size(47, 17);
      this.NEW.TabIndex = 1;
      this.NEW.TabStop = true;
      this.NEW.Text = "New";
      this.NEW.UseVisualStyleBackColor = true;
      // 
      // EXISTING
      // 
      this.EXISTING.AutoSize = true;
      this.EXISTING.Location = new System.Drawing.Point(58, 17);
      this.EXISTING.Margin = new System.Windows.Forms.Padding(0);
      this.EXISTING.Name = "EXISTING";
      this.EXISTING.Size = new System.Drawing.Size(61, 17);
      this.EXISTING.TabIndex = 2;
      this.EXISTING.TabStop = true;
      this.EXISTING.Text = "Existing";
      this.EXISTING.UseVisualStyleBackColor = true;
      // 
      // STATUS
      // 
      this.STATUS.Controls.Add(this.NEW);
      this.STATUS.Controls.Add(this.EXISTING);
      this.STATUS.Location = new System.Drawing.Point(121, 6);
      this.STATUS.Name = "STATUS";
      this.STATUS.Padding = new System.Windows.Forms.Padding(0);
      this.STATUS.Size = new System.Drawing.Size(125, 42);
      this.STATUS.TabIndex = 1;
      this.STATUS.TabStop = false;
      this.STATUS.Text = "Status";
      // 
      // panel1
      // 
      this.panel1.BackColor = System.Drawing.SystemColors.Window;
      this.panel1.Controls.Add(this.DISTRIBUTION_NAME);
      this.panel1.Location = new System.Drawing.Point(6, 22);
      this.panel1.Name = "panel1";
      this.panel1.Padding = new System.Windows.Forms.Padding(3);
      this.panel1.Size = new System.Drawing.Size(105, 21);
      this.panel1.TabIndex = 0;
      // 
      // DISTRIBUTION_NAME
      // 
      this.DISTRIBUTION_NAME.BorderStyle = System.Windows.Forms.BorderStyle.None;
      this.DISTRIBUTION_NAME.Location = new System.Drawing.Point(6, 4);
      this.DISTRIBUTION_NAME.Name = "DISTRIBUTION_NAME";
      this.DISTRIBUTION_NAME.Size = new System.Drawing.Size(93, 13);
      this.DISTRIBUTION_NAME.TabIndex = 0;
      // 
      // BREAKER_PANELS
      // 
      this.BREAKER_PANELS.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.BREAKER_PANELS.Location = new System.Drawing.Point(6, 612);
      this.BREAKER_PANELS.Name = "BREAKER_PANELS";
      this.BREAKER_PANELS.Size = new System.Drawing.Size(240, 23);
      this.BREAKER_PANELS.TabIndex = 11;
      this.BREAKER_PANELS.Text = "Breaker Panels";
      this.BREAKER_PANELS.UseVisualStyleBackColor = true;
      // 
      // BREAKER_LOAD_CALCULATIONS
      // 
      this.BREAKER_LOAD_CALCULATIONS.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.BREAKER_LOAD_CALCULATIONS.Location = new System.Drawing.Point(6, 641);
      this.BREAKER_LOAD_CALCULATIONS.Name = "BREAKER_LOAD_CALCULATIONS";
      this.BREAKER_LOAD_CALCULATIONS.Size = new System.Drawing.Size(240, 23);
      this.BREAKER_LOAD_CALCULATIONS.TabIndex = 12;
      this.BREAKER_LOAD_CALCULATIONS.Text = "Breaker Load Calculations";
      this.BREAKER_LOAD_CALCULATIONS.UseVisualStyleBackColor = true;
      // 
      // DELETE_BREAKER
      // 
      this.DELETE_BREAKER.BackColor = System.Drawing.Color.IndianRed;
      this.DELETE_BREAKER.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.DELETE_BREAKER.Location = new System.Drawing.Point(129, 583);
      this.DELETE_BREAKER.Name = "DELETE_BREAKER";
      this.DELETE_BREAKER.Size = new System.Drawing.Size(117, 23);
      this.DELETE_BREAKER.TabIndex = 10;
      this.DELETE_BREAKER.Text = "Delete Breaker";
      this.DELETE_BREAKER.UseVisualStyleBackColor = false;
      this.DELETE_BREAKER.Click += new System.EventHandler(this.DELETE_CHILD_Click);
      // 
      // ADD_BREAKER
      // 
      this.ADD_BREAKER.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.ADD_BREAKER.Location = new System.Drawing.Point(6, 583);
      this.ADD_BREAKER.Name = "ADD_BREAKER";
      this.ADD_BREAKER.Size = new System.Drawing.Size(113, 23);
      this.ADD_BREAKER.TabIndex = 9;
      this.ADD_BREAKER.Text = "Add Breaker";
      this.ADD_BREAKER.UseVisualStyleBackColor = true;
      this.ADD_BREAKER.Click += new System.EventHandler(this.ADD_CHILD_Click);
      // 
      // PARENT
      // 
      this.PARENT.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.PARENT.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.PARENT.FormattingEnabled = true;
      this.PARENT.Items.AddRange(new object[] {
            "Main"});
      this.PARENT.Location = new System.Drawing.Point(6, 61);
      this.PARENT.Margin = new System.Windows.Forms.Padding(2);
      this.PARENT.Name = "PARENT";
      this.PARENT.Size = new System.Drawing.Size(240, 21);
      this.PARENT.TabIndex = 2;
      this.PARENT.SelectedIndexChanged += new System.EventHandler(this.PARENT_SelectedIndexChanged);
      // 
      // label5
      // 
      this.label5.AutoSize = true;
      this.label5.Location = new System.Drawing.Point(9, 46);
      this.label5.Margin = new System.Windows.Forms.Padding(2);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(38, 13);
      this.label5.TabIndex = 27;
      this.label5.Text = "Parent";
      // 
      // SET_DISTRIBUTION_LOCATION
      // 
      this.SET_DISTRIBUTION_LOCATION.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.SET_DISTRIBUTION_LOCATION.Location = new System.Drawing.Point(6, 243);
      this.SET_DISTRIBUTION_LOCATION.Name = "SET_DISTRIBUTION_LOCATION";
      this.SET_DISTRIBUTION_LOCATION.Size = new System.Drawing.Size(240, 23);
      this.SET_DISTRIBUTION_LOCATION.TabIndex = 7;
      this.SET_DISTRIBUTION_LOCATION.Text = "Set Distribution Location";
      this.SET_DISTRIBUTION_LOCATION.UseVisualStyleBackColor = true;
      this.SET_DISTRIBUTION_LOCATION.Click += new System.EventHandler(this.SET_DISTRIBUTION_LOCATION_Click);
      // 
      // BREAKER_EQUIPMENT
      // 
      this.BREAKER_EQUIPMENT.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.BREAKER_EQUIPMENT.Location = new System.Drawing.Point(6, 670);
      this.BREAKER_EQUIPMENT.Name = "BREAKER_EQUIPMENT";
      this.BREAKER_EQUIPMENT.Size = new System.Drawing.Size(240, 23);
      this.BREAKER_EQUIPMENT.TabIndex = 13;
      this.BREAKER_EQUIPMENT.Text = "Breaker Equipment";
      this.BREAKER_EQUIPMENT.UseVisualStyleBackColor = true;
      // 
      // DISTANCE_FROM_PARENT_LABEL
      // 
      this.DISTANCE_FROM_PARENT_LABEL.AutoSize = true;
      this.DISTANCE_FROM_PARENT_LABEL.Location = new System.Drawing.Point(8, 206);
      this.DISTANCE_FROM_PARENT_LABEL.Margin = new System.Windows.Forms.Padding(2);
      this.DISTANCE_FROM_PARENT_LABEL.Name = "DISTANCE_FROM_PARENT_LABEL";
      this.DISTANCE_FROM_PARENT_LABEL.Size = new System.Drawing.Size(106, 13);
      this.DISTANCE_FROM_PARENT_LABEL.TabIndex = 31;
      this.DISTANCE_FROM_PARENT_LABEL.Text = "Distance from Parent";
      // 
      // DISTANCE_FROM_PARENT_PANEL
      // 
      this.DISTANCE_FROM_PARENT_PANEL.BackColor = System.Drawing.SystemColors.Control;
      this.DISTANCE_FROM_PARENT_PANEL.Controls.Add(this.DISTANCE_FROM_PARENT);
      this.DISTANCE_FROM_PARENT_PANEL.Location = new System.Drawing.Point(6, 220);
      this.DISTANCE_FROM_PARENT_PANEL.Name = "DISTANCE_FROM_PARENT_PANEL";
      this.DISTANCE_FROM_PARENT_PANEL.Padding = new System.Windows.Forms.Padding(3);
      this.DISTANCE_FROM_PARENT_PANEL.Size = new System.Drawing.Size(240, 21);
      this.DISTANCE_FROM_PARENT_PANEL.TabIndex = 6;
      // 
      // DISTANCE_FROM_PARENT
      // 
      this.DISTANCE_FROM_PARENT.BorderStyle = System.Windows.Forms.BorderStyle.None;
      this.DISTANCE_FROM_PARENT.Enabled = false;
      this.DISTANCE_FROM_PARENT.Location = new System.Drawing.Point(6, 4);
      this.DISTANCE_FROM_PARENT.Name = "DISTANCE_FROM_PARENT";
      this.DISTANCE_FROM_PARENT.Size = new System.Drawing.Size(228, 13);
      this.DISTANCE_FROM_PARENT.TabIndex = 0;
      // 
      // DistributionForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackColor = System.Drawing.SystemColors.Control;
      this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.Controls.Add(this.DISTANCE_FROM_PARENT_PANEL);
      this.Controls.Add(this.DISTANCE_FROM_PARENT_LABEL);
      this.Controls.Add(this.BREAKER_EQUIPMENT);
      this.Controls.Add(this.SET_DISTRIBUTION_LOCATION);
      this.Controls.Add(this.PARENT);
      this.Controls.Add(this.label5);
      this.Controls.Add(this.DELETE_BREAKER);
      this.Controls.Add(this.ADD_BREAKER);
      this.Controls.Add(this.BREAKER_LOAD_CALCULATIONS);
      this.Controls.Add(this.BREAKER_PANELS);
      this.Controls.Add(this.panel1);
      this.Controls.Add(this.STATUS);
      this.Controls.Add(this.SIZE);
      this.Controls.Add(this.TABS);
      this.Controls.Add(this.label4);
      this.Controls.Add(this.KAIC);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.CONFIGURATION);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.label1);
      this.Name = "DistributionForm";
      this.Padding = new System.Windows.Forms.Padding(6);
      this.Size = new System.Drawing.Size(252, 702);
      this.Tag = "UPWARDS_ARROW";
      this.STATUS.ResumeLayout(false);
      this.STATUS.PerformLayout();
      this.panel1.ResumeLayout(false);
      this.panel1.PerformLayout();
      this.DISTANCE_FROM_PARENT_PANEL.ResumeLayout(false);
      this.DISTANCE_FROM_PARENT_PANEL.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.ComboBox CONFIGURATION;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.ComboBox KAIC;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.TabControl TABS;
    private System.Windows.Forms.ComboBox SIZE;
    private System.Windows.Forms.RadioButton NEW;
    private System.Windows.Forms.RadioButton EXISTING;
    private System.Windows.Forms.GroupBox STATUS;
    private System.Windows.Forms.Panel panel1;
    private System.Windows.Forms.TextBox DISTRIBUTION_NAME;
    private System.Windows.Forms.Button BREAKER_PANELS;
    private System.Windows.Forms.Button BREAKER_LOAD_CALCULATIONS;
    private System.Windows.Forms.Button DELETE_BREAKER;
    private System.Windows.Forms.Button ADD_BREAKER;
    private System.Windows.Forms.ComboBox PARENT;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.Button SET_DISTRIBUTION_LOCATION;
    private System.Windows.Forms.Button BREAKER_EQUIPMENT;
    private System.Windows.Forms.Label DISTANCE_FROM_PARENT_LABEL;
    private System.Windows.Forms.Panel DISTANCE_FROM_PARENT_PANEL;
    private System.Windows.Forms.TextBox DISTANCE_FROM_PARENT;
  }
}
