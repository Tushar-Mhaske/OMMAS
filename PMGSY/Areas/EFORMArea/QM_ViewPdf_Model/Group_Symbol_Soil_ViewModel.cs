using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Areas.EFORMArea.QM_ViewPdf_Model
{
    public class Group_Symbol_Soil_ViewModel
    {


   
        public int SYMBOL_ID { get; set; }

     
        public int QOM_ID { get; set; }

    
        public int EFORM_ID { get; set; }


        public int IMS_PR_ROAD_CODE { get; set; }

        public Nullable<decimal> LOCATION_RD_11 { get; set; }


        public string GSS_ASPER_DPR { get; set; }

        public string GSS_OBSERVED { get; set; }


        public string SUITABILITY { get; set; }


        public string MATERIAL_QLTY { get; set; }


        public int QM_USER_ID { get; set; }


        public int RowId { get; set; }

        public string IPADD { get; set; }



    }
}