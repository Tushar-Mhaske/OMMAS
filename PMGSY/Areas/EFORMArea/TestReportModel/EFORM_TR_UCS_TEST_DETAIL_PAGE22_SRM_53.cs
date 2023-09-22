using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;
using System.ComponentModel.DataAnnotations;
using PMGSY.Areas.EFORMArea.Model;

namespace PMGSY.Areas.EFORMArea.TestReportModel
{
    public class EFORM_TR_UCS_TEST_DETAIL_PAGE22_SRM_53
    {
        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int ROW_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-22: NEW TECHNOLOGY TEST: Unconfined Compressive Strength (UCS) Test : Table: Please enter Chainage for chainage: ")]
        [RegularExpression(pattern: @"^(?:\d{0,7}\.\d{1,3})$|^\d{0,7}$", ErrorMessage = "Page-22: NEW TECHNOLOGY TEST: Unconfined Compressive Strength (UCS) Test :Table :  Negative number is not allowed, Maximum seven digits before decimal and maximum three digits after decimal are allowed in Chainage  for chainage: ")]

        public Nullable<decimal> CHAINAGE_22_2 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-22: NEW TECHNOLOGY TEST: Unconfined Compressive Strength (UCS) Test : Table: Please enter Weight of Sample(gm)  for chainage: ")]
        [RegularExpression(pattern: @"^(?:\d{0,7}\.\d{1,3})$|^\d{0,7}$", ErrorMessage = "Page-22: NEW TECHNOLOGY TEST: Unconfined Compressive Strength (UCS) Test :Table :  Negative number is not allowed, Maximum seven digits before decimal and maximum three digits after decimal are allowed in  Weight of Sample(gm)  for chainage: ")]

        public Nullable<decimal> SAMPLE_WT_22_2 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-22: NEW TECHNOLOGY TEST: Unconfined Compressive Strength (UCS) Test : Table: Please enter Density(gm / cc)  for chainage: ")]
        [RegularExpression(pattern: @"^(?:\d{0,7}\.\d{1,3})$|^\d{0,7}$", ErrorMessage = "Page-22: NEW TECHNOLOGY TEST: Unconfined Compressive Strength (UCS) Test :Table :  Negative number is not allowed, Maximum seven digits before decimal and maximum three digits after decimal are allowed in Density(gm / cc)  for chainage: ")]

        public Nullable<decimal> DENSITY_22_2 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-22: NEW TECHNOLOGY TEST: Unconfined Compressive Strength (UCS) Test : Table:Please select Date of Testing  for chainage: ")]
        [RegularExpression(pattern: @"^([0]?[0-9]|[12][0-9]|[3][01])/([0]?[1-9]|[1][0-2])/([0-9]{4})$", ErrorMessage = "Page-22: NEW TECHNOLOGY TEST: Unconfined Compressive Strength (UCS) Test : Table: Please Enter Valid date{in dd/mm/yyyy format} in Date of Testing  for chainage: ")]

        public Nullable<System.DateTime> TESTING_DATE_22_2 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-22: NEW TECHNOLOGY TEST: Unconfined Compressive Strength (UCS) Test : Table: Please enter Volume of sample(cc)  for chainage: ")]
        [RegularExpression(pattern: @"^(?:\d{0,7}\.\d{1,3})$|^\d{0,7}$", ErrorMessage = "Page-22: NEW TECHNOLOGY TEST: Unconfined Compressive Strength (UCS) Test :Table:  Negative number is not allowed, Maximum seven digits before decimal and maximum three digits after decimal are allowed in Volume of sample(cc)  for chainage: ")]

        public Nullable<decimal> SAMPLE_VOL_22_2 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-22: NEW TECHNOLOGY TEST: Unconfined Compressive Strength (UCS) Test :Table : Please enter Load in (KN)  for chainage: ")]
        [RegularExpression(pattern: @"^(?:\d{0,7}\.\d{1,3})$|^\d{0,7}$", ErrorMessage = "Page-22: NEW TECHNOLOGY TEST: Unconfined Compressive Strength (UCS) Test :Table:  Negative number is not allowed, Maximum seven digits before decimal and maximum three digits after decimal are allowed in Load in (KN)  for chainage: ")]

        public Nullable<decimal> LOAD_KN_22_2 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-22: NEW TECHNOLOGY TEST: Unconfined Compressive Strength (UCS) Test :Table : Please enter Compressive Strength (N / mm2)  for chainage: ")]
        [RegularExpression(pattern: @"^(?:\d{0,7}\.\d{1,3})$|^\d{0,7}$", ErrorMessage = "Page-22: NEW TECHNOLOGY TEST: Unconfined Compressive Strength (UCS) Test :Table CH 1:  Negative number is not allowed, Maximum seven digits before decimal and maximum three digits after decimal are allowed in Compressive Strength (N / mm2)  for chainage: ")]

        public Nullable<decimal> COMPR_STREANGTH_22_2 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-22: NEW TECHNOLOGY TEST: Unconfined Compressive Strength (UCS) Test :Table : Please select Whether confirms to the standard  for chainage: ")]
        [RegularExpression(pattern: @"^[01]{0,1}$", ErrorMessage = "Page-22: NEW TECHNOLOGY TEST: Unconfined Compressive Strength (UCS) Test: only one character{1/0} are allowed in Whether confirms to the standard  for chainage: ")]

        public string IS_STD_CONFIRM_22_2 { get; set; }
      
    }
}