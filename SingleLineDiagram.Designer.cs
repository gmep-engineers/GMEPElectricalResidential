namespace GMEPElectricalResidential
{
  partial class SINGLE_LINE_DIAGRAM
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SINGLE_LINE_DIAGRAM));
      this.UPWARDS_ARROW = new System.Windows.Forms.PictureBox();
      this.PANEL = new GMEPElectricalResidential.DoubleBufferedPanel();
      ((System.ComponentModel.ISupportInitialize)(this.UPWARDS_ARROW)).BeginInit();
      this.SuspendLayout();
      // 
      // UPWARDS_ARROW
      // 
      this.UPWARDS_ARROW.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.UPWARDS_ARROW.Image = ((System.Drawing.Image)(resources.GetObject("UPWARDS_ARROW.Image")));
      this.UPWARDS_ARROW.Location = new System.Drawing.Point(12, 12);
      this.UPWARDS_ARROW.Name = "UPWARDS_ARROW";
      this.UPWARDS_ARROW.Size = new System.Drawing.Size(64, 64);
      this.UPWARDS_ARROW.TabIndex = 5;
      this.UPWARDS_ARROW.TabStop = false;
      // 
      // PANEL
      // 
      this.PANEL.AllowDrop = true;
      this.PANEL.AutoScroll = true;
      this.PANEL.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.PANEL.Location = new System.Drawing.Point(82, 12);
      this.PANEL.Name = "PANEL";
      this.PANEL.Size = new System.Drawing.Size(972, 452);
      this.PANEL.TabIndex = 6;
      // 
      // SINGLE_LINE_DIAGRAM
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(1066, 476);
      this.Controls.Add(this.PANEL);
      this.Controls.Add(this.UPWARDS_ARROW);
      this.Name = "SINGLE_LINE_DIAGRAM";
      this.Text = "Single Line Diagram";
      ((System.ComponentModel.ISupportInitialize)(this.UPWARDS_ARROW)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion
    private System.Windows.Forms.PictureBox UPWARDS_ARROW;
    private DoubleBufferedPanel PANEL;
  }
}