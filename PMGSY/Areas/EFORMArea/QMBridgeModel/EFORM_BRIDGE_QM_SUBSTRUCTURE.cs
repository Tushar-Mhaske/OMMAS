using PMGSY.Areas.EFORMArea.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.QMBridgeModel
{
    public class EFORM_BRIDGE_QM_SUBSTRUCTURE
    {

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-16: Item 5. SUBSTRUCTURE- Please select Item Grading-5 ")]
        [RegularExpression(pattern: @"^[A-Za-z]{0,3}$", ErrorMessage = "Page-16: Item 5. SUBSTRUCTURE- Maximum three character is allowed in Item Grading-5 ")]
        public string ITEM_GRADING_5 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-16: Item 5. SUBSTRUCTURE- Please enter suggestions for improvement ")]
        [StringLength(250, ErrorMessage = "Page-16: Item 5. SUBSTRUCTURE- The length must be 250 character or less for  suggestions for improvement")]
        public string IMPROVEMENT_REMARK_5 { get; set; }


    }
}