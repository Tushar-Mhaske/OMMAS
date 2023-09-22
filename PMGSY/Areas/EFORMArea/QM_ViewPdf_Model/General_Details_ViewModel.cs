using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Areas.EFORMArea.QM_ViewPdf_Model
{
    public class General_Details_ViewModel
    {
        public int EFORM_ID { get; set; }        
        public decimal ROAD_FROM { get; set; }
        public decimal ROAD_TO { get; set; }
        public string INSPECTION_DATE { get; set; }
        public string Template_Version { get; set; }
        
    }
}