using PMGSY.Areas.EFORMArea.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.QMModels
{
    public class EFORM_BASE_COURSE_3_QM
    {
        public EFORM_BASE_COURSE_3_QM(bool IsNewTechUsed_BL3)
        {
            this.IsNewTechUsedBL3Status = IsNewTechUsed_BL3;
        }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public bool IsNewTechUsedBL3Status { get; set; }



        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-17: Item 9. BASE COURSE: 3rd Layer- I. Please select Provision made in the third layer in sanctioned DPR ")]
        [RegularExpression(@"^[A-Za-z0-9 -]{0,15}$", ErrorMessage = "Page-17 Item 9. BASE COURSE: 3rd Layer- I. Maximum twenty character is allowed in Provision made in the third layer in sanctioned DPR")]

        public string PROVISION_IN_DPR_17 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-17: Item 9. BASE COURSE: 3rd Layer- II. Please select Item execution status ")]
        [RegularExpression(@"^[A-Za-z ]{0,20}$", ErrorMessage = "Page-17 Item 9. BASE COURSE: 3rd Layer- II. Maximum twenty character is allowed in Item execution status")]

        public string ITEM_EXECUTION_STATUS_17 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-17: Item 9. BASE COURSE: 3rd Layer- III. Please select Actual execution in third layer of the Base course ")]
        [RegularExpression(@"^[A-Za-z- ]{0,20}$", ErrorMessage = "Page-17 Page-15 Item 9. BASE COURSE: 3rd Layer- III. Maximum twenty character is allowed in Actual execution in third layer of the Base course")]

        public string ACTUAL_EXECUTION_17 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-18: Item 9. BASE COURSE: 3rd Layer- IV. Please select Whether new technology used in this layer ")]
        [RegularExpression(@"^[A-Za-z]{0,1}$", ErrorMessage = "Page-18 Item 9. BASE COURSE: 3rd Layer- IV. Maximum one character is allowed in Whether new technology used in this layer")]

        public string IS_NEW_TECH_USED_18 { get; set; }

        [FieldType(PropertyType = PDFFieldType.ComboBox)]
        [IsNewTechBL3DependableGS]
         //   [StringLength(maximumLength: 50, ErrorMessage = "Page-18 Only 50 Characters Allowed in ")]
        [Required(ErrorMessage = "Page-18: Item 9. BASE COURSE: 3rd Layer- IV. (a) Please select Name of technology used ")]


        public short NEW_TECH_NAME_18 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [IsNewTechBL3DependableGS]
        [Required(ErrorMessage = "Page-18: Item 9. BASE COURSE: 3rd Layer- IV. (b) Please enter Name of technology provider ")]
        [StringLength(maximumLength: 50, ErrorMessage = "Page-18 Item 9. BASE COURSE: 3rd Layer- IV. (b) Only 50 Characters Allowed in Name of technology provider")]

        public string NEW_TECH_PROVIDER_18 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [IsNewTechBL3DependableGS]
        [Required(ErrorMessage = "Page-18: Item 9. BASE COURSE: 3rd Layer- IV. (c) Please enter Name of stabiliser used ")]
        [StringLength(maximumLength: 50, ErrorMessage = "Page-18 Item 9. BASE COURSE: 3rd Layer- IV. (c) Only 50 Characters Allowed in Name of stabiliser used")]

        public string NAME_STABILISER_USED_18 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [IsNewTechBL3DependableGS]
        [Required(ErrorMessage = "Page-18: Item 9. BASE COURSE: 3rd Layer- IV. (d) Please enter Quantity of stabiliser as per DPR ")]
        [StringLength(100, ErrorMessage = "Page-18: Item 9. BASE COURSE: 3rd Layer- IV. (d) Maximum 100 characters are allowed in Quantity of stabiliser as per DPR")]

        public string STABILISER_QTY_ASPER_DPR_18 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [IsNewTechBL3DependableGS]
        [Required(ErrorMessage = "Page-18: Item 9. BASE COURSE: 3rd Layer- IV. (e) Please enter Quantity of stabiliser used ")]
        [StringLength(100, ErrorMessage = "Page-18: Item 9. BASE COURSE: 3rd Layer- IV. (e) Maximum 100 characters are allowed in Quantity of stabiliser used")]

        public string STABILISER_QTY_USED_18 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [IsNewTechBL3DependableGS]
        [Required(ErrorMessage = "Page-18: Item 9. BASE COURSE: 3rd Layer- IV. (f) Please enter Unconfined compressive strength (UCS) as per DPR ")]
        [RegularExpression(@"^(?:\d{0,7}\.\d{1,3})$|^\d{0,7}$", ErrorMessage = "Page-18 Item 9. BASE COURSE: 3rd Layer- IV. (f) Maximum seven digits before decimal and maximum three digits after decimal are allowed in Unconfined compressive strength (UCS) as per DPR")]

        public Nullable<decimal> UCS_ASPER_DPR_18 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]

         [StringLength(maximumLength: 250, ErrorMessage = "Page-18 Item 9. BASE COURSE: 3rd Layer- V. Only 250 Characters Allowed in Reason for change in actual execution at site w.r.t provision made in DPR")]

        public string REASON_FOR_CHANGE_18 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-18: Item 9. BASE COURSE: 3rd Layer- Please select Item Grading- 7 ")]
        [RegularExpression(@"^[A-Za-z]{0,3}$", ErrorMessage = "Page-18 Item 9. BASE COURSE: 3rd Layer- IV. Maximum three character is allowed in Item Grading- 9:")]

        public string ITEM_GRADING_9 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-18: Item 9. BASE COURSE: 3rd Layer- Please enter suggestions for improvement ")]
        [StringLength(maximumLength: 250, ErrorMessage = "Page-18 Item 9. BASE COURSE: 3rd Layer- Only 250 Characters Allowed in suggestions for improvement")]

        public string IMPROVEMENT_REMARK_18 { get; set; }
    }
}