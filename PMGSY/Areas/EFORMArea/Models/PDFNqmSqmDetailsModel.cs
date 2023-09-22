using PMGSY.Areas.EFORMArea.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Areas.EFORMArea.Model
{
    public class PDFNqmSqmDetailsModel
    {
        public int id { get; set; }
        public string dateOfInsp { get; set; }
        public string qmName { get; set; }
        public string nqmOrsqm { get; set; }
        public string qmCode { get; set; }
        public string state { get; set; }
        public string District { get; set; }
        public string Block { get; set; }
        public string NameRoad { get; set; }
        public string Pkgnumber { get; set; }
        public string RdFrom { get; set; }
        public string RdTo { get; set; }
        public string currentStage { get; set; }
        public string PhysStatus { get; set; }

       

 
public string IS_FIELD_LAB_ESTD { get; set; }
public string IS_LAB_LOC_SAME { get; set; }
public string IS_EQUIP_AVAILABLE { get; set; }
public string IS_EQUIP_USED { get; set; }
public string IS_EQUIP_AVAIL_VERIFY { get; set; }
public string IS_ENGG_AVAILABLE { get; set; }
public string IS_ALT_ENGG_ARR_SATISFIED { get; set; }
public string IS_LAB_TECH_AVAILABLE { get; set; }
public string ITEM_GRADING_2 { get; set; }
public string IMPROVEMENT_REMARK_qm_Arrag { get; set; }


// Attention to quality
public string IS_ALL_TEST_CONDUCTED { get; set; }
public string IS_QC_REG_P1_MAINTAINED { get; set; }
public string IS_QC_REG_P2_MAINTAINED { get; set; }
public string LESS_TESTING_FLAG { get; set; }
public string LESS_TESTING_REASON { get; set; }
public string IS_NON_CONFORMITIES_QCR2 { get; set; }
public string ITEM_GRADING_3 { get; set; }
public string IMPROVEMENT_REMARK { get; set; }


public string ITEM_GRADING_4  { get; set; }
public string REASON_FOR_DEVIATION { get; set; }


        public List<QM_Result_Verification> ListResultVerification { set; get; }
        public List<QM_Test_Detail> ListTestDetail { set; get; }

        public List<EFORMGeometricOBSModel> ListRoadWayc { get; set; }
        public List<GeoElevation> ListElevation { get; set; }
        public List<GeoLong> ListLongitude { get; set; }

        public List<PresentWork> ListPresentWork { set; get; }







    }
}