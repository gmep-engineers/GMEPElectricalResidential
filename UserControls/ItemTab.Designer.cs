namespace GMEPElectricalResidential
{
  partial class ItemTab
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
      this.TYPE = new System.Windows.Forms.ComboBox();
      this.label5 = new System.Windows.Forms.Label();
      this.BREAKER_SIZES = new System.Windows.Forms.ComboBox();
      this.label1 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.NAME = new System.Windows.Forms.TextBox();
      this.NEW = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // TYPE
      // 
      this.TYPE.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.TYPE.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.TYPE.FormattingEnabled = true;
      this.TYPE.Items.AddRange(new object[] {
            "No Work",
            "Panel",
            "Load Calculation",
            "Distribution",
            "Equipment"});
      this.TYPE.Location = new System.Drawing.Point(6, 25);
      this.TYPE.Name = "TYPE";
      this.TYPE.Size = new System.Drawing.Size(216, 21);
      this.TYPE.TabIndex = 13;
      this.TYPE.SelectedIndexChanged += new System.EventHandler(this.TYPE_SelectedIndexChanged);
      // 
      // label5
      // 
      this.label5.AutoSize = true;
      this.label5.Location = new System.Drawing.Point(3, 6);
      this.label5.Margin = new System.Windows.Forms.Padding(3);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(31, 13);
      this.label5.TabIndex = 12;
      this.label5.Text = "Type";
      // 
      // BREAKER_SIZES
      // 
      this.BREAKER_SIZES.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.BREAKER_SIZES.FormattingEnabled = true;
      this.BREAKER_SIZES.Items.AddRange(new object[] {
            "20",
            "30",
            "40",
            "50",
            "60",
            "70",
            "80",
            "90",
            "100",
            "125",
            "150",
            "200"});
      this.BREAKER_SIZES.Location = new System.Drawing.Point(6, 65);
      this.BREAKER_SIZES.Name = "BREAKER_SIZES";
      this.BREAKER_SIZES.Size = new System.Drawing.Size(216, 21);
      this.BREAKER_SIZES.TabIndex = 15;
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(3, 49);
      this.label1.Margin = new System.Windows.Forms.Padding(0);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(67, 13);
      this.label1.TabIndex = 14;
      this.label1.Text = "Breaker Size";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(3, 89);
      this.label2.Margin = new System.Windows.Forms.Padding(0);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(35, 13);
      this.label2.TabIndex = 16;
      this.label2.Text = "Name";
      // 
      // NAME
      // 
      this.NAME.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.NAME.Location = new System.Drawing.Point(6, 105);
      this.NAME.Name = "NAME";
      this.NAME.Size = new System.Drawing.Size(216, 20);
      this.NAME.TabIndex = 17;
      // 
      // NEW
      // 
      this.NEW.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.NEW.Location = new System.Drawing.Point(6, 182);
      this.NEW.Name = "NEW";
      this.NEW.Size = new System.Drawing.Size(216, 23);
      this.NEW.TabIndex = 18;
      this.NEW.Text = "New";
      this.NEW.UseVisualStyleBackColor = true;
      this.NEW.Click += new System.EventHandler(this.NEW_Click);
      // 
      // ItemTab
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackColor = System.Drawing.SystemColors.Control;
      this.Controls.Add(this.NEW);
      this.Controls.Add(this.NAME);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.BREAKER_SIZES);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.TYPE);
      this.Controls.Add(this.label5);
      this.Name = "ItemTab";
      this.Padding = new System.Windows.Forms.Padding(3);
      this.Size = new System.Drawing.Size(232, 211);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.ComboBox TYPE;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.ComboBox BREAKER_SIZES;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.TextBox NAME;
    private System.Windows.Forms.Button NEW;
  }
}
