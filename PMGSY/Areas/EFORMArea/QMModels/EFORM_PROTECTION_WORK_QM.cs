using PMGSY.Areas.EFORMArea.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.QMModels
{
    public class EFORM_PROTECTION_WORK_QM
    {
        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-24: Item 15.PROTECTION WORK- I. Please select Whether sanctioned DPR has the provision of protection works ")]

        [RegularExpression(@"^[A-Za-z]{0,1}$", ErrorMessage = "Page-24: Item 15.PROTECTION WORK- I. Maximum one character is allowed in Whether sanctioned DPR has the provision of protection works")]
        public string IS_DPR_PROVISION_24 { get; set; }

        [FieldType(PropertyType = PDFFieldType.CheckBox)]
        [RegularExpression(@"^[A-Za-z]{0,1}$", ErrorMessage = "Page-24: Item 15.PROTECTION WORK- II. Maximum one character is allowed for Retaining wall  ")]
        public string Retaining_Wall { get; set; }

        [FieldType(PropertyType = PDFFieldType.CheckBox)]
        [RegularExpression(@"^[A-Za-z]{0,1}$", ErrorMessage = "Page-24: Item 15.PROTECTION WORK- II. Maximum one character is allowed for Breast wall")]
        public string Breast_Wall { get; set; }

        [FieldType(PropertyType = PDFFieldType.CheckBox)]
        [RegularExpression(@"^[A-Za-z]{0,1}$", ErrorMessage = "Page-24: Item 15.PROTECTION WORK- II. Maximum one character is allowed for Parapet wall  ")]
        public string Parapet_Wall { get; set; }

        [FieldType(PropertyType = PDFFieldType.CheckBox)]
        [RegularExpression(@"^[A-Za-z]{0,1}$", ErrorMessage = "Page-24: Item 15.PROTECTION WORK- II. Maximum one character is allowed for Any other type of Protection work")]
        public string Any_other { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [RegularExpression(@"^(?:\d{0,7}\.\d{1,3})$|^\d{0,7}$", ErrorMessage = "Page-24: Item 15.PROTECTION WORK- II. Maximum seven digits before decimal and maximum three digits after decimal are allowed in Retaining wall length ")]

        public Nullable<decimal> RETAINING_WALL_LENGTH { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [RegularExpression(@"^(?:\d{0,7}\.\d{1,3})$|^\d{0,7}$", ErrorMessage = "Page-24: Item 15.PROTECTION WORK- II. Maximum seven digits before decimal and maximum three digits after decimal are allowed in Breast wall length")]

        public Nullable<decimal> BREAST_WALL_LENGTH { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [RegularExpression(@"^(?:\d{0,7}\.\d{1,3})$|^\d{0,7}$", ErrorMessage = "Page-24: Item 15.PROTECTION WORK- II. Maximum seven digits before decimal and maximum three digits after decimal are allowed in Parapet wall length")]

        public Nullable<decimal> PARAPET_WALL_LENGTH { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]

        [StringLength(maximumLength: 20, ErrorMessage = "Page-24: Item 15.PROTECTION WORK- II. Only 20 Characters Allowed in Any other type of Protection work a. name ")]
        public string OTHER_TYPE1_NAME { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [StringLength(maximumLength: 20, ErrorMessage = "Page-24: Item 15.PROTECTION WORK- II. Only 20 Characters Allowed in Any other type of Protection work b. name ")]
        public string OTHER_TYPE2_NAME { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [StringLength(maximumLength: 20, ErrorMessage = "Page-24: Item 15.PROTECTION WORK- II. Only 20 Characters Allowed in Any other type of Protection work a. name ")]
        public string OTHER_TYPE3_NAME { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [RegularExpression(@"^(?:\d{0,7}\.\d{1,3})$|^\d{0,7}$", ErrorMessage = "Page-24: Item 15.PROTECTION WORK- II. Maximum seven digits before decimal and maximum three digits after decimal are allowed in Any other type of Protection work a. length ")]

        public Nullable<decimal> OTHER_A_WALL_LENGTH { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [RegularExpression(@"^(?:\d{0,7}\.\d{1,3})$|^\d{0,7}$", ErrorMessage = "Page-24: Item 15.PROTECTION WORK- II. Maximum seven digits before decimal and maximum three digits after decimal are allowed  Any other type of Protection work a. length ")]

        public Nullable<decimal> OTHER_B_WALL_LENGTH { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [RegularExpression(@"^(?:\d{0,7}\.\d{1,3})$|^\d{0,7}$", ErrorMessage = "Page-24: Item 15.PROTECTION WORK- II. Maximum seven digits before decimal and maximum three digits after decimal are allowed  Any other type of Protection work a. length ")]

        public Nullable<decimal> OTHER_C_WALL_LENGTH { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-24: Item 15.PROTECTION WORK-  III. Please select Total length of all protection work provided in DPR  ")]
        [RegularExpression(@"^(?:\d{0,7}\.\d{1,3})$|^\d{0,7}$", ErrorMessage = "Page-24: Item 15.PROTECTION WORK- III. Maximum seven digits before decimal and maximum three digits after decimal are allowed in Total length of all protection work provided in DPR ")]

        public decimal TOT_LENGTH { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [RegularExpression(@"^[A-Za-z ]{0,1}$", ErrorMessage = "Page-24: Item 15.PROTECTION WORK- VI. a) Maximum one character is allowed in Workmanship of stone masonry is acceptable")]

        public string IS_STONE_MEASONRY_ACCEPTABLE { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [RegularExpression(@"^[A-Za-z ]{0,1}$", ErrorMessage = "Page-24: Item 15.PROTECTION WORK- VI. a) Maximum one character is allowed in Bond stone has been provided in stone masonry ")]

        public string IS_BOND_STONE_PROVIDED { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]

        [Required(ErrorMessage = "Page-25: Item 15.PROTECTION WORK-  Please select Item Grading-15 ")]
        [RegularExpression(@"^[A-Za-z ]{0,3}$", ErrorMessage = "Page-24: Item 15.PROTECTION WORK- VI. a) Maximum three character is allowed in Item Grading-15 ")]

        public string ITEM_GRADING_15 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-25: Item 15.PROTECTION WORK-  Please enter suggestions for improvement ")]
        [StringLength(maximumLength: 250, ErrorMessage = "Page-25: Item 15.PROTECTION WORK-  Only 250 Characters Allowed in suggestions for improvement")]
        public string IMPROVE_SUGGESTIONS_25 { get; set; }
    }
}