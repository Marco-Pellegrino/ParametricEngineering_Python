using System;
using System.Drawing;
using Grasshopper.Kernel;

namespace GhMouseTracker
{
    public class GhMouseTrackerInfo : GH_AssemblyInfo
    {
        public override string Name
        {
            get
            {
                return "Mouse Tracker";
            }
        }
        public override Bitmap Icon
        {
            get
            {
            
                return Properties.Resources.Icon_GhcMouseTracker;
            }
        }
        public override string Description
        {
            get
            {             
                return "Track the mouse position in the screen space and world space";
            }
        }
        public override Guid Id
        {
            get
            {
                return new Guid("278b026f-b541-4d63-9664-547283726fae");
            }
        }

        public override string AuthorName
        {
            get
            {
                return "Long Nguyen";
            }
        }
        public override string AuthorContact
        {
            get
            {               
                return "Long Nguyen\nInstitute for Computational Design (ICD)\nUniversity of Stuttgart\nlong.nguyen@icd.uni-stuttgart.de";
            }
        }
    }
}
