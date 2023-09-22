using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Areas.EFORMArea.QM_ViewPdf_Model
{
    public class DEGREE_OF_COMPAQ_ViewModel
    {


        public int DEGREE_ID { get; set; }


        public int COMPAQ_ID { get; set; }

   
        public int EFORM_ID { get; set; }


        public int IMS_PR_ROAD_CODE { get; set; }

        public Nullable<decimal> LOCATION_RD_12 { get; set; }


        public Nullable<decimal> QCR1_DRY_DENSITY { get; set; }


        public Nullable<decimal> QCR1_PERCENT_COMPAQ { get; set; }

        public string QCR1_DATE_OF_TEST { get; set; }

      
        public Nullable<decimal> MEAS_FMC { get; set; }

  
        public Nullable<decimal> MEAS_DRY_DENSITY { get; set; }

 
        public Nullable<decimal> MEAS_PERCENT_COMPAQ { get; set; }

  
        public string MEAS_GRADE { get; set; }

        public int RowId { get; set; }
              
    }
}