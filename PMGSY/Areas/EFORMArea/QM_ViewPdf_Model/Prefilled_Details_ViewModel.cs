using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Areas.EFORMArea.QM_ViewPdf_Model
{
    public class Prefilled_Details_ViewModel
    {        
        public int EFORM_ID { get; set; }
        public string STATE { get; set; }
        public string DISTRICT { get; set; }
        public string BLOCK { get; set; }
        public string ROAD_NAME { get; set; }
        public string QM_NAME { get; set; }
        public string PACKAGE_ID { get; set; }
        public string QM_TYPE { get; set; }
        public string PHYSICAL_WORK_STATUS { get; set; }
        public string CURRENT_STAGE { get; set; }
        
    }
}