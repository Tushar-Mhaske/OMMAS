using PMGSY.Areas.EFORMArea.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.QMBridgeModel
{
    public class EFORM_BRIDGE_QM_LOAD_TEST
    {

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-23: Item 7. LOAD TEST- I. Please select Results of Load Test attached or not ")]
        [StringLength(3, ErrorMessage = "Page-23: Item 7. LOAD TEST- I. The length must be 3 character or less for Results of Load Test attached or not ")]
        public string LOAD_TEST_RESULT { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-23: Item 7. LOAD TEST- Please select tem Grading-7 ")]
        [StringLength(3, ErrorMessage = "Page-23: Item 7. LOAD TEST- The length must be 3 character or less for tem Grading-7 ")]
        public string ITEM_GRADING_7 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-23: Item 7. LOAD TEST- Please enter suggestions for improvement for Item Grading-7")]
        [StringLength(250, ErrorMessage = "Page-23: Item 7. LOAD TEST- The length must be 250 character or less for suggestions for improvement for Item Grading-7 ")]
        public string IMPROVEMENT_REMARK_7 { get; set; }
     

    }
}