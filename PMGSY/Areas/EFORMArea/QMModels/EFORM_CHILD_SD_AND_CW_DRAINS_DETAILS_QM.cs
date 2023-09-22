using PMGSY.Areas.EFORMArea.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.QMModels
{
    public class EFORM_CHILD_SD_AND_CW_DRAINS_DETAILS_QM
    {
        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int RowID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public string WORK_TYPE { get; set; }
        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-27: Item 17.SIDE DRAINS AND CATCH WATER DRAINS- III. Table- Please enter Location (RD) where side drains constructed From of row ")]
        [RegularExpression(@"^(?:\d{0,7}\.\d{1,3})$|^\d{0,7}$", ErrorMessage = "Page-27: Item 17.SIDE DRAINS AND CATCH WATER DRAINS- III. Table-Maximum seven digits before decimal and maximum three digits after decimal are allowed in Location (RD) where side drains constructed From of row ")]

        public decimal LOCATION_RD_FROM_27 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-27: Item 17.SIDE DRAINS AND CATCH WATER DRAINS- III. Table- Please enter Location (RD) where side drains constructed To of row ")]
        [RegularExpression(@"^(?:\d{0,7}\.\d{1,3})$|^\d{0,7}$", ErrorMessage = "Page-27: Item 17.SIDE DRAINS AND CATCH WATER DRAINS- III. Table-Maximum seven digits before decimal and maximum three digits after decimal are allowed in Location (RD) where side drains constructed To of row ")]

        public decimal LOCATION_RD_TO_27 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-27: Item 17.SIDE DRAINS AND CATCH WATER DRAINS- III. Table- Please enter Location (RD) of drain at which observation made of row ")]
        [RegularExpression(@"^(?:\d{0,7}\.\d{1,3})$|^\d{0,7}$", ErrorMessage = "Page-27: Item 17.SIDE DRAINS AND CATCH WATER DRAINS- III. Table-Maximum seven digits before decimal and maximum three digits after decimal are allowed in Location (RD) of drain at which observation made of row ")]

        public string LOCATION_RD_27 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-27: Item 17.SIDE DRAINS AND CATCH WATER DRAINS- III. Table- Please select Whether general quality of the side drains/catch-water drains is acceptable (Y / N) of row ")]
        [RegularExpression(@"^[A-Za-z]{0,1}$", ErrorMessage = "Page-27: Item 17.SIDE DRAINS AND CATCH WATER DRAINS- III. Table-Maximum one character is allowed in Whether general quality of the side drains/catch-water drains is acceptable (Y / N) of row ")]
        public string IS_GEN_QUAL_ACCEPTABLE_27 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-27: Item 17.SIDE DRAINS AND CATCH WATER DRAINS- III. Table- Please select Whether side drains are integrated to outfall. (Y / N) of row ")]
        [RegularExpression(@"^[A-Za-z]{0,1}$", ErrorMessage = "Page-27: Item 17.SIDE DRAINS AND CATCH WATER DRAINS- III. Table-Maximum one character is allowed in Whether side drains are integrated to outfall. (Y / N) of row ")]
        public string IS_SIDE_DRAINS_INTEGRATED_27 { get; set; }
 
    }
}