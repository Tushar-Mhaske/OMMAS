using PMGSY.Areas.EFORMArea.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.QMModels
{
    public class EFORM_WORK_COMPLETED_IN_PROGRESS_ASPER_PROG_QM
    {

        public EFORM_WORK_COMPLETED_IN_PROGRESS_ASPER_PROG_QM(bool Table_Status, bool IsDelay, bool DateExtend, bool TableStatusInProgress)
        {
            this.Table_Status = Table_Status;
            this.IsDelay = IsDelay;
            this.IsDateExtended = DateExtend;
            this.TableStatusInProgress = TableStatusInProgress;
        }



        [FieldType(PropertyType = PDFFieldType.Skip)]
        public bool Table_Status { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public bool IsDelay { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public bool IsDateExtended { get; set; }


        [FieldType(PropertyType = PDFFieldType.Skip)]
        public bool TableStatusInProgress { get; set; }




        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int STATUS_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int EFORM_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int IMS_PR_ROAD_CODE { get; set; }





        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-32: ITEM 21. General Observations of QM- B. Please Select work completed or In progress ")]
        [RegularExpression(pattern: @"^[A-Za-z]{0,1}$", ErrorMessage = "Page-32: ITEM 21. General Observations of QM- B.  Maximum one character is allowed in work completed or In progress  ")]
        public string WORK_STATUS_32_1 { get; set; }


        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-32: ITEM 21. General Observations of QM- B.  Please Select Whether there was delay")]
        [RegularExpression(pattern: @"^[A-Za-z]{0,1}$", ErrorMessage = "Page-32: ITEM 21. General Observations of QM- B.  Maximum one character is allowed in Whether there was delay ")]
        [InProgressDependable]
        [DelayStatusDependable]
        public string C_IS_COMPLETED_WITH_DELAY { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [RegularExpression(pattern: @"^(?:\d{0,7}\.\d{1,3})$|^\d{0,7}$", ErrorMessage = "Page-32: ITEM 21. General Observations of QM- B.  Please Enter Valid number{cant be greater than 9999999.99} in Period of delay  (in months) ")]
        [InProgressDependable]
        [DelayStatusDependable]
        public Nullable<decimal> C_PERIOD_OF_DELAY { get; set; }



        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-32: ITEM 21. General Observations of QM- B.  Please Select Amount is")]
        [RegularExpression(pattern: @"^[A-Za-z]{0,1}$", ErrorMessage = "Page-32: ITEM 21. General Observations of QM- B.  Maximum one character is allowed in Amount is ")]
        [InProgressDependable]
        [DelayStatusDependable]
        public string C_AMOUNT_STATUS { get; set; }



        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [RegularExpression(pattern: @"^(?:\d{0,16}\.\d{1,2})$|^\d{0,16}$", ErrorMessage = "Page-32: ITEM 21. General Observations of QM- B.  Please Enter Valid number{cant be greater than 9999999999999999.99} in Amount (in lakh) ")]
        [InProgressDependable]
        [DelayStatusDependable]
        public Nullable<decimal> C_AMOUNT { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [StringLength(250, ErrorMessage = "Page-32: ITEM 21. General Observations of QM- B.  The length must be 250 character or less for Any other comment")]
        [InProgressDependable]
        [DelayStatusDependable]
        public string C_COMMENT { get; set; }


        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-32: ITEM 21. General Observations of QM- B.  Please Select Work progress is")]
        [RegularExpression(pattern: @"^[A-Za-z]{0,1}$", ErrorMessage = "Page-32: ITEM 21. General Observations of QM- B.  Maximum one character is allowed in Work progress is ")]
        [IsDateExtendedDependable]
        [TableStatusDependable]
        public string P_IS_AS_PER_SCHEDULE { get; set; }


        //[FieldType(PropertyType = PDFFieldType.RadioButton)]
        //public string P_IS_EXT_LAST_DATE { get; set; }



        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [RegularExpression(pattern: @"^(?:\d{0,7}\.\d{1,3})$|^\d{0,7}$", ErrorMessage = "Page-32: ITEM 21. General Observations of QM- B.  Please Enter Valid number{cant be greater than 9999999.99} in Period of extension (in months) ")]
        [IsDateExtendedDependable]
        [TableStatusDependable]
        public Nullable<decimal> P_EXT_MONTHS { get; set; }


        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-32: ITEM 21. General Observations of QM- B.  Please Select withheld amount refunded")]
        [RegularExpression(pattern: @"^[A-Za-z]{0,1}$", ErrorMessage = "Page-32: ITEM 21. General Observations of QM- B.  Maximum one character is allowed in withheld amount refunded ")]
        [TableStatusDependable]
        [IsDateExtendedDependable]
        public string P_IS_AMOUNT_REFUNDED { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [RegularExpression(pattern: @"^(?:\d{0,16}\.\d{1,2})$|^\d{0,16}$", ErrorMessage = "Page-32: ITEM 21. General Observations of QM- B.  Please Enter Valid number{cant be greater than 9999999999999999.99} in Amount (in lakh) ")]
        [TableStatusDependable]
        [IsDateExtendedDependable]
        public Nullable<decimal> P_AMOUNT { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [RegularExpression(pattern: @"^(?:\d{0,16}\.\d{1,2})$|^\d{0,16}$", ErrorMessage = "Page-32: ITEM 21. General Observations of QM- B.  Please Enter Valid number{cant be greater than 9999999999999999.99} in Amount of penalty on contractor ")]
        [TableStatusDependable]
        [IsDateExtendedDependable]
        public Nullable<decimal> P_PANELTY_AMOUNT { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [StringLength(250, ErrorMessage = "Page-32: ITEM 21. General Observations of QM- B.  The length must be 250 character or less for Any other comment")]
        [TableStatusDependable]
        [IsDateExtendedDependable]
        public string P_COMMENT { get; set; }




        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int QM_USER_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public string IPADD { get; set; }


    }
}