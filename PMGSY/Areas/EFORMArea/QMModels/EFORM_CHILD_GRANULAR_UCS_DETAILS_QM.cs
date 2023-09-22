using PMGSY.Areas.EFORMArea.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.QMModels
{
    public class EFORM_CHILD_GRANULAR_UCS_DETAILS_QM
    {
        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int RowID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public string UCS_TYPE { get; set; }

        
        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-14: Item 6.GRANULAR SUB-BASE (GSB)- III. (g) UCS Details Table- Please enter Location of new technology section RD (km) From of row  ")]
        [RegularExpression(@"^(?:\d{0,7}\.\d{1,3})$|^\d{0,7}$", ErrorMessage = "Page-14 Item 6.GRANULAR SUB-BASE (GSB)- III. (g) UCS Details Table- Maximum seven digits before decimal and maximum three digits after decimal are allowed in Location of new technology section RD (km) From of row ")]

        public decimal LOCATION_RD_FROM_14 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-14: Item 6.GRANULAR SUB-BASE (GSB)- III. (g) UCS Details Table- Please enter Location of new technology section RD (km) To of row  ")]
        [RegularExpression(@"^(?:\d{0,7}\.\d{1,3})$|^\d{0,7}$", ErrorMessage = "Page-14 Item 6.GRANULAR SUB-BASE (GSB)- III. (g) UCS Details Table- Maximum seven digits before decimal and maximum three digits after decimal are allowed in Location of new technology section RD (km) To of row ")]

        public decimal LOCATION_RD_TO_14 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-14: Item 6.GRANULAR SUB-BASE (GSB)- III. (g) UCS Details Table- Please enter UCS value as per mix design (MPa) of row ")]
        [RegularExpression(@"^(?:\d{0,3}\.\d{1,2})$|^\d{0,3}$", ErrorMessage = "Page-14 Item 6.GRANULAR SUB-BASE (GSB)- III. (g) UCS Details Table- Maximum three digits before decimal and maximum two digits after decimal are allowed in UCS value as per mix design (MPa) of row ")]

        
        public decimal UCS_ASPER_MIX_DESIGN_14 { get; set; }
        [Required(ErrorMessage = "Page-14: Item 6.GRANULAR SUB-BASE (GSB)- III. (g) UCS Details Table- Please enter UCS value achieved as per records of PIU (MPa) of row ")]
        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [RegularExpression(@"^(?:\d{0,3}\.\d{1,2})$|^\d{0,3}$", ErrorMessage = "Page-14 Item 6.GRANULAR SUB-BASE (GSB)- III. (g) UCS Details Table- Maximum three digits before decimal and maximum two digits after decimal are allowed in UCS value achieved as per records of PIU (MPa) of row ")]


        public decimal UCS_ACHIEVED_14 { get; set; }
     
        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-14: Item 6.GRANULAR SUB-BASE (GSB)- III. (g) UCS Details Table- Please select Whether UCS value achieved on ground is acceptable (Yes / No) of row ")]
        [RegularExpression(@"^[A-Za-z ]{0,1}$", ErrorMessage = "Page-14 Item 6.GRANULAR SUB-BASE (GSB)- III. (g) UCS Details Table- Maximum one character is allowed in Whether UCS value achieved on ground is acceptable (Yes / No) of row ")]

        public string IS_UCS_ACCEPTABLE_14 { get; set; }
        

        
    }
}