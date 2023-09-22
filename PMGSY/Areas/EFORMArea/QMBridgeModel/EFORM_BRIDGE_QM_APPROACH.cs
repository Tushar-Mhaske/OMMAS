using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;
using System.ComponentModel.DataAnnotations;
using PMGSY.Areas.EFORMArea.Model;
namespace PMGSY.Areas.EFORMArea.QMBridgeModel
{
    public class EFORM_BRIDGE_QM_APPROACH
    {

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-28: Item 10. APPROACHES-  Please select Item Grading-10:")]
        [StringLength(3, ErrorMessage = "Page-28: Item 10. APPROACHES- The length must be 3 character or less for Item Grading-10:")]

        public string ITEM_GRADING_10 { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-28: Item 10. APPROACHES- Please enter suggestions for improvement")]
        [StringLength(250, ErrorMessage = "Page-28: Item 10. APPROACHES-  The length must be 250 character or less for suggestions for improvement)")]

        public string IMPROVEMENT_REMARK_10 { get; set; }
       


    }
}