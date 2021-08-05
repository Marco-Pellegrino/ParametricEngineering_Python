using System.Windows.Forms;
using Rhino.Geometry;
using Rhino.UI;

namespace GhMouseTracker
{
    public class MouseTrackerCallback : MouseCallback
    {
        private GhcMouseTracker ghcMouseTracker = null;


        public MouseTrackerCallback(GhcMouseTracker ghcMouseTracker)
        {
            Enabled = true;
            this.ghcMouseTracker = ghcMouseTracker;
        }


        protected override void OnMouseDown(MouseCallbackEventArgs e)
        {
            if (ghcMouseTracker.RequireAltKey && Control.ModifierKeys != Keys.Alt) return;
            if (e.Button != MouseButtons.Left || ghcMouseTracker.Locked) return;

            ghcMouseTracker.LeftMousePressed = true;

            ghcMouseTracker.MouseLine = GetMouseLine(e);
            ghcMouseTracker.MousePixels = new Point3d(e.ViewportPoint.X, e.ViewportPoint.Y, 0.0);
            ghcMouseTracker.MouseFraction = new Point3d((double)e.ViewportPoint.X / e.View.ActiveViewport.Size.Width, (double)e.ViewportPoint.Y / e.View.ActiveViewport.Size.Width, 0.0);

            Enabled = false;// Disable the current MouseTrackerCallback object, as a new one will be created by the mouse-tracker component every time it runs
            e.Cancel = true; // This prevents Rhino from unwantedly going into drag-and-select mode when the left mouse is being pressed while inside the viewport
            ghcMouseTracker.ExpireSolution(true);
        }


        protected override void OnMouseUp(MouseCallbackEventArgs e)
        {
            if (ghcMouseTracker.RequireAltKey && Control.ModifierKeys != Keys.Alt) return;
            if (e.Button != MouseButtons.Left || ghcMouseTracker.Locked) return;

            ghcMouseTracker.LeftMousePressed = false;
            ghcMouseTracker.JustReleased = true;

            Enabled = false;// Disable the current MouseTrackerCallback object, as a new one will be created by the mouse-tracker component every time it runs
            e.Cancel = true; // This prevents Rhino from unwantedly going into drag-and-select mode when the left mouse is being pressed while inside the viewport
            ghcMouseTracker.ExpireSolution(true);
        }


        protected override void OnMouseMove(MouseCallbackEventArgs e)
        {
            if (ghcMouseTracker.RequireAltKey && Control.ModifierKeys != Keys.Alt) return;
            if (ghcMouseTracker.Locked) return;

            if (ghcMouseTracker.LeftMousePressed)
            {
                ghcMouseTracker.MouseLine = GetMouseLine(e);
                ghcMouseTracker.MousePixels = new Point3d(e.ViewportPoint.X, e.ViewportPoint.Y, 0.0);
                ghcMouseTracker.MouseFraction = new Point3d((double)e.ViewportPoint.X / e.View.ActiveViewport.Size.Width, (double)e.ViewportPoint.Y / e.View.ActiveViewport.Size.Width, 0.0);
            }

            Enabled = false;// Disable the current MouseTrackerCallback object, as a new one will be created by the mouse-tracker component every time it runs
            e.Cancel = true; // This prevents Rhino from unwantedly going into drag-and-select mode when the left mouse is being pressed while inside the viewport
            ghcMouseTracker.ExpireSolution(true);
        }


        protected override void OnMouseLeave(MouseCallbackEventArgs e)
        {
            ghcMouseTracker.LeftMousePressed = false;
        }


        private Line GetMouseLine(MouseCallbackEventArgs e)
        {
            if (!e.View.ActiveViewport.IsParallelProjection)
            {
                Line line = e.View.ActiveViewport.ClientToWorld(e.ViewportPoint);
                return new Line(line.To, line.From);
            }

            Point3d P = e.View.ActiveViewport.ClientToWorld(e.ViewportPoint).From;
            Vector3d v = e.View.ActiveViewport.ClientToWorld(e.ViewportPoint).From - e.View.ActiveViewport.CameraLocation;

            Vector3d camDir = e.View.ActiveViewport.CameraDirection;
            camDir.Unitize();

            Point3d start = P - camDir * (camDir * v);
            Point3d end = start + camDir * e.View.ActiveViewport.CameraLocation.DistanceTo(e.View.ActiveViewport.CameraTarget);

            return new Line(start, end);
        }
    }
}