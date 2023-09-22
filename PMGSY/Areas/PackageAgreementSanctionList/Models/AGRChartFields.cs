using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Areas.PackageAgreementSanctionList.Models
{
    public class AGRChartFields
    {

      //  public string Year { get; set; }

        public string SanctionWorks { get; set; }
        public string AwardedWorks { get; set; }
        public string UnawardedWorks { get; set; }
        public string TerminatedWorks { get; set; }


        public string CompletedWorks { get; set; }
        public string ProgressWorks { get; set; }


        public string LessThanYear_1 { get; set; }
        public string Year_1_To_2 { get; set; }
        public string Year_2_To_3 { get; set; }
        public string GreaterThanYear_3 { get; set; }

    }
}