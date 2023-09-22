using PMGSY.Areas.EFORMArea.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.QMModels
{
    public class EFORM_QM_CC_PUCCA_DRAINS
    {
        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int DRAIN_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int EFORM_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int IMS_PR_ROAD_CODE { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-29: Item 19. CEMENT CONCRETE PUCCA DRAINS- I. Please select Whether sanctioned DPR has the provision of side drains and catch water drains ")]
        [StringLength(1, ErrorMessage = "Page-29: Item 19. CEMENT CONCRETE PUCCA DRAINS-  I. Maximum 1 character is allowed in Whether sanctioned DPR has the provision of side drains and catch water drains")]
        public string IS_DPR_PROVISION_29 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-29: Item 19. CEMENT CONCRETE PUCCA DRAINS-  II. Please select Shape of CC/Pucca side drain as per DPR ")]
        [StringLength(1, ErrorMessage = "Page-29: Item 19. CEMENT CONCRETE PUCCA DRAINS- II.  Maximum 1 character is allowed in Shape of CC/Pucca side drain as per DPR")]
        public string SHAPE_ASPER_DPR_29 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-29: Item 19. CEMENT CONCRETE PUCCA DRAINS-  III. Please Enter Length of CC drain as per DPR ")]
        [RegularExpression(@"^[0-9]{0,7}(?:\.[0-9]{1,3})?$", ErrorMessage = "Page-29: Item 19. CEMENT CONCRETE PUCCA DRAINS-  III. Maximum seven digits before decimal and maximum three digits after decimal are allowed in Length of CC drain as per DPR")]
        public decimal LENGTH_ASPER_DPR { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-29: Item 19. CEMENT CONCRETE PUCCA DRAINS-  IV. Please select Item execution status ")]
        [StringLength(20, ErrorMessage = "Page-29: Item 19. CEMENT CONCRETE PUCCA DRAINS-  IV. Maximum 20 characters are allowed in Item execution status")]
        public string ITEM_EXEC_STATUS_29 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-29: Item 19. CEMENT CONCRETE PUCCA DRAINS-  V. Please select Grade of concrete proposed for side drains ")]
        [StringLength(5, ErrorMessage = "Page-29: Item 19. CEMENT CONCRETE PUCCA DRAINS- V.  Maximum 5 characters are allowed in Grade of concrete proposed for side drains")]
        public string CONCRETE_PROP_GRADE_29 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-29: Item 19. CEMENT CONCRETE PUCCA DRAINS-  VII. Please select Whether the provision of CC/pucca side drains made in DPR is justified in your opinion: ")]
        [StringLength(1, ErrorMessage = "Page-29: Item 19. CEMENT CONCRETE PUCCA DRAINS-  VII. Maximum 1 character is allowed in Whether the provision of CC/pucca side drains made in DPR is justified in your opinion:")]
        public string IS_CC_DRAIN_JUSTIFIED_29 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-29: Item 19. CEMENT CONCRETE PUCCA DRAINS-  VIII. Please select Whether the side drains have been constructed as per the DPR ")]
        [StringLength(1, ErrorMessage = "Page-29: Item 19. CEMENT CONCRETE PUCCA DRAINS-  VIII. Maximum 1 character is allowed in Whether the side drains have been constructed as per the DPR")]
        public string IS_SIDE_DRAIN_CONSTRUCTED_29 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-29: Item 19. CEMENT CONCRETE PUCCA DRAINS-  IX. Please select If not, in your opinion, whether the pavement performance is likely to be adversely affected ")]
        [StringLength(1, ErrorMessage = "Page-29: Item 19. CEMENT CONCRETE PUCCA DRAINS-  IX. Maximum 1 character is allowed in If not, in your opinion, whether the pavement performance is likely to be adversely affected")]
        public string IS_PAVE_PERFORM_AFFECTED_29 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-29: Item 19. CEMENT CONCRETE PUCCA DRAINS-  X. Please select Whether surface texture of the drain is acceptable ")]
        [StringLength(1, ErrorMessage = "Page-29: Item 19. CEMENT CONCRETE PUCCA DRAINS-  X. Maximum 1 character is allowed in Whether surface texture of the drain is acceptable")]
        public string IS_SURFACE_TEXTURE_ACCEPTABLE_29 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-29: Item 19. CEMENT CONCRETE PUCCA DRAINS-  XI. Please select Whether surface of the drain is free of honeycombing ")]
        [StringLength(1, ErrorMessage = "Page-29: Item 19. CEMENT CONCRETE PUCCA DRAINS-  XI. Maximum 1 character is allowed in Whether surface of the drain is free of honeycombing")]
        public string IS_DRAIN_SURFACE_HONEYCOMBING_FREE_29 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-29: Item 19. CEMENT CONCRETE PUCCA DRAINS-  XII. Please select Whether longitudinal gradient is sufficient ")]
        [StringLength(1, ErrorMessage = "Page-29: Item 19. CEMENT CONCRETE PUCCA DRAINS-  XII. Maximum 1 character is allowed in Whether longitudinal gradient is sufficient")]
        public string IS_LONG_GRADIENT_SUFFICIENT_29 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-29: Item 19. CEMENT CONCRETE PUCCA DRAINS-  XIII. Please select Check that drain is terminating in stormwater drain ")]
        [StringLength(1, ErrorMessage = "Page-29: Item 19. CEMENT CONCRETE PUCCA DRAINS-  XIII. Maximum 1 character is allowed in Check that drain is terminating in stormwater drain")]
        public string IS_DRAIN_TERMINATE_29 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-29: Item 19. CEMENT CONCRETE PUCCA DRAINS-  XIV. Please select Slope of gap between pavement and drain is towards drain ")]
        [StringLength(1, ErrorMessage = "Page-29: Item 19. CEMENT CONCRETE PUCCA DRAINS-  XIV. Maximum 1 character is allowed in Slope of gap between pavement and drain is towards drain")]
        public string IS_SLOPE_EXIST_29 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-29: Item 19. CEMENT CONCRETE PUCCA DRAINS-  XV. Please select Whether the drains provided are serving the purpose ofstormwater drain ")]
        [StringLength(1, ErrorMessage = "Page-29: Item 19. CEMENT CONCRETE PUCCA DRAINS-  XV. Maximum 1 character is allowed in Whether the drains provided are serving the purpose ofstormwater drain")]
        public string IS_DRAINS_SERVE_SATISFIED_29 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-30: Item 19. CEMENT CONCRETE PUCCA DRAINS-  Please select Item Grading-19 ")]
        [StringLength(3, ErrorMessage = "Page-30: Item 19. CEMENT CONCRETE PUCCA DRAINS-  Maximum 3 character are allowed in Item Grading-19")]
        public string ITEM_GRADING_19 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-30: Item 19. CEMENT CONCRETE PUCCA DRAINS-  Please enter suggestions for improvement ")]
        [StringLength(250, ErrorMessage = "Page-30: Item 19. CEMENT CONCRETE PUCCA DRAINS-  Maximum 250 character are allowed in suggestions for improvement ")]
        public string IMPROVE_SUGGESTIONS_30 { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int QM_USER_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public string IPADD { get; set; }
    }
}