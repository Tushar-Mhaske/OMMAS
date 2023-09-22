using PMGSY.Areas.EFORMArea.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.QMModels
{
    public class EFORM_QM_CHILD_EARTHWORK_SUBGRADE_CBR_DETAILS
    {
        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int CBR_DETAIL_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int TECH_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int EFORM_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int IMS_PR_ROAD_CODE { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [RegularExpression(@"^[0-9]{0,7}(?:\.[0-9]{1,3})?$", ErrorMessage = "Page-11: Item 5.EARTHWORK & SUB GRADE- I. (g) vii) Maximum seven digits before decimal and maximum three digits after decimal are allowed in Location of new technology section RD (km) From of row ")]
        [Required(ErrorMessage = "Page-11: Item 5.EARTHWORK & SUB GRADE- I. (g) vii) Please enter Location of new technology section RD (km) From of row  ")]
        public Nullable<decimal> LOCATION_RD_FROM_11 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]

        [RegularExpression(@"^[0-9]{0,7}(?:\.[0-9]{1,3})?$", ErrorMessage = "Page-11: Item 5.EARTHWORK & SUB GRADE- I. (g) vii) Maximum seven digits before decimal and maximum three digits after decimal are allowed Location of new technology section RD (km) To of row ")]
        [Required(ErrorMessage = "Page-11: Item 5.EARTHWORK & SUB GRADE- I. (g) vii) Please enter Location of new technology section RD (km) To of row ")]
        public Nullable<decimal> LOCATION_RD_TO_11 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]

        [RegularExpression(@"^[0-9]{0,7}(?:\.[0-9]{1,3})?$", ErrorMessage = "Page-11: Item 5.EARTHWORK & SUB GRADE- I. (g) vii) Maximum seven digits before decimal and maximum three digits after decimal are allowed in CBR as per mix design (%) of row ")]
        [Required(ErrorMessage = "Page-11: Item 5.EARTHWORK & SUB GRADE- I. (g) vii) Please enter CBR as per mix design (%) of row  ")]
        public Nullable<decimal> CBR_ASPER_MIX_DESIGN_11 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]

        [RegularExpression(@"^[0-9]{0,7}(?:\.[0-9]{1,3})?$", ErrorMessage = "Page-11: Item 5.EARTHWORK & SUB GRADE- I. (g) vii) Maximum seven digits before decimal and maximum three digits after decimal are allowed in CBR achieved as per records of PIU (%) of row ")]
        [Required(ErrorMessage = "Page-11: Item 5.EARTHWORK & SUB GRADE- I. (g) vii) Please enter CBR achieved as per records of PIU (%) of row ")]
        public Nullable<decimal> CBR_ASPER_PIU_11 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [RegularExpression(@"^[A-Za-z ]{0,1}$", ErrorMessage = "Page-11 Item 5.EARTHWORK & SUB GRADE- I. (g) vii) Maximum one character is allowed in Whether CBR achieved on ground is acceptable of row ")]
        [Required(ErrorMessage = "Page-11: Item 5.EARTHWORK & SUB GRADE- I. (g) vii) Please select Whether CBR achieved on ground is acceptable of row ")]
        public string IS_CBR_ACCEPTABLE_11 { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int QM_USER_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int RowId { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public string IPADD { get; set; }
    }
}