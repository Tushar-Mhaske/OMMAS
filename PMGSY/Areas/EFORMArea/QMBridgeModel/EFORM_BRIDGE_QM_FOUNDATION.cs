using PMGSY.Areas.EFORMArea.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.QMBridgeModel
{
    public class EFORM_BRIDGE_QM_FOUNDATION
    {
        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int FOUNDATION_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int EFORM_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int PR_ROAD_CODE { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-14: Item 4. FOUNDATION- Please select Item Grading-4 ")]        
        [RegularExpression(pattern: @"^[A-Za-z]{0,3}$", ErrorMessage = "Page-14: Item 4. FOUNDATION- Maximum three character is allowed in Item Grading-4 ")]
        public string ITEM_GRADING_4 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-14: Item 4. FOUNDATION- Please enter suggestions for improvement ")]
        [StringLength(250, ErrorMessage = "Page-14: Item 4. FOUNDATION- The length must be 250 character or less for  suggestions for improvement")]
        public string IMPROVEMENT_REMARK_4 { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public string IPADD { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int USER_ID { get; set; }

    }
}