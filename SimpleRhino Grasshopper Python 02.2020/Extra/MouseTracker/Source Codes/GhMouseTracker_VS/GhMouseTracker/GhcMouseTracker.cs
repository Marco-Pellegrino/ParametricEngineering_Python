using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Grasshopper.Kernel;
using Rhino.Geometry;


namespace GhMouseTracker
{
    // HOW THIS COMPONENT IS USED
    // When this component is turned on, the mouse position will be tracked and output as long as the left mouse is pressed down ...
    // ... The tracking will pause as soon as the left mouse is released. The output will remain the same as the last tracked position.

    // HOW IT WORKS BEHIND THE SCENE
    // Every time this component runs, it creates a MouseTrackerCallback object (class definition included) that respond to mouse-move, mouse-pressed and mouse-released events ...
    // ... then this object is disabled immediately and the component is automatically triggered to run again, which will create a brand new MouseCallback object
    // ... The reason why it is necessary to create a new MouseTrackerCallback object every ime is because ...
    // ... this strategy allows to prevent Rhino from going to drag-and-select mode when the left mouse is being pressed while inside the viewport.

    public class GhcMouseTracker : GH_Component
    {
        public Line MouseLine = Line.Unset;
        public Point3d MousePixels = Point3d.Unset;
        public Point3d MouseFraction = Point3d.Unset;

        public bool LeftMousePressed = false;
        public bool JustReleased = false;
        public bool RequireAltKey = true;

        public MouseTrackerCallback Callback = null;

        protected override System.Drawing.Bitmap Icon => Properties.Resources.Icon_GhcMouseTracker;
        public override Guid ComponentGuid => new Guid("{5ef6217e-c35e-4ad2-a3d6-a9a9807d9176}");
        public override GH_Exposure Exposure => GH_Exposure.quinary;


        public GhcMouseTracker()
          : base(
            "Mouse Tracker",
            "Mouse",
            "This component continuously output the current mouse position in the viewport as long as the left mouse button is pressed." +
                    "Note: when mouse tracking is enabled, you will no longer be able to select objects in the viewport by clicking on them. To select objects, you need to first deactivate the mouse tracking by setting the \"On\" input parameter to False." +
                    "In the right-click menu there is option called \"Require Alt Key\". When this is enabled, you will also need to addtionally hold down the Alt key for the mouse tracking to work. This allows you to quickly switch between tracking/non-tracking mode with the Alt key.",
            "Params",
            "Input")
        {
        }


        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("On", "On", "Turn on/off mouse tracking", GH_ParamAccess.item, true);
        }


        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddLineParameter("Mouse Line", "L", "The line from the camera to the mouse position", GH_ParamAccess.item);
            pManager.AddPointParameter("Mouse Position (pixels)", "Sp", "Position of the mouse in the 2D viewport space, measured in pixels from the viewport's top left corner", GH_ParamAccess.item);
            pManager.AddPointParameter("Mouse Position (fraction)", "Sf", "Position of the mouse in 2D viewport space, measured as a fraction of the viewport's width and height", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Left Mouse Pressed", "B", "True if the left mouse butting is being pressed while the mouse cursor is in the viewport", GH_ParamAccess.item);
        }


        protected override void SolveInstance(IGH_DataAccess DA)
        {
            bool iOn = true;
            DA.GetData("On", ref iOn);

            if (iOn)
                Callback = new MouseTrackerCallback(this);
            else
            {
                if (Callback != null)
                {
                    Callback.Enabled = false;
                    LeftMousePressed = false;
                }
            }

            DA.SetData("Mouse Line", MouseLine);
            DA.SetData("Mouse Position (pixels)", MousePixels);
            DA.SetData("Mouse Position (fraction)", MouseFraction);
            DA.SetData("Left Mouse Pressed", LeftMousePressed);
        }


        protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu)
        {
            Menu_AppendItem(
                menu,
                "Require Alt key",
                (s, e) => { RequireAltKey = !RequireAltKey; },
                true,
                RequireAltKey);
        }


        protected override void ExpireDownStreamObjects()
        {
            // Only when the left mouse is pressed that the output data of this component actually change ...
            // ... Hence we only expire downstream objects when the left mouse is pressed. This can speed up heavy solution when mouse moves in the viewport while the left mouse is NOT pressed
            if (LeftMousePressed || JustReleased) base.ExpireDownStreamObjects();
            JustReleased = false;
        }


        //=============================================================================================================
        // This method ensures that no more than one mouse-tracker component can be added to the document
        //=============================================================================================================
        public override void AddedToDocument(GH_Document document)
        {
            base.AddedToDocument(document);

            List<IGH_ActiveObject> activeObjects = document.ActiveObjects();

            foreach (IGH_ActiveObject activeObject in activeObjects)
                if (activeObject != this && activeObject is GhcMouseTracker)
                {
                    MessageBox.Show("There is already a mouse-tracker component on the canvas. It is not possible to have more than one component of this type on the same canvas.");
                    document.RemoveObject(this, false);
                }
        }


        //=============================================================================================================
        // This method ensures that no more than one mouse-tracker component can be added to the document (via copying)
        //=============================================================================================================
        public override void MovedBetweenDocuments(GH_Document oldDocument, GH_Document newDocument)
        {
            base.MovedBetweenDocuments(oldDocument, newDocument);

            base.AddedToDocument(newDocument);

            List<IGH_ActiveObject> activeObjects = newDocument.ActiveObjects();

            foreach (IGH_ActiveObject activeObject in activeObjects)
                if (activeObject != this && activeObject is GhcMouseTracker)
                {
                    MessageBox.Show("There is already a mouse-tracker component on the canvas. It is not possible to have more than one component of this type on the same canvas.");
                    newDocument.RemoveObject(this, false);
                }
        }


//        public override bool Write(GH_IWriter writer)
//        {
//            writer.SetBoolean("RequireAltKey", RequireAltKey);
//            return true;
//        }
//
//
//        public override bool Read(GH_IReader reader)
//        {
//            RequireAltKey = reader.GetBoolean("RequireAltKey");
//            return true;
//        }
    }
}
