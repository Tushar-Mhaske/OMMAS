using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Areas.EFORMArea.QMModels
{
    public class IsNewTechGSDependableGSAttribute:Attribute
    {
        public bool NewTechused { get; set; }
    }

    public class IsNewTechBL1DependableGSAttribute : Attribute
    {
        public bool NewTechused { get; set; }
    }

    public class IsNewTechBL2DependableGSAttribute : Attribute
    {
        public bool NewTechused { get; set; }
    }

    public class IsNewTechBL3DependableGSAttribute : Attribute
    {
        public bool NewTechused { get; set; }
    }





    public class IsNewTechUsed19ValAttribute : Attribute
    {
        public bool NewTechused { get; set; }
    }

    public class IsNewTechQtyUsed20ValAttribute : Attribute
    {
        public bool NewTechused { get; set; }
    }

    public class IsNewTechUsed22ValAttribute : Attribute
    {
        public bool NewTechused { get; set; }
    }




    public class IsNewTechUsedNTValAttribute : Attribute
    {
        public bool NewTechused { get; set; }
    }

}