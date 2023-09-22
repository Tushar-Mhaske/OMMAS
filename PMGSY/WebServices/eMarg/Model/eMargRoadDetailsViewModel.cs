using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.WebServices.eMarg.Model
{
    public class eMargRoadDetailsViewModel
    {
        public int state { get; set; }
        public int district { get; set; }
        public string packageNo { get; set; }
        public string contractor { get; set; }
        public string pan { get; set; }
        public string agreementNo { get; set; }
        public string agreementDate { get; set; }
        public long roadCode { get; set; }
        public string roadName { get; set; }
        public string completionDate { get; set; }
        public string sanctionedLength { get; set; }
        public string carriageWidth { get; set; }
        public string trafficDensity { get; set; }
        public string remarks { get; set; }
    }
}