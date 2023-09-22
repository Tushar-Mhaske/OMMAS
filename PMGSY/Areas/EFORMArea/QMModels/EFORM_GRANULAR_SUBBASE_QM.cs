using PMGSY.Areas.EFORMArea.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.QMModels
{
    public class EFORM_GRANULAR_SUBBASE_QM
    {

        public EFORM_GRANULAR_SUBBASE_QM(bool IsNewTechUsed)
        {
            this.IsNewTechUsedGSStatus = IsNewTechUsed;
        }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public bool IsNewTechUsedGSStatus { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-14: Item 6.GRANULAR SUB-BASE (GSB)- I. Please select Provision made in the sanctioned DPR ")]
        [RegularExpression(@"^[A-Za-z]{0,1}$", ErrorMessage = "Page-14: Item 6.GRANULAR SUB-BASE (GSB)- I. Maximum one character is allowed in Provision made in the sanctioned DPR")]
        public string IS_PROVISION_IN_DPR_14 { get; set; }
      

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-14: Item 6.GRANULAR SUB-BASE (GSB)- II. Please select Item execution status ")]
        [RegularExpression(@"^[A-Za-z ]{0,20}$", ErrorMessage = "Page-14 Item 6.GRANULAR SUB-BASE (GSB)- II.  Maximum twenty character is allowed{digits are not allowed} in Item execution status")]
        public string EXECUTION_STATUS_14 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-14: Item 6.GRANULAR SUB-BASE (GSB)- III. Please select Whether new technology used in this layer ")]
        [RegularExpression(@"^[A-Za-z ]{0,1}$", ErrorMessage = "Page-14: Item 6.GRANULAR SUB-BASE (GSB)- III. Maximum one character is allowed in Whether new technology used in this layer")]

        public string IS_NEW_TECH_USED_14 { get; set; }

        [FieldType(PropertyType = PDFFieldType.ComboBox)]
        [IsNewTechGSDependableGS]
   
        [Required(ErrorMessage = "Page-14: Item 6.GRANULAR SUB-BASE (GSB)- III. (a) Please select Name of new technology used ")]
        public short NEW_TECH_NAME_14 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [IsNewTechGSDependableGS]
        
        [StringLength(maximumLength: 50, ErrorMessage = "Page-14: Item 6.GRANULAR SUB-BASE (GSB)- III. (b) Only 50 Characters Allowed in Name of new technology provider")]
        [Required(ErrorMessage = "Page-14: Item 6.GRANULAR SUB-BASE (GSB)- III. (b) Please enter Name of new technology provider ")]
        public string NEW_TECH_PROVIDER_14 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [IsNewTechGSDependableGS]
     
        [StringLength(maximumLength: 50, ErrorMessage = "Page-14: Item 6.GRANULAR SUB-BASE (GSB)- III. (c) Only 50 Characters Allowed in Name of stabiliser used")]
        [Required(ErrorMessage = "Page-14: Item 6.GRANULAR SUB-BASE (GSB)- III. (c) Please enter Name of stabiliser used ")]
        public string NAME_STABILISER_USED_14 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [IsNewTechGSDependableGS]
        [Required(ErrorMessage = "Page-14: Item 6.GRANULAR SUB-BASE (GSB)- III. (d) Please enter Quantity of stabiliser as per DPR ")]
        [StringLength(100, ErrorMessage = "Page-14: Item 6.GRANULAR SUB-BASE (GSB)- III. (d) Maximum 100 characters are allowed in Quantity of stabiliser as per DPR")]

        public string STABILISER_QTY_ASPER_DPR_14 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [IsNewTechGSDependableGS]
        [Required(ErrorMessage = "Page-14: Item 6.GRANULAR SUB-BASE (GSB)- III. (e) Please enter Quantity of stabiliser used ")]
        [StringLength(100, ErrorMessage = "Page-14: Item 6.GRANULAR SUB-BASE (GSB)- III. (e) Maximum 100 characters are allowed in Quantity of stabiliser used")]

        public string STABILISER_QTY_USED_14 { get; set; }
        
        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [IsNewTechGSDependableGS]
        [Required(ErrorMessage = "Page-14: Item 6.GRANULAR SUB-BASE (GSB)- III. (f) Please enter Unconfined compressive strength (UCS) as per DPR ")]
        [RegularExpression(@"^(?:\d{0,7}\.\d{1,3})$|^\d{0,7}$", ErrorMessage = "Page-14 Item 6.GRANULAR SUB-BASE (GSB)- III. (f) Maximum seven digits before decimal and maximum three digits after decimal are allowed in Unconfined compressive strength (UCS) as per DPR")]
        public Nullable<decimal> UCS_ASPER_DPR_14 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-14: Item 6.GRANULAR SUB-BASE (GSB)- IV. Please select GSB Grading as per DPR ")]
        [StringLength(maximumLength: 10, ErrorMessage = "Page-14: Item 6.GRANULAR SUB-BASE (GSB)- IV. Only 10 Characters Allowed in GSB Grading as per DPR")]

        public string GSB_GRADING_ASPER_DPR_14 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-14: Item 6.GRANULAR SUB-BASE (GSB)- V. Please enter Maximum Dry density ")]
        [RegularExpression(@"^(?:\d{0,7}\.\d{1,3})$|^\d{0,7}$", ErrorMessage = "Page-14 Item 6.GRANULAR SUB-BASE (GSB)- V. Maximum seven digits before decimal and maximum three digits after decimal are allowed in Maximum Dry density")]

        public decimal MDD_14 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-14: Item 6.GRANULAR SUB-BASE (GSB)- VI. Please enter Optimum Moisture Content ")]
        [RegularExpression(@"^(?:\d{0,3}\.\d{1,2})$|^\d{0,3}$", ErrorMessage = "Page-14 Item 6.GRANULAR SUB-BASE (GSB)- VI. Maximum 999.99 is allowed and maximum two digits after decimal are allowed in Optimum Moisture Content")]

        public decimal OMC_14 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-15: Item 6.GRANULAR SUB-BASE (GSB)- VIII. Please select Whether GSB is to be constructed in layers as per DPR ")]
        [RegularExpression(@"^[A-Za-z]{0,1}$", ErrorMessage = "Page-15: Item 6.GRANULAR SUB-BASE (GSB)- VIII. Maximum one character is allowed in Whether GSB is to be constructed in layers as per DPR")]

        public string IS_GSB_ASPER_DPR { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [RegularExpression(@"^[0-9 ]{0,1}$", ErrorMessage = "Page-15: Item 6.GRANULAR SUB-BASE (GSB)- VIII. Maximum one character is allowed in number of layers")]

        public string LAYERS_ASPER_DPR { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-15: Item 6.GRANULAR SUB-BASE (GSB)- IX. Please select Whether GSB has been actually constructed in layers at site ")]
        [RegularExpression(@"^[A-Za-z]{0,1}$", ErrorMessage = "Page-15: Item 6.GRANULAR SUB-BASE (GSB)- IX. Maximum one character is allowed in Whether GSB has been actually constructed in layers at site")]

        public string IS_GSB_ATSITE { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [RegularExpression(@"^[0-9 ]{0,1}$", ErrorMessage = "Page-15: Item 6.GRANULAR SUB-BASE (GSB)- IX. Maximum one character is allowed in number of layers")]

        public string LAYERS_ATSITE { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-15: Item 6.GRANULAR SUB-BASE (GSB)- X. Please select Whether compaction has been done as per the provision in DPR ")]
        [RegularExpression(@"^[A-Za-z]{0,1}$", ErrorMessage = "Page-15: Item 6.GRANULAR SUB-BASE (GSB)- X. Maximum one character is allowed in Whether compaction has been done as per the provision in DPR")]

        public string IS_COMPAQ { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-15: Item 6.GRANULAR SUB-BASE (GSB)- Please select Item Grading-6")]
        [RegularExpression(@"^[A-Za-z]{0,3}$", ErrorMessage = "Page-15: Item 6.GRANULAR SUB-BASE (GSB)- Maximum three character is allowed in Item Grading-6")]

        public string ITEM_GRADING_6 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [StringLength(maximumLength: 250, ErrorMessage = "Page-15: Item 6.GRANULAR SUB-BASE (GSB)- Only 250 Characters Allowed in suggestions for improvement")]
        [Required(ErrorMessage = "Page-15: Item 6.GRANULAR SUB-BASE (GSB)- Please enter suggestions for improvement ")]
        public string IMPROVEMENT_REMARK_15 { get; set; }

        
    }
}