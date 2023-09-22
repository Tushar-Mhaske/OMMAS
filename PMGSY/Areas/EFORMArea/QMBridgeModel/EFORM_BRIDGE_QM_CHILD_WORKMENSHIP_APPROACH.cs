using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;
using System.ComponentModel.DataAnnotations;
using PMGSY.Areas.EFORMArea.Model;
namespace PMGSY.Areas.EFORMArea.QMBridgeModel
{
    public class EFORM_BRIDGE_QM_CHILD_WORKMENSHIP_APPROACH
    {

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int ROW_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-28: Item 10. APPROACHES: 10.5 Protection Work- IV. Workmanship of Retaining structures- Please enter Location(RD) of row  ")]
        [RegularExpression(@"^(?:\d{0,7}\.\d{1,3})$|^\d{0,7}$", ErrorMessage = "Page-28 Item 10. APPROACHES: 10.5 Protection Work- IV. Workmanship of Retaining structures- Maximum seven digits before decimal and maximum three digits after decimal are allowed in  Location(RD) of row ")]

        public Nullable<decimal> RD_LOC_28 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-28: Item 10. APPROACHES: 10.5 Protection Work- IV. Workmanship of Retaining structures- Please enter Workmanship of retaining structures of row  ")]
        [StringLength(20, ErrorMessage = "Page-28 Item 10. APPROACHES: 10.5 Protection Work- IV. Workmanship of Retaining structures- The length must be 20 character or less for Workmanship of retaining structures of row ")]

        public string RETAIN_STRUCTURE_28 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-28: Item 10. APPROACHES: 10.5 Protection Work- IV. Workmanship of Retaining structures- Please select Whether honeycombing/any other defects are observed (Y/N) of row  ")]
        [StringLength(3, ErrorMessage = "Page-28 Item 10. APPROACHES: 10.5 Protection Work- IV. Workmanship of Retaining structures- The length must be 3 character or less for Whether honeycombing/any other defects are observed (Y/N) of row ")]

        public string IS_ANY_DEFECT__28 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-28: Item 10. APPROACHES: 10.5 Protection Work- IV. Workmanship of Retaining structures- Please select Have weep holes been provided (Yes/No) of row  ")]
        [StringLength(3, ErrorMessage = "Page-28 Item 10. APPROACHES: 10.5 Protection Work- IV. Workmanship of Retaining structures- The length must be 3 character or less for Have weep holes been provided (Yes/No) of row ")]

        public string IS_WEEP_HOLES_PROVIDED_28 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-28: Item 10. APPROACHES: 10.5 Protection Work- IV. Workmanship of Retaining structures- Please select Adequacy of weep holes (if provided) (Yes / No) of row  ")]
        [StringLength(3, ErrorMessage = "Page-28 Item 10. APPROACHES: 10.5 Protection Work- IV. Workmanship of Retaining structures- The length must be 3 character or less for Adequacy of weep holes (if provided) (Yes / No) of row ")]

        public string IS_ADEQUACY_WEEP_HOLES_28 { get; set; }
       

    }
}