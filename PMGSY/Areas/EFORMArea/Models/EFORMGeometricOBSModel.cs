using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Areas.EFORMArea.Model
{
    public class EFORMGeometricOBSModel
    {
        public string ROAD_LOC { get; set; }
        public string IA_ROAD_WIDTH_DPR { get; set; }
        public string IA_ROAD_WIDTH_ACTUAL { get; set; }
        public string IA_ROAD_WIDTH_GRADE { get; set; }
        public string IB_CARRIAGE_WIDTH_DPR { get; set; }
        public string IB_CARRIAGE_WIDTH_ACTUAL { get; set; }
        public string IB_CARRIAGE_WIDTH_GRADE { get; set; }
        public string IC_CAMBER_PER_DPR { get; set; }
        public string IC_CAMBER_PER_ACTUAL { get; set; }
        public string IC_CAMBER_PER_GRADE { get; set; }
        public string TABLE_FLAG { get; set; }
    }

    public class GeoElevation
    {
        public string ROAD_LOC { get; set; }
        public string IIA_ELEVATION_PER_DPR { get; set; }
        public string IIA_ELEVATION_PER_ACTUAL { get; set; }
        public string IIA_ELEVATION_PER_GRADE { get; set; }
        public string IIB_EXTRA_WIDENING_DPR { get; set; }
        public string IIB_EXTRA_WIDENING_ACTUAL { get; set; }
        public string IIB_EXTRA_WIDENING_GRADE { get; set; }
        public string TABLE_FLAG { get; set; }

    }
public class GeoLong
{
        public string IIIA_LONG_GRAD_PER_DPR { get; set; }
        public string IIIA_LONG_GRAD_PER_ACTUAL { get; set; }
        public string IIIA_LONG_GRAD_PER_GRADE { get; set; }
        public string TABLE_FLAG { get; set; }
        public string ROAD_FROM { get; set; }
        public string ROAD_TO { get; set; }

    }


    public class QM_Result_Verification
    {
        public string ROAD_LOC_Tresult { get; set; }
        public string TEST_NAME { get; set; }
        public string TEST_CONDUCTED_RESULT { get; set; }
        public string TEST_RESULT_QCR1 { get; set; }
        public string TEST_RESULT_PREVIOUS { get; set; }
        public string TEST_RESULT_CONFRM { get; set; }
    
    }

    public class QM_Test_Detail
    {
        public string DPR_QUANTITY { get; set; }
        public string EXECUTED_QUANTITY { get; set; }
        public string Test_Detail_TEST_NAME { get; set; }
        public string REQD_TEST_NUMBER { get; set; }
        public string CONDUCTED_TEST_NUMBER { get; set; }
        public string IS_TESTING_ADEQUATE { get; set; }
        public string ActivityName { get; set; }
    }
    public class QM_Attention
    {
        public string IS_ALL_TEST_CONDUCTED { get; set; }
        public string IS_QC_REG_P1_MAINTAINED { get; set; }
        public string IS_QC_REG_P2_MAINTAINED { get; set; }
        public string LESS_TESTING_FLAG { get; set; }
        public string LESS_TESTING_REASON { get; set; }
        public string IS_NON_CONFORMITIES_QCR2 { get; set; }
        public string ITEM_GRADING_3 { get; set; }
        public string IMPROVEMENT_REMARK { get; set; }
    }

    public class PresentWork
    {
     public string workRdFrom { get; set; }
        public string workRdTo { get; set; }
        public string ActivityName {get;  set;}
    }
}