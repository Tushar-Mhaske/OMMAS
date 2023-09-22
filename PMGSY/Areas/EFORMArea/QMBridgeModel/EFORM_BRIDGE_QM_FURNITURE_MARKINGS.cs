using PMGSY.Areas.EFORMArea.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.QMBridgeModel
{
    public class EFORM_BRIDGE_QM_FURNITURE_MARKINGS
    {
        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-29: Item 11. 11.1 Observations: i) Please Select Main Information BoardFixed ")]
        [StringLength(3, ErrorMessage = "Page-29: Item 11. 11.1 Observations: i) The length must be 3 character or less for Main Information BoardFixed")]

        public string IS_MAIN_INFO_BOARD_FIXED { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-29: Item 11. 11.1 Observations: ii) Please Select Citizen Information BoardFixed ")]
        [StringLength(3, ErrorMessage = "Page-29: Item 11. 11.1 Observations: ii) The length must be 3 character or less for Citizen Information BoardFixed")]

        public string IS_CITIZEN_INFO_BOARD_FIXED { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-29: Item 11. Item Grading-11.1 Please select item Grading-11.1 ")]
        [StringLength(3, ErrorMessage = "Page-29: Item 11. Item Grading-11.1 The length must be 3 character or less for item Grading-11.1")]

        public string ITEM_GRADING_11_1 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-29: Item 11. Item Grading-11.1-  Please enter suggestions for improvement ")]
        [StringLength(250, ErrorMessage = "Page-29: Item 11. Item Grading-11.1- The length must be 250 character or less for suggestions for improvement  ")]
        public string IMPROVEMENT_REMARK_11_1 { get; set; }
       
    }
}