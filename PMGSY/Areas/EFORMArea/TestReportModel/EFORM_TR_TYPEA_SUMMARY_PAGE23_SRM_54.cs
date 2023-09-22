using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;
using System.ComponentModel.DataAnnotations;
using PMGSY.Areas.EFORMArea.Model;

namespace PMGSY.Areas.EFORMArea.TestReportModel
{
    public class EFORM_TR_TYPEA_SUMMARY_PAGE23_SRM_54
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
        [Required(ErrorMessage = "Page-23: NEW TECHNOLOGY TEST: Rebound Hammer Test : Please enter Age of sample(M)")]
        [RegularExpression(pattern: @"^(?:\d{0,7}\.\d{1,3})$|^\d{0,7}$", ErrorMessage = "Page-23: NEW TECHNOLOGY TEST: Rebound Hammer Test:  Maximum seven digits before decimal and maximum three digits after decimal are allowed in Age of sample(M) ")]

        public Nullable<decimal> SAMPLE_AGE_23_1 { get; set; }


        //[FieldType(PropertyType = PDFFieldType.ComboBox)]
        //[Required(ErrorMessage = "Page-23: NEW TECHNOLOGY TEST: Rebound Hammer Test :Please select Layer from dropdown")]
        //[RegularExpression(pattern: @"^[12345 ]{0,1}$", ErrorMessage = "Page-23: NEW TECHNOLOGY TEST: Rebound Hammer Test : Only {1/2/3/4/5} is allowed in Layer dropdown")]

        //public int Layer_23_1 { get; set; }

        [FieldType(PropertyType = PDFFieldType.ComboBox)]
        [Required(ErrorMessage = "Page-23: NEW TECHNOLOGY TEST: Rebound Hammer Test :Please select CC Pavement Type:CH1 from dropdown")]
        [RegularExpression(pattern: @"^([1]){0,1}$|^([6]){0,1}$|^([7]){0,1}$|^([8]){0,1}$|^([9]){0,1}$|^([1][0]){0,1}$", ErrorMessage = "Page-23: NEW TECHNOLOGY TEST: Rebound Hammer Test : Only {1/6/7/8/9/10} is allowed in CC Pavement Type:CH1 dropdown")]

        public int? CC_PAVEMENT_TYPE_CH1_23_1 { get; set; }

        [FieldType(PropertyType = PDFFieldType.ComboBox)]
        [Required(ErrorMessage = "Page-23: NEW TECHNOLOGY TEST: Rebound Hammer Test :Please select CC Pavement Type:CH2 from dropdown")]
        [RegularExpression(pattern: @"^([1]){0,1}$|^([6]){0,1}$|^([7]){0,1}$|^([8]){0,1}$|^([9]){0,1}$|^([1][0]){0,1}$", ErrorMessage = "Page-23: NEW TECHNOLOGY TEST: Rebound Hammer Test : Only {1/6/7/8/9/10} is allowed in CC Pavement Type:CH2 dropdown")]

        public int? CC_PAVEMENT_TYPE_CH2_23_1 { get; set; }

        [FieldType(PropertyType = PDFFieldType.ComboBox)]
        [Required(ErrorMessage = "Page-23: NEW TECHNOLOGY TEST: Rebound Hammer Test :Please select CC Pavement Type:CH3 from dropdown")]
        [RegularExpression(pattern: @"^([1]){0,1}$|^([6]){0,1}$|^([7]){0,1}$|^([8]){0,1}$|^([9]){0,1}$|^([1][0]){0,1}$", ErrorMessage = "Page-23: NEW TECHNOLOGY TEST: Rebound Hammer Test : Only {1/6/7/8/9/10} is allowed in CC Pavement Type:CH3 dropdown")]

        public int? CC_PAVEMENT_TYPE_CH3_23_1 { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-23: NEW TECHNOLOGY TEST: Rebound Hammer Test : Please enter Design Strength")]
        [RegularExpression(pattern: @"^(?:\d{0,7}\.\d{1,3})$|^\d{0,7}$", ErrorMessage = "Page-23: NEW TECHNOLOGY TEST: Rebound Hammer Test:  Maximum seven digits before decimal and maximum three digits after decimal are allowed in Design Strength ")]
        public Nullable<decimal> DESIGN_STRENGTH_23_1 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-23: NEW TECHNOLOGY TEST: Rebound Hammer Test :Please select Date of Testing")]
        [RegularExpression(pattern: @"^([0]?[0-9]|[12][0-9]|[3][01])/([0]?[1-9]|[1][0-2])/([0-9]{4})$", ErrorMessage = "Page-23: NEW TECHNOLOGY TEST: Rebound Hammer Test : Please Enter Valid date{in dd/mm/yyyy format} in Date of Testing ")]

        public Nullable<System.DateTime> TESTING_DATE_23_1 { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
       [Required(ErrorMessage = "Page-23: NEW TECHNOLOGY TEST: Rebound Hammer Test : Remark is required.")]
        [StringLength(2000, ErrorMessage = "Page-23: NEW TECHNOLOGY TEST: Rebound Hammer Test : The length must be 2000 character or less for Remark")]
        public string REMARK_23_1 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-23: NEW TECHNOLOGY TEST: Rebound Hammer Test : Please enter comment.")]
        [StringLength(2000, ErrorMessage = "Page-23: NEW TECHNOLOGY TEST: Rebound Hammer Test : The length must be 2000 character or less for Comment")]

        public string COMMENT_23_1 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-23: NEW TECHNOLOGY TEST: Rebound Hammer Test :  Please enter Tested By: (Name of Head of PIU)")]
        [StringLength(2000, ErrorMessage = "Page-23: NEW TECHNOLOGY TEST: Rebound Hammer Test : The length must be 2000 character or less for Tested By: (Name of Head of PIU)")]

        public string TESTED_BY_PIU { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-23: NEW TECHNOLOGY TEST: Rebound Hammer Test :  Please enter Test conducted in my presence")]
        [StringLength(3000, ErrorMessage = "Page-23: NEW TECHNOLOGY TEST: Rebound Hammer Test : The length must be 2000 character or less for Test conducted in my presence")]
        public string TEST_CONDUCTED_IN_PRESENCE { get; set; }
    }
}