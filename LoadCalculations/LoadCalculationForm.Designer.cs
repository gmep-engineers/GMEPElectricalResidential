namespace GMEPElectricalResidential.LoadCalculations
{
  partial class LOAD_CALCULATION_FORM
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
      this.TAB_CONTROL = new System.Windows.Forms.TabControl();
      this.UNIT_TAB = new System.Windows.Forms.TabPage();
      this.UNIT_TAB_CONTROL = new System.Windows.Forms.TabControl();
      this.BUILDING_TAB = new System.Windows.Forms.TabPage();
      this.BUILDING_TAB_CONTROL = new System.Windows.Forms.TabControl();
      this.CREATE_UNIT_BUTTON = new System.Windows.Forms.Button();
      this.DELETE_UNIT_BUTTON = new System.Windows.Forms.Button();
      this.SAVE_BUTTON = new System.Windows.Forms.Button();
      this.CREATE = new System.Windows.Forms.Button();
      this.UPDATE = new System.Windows.Forms.Button();
      this.GROUP_BUILDING_CALCS = new System.Windows.Forms.CheckBox();
      this.DUPLICATE = new System.Windows.Forms.Button();
      this.label5 = new System.Windows.Forms.Label();
      this.panel2 = new System.Windows.Forms.Panel();
      this.INNER_SHEET_WIDTH = new System.Windows.Forms.TextBox();
      this.TAB_CONTROL.SuspendLayout();
      this.UNIT_TAB.SuspendLayout();
      this.BUILDING_TAB.SuspendLayout();
      this.panel2.SuspendLayout();
      this.SuspendLayout();
      // 
      // TAB_CONTROL
      // 
      this.TAB_CONTROL.Controls.Add(this.UNIT_TAB);
      this.TAB_CONTROL.Controls.Add(this.BUILDING_TAB);
      this.TAB_CONTROL.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.TAB_CONTROL.Location = new System.Drawing.Point(101, 12);
      this.TAB_CONTROL.Name = "TAB_CONTROL";
      this.TAB_CONTROL.SelectedIndex = 0;
      this.TAB_CONTROL.Size = new System.Drawing.Size(1558, 688);
      this.TAB_CONTROL.TabIndex = 0;
      // 
      // UNIT_TAB
      // 
      this.UNIT_TAB.Controls.Add(this.UNIT_TAB_CONTROL);
      this.UNIT_TAB.Location = new System.Drawing.Point(4, 29);
      this.UNIT_TAB.Name = "UNIT_TAB";
      this.UNIT_TAB.Padding = new System.Windows.Forms.Padding(3);
      this.UNIT_TAB.Size = new System.Drawing.Size(1550, 655);
      this.UNIT_TAB.TabIndex = 0;
      this.UNIT_TAB.Text = "Unit";
      this.UNIT_TAB.UseVisualStyleBackColor = true;
      // 
      // UNIT_TAB_CONTROL
      // 
      this.UNIT_TAB_CONTROL.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.UNIT_TAB_CONTROL.Location = new System.Drawing.Point(3, 3);
      this.UNIT_TAB_CONTROL.Name = "UNIT_TAB_CONTROL";
      this.UNIT_TAB_CONTROL.SelectedIndex = 0;
      this.UNIT_TAB_CONTROL.Size = new System.Drawing.Size(1547, 652);
      this.UNIT_TAB_CONTROL.TabIndex = 1;
      // 
      // BUILDING_TAB
      // 
      this.BUILDING_TAB.Controls.Add(this.BUILDING_TAB_CONTROL);
      this.BUILDING_TAB.Location = new System.Drawing.Point(4, 29);
      this.BUILDING_TAB.Name = "BUILDING_TAB";
      this.BUILDING_TAB.Padding = new System.Windows.Forms.Padding(3);
      this.BUILDING_TAB.Size = new System.Drawing.Size(1550, 655);
      this.BUILDING_TAB.TabIndex = 1;
      this.BUILDING_TAB.Text = "Building";
      this.BUILDING_TAB.UseVisualStyleBackColor = true;
      // 
      // BUILDING_TAB_CONTROL
      // 
      this.BUILDING_TAB_CONTROL.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.BUILDING_TAB_CONTROL.Location = new System.Drawing.Point(2, 1);
      this.BUILDING_TAB_CONTROL.Name = "BUILDING_TAB_CONTROL";
      this.BUILDING_TAB_CONTROL.SelectedIndex = 0;
      this.BUILDING_TAB_CONTROL.Size = new System.Drawing.Size(1547, 652);
      this.BUILDING_TAB_CONTROL.TabIndex = 2;
      // 
      // CREATE_UNIT_BUTTON
      // 
      this.CREATE_UNIT_BUTTON.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.CREATE_UNIT_BUTTON.Location = new System.Drawing.Point(12, 52);
      this.CREATE_UNIT_BUTTON.Name = "CREATE_UNIT_BUTTON";
      this.CREATE_UNIT_BUTTON.Size = new System.Drawing.Size(83, 34);
      this.CREATE_UNIT_BUTTON.TabIndex = 1;
      this.CREATE_UNIT_BUTTON.Text = "New";
      this.CREATE_UNIT_BUTTON.UseVisualStyleBackColor = true;
      this.CREATE_UNIT_BUTTON.Click += new System.EventHandler(this.CREATE_UNIT_BUTTON_Click);
      // 
      // DELETE_UNIT_BUTTON
      // 
      this.DELETE_UNIT_BUTTON.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.DELETE_UNIT_BUTTON.ForeColor = System.Drawing.Color.Red;
      this.DELETE_UNIT_BUTTON.Location = new System.Drawing.Point(12, 663);
      this.DELETE_UNIT_BUTTON.Name = "DELETE_UNIT_BUTTON";
      this.DELETE_UNIT_BUTTON.Size = new System.Drawing.Size(83, 33);
      this.DELETE_UNIT_BUTTON.TabIndex = 2;
      this.DELETE_UNIT_BUTTON.Text = "Delete";
      this.DELETE_UNIT_BUTTON.UseVisualStyleBackColor = true;
      this.DELETE_UNIT_BUTTON.Click += new System.EventHandler(this.DELETE_UNIT_BUTTON_Click);
      // 
      // SAVE_BUTTON
      // 
      this.SAVE_BUTTON.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.SAVE_BUTTON.Location = new System.Drawing.Point(12, 12);
      this.SAVE_BUTTON.Name = "SAVE_BUTTON";
      this.SAVE_BUTTON.Size = new System.Drawing.Size(83, 34);
      this.SAVE_BUTTON.TabIndex = 4;
      this.SAVE_BUTTON.Text = "Save";
      this.SAVE_BUTTON.UseVisualStyleBackColor = true;
      this.SAVE_BUTTON.Click += new System.EventHandler(this.SAVE_BUTTON_Click);
      // 
      // CREATE
      // 
      this.CREATE.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.CREATE.Location = new System.Drawing.Point(12, 92);
      this.CREATE.Name = "CREATE";
      this.CREATE.Size = new System.Drawing.Size(83, 34);
      this.CREATE.TabIndex = 5;
      this.CREATE.Text = "Create";
      this.CREATE.UseVisualStyleBackColor = true;
      this.CREATE.Click += new System.EventHandler(this.CREATE_Click);
      // 
      // UPDATE
      // 
      this.UPDATE.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.UPDATE.Location = new System.Drawing.Point(12, 132);
      this.UPDATE.Name = "UPDATE";
      this.UPDATE.Size = new System.Drawing.Size(83, 34);
      this.UPDATE.TabIndex = 6;
      this.UPDATE.Text = "Update";
      this.UPDATE.UseVisualStyleBackColor = true;
      this.UPDATE.Click += new System.EventHandler(this.UPDATE_Click);
      // 
      // GROUP_BUILDING_CALCS
      // 
      this.GROUP_BUILDING_CALCS.AutoSize = true;
      this.GROUP_BUILDING_CALCS.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.GROUP_BUILDING_CALCS.Enabled = false;
      this.GROUP_BUILDING_CALCS.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.GROUP_BUILDING_CALCS.Location = new System.Drawing.Point(11, 302);
      this.GROUP_BUILDING_CALCS.Name = "GROUP_BUILDING_CALCS";
      this.GROUP_BUILDING_CALCS.Size = new System.Drawing.Size(84, 64);
      this.GROUP_BUILDING_CALCS.TabIndex = 205;
      this.GROUP_BUILDING_CALCS.Text = "Group\r\nBuilding\r\nCalcs";
      this.GROUP_BUILDING_CALCS.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      this.GROUP_BUILDING_CALCS.UseVisualStyleBackColor = true;
      // 
      // DUPLICATE
      // 
      this.DUPLICATE.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.DUPLICATE.Location = new System.Drawing.Point(12, 172);
      this.DUPLICATE.Name = "DUPLICATE";
      this.DUPLICATE.Size = new System.Drawing.Size(83, 34);
      this.DUPLICATE.TabIndex = 206;
      this.DUPLICATE.Text = "Duplicate";
      this.DUPLICATE.UseVisualStyleBackColor = true;
      this.DUPLICATE.Click += new System.EventHandler(this.DUPLICATE_Click);
      // 
      // label5
      // 
      this.label5.AutoSize = true;
      this.label5.BackColor = System.Drawing.Color.Transparent;
      this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label5.Location = new System.Drawing.Point(8, 222);
      this.label5.Margin = new System.Windows.Forms.Padding(0, 0, 0, 3);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(93, 40);
      this.label5.TabIndex = 208;
      this.label5.Text = "Inner Sheet\r\nWidth";
      // 
      // panel2
      // 
      this.panel2.BackColor = System.Drawing.SystemColors.Window;
      this.panel2.Controls.Add(this.INNER_SHEET_WIDTH);
      this.panel2.Location = new System.Drawing.Point(12, 265);
      this.panel2.Margin = new System.Windows.Forms.Padding(6, 0, 0, 6);
      this.panel2.Name = "panel2";
      this.panel2.Padding = new System.Windows.Forms.Padding(5);
      this.panel2.Size = new System.Drawing.Size(83, 28);
      this.panel2.TabIndex = 207;
      // 
      // INNER_SHEET_WIDTH
      // 
      this.INNER_SHEET_WIDTH.BorderStyle = System.Windows.Forms.BorderStyle.None;
      this.INNER_SHEET_WIDTH.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.INNER_SHEET_WIDTH.Location = new System.Drawing.Point(5, 5);
      this.INNER_SHEET_WIDTH.Margin = new System.Windows.Forms.Padding(0);
      this.INNER_SHEET_WIDTH.Name = "INNER_SHEET_WIDTH";
      this.INNER_SHEET_WIDTH.Size = new System.Drawing.Size(73, 19);
      this.INNER_SHEET_WIDTH.TabIndex = 0;
      // 
      // LOAD_CALCULATION_FORM
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackColor = System.Drawing.SystemColors.Control;
      this.ClientSize = new System.Drawing.Size(1671, 727);
      this.Controls.Add(this.label5);
      this.Controls.Add(this.panel2);
      this.Controls.Add(this.DUPLICATE);
      this.Controls.Add(this.GROUP_BUILDING_CALCS);
      this.Controls.Add(this.UPDATE);
      this.Controls.Add(this.CREATE);
      this.Controls.Add(this.SAVE_BUTTON);
      this.Controls.Add(this.DELETE_UNIT_BUTTON);
      this.Controls.Add(this.CREATE_UNIT_BUTTON);
      this.Controls.Add(this.TAB_CONTROL);
      this.Name = "LOAD_CALCULATION_FORM";
      this.Text = "Load Calculation Form";
      this.TAB_CONTROL.ResumeLayout(false);
      this.UNIT_TAB.ResumeLayout(false);
      this.BUILDING_TAB.ResumeLayout(false);
      this.panel2.ResumeLayout(false);
      this.panel2.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.TabControl TAB_CONTROL;
    private System.Windows.Forms.Button CREATE_UNIT_BUTTON;
    private System.Windows.Forms.Button DELETE_UNIT_BUTTON;
    private System.Windows.Forms.Button SAVE_BUTTON;
    private System.Windows.Forms.Button CREATE;
    private System.Windows.Forms.TabPage UNIT_TAB;
    private System.Windows.Forms.TabPage BUILDING_TAB;
    private System.Windows.Forms.TabControl UNIT_TAB_CONTROL;
    private System.Windows.Forms.TabControl BUILDING_TAB_CONTROL;
    private System.Windows.Forms.Button UPDATE;
    private System.Windows.Forms.CheckBox GROUP_BUILDING_CALCS;
    private System.Windows.Forms.Button DUPLICATE;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.Panel panel2;
    private System.Windows.Forms.TextBox INNER_SHEET_WIDTH;
  }
}