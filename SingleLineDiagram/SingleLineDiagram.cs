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

namespace GMEPElectricalResidential
{
  public partial class SINGLE_LINE_DIAGRAM : Form
  {
    private List<DraggableObject> draggableObjects = new List<DraggableObject>();
    private DraggableObject currentDraggableObject;
    private bool isDraggingForDeletion = false;
    private int currentID;
    private Point location;

    public SINGLE_LINE_DIAGRAM()
    {
      InitializeComponent();
      currentID = 0;
      location = USERCONTROL_PLACEHOLDER.Location;
      UPWARDS_ARROW.MouseDown += new MouseEventHandler(UPWARDS_ARROW_MouseDown);
      DOWNWARDS_ARROW.MouseDown += new MouseEventHandler(DOWNWARDS_ARROW_MouseDown);
      METER_MAIN.MouseDown += new MouseEventHandler(METER_MAIN_MouseDown);
      MAIN.MouseDown += new MouseEventHandler(MAIN_MouseDown);
      METER_COMBO.MouseDown += new MouseEventHandler(METER_COMBO_MouseDown);
      DISTRIBUTION.MouseDown += new MouseEventHandler(DISTRIBUTION_MouseDown);
      MULTI_METER.MouseDown += new MouseEventHandler(MULTI_METER_MouseDown);
      PANEL.AllowDrop = true;
      PANEL.DragEnter += new DragEventHandler(PANEL_DragEnter);
      PANEL.DragDrop += new DragEventHandler(PANEL_DragDrop);
      PANEL.MouseDown += new MouseEventHandler(PANEL_MouseDown);
      PANEL.MouseMove += new MouseEventHandler(PANEL_MouseMove);
      PANEL.MouseUp += new MouseEventHandler(PANEL_MouseUp);
      PANEL.Paint += new PaintEventHandler(PANEL_Paint);
      PANEL.DoubleClick += new EventHandler(PANEL_DoubleClick);
      TRASH.AllowDrop = true;
      TRASH.DragEnter += new DragEventHandler(TRASH_DragEnter);
      TRASH.DragDrop += new DragEventHandler(TRASH_DragDrop);
      TRASH.DragLeave += TRASH_DragLeave;
    }

    private void PANEL_DoubleClick(object sender, EventArgs e)
    {
      if (currentDraggableObject != null)
      {
        Controls.Remove(USERCONTROL_PLACEHOLDER);
        RemoveAllControls();

        Controls.Add(currentDraggableObject.Form as System.Windows.Forms.Control);
        BLOCK_INFORMATION.Text = currentDraggableObject.Message;
      }
    }

    private void RemoveAllControls()
    {
      draggableObjects.ForEach(draggableObject =>
      {
        Controls.Remove(draggableObject.Form as System.Windows.Forms.Control);
      });
    }

    private void DOWNWARDS_ARROW_MouseDown(object sender, MouseEventArgs e)
    {
      if (e.Button == MouseButtons.Left)
      {
        var data = new DraggableObject()
        {
          Image = DOWNWARDS_ARROW.Image,
          Bounds = new Rectangle(0, 0, 64, 64),
          ID = currentID,
          Name = "DOWNWARDS_ARROW",
          Form = new PullSectionBelowForm(location),
          Message = "Pull Section Below Block"
        };
        DOWNWARDS_ARROW.DoDragDrop(data, DragDropEffects.Copy);
      }
    }

    private void UPWARDS_ARROW_MouseDown(object sender, MouseEventArgs e)
    {
      if (e.Button == MouseButtons.Left)
      {
        var data = new DraggableObject()
        {
          Image = UPWARDS_ARROW.Image,
          Bounds = new Rectangle(0, 0, 64, 64),
          ID = currentID,
          Name = "UPWARDS_ARROW",
          Form = new PullSectionAboveForm(location),
          Message = "Pull Section Above Block"
        };
        UPWARDS_ARROW.DoDragDrop(data, DragDropEffects.Copy);
      }
    }

    private void METER_MAIN_MouseDown(object sender, MouseEventArgs e)
    {
      if (e.Button == MouseButtons.Left)
      {
        var data = new DraggableObject()
        {
          Image = METER_MAIN.Image,
          Bounds = new Rectangle(0, 0, 64, 64),
          ID = currentID,
          Name = "METER_MAIN",
          Form = new MeterAndMainForm(location),
          Message = "Meter Main Block"
        };
        METER_MAIN.DoDragDrop(data, DragDropEffects.Copy);
      }
    }

    private void MAIN_MouseDown(object sender, MouseEventArgs e)
    {
      if (e.Button == MouseButtons.Left)
      {
        var data = new DraggableObject()
        {
          Image = MAIN.Image,
          Bounds = new Rectangle(0, 0, 64, 64),
          ID = currentID,
          Name = "MAIN",
          Form = new MainForm(location),
          Message = "Main Panel Block"
        };
        MAIN.DoDragDrop(data, DragDropEffects.Copy);
      }
    }

