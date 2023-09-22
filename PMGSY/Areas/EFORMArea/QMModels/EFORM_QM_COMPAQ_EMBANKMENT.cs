using PMGSY.Areas.EFORMArea.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.QMModels
{
    public class EFORM_QM_COMPAQ_EMBANKMENT
    {
        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int COMPAQ_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int EFORM_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int IMS_PR_ROAD_CODE { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-12: Item 5.EARTHWORK & SUB GRADE- III. Please enter Maximum dry density (MDD) ")]
        [RegularExpression(@"^[0-9]{0,7}(?:\.[0-9]{1,3})?$", ErrorMessage = "Page-11: Item 5.EARTHWORK & SUB GRADE- III. Maximum seven digits before decimal and maximum three digits after decimal are allowed in Maximum dry density (MDD)")]
        public decimal MDD { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-12: Item 5.EARTHWORK & SUB GRADE- III. Please enter Optimum moisture content (OMC) ")]
        [RegularExpression(@"^[0-9]{0,3}(?:\.[0-9]{1,3})?$", ErrorMessage = "Page-11: Item 5.EARTHWORK & SUB GRADE- III. Maximum three digits before decimal and maximum three digits after decimal are allowed in Optimum moisture content (OMC)")]
        public decimal OMC { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-12: Item 5.EARTHWORK & SUB GRADE- III. Please select Sub-Item Grading 5-III ")]
        [StringLength(3, ErrorMessage = "Page-12: Item 5.EARTHWORK & SUB GRADE- III. Maximum three characters are allowed in Sub-Item Grading 5-III")]
        public string SUBITEM_GRADING_5_III { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-12: Item 5.EARTHWORK & SUB GRADE- III. Please enter suggestions for improvement ")]
        [StringLength(250, ErrorMessage = "Page-12: Item 5.EARTHWORK & SUB GRADE- III. Maximum 250 characters are allowed in suggestions for improvement")]
        public string IMPROVEMENT_REMARK_12_2 { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int QM_USER_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public string IPADD { get; set; }

    }
}