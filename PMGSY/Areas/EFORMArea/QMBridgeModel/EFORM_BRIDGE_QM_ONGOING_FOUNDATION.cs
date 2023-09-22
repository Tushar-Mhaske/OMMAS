using PMGSY.Areas.EFORMArea.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.QMBridgeModel
{
    public class EFORM_BRIDGE_QM_ONGOING_FOUNDATION
    {
        //[FieldType(PropertyType = PDFFieldType.Skip)]
        //public int FOND_ON_ID { get; set; }

        //[FieldType(PropertyType = PDFFieldType.Skip)]
        //public int EFORM_ID { get; set; }

        //[FieldType(PropertyType = PDFFieldType.Skip)]
        //public int PR_ROAD_CODE { get; set; }

        //[FieldType(PropertyType = PDFFieldType.Skip)]
        //public int FOUNDATION_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-13: Item 4. FOUNDATION:- A) Ongoing- Please enter Abutments A1: ")]
        [RegularExpression(pattern: @"^(?:\d{0,3}\.\d{1,2})$|^\d{0,3}$", ErrorMessage = "Page-13: Item 4. FOUNDATION:- A) Ongoing- Please Enter Valid number{cant be greater than 999.99} in Abutments A1 ")]
        public Nullable<decimal> FOUNDATION_ABUTMENT1 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-13: Item 4. FOUNDATION:- A) Ongoing- Please enter Abutments A2: ")]
        [RegularExpression(pattern: @"^(?:\d{0,3}\.\d{1,2})$|^\d{0,3}$", ErrorMessage = "Page-13: Item 4. FOUNDATION:- A) Ongoing- Please Enter Valid number{cant be greater than 999.99} in Abutments A2 ")]
        public Nullable<decimal> FOUNDATION_ABUTMENT2 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-13: Item 4. FOUNDATION:- A) Ongoing- Please enter Piers P1: ")]
        [RegularExpression(pattern: @"^(?:\d{0,3}\.\d{1,2})$|^\d{0,3}$", ErrorMessage = "Page-13: Item 4. FOUNDATION:- A) Ongoing- Please Enter Valid number{cant be greater than 999.99} in Piers P1 ")]
        public Nullable<decimal> FOUNDATION_PIERS1 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-13: Item 4. FOUNDATION:- A) Ongoing- Please enter Piers P2: ")]
        [RegularExpression(pattern: @"^(?:\d{0,3}\.\d{1,2})$|^\d{0,3}$", ErrorMessage = "Page-13: Item 4. FOUNDATION:- A) Ongoing- Please Enter Valid number{cant be greater than 999.99} in Piers P2 ")]
        public Nullable<decimal> FOUNDATION_PIERS2 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-13: Item 4. FOUNDATION:- A) Ongoing- Please enter Piers P3: ")]
        [RegularExpression(pattern: @"^(?:\d{0,3}\.\d{1,2})$|^\d{0,3}$", ErrorMessage = "Page-13: Item 4. FOUNDATION:- A) Ongoing- Please Enter Valid number{cant be greater than 999.99} in Piers P3 ")]
        public Nullable<decimal> FOUNDATION_PIERS3 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-13: Item 4. FOUNDATION:- A) Ongoing- Please enter Piers P4 : ")]
        [RegularExpression(pattern: @"^(?:\d{0,3}\.\d{1,2})$|^\d{0,3}$", ErrorMessage = "Page-13: Item 4. FOUNDATION:- A) Ongoing- Please Enter Valid number{cant be greater than 999.99} in Piers P4 ")]
        public Nullable<decimal> FOUNDATION_PIERS4 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-13: Item 4. FOUNDATION:- A) Ongoing- Please enter Piers P5: ")]
        [RegularExpression(pattern: @"^(?:\d{0,3}\.\d{1,2})$|^\d{0,3}$", ErrorMessage = "Page-13: Item 4. FOUNDATION:- A) Ongoing- Please Enter Valid number{cant be greater than 999.99} in Piers P5 ")]
        public Nullable<decimal> FOUNDATION_PIERS5 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-13: Item 4. FOUNDATION:- A) Ongoing- Please enter Piers P6: ")]
        [RegularExpression(pattern: @"^(?:\d{0,3}\.\d{1,2})$|^\d{0,3}$", ErrorMessage = "Page-13: Item 4. FOUNDATION:- A) Ongoing- Please Enter Valid number{cant be greater than 999.99} in Piers P6 ")]
        public Nullable<decimal> FOUNDATION_PIERS6 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-13: Item 4. FOUNDATION:- A) Ongoing- Please enter Piers P7: ")]
        [RegularExpression(pattern: @"^(?:\d{0,3}\.\d{1,2})$|^\d{0,3}$", ErrorMessage = "Page-13: Item 4. FOUNDATION:- A) Ongoing- Please Enter Valid number{cant be greater than 999.99} in Piers P7 ")]
        public Nullable<decimal> FOUNDATION_PIERS7 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-13: Item 4. FOUNDATION:- A) Ongoing- Please enter Piers P8: ")]
        [RegularExpression(pattern: @"^(?:\d{0,3}\.\d{1,2})$|^\d{0,3}$", ErrorMessage = "Page-13: Item 4. FOUNDATION:- A) Ongoing- Please Enter Valid number{cant be greater than 999.99} in Piers P8 ")]
        public Nullable<decimal> FOUNDATION_PIERS8 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
      //  [Required(ErrorMessage = "Page-13: Item 4. FOUNDATION:- A) Ongoing- Please enter Piers P9: ")]
        [RegularExpression(pattern: @"^(?:\d{0,3}\.\d{1,2})$|^\d{0,3}$", ErrorMessage = "Page-13: Item 4. FOUNDATION:- A) Ongoing- Please Enter Valid number{cant be greater than 999.99} in Piers P9 ")]
        public Nullable<decimal> FOUNDATION_PIERS9 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-13: Item 4. FOUNDATION:- A) Ongoing- Please enter Returns R1: ")]
        [RegularExpression(pattern: @"^(?:\d{0,3}\.\d{1,2})$|^\d{0,3}$", ErrorMessage = "Page-13: Item 4. FOUNDATION:- A) Ongoing- Please Enter Valid number{cant be greater than 999.99} in Returns R1 ")]
        public Nullable<decimal> FOUNDATION_RETURN1 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
       // [Required(ErrorMessage = "Page-13: Item 4. FOUNDATION:- A) Ongoing- Please enter Returns R2: ")]
        [RegularExpression(pattern: @"^(?:\d{0,3}\.\d{1,2})$|^\d{0,3}$", ErrorMessage = "Page-13: Item 4. FOUNDATION:- A) Ongoing- Please Enter Valid number{cant be greater than 999.99} in Returns R2 ")]
        public Nullable<decimal> FOUNDATION_RETURN2 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-13: Item 4. FOUNDATION:- A) Ongoing- II. Please select Whether Cement and Steel tests (other than those provided by supplier), got conducted byindependent laboratories ")]
        [RegularExpression(pattern: @"^[A-Za-z]{0,3}$", ErrorMessage = "Page-13: Item 4. FOUNDATION:- A) Ongoing- II. Maximum one character is allowed in Whether Cement and Steel tests (other than those provided by supplier), got conducted byindependent laboratories ")]
        public string IS_TEST_CONDUCTED { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-13: Item 4. FOUNDATION:- A) Ongoing-  III. Please select Whether flushing of bore before and after placement of reinforcement done in case of boreduncased cast-insitu piles ")]
        [RegularExpression(pattern: @"^[A-Za-z]{0,3}$", ErrorMessage = "Page-13: Item 4. FOUNDATION:- A) Ongoing- III. Maximum one character is allowed in Whether flushing of bore before and after placement of reinforcement done in case of boreduncased cast-insitu piles ")]
        public string IS_REINFORCEMENT_DONE { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-14: Item 4. FOUNDATION:- A) Ongoing- IV. Please select Whether at least 600MM extra length of pile casted beyond cut off level, to be dismantled forlaitance effect ")]
        [RegularExpression(pattern: @"^[A-Za-z]{0,3}$", ErrorMessage = "Page-14: Item 4. FOUNDATION:- A) Ongoing- IV. Maximum one character is allowed in Whether at least 600MM extra length of pile casted beyond cut off level, to be dismantled forlaitance effect ")]
        public string IS_LAITANCE_EFFECT { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-14: Item 4. FOUNDATION:- A) Ongoing- V. Please select Whether bottom plugging in case of well foundation carried out by using tremie method only ")]
        [RegularExpression(pattern: @"^[A-Za-z]{0,3}$", ErrorMessage = "Page-14: Item 4. FOUNDATION:- A) Ongoing- V. Maximum one character is allowed in Whether bottom plugging in case of well foundation carried out by using tremie method only ")]
        public string IS_BOTTOM_PLUGING { get; set; }


        [FieldType(PropertyType = PDFFieldType.Skip)]
        public string IPADD { get; set; }

        //[FieldType(PropertyType = PDFFieldType.Skip)]
        //public int USER_ID { get; set; }

    }
}