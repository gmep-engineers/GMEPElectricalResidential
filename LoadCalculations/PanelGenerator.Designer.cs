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
      this.label3 = new System.Windows.Forms.Label();
      this.label4 = new System.Windows.Forms.Label();
      this.BREAKERS_REQUIRED = new System.Windows.Forms.Label();
      this.label6 = new System.Windows.Forms.Label();
      this.BREAKERS = new System.Windows.Forms.ComboBox();
      this.label5 = new System.Windows.Forms.Label();
      this.label7 = new System.Windows.Forms.Label();
      this.NAME = new System.Windows.Forms.TextBox();
      this.label8 = new System.Windows.Forms.Label();
      this.textBox1 = new System.Windows.Forms.TextBox();
      this.label9 = new System.Windows.Forms.Label();
      this.label10 = new System.Windows.Forms.Label();
      this.comboBox1 = new System.Windows.Forms.ComboBox();
      this.button1 = new System.Windows.Forms.Button();
      this.button2 = new System.Windows.Forms.Button();
      this.SUBPANELS = new System.Windows.Forms.ListBox();
      this.GENERATE_PANEL = new System.Windows.Forms.Button();
      this.label11 = new System.Windows.Forms.Label();
      this.BREAKERS_AVAILABLE = new System.Windows.Forms.Label();
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
      this.PANEL_CUSTOM_LOADS.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
      this.PANEL_CUSTOM_LOADS.Location = new System.Drawing.Point(30, 201);
      this.PANEL_CUSTOM_LOADS.Name = "PANEL_CUSTOM_LOADS";
      this.PANEL_CUSTOM_LOADS.Size = new System.Drawing.Size(291, 137);
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
      this.label2.Size = new System.Drawing.Size(98, 17);
      this.label2.TabIndex = 5;
      this.label2.Text = "Custom Loads";
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
      this.label3.Location = new System.Drawing.Point(174, 169);
      this.label3.Margin = new System.Windows.Forms.Padding(12);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(113, 17);
      this.label3.TabIndex = 6;
      this.label3.Text = "Number of Poles";
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
      this.label4.Location = new System.Drawing.Point(36, 359);
      this.label4.Margin = new System.Windows.Forms.Padding(12, 18, 12, 12);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(201, 17);
      this.label4.TabIndex = 7;
      this.label4.Text = "Number of Breakers Required:";
      // 
      // BREAKERS_REQUIRED
      // 
      this.BREAKERS_REQUIRED.AutoSize = true;
      this.BREAKERS_REQUIRED.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
      this.BREAKERS_REQUIRED.Location = new System.Drawing.Point(239, 359);
      this.BREAKERS_REQUIRED.Margin = new System.Windows.Forms.Padding(12, 18, 12, 12);
      this.BREAKERS_REQUIRED.Name = "BREAKERS_REQUIRED";
      this.BREAKERS_REQUIRED.Size = new System.Drawing.Size(16, 17);
      this.BREAKERS_REQUIRED.TabIndex = 8;
      this.BREAKERS_REQUIRED.Text = "0";
      // 
      // label6
      // 
      this.label6.AutoSize = true;
      this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
      this.label6.Location = new System.Drawing.Point(36, 430);
      this.label6.Margin = new System.Windows.Forms.Padding(12, 18, 12, 6);
      this.label6.Name = "label6";
      this.label6.Size = new System.Drawing.Size(78, 17);
      this.label6.TabIndex = 10;
      this.label6.Text = "Main Panel";
      // 
      // BREAKERS
      // 
      this.BREAKERS.AutoCompleteCustomSource.AddRange(new string[] {
            "6",
            "12",
            "18",
            "24",
            "32",
            "42",
            "84"});
      this.BREAKERS.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
      this.BREAKERS.FormattingEnabled = true;
      this.BREAKERS.Items.AddRange(new object[] {
            "6",
            "12",
            "20",
            "24",
            "30",
            "32",
            "42",
            "84"});
      this.BREAKERS.Location = new System.Drawing.Point(164, 488);
      this.BREAKERS.Name = "BREAKERS";
      this.BREAKERS.Size = new System.Drawing.Size(157, 24);
      this.BREAKERS.TabIndex = 11;
      // 
      // label5
      // 
      this.label5.AutoSize = true;
      this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
      this.label5.Location = new System.Drawing.Point(172, 465);
      this.label5.Margin = new System.Windows.Forms.Padding(12, 18, 12, 3);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(65, 17);
      this.label5.TabIndex = 12;
      this.label5.Text = "Breakers";
      // 
      // label7
      // 
      this.label7.AutoSize = true;
      this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
      this.label7.Location = new System.Drawing.Point(36, 465);
      this.label7.Margin = new System.Windows.Forms.Padding(12, 12, 12, 3);
      this.label7.Name = "label7";
      this.label7.Size = new System.Drawing.Size(45, 17);
      this.label7.TabIndex = 13;
      this.label7.Text = "Name";
      // 
      // NAME
      // 
      this.NAME.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
      this.NAME.Location = new System.Drawing.Point(30, 488);
      this.NAME.Name = "NAME";
      this.NAME.Size = new System.Drawing.Size(128, 23);
      this.NAME.TabIndex = 14;
      // 
      // label8
      // 
      this.label8.AutoSize = true;
      this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
      this.label8.Location = new System.Drawing.Point(36, 529);
      this.label8.Margin = new System.Windows.Forms.Padding(12, 18, 12, 6);
      this.label8.Name = "label8";
      this.label8.Size = new System.Drawing.Size(75, 17);
      this.label8.TabIndex = 15;
      this.label8.Text = "Subpanels";
      // 
      // textBox1
      // 
      this.textBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
      this.textBox1.Location = new System.Drawing.Point(33, 587);
      this.textBox1.Name = "textBox1";
      this.textBox1.Size = new System.Drawing.Size(125, 23);
      this.textBox1.TabIndex = 19;
      // 
      // label9
      // 
      this.label9.AutoSize = true;
      this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
      this.label9.Location = new System.Drawing.Point(39, 564);
      this.label9.Margin = new System.Windows.Forms.Padding(12, 12, 12, 3);
      this.label9.Name = "label9";
      this.label9.Size = new System.Drawing.Size(45, 17);
      this.label9.TabIndex = 18;
      this.label9.Text = "Name";
      // 
      // label10
      // 
      this.label10.AutoSize = true;
      this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
      this.label10.Location = new System.Drawing.Point(175, 564);
      this.label10.Margin = new System.Windows.Forms.Padding(12, 18, 12, 3);
      this.label10.Name = "label10";
      this.label10.Size = new System.Drawing.Size(65, 17);
      this.label10.TabIndex = 17;
      this.label10.Text = "Breakers";
      // 
      // comboBox1
      // 
      this.comboBox1.AutoCompleteCustomSource.AddRange(new string[] {
            "6",
            "12",
            "18",
            "24",
            "32",
            "42",
            "84"});
      this.comboBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
      this.comboBox1.FormattingEnabled = true;
      this.comboBox1.Items.AddRange(new object[] {
            "6",
            "12",
            "20",
            "24",
            "30",
            "32",
            "42",
            "84"});
      this.comboBox1.Location = new System.Drawing.Point(164, 587);
      this.comboBox1.Name = "comboBox1";
      this.comboBox1.Size = new System.Drawing.Size(157, 24);
      this.comboBox1.TabIndex = 16;
      // 
      // button1
      // 
      this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
      this.button1.Location = new System.Drawing.Point(33, 617);
      this.button1.Name = "button1";
      this.button1.Size = new System.Drawing.Size(125, 33);
      this.button1.TabIndex = 20;
      this.button1.Text = "Add";
      this.button1.UseVisualStyleBackColor = true;
      // 
      // button2
      // 
      this.button2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
      this.button2.Location = new System.Drawing.Point(164, 617);
      this.button2.Name = "button2";
      this.button2.Size = new System.Drawing.Size(157, 33);
      this.button2.TabIndex = 21;
      this.button2.Text = "Remove";
      this.button2.UseVisualStyleBackColor = true;
      // 
      // SUBPANELS
      // 
      this.SUBPANELS.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
      this.SUBPANELS.FormattingEnabled = true;
      this.SUBPANELS.ItemHeight = 16;
      this.SUBPANELS.Location = new System.Drawing.Point(33, 656);
      this.SUBPANELS.Name = "SUBPANELS";
      this.SUBPANELS.Size = new System.Drawing.Size(288, 68);
      this.SUBPANELS.TabIndex = 22;
      // 
      // GENERATE_PANEL
      // 
      this.GENERATE_PANEL.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
      this.GENERATE_PANEL.Location = new System.Drawing.Point(33, 739);
      this.GENERATE_PANEL.Name = "GENERATE_PANEL";
      this.GENERATE_PANEL.Size = new System.Drawing.Size(288, 33);
      this.GENERATE_PANEL.TabIndex = 23;
      this.GENERATE_PANEL.Text = "Generate Panels";
      this.GENERATE_PANEL.UseVisualStyleBackColor = true;
      // 
      // label11
      // 
      this.label11.AutoSize = true;
      this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
      this.label11.Location = new System.Drawing.Point(36, 383);
      this.label11.Margin = new System.Windows.Forms.Padding(12, 18, 12, 12);
      this.label11.Name = "label11";
      this.label11.Size = new System.Drawing.Size(200, 17);
      this.label11.TabIndex = 24;
      this.label11.Text = "Number of Breakers Available:";
      // 
      // BREAKERS_AVAILABLE
      // 
      this.BREAKERS_AVAILABLE.AutoSize = true;
      this.BREAKERS_AVAILABLE.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
      this.BREAKERS_AVAILABLE.Location = new System.Drawing.Point(239, 383);
      this.BREAKERS_AVAILABLE.Margin = new System.Windows.Forms.Padding(12, 18, 12, 12);
      this.BREAKERS_AVAILABLE.Name = "BREAKERS_AVAILABLE";
      this.BREAKERS_AVAILABLE.Size = new System.Drawing.Size(16, 17);
      this.BREAKERS_AVAILABLE.TabIndex = 25;
      this.BREAKERS_AVAILABLE.Text = "0";
      // 
      // PanelGenerator
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(353, 793);
      this.Controls.Add(this.BREAKERS_AVAILABLE);
      this.Controls.Add(this.label11);
      this.Controls.Add(this.GENERATE_PANEL);
      this.Controls.Add(this.SUBPANELS);
      this.Controls.Add(this.button2);
      this.Controls.Add(this.button1);
      this.Controls.Add(this.textBox1);
      this.Controls.Add(this.label9);
      this.Controls.Add(this.label10);
      this.Controls.Add(this.comboBox1);
      this.Controls.Add(this.label8);
      this.Controls.Add(this.NAME);
      this.Controls.Add(this.label7);
      this.Controls.Add(this.label5);
      this.Controls.Add(this.BREAKERS);
      this.Controls.Add(this.label6);
      this.Controls.Add(this.BREAKERS_REQUIRED);
      this.Controls.Add(this.label4);
      this.Controls.Add(this.label3);
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
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.Label BREAKERS_REQUIRED;
    private System.Windows.Forms.Label label6;
    private System.Windows.Forms.ComboBox BREAKERS;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.Label label7;
    private System.Windows.Forms.TextBox NAME;
    private System.Windows.Forms.Label label8;
    private System.Windows.Forms.TextBox textBox1;
    private System.Windows.Forms.Label label9;
    private System.Windows.Forms.Label label10;
    private System.Windows.Forms.ComboBox comboBox1;
    private System.Windows.Forms.Button button1;
    private System.Windows.Forms.Button button2;
    private System.Windows.Forms.ListBox SUBPANELS;
    private System.Windows.Forms.Button GENERATE_PANEL;
    private System.Windows.Forms.Label label11;
    private System.Windows.Forms.Label BREAKERS_AVAILABLE;
  }
}