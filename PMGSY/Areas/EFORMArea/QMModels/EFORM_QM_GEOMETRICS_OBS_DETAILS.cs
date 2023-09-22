using PMGSY.Areas.EFORMArea.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.QMModels
{
    public class EFORM_QM_GEOMETRICS_OBS_DETAILS
    {
        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int OBS_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int RowID { get; set; }




        [FieldType(PropertyType = PDFFieldType.Skip)]
        public string GEOMETRIC_TYPE { get; set; }


        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int EFORM_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int QM_USER_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int INFO_ID { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-9: Item 4. GEOMETRICS- Please enter Location RD ")]
        [RegularExpression(pattern: @"^(?:\d{0,7}\.\d{1,3})$|^\d{0,7}$", ErrorMessage = "Page-9: Item 4. GEOMETRICS- Please Enter Valid number{cant be greater than 9999999.99} in Location RD ")]
        public Nullable<decimal> ROAD_LOC { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-9: Item 4. GEOMETRICS- I. Please enter 4(I)a Roadway width(m) As per DPR ")]
        [RegularExpression(pattern: @"^(?:\d{0,7}\.\d{1,3})$|^\d{0,7}$", ErrorMessage = "Page-9: Item 4. GEOMETRICS- I. Please Enter Valid number{cant be greater than 9999999.99} in 4(I)a Roadway width(m) As per DPR ")]
        public Nullable<decimal> C4IA_ROAD_WIDTH_DPR { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-9: Item 4. GEOMETRICS- I. Please enter 4(I)a Roadway width(m) Actual at site ")]
        [RegularExpression(pattern: @"^(?:\d{0,7}\.\d{1,3})$|^\d{0,7}$", ErrorMessage = "Page-9: Item 4. GEOMETRICS- I. Please Enter Valid number{cant be greater than 9999999.99} in 4(I)a Roadway width(m) Actual at site ")]
        public Nullable<decimal> C4IA_ROAD_WIDTH_ACTUAL { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-9: Item 4. GEOMETRICS- I. Please select 4(I)a Roadway width(m) Grade (S/U) ")]
        [RegularExpression(pattern: @"^[A-Za-z]{0,1}$", ErrorMessage = "Page-9: Item 4. GEOMETRICS- I. Maximum one character is allowed in 4(I)a Roadway width(m) Grade (S/U) ")]
        public string C4IA_ROAD_WIDTH_GRADE { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        //  [Required(ErrorMessage = "Page-9: Item 4. GEOMETRICS- I. Please enter 4(I)b Carriageway width(m) As per DPR ")]
        [RegularExpression(pattern: @"^(?:\d{0,7}\.\d{1,3})$|^\d{0,7}$", ErrorMessage = "Page-9: Item 4. GEOMETRICS- I. Please Enter Valid number{cant be greater than 9999999.99} in 4(I)b Carriageway width(m) As per DPR ")]
        public Nullable<decimal> C4IB_CARRIAGE_WIDTH_DPR { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        //  [Required(ErrorMessage = "Page-9: Item 4. GEOMETRICS- I. Please enter 4(I)b Carriageway width(m) Actual at site ")]
        [RegularExpression(pattern: @"^(?:\d{0,7}\.\d{1,3})$|^\d{0,7}$", ErrorMessage = "Page-9: Item 4. GEOMETRICS- I. Please Enter Valid number{cant be greater than 9999999.99} in 4(I)b Carriageway width(m) Actual at site ")]
        public Nullable<decimal> C4IB_CARRIAGE_WIDTH_ACTUAL { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        //  [Required(ErrorMessage = "Page-9: Item 4. GEOMETRICS- I. Please select 4(I)b Carriageway width(m) Grade (S/U) ")]
        [RegularExpression(pattern: @"^[A-Za-z]{0,1}$", ErrorMessage = "Page-9: Item 4. GEOMETRICS- I. Maximum one character is allowed in 4(I)b Carriageway width(m) Grade (S/U) ")]
        public string C4IB_CARRIAGE_WIDTH_GRADE { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        // [Required(ErrorMessage = "Page-9: Item 4. GEOMETRICS- I. Please enter 4(I)c Camber(%) As per DPR ")]
        [RegularExpression(pattern: @"^(?:\d{0,7}\.\d{1,3})$|^\d{0,7}$", ErrorMessage = "Page-9: Item 4. GEOMETRICS- I. Please Enter Valid number{cant be greater than 9999999.99} in 4(I)c Camber(%) As per DPR ")]
        public Nullable<decimal> C4IC_CAMBER_PER_DPR { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        //   [Required(ErrorMessage = "Page-9: Item 4. GEOMETRICS- I. Please enter 4(I)c Camber(%) Actual at site  ")]
        [RegularExpression(pattern: @"^(?:\d{0,7}\.\d{1,3})$|^\d{0,7}$", ErrorMessage = "Page-9: Item 4. GEOMETRICS- I. Please Enter Valid number{cant be greater than 9999999.99} in 4(I)c Camber(%) Actual at site ")]
        public Nullable<decimal> C4IC_CAMBER_PER_ACTUAL { get; set; }



        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        // [Required(ErrorMessage = "Page-9: Item 4. GEOMETRICS- I. Please select 4(I)c Camber(%)  Grade (S/U)")]
        [RegularExpression(pattern: @"^[A-Za-z]{0,1}$", ErrorMessage = "Page-9: Item 4. GEOMETRICS- I. Maximum one character is allowed  ")]
        public string C4IC_CAMBER_PER_GRADE { get; set; }



        [FieldType(PropertyType = PDFFieldType.TextBox)]
        // [Required(ErrorMessage = "Page-9: Please enter  ")]
        [RegularExpression(pattern: @"^(?:\d{0,7}\.\d{1,3})$|^\d{0,7}$", ErrorMessage = "Page-9: Item 4. GEOMETRICS- II. Please Enter Valid number{cant be greater than 9999999.99} in 4 (II)a Super elevation(%) As per DPR")]
        public Nullable<decimal> C4IIA_ELEVATION_PER_DPR { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        // [Required(ErrorMessage = "Page-9: Please enter  ")]
        [RegularExpression(pattern: @"^(?:\d{0,7}\.\d{1,3})$|^\d{0,7}$", ErrorMessage = "Page-9: Item 4. GEOMETRICS- II. Please Enter Valid number{cant be greater than 9999999.99} in 4 (II)a Super elevation(%) Actual at site ")]
        public Nullable<decimal> C4IIA_ELEVATION_PER_ACTUAL { get; set; }


        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        // [Required(ErrorMessage = "Page-9: Please select  ")]
        [RegularExpression(pattern: @"^[A-Za-z]{0,1}$", ErrorMessage = "Page-9: Item 4. GEOMETRICS- II. Maximum one character is allowed 4 (II)a Super elevation(%) Grade (S / U) ")]
        public string C4IIA_ELEVATION_PER_GRADE { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        //[Required(ErrorMessage = "Page-9: Please enter  ")]
        [RegularExpression(pattern: @"^(?:\d{0,7}\.\d{1,3})$|^\d{0,7}$", ErrorMessage = "Page-9: Item 4. GEOMETRICS- II. Please Enter Valid number{cant be greater than 9999999.99} in 4(II)b Extra widening provided(m) As per DPR ")]
        public Nullable<decimal> C4IIB_EXTRA_WIDENING_DPR { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        // [Required(ErrorMessage = "Page-9: Please enter  ")]
        [RegularExpression(pattern: @"^(?:\d{0,7}\.\d{1,3})$|^\d{0,7}$", ErrorMessage = "Page-9: Item 4. GEOMETRICS- II. Please Enter Valid number{cant be greater than 9999999.99} in 4(II)b Extra widening provided(m) Actual at site ")]
        public Nullable<decimal> C4IIB_EXTRA_WIDENING_ACTUAL { get; set; }


        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        // [Required(ErrorMessage = "Page-9: Please select  ")]
        [RegularExpression(pattern: @"^[A-Za-z]{0,1}$", ErrorMessage = "Page-9: Item 4. GEOMETRICS- II. Maximum one character is allowed in 4(II)b Extra widening provided(m) Grade (S / U) ")]
        public string C4IIB_EXTRA_WIDENING_GRADE { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-10: Item 4. GEOMETRICS- III. Please enter Ref. between RD From ")]
        [RegularExpression(pattern: @"^(?:\d{0,7}\.\d{1,3})$|^\d{0,7}$", ErrorMessage = "Page-10: Item 4. GEOMETRICS- III. Please Enter Valid number{cant be greater than 9999999.99} in Ref. between RD From ")]
        public Nullable<decimal> ROAD_FROM { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-10: Item 4. GEOMETRICS- III. Please enter Ref. between RD To ")]
        [RegularExpression(pattern: @"^(?:\d{0,7}\.\d{1,3})$|^\d{0,7}$", ErrorMessage = "Page-10: Item 4. GEOMETRICS- III. Please Enter Valid number{cant be greater than 9999999.99} in Ref. between RD To  ")]
        public Nullable<decimal> ROAD_TO { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-10: Item 4. GEOMETRICS- III. Please enter 4(III)a Longitudinal gradient(%) As per DPR")]
        [RegularExpression(pattern: @"^(?:\d{0,7}\.\d{1,3})$|^\d{0,7}$", ErrorMessage = "Page-9: Item 4. GEOMETRICS- III. Please Enter Valid number{cant be greater than 9999999.99} in  4(III)a Longitudinal gradient(%) As per DPR ")]
        public Nullable<decimal> C4IIIA_LONG_GRAD_PER_DPR { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-10: Item 4. GEOMETRICS- III. Please enter 4(III)a Longitudinal gradient(%) Actual at site")]
        [RegularExpression(pattern: @"^(?:\d{0,7}\.\d{1,3})$|^\d{0,7}$", ErrorMessage = "Page-9: Item 4. GEOMETRICS- III. Please Enter Valid number{cant be greater than 9999999.99} in 4(III)a Longitudinal gradient(%) Actual at site ")]
        public Nullable<decimal> C4IIIA_LONG_GRAD_PER_ACTUAL { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-10: Item 4. GEOMETRICS- III. Please select 4(III)a Longitudinal gradient(%) Grade(S/U) ")]
        [RegularExpression(pattern: @"^[A-Za-z]{0,1}$", ErrorMessage = "Page-9: Item 4. GEOMETRICS- III. Maximum one character is allowed in 4(III)a Longitudinal gradient(%) Grade(S/U) ")]
        public string C4IIIA_LONG_GRAD_PER_GRADE { get; set; }


        [FieldType(PropertyType = PDFFieldType.Skip)]
        public string TABLE_FLAG { get; set; }






    }
}