namespace GMEPElectricalResidential
{
  partial class MainForm
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
      // MainForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.Controls.Add(this.KAIC);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.CONFIGURATION);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.AMPERAGE);
      this.Name = "MainForm";
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
  }
}
