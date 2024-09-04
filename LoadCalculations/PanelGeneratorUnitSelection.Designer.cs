namespace GMEPElectricalResidential.LoadCalculations
{
  partial class PanelGeneratorUnitSelection
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
      this.label1 = new System.Windows.Forms.Label();
      this.UNIT_TYPES = new System.Windows.Forms.ListBox();
      this.PROCEED_BUTTON = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
      this.label1.Location = new System.Drawing.Point(27, 24);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(288, 17);
      this.label1.TabIndex = 0;
      this.label1.Text = "Select similar unit types for panel generation";
      // 
      // UNIT_TYPES
      // 
      this.UNIT_TYPES.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
      this.UNIT_TYPES.FormattingEnabled = true;
      this.UNIT_TYPES.ItemHeight = 16;
      this.UNIT_TYPES.Location = new System.Drawing.Point(27, 44);
      this.UNIT_TYPES.Name = "UNIT_TYPES";
      this.UNIT_TYPES.Size = new System.Drawing.Size(288, 260);
      this.UNIT_TYPES.TabIndex = 1;
      // 
      // PROCEED_BUTTON
      // 
      this.PROCEED_BUTTON.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
      this.PROCEED_BUTTON.Location = new System.Drawing.Point(27, 310);
      this.PROCEED_BUTTON.Name = "PROCEED_BUTTON";
      this.PROCEED_BUTTON.Size = new System.Drawing.Size(288, 30);
      this.PROCEED_BUTTON.TabIndex = 2;
      this.PROCEED_BUTTON.Text = "Proceed";
      this.PROCEED_BUTTON.UseVisualStyleBackColor = true;
      // 
      // PanelGeneratorUnitSelection
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(348, 366);
      this.Controls.Add(this.PROCEED_BUTTON);
      this.Controls.Add(this.UNIT_TYPES);
      this.Controls.Add(this.label1);
      this.Name = "PanelGeneratorUnitSelection";
      this.Padding = new System.Windows.Forms.Padding(24);
      this.Text = "PanelGeneratorUnitSelection";
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.ListBox UNIT_TYPES;
    private System.Windows.Forms.Button PROCEED_BUTTON;
  }
}