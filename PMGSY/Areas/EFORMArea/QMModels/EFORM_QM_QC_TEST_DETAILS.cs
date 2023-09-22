using PMGSY.Areas.EFORMArea.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.QMModels
{
    public class EFORM_QM_QC_TEST_DETAILS
    {
        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int WORK_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int INFO_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int QM_USER_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public short ITEM_ID { get; set; }



        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-8: Item 3. ATTENTION TO QUALITY- II (a) Adequacy of quality control tests Table- Please enter Quantity as per DPR for ")]
        [RegularExpression(pattern: @"^(?:\d{0,6}\.\d{1,3})$|^\d{0,6}$", ErrorMessage = "Page-8: Item 3. ATTENTION TO QUALITY- II (a) Adequacy of quality control tests Table- Please Enter Valid number{cant be greater than 999999.99} in Quantity as per DPR for ")]
        public Nullable<decimal> DPR_QUANTITY { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-8: Item 3. ATTENTION TO QUALITY- II (a) Adequacy of quality control tests Table- Please enter Quantity executed for ")]
        [RegularExpression(pattern: @"^(?:\d{0,6}\.\d{1,3})$|^\d{0,6}$", ErrorMessage = "Page-8: Item 3. ATTENTION TO QUALITY- II (a) Adequacy of quality control tests Table- Please Enter Valid number{cant be greater than 999999.99} in Quantity executed for ")]
        public Nullable<decimal> EXECUTED_QUANTITY { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-8: Item 3. ATTENTION TO QUALITY- II (a) Adequacy of quality control tests Table- Please enter Name of the test for ")]
        [StringLength(250, ErrorMessage = "Page-8: Item 3. ATTENTION TO QUALITY- II (a) Adequacy of quality control tests Table- The length must be 250 character or less in Name of the test for ")]
        public string TEST_NAME { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-8: Item 3. ATTENTION TO QUALITY- II (a) Adequacy of quality control tests Table- Please enter Number of tests required for ")]
        [RegularExpression(pattern: @"^\d{0,9}$", ErrorMessage = "Page-8: Item 3. ATTENTION TO QUALITY- II (a) Adequacy of quality control tests Table- Please Enter Valid number{cant be greater than 999999999} in Number of tests required for ")]

        public Nullable<int> REQD_TEST_NUMBER { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-8: Item 3. ATTENTION TO QUALITY- II (a) Adequacy of quality control tests Table- Please enter Number of tests actually conducted for  ")]
        [RegularExpression(pattern: @"^\d{0,9}$", ErrorMessage = "Page-8: Item 3. ATTENTION TO QUALITY- II (a) Adequacy of quality control tests Table- Please Enter Valid number{cant be greater than 999999999} in Number of tests actually conducted for ")]

        public Nullable<int> CONDUCTED_TEST_NUMBER { get; set; }



        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-8: Item 3. ATTENTION TO QUALITY- II (a) Adequacy of quality control tests Table- Please select Testing adequate for ")]
        [RegularExpression(pattern: @"^[A-Za-z]{0,1}$", ErrorMessage = "Page-8: Item 3. ATTENTION TO QUALITY- II (a) Adequacy of quality control tests Table- Maximum one character is allowed in Testing adequate for ")]
        public string IS_TESTING_ADEQUATE { get; set; }




        [FieldType(PropertyType = PDFFieldType.Skip)]
        public string IPADD { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int PR_ROAD_CODE { get; set; }



    }
}