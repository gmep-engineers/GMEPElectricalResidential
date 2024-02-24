namespace GMEPElectricalResidential
{
  partial class PullSectionBelowForm
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
      this.SuspendLayout();
      // 
      // AMPERAGE
      // 
      this.AMPERAGE.Location = new System.Drawing.Point(9, 22);
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
      // PullSectionBelowForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.Controls.Add(this.label1);
      this.Controls.Add(this.AMPERAGE);
      this.Name = "PullSectionBelowForm";
      this.Padding = new System.Windows.Forms.Padding(6);
      this.Size = new System.Drawing.Size(1040, 160);
      this.Tag = "UPWARDS_ARROW";
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.TextBox AMPERAGE;
    private System.Windows.Forms.Label label1;
  }
}
