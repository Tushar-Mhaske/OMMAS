using PMGSY.Areas.EFORMArea.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.QMBridgeModel
{
    public class EFORM_BRIDGE_QM_WORK_COMPLETED_IN_PROGRESS_ASPER_COST
    {

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-31: Item 12. C. Please Select Whether the work is completed within the sanctioned cost:")]
        [StringLength(3, ErrorMessage = "Page-31: Item 12. C. The length must be 3 character or less for Whether the work is completed within the sanctioned cost")]
        public string WORK_STATUS_31 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-31: Item 12. C. Please enter Sanctioned cost :")]
        [RegularExpression(@"^(\d{0,16}(\.\d{0,2})?)$", ErrorMessage = "Page-31 Item 12. C.Sanctioned cost - Maximum 16 digits before decimal and maximum two digits after decimal are allowed in  Sanctioned cost.")]
        public Nullable<decimal> SANCTION_COST { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-31: Item 12. C. Please enter Completion cost :")]
        [RegularExpression(@"^(\d{0,16}(\.\d{0,2})?)$", ErrorMessage = "Page-31 Item 12. C.Completion cost - Maximum 16 digits before decimal and maximum two digits after decimal are allowed in  Completion cost.")]
        public Nullable<decimal> COMPLETION_COST { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-31: Item 12. C. Please enter Reason for extra cost:")]
        [StringLength(250, ErrorMessage = "Page-31: Item 12. C. The length must be 250 character or less for Reason for extra cost.  ")]
        public string REASON_EXTRA_COST { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-31: Item 12. C. Please enter Action taken By PIU:")]
        [StringLength(250, ErrorMessage = "Page-31: Item 12. C.- The length must be 250 character or less for Action taken By PIU.  ")]
        public string ACTION_BY_PIU { get; set; }
       
    }
}