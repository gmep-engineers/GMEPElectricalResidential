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
    private bool isDraggingForDeletion = false;

    public SINGLE_LINE_DIAGRAM()
    {
      InitializeComponent();
      UPWARDS_ARROW.MouseDown += new MouseEventHandler(UPWARDS_ARROW_MouseDown);
      DOWNWARDS_ARROW.MouseDown += new MouseEventHandler(DOWNWARDS_ARROW_MouseDown);
      METER_MAIN.MouseDown += new MouseEventHandler(METER_MAIN_MouseDown);
      MAIN.MouseDown += new MouseEventHandler(MAIN_MouseDown);
      METER_COMBO.MouseDown += new MouseEventHandler(METER_COMBO_MouseDown);
      DISTRIBUTION.MouseDown += new MouseEventHandler(DISTRIBUTION_MouseDown);
      MULTI_METER.MouseDown += new MouseEventHandler(MULTI_METER_MouseDown);
      VERTICAL_WIRE.MouseDown += new MouseEventHandler(VERTICAL_WIRE_MouseDown);
      HORIZONTAL_WIRE.MouseDown += new MouseEventHandler(HORIZONTAL_WIRE_MouseDown);
      PANEL.AllowDrop = true;
      PANEL.DragEnter += new DragEventHandler(PANEL_DragEnter);
      PANEL.DragDrop += new DragEventHandler(PANEL_DragDrop);
      PANEL.MouseDown += new MouseEventHandler(PANEL_MouseDown);
      PANEL.MouseMove += new MouseEventHandler(PANEL_MouseMove);
      PANEL.MouseUp += new MouseEventHandler(PANEL_MouseUp);
      PANEL.Paint += new PaintEventHandler(PANEL_Paint);
      TRASH.AllowDrop = true;
      TRASH.DragEnter += new DragEventHandler(TRASH_DragEnter);
      TRASH.DragDrop += new DragEventHandler(TRASH_DragDrop);
      TRASH.DragLeave += TRASH_DragLeave;
    }

    private void VERTICAL_WIRE_MouseDown(object sender, MouseEventArgs e)
    {
      if (e.Button == MouseButtons.Left)
      {
        VERTICAL_WIRE.DoDragDrop(VERTICAL_WIRE.Image, DragDropEffects.Copy);
      }
    }

    private void HORIZONTAL_WIRE_MouseDown(object sender, MouseEventArgs e)
    {
      if (e.Button == MouseButtons.Left)
      {
        HORIZONTAL_WIRE.DoDragDrop(HORIZONTAL_WIRE.Image, DragDropEffects.Copy);
      }
    }

    private void UPWARDS_ARROW_MouseDown(object sender, MouseEventArgs e)
    {
      if (e.Button == MouseButtons.Left)
      {
        UPWARDS_ARROW.DoDragDrop(UPWARDS_ARROW.Image, DragDropEffects.Copy);
      }
    }

    private void DOWNWARDS_ARROW_MouseDown(object sender, MouseEventArgs e)
    {
      if (e.Button == MouseButtons.Left)
      {
        DOWNWARDS_ARROW.DoDragDrop(DOWNWARDS_ARROW.Image, DragDropEffects.Copy);
      }
    }

    private void METER_MAIN_MouseDown(object sender, MouseEventArgs e)
    {
      if (e.Button == MouseButtons.Left)
      {
        METER_MAIN.DoDragDrop(METER_MAIN.Image, DragDropEffects.Copy);
      }
    }

    private void MAIN_MouseDown(object sender, MouseEventArgs e)
    {
      if (e.Button == MouseButtons.Left)
      {
        MAIN.DoDragDrop(MAIN.Image, DragDropEffects.Copy);
      }
    }

    private void METER_COMBO_MouseDown(object sender, MouseEventArgs e)
    {
      if (e.Button == MouseButtons.Left)
      {
        METER_COMBO.DoDragDrop(METER_COMBO.Image, DragDropEffects.Copy);
      }
    }

    private void DISTRIBUTION_MouseDown(object sender, MouseEventArgs e)
    {
      if (e.Button == MouseButtons.Left)
      {
        DISTRIBUTION.DoDragDrop(DISTRIBUTION.Image, DragDropEffects.Copy);
      }
    }

    private void MULTI_METER_MouseDown(object sender, MouseEventArgs e)
    {
      if (e.Button == MouseButtons.Left)
      {
        MULTI_METER.DoDragDrop(MULTI_METER.Image, DragDropEffects.Copy);
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
          using (Pen borderPen = new Pen(Color.Blue, 2)) // Red border, 2 pixels wide
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

          PANEL.Invalidate();
          break;
        }
      }
    }

    private void PANEL_MouseMove(object sender, MouseEventArgs e)
    {
      if (currentDraggableObject != null && currentDraggableObject.IsDragging)
      {
        Point clientPoint = new Point(e.X - PANEL.AutoScrollPosition.X, e.Y - PANEL.AutoScrollPosition.Y);

        // Calculate the new position, factoring in the click offset
        int newX = clientPoint.X - currentDraggableObject.ClickOffset.X;
        int newY = clientPoint.Y - currentDraggableObject.ClickOffset.Y;

        // Initialize snapping logic with block size and threshold
        const int snapThreshold = 10; // Pixels for snapping threshold
        const int blockSize = 64; // Size of the blocks
        bool snappedHorizontally = false;

        foreach (var obj in draggableObjects.Where(o => o != currentDraggableObject))
        {
          // Horizontal Snapping (Left and Right edges)
          if (Math.Abs(newX + blockSize - obj.Bounds.Left) < snapThreshold)
          {
            newX = obj.Bounds.Left - blockSize;
            newY = obj.Bounds.Y; // Align tops for perfect fit
            snappedHorizontally = true;
            break; // Assuming one snap target is enough
          }
          else if (Math.Abs(newX - obj.Bounds.Right) < snapThreshold)
          {
            newX = obj.Bounds.Right;
            newY = obj.Bounds.Y; // Align tops for perfect fit
            snappedHorizontally = true;
            break; // Assuming one snap target is enough
          }
        }

        // If not snapped horizontally, consider vertical snapping
        if (!snappedHorizontally)
        {
          foreach (var obj in draggableObjects.Where(o => o != currentDraggableObject))
          {
            // Vertical Snapping (Top and Bottom edges)
            if (Math.Abs(newY + blockSize - obj.Bounds.Top) < snapThreshold)
            {
              newY = obj.Bounds.Top - blockSize;
              newX = obj.Bounds.X; // Align lefts for perfect fit
              break; // Assuming one snap target is enough
            }
            else if (Math.Abs(newY - obj.Bounds.Bottom) < snapThreshold)
            {
              newY = obj.Bounds.Bottom;
              newX = obj.Bounds.X; // Align lefts for perfect fit
              break; // Assuming one snap target is enough
            }
          }
        }

        // Prevent moving beyond panel bounds
        newX = Math.Max(newX, 0);
        newY = Math.Max(newY, 0);

        // Update the draggable object's position
        currentDraggableObject.Bounds = new Rectangle(newX, newY, blockSize, blockSize);

        if (NearTrashCan(e.Location))
        {
          isDraggingForDeletion = true;
          DoDragDrop(currentDraggableObject, DragDropEffects.Move);
        }
        else
        {
          isDraggingForDeletion = false;
          PANEL.Invalidate(); // Repaint to update the position of the dragged image
        }

        UpdateScrollBars();
        PANEL.Invalidate();
      }
    }

    private void PANEL_MouseUp(object sender, MouseEventArgs e)
    {
      if (currentDraggableObject != null)
      {
        if (!isDraggingForDeletion)
        {
          currentDraggableObject.IsDragging = false;
          currentDraggableObject = null;
          UpdateScrollBars();
          PANEL.Invalidate(); // Repaint to finalize the position and remove the highlight
        }
      }
    }

    private void PANEL_DragDrop(object sender, DragEventArgs e)
    {
      if (!e.Data.GetDataPresent(typeof(DraggableObject)) || sender == TRASH)
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
    }

    private void TRASH_DragEnter(object sender, DragEventArgs e)
    {
      // Check if the data being dragged is a DraggableObject
      if (e.Data.GetDataPresent(typeof(DraggableObject)) && isDraggingForDeletion)
      {
        e.Effect = DragDropEffects.Move; // Change the cursor to indicate moving
      }
    }

    private void TRASH_DragLeave(object sender, EventArgs e)
    {
      isDraggingForDeletion = false;
      PANEL.Invalidate();
    }

    private void TRASH_DragDrop(object sender, DragEventArgs e)
    {
      // Check if the data being dropped is a DraggableObject
      if (e.Data.GetDataPresent(typeof(DraggableObject)) && isDraggingForDeletion)
      {
        DraggableObject draggable = (DraggableObject)e.Data.GetData(typeof(DraggableObject));

        // Remove the draggable object from the list
        draggableObjects.Remove(draggable);

        // Redraw the panel to reflect the changes
        PANEL.Invalidate();
      }
    }

    private bool NearTrashCan(Point mouseLocation)
    {
      var relativeX = mouseLocation.X + PANEL.Location.X;
      var relativeY = mouseLocation.Y + PANEL.Location.Y;

      var newLocation = new Point(relativeX, relativeY);

      Console.WriteLine("Mouse Location: " + newLocation);
      Console.WriteLine("Trash Can Contains Mouse: " + TRASH.Bounds.Contains(newLocation));

      return TRASH.Bounds.Contains(newLocation);
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