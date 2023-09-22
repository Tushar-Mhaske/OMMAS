using PMGSY.Areas.EFORMArea.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.QMModels
{
    public class EFORM_CHILD_PROT_WORKS_WORKMANSHIP_OF_RS_DETAILS_QM
    {
        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int RowID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public string WORK_TYPE { get; set; }
        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-25: Item 15.PROTECTION WORK- V.Workmanship of retaining structures Table- Please enter Location / RD of row ")]
        [RegularExpression(@"^(?:\d{0,7}\.\d{1,3})$|^\d{0,7}$", ErrorMessage = "Page-25: Item 15.PROTECTION WORK- V.Workmanship of retaining structures Table- Maximum seven digits before decimal and maximum three digits after decimal are allowed in Location / RD of row ")]

        public decimal LOCATION_RD_25_2 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-25: Item 15.PROTECTION WORK- V.Workmanship of retaining structures Table-  Please select Workmanship of retaining structures (S//U) of row  ")]
        [RegularExpression(@"^[A-Za-z]{0,1}$", ErrorMessage = "Page-25: Item 15.PROTECTION WORK- V.Workmanship of retaining structures Table- Maximum one character is allowed in Workmanship of retaining structures (S//U) of row ")]
        public string WORKMANSHIP_RS_25 { get; set; }
        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-25: Item 15.PROTECTION WORK- V.Workmanship of retaining structures Table-  Please select Whether surface is free from honeycombing/any other defects(Y/N) of row  ")]
        [RegularExpression(@"^[A-Za-z]{0,1}$", ErrorMessage = "Page-25: Item 15.PROTECTION WORK- V.Workmanship of retaining structures Table- Maximum one character is allowed in Whether surface is free from honeycombing/any other defects(Y/N) of row ")]
        public string IS_SURFACE_HONEYCOMBING_FREE_25 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-25: Item 15.PROTECTION WORK- V.Workmanship of retaining structures Table-  Please select Have weep holes been provided (Yes / No) of row ")]
        [RegularExpression(@"^[A-Za-z]{0,1}$", ErrorMessage = "Page-25: Item 15.PROTECTION WORK- V.Workmanship of retaining structures Table- Maximum one character is allowed in Have weep holes been provided (Yes / No) of row ")]
        public string IS_WEEP_HOLES_PROVIDED_25 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-25: Item 15.PROTECTION WORK- V.Workmanship of retaining structures Table-  Please enter Spacing of weep holes (if provided) (mm) As per drawing of row ")]
        [RegularExpression(@"^(?:\d{0,7}\.\d{1,3})$|^\d{0,7}$", ErrorMessage = "Page-25: Item 15.PROTECTION WORK- V.Workmanship of retaining structures Table- Maximum seven digits before decimal and maximum three digits after decimal are allowed in Spacing of weep holes (if provided) (mm) As per drawing of row  ")]

        public Nullable<decimal> WEEP_HOLES_SPACING_ASPER_DRAWING_25 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-25: Item 15.PROTECTION WORK- V.Workmanship of retaining structures Table-  Please enter Spacing of weep holes (if provided) (mm) Actual at site  of row ")]
        [RegularExpression(@"^(?:\d{0,7}\.\d{1,3})$|^\d{0,7}$", ErrorMessage = "Page-25: Item 15.PROTECTION WORK- V.Workmanship of retaining structures Table- Maximum seven digits before decimal and maximum three digits after decimal are allowed in Spacing of weep holes (if provided) (mm) Actual at site of row ")]

        public Nullable<decimal> WEEP_HOLES_SPACING_ACTUAL_25 { get; set; }
    }
}