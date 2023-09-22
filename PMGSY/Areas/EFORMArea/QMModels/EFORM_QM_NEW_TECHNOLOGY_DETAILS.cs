using PMGSY.Areas.EFORMArea.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.QMModels
{
    public class EFORM_QM_NEW_TECHNOLOGY_DETAILS
    {

        public EFORM_QM_NEW_TECHNOLOGY_DETAILS(bool IsNewTechUsed_NT)
        {
            this.IsNewTechUsedNTStatus = IsNewTechUsed_NT;
        }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public bool IsNewTechUsedNTStatus { get; set; }


        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int TECH_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int EFORM_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int IMS_PR_ROAD_CODE { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-10: Item 5.EARTHWORK & SUB GRADE- I. Please select Whether new technology used in this layer ")]
        [StringLength(1, ErrorMessage = "Page-10: Item 5.EARTHWORK & SUB GRADE- I. Maximum 1 character is allowed in Whether new technology used in this layer")]
        public string IS_NEW_TECH_USED_10 { get; set; }

        [FieldType(PropertyType = PDFFieldType.ComboBox)]
        [IsNewTechUsedNTVal]
        [Required(ErrorMessage = "Page-10: Item 5.EARTHWORK & SUB GRADE- I. (a) Please select Name of new technology used ")]
       // [StringLength(100, ErrorMessage = "Page-10: Maximum 100 characters are allowed ")]
       
        public short TECH_NAME_10 { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-10: Item 5.EARTHWORK & SUB GRADE- I. (b) Please enter Name of new technology provider ")]

        [StringLength(100, ErrorMessage = "Page-10: Item 5.EARTHWORK & SUB GRADE- I. (b) Maximum 100 characters are allowed in Name of new technology provider")]
        [IsNewTechUsedNTVal]
        public string NEW_TECH_PROVIDER_10 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-10: Item 5.EARTHWORK & SUB GRADE- I. (c) Please enter Name of stabiliser used ")]

        [StringLength(100, ErrorMessage = "Page-10: Item 5.EARTHWORK & SUB GRADE- I. (c) Maximum 100 characters are allowed in Name of stabiliser used ")]
        [IsNewTechUsedNTVal]
        public string STABILIZER_NAME_10 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-10: Item 5.EARTHWORK & SUB GRADE- I. (d) Please enter Quantity of stabiliser as per DPR ")]
        [StringLength(100, ErrorMessage = "Page-10: Item 5.EARTHWORK & SUB GRADE- I. (d) Maximum 100 characters are allowed in Quantity of stabiliser as per DPR")]
        [IsNewTechUsedNTVal]
        public string STABILIZER_QTY_DPR_10 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-10: Item 5.EARTHWORK & SUB GRADE- I. (e) Please enter Quantity of stabiliser used ")]
        [StringLength(100, ErrorMessage = "Page-10: Item 5.EARTHWORK & SUB GRADE- I. (e) Maximum 100 characters are allowed in Quantity of stabiliser used")]
        [IsNewTechUsedNTVal]
        public string STABILIZER_QTY_USED_10 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-10: Item 5.EARTHWORK & SUB GRADE- I. (f) Please enter  Unconfined compressive strength (UCS) as per DPR")]
        [RegularExpression(@"^[0-9]{0,7}(?:\.[0-9]{1,3})?$", ErrorMessage = "Page-10: Item 5.EARTHWORK & SUB GRADE- I. (f) Maximum seven digits before decimal and maximum three digits after decimal are allowed in Unconfined compressive strength (UCS) as per DPR ")]
        [IsNewTechUsedNTVal]
        public Nullable<decimal> UCS_ASPER_DPR_10 { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [StringLength(100, ErrorMessage = "Page-11: Item 5.EARTHWORK & SUB GRADE- I. (g) i) maximum 100 characters are allowed in Name of new technology material used")]
        public string JC_TECH_NAME_11 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [StringLength(100, ErrorMessage = "Page-11: Item 5.EARTHWORK & SUB GRADE- I. (g) ii) maximum 100 characters are allowed in Name of new technology provider ")]
        public string JC_TECH_PROVIDER_11 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [StringLength(100, ErrorMessage = "Page-11: Item 5.EARTHWORK & SUB GRADE- I. (g) iii) Maximum 100 characters are allowed in Quantity of new technology material as per DPR")]
        public string JC_TECH_QTY_DPR_11 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [StringLength(100, ErrorMessage = "Page-11: Item 5.EARTHWORK & SUB GRADE- I. (g) iv) Maximum 100 characters are allowed in Quantity of new technology material used")]
        public string JC_TECH_QTY_USED_11 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [RegularExpression(pattern: @"^[A-Za-z]{0,1}$", ErrorMessage = "Page-9: Item 5.EARTHWORK & SUB GRADE- I. (g) v) Maximum one character is allowed in Whether the test certificate of material is provided by the jute / coir material provider ")]

        public string IS_TEST_CERT_PROVIDED_11 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [RegularExpression(@"^(?:\d{0,3}\.\d{1,2})$|^\d{0,3}$", ErrorMessage = "Page-11: Item 5.EARTHWORK & SUB GRADE- I. (g) vi) Maximum 999.99 is allowed and maximum two digits after decimal are allowed in CBR of subgrade as per DPR")]
        public Nullable<decimal> SUBGRADE_CBR_11 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        // [Required(ErrorMessage = "Page-11: Please select  ")]
        [StringLength(3, ErrorMessage = "Page-11: Item 5.EARTHWORK & SUB GRADE- I. Maximum 3 characters are allowed in Sub-Item Grading 5-I:")]
        public string SUBITEM_GRADING_5_I { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-11: Item 5.EARTHWORK & SUB GRADE- I. Please enter suggestions for improvement ")]
        [StringLength(250, ErrorMessage = "Page-11: Item 5.EARTHWORK & SUB GRADE- I. Maximum 250 characters are allowed in suggestions for improvement")]
        public string IMPROVEMENT_REMARK_11 { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int QM_USER_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public string IPADD { get; set; }

    }
}