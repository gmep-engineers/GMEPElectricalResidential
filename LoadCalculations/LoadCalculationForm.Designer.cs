namespace GMEPElectricalResidential
{
  partial class LoadCalculationForm
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
      this.CREATE_UNIT_BUTTON = new System.Windows.Forms.Button();
      this.DELETE_UNIT_BUTTON = new System.Windows.Forms.Button();
      this.SAVE_BUTTON = new System.Windows.Forms.Button();
      this.CREATE = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // TAB_CONTROL
      // 
      this.TAB_CONTROL.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.TAB_CONTROL.Location = new System.Drawing.Point(101, 12);
      this.TAB_CONTROL.Name = "TAB_CONTROL";
      this.TAB_CONTROL.SelectedIndex = 0;
      this.TAB_CONTROL.Size = new System.Drawing.Size(1483, 652);
      this.TAB_CONTROL.TabIndex = 0;
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
      this.DELETE_UNIT_BUTTON.Location = new System.Drawing.Point(12, 630);
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
      // LoadCalculationForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackColor = System.Drawing.SystemColors.Control;
      this.ClientSize = new System.Drawing.Size(1596, 675);
      this.Controls.Add(this.CREATE);
      this.Controls.Add(this.SAVE_BUTTON);
      this.Controls.Add(this.DELETE_UNIT_BUTTON);
      this.Controls.Add(this.CREATE_UNIT_BUTTON);
      this.Controls.Add(this.TAB_CONTROL);
      this.Name = "LoadCalculationForm";
      this.Text = "LoadCalculationForm";
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TabControl TAB_CONTROL;
    private System.Windows.Forms.Button CREATE_UNIT_BUTTON;
    private System.Windows.Forms.Button DELETE_UNIT_BUTTON;
    private System.Windows.Forms.Button SAVE_BUTTON;
    private System.Windows.Forms.Button CREATE;
  }
}