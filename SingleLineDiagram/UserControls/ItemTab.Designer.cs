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
      this.NEW = new System.Windows.Forms.RadioButton();
      this.EXISTING = new System.Windows.Forms.RadioButton();
      this.STATUS = new System.Windows.Forms.GroupBox();
      this.XFMR = new System.Windows.Forms.ComboBox();
      this.label3 = new System.Windows.Forms.Label();
      this.panel1 = new System.Windows.Forms.Panel();
      this.NAME = new System.Windows.Forms.TextBox();
      this.SET_BREAKER_ITEM_LOCATION = new System.Windows.Forms.Button();
      this.SET_XFMR_LOCATION = new System.Windows.Forms.Button();
      this.POLE_GROUP = new System.Windows.Forms.GroupBox();
      this.POLE3 = new System.Windows.Forms.RadioButton();
      this.POLE2 = new System.Windows.Forms.RadioButton();
      this.POLE1 = new System.Windows.Forms.RadioButton();
      this.STATUS.SuspendLayout();
      this.panel1.SuspendLayout();
      this.POLE_GROUP.SuspendLayout();
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
      this.TYPE.Size = new System.Drawing.Size(220, 21);
      this.TYPE.TabIndex = 0;
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
            "20A",
            "30A",
            "40A",
            "50A",
            "60A",
            "70A",
            "80A",
            "90A",
            "100A",
            "125A",
            "150A",
            "200A",
            "400A",
            "600A",
            "800A",
            "1000A"});
      this.BREAKER_SIZES.Location = new System.Drawing.Point(6, 65);
      this.BREAKER_SIZES.Name = "BREAKER_SIZES";
      this.BREAKER_SIZES.Size = new System.Drawing.Size(220, 21);
      this.BREAKER_SIZES.TabIndex = 1;
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
      this.EXISTING.TabIndex = 0;
      this.EXISTING.TabStop = true;
      this.EXISTING.Text = "Existing";
      this.EXISTING.UseVisualStyleBackColor = true;
      // 
      // STATUS
      // 
      this.STATUS.Controls.Add(this.NEW);
      this.STATUS.Controls.Add(this.EXISTING);
      this.STATUS.Location = new System.Drawing.Point(101, 131);
      this.STATUS.Name = "STATUS";
      this.STATUS.Padding = new System.Windows.Forms.Padding(0);
      this.STATUS.Size = new System.Drawing.Size(125, 42);
      this.STATUS.TabIndex = 5;
      this.STATUS.TabStop = false;
      this.STATUS.Text = "Status";
      // 
      // XFMR
      // 
      this.XFMR.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.XFMR.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.XFMR.FormattingEnabled = true;
      this.XFMR.Items.AddRange(new object[] {
            "N/A",
            "15 KVA",
            "30 KVA",
            "45 KVA",
            "75 KVA",
            "112.5 KVA",
            "150 KVA",
            "225 KVA",
            "300 KVA",
            "500 KVA",
            "750 KVA",
            "1000 KVA"});
      this.XFMR.Location = new System.Drawing.Point(6, 147);
      this.XFMR.Name = "XFMR";
      this.XFMR.Size = new System.Drawing.Size(82, 21);
      this.XFMR.TabIndex = 4;
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(3, 131);
      this.label3.Margin = new System.Windows.Forms.Padding(0);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(85, 13);
      this.label3.TabIndex = 20;
      this.label3.Text = "Upstream XFMR";
      // 
      // panel1
      // 
      this.panel1.BackColor = System.Drawing.SystemColors.Window;
      this.panel1.Controls.Add(this.NAME);
      this.panel1.Location = new System.Drawing.Point(6, 104);
      this.panel1.Name = "panel1";
      this.panel1.Padding = new System.Windows.Forms.Padding(3, 0, 3, 0);
      this.panel1.Size = new System.Drawing.Size(64, 21);
      this.panel1.TabIndex = 2;
      // 
      // NAME
      // 
      this.NAME.BorderStyle = System.Windows.Forms.BorderStyle.None;
      this.NAME.Location = new System.Drawing.Point(6, 4);
      this.NAME.Name = "NAME";
      this.NAME.Size = new System.Drawing.Size(52, 13);
      this.NAME.TabIndex = 0;
      // 
      // SET_BREAKER_ITEM_LOCATION
      // 
      this.SET_BREAKER_ITEM_LOCATION.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.SET_BREAKER_ITEM_LOCATION.Location = new System.Drawing.Point(6, 179);
      this.SET_BREAKER_ITEM_LOCATION.Name = "SET_BREAKER_ITEM_LOCATION";
      this.SET_BREAKER_ITEM_LOCATION.Size = new System.Drawing.Size(220, 23);
      this.SET_BREAKER_ITEM_LOCATION.TabIndex = 6;
      this.SET_BREAKER_ITEM_LOCATION.Text = "Set Breaker Item Location";
      this.SET_BREAKER_ITEM_LOCATION.UseVisualStyleBackColor = true;
      // 
      // SET_XFMR_LOCATION
      // 
      this.SET_XFMR_LOCATION.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.SET_XFMR_LOCATION.Location = new System.Drawing.Point(6, 208);
      this.SET_XFMR_LOCATION.Name = "SET_XFMR_LOCATION";
      this.SET_XFMR_LOCATION.Size = new System.Drawing.Size(220, 23);
      this.SET_XFMR_LOCATION.TabIndex = 7;
      this.SET_XFMR_LOCATION.Text = "Set XFMR Location";
      this.SET_XFMR_LOCATION.UseVisualStyleBackColor = true;
      // 
      // POLE_GROUP
      // 
      this.POLE_GROUP.Controls.Add(this.POLE3);
      this.POLE_GROUP.Controls.Add(this.POLE2);
      this.POLE_GROUP.Controls.Add(this.POLE1);
      this.POLE_GROUP.Location = new System.Drawing.Point(83, 89);
      this.POLE_GROUP.Name = "POLE_GROUP";
      this.POLE_GROUP.Padding = new System.Windows.Forms.Padding(0);
      this.POLE_GROUP.Size = new System.Drawing.Size(143, 42);
      this.POLE_GROUP.TabIndex = 3;
      this.POLE_GROUP.TabStop = false;
      this.POLE_GROUP.Text = "Poles";
      // 
      // POLE3
      // 
      this.POLE3.AutoSize = true;
      this.POLE3.Checked = true;
      this.POLE3.Location = new System.Drawing.Point(99, 17);
      this.POLE3.Name = "POLE3";
      this.POLE3.Size = new System.Drawing.Size(38, 17);
      this.POLE3.TabIndex = 3;
      this.POLE3.TabStop = true;
      this.POLE3.Text = "3P";
      this.POLE3.UseVisualStyleBackColor = true;
      // 
      // POLE2
      // 
      this.POLE2.AutoSize = true;
      this.POLE2.Location = new System.Drawing.Point(55, 17);
      this.POLE2.Name = "POLE2";
      this.POLE2.Size = new System.Drawing.Size(38, 17);
      this.POLE2.TabIndex = 2;
      this.POLE2.Text = "2P";
      this.POLE2.UseVisualStyleBackColor = true;
      // 
      // POLE1
      // 
      this.POLE1.AutoSize = true;
      this.POLE1.Location = new System.Drawing.Point(11, 17);
      this.POLE1.Name = "POLE1";
      this.POLE1.Size = new System.Drawing.Size(38, 17);
      this.POLE1.TabIndex = 1;
      this.POLE1.Text = "1P";
      this.POLE1.UseVisualStyleBackColor = true;
      // 
      // ItemTab
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackColor = System.Drawing.Color.Snow;
      this.Controls.Add(this.POLE_GROUP);
      this.Controls.Add(this.SET_XFMR_LOCATION);
      this.Controls.Add(this.SET_BREAKER_ITEM_LOCATION);
      this.Controls.Add(this.panel1);
      this.Controls.Add(this.XFMR);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.STATUS);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.BREAKER_SIZES);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.TYPE);
      this.Controls.Add(this.label5);
      this.Margin = new System.Windows.Forms.Padding(0);
      this.Name = "ItemTab";
      this.Padding = new System.Windows.Forms.Padding(3);
      this.Size = new System.Drawing.Size(232, 240);
      this.STATUS.ResumeLayout(false);
      this.STATUS.PerformLayout();
      this.panel1.ResumeLayout(false);
      this.panel1.PerformLayout();
      this.POLE_GROUP.ResumeLayout(false);
      this.POLE_GROUP.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.ComboBox TYPE;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.ComboBox BREAKER_SIZES;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.RadioButton NEW;
    private System.Windows.Forms.RadioButton EXISTING;
    private System.Windows.Forms.GroupBox STATUS;
    private System.Windows.Forms.ComboBox XFMR;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Panel panel1;
    private System.Windows.Forms.TextBox NAME;
    private System.Windows.Forms.Button SET_BREAKER_ITEM_LOCATION;
    private System.Windows.Forms.Button SET_XFMR_LOCATION;
    private System.Windows.Forms.GroupBox POLE_GROUP;
    private System.Windows.Forms.RadioButton POLE3;
    private System.Windows.Forms.RadioButton POLE2;
    private System.Windows.Forms.RadioButton POLE1;
  }
}
