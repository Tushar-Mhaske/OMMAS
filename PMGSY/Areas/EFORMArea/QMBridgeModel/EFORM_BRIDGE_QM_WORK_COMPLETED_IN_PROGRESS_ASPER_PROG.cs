using PMGSY.Areas.EFORMArea.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.QMBridgeModel
{
    public class EFORM_BRIDGE_QM_WORK_COMPLETED_IN_PROGRESS_ASPER_PROG
    {
       
        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-30: Item 12. B. Please Select Whether the work has been completed/is in progress as per work programme or the delay hasoccurred")]
        [StringLength(3, ErrorMessage = "Page-30: Item 12. B. The length must be 3 character or less for Whether the work has been completed/is in progress as per work programme or the delay hasoccurred")]

        public string WORK_STATUS_30 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-30: Item 12. B. Please Select Whether there was delay:")]
        [StringLength(3, ErrorMessage = "Page-30: Item 12. B. The length must be 3 character or less for Whether there was delay")]

        public string C_IS_COMPLETED_WITH_DELAY { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-30: Item 12. B. Please enter Period of delay :")]
        [RegularExpression(@"^(?:\d{0,7}\.\d{1,3})$|^\d{0,7}$", ErrorMessage = "Page-30 Item 12. B.Period of delay - Maximum seven digits before decimal and maximum three digits after decimal are allowed in  Period of delay. ")]
        public Nullable<decimal> C_PERIOD_OF_DELAY { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-30: Item 12. B. Please Select Amount is Withhold or Recovered:")]
        [StringLength(20, ErrorMessage = "Page-30: Item 12. B. The length must be 20 character or less for Amount is Withhold or Recovered")]

        public string C_AMOUNT_STATUS { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-30: Item 12. B. Please enter Amount :")]
        [RegularExpression(@"^(\d{0,16}(\.\d{0,2})?)$", ErrorMessage = "Page-30 Item 12. B.Amount - Maximum 16 digits before decimal and maximum two digits after decimal are allowed in  Amount.")]
        public Nullable<decimal> C_AMOUNT { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-30: Item 12. B. Please enter Reason for delay/any other comment:")]
        [StringLength(250, ErrorMessage = "Page-30: Item 12. B.- The length must be 250 character or less for comment.  ")]
        public string C_COMMENT { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-30: Item 12. B. Please Select Work progress is :")]
        [StringLength(1, ErrorMessage = "Page-30: Item 12. B. The length must be 1 character or less for Work progress is")]

        public string P_IS_AS_PER_SCHEDULE { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-30: Item 12. B. Please enter Period of extension :")]
        [RegularExpression(@"^(?:\d{0,7}\.\d{1,3})$|^\d{0,7}$", ErrorMessage = "Page-30 Item 12. B.Period of extension - Maximum seven digits before decimal and maximum three digits after decimal are allowed in  Period of extension. ")]
        public Nullable<decimal> P_EXT_MONTHS { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-30: Item 12. B. Please Select Withhold amount refunded :")]
        [StringLength(1, ErrorMessage = "Page-30: Item 12. B. The length must be 1 character or less for Withhold amount refunded")]

        public string P_IS_AMOUNT_REFUNDED { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-30: Item 12. B. Please enter Amount:")]
        [RegularExpression(@"^(\d{0,16}(\.\d{0,2})?)$", ErrorMessage = "Page-30 Item 12. B.Amount - Maximum 16 digits before decimal and maximum two digits after decimal are allowed in  Amount.")]
        public Nullable<decimal> P_AMOUNT { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-30: Item 12. B. Please enter Amount of penalty on Contractor :")]
        [RegularExpression(@"^(\d{0,16}(\.\d{0,2})?)$", ErrorMessage = "Page-30 Item 12. B.Amount of penalty on Contractor - Maximum 16 digits before decimal and maximum two digits after decimal are allowed in  Amount of penalty on Contractor.")]
        public Nullable<decimal> P_PANELTY_AMOUNT { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-30: Item 12. B. Please enter Reason for delay/any other comment:")]
        [StringLength(250, ErrorMessage = "Page-30: Item 12. B.- The length must be 250 character or less for comment. ")]
        public string P_COMMENT { get; set; }

    }
}