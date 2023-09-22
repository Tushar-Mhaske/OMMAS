using PMGSY.Areas.EFORMArea.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.QMModels
{
    public class EFORM_QM_BITUMINOUS_BASE_COURSE
    {

        public EFORM_QM_BITUMINOUS_BASE_COURSE(bool IS_NEW_TECH_USED_19_val)
        {

            this.radioButtonValue = IS_NEW_TECH_USED_19_val;
        }


        [FieldType(PropertyType = PDFFieldType.Skip)]
        public bool radioButtonValue { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int LAYER_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int EFORM_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int IMS_PR_ROAD_CODE { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-19 Item 10. BITUMINOUS BASE COURSE- I. Please select Provision made in the sanctioned DPR")]
        //[RegularExpression(@"^[A-Za-z ]{0,15}$", ErrorMessage = "Page-19: Maximum fifteen characters are allowed ")]
        [StringLength(15, ErrorMessage = "Page-19: Item 10. BITUMINOUS BASE COURSE- I. Maximum fifteen characters are allowed in Provision made in the sanctioned DPR")]
        public string PROVISION_IN_DPR_19 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-19 Item 10. BITUMINOUS BASE COURSE- II. Please select Item execution status")]
        //[RegularExpression(@"^[A-Za-z ]{0,20}$", ErrorMessage = "Page-19: Maximum twenty characters are allowed ")]
        [StringLength(20, ErrorMessage = "Page-19: Item 10. BITUMINOUS BASE COURSE- II. Maximum twenty characters are allowed in Item execution status")]
        public string ITEM_EXECUTION_STATUS_19 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-19 Item 10. BITUMINOUS BASE COURSE- III. Please select Actual execution at the site")]
        //[RegularExpression(@"^[A-Za-z ]{0,20}$", ErrorMessage = "Page-19: Maximum twenty characters are allowed ")]
        [StringLength(20, ErrorMessage = "Page-19: Item 10. BITUMINOUS BASE COURSE- III. Maximum twenty characters are allowed in Actual execution at the site")]
        public string ACTUAL_EXECUTION_19 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-19 Item 10. BITUMINOUS BASE COURSE- IV. Please select Whether new technology used in this layer")]
        //[RegularExpression(@"^[A-Za-z ]{0,1}$", ErrorMessage = "Page-19: Maximum one character is allowed ")]
        [StringLength(1, ErrorMessage = "Page-19: Item 10. BITUMINOUS BASE COURSE- IV. Maximum one character is allowed in Whether new technology used in this layer")]
        public string IS_NEW_TECH_USED_19 { get; set; }

        [FieldType(PropertyType = PDFFieldType.ComboBox)]
        [Required(ErrorMessage = "Page-19: Item 10. BITUMINOUS BASE COURSE- IV. (a) Please select Name of technology used")]
        //[RegularExpression(@"^[A-Za-z0-9 ]{0,50}$", ErrorMessage = "Page-19: Maximum fifty characters are allowed ")]
       // [StringLength(50, ErrorMessage = "Page-19: Maximum fifty characters are allowed ")]
      
        [IsNewTechUsed19Val]
        public short NEW_TECH_NAME_19 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-19: Item 10. BITUMINOUS BASE COURSE- IV. (b) Please enter Name of technology provider")]
        //[RegularExpression(@"^[A-Za-z0-9 ]{0,50}$", ErrorMessage = "Page-19: Maximum fifty characters are allowed ")]
        [StringLength(50, ErrorMessage = "Page-19: Item 10. BITUMINOUS BASE COURSE- IV. (b) Maximum fifty characters are allowed in Name of technology provider")]
        

        [IsNewTechUsed19Val]
        public string NEW_TECH_PROVIDER_19 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-19: Item 10. BITUMINOUS BASE COURSE- IV. (c) Please enter Name of new technology material")]
        //[RegularExpression(@"^[A-Za-z0-9 ]{0,20}$", ErrorMessage = "Page-19: Maximum twenty characters are allowed ")]
        [StringLength(20, ErrorMessage = "Page-19: Item 10. BITUMINOUS BASE COURSE- IV. (c) Maximum twenty characters are allowed in Name of new technology material")]
        [IsNewTechUsed19Val]
        public string NEW_TECH_MATERIAL_NAME_19 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-19: Item 10. BITUMINOUS BASE COURSE- IV. (d) Please enter Quantity of new technology material as per DPR")]
        //[RegularExpression(@"^[A-Za-z0-9 ]{0,20}$", ErrorMessage = "Page-19: Maximum twenty characters are allowed ")]
        [StringLength(100, ErrorMessage = "Page-19: Item 10. BITUMINOUS BASE COURSE- IV. (d) Maximum 100 characters are allowed in Quantity of new technology material as per DPR")]
        [IsNewTechUsed19Val]
        public string NEW_TECH_QTY_MATERIAL_ASPER_DPR_19 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-19: Item 10. BITUMINOUS BASE COURSE- IV. (e) Please enter Quantity of new technology material used")]
        [StringLength(100, ErrorMessage = "Page-19: Item 10. BITUMINOUS BASE COURSE- IV. (e) Maximum 100 characters are allowed in Quantity of new technology material used")]

        [IsNewTechUsed19Val]
        public string NEW_TECH_QTY_MATERIAL_USED_19 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-19: Item 10. BITUMINOUS BASE COURSE- IV. (f) Please enter Location of new technology section: From RD")]
        [RegularExpression(@"^[0-9]{0,7}(?:\.[0-9]{1,3})?$", ErrorMessage = "Page-19: Item 10. BITUMINOUS BASE COURSE- IV. (f) Maximum seven digits before decimal and maximum three digits after decimal are allowed in Location of new technology section: From RD")]
        [IsNewTechUsed19Val]
        public Nullable<decimal> NEW_TECH_LOC_FROM_ROAD_19 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-19: Item 10. BITUMINOUS BASE COURSE- IV. (f) Please enter Location of new technology section: To RD")]
        [RegularExpression(@"^[0-9]{0,7}(?:\.[0-9]{1,3})?$", ErrorMessage = "Page-19: Item 10. BITUMINOUS BASE COURSE- IV. (f) Maximum seven digits before decimal and maximum three digits after decimal are allowed in Location of new technology section: To RD")]
        [IsNewTechUsed19Val]
        public Nullable<decimal> NEW_TECH_LOC_TO_ROAD_19 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-19: Item 10. BITUMINOUS BASE COURSE- IV. (g) Please enter Evaluation of new technology section")]
        //[RegularExpression(@"^[A-Za-z ]{0,1}$", ErrorMessage = "Page-19: Maximum one character is allowed ")]
        [StringLength(1, ErrorMessage = "Page-19: Item 10. BITUMINOUS BASE COURSE- IV. (g) Maximum one character is allowed in Evaluation of new technology section")]
        [IsNewTechUsed19Val]
        public string GRADING_NEW_TECH_EVALUATION_19 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-19: Item 10. BITUMINOUS BASE COURSE- V. Please enter Thickness of layer as per DPR")]
        [RegularExpression(@"^[0-9]{0,7}(?:\.[0-9]{1,3})?$", ErrorMessage = "Page-19: Item 10. BITUMINOUS BASE COURSE- V. Maximum seven digits before decimal and maximum three digits after decimal are allowed in Thickness of layer as per DPR")]
        public decimal THICKNESS_ASPER_DPR_19 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-19: Item 10. BITUMINOUS BASE COURSE- VI. Please enter Type and grade of binder used")]
        //[RegularExpression(@"^[A-Za-z0-9 ]{0,50}$", ErrorMessage = "Page-19: Maximum fifty characters are allowed ")]
        [StringLength(50, ErrorMessage = "Page-19 Item 10. BITUMINOUS BASE COURSE- VI. Maximum fifty characters are allowed in Type and grade of binder used")]
        public string TYPE_GRADE_BINDER_19 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-19: Item 10. BITUMINOUS BASE COURSE- VII. Please enter Brand name of bitumen")]
        //[RegularExpression(@"^[A-Za-z0-9 ]{0,100}$", ErrorMessage = "Page-19: Maximum hundred characters are allowed ")]
        [StringLength(100, ErrorMessage = "Page-19 Item 10. BITUMINOUS BASE COURSE- VII. Maximum hundred characters are allowed in Brand name of bitumen")]
        public string BRAND_NAME_BITUMEN_19 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-19: Item 10. BITUMINOUS BASE COURSE- VIII. Please select Whether the invoices for the whole quantity of bitumenused at site are available")]
        //[RegularExpression(@"^[A-Za-z ]{0,1}$", ErrorMessage = "Page-19: Maximum one character is allowed ")]
        [StringLength(1, ErrorMessage = "Page-19 Item 10. BITUMINOUS BASE COURSE- VIII. Maximum one character is allowed in Whether the invoices for the whole quantity of bitumenused at site are available")]
        public string IS_BITUMEN_USED_19 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        //[RegularExpression(@"^[A-Za-z0-9 ]{0,150}$", ErrorMessage = "Page-19: Maximum one fifty character are allowed ")]
        [StringLength(150, ErrorMessage = "Page-19: Item 10. BITUMINOUS BASE COURSE- IX. Maximum one fifty character are allowed in If the invoice of sufficient quantity not available reason thereof")]
        public string INVOICE_INSUFFICIENT_REASON_19 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-19: Item 10. BITUMINOUS BASE COURSE- X. Please enter Bitumen content as per DPR")]
        [RegularExpression(@"^[0-9]{0,3}(?:\.[0-9]{1,2})?$", ErrorMessage = "Page-19: Item 10. BITUMINOUS BASE COURSE- X. Maximum three digits before decimal and maximum two digits after decimal are allowed in Bitumen content as per DPR")]
        public decimal PERCENT_BITUMEN_CONTENT_19 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-19: Item 10. BITUMINOUS BASE COURSE- XI. Please select Whether prime coat is applied on non bituminous course")]
        //[RegularExpression(@"^[A-Za-z ]{0,1}$", ErrorMessage = "Page-19: Maximum one character is allowed ")]
        [StringLength(1, ErrorMessage = "Page-19 Item 10. BITUMINOUS BASE COURSE- XI. Maximum one character is allowed in Whether prime coat is applied on non bituminous course")]
        public string IS_PRIME_COAT_USED_19 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-19: Item 10. BITUMINOUS BASE COURSE- XII. Please enter Laying temperature of the mix as per QCR-1")]
        [RegularExpression(@"^[0-9]{0,7}(?:\.[0-9]{1,3})?$", ErrorMessage = "Page-19: Item 10. BITUMINOUS BASE COURSE- XII. Maximum seven digits before decimal and maximum three digits after decimal are allowed in Laying temperature of the mix as per QCR-1")]
        public decimal LAYING_TEMP_19 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        //[RegularExpression(@"^[A-Za-z ]{0,1}$", ErrorMessage = "Page-19: Maximum one character is allowed")]
        [StringLength(1, ErrorMessage = "Page-19: Item 10. BITUMINOUS BASE COURSE- XIII. Maximum one character is allowed in Whether mix design is done")]
        public string IS_MIX_DESIGN_19 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [RegularExpression(@"^[0-9]{0,7}(?:\.[0-9]{1,3})?$", ErrorMessage = "Page-19: Item 10. BITUMINOUS BASE COURSE- XIV. Maximum seven digits before decimal and maximum three digits after decimal are allowed in Marshal stability as per mix design")]
        public Nullable<decimal> MARSHAL_STAB_19 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [RegularExpression(@"^[0-9]{0,7}(?:\.[0-9]{1,3})?$", ErrorMessage = "Page-19: Item 10.BITUMINOUS BASE COURSE- XV. Maximum seven digits before decimal and maximum three digits after decimal are allowed in Design density as per mix design")]
        public Nullable<decimal> DESIGN_DENSITY_19 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-19: Item 10. BITUMINOUS BASE COURSE- XVI. Please select Whether inspection of hot mix plant done by PIU/SE:")]
        //[RegularExpression(@"^[A-Za-z ]{0,1}$", ErrorMessage = "Page-19: Maximum one character is allowed ")]
        [StringLength(1, ErrorMessage = "Page-19 Item 10. BITUMINOUS BASE COURSE- XVI. Maximum one character is allowed in Whether inspection of hot mix plant done by PIU/SE:")]
        public string IS_HOT_MIX_DONE_19 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-19: Item 10. BITUMINOUS BASE COURSE- Please select Date of inspection")]
        [RegularExpression(pattern: @"^([0]?[0-9]|[12][0-9]|[3][01])/([0]?[1-9]|[1][0-2])/([0-9]{4})$", ErrorMessage = "Page-19: Item 10. BITUMINOUS BASE COURSE- Please Enter Valid date{in dd/mm/yyyy format} in Date of inspection ")]

        public Nullable<System.DateTime> INSP_DATE_19_af_date { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-20: Item 10. BITUMINOUS BASE COURSE- Please select Item Grading-10")]
        //[RegularExpression(@"^[A-Za-z ]{0,3}$", ErrorMessage = "Page-20: Maximum three characters are allowed ")]
        [StringLength(3, ErrorMessage = "Page-20 Item 10. BITUMINOUS BASE COURSE- X. Maximum three characters are allowed in Item Grading-10")]
        public string ITEM_GRADING_10 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-20: Item 10. BITUMINOUS BASE COURSE- Please enter suggestions for improvement")]
        //  [RegularExpression(@"^[A-Za-z ]{0,150}$", ErrorMessage = "Page-20: Maximum one fifty characters are allowed ")]
        [StringLength(150, ErrorMessage = "Page-20: Item 10. BITUMINOUS BASE COURSE- Maximum one fifty characters are allowed in suggestions for improvement")]
        public string IMPROVE_SUGGESTION_20 { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int QM_USER_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public string IPADD { get; set; }

    }
}