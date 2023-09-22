using PMGSY.Areas.EFORMArea.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.QMModels
{
    public class EFORM_DIFFEENCE_IN_OBSERVATION_QM
    {



        public EFORM_DIFFEENCE_IN_OBSERVATION_QM(string DifferenceinPrevQM)
        {
            this.DifferenceInPrevQM = DifferenceinPrevQM;

        }
        [FieldType(PropertyType = PDFFieldType.Skip)]
        public string DifferenceInPrevQM { get; set; }

        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int DIFFERENCE_ID { get; set; }


        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int EFORM_ID { get; set; }


        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int IMS_PR_ROAD_CODE { get; set; }





        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-33: ITEM 21. General Observations of QM- E. Please select Whether any difference found from previous observations of QMs ")]
        [RegularExpression(pattern: @"^[A-Za-z]{0,1}$", ErrorMessage = "Page-33: ITEM 21. General Observations of QM- E.  Maximum one character is allowed  in Whether any difference found from previous observations of QMs")]
        public string IS_DIFFERENCE_FOUND { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [DeficiencyStatusDependable]
        [Required(ErrorMessage = "Page-33: ITEM 21. General Observations of QM- E.  Please enter comment on difference in observationyes ")]
        [StringLength(250, ErrorMessage = "Page-33: ITEM 21. General Observations of QM- E.  The length must be 250 character or less for comment on difference in observationyes")]
        public string COMMENT_ON_DIFFERENCE_33 { get; set; }


        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [StringLength(250, ErrorMessage = "Page-33: ITEM 21. General Observations of QM- F.  The length must be 250 character or less for Other observations, if any")]
        public string OTHER_OBSERVATIONS_33 { get; set; }






        [FieldType(PropertyType = PDFFieldType.Skip)]
        public int QM_USER_ID { get; set; }


        [FieldType(PropertyType = PDFFieldType.Skip)]
        public string IPADD { get; set; }


    }
}