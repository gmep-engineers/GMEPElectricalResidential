using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace GMEPElectricalResidential
{
  public partial class DistributionForm : UserControl
  {
    private TabPage _draggingTab = null;
    private string distributionWaterMark = "Enter dist name...";
    private int tabCount = 0;

    public DistributionForm(Point location)
    {
      Location = location;
      InitializeComponent();
      SetWatermarkText();

      CONFIGURATION.SelectedIndex = 0;
      KAIC.SelectedIndex = 1;
      SIZE.SelectedIndex = 0;
      PARENT.SelectedIndex = 0;

      DISTRIBUTION_NAME.Enter += PANEL_NAME_Enter;
      DISTRIBUTION_NAME.Leave += PANEL_NAME_Leave;
      TABS.MouseDown += TABS_MouseDown;
      TABS.MouseMove += TABS_MouseMove;
      TABS.MouseUp += TABS_MouseUp;
    }

    private void ADD_CHILD_Click(object sender, EventArgs e)
    {
      CreateTab();
    }

    private void DELETE_CHILD_Click(object sender, EventArgs e)
    {
      int index = TABS.SelectedIndex;
      RemoveTab(index);
    }

    // WATERMARKS
    private void SetWatermarkText()
    {
      DISTRIBUTION_NAME.ForeColor = Color.Gray;
      DISTRIBUTION_NAME.Text = distributionWaterMark;
    }

    private void PANEL_NAME_Enter(object sender, EventArgs e)
    {
      if (DISTRIBUTION_NAME.Text == distributionWaterMark)
      {
        DISTRIBUTION_NAME.Text = "";
        DISTRIBUTION_NAME.ForeColor = Color.Black;
      }
    }

    private void PANEL_NAME_Leave(object sender, EventArgs e)
    {
      if (string.IsNullOrWhiteSpace(DISTRIBUTION_NAME.Text))
      {
        SetWatermarkText();
      }
    }

    // TABS
    private void TABS_MouseDown(object sender, MouseEventArgs e)
    {
      _draggingTab = GetTabPageByTab(TABS, e.Location);
    }

    private void TABS_MouseMove(object sender, MouseEventArgs e)
    {
      if (e.Button != MouseButtons.Left || _draggingTab == null)
        return;

      var targetTab = GetTabPageByTab(TABS, e.Location);
      if (targetTab != null && _draggingTab != targetTab)
      {
        int targetIndex = TABS.TabPages.IndexOf(targetTab);
        int draggingTabIndex = TABS.TabPages.IndexOf(_draggingTab);

        // Check if we need to move the tab
        if (targetIndex != draggingTabIndex)
        {
          // Determine the direction of the move based on mouse position relative to the target tab's midpoint
          Rectangle targetTabRect = TABS.GetTabRect(targetIndex);
          bool moveRight = e.X > (targetTabRect.Left + targetTabRect.Width / 2);

          // Calculate new index based on the direction
          int newIndex = moveRight ? targetIndex + 1 : targetIndex;

          // Ensure new index is within bounds
          newIndex = Math.Max(0, Math.Min(TABS.TabPages.Count - 1, newIndex));

          // Move the tab
          TABS.TabPages.Remove(_draggingTab);
          TABS.TabPages.Insert(newIndex, _draggingTab);
          TABS.SelectedTab = _draggingTab;
        }
      }
    }

    private void TABS_MouseUp(object sender, MouseEventArgs e)
    {
      _draggingTab = null;
      RenameTabs();
    }

    private TabPage GetTabPageByTab(TabControl tabControl, Point position)
    {
      for (int i = 0; i < tabControl.TabCount; i++)
      {
        if (tabControl.GetTabRect(i).Contains(position))
          return tabControl.TabPages[i];
      }
      return null;
    }

    private void CreateTab()
    {
      tabCount++;

      TabPage tab = new TabPage
      {
        Text = "Child " + tabCount.ToString()
      };

      var tabUserControl = new ItemTab();

      tab.Controls.Add(tabUserControl);
      TABS.TabPages.Add(tab);
    }

    private void RemoveTab(int index)
    {
      if (TABS.TabPages.Count > 0)
      {
        TABS.TabPages.RemoveAt(index);
      }

      RenameTabs();
      tabCount--;
    }

    private void RenameTabs()
    {
      for (int i = 0; i < TABS.TabPages.Count; i++)
      {
        TABS.TabPages[i].Text = $"Child {i + 1}";
      }
    }
  }
}