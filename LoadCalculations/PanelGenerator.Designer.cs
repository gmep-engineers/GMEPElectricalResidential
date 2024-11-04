namespace GMEPElectricalResidential.LoadCalculations
{
  partial class PanelGenerator
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
    #region Windows Form Designer generated code
    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.EXTERIOR_LIGHTING = new System.Windows.Forms.CheckBox();
      this.EXTERIOR_RECEPTACLE = new System.Windows.Forms.CheckBox();
      this.GAS_RECEPTACLE = new System.Windows.Forms.CheckBox();
      this.PANEL_CUSTOM_LOADS = new System.Windows.Forms.Panel();
      this.label1 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.label8 = new System.Windows.Forms.Label();
      this.SUB_BREAKERS = new System.Windows.Forms.ComboBox();
      this.ADD_BUTTON = new System.Windows.Forms.Button();
      this.REMOVE_BUTTON = new System.Windows.Forms.Button();
      this.SUBPANELS = new System.Windows.Forms.ListBox();
      this.GENERATE_PANEL = new System.Windows.Forms.Button();
      this.MP_BREAKERS = new System.Windows.Forms.ComboBox();
      this.label3 = new System.Windows.Forms.Label();
      this.label4 = new System.Windows.Forms.Label();
      this.PANEL_BREAKERS = new System.Windows.Forms.Panel();
      this.SuspendLayout();
      // 
      // EXTERIOR_LIGHTING
      // 
      this.EXTERIOR_LIGHTING.AutoSize = true;
      this.EXTERIOR_LIGHTING.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
      this.EXTERIOR_LIGHTING.Location = new System.Drawing.Point(30, 73);
      this.EXTERIOR_LIGHTING.Name = "EXTERIOR_LIGHTING";
      this.EXTERIOR_LIGHTING.Size = new System.Drawing.Size(129, 21);
      this.EXTERIOR_LIGHTING.TabIndex = 0;
      this.EXTERIOR_LIGHTING.Text = "Exterior Lighting";
      this.EXTERIOR_LIGHTING.UseVisualStyleBackColor = true;
      // 
      // EXTERIOR_RECEPTACLE
      // 
      this.EXTERIOR_RECEPTACLE.AutoSize = true;
      this.EXTERIOR_RECEPTACLE.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
      this.EXTERIOR_RECEPTACLE.Location = new System.Drawing.Point(30, 100);
      this.EXTERIOR_RECEPTACLE.Name = "EXTERIOR_RECEPTACLE";
      this.EXTERIOR_RECEPTACLE.Size = new System.Drawing.Size(150, 21);
      this.EXTERIOR_RECEPTACLE.TabIndex = 1;
      this.EXTERIOR_RECEPTACLE.Text = "Exterior Receptacle";
      this.EXTERIOR_RECEPTACLE.UseVisualStyleBackColor = true;
      // 
      // GAS_RECEPTACLE
      // 
      this.GAS_RECEPTACLE.AutoSize = true;
      this.GAS_RECEPTACLE.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
      this.GAS_RECEPTACLE.Location = new System.Drawing.Point(30, 127);
      this.GAS_RECEPTACLE.Name = "GAS_RECEPTACLE";
      this.GAS_RECEPTACLE.Size = new System.Drawing.Size(128, 21);
      this.GAS_RECEPTACLE.TabIndex = 2;
      this.GAS_RECEPTACLE.Text = "Gas Receptacle";
      this.GAS_RECEPTACLE.UseVisualStyleBackColor = true;
      // 
      // PANEL_CUSTOM_LOADS
      // 
      this.PANEL_CUSTOM_LOADS.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.PANEL_CUSTOM_LOADS.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
      this.PANEL_CUSTOM_LOADS.Location = new System.Drawing.Point(30, 201);
      this.PANEL_CUSTOM_LOADS.Name = "PANEL_CUSTOM_LOADS";
      this.PANEL_CUSTOM_LOADS.Size = new System.Drawing.Size(291, 123);
      this.PANEL_CUSTOM_LOADS.TabIndex = 3;
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
      this.label1.Location = new System.Drawing.Point(27, 24);
      this.label1.Margin = new System.Windows.Forms.Padding(12);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(278, 34);
      this.label1.TabIndex = 4;
      this.label1.Text = "Do the selected unit types have any of the \r\nfollowing?";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
      this.label2.Location = new System.Drawing.Point(36, 169);
      this.label2.Margin = new System.Windows.Forms.Padding(12, 18, 12, 12);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(211, 17);
      this.label2.TabIndex = 5;
      this.label2.Text = "Unit Type - Custom Load | Poles";
      // 
      // label8
      // 
      this.label8.AutoSize = true;
      this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
      this.label8.Location = new System.Drawing.Point(36, 570);
      this.label8.Margin = new System.Windows.Forms.Padding(12, 18, 12, 6);
      this.label8.Name = "label8";
      this.label8.Size = new System.Drawing.Size(129, 17);
      this.label8.TabIndex = 15;
      this.label8.Text = "Subpanel Breakers";
      // 
      // SUB_BREAKERS
      // 
      this.SUB_BREAKERS.AutoCompleteCustomSource.AddRange(new string[] {
            "6",
            "12",
            "18",
            "24",
            "32",
            "42",
            "84"});
      this.SUB_BREAKERS.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
      this.SUB_BREAKERS.FormattingEnabled = true;
      this.SUB_BREAKERS.Items.AddRange(new object[] {
            "6",
            "12",
            "20",
            "24",
            "30",
            "32",
            "42",
            "84"});
      this.SUB_BREAKERS.Location = new System.Drawing.Point(33, 593);
      this.SUB_BREAKERS.Name = "SUB_BREAKERS";
      this.SUB_BREAKERS.Size = new System.Drawing.Size(288, 24);
      this.SUB_BREAKERS.TabIndex = 16;
      // 
      // ADD_BUTTON
      // 
      this.ADD_BUTTON.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
      this.ADD_BUTTON.Location = new System.Drawing.Point(33, 623);
      this.ADD_BUTTON.Name = "ADD_BUTTON";
      this.ADD_BUTTON.Size = new System.Drawing.Size(125, 33);
      this.ADD_BUTTON.TabIndex = 20;
      this.ADD_BUTTON.Text = "Add";
      this.ADD_BUTTON.UseVisualStyleBackColor = true;
      this.ADD_BUTTON.Click += new System.EventHandler(this.ADD_BUTTON_Click);
      // 
      // REMOVE_BUTTON
      // 
      this.REMOVE_BUTTON.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
      this.REMOVE_BUTTON.Location = new System.Drawing.Point(164, 623);
      this.REMOVE_BUTTON.Name = "REMOVE_BUTTON";
      this.REMOVE_BUTTON.Size = new System.Drawing.Size(157, 33);
      this.REMOVE_BUTTON.TabIndex = 21;
      this.REMOVE_BUTTON.Text = "Remove";
      this.REMOVE_BUTTON.UseVisualStyleBackColor = true;
      this.REMOVE_BUTTON.Click += new System.EventHandler(this.REMOVE_BUTTON_Click);
      // 
      // SUBPANELS
      // 
      this.SUBPANELS.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
      this.SUBPANELS.FormattingEnabled = true;
      this.SUBPANELS.ItemHeight = 16;
      this.SUBPANELS.Location = new System.Drawing.Point(33, 662);
      this.SUBPANELS.Name = "SUBPANELS";
      this.SUBPANELS.Size = new System.Drawing.Size(288, 68);
      this.SUBPANELS.TabIndex = 22;
      // 
      // GENERATE_PANEL
      // 
      this.GENERATE_PANEL.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
      this.GENERATE_PANEL.Location = new System.Drawing.Point(33, 736);
      this.GENERATE_PANEL.Name = "GENERATE_PANEL";
      this.GENERATE_PANEL.Size = new System.Drawing.Size(288, 33);
      this.GENERATE_PANEL.TabIndex = 23;
      this.GENERATE_PANEL.Text = "Generate Panels";
      this.GENERATE_PANEL.UseVisualStyleBackColor = true;
      this.GENERATE_PANEL.Click += new System.EventHandler(this.GENERATE_PANEL_Click);
      // 
      // MP_BREAKERS
      // 
      this.MP_BREAKERS.AutoCompleteCustomSource.AddRange(new string[] {
            "6",
            "12",
            "18",
            "24",
            "32",
            "42",
            "84"});
      this.MP_BREAKERS.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
      this.MP_BREAKERS.FormattingEnabled = true;
      this.MP_BREAKERS.Items.AddRange(new object[] {
            "6",
            "12",
            "20",
            "24",
            "30",
            "32",
            "42",
            "84"});
      this.MP_BREAKERS.Location = new System.Drawing.Point(33, 525);
      this.MP_BREAKERS.Name = "MP_BREAKERS";
      this.MP_BREAKERS.Size = new System.Drawing.Size(288, 24);
      this.MP_BREAKERS.TabIndex = 25;
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
      this.label3.Location = new System.Drawing.Point(36, 502);
      this.label3.Margin = new System.Windows.Forms.Padding(12, 18, 12, 6);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(139, 17);
      this.label3.TabIndex = 24;
      this.label3.Text = "Main Panel Breakers";
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
      this.label4.Location = new System.Drawing.Point(36, 345);
      this.label4.Margin = new System.Windows.Forms.Padding(12, 18, 12, 12);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(262, 17);
      this.label4.TabIndex = 27;
      this.label4.Text = "Unit Type - Breakers Required/Available";
      // 
      // PANEL_BREAKERS
      // 
      this.PANEL_BREAKERS.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.PANEL_BREAKERS.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
      this.PANEL_BREAKERS.Location = new System.Drawing.Point(30, 377);
      this.PANEL_BREAKERS.Name = "PANEL_BREAKERS";
      this.PANEL_BREAKERS.Size = new System.Drawing.Size(291, 104);
      this.PANEL_BREAKERS.TabIndex = 26;
      // 
      // PanelGenerator
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(353, 793);
      this.Controls.Add(this.label4);
      this.Controls.Add(this.PANEL_BREAKERS);
      this.Controls.Add(this.MP_BREAKERS);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.GENERATE_PANEL);
      this.Controls.Add(this.SUBPANELS);
      this.Controls.Add(this.REMOVE_BUTTON);
      this.Controls.Add(this.ADD_BUTTON);
      this.Controls.Add(this.SUB_BREAKERS);
      this.Controls.Add(this.label8);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.PANEL_CUSTOM_LOADS);
      this.Controls.Add(this.GAS_RECEPTACLE);
      this.Controls.Add(this.EXTERIOR_RECEPTACLE);
      this.Controls.Add(this.EXTERIOR_LIGHTING);
      this.Name = "PanelGenerator";
      this.Padding = new System.Windows.Forms.Padding(24);
      this.Text = "PanelGenerator";
      this.ResumeLayout(false);
      this.PerformLayout();

    }
    #endregion
    private System.Windows.Forms.CheckBox EXTERIOR_LIGHTING;
    private System.Windows.Forms.CheckBox EXTERIOR_RECEPTACLE;
    private System.Windows.Forms.CheckBox GAS_RECEPTACLE;
    private System.Windows.Forms.Panel PANEL_CUSTOM_LOADS;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label8;
    private System.Windows.Forms.ComboBox SUB_BREAKERS;
    private System.Windows.Forms.Button ADD_BUTTON;
    private System.Windows.Forms.Button REMOVE_BUTTON;
    private System.Windows.Forms.ListBox SUBPANELS;
    private System.Windows.Forms.Button GENERATE_PANEL;
    private System.Windows.Forms.ComboBox MP_BREAKERS;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.Panel PANEL_BREAKERS;
  }
}