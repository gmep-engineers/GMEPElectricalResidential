using System.Windows.Forms;
namespace GMEPElectricalResidential.LoadCalculations
{
  partial class ItemSelectionControl : UserControl
  {
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;
    // Declare control variables here
    private Label lblItem;
    private RadioButton rbOption1;
    private RadioButton rbOption2;
    private RadioButton rbOption3;
    private GroupBox groupBox;
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
      this.lblItem = new System.Windows.Forms.Label();
      this.rbOption1 = new System.Windows.Forms.RadioButton();
      this.rbOption2 = new System.Windows.Forms.RadioButton();
      this.rbOption3 = new System.Windows.Forms.RadioButton();
      this.groupBox = new System.Windows.Forms.GroupBox();
      this.groupBox.SuspendLayout();
      this.SuspendLayout();
      // 
      // lblItem
      // 
      this.lblItem.AutoSize = false;
      this.lblItem.Location = new System.Drawing.Point(5, 5);
      this.lblItem.Name = "lblItem";
      this.lblItem.Size = new System.Drawing.Size(80, 40);
      this.lblItem.TabIndex = 0;
      this.lblItem.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.lblItem.AutoEllipsis = true;
      this.lblItem.BackColor = System.Drawing.Color.Transparent;
      // 
      // rbOption1
      // 
      this.rbOption1.AutoSize = true;
      this.rbOption1.Checked = true;
      this.rbOption1.Location = new System.Drawing.Point(90, 16);
      this.rbOption1.Name = "rbOption1";
      this.rbOption1.Size = new System.Drawing.Size(31, 17);
      this.rbOption1.TabIndex = 1;
      this.rbOption1.TabStop = true;
      this.rbOption1.Text = "1";
      // 
      // rbOption2
      // 
      this.rbOption2.AutoSize = true;
      this.rbOption2.Location = new System.Drawing.Point(125, 16);
      this.rbOption2.Name = "rbOption2";
      this.rbOption2.Size = new System.Drawing.Size(31, 17);
      this.rbOption2.TabIndex = 2;
      this.rbOption2.Text = "2";
      // 
      // rbOption3
      // 
      this.rbOption3.AutoSize = true;
      this.rbOption3.Location = new System.Drawing.Point(160, 16);
      this.rbOption3.Name = "rbOption3";
      this.rbOption3.Size = new System.Drawing.Size(31, 17);
      this.rbOption3.TabIndex = 3;
      this.rbOption3.Text = "3";
      // 
      // groupBox
      // 
      this.groupBox.Controls.Add(this.lblItem);
      this.groupBox.Controls.Add(this.rbOption1);
      this.groupBox.Controls.Add(this.rbOption2);
      this.groupBox.Controls.Add(this.rbOption3);
      this.groupBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.groupBox.Location = new System.Drawing.Point(0, 0);
      this.groupBox.Name = "groupBox";
      this.groupBox.Size = new System.Drawing.Size(200, 50);
      this.groupBox.TabIndex = 0;
      this.groupBox.TabStop = false;
      this.groupBox.Padding = new Padding(3);
      // 
      // ItemSelectionControl
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.groupBox);
      this.Name = "ItemSelectionControl";
      this.Size = new System.Drawing.Size(200, 50);
      this.groupBox.ResumeLayout(false);
      this.groupBox.PerformLayout();
      this.ResumeLayout(false);
    }
    #endregion
  }
}