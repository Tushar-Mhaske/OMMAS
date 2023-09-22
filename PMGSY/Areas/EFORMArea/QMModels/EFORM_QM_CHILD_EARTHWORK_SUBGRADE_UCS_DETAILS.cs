using PMGSY.Areas.EFORMArea.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.QMModels
{
    public class EFORM_QM_CHILD_EARTHWORK_SUBGRADE_UCS_DETAILS
    {
        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int UCS_DETAIL_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int TECH_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int EFORM_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int IMS_PR_ROAD_CODE { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-10: Item 5. EARTHWORK & SUB GRADE- I. UCS Details Table- Please enter Location of new technology section RD (km) From of row ")]
        [RegularExpression(@"^[0-9]{0,7}(?:\.[0-9]{1,3})?$", ErrorMessage = "Page-10: Item 5. EARTHWORK & SUB GRADE- I. UCS Details Table- Maximum seven digits before decimal and maximum three digits after decimal are allowed in Location of new technology section RD (km) From of row ")]
        public Nullable<decimal> LOCATION_RD_FROM_10 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-10: Item 5. EARTHWORK & SUB GRADE- I. UCS Details Table- Please enter of row ")]
        [RegularExpression(@"^[0-9]{0,7}(?:\.[0-9]{1,3})?$", ErrorMessage = "Page-10: Item 5. EARTHWORK & SUB GRADE- I. UCS Details Table- Maximum seven digits before decimal and maximum three digits after decimal are allowed Location of new technology section RD (km) To of row ")]
        public Nullable<decimal> LOCATION_RD_TO_10 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-10: Item 5. EARTHWORK & SUB GRADE- I. UCS Details Table- Please enter UCS value as per mix design (MPa) of row ")]
        [RegularExpression(@"^[0-9]{0,3}(?:\.[0-9]{1,2})?$", ErrorMessage = "Page-10: Item 5. EARTHWORK & SUB GRADE- I. UCS Details Table- Maximum three digits before decimal and maximum two digits after decimal are allowed UCS value as per mix design (MPa) of row ")]
        public Nullable<decimal> UCS_ASPER_MIX_DESIGN_10 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-10: Item 5. EARTHWORK & SUB GRADE- I. UCS Details Table- Please enter UCS value achieved as per records of PIU (MPa) of row  ")]
        [RegularExpression(@"^[0-9]{0,3}(?:\.[0-9]{1,2})?$", ErrorMessage = "Page-10: Item 5. EARTHWORK & SUB GRADE- I. UCS Details Table- Maximum three digits before decimal and maximum two digits after decimal are allowed in UCS value achieved as per records of PIU (MPa) of row ")]
        public Nullable<decimal> UCS_ASPER_PIU_10 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-10: Item 5. EARTHWORK & SUB GRADE- I. UCS Details Table- Please select Whether UCS value achieved on ground is acceptable of row  ")]
        [StringLength(1,ErrorMessage = "Page-11 Item 5. EARTHWORK & SUB GRADE- I. UCS Details Table- Maximum one character is allowed in Whether UCS value achieved on ground is acceptable of row ")]
        public string IS_UCS_ACCEPTABLE_10 { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int QM_USER_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int RowId { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public string IPADD { get; set; }
    }
}