using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.Model
{
    public class EFORM_GENERAL_INFO_PIU_View
    {

        #region Added By rohit on 21-03-2022
        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int INFO_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int EFORM_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int ADMIN_ND_CODE { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int PIU_USER_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int PR_ROAD_CODE { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public string QM_CODE { get; set; }

        #endregion rohit changes end


        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int ITEM_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        public string checksum { get; set; }

        [FieldType(PropertyType =PDFFieldType.TextBox)]
        public string INSPECTION_DATE { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        public string SAN_FLEX_PAVEMENT { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        public string SAN_RIGID_PAVEMENT { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        public decimal EXEC_LENGTH { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        public string EXEC_FLEX_PAVEMENT { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        public string EXEC_RIGID_PAVEMENT { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        public string DEVIATION_REASON { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        public string NEW_TECHNOLOGY_NAME { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        public decimal ROAD_FROM { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        public decimal ROAD_TO { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        public decimal ESTIMATED_COST { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Please enter technical sanctioned cost")]
        public decimal TECHNICAL_SANC_COST { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        public decimal AWARDED_COST { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Please enter expenditure done details")]
        public decimal EXPENDITURE_DONE { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Please enter bill pending details")]
        public decimal BILLS_PENDING { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        public decimal TOTAL_EXPENDITURE { get; set; }
        

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        public decimal COMPLETION_COST { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Please select Work Type")]
        public string WORK_TYPE { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        public decimal? TOTAL_LEN { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        public string CARRIAGEWAY_WIDTH_NEW { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        public string CARRIAGEWAY_WIDTH_TYPE { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        public string CARRIAGEWAY_WIDTH_EXISTING { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        public string CARRIAGEWAY_WIDTH_PROPOSED { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        public string CARRIAGEWAY_LENGTH { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        public string TERRAIN_TYPE { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        public string AWARD_OF_WORK_DATE { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        public string START_OF_WORK_DATE { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        public string STIPULATED_COMPLETION_DATE { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]

        public string ACTUAL_COMPLETION_DATE { get; set; }

        [FieldType(PropertyType = PDFFieldType.CheckBox)]
        public string MIX_DESIGN_APPLICABLE { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Please enter PIU Head Name")]
        public string PIU_HEAD_NAME { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Please enter PIU Head mobile number")]
        public string PIU_HEAD_MOBILE_NO { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        public string PIU_HEAD_EMAIL { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Please enter PIU Address")]
        public string PIU_ADDR { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Please enter PIU sign date")]
        public string PIU_SIGN_DATE { get; set; }
    }
}