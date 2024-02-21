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
      this.TRASH = new System.Windows.Forms.PictureBox();
      this.DOWNWARDS_ARROW = new System.Windows.Forms.PictureBox();
      this.MAIN = new System.Windows.Forms.PictureBox();
      this.METER_MAIN = new System.Windows.Forms.PictureBox();
      ((System.ComponentModel.ISupportInitialize)(this.UPWARDS_ARROW)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.TRASH)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.DOWNWARDS_ARROW)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.MAIN)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.METER_MAIN)).BeginInit();
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
      this.PANEL.Location = new System.Drawing.Point(152, 12);
      this.PANEL.Name = "PANEL";
      this.PANEL.Size = new System.Drawing.Size(902, 452);
      this.PANEL.TabIndex = 6;
      // 
      // TRASH
      // 
      this.TRASH.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.TRASH.Image = ((System.Drawing.Image)(resources.GetObject("TRASH.Image")));
      this.TRASH.Location = new System.Drawing.Point(82, 400);
      this.TRASH.Name = "TRASH";
      this.TRASH.Size = new System.Drawing.Size(64, 64);
      this.TRASH.TabIndex = 7;
      this.TRASH.TabStop = false;
      // 
      // DOWNWARDS_ARROW
      // 
      this.DOWNWARDS_ARROW.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.DOWNWARDS_ARROW.Image = ((System.Drawing.Image)(resources.GetObject("DOWNWARDS_ARROW.Image")));
      this.DOWNWARDS_ARROW.Location = new System.Drawing.Point(82, 12);
      this.DOWNWARDS_ARROW.Name = "DOWNWARDS_ARROW";
      this.DOWNWARDS_ARROW.Size = new System.Drawing.Size(64, 64);
      this.DOWNWARDS_ARROW.TabIndex = 8;
      this.DOWNWARDS_ARROW.TabStop = false;
      // 
      // MAIN
      // 
      this.MAIN.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.MAIN.Image = ((System.Drawing.Image)(resources.GetObject("MAIN.Image")));
      this.MAIN.Location = new System.Drawing.Point(82, 82);
      this.MAIN.Name = "MAIN";
      this.MAIN.Size = new System.Drawing.Size(64, 64);
      this.MAIN.TabIndex = 9;
      this.MAIN.TabStop = false;
      // 
      // METER_MAIN
      // 
      this.METER_MAIN.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.METER_MAIN.Image = ((System.Drawing.Image)(resources.GetObject("METER_MAIN.Image")));
      this.METER_MAIN.Location = new System.Drawing.Point(12, 82);
      this.METER_MAIN.Name = "METER_MAIN";
      this.METER_MAIN.Size = new System.Drawing.Size(64, 64);
      this.METER_MAIN.TabIndex = 10;
      this.METER_MAIN.TabStop = false;
      // 
      // SINGLE_LINE_DIAGRAM
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(1066, 476);
      this.Controls.Add(this.METER_MAIN);
      this.Controls.Add(this.MAIN);
      this.Controls.Add(this.DOWNWARDS_ARROW);
      this.Controls.Add(this.TRASH);
      this.Controls.Add(this.PANEL);
      this.Controls.Add(this.UPWARDS_ARROW);
      this.Name = "SINGLE_LINE_DIAGRAM";
      this.Text = "Single Line Diagram";
      ((System.ComponentModel.ISupportInitialize)(this.UPWARDS_ARROW)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.TRASH)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.DOWNWARDS_ARROW)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.MAIN)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.METER_MAIN)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion
    private System.Windows.Forms.PictureBox UPWARDS_ARROW;
    private DoubleBufferedPanel PANEL;
    private System.Windows.Forms.PictureBox TRASH;
    private System.Windows.Forms.PictureBox DOWNWARDS_ARROW;
    private System.Windows.Forms.PictureBox MAIN;
    private System.Windows.Forms.PictureBox METER_MAIN;
  }
}