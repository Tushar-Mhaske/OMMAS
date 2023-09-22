using PMGSY.Areas.EFORMArea.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.QMModels
{
    public class EFORM_SIDE_AND_CATCH_DRAINS_EARTHEN_QM
    {

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-26: Item 17. SIDE DRAINS AND CATCH WATER DRAINS- I. Please select Whether sanctioned DPR has the provision of side drains and catch water drains ")]
        [RegularExpression(@"^[A-Za-z]{0,1}$", ErrorMessage = "Page-26: Item 17. SIDE DRAINS AND CATCH WATER DRAINS- I. Maximum one character is allowed IN Whether sanctioned DPR has the provision of side drains and catch water drains")]
        public string IS_DPR_PROVISION_26 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-26: Item 17. SIDE DRAINS AND CATCH WATER DRAINS-  II. Please select Whether the drains have adequate longitudinal slope ")]
        [RegularExpression(@"^[A-Za-z]{0,1}$", ErrorMessage = "Page-26: Item 17. SIDE DRAINS AND CATCH WATER DRAINS- II. Maximum one character is allowed IN Whether the drains have adequate longitudinal slope")]
        public string IS_LONG_SLOPE_ADEQUATE_26 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-27: Item 17. SIDE DRAINS AND CATCH WATER DRAINS-  Please select Item Grading-17 ")]
        [RegularExpression(@"^[A-Za-z]{0,3}$", ErrorMessage = "Page-27: Item 17. SIDE DRAINS AND CATCH WATER DRAINS- Maximum three character is allowed in Item Grading-17")]

        public string ITEM_GRADING_17 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-27: Item 17. SIDE DRAINS AND CATCH WATER DRAINS-  Please enter suggestions for improvement ")]
        [StringLength(maximumLength: 250, ErrorMessage = "Page-27: Item 17. SIDE DRAINS AND CATCH WATER DRAINS- Only 250 Characters Allowed in suggestions for improvement ")]

        public string IMPROVE_SUGGESTIONS_27 { get; set; }
    }
}