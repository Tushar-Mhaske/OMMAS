using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;
using System.ComponentModel.DataAnnotations;
using PMGSY.Areas.EFORMArea.Model;

namespace PMGSY.Areas.EFORMArea.TestReportModel
{
    public class EFORM_TR_TYPEA_SUMMARY_PAGE22_SRM_53
    {
        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int TYPEA_SUMM_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int MAIN_ITEM_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int SUBITEM_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int TABLE_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-22: NEW TECHNOLOGY TEST: Unconfined Compressive Strength (UCS) Test : Please enter Age of sample(M)")]
        [RegularExpression(pattern: @"^(?:\d{0,7}\.\d{1,3})$|^\d{0,7}$", ErrorMessage = "Page-22: NEW TECHNOLOGY TEST: Unconfined Compressive Strength (UCS) Test:  Maximum seven digits before decimal and maximum three digits after decimal are allowed in Age of sample(M) ")]

        public Nullable<decimal> SAMPLE_AGE_22_2 { get; set; }


        [FieldType(PropertyType = PDFFieldType.ComboBox)]
        [Required(ErrorMessage = "Page-22: NEW TECHNOLOGY TEST: Unconfined Compressive Strength (UCS) Test :Please select Layer from dropdown")]
        [RegularExpression(pattern: @"^[12345 ]{0,1}$", ErrorMessage = "Page-22: NEW TECHNOLOGY TEST: Unconfined Compressive Strength (UCS) Test : Only {1/2/3/4/5} is allowed in Layer dropdown")]

        public int Layer_22_2 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-22: NEW TECHNOLOGY TEST: Unconfined Compressive Strength (UCS) Test : Please enter UCS as per DPR")]
        [RegularExpression(pattern: @"^(?:\d{0,7}\.\d{1,3})$|^\d{0,7}$", ErrorMessage = "Page-22: NEW TECHNOLOGY TEST: Unconfined Compressive Strength (UCS) Test:  Maximum seven digits before decimal and maximum three digits after decimal are allowed in UCS as per DPR ")]
        public Nullable<decimal> UCS_DPR_22_2 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-22: NEW TECHNOLOGY TEST: Unconfined Compressive Strength (UCS) Test :Please select Date of Testing")]
        [RegularExpression(pattern: @"^([0]?[0-9]|[12][0-9]|[3][01])/([0]?[1-9]|[1][0-2])/([0-9]{4})$", ErrorMessage = "Page-22: NEW TECHNOLOGY TEST: Unconfined Compressive Strength (UCS) Test : Please Enter Valid date{in dd/mm/yyyy format} in Date of Testing ")]

        public Nullable<System.DateTime> TESTING_DATE_22_2 { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
      [Required(ErrorMessage = "Page-22: NEW TECHNOLOGY TEST: Unconfined Compressive Strength (UCS) Test : Remark is required.")]
        [StringLength(2000, ErrorMessage = "Page-22: NEW TECHNOLOGY TEST: Unconfined Compressive Strength (UCS) Test : The length must be 2000 character or less for Remark")]
        public string REMARK_22_2 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-22: NEW TECHNOLOGY TEST: Unconfined Compressive Strength (UCS) Test : Please enter comment.")]
        [StringLength(2000, ErrorMessage = "Page-22: NEW TECHNOLOGY TEST: Unconfined Compressive Strength (UCS) Test : The length must be 2000 character or less for Comment")]

        public string COMMENT_22_2 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-22: NEW TECHNOLOGY TEST: Unconfined Compressive Strength (UCS) Test :  Please enter Tested By: (Name of Head of PIU)")]
        [StringLength(2000, ErrorMessage = "Page-22: NEW TECHNOLOGY TEST: Unconfined Compressive Strength (UCS) Test : The length must be 2000 character or less for Tested By: (Name of Head of PIU)")]

        public string TESTED_BY_PIU { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-22: NEW TECHNOLOGY TEST: Unconfined Compressive Strength (UCS) Test :  Please enter Test conducted in my presence")]
        [StringLength(3000, ErrorMessage = "Page-22: NEW TECHNOLOGY TEST: Unconfined Compressive Strength (UCS) Test : The length must be 2000 character or less for Test conducted in my presence")]
        public string TEST_CONDUCTED_IN_PRESENCE { get; set; }
    }
}