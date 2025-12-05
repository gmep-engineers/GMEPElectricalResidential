using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.GraphicsInterface;
using Autodesk.AutoCAD.Runtime;
using Ellipse = Autodesk.AutoCAD.DatabaseServices.Ellipse;
using Line = Autodesk.AutoCAD.DatabaseServices.Line;
using Polyline = Autodesk.AutoCAD.DatabaseServices.Polyline;

namespace GMEPElectricalResidential
{
  public class LineJig : DrawJig
  {
    private Point3d startPoint;
    private Point3d endPoint;
    public Line line;
    public string message;

    public LineJig(Point3d startPt, string layer, string _message = "\nSelect end point:")
    {
      startPoint = startPt;
      endPoint = startPt;
      line = new Line(startPoint, startPoint);
      line.Layer = layer;
      message = _message;
    }

    protected override bool WorldDraw(WorldDraw draw)
    {
      try
      {
        if (line != null)
        {
          draw.Geometry.Draw(line);
        }
        return true;
      }
      catch
      {
        return false;
      }
    }

    protected override SamplerStatus Sampler(JigPrompts prompts)
    {
      try
      {
        // Validate prompts parameter
        if (prompts == null)
        {
          return SamplerStatus.Cancel;
        }

        // Validate line object is in a valid state
        if (line == null || line.IsDisposed)
        {
          return SamplerStatus.Cancel;
        }

        JigPromptPointOptions opts = new JigPromptPointOptions(message);
        opts.BasePoint = startPoint;
        opts.UseBasePoint = true;
        opts.Cursor = CursorType.RubberBand;

        PromptPointResult res = prompts.AcquirePoint(opts);
        if (res == null || res.Status != PromptStatus.OK)
          return SamplerStatus.Cancel;
        if (res.Value == null)
          return SamplerStatus.Cancel;
        if (endPoint.DistanceTo(res.Value) < Tolerance.Global.EqualPoint)
          return SamplerStatus.NoChange;

        endPoint = new Point3d(res.Value.X, res.Value.Y, startPoint.Z);
        line.EndPoint = endPoint;

        return SamplerStatus.OK;
      }
      catch
      {
        return SamplerStatus.Cancel;
      }
    }
  }

  public class LineStartPointPreviewJig : DrawJig
  {
    private Line _baseLine;
    private Point3d _mousePoint;
    public Point3d ProjectedPoint { get; private set; }

    public LineStartPointPreviewJig(Line baseLine)
    {
      _baseLine = baseLine;
      _mousePoint = baseLine.StartPoint;
      ProjectedPoint = baseLine.StartPoint;
    }

    protected override bool WorldDraw(WorldDraw draw)
    {
      try
      {
        Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
        ViewTableRecord view = ed.GetCurrentView();

        double unitsPerPixel = view.Width / 6000;
        double markerRadius = unitsPerPixel * 10;

        // Draw an "X" at the projected point
        Point3d p1 = ProjectedPoint + new Vector3d(-markerRadius, -markerRadius, 0);
        Point3d p2 = ProjectedPoint + new Vector3d(markerRadius, markerRadius, 0);
        Point3d p3 = ProjectedPoint + new Vector3d(-markerRadius, markerRadius, 0);
        Point3d p4 = ProjectedPoint + new Vector3d(markerRadius, -markerRadius, 0);

        Line line1 = new Line(p1, p2);
        Line line2 = new Line(p3, p4);

        draw.Geometry.Draw(line1);
        draw.Geometry.Draw(line2);

        line1.Dispose();
        line2.Dispose();

        return true;
      }
      catch
      {
        return false;
      }
    }

    protected override SamplerStatus Sampler(JigPrompts prompts)
    {
      try
      {
        // Validate prompts parameter
        if (prompts == null)
        {
          return SamplerStatus.Cancel;
        }

        // Validate base line object is in a valid state
        if (_baseLine == null || _baseLine.IsDisposed)
        {
          return SamplerStatus.Cancel;
        }

        JigPromptPointOptions opts = new JigPromptPointOptions(
          "\nMove cursor to preview start point, click to select:"
        );

        opts.UserInputControls =
          UserInputControls.Accept3dCoordinates | UserInputControls.NullResponseAccepted;
        PromptPointResult res = prompts.AcquirePoint(opts);

        if (res == null || res.Status != PromptStatus.OK)
          return SamplerStatus.Cancel;
        if (res.Value == null)
          return SamplerStatus.Cancel;

        if (_mousePoint.DistanceTo(res.Value) < Tolerance.Global.EqualPoint)
          return SamplerStatus.NoChange;

        _mousePoint = res.Value;
        ProjectedPoint = ProjectPointToLineSegment(
          _baseLine.StartPoint,
          _baseLine.EndPoint,
          _mousePoint
        );
        return SamplerStatus.OK;
      }
      catch
      {
        return SamplerStatus.Cancel;
      }
    }