    private void METER_COMBO_MouseDown(object sender, MouseEventArgs e)
    {
      if (e.Button == MouseButtons.Left)
      {
        var data = new DraggableObject()
        {
          Image = METER_COMBO.Image,
          Bounds = new Rectangle(0, 0, 64, 64),
          ID = currentID,
          Name = "METER_COMBO",
          Form = new MeterComboForm(location),
          Message = "Meter Combo Block"
        };
        METER_COMBO.DoDragDrop(data, DragDropEffects.Copy);
      }
    }

    private void DISTRIBUTION_MouseDown(object sender, MouseEventArgs e)
    {
      if (e.Button == MouseButtons.Left)
      {
        var data = new DraggableObject()
        {
          Image = DISTRIBUTION.Image,
          Bounds = new Rectangle(0, 0, 64, 64),
          ID = currentID,
          Name = "DISTRIBUTION",
          Form = new DistributionForm(location),
          Message = "Distribution Block"
        };
        DISTRIBUTION.DoDragDrop(data, DragDropEffects.Copy);
      }
    }

    private void MULTI_METER_MouseDown(object sender, MouseEventArgs e)
    {
      if (e.Button == MouseButtons.Left)
      {
        var data = new DraggableObject()
        {
          Image = MULTI_METER.Image,
          Bounds = new Rectangle(0, 0, 64, 64),
          ID = currentID,
          Name = "MULTI_METER"
        };
        MULTI_METER.DoDragDrop(data, DragDropEffects.Copy);
      }
    }

    private void PANEL_DragEnter(object sender, DragEventArgs e)
    {
      if (e.Data.GetDataPresent(typeof(DraggableObject)))
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

        // Find the closest draggable object
        var closestObject = draggableObjects
            .Where(o => o != currentDraggableObject)
            .OrderBy(o => Math.Sqrt(Math.Pow(o.Bounds.X - currentDraggableObject.Bounds.X, 2) + Math.Pow(o.Bounds.Y - currentDraggableObject.Bounds.Y, 2)))
            .FirstOrDefault();

        if (closestObject != null)
        {
          // Horizontal Snapping (Left and Right edges)
          if (Math.Abs(newX + blockSize - closestObject.Bounds.Left) < snapThreshold)
          {
            newX = closestObject.Bounds.Left - blockSize;
            newY = closestObject.Bounds.Y; // Align tops for perfect fit
            snappedHorizontally = true;
          }
          else if (Math.Abs(newX - closestObject.Bounds.Right) < snapThreshold)
          {
            newX = closestObject.Bounds.Right;
            newY = closestObject.Bounds.Y; // Align tops for perfect fit
            snappedHorizontally = true;
          }

          // If not snapped horizontally, consider vertical snapping
          if (!snappedHorizontally)
          {
            // Vertical Snapping (Top and Bottom edges)
            if (Math.Abs(newY + blockSize - closestObject.Bounds.Top) < snapThreshold)
            {
              newY = closestObject.Bounds.Top - blockSize;
              newX = closestObject.Bounds.X; // Align lefts for perfect fit
            }
            else if (Math.Abs(newY - closestObject.Bounds.Bottom) < snapThreshold)
            {
              newY = closestObject.Bounds.Bottom;
              newX = closestObject.Bounds.X; // Align lefts for perfect fit
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
      if (e.Data.GetDataPresent(typeof(DraggableObject)))
      {
        Point dropPoint = PANEL.PointToClient(new Point(e.X, e.Y));

        var draggableObject = e.Data.GetData(typeof(DraggableObject)) as DraggableObject;

        if (draggableObject != null)
        {
          Image droppedImage = draggableObject.Image;
          draggableObject.Bounds = new Rectangle(dropPoint, droppedImage.Size);
          currentID++;
          draggableObjects.Add(draggableObject);
          UpdateScrollBars();
          PANEL.Invalidate();
        }
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

        RemoveAllControls();
        Controls.Add(USERCONTROL_PLACEHOLDER);
        BLOCK_INFORMATION.Text = "Double click a placed block to view its information";

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

    private void SaveEventData(object sender, EventArgs e)
    {
      HelperMethods.SaveDataToJsonFile(currentDraggableObject, "currentDraggableObject.json");
      HelperMethods.SaveDataToJsonFile(sender, "sender.json");
      HelperMethods.SaveDataToJsonFile(e, "e.json");
    }
  }

  public class DraggableObject
  {
    public Image Image { get; set; }
    public Rectangle Bounds { get; set; }
    public bool IsDragging { get; set; }
    public Point ClickOffset { get; set; }
    public int ID { get; set; }
    public string Name { get; set; }
    public object Form { get; set; }
    public string Message { get; set; }
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