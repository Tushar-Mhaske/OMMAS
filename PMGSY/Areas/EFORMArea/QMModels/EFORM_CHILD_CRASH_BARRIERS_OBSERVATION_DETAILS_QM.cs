using PMGSY.Areas.EFORMArea.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.QMModels
{
    public class EFORM_CHILD_CRASH_BARRIERS_OBSERVATION_DETAILS_QM
    {
        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int RowID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public string WORK_TYPE { get; set; }
        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-26: Item 16.CRASH BARRIERS AND ROAD SAFETY SIGN BOARDS- IV.Table- Please enter Location (RD) of row ")]
        [RegularExpression(@"^(?:\d{0,7}\.\d{1,3})$|^\d{0,7}$", ErrorMessage = "Page-26: Item 16.CRASH BARRIERS AND ROAD SAFETY SIGN BOARDS- IV.Table- Maximum seven digits before decimal and maximum three digits after decimal are allowed in Location (RD) of row ")]

        public decimal LOCATION_RD_26 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-26: Item 16.CRASH BARRIERS AND ROAD SAFETY SIGN BOARDS- IV.Table-  Please enter Type of crash barrier of row ")]
        [StringLength(maximumLength: 20, ErrorMessage = "Page-26: Item 16.CRASH BARRIERS AND ROAD SAFETY SIGN BOARDS- IV.Table- Only 20 Characters Allowed in Type of crash barrier of row ")]

        public string CRASH_BARRIERS_TYPE_26 { get; set; }


        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-26: Item 16.CRASH BARRIERS AND ROAD SAFETY SIGN BOARDS- IV.Table-  Please select Overall quality of safety measures in road (S / U) of row ")]
        [RegularExpression(@"^[A-Za-z]{0,1}$", ErrorMessage = "Page-26: Item 16.CRASH BARRIERS AND ROAD SAFETY SIGN BOARDS- IV.Table- Maximum one character is allowed in Overall quality of safety measures in road (S / U) of row ")]

        public string GRADING_SAFETY_MEAS_26 { get; set; }


        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-26: Item 16.CRASH BARRIERS AND ROAD SAFETY SIGN BOARDS- IV.Table-  Please select Mandatory and cautionary signboards fixed at appropriate location (Yes / No) of row ")]
        [RegularExpression(@"^[A-Za-z]{0,1}$", ErrorMessage = "Page-26: Item 16.CRASH BARRIERS AND ROAD SAFETY SIGN BOARDS- IV.Table- Maximum one character is allowed in Mandatory and cautionary signboards fixed at appropriate location (Yes / No) of row ")]

        public string IS_SIGNBOARDS_FIXED_26 { get; set; }
    }
}