using GMEPUtilities;
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
      PANEL.Paint += new PaintEventHandler(PANEL_Paint);
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

    private void PANEL_Paint(object sender, PaintEventArgs e)
    {
      var g = e.Graphics;
      g.TranslateTransform(PANEL.AutoScrollPosition.X, PANEL.AutoScrollPosition.Y);

      foreach (var draggable in draggableObjects)
      {
        e.Graphics.DrawImage(draggable.Image, draggable.Bounds);

        // Draw a border around the image
        if (draggable.IsDragging)
        {
          // You can highlight the border if the item is being dragged
          using (Pen borderPen = new Pen(Color.Red, 2)) // Red border, 2 pixels wide
          {
            e.Graphics.DrawRectangle(borderPen, draggable.Bounds);
          }
        }
        else
        {
          // Regular border for not-dragging state
          using (Pen borderPen = new Pen(Color.Black, 1)) // Black border, 1 pixel wide
          {
            e.Graphics.DrawRectangle(borderPen, draggable.Bounds);
          }
        }
      }
    }

    private void PANEL_MouseDown(object sender, MouseEventArgs e)
    {
      Point clientPoint = new Point(e.X - PANEL.AutoScrollPosition.X, e.Y - PANEL.AutoScrollPosition.Y);
      foreach (var draggable in draggableObjects)
      {
        if (draggable.Bounds.Contains(clientPoint))
        {
          currentDraggableObject = draggable;
          currentDraggableObject.ClickOffset = new Point(clientPoint.X - draggable.Bounds.X, clientPoint.Y - draggable.Bounds.Y);
          currentDraggableObject.IsDragging = true;
          break;
        }
      }
    }

    private void PANEL_MouseMove(object sender, MouseEventArgs e)
    {
      if (currentDraggableObject != null && currentDraggableObject.IsDragging)
      {
        Point clientPoint = new Point(e.X - PANEL.AutoScrollPosition.X, e.Y - PANEL.AutoScrollPosition.Y);
        currentDraggableObject.Bounds = new Rectangle(clientPoint.X - currentDraggableObject.ClickOffset.X, clientPoint.Y - currentDraggableObject.ClickOffset.Y, currentDraggableObject.Bounds.Width, currentDraggableObject.Bounds.Height);
        UpdateScrollBars();
        PANEL.Invalidate();
      }
    }

    private void PANEL_MouseUp(object sender, MouseEventArgs e)
    {
      if (currentDraggableObject != null)
      {
        currentDraggableObject.IsDragging = false;
        currentDraggableObject = null;
      }
      UpdateScrollBars();
      PANEL.Invalidate();
    }

    private void PANEL_DragDrop(object sender, DragEventArgs e)
    {
      Point dropPoint = PANEL.PointToClient(new Point(e.X, e.Y));
      Image droppedImage = (Image)e.Data.GetData(DataFormats.Bitmap);

      DraggableObject draggable = new DraggableObject()
      {
        Image = droppedImage,
        Bounds = new Rectangle(dropPoint, droppedImage.Size)
      };

      draggableObjects.Add(draggable);
      UpdateScrollBars();
      PANEL.Invalidate();
    }

    private void UpdateScrollBars()
    {
      int maxWidth = 0;
      int maxHeight = 0;

      foreach (var draggable in draggableObjects)
      {
        // Find the furthest edges of all draggable objects
        int rightEdge = draggable.Bounds.Right;
        int bottomEdge = draggable.Bounds.Bottom;

        if (rightEdge > maxWidth)
        {
          maxWidth = rightEdge;
        }
        if (bottomEdge > maxHeight)
        {
          maxHeight = bottomEdge;
        }
      }

      // Update the AutoScrollMinSize property
      PANEL.AutoScrollMinSize = new Size(maxWidth, maxHeight);
    }
  }

  public class DraggableObject
  {
    public Image Image { get; set; }
    public Rectangle Bounds { get; set; }
    public bool IsDragging { get; set; }
    public Point ClickOffset { get; set; }
  }

  public class DoubleBufferedPanel : Panel
  {
    public DoubleBufferedPanel()
    {
      this.DoubleBuffered = true;
      this.SetStyle(ControlStyles.ResizeRedraw, true);
      this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
      this.UpdateStyles();
    }
  }
}