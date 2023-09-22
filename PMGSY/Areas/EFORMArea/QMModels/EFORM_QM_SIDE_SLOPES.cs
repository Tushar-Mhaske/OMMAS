using PMGSY.Areas.EFORMArea.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.QMModels
{
    public class EFORM_QM_SIDE_SLOPES
    {
        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int SIDE_SLOP_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int EFORM_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int IMS_PR_ROAD_CODE { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-13: Item 5.EARTHWORK & SUB GRADE- IV. (a) Please select Record side slopes of embankment proposed in DPR ")]
        public string SIDE_SLOPS_ASPER_DPR { get; set; }


        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        //  [Required(ErrorMessage = "Page-13: Item 5.EARTHWORK & SUB GRADE- IV. (c) Please select Whether stability analysis has been carried out in DPR ")]
        [RegularExpression(pattern: @"^[A-Za-z]{0,1}$", ErrorMessage = "Page-13: Item 5.EARTHWORK & SUB GRADE- IV. (c) Maximum one character is allowed in Whether stability analysis has been carried out in DPR ")]
        public string IS_ANALYSIS_DONE { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-13: Item 5.EARTHWORK & SUB GRADE- IV. (c) Please enter observations about adequacy of slopes provided ")]
        [StringLength(100, ErrorMessage = "Page-13: Item 5.EARTHWORK & SUB GRADE- IV. (c) The length must be 100 character or less for observations about adequacy of slopes provided")]
        public string OBSERVATIONS { get; set; }


        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-13: Item 5.EARTHWORK & SUB GRADE- IV. Please select Sub-Item Grading 5-IV ")]
        [RegularExpression(pattern: @"^[A-Za-z]{0,3}$", ErrorMessage = "Page-13: Item 5.EARTHWORK & SUB GRADE- IV. Maximum one character is allowed in Sub-Item Grading 5-IV ")]

        public string SUBITEM_GRADING_5IV { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-13: Item 5.EARTHWORK & SUB GRADE- IV. Please enter suggestions for improvement for Sub-Item Grading 5-IV ")]
        [StringLength(250, ErrorMessage = "Page-13: Item 5.EARTHWORK & SUB GRADE- IV. The length must be 250 character or less for suggestions for improvement for Sub-Item Grading 5-IV")]
        public string IMPROVEMENT_REMARK_5IV { get; set; }


        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-13: Item 5.EARTHWORK & SUB GRADE- IV. Please select Item Grading-5 ")]
        [RegularExpression(pattern: @"^[A-Za-z]{0,3}$", ErrorMessage = "Page-13: Item 5.EARTHWORK & SUB GRADE- IV. Maximum three character is allowed in Item Grading-5: ")]

        public string ITEM_GRADING_5 { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-13: Item 5.EARTHWORK & SUB GRADE- IV. Please enter suggestions for improvement for Item Grading-5 ")]
        [StringLength(250, ErrorMessage = "Page-13: Item 5.EARTHWORK & SUB GRADE- IV. The length must be 250 character or less in suggestions for improvement for Item Grading-5")]
        public string IMPROVEMENT_REMARK_5 { get; set; }



        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int QM_USER_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public string IPADD { get; set; }


    }
}