    public static Point3d ProjectPointToLineSegment(Point3d a, Point3d b, Point3d p)
    {
      Vector3d ab = b - a;
      Vector3d ap = p - a;
      double t = ab.DotProduct(ap) / ab.LengthSqrd;
      t = Math.Max(0, Math.Min(1, t)); // Clamp to segment
      return a + ab * t;
    }
  }

  public class LineBlockJig : DrawJig
  {
    private readonly Line _baseLine;
    private readonly ObjectId _blockDefId;
    private Point3d _mousePoint;
    public Point3d InsertionPoint { get; private set; }
    private double _blockScale;
    private double _blockRotation;
    private double _maxDistance;
    public double rotation;

    public LineBlockJig(
      Line baseLine,
      ObjectId blockDefId,
      double blockScale = 1,
      double blockRotation = 0,
      double maxDistance = 0
    )
    {
      _baseLine = baseLine;
      _blockDefId = blockDefId;
      _mousePoint = baseLine.StartPoint;
      InsertionPoint = baseLine.StartPoint;
      _blockScale = blockScale;
      _blockRotation = blockRotation;
      _maxDistance = maxDistance;
    }

    protected override bool WorldDraw(WorldDraw draw)
    {
      // Preview the block at the projected point

      rotation = _blockRotation;
      bool rotationApplied = false;
      if (_mousePoint.X > InsertionPoint.X)
      {
        rotation = rotation + 3.14159265359;
        rotationApplied = true;
      }
      else if (_mousePoint.Y < InsertionPoint.Y)
      {
        rotation = rotation + 3.14159265359;
        rotationApplied = true;
      }
      bool increasingDirection = false;
      if (_baseLine.StartPoint.X < _baseLine.EndPoint.X)
      {
        increasingDirection = true;
      }
      if (_baseLine.StartPoint.Y < _baseLine.EndPoint.Y)
      {
        increasingDirection = true;
      }

      bool angleFlip = true;
      if (
        _baseLine.StartPoint.X != _baseLine.EndPoint.X
        && _baseLine.StartPoint.Y != _baseLine.EndPoint.Y
      )
      {
        if (
          _baseLine.EndPoint.X > _baseLine.StartPoint.X
          && _baseLine.EndPoint.Y > _baseLine.StartPoint.Y
        )
        {
          angleFlip = false;
        }
        if (
          _baseLine.EndPoint.X < _baseLine.StartPoint.X
          && _baseLine.EndPoint.Y < _baseLine.StartPoint.Y
        )
        {
          angleFlip = false;
        }
      }
      else
      {
        angleFlip = false;
      }

      if (_maxDistance > 0 && _mousePoint.DistanceTo(_baseLine.StartPoint) > _maxDistance)
      {
        double x =
          _baseLine.StartPoint.X
          + _maxDistance
            * Math.Cos(rotation)
            * (rotationApplied ? 1 : -1)
            * (increasingDirection ? -1 : 1)
            * (angleFlip ? -1 : 1);
        double y =
          _baseLine.StartPoint.Y
          + _maxDistance
            * Math.Sin(rotation)
            * (rotationApplied ? 1 : -1)
            * (increasingDirection ? -1 : 1)
            * (angleFlip ? -1 : 1);
        InsertionPoint = new Point3d(x, y, 0);
      }

      if (angleFlip)
      {
        rotation = _blockRotation;
        if (_mousePoint.X < InsertionPoint.X)
        {
          rotation = rotation + 3.14159265359;
        }
        else if (_mousePoint.Y < InsertionPoint.Y)
        {
          rotation = rotation + 3.14159265359;
        }
      }
      BlockReference previewBlock = new BlockReference(InsertionPoint, _blockDefId)
      {
        ScaleFactors = new Scale3d(_blockScale),
        Rotation = rotation,
      };
      draw.Geometry.Draw(previewBlock);
      previewBlock.Dispose();
      return true;
    }

    protected override SamplerStatus Sampler(JigPrompts prompts)
    {
      // Validate prompts parameter
      if (prompts == null)
      {
        return SamplerStatus.Cancel;
      }

      // Validate base line and block definition are in a valid state
      if (
        _baseLine == null
        || _baseLine.IsDisposed
        || _blockDefId == ObjectId.Null
        || !_blockDefId.IsValid
      )
      {
        return SamplerStatus.Cancel;
      }

      JigPromptPointOptions opts = new JigPromptPointOptions("\nPlace Line Block:");
      opts.UserInputControls =
        UserInputControls.Accept3dCoordinates | UserInputControls.NullResponseAccepted;
      PromptPointResult res = prompts.AcquirePoint(opts);

      if (res.Status != PromptStatus.OK)
        return SamplerStatus.Cancel;

      if (_mousePoint.DistanceTo(res.Value) < Tolerance.Global.EqualPoint)
        return SamplerStatus.NoChange;

      _mousePoint = res.Value;
      InsertionPoint = LineStartPointPreviewJig.ProjectPointToLineSegment(
        _baseLine.StartPoint,
        _baseLine.EndPoint,
        _mousePoint
      );
      return SamplerStatus.OK;
    }
  }
}
