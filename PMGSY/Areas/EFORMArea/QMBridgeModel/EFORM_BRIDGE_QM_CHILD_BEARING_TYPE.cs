using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;
using System.ComponentModel.DataAnnotations;
using PMGSY.Areas.EFORMArea.Model;

namespace PMGSY.Areas.EFORMArea.QMBridgeModel
{
    public class EFORM_BRIDGE_QM_CHILD_BEARING_TYPE
    {

        [FieldType(PropertyType = PDFFieldType.CheckBox)]
        [Required(ErrorMessage = "Page-23: Item 8. BEARINGS- B) Condition of Bearings- (a) Metallic Bearings- only (Y/N) is allowed in	Metallic Bearings checkbox")]
        [RegularExpression(pattern: @"^[Y,N]{1}$", ErrorMessage = "Page-23: Item 8. BEARINGS- B) Condition of Bearings- (a) Metallic Bearings- only (Y/N) is allowed in	Metallic Bearings checkbox ")]
        public string IS_METALLIC_BEARING { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-23: Item 8. BEARINGS- B) Condition of Bearings- (a) Metallic Bearings- i. Please select General Condition- any rusting / ceasing of plates/ cleanliness or not")]
        [StringLength(3, ErrorMessage = "Page-23: Item 8. BEARINGS- B) Condition of Bearings- (a) Metallic Bearings- i. The length must be 3 character or less for General Condition- any rusting / ceasing of plates/ cleanliness or not ")]

        public string IS_RUSTED { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-23: Item 8. BEARINGS-B) Condition of Bearings- (a) Metallic Bearings- ii. Please select Functioning –any excessive movement/ tilting/ jumping off guides ")]
        [StringLength(3, ErrorMessage = "Page-23: Item 8. BEARINGS-B) Condition of Bearings- (a) Metallic Bearings- ii. The length must be 3 character or less for Functioning –any excessive movement/ tilting/ jumping off guides ")]
        public string IS_FUNCTIONING { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-23: Item 8. BEARINGS- B) Condition of Bearings- (a) Metallic Bearings- iii. Please select Greasing/ oil bath required to be redone or not ")]
        [StringLength(3, ErrorMessage = "Page-23: Item 8. BEARINGS-B) Condition of Bearings- (a) Metallic Bearings- iii. The length must be 3 character or less for Greasing/ oil bath required to be redone or not ")]

        public string IS_GREASE_REQUIRED { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
         [Required(ErrorMessage = "Page-23: Item 8. BEARINGS- B) Condition of Bearings- (a) Metallic Bearings- iv. Please select Cracks in supporting member- Abutment cap, Pier cap, Pedestal, if any")]
        [StringLength(3, ErrorMessage = "Page-23: Item 8. BEARINGS- B) Condition of Bearings- (a) Metallic Bearings- iv. The length must be 3 character or less for iv.	Cracks in supporting member- Abutment cap, Pier cap, Pedestal, if any ")]

        public string IS_CRACK_IN_SUPPORRT_MEMBER { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-23: Item 8. BEARINGS- B) Condition of Bearings- (a) Metallic Bearings- v. Please select  Effectiveness of anchor bolts, whether in position and tight or not  ")]
        [StringLength(3, ErrorMessage = "Page-23: Item 8. BEARINGS- B) Condition of Bearings- (a) Metallic Bearings- v. The length must be 3 character or less for Effectiveness of anchor bolts, whether in position and tight or not ")]

        public string IS_EFFECTIVE_ANCHOR_BOLT { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
      //  [Required(ErrorMessage = "Page-23: Item 8. BEARINGS- B) Condition of Bearings- (a) Metallic Bearings- vi. Please enter report any other defect or observation found  ")]
        [StringLength(250, ErrorMessage = "Page-23: Item 8. BEARINGS- B) Condition of Bearings- (a) Metallic Bearings- vi. The length must be 250 character or less for report any other defect or observation found  ")]

        public string OTHER_DEFECT_IN_METALLIC { get; set; }

        [FieldType(PropertyType = PDFFieldType.CheckBox)]
        [Required(ErrorMessage = "Page-23: Item 8. BEARINGS- B) Condition of Bearings- (b) Elastomeric Bearings- only (Y/N) is allowed in Elastomeric Bearings checkbox")]
        [RegularExpression(pattern: @"^[Y,N]{1}$", ErrorMessage = "Page-23: Item 8. BEARINGS- B) Condition of Bearings- (b) Elastomeric Bearings- only (Y/N) is allowed in Elastomeric Bearings checkbox ")]
        public string IS_ELASTOMETRIC_BEARING { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
       [Required(ErrorMessage = "Page-24: Item 8. BEARINGS- B) Condition of Bearings- (b)  Elastomeric Bearings- i. Please select Report condition of pads – Oxidation, Creep, Flattening, Bulging, Splitting, Displacement if any ")]
        [StringLength(3, ErrorMessage = "Page-24: Item 8. BEARINGS- B) Condition of Bearings- (b)  Elastomeric Bearings- i. The length must be 3 character or less for Report condition of pads – Oxidation, Creep, Flattening, Bulging, Splitting, Displacement if any ")]

        public string IS_PAD_COND_BAD { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-24: Item 8. BEARINGS- B) Condition of Bearings- (b) Elastomeric Bearings- ii. Please select  Report – Bearings are clean and free or not ")]
        [StringLength(3, ErrorMessage = "Page-24: Item 8. BEARINGS- B) Condition of Bearings- (b) Elastomeric Bearings- ii. The length must be 3 character or less for  Report – Bearings are clean and free or not ")]

        public string IS_BEARING_CLEAN { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        //[Required(ErrorMessage = "Page-24: Item 8. BEARINGS- B) Condition of Bearings- (b) Elastomeric Bearings- iii. Please select ")]
        [StringLength(250, ErrorMessage = "Page-24: Item 8. BEARINGS- B) Condition of Bearings- (b) Elastomeric Bearings- iii. The length must be 250 character or less for Report any other defect or observation found ")]

        public string OTHER_DEFECT_IN_ELASTOMERIC { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
         [StringLength(250, ErrorMessage = "Page-24: Item 8. BEARINGS-  B) Condition of Bearings- (c) The length must be 250 character or less for Any type other than ‘a’ and ‘b’ above ")]

        public string OTHER_DEFECT_THAN_BOTH { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-24: Item 8. BEARINGS- I. Please select Whether test results of bearings are available or not: ")]
        [StringLength(3, ErrorMessage = "Page-24: Item 8. BEARINGS- I. The length must be 3 character or less for Whether test results of bearings are available or not: ")]

        public string IS_TEST_RESULT_BEARING_AVL { get; set; }
       


    }
}