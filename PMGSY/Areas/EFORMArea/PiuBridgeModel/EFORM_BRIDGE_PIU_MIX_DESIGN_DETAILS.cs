using PMGSY.Areas.EFORMArea.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.PiuBridgeModel
{
    public class EFORM_BRIDGE_PIU_MIX_DESIGN_DETAILS
    {

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int ITEM_ID { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-7: Item 5. DETAILS OF MIX DESIGN- Please enter Mix design Strength of ")]
        [StringLength(250, ErrorMessage = "Page-7: Item 5. DETAILS OF MIX DESIGN-The length must be 250 character or less for Mix design Strength of ")]
        public string DESIGN_STRENGTH { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-7:  Item 5. DETAILS OF MIX DESIGN-Please enter Institute/laboratory where mix design was done for ")]
        [StringLength(250, ErrorMessage = "Page-7: Item 5. DETAILS OF MIX DESIGN-The length must be 250 character or less for Mix design Strength for ")]
        public string DESIGN_LAB { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-7: Item 5. DETAILS OF MIX DESIGN-Please enter date for ")]
        [StringLength(250, ErrorMessage = "Page-7: Item 5. DETAILS OF MIX DESIGN-The length must be 250 character or less for Mix design application authority for ")]
        public string DESIGN_APP_AUTH { get; set; }
    }
}