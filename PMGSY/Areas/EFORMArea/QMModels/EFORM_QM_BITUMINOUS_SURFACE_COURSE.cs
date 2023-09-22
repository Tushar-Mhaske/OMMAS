using PMGSY.Areas.EFORMArea.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.QMModels
{
    public class EFORM_QM_BITUMINOUS_SURFACE_COURSE
    {

        public EFORM_QM_BITUMINOUS_SURFACE_COURSE(bool IS_NEW_TECH_QTY_USED_20_VAL)
        {
            this.radioButtonValue = IS_NEW_TECH_QTY_USED_20_VAL;
        }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public bool radioButtonValue { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int SC_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int EFORM_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int IMS_PR_ROAD_CODE { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-20: Item 11. BITUMINOUS SURFACE COURSE- I. Please select Provision made in the sanctioned DPR")]
        //[RegularExpression(@"^[&A-Za-z0-9 ]{0,50}$", ErrorMessage = "Page-20: Maximum fifty characters are allowed ")]
        [StringLength(50, ErrorMessage = "Page-20 Item 11. BITUMINOUS SURFACE COURSE- I. Maximum fifty characters are allowed in Provision made in the sanctioned DPR")]
        public string PROVISION_IN_DPR_20 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-20: Item 11. BITUMINOUS SURFACE COURSE- II. Please select Item execution status")]
        //[RegularExpression(@"^[A-Za-z ]{0,20}$", ErrorMessage = "Page-20: Maximum twenty characters are allowed ")]
        [StringLength(20, ErrorMessage = "Page-20 Item 11. BITUMINOUS SURFACE COURSE- II. Maximum twnety characters are allowed in Item execution status")]
        public string ITEM_EXECUTION_STATUS_20 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-20: Item 11. BITUMINOUS SURFACE COURSE- III. Please select Type of bituminous surface executed")]
        //[RegularExpression(@"^[A-Za-z& ]{0,50}$", ErrorMessage = "Page-20: Maximum fifty characters are allowed ")]
        [StringLength(50, ErrorMessage = "Page-20 Item 11. BITUMINOUS SURFACE COURSE- III. Maximum fifty characters are allowed in Type of bituminous surface executed")]
        public string BITUMINOUS_TYPE_20 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-20: Item 11. BITUMINOUS SURFACE COURSE- IV. Please select Whether new technology used in this layer")]
        [RegularExpression(@"^[A-Za-z]{0,1}$", ErrorMessage = "Page-20 Item Item 11. BITUMINOUS SURFACE COURSE- IV. Maximum one character is allowed in Whether new technology used in this layer")]

        public string NEW_TECH_QTY_USED_20 { get; set; }

        [FieldType(PropertyType = PDFFieldType.ComboBox)]
        [Required(ErrorMessage = "Page-20: Item 11. BITUMINOUS SURFACE COURSE- IV. (a) Please select Name of technology used")]
        //[RegularExpression(@"^[A-Za-z0-9 ]{0,50}$", ErrorMessage = "Page-20: Maximum fifty characters are allowed ")]
      //  [StringLength(50, ErrorMessage = "Page-20: Maximum fifty characters are allowed ")]
        [IsNewTechQtyUsed20Val]
        public short NEW_TECH_NAME_20 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-20: Item 11. BITUMINOUS SURFACE COURSE- IV. (b) Please enter Name of technology provider")]
        //[RegularExpression(@"^[A-Za-z0-9 ]{0,50}$", ErrorMessage = "Page-20: Maximum fifty characters are allowed ")]
        [StringLength(50, ErrorMessage = "Page-20: Item 11. BITUMINOUS SURFACE COURSE- IV. (b) Maximum fifty characters are allowed in Name of technology provider")]
        [IsNewTechQtyUsed20Val]
        public string NEW_TECH_PROVIDER_20 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-20: Item 11. BITUMINOUS SURFACE COURSE- IV. (c) Please enter Name of new technology material")]
        //[RegularExpression(@"^[A-Za-z0-9 ]{0,20}$", ErrorMessage = "Page-20: Maximum twenty characters are allowed ")]
        [StringLength(20, ErrorMessage = "Page-20: Item 11. BITUMINOUS SURFACE COURSE- IV. (c) Maximum twenty characters are allowed in Name of new technology material")]
        [IsNewTechQtyUsed20Val]
        public string NEW_TECH_MATERIAL_USED_20 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-20: Item 11. BITUMINOUS SURFACE COURSE- IV. (d) Please enter Quantity of new technology material as per DPR")]
        [StringLength(100, ErrorMessage = "Page-20: Item 11. BITUMINOUS SURFACE COURSE- IV. (d) Maximum 100 characters are allowed in Quantity of new technology material as per DPR")]

        [IsNewTechQtyUsed20Val]
        public string NEW_TECH_QTY_USED_ASPER_DPR_20 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-20: Item 11. BITUMINOUS SURFACE COURSE- IV. (e) Please enter Quantity of new technology material used")]
        [StringLength(100, ErrorMessage = "Page-20: Item 11. BITUMINOUS SURFACE COURSE- IV. (e) Maximum 100 characters are allowed in Quantity of new technology material used")]

        [IsNewTechQtyUsed20Val]
        public string NEW_TECH_QTY_MATERIAL_20 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-20: Item 11. BITUMINOUS SURFACE COURSE- IV. (f) Please enter Location of new technology section: From RD ")]
        [RegularExpression(@"^[0-9]{0,7}(?:\.[0-9]{1,3})?$", ErrorMessage = "Page-20: Item 11. BITUMINOUS SURFACE COURSE- IV. (f) Maximum seven digits before decimal and maximum three digits after decimal are allowed in Location of new technology section: To RD")]
        [IsNewTechQtyUsed20Val]
        public Nullable<decimal> NEW_TECH_LOC_FROM_ROAD_20 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-20: Item 11. BITUMINOUS SURFACE COURSE- IV. (f) Please enter Evaluation of new technology section")]
        [RegularExpression(@"^[0-9]{0,7}(?:\.[0-9]{1,3})?$", ErrorMessage = "Page-20: Item 11. BITUMINOUS SURFACE COURSE- IV. (f) Maximum seven digits before decimal and maximum three digits after decimal are allowed Evaluation of new technology section")]
        [IsNewTechQtyUsed20Val]
        public Nullable<decimal> NEW_TECH_LOC_TO_ROAD_20 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-21: Item 11. BITUMINOUS SURFACE COURSE- IV. (g) Please select Evaluation of new technology section")]
        [IsNewTechQtyUsed20Val]
        //[RegularExpression(@"^[A-Za-z ]{0,1}$", ErrorMessage = "Page-21: Maximum one character is allowed ")]       
        [StringLength(1, ErrorMessage = "Page-21: Item 11. BITUMINOUS SURFACE COURSE- IV. (g) Maximum one character is allowed in Evaluation of new technology section")]
        public string GRADING_NEW_TECH_EVALUATION_21 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-21: Item 11. BITUMINOUS SURFACE COURSE- V. Please enter Thickness of layer as per DPR")]
        [RegularExpression(@"^[0-9]{0,7}(?:\.[0-9]{1,3})?$", ErrorMessage = "Page-21: Item 11. BITUMINOUS SURFACE COURSE- V. Maximum seven digits before decimal and maximum three digits after decimal are allowed in Thickness of layer as per DPR")]
        public decimal THICKNESS_ASPER_DPR { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-21: Item 11. BITUMINOUS SURFACE COURSE- VI. Please enter Type and grade of binder used")]
        //[RegularExpression("^[A-Za-z0-9 ]{0,50}$", ErrorMessage = "Page-21: Maximum fifty characters are allowed ")]
        [StringLength(50, ErrorMessage = "Page-21: Item 11. BITUMINOUS SURFACE COURSE- VI. Maximum fifty characters are allowed in Type and grade of binder used")]
        public string TYPE_GRADE_BINDER_21 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-21: Item 11. BITUMINOUS SURFACE COURSE- VII. Please enter Brand name of bitumen supplier")]
        //[RegularExpression(@"^[A-Za-z0-9 ]{0,100}$", ErrorMessage = "Page-21: Maximum hundred characters are allowed ")]
        [StringLength(100, ErrorMessage = "Page-21: Item 11. BITUMINOUS SURFACE COURSE- VII. Maximum hundred characters are allowed in Brand name of bitumen supplier")]
        public string BRAND_NAME_BITUMEN_21 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-21: Item 11. BITUMINOUS SURFACE COURSE- VIII. Please select Whether the invoices for the whole quantity of bitumenused at site are available")]
        //[RegularExpression(@"^[A-Za-z ]{0,1}$", ErrorMessage = "Page-21: Maximum one character is allowed ")]
        [StringLength(10, ErrorMessage = "Page-21: Item 11. BITUMINOUS SURFACE COURSE- VIII. Maximum one character is allowed in Whether the invoices for the whole quantity of bitumenused at site are available")]
        public string IS_BITUMEN_USED_21 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-21: Item 11. BITUMINOUS SURFACE COURSE- IX. Please enter in If invoice of sufficient quantity not available reason thereof")]

        // [RegularExpression(@"^[A-Za-z0-9 ]{0,150}$", ErrorMessage = "Page-21: Maximum one fifty characters are allowed ")]
        [StringLength(150, ErrorMessage = "Page-21: Item 11. BITUMINOUS SURFACE COURSE- IX. Maximum one fifty characters are allowed in If invoice of sufficient quantity not available reason thereof")]
        public string INVOICE_INSUFFICIENT_REASON_21 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-21 Item 11. BITUMINOUS SURFACE COURSE- X. Please enter Bitumen content as per DPR")]
        //[RegularExpression(@"^[0-9]{0,3}(?:\.[0-9]{1,2})?$", ErrorMessage = "Page-21: Maximum three digits before decimal and maximum two digits after decimal are allowed ")]
        [RegularExpression(@"^(?:\d{0,3}\.\d{1,2})$|^\d{0,3}$", ErrorMessage = "Page-21: Item 11. BITUMINOUS SURFACE COURSE- X. Maximum 999.99 is allowed and maximum two digits after decimal are allowed in Bitumen content as per DPR")]
        public decimal PERCENT_BITUMEN_CONTENT_21 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-21: Item 11. BITUMINOUS SURFACE COURSE- XI. Please select Whether tack coat is applied")]
        //[RegularExpression(@"^[A-Za-z ]{0,1}$", ErrorMessage = "Page-21: Maximum one character is allowed ")]
        [StringLength(1, ErrorMessage = "Page-21: Item 11. BITUMINOUS SURFACE COURSE- XI. Maximum one character is allowed in Whether tack coat is applied")]
        public string IS_TACK_COAT_USED_21 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        //[RegularExpression(@"^[A-Za-z ]{0,1}$", ErrorMessage = "Page-21: Maximum one character is allowed ")]
        [StringLength(1, ErrorMessage = "Page-21: Item 11. BITUMINOUS SURFACE COURSE- XII. Maximum one character is allowed in Whether mix design done")]
        public string IS_MIX_DESIGN_21 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [RegularExpression(@"^[0-9]{0,7}(?:\.[0-9]{1,3})?$", ErrorMessage = "Page-21: Item 11. BITUMINOUS SURFACE COURSE- XIII. Maximum seven digits before decimal and maximum three digits after decimal are allowed in Marshal stability as per mix design")]
        public Nullable<decimal> MARSHAL_STAB_21 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-21:  Item 11. BITUMINOUS SURFACE COURSE- XIV. Please select Any signs of distress on surface")]
        //[RegularExpression(@"^[A-Za-z ]{0,1}$", ErrorMessage = "Page-21: Maximum one character is allowed ")]
        [StringLength(1, ErrorMessage = "Page-21:  Item 11. BITUMINOUS SURFACE COURSE- XIV. Maximum one character is allowed in Any signs of distress on surface")]
        public string IS_SIGN_DISTRESS_21 { get; set; }

        [FieldType(PropertyType = PDFFieldType.CheckBox)]
        //[RegularExpression(@"^[A-Za-z ]{0,1}$", ErrorMessage = "Page-21: Maximum one character is allowed ")]
        [StringLength(1, ErrorMessage = "Page-21: Item 11. BITUMINOUS SURFACE COURSE- XIV. Maximum one character is allowed in Due to laying at low temperature")]
        public string DISTRESS_REASON_21_1 { get; set; }

        [FieldType(PropertyType = PDFFieldType.CheckBox)]
        //[RegularExpression(@"^[A-Za-z ]{0,1}$", ErrorMessage = "Page-21: Maximum one character is allowed ")]
        [StringLength(1, ErrorMessage = "Page-21: Item 11. BITUMINOUS SURFACE COURSE- XIV. Maximum one character is allowed in Due to poor workmanship of base course")]
        public string DISTRESS_REASON_21_2 { get; set; }

        [FieldType(PropertyType = PDFFieldType.CheckBox)]
        //[RegularExpression(@"^[A-Za-z ]{0,1}$", ErrorMessage = "Page-21: Maximum one character is allowed ")]
        [StringLength(1, ErrorMessage = "Page-21: Item 11. BITUMINOUS SURFACE COURSE- XIV. Maximum one character is allowed in Due to over rolling")]
        public string DISTRESS_REASON_21_3 { get; set; }

        [FieldType(PropertyType = PDFFieldType.CheckBox)]
        //[RegularExpression(@"^[A-Za-z ]{0,1}$", ErrorMessage = "Page-21: Maximum one character is allowed ")]
        [StringLength(1, ErrorMessage = "Page-21: Item 11. BITUMINOUS SURFACE COURSE- XIV. Maximum one character is allowed in Due to less/excess bitumen content")]
        public string DISTRESS_REASON_21_4 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-22: Item 11. BITUMINOUS SURFACE COURSE- Please select Item Grading-11")]
        //[RegularExpression(@"^[A-Za-z ]{0,1}$", ErrorMessage = "Page-22: Maximum one character is allowed ")]
        [StringLength(3, ErrorMessage = "Page-22 Item 11. BITUMINOUS SURFACE COURSE- Maximum one character is allowed in Item Grading-11")]
        public string ITEM_GRADING_11_22 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-22: Item 11. BITUMINOUS SURFACE COURSE- Please enter suggestions for improvement")]
        //   [RegularExpression(@"^[A-Za-z ]{0,250}$", ErrorMessage = "Page-22: Maximum two hundred fifty characters are allowed ")]
        [StringLength(250, ErrorMessage = "Page-22: Item 11. BITUMINOUS SURFACE COURSE- Maximum two hundred fifty characters are allowed in suggestions for improvement")]
        public string IMPROVE_SUGGESTION_22 { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int QM_USER_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public string IPADD { get; set; }



    }
}