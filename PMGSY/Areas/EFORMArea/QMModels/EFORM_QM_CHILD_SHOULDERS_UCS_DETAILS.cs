using PMGSY.Areas.EFORMArea.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.QMModels
{
    public class EFORM_QM_CHILD_SHOULDERS_UCS_DETAILS
    {

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int RowId { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int SHUCS_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int SH_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int EFORM_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int IMS_PR_ROAD_CODE { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-22: Item 12. SHOULDERS- (g) UCS Details Table- Please enter Location of new technology section RD (km) From of row ")]
        [RegularExpression(@"^[0-9]{0,7}(?:\.[0-9]{1,3})?$", ErrorMessage = "Page-22: Item 12. SHOULDERS- (g) UCS Details Table- Maximum seven digits before decimal and maximum three digits after decimal are allowed in Location of new technology section RD (km) From of row ")]
        public decimal LOCATION_RD_FROM_22 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-22: Item 12. SHOULDERS- (g) UCS Details Table- Please enter Location of new technology section RD (km) To of row ")]
        [RegularExpression(@"^[0-9]{0,7}(?:\.[0-9]{1,3})?$", ErrorMessage = "Page-22: Item 12. SHOULDERS- (g) UCS Details Table- Maximum seven digits before decimal and maximum three digits after decimal are allowed in Location of new technology section RD (km) To of row ")]
        public decimal LOCATION_RD_TO_22 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-22: Item 12. SHOULDERS- (g) UCS Details Table- Please enter UCS value as per mix design (MPa) of row ")]
        [RegularExpression(@"^[0-9]{0,7}(?:\.[0-9]{1,3})?$", ErrorMessage = "Page-22: Item 12. SHOULDERS- (g) UCS Details Table- Maximum seven digits before decimal and maximum three digits after decimal are allowed in UCS value as per mix design (MPa) of row ")]
        public decimal UCS_ASPER_MIX_DESIGN_22 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-22: Item 12. SHOULDERS- (g) UCS Details Table- Please enter UCS value achieved as per records of PIU (MPa)")]
        [RegularExpression(@"^[0-9]{0,7}(?:\.[0-9]{1,3})?$", ErrorMessage = "Page-22: Item 12. SHOULDERS- (g) UCS Details Table- Maximum seven digits before decimal and maximum three digits after decimal are allowed in UCS value achieved as per records of PIU (MPa)")]
        public decimal UCS_ACHIEVED_22 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-22: Item 12. SHOULDERS- (g) UCS Details Table- Please select Whether UCS value achieved on ground is acceptable (Yes / No) of row ")]
        //[RegularExpression(@"^[A-Za-z ]{0,1}$", ErrorMessage = "Page-22: Item 12. SHOULDERS- (g) UCS Details Table- Maximum one characters is allowed ")]
        [StringLength(1, ErrorMessage = "Page-22: Item 12. SHOULDERS- (g) UCS Details Table- Maximum one character is allowed in Whether UCS value achieved on ground is acceptable (Yes / No) of row ")]
        public string IS_UCS_ACCEPTABLE_22 { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int QM_USER_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public string IPADD { get; set; }

    }
}