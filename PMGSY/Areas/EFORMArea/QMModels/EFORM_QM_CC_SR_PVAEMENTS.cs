using PMGSY.Areas.EFORMArea.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.QMModels
{
    public class EFORM_QM_CC_SR_PVAEMENTS
    {
        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int PAVE_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int EFORM_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int IMS_PR_ROAD_CODE { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-27: Item 18.CEMENT CONCRETE/SEMI-RIGID (SR) PAVEMENTS- I. Please Select Item execution status ")]
        [StringLength(20, ErrorMessage = "Page-27: Item 18.CEMENT CONCRETE/SEMI-RIGID (SR) PAVEMENTS-  I. Maximum 20 characters are allowed in Item execution status")]
        public string ITEM_EXEC_STATUS_27 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-27: Item 18.CEMENT CONCRETE/SEMI-RIGID (SR) PAVEMENTS-  II. Please Select Type of Cement Concrete Pavement ")]
        [StringLength(50, ErrorMessage = "Page-27: Item 18.CEMENT CONCRETE/SEMI-RIGID (SR) PAVEMENTS-  II. Maximum 50 characters are allowed in Type of Cement Concrete Pavement")]
        public string CCP_TYPE_27 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-27: Item 18.CEMENT CONCRETE/SEMI-RIGID (SR) PAVEMENTS-  III. Please Select Grade of Concrete as per DPR ")]
        [StringLength(3, ErrorMessage = "Page-27: Item 18.CEMENT CONCRETE/SEMI-RIGID (SR) PAVEMENTS-  III. Maximum 3 characters are allowed in Grade of Concrete as per DPR")]
        public string CONCRETE_GRADE_ASPER_DPR_27 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-27: Item 18.CEMENT CONCRETE/SEMI-RIGID (SR) PAVEMENTS-  IV. Please Enter CC /SR pavement length proposed as per sanctioned DPR: Proposed length ")]
        [RegularExpression(@"^[0-9]{0,7}(?:\.[0-9]{1,3})?$", ErrorMessage = "Page-27: Item 18.CEMENT CONCRETE/SEMI-RIGID (SR) PAVEMENTS-  IV. Maximum seven digits before decimal and maximum three digits after decimal are allowed in CC /SR pavement length proposed as per sanctioned DPR: Proposed length ")]
        public Nullable<decimal> CC_SR_PROPOSED_LENGTH_27 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-27: Item 18.CEMENT CONCRETE/SEMI-RIGID (SR) PAVEMENTS-  IV. Please Enter CC /SR pavement length proposed as per sanctioned DPR: Executed length ")]
        [RegularExpression(@"^[0-9]{0,7}(?:\.[0-9]{1,3})?$", ErrorMessage = "Page-27: Item 18.CEMENT CONCRETE/SEMI-RIGID (SR) PAVEMENTS-  IV. Maximum seven digits before decimal and maximum three digits after decimal are allowed in CC /SR pavement length proposed as per sanctioned DPR: Executed length")]
        public Nullable<decimal> CC_SR_EXECUTED_LENGTH_27 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-28: Item 18.CEMENT CONCRETE/SEMI-RIGID (SR) PAVEMENTS-  VI. Please Select Whether strength of extracted cc core or strength from rebound hammer is acceptable ")]
        [StringLength(1, ErrorMessage = "Page-28: Item 18.CEMENT CONCRETE/SEMI-RIGID (SR) PAVEMENTS-  VI. Maximum 1 character is allowed in Whether strength of extracted cc core or strength from rebound hammer is acceptable")]
        public string IS_CC_CORE_STRENGTH_ACCEPTABLE_28 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-28: Item 18.CEMENT CONCRETE/SEMI-RIGID (SR) PAVEMENTS-  VII. Please Select Whether Expansion/construction joints are provided as per requirement ")]
        [StringLength(1, ErrorMessage = "Page-28: Item 18.CEMENT CONCRETE/SEMI-RIGID (SR) PAVEMENTS-  VII. Maximum 1 character is allowed in Whether Expansion/construction joints are provided as perrequirement")]
        public string IS_EXPANS_CONCTRUCT_PROVIDED_28 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-28: Item 18.CEMENT CONCRETE/SEMI-RIGID (SR) PAVEMENTS-  VIII. Please Select Quality of cuts and joints is acceptable ")]
        [StringLength(1, ErrorMessage = "Page-28: Item 18.CEMENT CONCRETE/SEMI-RIGID (SR) PAVEMENTS-  VIII. Maximum 1 character is allowed in Quality of cuts and joints is acceptable")]
        public string IS_CUTS_JOINTS_ACCEPTABLE_28 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-28: Item 18.CEMENT CONCRETE/SEMI-RIGID (SR) PAVEMENTS-  IX. Please Select Whether the joints have been properly filled with a sealant ")]
        [StringLength(1, ErrorMessage = "Page-28: Item 18.CEMENT CONCRETE/SEMI-RIGID (SR) PAVEMENTS-  IX. Maximum 1 character is allowed in Whether the joints have been properly filled with a sealant")]
        public string IS_JOINTS_FILLED_28 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-28: Item 18.CEMENT CONCRETE/SEMI-RIGID (SR) PAVEMENTS-  X. Please select Whether surface texture of the pavement is acceptable ")]
        [StringLength(1, ErrorMessage = "Page-28: Item 18.CEMENT CONCRETE/SEMI-RIGID (SR) PAVEMENTS-  X. Maximum 1 character is allowed in Whether surface texture of the pavement is acceptable")]
        public string IS_SURFACE_TEXTURE_ACCEPTABLE_28 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-28: Item 18.CEMENT CONCRETE/SEMI-RIGID (SR) PAVEMENTS-  XI. Please Select Whether edges of the pavement are free from honeycombing ")]
        [StringLength(1, ErrorMessage = "Page-28: Item 18.CEMENT CONCRETE/SEMI-RIGID (SR) PAVEMENTS-  XI. Maximum 1 character is allowed in Whether edges of the pavement are free from honeycombing")]
        public string IS_EDGES_FREE_28 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-28: Item 18.CEMENT CONCRETE/SEMI-RIGID (SR) PAVEMENTS-  XII. Please Select Whether adequate camber is provided ")]
        [StringLength(1, ErrorMessage = "Page-28: Item 18.CEMENT CONCRETE/SEMI-RIGID (SR) PAVEMENTS-  XII. Maximum 1 character is allowed in Whether adequate camber is provided")]
        public string IS_CAMBER_PROVIDED_28 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-28: Item 18.CEMENT CONCRETE/SEMI-RIGID (SR) PAVEMENTS- XIII. Please Select Whether CC pavement was existing earlier and credit for the same was given in DPR ")]
        [StringLength(1, ErrorMessage = "Page-28: Item 18.CEMENT CONCRETE/SEMI-RIGID (SR) PAVEMENTS- XIII. Maximum 1 character is allowed in Whether CC pavement was existing earlier and credit for the same was given in DPR")]
        public string IS_CC_PAVEMENT_EXIST_28 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-28: Item 18.CEMENT CONCRETE/SEMI-RIGID (SR) PAVEMENTS-  Please Select Item Grading-18 ")]
        [StringLength(3, ErrorMessage = "Page-28: Item 18.CEMENT CONCRETE/SEMI-RIGID (SR) PAVEMENTS-  Maximum 3 character is allowed in Item Grading-18")]
        public string ITEM_GRADING_18 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-28: Item 18.CEMENT CONCRETE/SEMI-RIGID (SR) PAVEMENTS-  Please enter suggestions for improvement ")]
        [StringLength(250, ErrorMessage = "Page-28: Item 18.CEMENT CONCRETE/SEMI-RIGID (SR) PAVEMENTS-  Maximum 250 character are allowed in suggestions for improvement")]
        public string IMPROVE_SUGGESTIONS_28 { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int QM_USER_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public string IPADD { get; set; }
    }
}