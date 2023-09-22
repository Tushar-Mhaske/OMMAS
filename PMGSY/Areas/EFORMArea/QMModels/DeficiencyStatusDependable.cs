using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Areas.EFORMArea.QMModels
{
    public class DeficiencyStatusDependable : Attribute
    {
        public string DeficiancyStatus { get; set; }
    }

    public class TableStatusDependable : Attribute
    {

        public bool TableStatus { get; set; }

    }
    public class DelayStatusDependable : Attribute
    {

        public bool DelayStatus { get; set; }

    }

    public class IsDateExtendedDependable : Attribute
    {

        public bool IsDateExtendStatus { get; set; }

    }
    public class InProgressDependable : Attribute
    {

        public bool InProgressTableStatus { get; set; }

    }
}