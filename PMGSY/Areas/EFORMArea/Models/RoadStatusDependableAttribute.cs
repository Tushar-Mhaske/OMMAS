using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Areas.EFORMArea.Model 
{
    public class RoadStatusDependableAttribute:Attribute
    {
        public bool RoadIsCompleted { get; set; }
    }

    
}   