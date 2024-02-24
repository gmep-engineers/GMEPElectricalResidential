namespace GMEPElectricalResidential
{
  partial class MeterComboForm
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
      this.AMPERAGE = new System.Windows.Forms.TextBox();
      this.label1 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.CONFIGURATION = new System.Windows.Forms.ComboBox();
      this.label3 = new System.Windows.Forms.Label();
      this.KAIC = new System.Windows.Forms.ComboBox();
      this.label4 = new System.Windows.Forms.Label();
      this.PANEL_NAME = new System.Windows.Forms.TextBox();
      this.label5 = new System.Windows.Forms.Label();
      this.PANEL_SIZE = new System.Windows.Forms.TextBox();
      this.SuspendLayout();
      // 
      // AMPERAGE
      // 
      this.AMPERAGE.Location = new System.Drawing.Point(9, 22);
      this.AMPERAGE.Margin = new System.Windows.Forms.Padding(6, 3, 6, 6);
      this.AMPERAGE.Name = "AMPERAGE";
      this.AMPERAGE.Size = new System.Drawing.Size(100, 20);
      this.AMPERAGE.TabIndex = 0;
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(9, 6);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(55, 13);
      this.label1.TabIndex = 1;
      this.label1.Text = "Amperage";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(9, 48);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(69, 13);
      this.label2.TabIndex = 2;
      this.label2.Text = "Configuration";
      // 
      // CONFIGURATION
      // 
      this.CONFIGURATION.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.CONFIGURATION.FormattingEnabled = true;
      this.CONFIGURATION.Items.AddRange(new object[] {
            "120/208V 1PH 3W",
            "120/240V 1PH 3W",
            "120/208V 3PH 4W",
            "120/240V 3PH 4W",
            "277/480V 3PH 4W"});
      this.CONFIGURATION.Location = new System.Drawing.Point(9, 64);
      this.CONFIGURATION.Margin = new System.Windows.Forms.Padding(6, 3, 6, 6);
      this.CONFIGURATION.Name = "CONFIGURATION";
      this.CONFIGURATION.Size = new System.Drawing.Size(121, 21);
      this.CONFIGURATION.TabIndex = 3;
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(9, 91);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(31, 13);
      this.label3.TabIndex = 5;
      this.label3.Text = "KAIC";
      // 
      // KAIC
      // 
      this.KAIC.FormattingEnabled = true;
      this.KAIC.Items.AddRange(new object[] {
            "10",
            "22",
            "42",
            "65"});
      this.KAIC.Location = new System.Drawing.Point(9, 107);
      this.KAIC.Margin = new System.Windows.Forms.Padding(6, 3, 6, 6);
      this.KAIC.Name = "KAIC";
      this.KAIC.Size = new System.Drawing.Size(121, 21);
      this.KAIC.TabIndex = 7;
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(159, 6);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(65, 13);
      this.label4.TabIndex = 9;
      this.label4.Text = "Panel Name";
      // 
      // PANEL_NAME
      // 
      this.PANEL_NAME.Location = new System.Drawing.Point(159, 22);
      this.PANEL_NAME.Margin = new System.Windows.Forms.Padding(6, 3, 6, 6);
      this.PANEL_NAME.Name = "PANEL_NAME";
      this.PANEL_NAME.Size = new System.Drawing.Size(100, 20);
      this.PANEL_NAME.TabIndex = 8;
      // 
      // label5
      // 
      this.label5.AutoSize = true;
      this.label5.Location = new System.Drawing.Point(159, 49);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(57, 13);
      this.label5.TabIndex = 11;
      this.label5.Text = "Panel Size";
      // 
      // PANEL_SIZE
      // 
      this.PANEL_SIZE.Location = new System.Drawing.Point(159, 65);
      this.PANEL_SIZE.Margin = new System.Windows.Forms.Padding(6, 3, 6, 6);
      this.PANEL_SIZE.Name = "PANEL_SIZE";
      this.PANEL_SIZE.Size = new System.Drawing.Size(100, 20);
      this.PANEL_SIZE.TabIndex = 10;
      // 
      // MeterComboForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.Controls.Add(this.label5);
      this.Controls.Add(this.PANEL_SIZE);
      this.Controls.Add(this.label4);
      this.Controls.Add(this.PANEL_NAME);
      this.Controls.Add(this.KAIC);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.CONFIGURATION);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.AMPERAGE);
      this.Name = "MeterComboForm";
      this.Padding = new System.Windows.Forms.Padding(6);
      this.Size = new System.Drawing.Size(1040, 160);
      this.Tag = "UPWARDS_ARROW";
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.TextBox AMPERAGE;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.ComboBox CONFIGURATION;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.ComboBox KAIC;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.TextBox PANEL_NAME;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.TextBox PANEL_SIZE;
  }
}
