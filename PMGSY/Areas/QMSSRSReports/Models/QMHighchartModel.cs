using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Areas.QMSSRSReports.Models
{
    public class QMHighchartModel
    {

        public string Quarter { get; set; }

        public decimal TotalInspection { get; set; }

        public decimal TotalATRRequired_RI_U { get; set; }

        public decimal TotalATRSubmitted_RI_U { get; set; }

        public decimal TotalATRRequired_U { get; set; }

        public decimal TotalATRSubmitted_U { get; set; }
    }
}


