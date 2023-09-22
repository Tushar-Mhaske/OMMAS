using PMGSY.Areas.EFORMArea.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.QMModels
{
    public class EFORM_QM_TEST_RESULT_VERIFICATION_DETAILS_Temp2_0
    {

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public string VerificationType { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int TEST_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int INFO_ID { get; set; }


        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int RowID { get; set; }



        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-8: Item 3. ATTENTION TO QUALITY- II. (c) Verification of test results table- Please enter Location RD of row ")]
        [RegularExpression(pattern: @"^(?:\d{0,7}\.\d{1,3})$|^\d{0,7}$", ErrorMessage = "Page-8: Item 3. ATTENTION TO QUALITY- II. (c) Verification of test results table- Please Enter Valid number{cant be greater than 9999999.99} in Location RD of row ")]
        public string ROAD_LOC { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-8: Item 3. ATTENTION TO QUALITY- II. (c) Verification of test results table- Please enter Name of Test of row ")]
        [StringLength(250, ErrorMessage = "Page-8: Item 3. ATTENTION TO QUALITY- II. (c) Verification of test results table- The length must be 250 character or less for Name of Test of row ")]
        public string TEST_NAME { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-8: Item 3. ATTENTION TO QUALITY- II. (c) Verification of test results table- Please enter Results of the test conducted by QM at a defined location.(Record Results) of row ")]
        [StringLength(250, ErrorMessage = "Page-8: Item 3. ATTENTION TO QUALITY- II. (c) Verification of test results table- The length must be 250 character or less for Results of the test conducted by QM at a defined location.(Record Results) of row ")]
        // [RegularExpression(pattern: @"^[A-Za-z]{0,1}$", ErrorMessage = "Page-8: Item 3. ATTENTION TO QUALITY- II. (c) Verification of test results table- Maximum one character is allowed in Results of the test conducted by QM at a defined location.(C / N) of row of row  ")]
        public string TEST_CONDUCTED_RESULT { get; set; }



        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-8: Item 3. ATTENTION TO QUALITY- II. (c) Verification of test results table- Please enter Test results as per QCR-I at the nearest location: Result of row ")]
        [StringLength(250, ErrorMessage = "Page-8: Item 3. ATTENTION TO QUALITY- II. (c) Verification of test results table- The length must be 250 character or less for Test results as per QCR-I at the nearest location: Result of row ")]
        // [RegularExpression(pattern: @"^[A-Za-z]{0,1}$", ErrorMessage = "Page-8: Item 3. ATTENTION TO QUALITY- II. (c) Verification of test results table- Maximum one character is allowed in Results of the test conducted previously by QM at defined location.(C / N) of row  ")]
        public string TEST_RESULT_PREVIOUS { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-8: Item 3. ATTENTION TO QUALITY- II. (c) Verification of test results table- Please enter Test results as per QCR-I at the nearest location: Page no. of row ")]
        [StringLength(250, ErrorMessage = "Page-8: Item 3. ATTENTION TO QUALITY- II. (c) Verification of test results table- The length must be 250 character or less for Test results as per QCR-I at the nearest location: Page no. of row ")]
        public string TEST_RESULT_QCR1 { get; set; }




        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-8: Item 3. ATTENTION TO QUALITY- II. (c) Verification of test results table- Please select Whether the test results recorded in QCR-I register and as conducted by QM are conforming/non-conforming (C / N) of row ")]
        [RegularExpression(pattern: @"^[A-Za-z]{0,1}$", ErrorMessage = "Page-8: Item 3. ATTENTION TO QUALITY- II. (c) Verification of test results table- Maximum one character is allowed in Whether the test results recorded in QCR-I register and as conducted by QM are conforming/non-conforming (C / N) of row ")]
        public string TEST_RESULT_CONFRM { get; set; }





        [FieldType(PropertyType = PDFFieldType.Skip)]
        public string IPADD { get; set; }


        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int PR_ROAD_CODE { get; set; }


        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int QM_USER_ID { get; set; }
    }
}