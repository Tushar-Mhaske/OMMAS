using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;
using System.ComponentModel.DataAnnotations;
using PMGSY.Areas.EFORMArea.Model;

namespace PMGSY.Areas.EFORMArea.QMBridgeModel
{
    public class EFORM_BRIDGE_QM_BEARING
    {
       
        [FieldType(PropertyType = PDFFieldType.CheckBox)]
        // [Required(ErrorMessage = "Page-23: Item 8. BEARINGS- A) Types of Bearing  Please select Results of BEARINGS attached or not ")]
        [RegularExpression(pattern: @"^[Y,N]{1}$", ErrorMessage = "Page-23: Item 8. BEARINGS- A) Types of Bearing: only (Y/N) is allowed in Roller and Rocker Bearing ")]
        public string ROLLER_ROCKER_BEARING { get; set; }

        [FieldType(PropertyType = PDFFieldType.CheckBox)]
        // [Required(ErrorMessage = "Page-23: Item 8. BEARINGS- A) Types of Bearing  Please select Results of BEARINGS attached or not ")]
        [RegularExpression(pattern: @"^[Y,N]{1}$", ErrorMessage = "Page-23: Item 8. BEARINGS- A) Types of Bearing: only (Y/N) is allowed in Elastomeric Bearing")]
        public string ELASTOMERIC_BEARING { get; set; }

        [FieldType(PropertyType = PDFFieldType.CheckBox)]
        //  [Required(ErrorMessage = "Page-23: Item 8. BEARINGS- A) Types of Bearing  Please select Results of BEARINGS attached or not ")]
        [RegularExpression(pattern: @"^[Y,N]{1}$", ErrorMessage = "Page-23: Item 8. BEARINGS- A) Types of Bearing: only (Y/N) is allowed in Pot cum PTFE Bearing")] 
        public string POT_BEARING { get; set; }

        [FieldType(PropertyType = PDFFieldType.CheckBox)]
        // [Required(ErrorMessage = "Page-23: Item 8. BEARINGS- A) Types of Bearing  Please select Results of BEARINGS attached or not ")]
        [RegularExpression(pattern: @"^[Y,N]{1}$", ErrorMessage = "Page-23: Item 8. BEARINGS- A) Types of Bearing: only (Y/N) is allowed in spherical Bearing")]
        public string SPHERICAL_BEARING { get; set; }


        [FieldType(PropertyType = PDFFieldType.CheckBox)]
        // [Required(ErrorMessage = "Page-23: Item 8. BEARINGS- A) Types of Bearing  Please select Results of BEARINGS attached or not ")]
        [RegularExpression(pattern: @"^[Y,N]{1}$", ErrorMessage = "Page-23: Item 8. BEARINGS- A) Types of Bearing: only (Y/N) is allowed in cylindrical Bearing")]
        public string CYLINDRICAL_BEARING { get; set; }


        [FieldType(PropertyType = PDFFieldType.CheckBox)]
        // [Required(ErrorMessage = "Page-23: Item 8. BEARINGS- I. Please select Results of BEARINGS attached or not ")]
        [RegularExpression(pattern: @"^[Y,N]{1}$", ErrorMessage = "Page-23: Item 8. BEARINGS- A) Types of Bearing: only (Y/N) is allowed in Any other type")]
        public string IS_OTHER_BEARING_TYPE { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-23: Item 8. BEARINGS- A) Types of Bearing: Please enter other type bearing details ")]
        [StringLength(30, ErrorMessage = "Page-23: Item 8. BEARINGS- A) Types of Bearing: The length must be 30 character or less for other type bearing details ")]
        public string OTHER_BEARING_TYPE { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-24: Item 8. BEARINGS-  Please select item Grading-8 ")]
        [StringLength(3, ErrorMessage = "Page-24: Item 8. BEARINGS- The length must be 3 character or less for item Grading-8 ")]
        public string ITEM_GRADING_8 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-24: Item 8. BEARINGS-  Please enter suggestions for improvement ")]
        [StringLength(250, ErrorMessage = "Page-24: Item 8. BEARINGS-  The length must be 250 character or less for uggestions for improvement  ")]
        public string IMPROVEMENT_REMARK_8 { get; set; }
 
    }
}