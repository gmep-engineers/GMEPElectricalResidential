using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static GMEPElectricalResidential.SINGLE_LINE_DIAGRAM;

namespace GMEPElectricalResidential
{
  public partial class SINGLE_LINE_DIAGRAM : Form
  {
    private List<DraggableObject> draggableObjects = new List<DraggableObject>();
    private DraggableObject currentDraggableObject;

    public SINGLE_LINE_DIAGRAM()
    {
      InitializeComponent();
      UPWARDS_ARROW.MouseDown += new MouseEventHandler(UPWARDS_ARROW_MouseDown);
      PANEL.AllowDrop = true;
      PANEL.DragEnter += new DragEventHandler(PANEL_DragEnter);
      PANEL.DragDrop += new DragEventHandler(PANEL_DragDrop);
      PANEL.MouseDown += new MouseEventHandler(PANEL_MouseDown);
      PANEL.MouseMove += new MouseEventHandler(PANEL_MouseMove);
      PANEL.MouseUp += new MouseEventHandler(PANEL_MouseUp);
    }

    private void UPWARDS_ARROW_MouseDown(object sender, MouseEventArgs e)
    {
      if (e.Button == MouseButtons.Left)
      {
        UPWARDS_ARROW.DoDragDrop(UPWARDS_ARROW.Image, DragDropEffects.Copy);
      }
    }

    private void PANEL_DragEnter(object sender, DragEventArgs e)
    {
      if (e.Data.GetDataPresent(DataFormats.Bitmap))
      {
        e.Effect = DragDropEffects.Copy;
      }
      else
      {
        e.Effect = DragDropEffects.None;
      }
    }

    protected override void OnPaint(PaintEventArgs e)
    {
      base.OnPaint(e);
      foreach (var draggable in draggableObjects)
      {
        e.Graphics.DrawImage(draggable.Image, draggable.Bounds);
      }
    }

    private void PANEL_MouseDown(object sender, MouseEventArgs e)
    {
      // Check if the click is within the bounds of any draggable object
      foreach (var draggable in draggableObjects)
      {
        if (draggable.Bounds.Contains(e.Location))
        {
          currentDraggableObject = draggable;
          // Calculate the click offset from the top-left corner of the image
          currentDraggableObject.ClickOffset = new Point(e.X - draggable.Bounds.X, e.Y - draggable.Bounds.Y);
          currentDraggableObject.IsDragging = true;
          break;
        }
      }
    }

    private void PANEL_MouseMove(object sender, MouseEventArgs e)
    {
      if (currentDraggableObject != null && currentDraggableObject.IsDragging)
      {
        // Update the position of the image
        currentDraggableObject.Bounds = new Rectangle(e.X - currentDraggableObject.ClickOffset.X, e.Y - currentDraggableObject.ClickOffset.Y, currentDraggableObject.Bounds.Width, currentDraggableObject.Bounds.Height);
        PANEL.Invalidate(); // Invalidate the PANEL to trigger a repaint
      }
    }

    private void PANEL_MouseUp(object sender, MouseEventArgs e)
    {
      if (currentDraggableObject != null)
      {
        currentDraggableObject.IsDragging = false;
        currentDraggableObject = null;
      }
    }

    private void PANEL_DragDrop(object sender, DragEventArgs e)
    {
      Point dropPoint = PANEL.PointToClient(new Point(e.X, e.Y));
      Image droppedImage = (Image)e.Data.GetData(DataFormats.Bitmap);

      // Create a new DraggableObject with the dropped image
      DraggableObject draggable = new DraggableObject()
      {
        Image = droppedImage,
        Bounds = new Rectangle(dropPoint, droppedImage.Size)
      };

      // Add to the list of draggable objects
      draggableObjects.Add(draggable);

      // Invalidate the PANEL to trigger a repaint
      PANEL.Invalidate();
    }
  }

  public class DraggableObject
  {
    public Image Image { get; set; }
    public Rectangle Bounds { get; set; }
    public bool IsDragging { get; set; }
    public Point ClickOffset { get; set; }
  }
}