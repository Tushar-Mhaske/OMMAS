using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Areas.EFORMArea.QM_ViewPdf_Model
{
    public class Present_Work_Details_ViewModel
    {
        public int EFORM_ID { get; set; }
        public int ITEM_ID { get; set; }
        public decimal? ROAD_FROM { get; set; }
        public decimal? ROAD_TO { get; set; }
    }
}