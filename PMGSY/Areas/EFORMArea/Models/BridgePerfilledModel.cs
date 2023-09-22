using PMGSY.Areas.EFORMArea.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.Models
{
    public class BridgePerfilledModel
    {

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        public string checksum { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        public string EFORM_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        public string RoadCode { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        public string WORK_STATUS { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        public string QM_NAME { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        public string QM_TYPE { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        public string STATE { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        public string DISTRICT { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        public string BLOCK { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        public string BRIDGE_NAME { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        public string PACKAGE_NUMBER { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        public string SANC_LENGTH { get; set; }

        //[FieldType(PropertyType = PDFFieldType.TextBox)]
        //public string DEVIATION_LENGTH { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        public string AWARDED_COST { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        public string ESTIMATED_COST { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        public string EXPENDITURE_DONE { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        public string COMPLETION_COST { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        public string AWARD_OF_WORK_DATE { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        public string STIPULATED_COMPLETION_DATE { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        public string START_OF_WORK_DATE { get; set; }


        //[FieldType(PropertyType = PDFFieldType.RadioButton)]
        //public string CURRENT_STAGE { get; set; }

        //add on 22-07-2022
        [FieldType(PropertyType = PDFFieldType.TextBox)]
        public string INSP_ID_1 { get; set; }
        [FieldType(PropertyType = PDFFieldType.TextBox)]
        public string INSP_ID_2 { get; set; }
        [FieldType(PropertyType = PDFFieldType.TextBox)]
        public string INSP_ID_3 { get; set; }
        [FieldType(PropertyType = PDFFieldType.TextBox)]
        public string INSP_ID_4 { get; set; }
        [FieldType(PropertyType = PDFFieldType.TextBox)]
        public string INSP_ID_5 { get; set; }
        [FieldType(PropertyType = PDFFieldType.TextBox)]
        public string INSP_ID_6 { get; set; }
        [FieldType(PropertyType = PDFFieldType.TextBox)]
        public string INSP_ID_7 { get; set; }
        [FieldType(PropertyType = PDFFieldType.TextBox)]
        public string INSP_ID_8 { get; set; }
        //add on 22-07-2022


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        public string VISIT_DATE_1 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        public string VISIT_DATE_2 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        public string VISIT_DATE_3 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        public string VISIT_DATE_4 { get; set; }
        [FieldType(PropertyType = PDFFieldType.TextBox)]
        public string VISIT_DATE_5 { get; set; }
        [FieldType(PropertyType = PDFFieldType.TextBox)]
        public string VISIT_DATE_6 { get; set; }
        [FieldType(PropertyType = PDFFieldType.TextBox)]
        public string VISIT_DATE_7 { get; set; }
        [FieldType(PropertyType = PDFFieldType.TextBox)]
        public string VISIT_DATE_8 { get; set; }








        [FieldType(PropertyType = PDFFieldType.TextBox)]
        public string VISITOR_NAME_DESG_1 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        public string VISITOR_NAME_DESG_2 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        public string VISITOR_NAME_DESG_3 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        public string VISITOR_NAME_DESG_4 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        public string VISITOR_NAME_DESG_5 { get; set; }
        [FieldType(PropertyType = PDFFieldType.TextBox)]
        public string VISITOR_NAME_DESG_6 { get; set; }
        [FieldType(PropertyType = PDFFieldType.TextBox)]
        public string VISITOR_NAME_DESG_7 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        public string VISITOR_NAME_DESG_8 { get; set; }









        [FieldType(PropertyType = PDFFieldType.TextBox)]
        public string ROAD_FROM_1 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        public string ROAD_FROM_2 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        public string ROAD_FROM_3 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        public string ROAD_FROM_4 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        public string ROAD_FROM_5 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        public string ROAD_FROM_6 { get; set; }
        [FieldType(PropertyType = PDFFieldType.TextBox)]
        public string ROAD_FROM_7 { get; set; }
        [FieldType(PropertyType = PDFFieldType.TextBox)]
        public string ROAD_FROM_8 { get; set; }





        [FieldType(PropertyType = PDFFieldType.TextBox)]
        public string ROAD_TO_1 { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        public string ROAD_TO_2 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        public string ROAD_TO_3 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        public string ROAD_TO_4 { get; set; }
        [FieldType(PropertyType = PDFFieldType.TextBox)]
        public string ROAD_TO_5 { get; set; }
        [FieldType(PropertyType = PDFFieldType.TextBox)]
        public string ROAD_TO_6 { get; set; }
        [FieldType(PropertyType = PDFFieldType.TextBox)]
        public string ROAD_TO_7 { get; set; }
        [FieldType(PropertyType = PDFFieldType.TextBox)]
        public string ROAD_TO_8 { get; set; }


      



    }
}