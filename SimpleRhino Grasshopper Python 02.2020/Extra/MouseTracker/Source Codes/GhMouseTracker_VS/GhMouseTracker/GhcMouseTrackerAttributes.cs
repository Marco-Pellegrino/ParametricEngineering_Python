using System;
using System.Drawing;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Attributes;


namespace GhMouseTracker
{
    public class GhcMouseTrackerAttributes : GH_ComponentAttributes
    {
        public GhcMouseTrackerAttributes(GhcMouseTracker owner)
          : base(owner)
        {
           
        }


        protected override void Layout()
        {
            base.Layout();
        }


        protected override void Render(GH_Canvas canvas, Graphics graphics, GH_CanvasChannel channel)
        {
            switch (channel)
            {
                case GH_CanvasChannel.Wires:

                    foreach (IGH_Param param in Owner.Params.Input)
                        param.Attributes.RenderToCanvas(canvas, GH_CanvasChannel.Wires);
                    break;

                case GH_CanvasChannel.Objects:

                    GH_Palette palette = GH_CapsuleRenderEngine.GetImpliedPalette(Owner);
                    GH_Capsule capsule = GH_Capsule.CreateCapsule(Bounds, palette);

                    foreach (IGH_Param param in Owner.Params.Input)
                        capsule.AddInputGrip(Bounds.Bottom - 20 ); //param.Attributes.InputGrip.Y);

                    capsule.Render(graphics, GH_CapsuleRenderEngine.GetImpliedStyle(palette, Selected, Owner.Locked, Owner.Hidden));
                    break;
            }
        }   
    }
}