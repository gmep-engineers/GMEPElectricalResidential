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
      this.DISTRIBUTION_NAME = new System.Windows.Forms.TextBox();
      this.TABS = new System.Windows.Forms.TabControl();
      this.SIZE = new System.Windows.Forms.ComboBox();
      this.STATUS = new System.Windows.Forms.GroupBox();
      this.EXISTING = new System.Windows.Forms.RadioButton();
      this.NEW = new System.Windows.Forms.RadioButton();
      this.STATUS.SuspendLayout();
      this.SuspendLayout();
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(9, 89);
      this.label1.Margin = new System.Windows.Forms.Padding(0);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(43, 13);
      this.label1.TabIndex = 1;
      this.label1.Text = "Size (A)";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(9, 129);
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
      this.CONFIGURATION.Location = new System.Drawing.Point(8, 146);
      this.CONFIGURATION.Margin = new System.Windows.Forms.Padding(2);
      this.CONFIGURATION.Name = "CONFIGURATION";
      this.CONFIGURATION.Size = new System.Drawing.Size(236, 21);
      this.CONFIGURATION.TabIndex = 3;
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(9, 46);
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
            "10",
            "22",
            "42",
            "65"});
      this.KAIC.Location = new System.Drawing.Point(9, 61);
      this.KAIC.Margin = new System.Windows.Forms.Padding(2);
      this.KAIC.Name = "KAIC";
      this.KAIC.Size = new System.Drawing.Size(235, 21);
      this.KAIC.TabIndex = 7;
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.BackColor = System.Drawing.Color.Transparent;
      this.label4.Location = new System.Drawing.Point(9, 7);
      this.label4.Margin = new System.Windows.Forms.Padding(0);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(35, 13);
      this.label4.TabIndex = 9;
      this.label4.Text = "Name";
      // 
      // DISTRIBUTION_NAME
      // 
      this.DISTRIBUTION_NAME.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.DISTRIBUTION_NAME.Location = new System.Drawing.Point(8, 22);
      this.DISTRIBUTION_NAME.Margin = new System.Windows.Forms.Padding(2);
      this.DISTRIBUTION_NAME.Name = "DISTRIBUTION_NAME";
      this.DISTRIBUTION_NAME.Size = new System.Drawing.Size(97, 20);
      this.DISTRIBUTION_NAME.TabIndex = 8;
      // 
      // TABS
      // 
      this.TABS.Location = new System.Drawing.Point(9, 174);
      this.TABS.Name = "TABS";
      this.TABS.SelectedIndex = 0;
      this.TABS.Size = new System.Drawing.Size(233, 235);
      this.TABS.TabIndex = 10;
      // 
      // SIZE
      // 
      this.SIZE.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.SIZE.FormattingEnabled = true;
      this.SIZE.Items.AddRange(new object[] {
            "100",
            "200",
            "400",
            "600",
            "800",
            "1000",
            "1200",
            "1600",
            "2000"});
      this.SIZE.Location = new System.Drawing.Point(8, 104);
      this.SIZE.Margin = new System.Windows.Forms.Padding(2);
      this.SIZE.Name = "SIZE";
      this.SIZE.Size = new System.Drawing.Size(236, 21);
      this.SIZE.TabIndex = 11;
      // 
      // STATUS
      // 
      this.STATUS.Controls.Add(this.NEW);
      this.STATUS.Controls.Add(this.EXISTING);
      this.STATUS.Location = new System.Drawing.Point(121, 7);
      this.STATUS.Name = "STATUS";
      this.STATUS.Size = new System.Drawing.Size(122, 42);
      this.STATUS.TabIndex = 12;
      this.STATUS.TabStop = false;
      this.STATUS.Text = "Status";
      // 
      // EXISTING
      // 
      this.EXISTING.AutoSize = true;
      this.EXISTING.Location = new System.Drawing.Point(59, 18);
      this.EXISTING.Name = "EXISTING";
      this.EXISTING.Size = new System.Drawing.Size(61, 17);
      this.EXISTING.TabIndex = 0;
      this.EXISTING.TabStop = true;
      this.EXISTING.Text = "Existing";
      this.EXISTING.UseVisualStyleBackColor = true;
      // 
      // NEW
      // 
      this.NEW.AutoSize = true;
      this.NEW.Checked = true;
      this.NEW.Location = new System.Drawing.Point(6, 18);
      this.NEW.Name = "NEW";
      this.NEW.Size = new System.Drawing.Size(47, 17);
      this.NEW.TabIndex = 1;
      this.NEW.TabStop = true;
      this.NEW.Text = "New";
      this.NEW.UseVisualStyleBackColor = true;
      // 
      // DistributionForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackColor = System.Drawing.SystemColors.Control;
      this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.Controls.Add(this.STATUS);
      this.Controls.Add(this.SIZE);
      this.Controls.Add(this.TABS);
      this.Controls.Add(this.label4);
      this.Controls.Add(this.DISTRIBUTION_NAME);
      this.Controls.Add(this.KAIC);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.CONFIGURATION);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.label1);
      this.Name = "DistributionForm";
      this.Padding = new System.Windows.Forms.Padding(6);
      this.Size = new System.Drawing.Size(252, 419);
      this.Tag = "UPWARDS_ARROW";
      this.STATUS.ResumeLayout(false);
      this.STATUS.PerformLayout();
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
    private System.Windows.Forms.TextBox DISTRIBUTION_NAME;
    private System.Windows.Forms.TabControl TABS;
    private System.Windows.Forms.ComboBox SIZE;
    private System.Windows.Forms.GroupBox STATUS;
    private System.Windows.Forms.RadioButton EXISTING;
    private System.Windows.Forms.RadioButton NEW;
  }
}
