using PMGSY.Areas.EFORMArea.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.QMBridgeModel
{
    public class EFORM_BRIDGE_QM_CHILD_OBS_FURNITURE_MARKING
    {

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-29: Item 11. 11.2 Observations: iii) Please Select Guard stones fixed on approaches ")]
        [StringLength(3, ErrorMessage = "Page-29: Item 11. 11.2 Observations: iii) The length must be 3 character or less for Guard stones fixed on approaches")]

        public string IS_GUARD_STONES_FIXED { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-29: Item 11. 11.2 Observations: iv) Please Select Mandatory and cautionsignage ")]
        [StringLength(3, ErrorMessage = "Page-29: Item 11. 11.2 Observations: iv) The length must be 3 character or less for Mandatory and cautionsignage")]

        public string IS_MANDATORY_CAUTION_SIGNAGE { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-29: Item 11. Item Grading-11.2 Please select item Grading-11.2 ")]
        [StringLength(3, ErrorMessage = "Page-29: Item 11. Item Grading-11.2 The length must be 3 character or less for item Grading-11.2")]

        public string ITEM_GRADING_11_2 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-29: Item 11. Item Grading-11.2-  Please enter suggestions for improvement ")]
        [StringLength(250, ErrorMessage = "Page-29: Item 11. Item Grading-11.2- The length must be 250 character or less for suggestions for improvement  ")]
        public string IMPROVEMENT_REMARK_11_2 { get; set; }
       

    }
}