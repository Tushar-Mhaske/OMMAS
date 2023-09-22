using PMGSY.Areas.EFORMArea.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.TestReportModel
{
    public class EFORM_TR_TYPEB_SUMMARY_PAGE13_CEMENT_FDR_3
    {

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int TYPEB_SUMM_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int MAIN_ITEM_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int SUBITEM_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int TABLE_ID { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-13: Item 14.3. BASE COURSE: 3rd Layer - Cement Stabilised Subbase-Base (FDR):  Please enter Chainage for Chainage-III")]
        [RegularExpression(pattern: @"^(?:\d{0,7}\.\d{1,3})$|^\d{0,7}$", ErrorMessage = "Page-13: Item 14.3 BASE COURSE: 3rd Layer - Cement Stabilised Subbase-Base (FDR)  Please Enter Valid number{cant be greater than 9999999.999} for Chainage-III")]
        public Nullable<decimal> CHAINAGE_ID_13_3 { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-13: Item 14.3. BASE COURSE: 3rd Layer - Cement Stabilised Subbase-Base (FDR):  -Please enter Weight of Sample Taken (gm) for Chainage-III")]
        [RegularExpression(pattern: @"^(?:\d{0,12}\.\d{1,3})$|^\d{0,12}$", ErrorMessage = "Page-13: Item 14.3 BASE COURSE: 3rd Layer - Cement Stabilised Subbase-Base (FDR)  Please Enter Valid number{cant be greater than 999999999999.999} in Weight of Sample Taken (gm) for Chainage-III")]
        public Nullable<decimal> WEIGHT_SAMPLE_TAKEN_13_3 { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-13: Item 14.3. BASE COURSE: 3rd Layer - Cement Stabilised Subbase-Base (FDR):  -Please select Date of Testing for Chainage-III")]
        [RegularExpression(pattern: @"^([0]?[0-9]|[12][0-9]|[3][01])/([0]?[1-9]|[1][0-2])/([0-9]{4})$", ErrorMessage = "Page-13: Item 14.3 BASE COURSE: 3rd Layer - Cement Stabilised Subbase-Base (FDR)  Please Enter Valid date{in dd/mm/yyyy format} in Date of Testing for Chainage-III")]
        public Nullable<System.DateTime> TESTING_DATE_13_3 { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-13: Item 14.3. BASE COURSE: 3rd Layer - Cement Stabilised Subbase-Base (FDR):  -Please enter Material for Chainage-III")]
        [StringLength(50, ErrorMessage = "Page-13: Item 14.3 BASE COURSE: 3rd Layer - Cement Stabilised Subbase-Base (FDR) The length must be 50 character or less for GSB Grading for Chainage-III")]
        public string GSB_GRADING_13_3 { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-13: Item 14.3. BASE COURSE: 3rd Layer - Cement Stabilised Subbase-Base (FDR):   -Remark is required for Chainage-III. Please click on Calculate button.")]
        [StringLength(2000, ErrorMessage = "Page-13: Item 14.3 BASE COURSE: 3rd Layer - Cement Stabilised Subbase-Base (FDR) The length must be 2000 character or less for Remark for Chainage-III")]
        public string REMARK_13_3 { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-13: Item 14.3. BASE COURSE: 3rd Layer - Cement Stabilised Subbase-Base (FDR):   -Please enter Comment for Chainage-III")]
        [StringLength(2000, ErrorMessage = "Page-13: Item 14.3 BASE COURSE: 3rd Layer - Cement Stabilised Subbase-Base (FDR) The length must be 2000 character or less for Comment for Chainage-III")]
        public string COMMENT_13_3 { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-13: Item 14.3. BASE COURSE: 3rd Layer - Cement Stabilised Subbase-Base (FDR):   -Please enter Tested By: (Name of Head of PIU) for Chainage-III")]
        [StringLength(2000, ErrorMessage = "Page-13: Item 14.3 BASE COURSE: 3rd Layer - Cement Stabilised Subbase-Base (FDR) The length must be 2000 character or less for Tested By: (Name of Head of PIU) for Chainage-III")]
        public string TESTED_BY_PIU { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-13: Item 14.3. BASE COURSE: 3rd Layer - Cement Stabilised Subbase-Base (FDR):   -Please enter Test conducted in my presence for Chainage-III")]
        [StringLength(2000, ErrorMessage = "Page-13: Item 14.3 BASE COURSE: 3rd Layer - Cement Stabilised Subbase-Base (FDR) The length must be 2000 character or less for Test conducted in my presence for Chainage-III")]
        public string TEST_CONDUCTED_IN_PRESENCE { get; set; }


        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-13: Item 14.3. BASE COURSE: 3rd Layer - Cement Stabilised Subbase-Base (FDR):   -Confirms to the prescribed limit is required for Chainage-III. Please click on Calculate button.")]
        [StringLength(3, ErrorMessage = "Page-13: Item 14.3 BASE COURSE: 3rd Layer - Cement Stabilised Subbase-Base (FDR) The length must be 3 character or less for confirms to the prescribed limit for Chainage-III")]
        public string CON_PRESCRIBED_LIMIT_13_3 { get; set; }


    }
}