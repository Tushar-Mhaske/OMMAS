using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.WebServices.eMarg.Model
{
    public class RoadDetails
    {

        public Int32 EID { get; set; }
        public string state { get; set; }
        public string district { get; set; }

        public string packageNo { get; set; }

        public string contractor { get; set; }

        public string pan { get; set; }
        public string agreementNo { get; set; }
        public string agreementDate { get; set; }
        public int roadCode { get; set; }
        public string roadName { get; set; }

        public string completionDate { get; set; }
        public string sanctionedLength { get; set; }
        public string completedLength { get; set; }

        public string carriageWidth { get; set; }

        public string trafficDensity { get; set; }

        public string remarks { get; set; }







    }
}