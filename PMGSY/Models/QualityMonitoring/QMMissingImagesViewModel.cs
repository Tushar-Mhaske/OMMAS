using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Models.QualityMonitoring
{
    public class QMMissingImagesViewModel
    {
        public int QMSchCode { get; set; }
        public int PrRoadCode { get; set; }
        public int QMObsId { get; set; }
        public string qmFileName { get; set; }
    }
}