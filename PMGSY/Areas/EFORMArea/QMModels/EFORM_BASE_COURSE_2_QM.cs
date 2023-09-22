using PMGSY.Areas.EFORMArea.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.QMModels
{
    public class EFORM_BASE_COURSE_2_QM
    {
        public EFORM_BASE_COURSE_2_QM(bool IsNewTechUsed_BL2)
        {
            this.IsNewTechUsedBL2Status = IsNewTechUsed_BL2;
        }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public bool IsNewTechUsedBL2Status { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-16: Item 8. BASE COURSE: 2nd Layer- I. Please select Provision made in the second layer in sanctioned DPR ")]
        [RegularExpression(@"^[A-Za-z0-9 -]{0,15}$", ErrorMessage = "Page-16 Item 8. BASE COURSE: 2nd Layer- I. Maximum twenty character is allowed in Provision made in the second layer in sanctioned DPR")]

        public string PROVISION_IN_DPR_16 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-16: Item 8. BASE COURSE: 2nd Layer- II. Please select Item execution status ")]
        [RegularExpression(@"^[A-Za-z ]{0,20}$", ErrorMessage = "Page-16 Item 8. BASE COURSE: 2nd Layer- II. Maximum twenty character is allowed in Item execution status")]

        public string ITEM_EXECUTION_STATUS_16 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
         [Required(ErrorMessage = "Page-16: Item 8. BASE COURSE: 2nd Layer- III. Please select Actual execution in second layer of the Base course ")]
        [RegularExpression(@"^[A-Za-z- ]{0,20}$", ErrorMessage = "Page-16 Page-15 Item 8. BASE COURSE: 2nd Layer- III. Maximum twenty character is allowed in Actual execution in second layer of the Base course")]

        public string ACTUAL_EXECUTION_16 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-16: Item 8. BASE COURSE: 2nd Layer- IV. Please select Whether new technology used in this layer ")]
        [RegularExpression(@"^[A-Za-z]{0,1}$", ErrorMessage = "Page-16 Item 8. BASE COURSE: 2nd Layer- IV. Maximum one character is allowed in Whether new technology used in this layer")]

        public string IS_NEW_TECH_USED_16 { get; set; }

        [FieldType(PropertyType = PDFFieldType.ComboBox)]
        [IsNewTechBL2DependableGS]
        // [StringLength(maximumLength: 50, ErrorMessage = "Page-17 Only 50 Characters Allowed in ")]
        [Required(ErrorMessage = "Page-16: Item 8. BASE COURSE: 2nd Layer- IV. (a) Please select Name of technology used ")]

        public short NEW_TECH_NAME_16 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [IsNewTechBL2DependableGS]
        [Required(ErrorMessage = "Page-16: Item 8. BASE COURSE: 2nd Layer- IV. (b) Please enter Name of technology provider ")]
        [StringLength(maximumLength: 50, ErrorMessage = "Page-16 Item 8. BASE COURSE: 2nd Layer- IV. (b) Only 50 Characters Allowed in Name of technology provider")]

        public string NEW_TECH_PROVIDER_16 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [IsNewTechBL2DependableGS]
        [Required(ErrorMessage = "Page-16: Item 8. BASE COURSE: 2nd Layer- IV. (c) Please enter Name of stabiliser used ")]
        [StringLength(maximumLength: 50, ErrorMessage = "Page-16 Item 8. BASE COURSE: 2nd Layer- IV. (c) Only 50 Characters Allowed in Name of stabiliser used")]

        public string NAME_STABILISER_USED_16 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [IsNewTechBL2DependableGS]
        [Required(ErrorMessage = "Page-16: Item 8. BASE COURSE: 2nd Layer- IV. (d) Please enter Quantity of stabiliser as per DPR ")]
        [StringLength(100, ErrorMessage = "Page-16: Item 8. BASE COURSE: 2nd Layer- IV. (d) Maximum 100 characters are allowed in Quantity of stabiliser as per DPR")]

        public string STABILISER_QTY_ASPER_DPR_16 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [IsNewTechBL2DependableGS]
        [Required(ErrorMessage = "Page-16: Item 8. BASE COURSE: 2nd Layer- IV. (e) Please enter Quantity of stabiliser used ")]
        [StringLength(100, ErrorMessage = "Page-16: Item 8. BASE COURSE: 2nd Layer- IV. (e) Maximum 100 characters are allowed in Quantity of stabiliser used")]

        public string STABILISER_QTY_USED_16 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [IsNewTechBL2DependableGS]
        [Required(ErrorMessage = "Page-16: Item 8. BASE COURSE: 2nd Layer- IV. (f) Please enter Unconfined compressive strength (UCS) as per DPR ")]
        [RegularExpression(@"^(?:\d{0,7}\.\d{1,3})$|^\d{0,7}$", ErrorMessage = "Page-16 Item 8. BASE COURSE: 2nd Layer- IV. (f) Maximum seven digits before decimal and maximum three digits after decimal are allowed in Unconfined compressive strength (UCS) as per DPR")]

        public Nullable<decimal> UCS_ASPER_DPR_17 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
 
         [StringLength(maximumLength: 250, ErrorMessage = "Page-17 Item 8. BASE COURSE: 2nd Layer- V. Only 250 Characters Allowed in Reason for change in actual execution at site w.r.t provision made in DPR")]

        public string REASON_FOR_CHANGE_17 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-17: Item Item 8. BASE COURSE: 2nd Layer- Please select Item Grading- 8 ")]

        [RegularExpression(@"^[A-Za-z]{0,3}$", ErrorMessage = "Page-17 Item 8. BASE COURSE: 2nd Layer- IV. Maximum three character is allowed in Item Grading- 8:")]
        
        public string ITEM_GRADING_8 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-17: Item 8. BASE COURSE: 2nd Layer- Please enter suggestions for improvement ")]
        [StringLength(maximumLength: 250, ErrorMessage = "Page-17 Item 8. BASE COURSE: 2nd Layer- Only 250 Characters Allowed in suggestions for improvement")]

        public string IMPROVEMENT_REMARK_17 { get; set; }
    }
}