using PMGSY.Areas.EFORMArea.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.QMModels
{
    public class EFORM_CRASH_BARRIERS_ROAD_SAFETY_QM
    {
        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-26: Item 16. CRASH BARRIERS AND ROAD SAFETY SIGN BOARDS- I. Please select Whether sanctioned DPR has the provision of crash barriers & road safety boards ")]
        [RegularExpression(@"^[A-Za-z]{0,1}$", ErrorMessage = "Page-26: Item 16. CRASH BARRIERS AND ROAD SAFETY SIGN BOARDS- I. Maximum one character is allowed in Whether sanctioned DPR has the provision of crash barriers & road safety boards")]


        public string IS_DPR_PROVISION_26_1 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-26: Item 16. CRASH BARRIERS AND ROAD SAFETY SIGN BOARDS-  II. Please enter Total length of crash barriers as per DPR ")]
        [RegularExpression(@"^(?:\d{0,7}\.\d{1,3})$|^\d{0,7}$", ErrorMessage = "Page-26: Item 16. CRASH BARRIERS AND ROAD SAFETY SIGN BOARDS- II. Maximum seven digits before decimal and maximum three digits after decimal are allowed in Total length of crash barriers as per DPR")]

        public decimal TOT_LENGTH_ASPER_DPR_26 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-26: Item 16. CRASH BARRIERS AND ROAD SAFETY SIGN BOARDS-  III. Please enter Total length of crash barriers erected at site ")]
        [RegularExpression(@"^(?:\d{0,7}\.\d{1,3})$|^\d{0,7}$", ErrorMessage = "Page-26: Item 16. CRASH BARRIERS AND ROAD SAFETY SIGN BOARDS- III. Maximum seven digits before decimal and maximum three digits after decimal are allowed in Total length of crash barriers erected at site")]

        public decimal TOT_LENGTH_ERECTED_26 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-26: Item 16. CRASH BARRIERS AND ROAD SAFETY SIGN BOARDS-  V. Please enter Total number of road safety sign boards in DPR ")]
        [RegularExpression(@"^(?:\d{0,7}\.\d{1,3})$|^\d{0,7}$", ErrorMessage = "Page-26: Item 16. CRASH BARRIERS AND ROAD SAFETY SIGN BOARDS- V. Maximum seven digits before decimal and maximum three digits after decimal are allowed in Total number of road safety sign boards in DPR")]

        public decimal TOT_SIGNBOARDS_ASPER_DPR_26 { get; set; }

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-26: Item 16. CRASH BARRIERS AND ROAD SAFETY SIGN BOARDS-  VI. Please enter Total number of road safety sign boards erected at site ")]
        [RegularExpression(@"^(?:\d{0,7}\.\d{1,3})$|^\d{0,7}$", ErrorMessage = "Page-26: Item 16. CRASH BARRIERS AND ROAD SAFETY SIGN BOARDS- VI. Maximum seven digits before decimal and maximum three digits after decimal are allowed in Total number of road safety sign boards erected at site")]

        public decimal TOT_SIGNBOARDS_ERECTED_26 { get; set; }

        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-26: Item 16. CRASH BARRIERS AND ROAD SAFETY SIGN BOARDS-  Please select Item Grading-16 ")]
        [RegularExpression(@"^[A-Za-z]{0,3}$", ErrorMessage = "Page-26: Item 16. CRASH BARRIERS AND ROAD SAFETY SIGN BOARDS- I. Maximum THREE character is allowed in Item Grading-16")]

        public string ITEM_GRADING_16 { get; set; }



        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-26: Item 16. CRASH BARRIERS AND ROAD SAFETY SIGN BOARDS-  Please enter suggestions for improvement ")]
        [StringLength(maximumLength: 250, ErrorMessage = "Page-26: Item 16. CRASH BARRIERS AND ROAD SAFETY SIGN BOARDS- Only 250 Characters Allowed in suggestions for improvement")]
        public string IMPROVE_SUGGESTIONS_26 { get; set; }
    }
}