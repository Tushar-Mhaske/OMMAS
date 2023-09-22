using PMGSY.Areas.EFORMArea.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.QMModels
{
    public class EFORM_QM_SHOULDERS
    {

        public EFORM_QM_SHOULDERS(bool IS_NEW_TECH_USED_22_VAL)
        {
            this.radioButtonValue = IS_NEW_TECH_USED_22_VAL;
        }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public bool radioButtonValue { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int SH_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int EFORM_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int IMS_PR_ROAD_CODE { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-22: Item 12.SHOULDERS- I. Please select Item execution status")]
        //[RegularExpression(@"^[A-Za-z ]{0,20}$", ErrorMessage = "Page-22: Maximum twenty characters are allowed ")]
        [StringLength(20, ErrorMessage = "Page-22: Item 12.SHOULDERS- I. Maximum twenty characters are allowed in Item execution status")]
        public string ITEM_EXECUTION_STATUS_22 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-22: Item 12.SHOULDERS- II. Please select Whether new technology used in this layer")]
        //[RegularExpression(@"^[A-Za-z ]{0,1}$", ErrorMessage = "Page-22: Maximum one characters is allowed ")]
        [StringLength(1, ErrorMessage = "Page-22: Item 12.SHOULDERS- II. Maximum one character is allowed in Whether new technology used in this layer")]
        public string IS_NEW_TECH_USED_22 { get; set; }

        [FieldType(PropertyType = PDFFieldType.ComboBox)]
        //[RegularExpression(@"^[A-Za-z0-9 ]{0,50}$", ErrorMessage = "Page-22: Maximum fifty characters are allowed ")]
        [Required(ErrorMessage = "Page-22: Item 12.SHOULDERS- II. (a) Please select Name of technology used")]
        [StringLength(50, ErrorMessage = "Page-22: Item 12.SHOULDERS- II. (a) Maximum fifty characters are allowed in Name of technology used")]
        [IsNewTechUsed22Val]
        public short NEW_TECH_NAME_22 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-22: Item 12.SHOULDERS- II. (b) Please enter Name of technology provider")]
        //[RegularExpression(@"^[A-Za-z0-9 ]{0,50}$", ErrorMessage = "Page-22: Maximum fifty characters are allowed ")]
        [StringLength(50, ErrorMessage = "Page-22: Item 12.SHOULDERS- II. (b) Maximum fifty characters are allowed in Name of technology provider")]
        [IsNewTechUsed22Val]
        public string NEW_TECH_PROVIDER_22 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-22: Item 12.SHOULDERS- II. (c) Please enter Name of stabiliser used")]
        [IsNewTechUsed22Val]
        //[RegularExpression(@"^[A-Za-z0-9 ]{0,50}$", ErrorMessage = "Page-22: Maximum fifty characters are allowed ")]
        [StringLength(50, ErrorMessage = "Page-22: Item 12.SHOULDERS- II. (c) Maximum fifty characters are allowed in Name of stabiliser used")]
        public string NAME_STABILISER_USED_22 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-22: Item 12.SHOULDERS- II. (d) Please enter Quantity of stabiliser as per DPR")]
        [IsNewTechUsed22Val]
        //[RegularExpression(@"^[A-Za-z0-9 ]{0,20}$", ErrorMessage = "Page-22: Maximum twenty characters are allowed ")]
        [StringLength(100, ErrorMessage = "Page-22: Item 12.SHOULDERS- II. (d) Maximum 100 characters are allowed in Quantity of stabiliser as per DPR")]
        public string STABILISER_QTY_ASPER_DP_22 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-22: Item 12.SHOULDERS- II. (e) Please enter Quantity of stabiliser used")]
        //[RegularExpression(@"^[A-Za-z0-9 ]{0,20}$", ErrorMessage = "Page-22: Maximum twenty characters are allowed ")]
        [StringLength(100, ErrorMessage = "Page-22: Item 12.SHOULDERS- II. (e) Maximum 100 characters are allowed in Quantity of stabiliser used")]
        [IsNewTechUsed22Val]
        public string STABILISER_QTY_USED_22 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-22: Item 12.SHOULDERS- II. (f) Please enter Unconfined compressive strength (UCS) as per DPR")]
        [IsNewTechUsed22Val]
        [RegularExpression(@"^[0-9]{0,7}(?:\.[0-9]{1,3})?$", ErrorMessage = "Page-22: Item 12.SHOULDERS- II. (f) Maximum seven digits before decimal and maximum three digits after decimal are allowed in Unconfined compressive strength (UCS) as per DPR")]
        public Nullable<decimal> UCS_ASPER_DPR_22 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-22: Item 12.SHOULDERS- III. Please select Name of material to be used in the shoulder as per DPR")]
        //[RegularExpression(@"^[A-Za-z ]{0,15}$", ErrorMessage = "Page-22: Maximum fifteen characters are allowed ")]
        [StringLength(15, ErrorMessage = "Page-22: Item 12.SHOULDERS- III. Maximum fifteen characters are allowed in Name of material to be used in the shoulder as per DPR")]
        public string MATERIAL_TYPE_22 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-22: Item 12.SHOULDERS- III. Please enter Width")]
        [RegularExpression(@"^[0-9]{0,7}(?:\.[0-9]{1,3})?$", ErrorMessage = "Page-22: Item 12.SHOULDERS- III. Maximum seven digits before decimal and maximum three digits after decimal are allowed Width")]
        public decimal MATERIAL_WIDTH_22 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-22: Item 12.SHOULDERS- III. Please enter Thickness")]
        [RegularExpression(@"^[0-9]{0,7}(?:\.[0-9]{1,3})?$", ErrorMessage = "Page-22: Item 12.SHOULDERS- III. Maximum seven digits before decimal and maximum three digits after decimal are allowed Thickness")]
        public decimal MATERIAL_THICKNESS_22 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-23: Item 12.SHOULDERS- Please select Item Grading-12")]
        //[RegularExpression(@"^[A-Za-z ]{0,3}$", ErrorMessage = "Page-23: Maximum three characters are allowed ")]
        [StringLength(3, ErrorMessage = "Page-23: Item 12.SHOULDERS- Maximum three characters are allowed in Item Grading-12")]
        public string ITEM_GRADING_12 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-23: Item 12.SHOULDERS- Please enter suggestions for improvement")]
        // [RegularExpression(@"^[A-Za-z ]{0,250}$", ErrorMessage = "Page-23: Maximum two fifty characters are allowed ")]
        [StringLength(250, ErrorMessage = "Page-23: Item 12.SHOULDERS- Maximum two fifty characters are allowed in suggestions for improvement")]
        public string IMPROVE_SUGGESTION_23 { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int QM_USER_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public string IPADD { get; set; }


    }
}