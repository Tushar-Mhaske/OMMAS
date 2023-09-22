using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.Model
{
    public class EFORM_MIX_DESIGN_DETAILS_PIU
    {
        [FieldType(PropertyType = PDFFieldType.Skip)]
        public short ITEM_ID { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-4: Item 4. DETAILS OF MIX DESIGN- Please enter Mix design Strength of ")]
        [StringLength(250, ErrorMessage = "Page-4: Item 4. DETAILS OF MIX DESIGN-The length must be 250 character or less for Mix design Strength of ")]
        public string DESIGN_STRENGTH { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-4:  Item 4. DETAILS OF MIX DESIGN-Please enter Institute/laboratory where mix design was done for ")]
        [StringLength(250, ErrorMessage = "Page-4: Item 4. DETAILS OF MIX DESIGN-The length must be 250 character or less for Mix design Strength for")]
        public string DESIGN_LAB { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-4: Item 4. DETAILS OF MIX DESIGN-Please enter date for ")]
        [RegularExpression(pattern: @"^([0]?[0-9]|[12][0-9]|[3][01])/([0]?[1-9]|[1][0-2])/([0-9]{4})$", ErrorMessage = "Page-4: Item 4. DETAILS OF MIX DESIGN-Please Enter Valid date{in dd/mm/yyyy format} in date for ")]

        public string DESIGN_DATE { get; set; }

    }
}