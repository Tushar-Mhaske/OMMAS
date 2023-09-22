using PMGSY.Areas.EFORMArea.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
 using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.QMModels
{
    public class EFORM_CHILD_BASECOURSE_UCS_DETAILS_LAYER2_QM
    {
        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int RowID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public string UCS_TYPE { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-17: Item 8. BASE COURSE: 2nd Layer- IV. (g) UCS Details Table- Please enter Location of new technology section RD (km) From of row ")]
        [RegularExpression(@"^(?:\d{0,7}\.\d{1,3})$|^\d{0,7}$", ErrorMessage = "Page-17 Item 8. BASE COURSE: 2nd Layer- IV. (g) UCS Details Table- Maximum seven digits before decimal and maximum three digits after decimal are allowed in Location of new technology section RD (km) From of row ")]

        public Nullable<decimal> LOCATION_RD_FROM_17 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-17: Item 8. BASE COURSE: 2nd Layer- IV. (g) UCS Details Table- Please enter Location of new technology section RD (km) To of row  ")]
        [RegularExpression(@"^(?:\d{0,7}\.\d{1,3})$|^\d{0,7}$", ErrorMessage = "Page-17 Item 8. BASE COURSE: 2nd Layer- IV. (g) UCS Details Table- Maximum seven digits before decimal and maximum three digits after decimal are allowed in Location of new technology section RD (km) To of row ")]

        public Nullable<decimal> LOCATION_RD_TO_17 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-17: Item 8. BASE COURSE: 2nd Layer- IV. (g) UCS Details Table- Please enter UCS value as per mix design (MPa) of row ")]
        [RegularExpression(@"^(?:\d{0,7}\.\d{1,3})$|^\d{0,7}$", ErrorMessage = "Page-17 Item 8. BASE COURSE: 2nd Layer- IV. (g) UCS Details Table- Maximum seven digits before decimal and maximum three digits after decimal are allowed in UCS value as per mix design (MPa) of row ")]

        public Nullable<decimal> UCS_ASPER_MIX_DESIGN_17 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-17: Item 8. BASE COURSE: 2nd Layer- IV. (g) UCS Details Table- Please enter UCS value achieved as per records of PIU (MPa) of row ")]
        [RegularExpression(@"^(?:\d{0,7}\.\d{1,3})$|^\d{0,7}$", ErrorMessage = "Page-17 Item 8. BASE COURSE: 2nd Layer- IV. (g) UCS Details Table- Maximum seven digits before decimal and maximum three digits after decimal are allowed in UCS value achieved as per records of PIU (MPa) of row ")]

        public Nullable<decimal> UCS_ACHIEVED_17 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-17: Item 8. BASE COURSE: 2nd Layer- IV. (g) UCS Details Table- Please select Whether UCS value achieved on ground is acceptable (Yes / No) of row ")]
        [RegularExpression(@"^[A-Za-z ]{0,1}$", ErrorMessage = "Page-17 Item 8. BASE COURSE: 2nd Layer- IV. (g) UCS Details Table- Maximum one character is allowed in Whether UCS value achieved on ground is acceptable (Yes / No) of row ")]

        public string IS_UCS_ACCEPTABLE_17 { get; set; }
    }
}