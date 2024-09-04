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
      components = new System.ComponentModel.Container();
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;

      lblItem = new Label();
      rbOption1 = new RadioButton();
      rbOption2 = new RadioButton();
      groupBox = new GroupBox();

      lblItem.AutoSize = true;
      lblItem.Location = new System.Drawing.Point(10, 20);
      lblItem.Name = "lblItem";
      lblItem.Size = new System.Drawing.Size(100, 20);

      rbOption1.AutoSize = true;
      rbOption1.Location = new System.Drawing.Point(120, 20);
      rbOption1.Name = "rbOption1";
      rbOption1.Size = new System.Drawing.Size(40, 20);
      rbOption1.Text = "1";
      rbOption1.Checked = true;

      rbOption2.AutoSize = true;
      rbOption2.Location = new System.Drawing.Point(170, 20);
      rbOption2.Name = "rbOption2";
      rbOption2.Size = new System.Drawing.Size(40, 20);
      rbOption2.Text = "2";

      groupBox.Controls.Add(lblItem);
      groupBox.Controls.Add(rbOption1);
      groupBox.Controls.Add(rbOption2);
      groupBox.Dock = DockStyle.Fill;

      this.Controls.Add(groupBox);
      this.Size = new System.Drawing.Size(220, 50);
    }
    #endregion
  }
}