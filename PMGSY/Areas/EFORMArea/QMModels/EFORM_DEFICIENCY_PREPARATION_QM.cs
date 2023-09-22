using PMGSY.Areas.EFORMArea.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.QMModels
{
    public class EFORM_DEFICIENCY_PREPARATION_QM
    {

        public EFORM_DEFICIENCY_PREPARATION_QM(string DeficiasyStatus)
        {
            this.DEFICIENCY_STATUS = DeficiasyStatus;

        }
        [FieldType(PropertyType = PDFFieldType.Skip)]
        public string DEFICIENCY_STATUS { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int DEF_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int EFORM_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int IMS_PR_ROAD_CODE { get; set; }





        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-31: Item 21. General  Observations of QM- A. Please select  ")]
        [RegularExpression(pattern: @"^[A-Za-z]{0,1}$", ErrorMessage = "Page-31: Item 21. General  Observations of QM- Maximum one character is allowed  ")]
        public string IS_NO_DEFICIENCY { get; set; }



        [FieldType(PropertyType = PDFFieldType.CheckBox)]
        [RegularExpression(pattern: @"^[A-Za-z]{0,1}$", ErrorMessage = "Page-31: Item 21. General  Observations of QM- A. II. Maximum one character is allowed  ")]

        [DeficiencyStatusDependable]
        public string IS_BOQ_NOT_CLEAR { get; set; }

        [FieldType(PropertyType = PDFFieldType.CheckBox)]
        [RegularExpression(pattern: @"^[A-Za-z]{0,1}$", ErrorMessage = "Page-31: Item 21. General  Observations of QM- A. II. Maximum one character is allowed  ")]

        [DeficiencyStatusDependable]
        public string IS_INVERT_LEVEL_INCORRECT { get; set; }

        [FieldType(PropertyType = PDFFieldType.CheckBox)]
        [RegularExpression(pattern: @"^[A-Za-z]{0,1}$", ErrorMessage = "Page-31: Item 21. General  Observations of QM- A. II. Maximum one character is allowed  ")]

        [DeficiencyStatusDependable]
        public string IS_CD_STRUCT_INSUFFICE { get; set; }


        [FieldType(PropertyType = PDFFieldType.CheckBox)]
         [RegularExpression(pattern: @"^[A-Za-z]{0,1}$", ErrorMessage = "Page-31: Item 21. General  Observations of QM- A. II. Maximum one character is allowed  ")]
      
        [DeficiencyStatusDependable]
        public string IS_NO_SIDE_DRAIN { get; set; }


        [FieldType(PropertyType = PDFFieldType.CheckBox)]
        [RegularExpression(pattern: @"^[A-Za-z]{0,1}$", ErrorMessage = "Page-31: Item 21. General  Observations of QM- A. II. Maximum one character is allowed  ")]

        [DeficiencyStatusDependable]
        public string IS_DESIGN_NOT_PROVIDED { get; set; }


        [FieldType(PropertyType = PDFFieldType.CheckBox)]
        [RegularExpression(pattern: @"^[A-Za-z]{0,1}$", ErrorMessage = "Page-31: Item 21. General  Observations of QM- A. II. Maximum one character is allowed  ")]

        [DeficiencyStatusDependable]
        public string IS_JUNCTION_DESIGN_INAP { get; set; }


        [FieldType(PropertyType = PDFFieldType.CheckBox)]
        [RegularExpression(pattern: @"^[A-Za-z]{0,1}$", ErrorMessage = "Page-31: Item 21. General  Observations of QM- A. II. Maximum one character is allowed  ")]

        [DeficiencyStatusDependable]
        public string IS_GUARD_NOT_PROVIDED { get; set; }


        [FieldType(PropertyType = PDFFieldType.CheckBox)]
        [RegularExpression(pattern: @"^[A-Za-z]{0,1}$", ErrorMessage = "Page-31: Item 21. General  Observations of QM- A. II. Maximum one character is allowed  ")]

        [DeficiencyStatusDependable]
        public string IS_DEVIATION { get; set; }

        [FieldType(PropertyType = PDFFieldType.CheckBox)]
        [RegularExpression(pattern: @"^[A-Za-z]{0,1}$", ErrorMessage = "Page-31: Item 21. General  Observations of QM- A. II. Maximum one character is allowed  ")]

        [DeficiencyStatusDependable]
        public string IS_EARTHWORK_NOT_BAL { get; set; }


        [FieldType(PropertyType = PDFFieldType.CheckBox)]
        [RegularExpression(pattern: @"^[A-Za-z]{0,1}$", ErrorMessage = "Page-31: Item 21. General  Observations of QM- A. II. Maximum one character is allowed  ")]

        [DeficiencyStatusDependable]
        public string IS_PAVMENT_NOT_ASPER { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [StringLength(250, ErrorMessage = "Page-31: Item 21. General  Observations of QM- The length must be 250 character or less for ")]
        [DeficiencyStatusDependable]
        public string ANY_OTHER_COMMENT { get; set; }





        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int QM_USER_ID { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public string IPADD { get; set; }




    }
}