using PMGSY.Areas.EFORMArea.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static PMGSY.Areas.EFORMArea.Common.CommonEnum;

namespace PMGSY.Areas.EFORMArea.QMBridgeModel
{
    public class EFORM_BRIDGE_QM_GENERAL_DETAILS
    {
        

        [FieldType(PropertyType = PDFFieldType.TextBox)]
        [Required(ErrorMessage = "Page-9: Item 1. GENERAL:- I. please enter Date of inspection")]
        [RegularExpression(pattern: @"^([0]?[0-9]|[12][0-9]|[3][01])/([0]?[1-9]|[1][0-2])/([0-9]{4})$", ErrorMessage = "Page-9: Item 1. GENERAL:- I. Please Enter Valid date{in dd/mm/yyyy format} in Date of inspection")]
        public string INSPECTION_DATE { get; set; }

        [FieldType(PropertyType = PDFFieldType.CheckBox)]
       
        [RegularExpression(pattern: @"^[Y,N]{1}$", ErrorMessage = "Page-9: Item 1. GENERAL:- VII. Present status of work is not in valid format for Layout")]
        public string IS_WORK_STAT_LAYOUT { get; set; }

        [FieldType(PropertyType = PDFFieldType.CheckBox)]
        [RegularExpression(pattern: @"^[Y,N]{1}$", ErrorMessage = "Page-9: Item 1. GENERAL:- VII. Present status of work is not in valid format for Foundation ")]
        public string IS_WORK_STAT_FOUNDATION { get; set; }

        [FieldType(PropertyType = PDFFieldType.CheckBox)]
        [RegularExpression(pattern: @"^[Y,N]{1}$", ErrorMessage = "Page-9: Item 1. GENERAL:- VII. Present status of work is not in valid format for Substructure")]
        public string IS_WORK_STAT_SUBSTRUCTURE { get; set; }

        [FieldType(PropertyType = PDFFieldType.CheckBox)]
        [RegularExpression(pattern: @"^[Y,N]{1}$", ErrorMessage = "Page-9: Item 1. GENERAL:- VII. Present status of work is not in valid format for Superstructure")]
        public string IS_WORK_STAT_SUPERSTRUCTURE { get; set; }

        [FieldType(PropertyType = PDFFieldType.CheckBox)]
        [RegularExpression(pattern: @"^[Y,N]{1}$", ErrorMessage = "Page-9: Item 1. GENERAL:- VII. Present status of work is not in valid format for Protection work")]
        public string IS_WORK_STAT_PROT_WORK { get; set; }

        [FieldType(PropertyType = PDFFieldType.CheckBox)]
        [RegularExpression(pattern: @"^[Y,N]{1}$", ErrorMessage = "Page-9: Item 1. GENERAL:- VII. Present status of work is not in valid format for Approaches")]
        public string IS_WORK_STAT_APPROACH { get; set; }

        [FieldType(PropertyType = PDFFieldType.CheckBox)]
        [RegularExpression(pattern: @"^[Y,N]{1}$", ErrorMessage = "Page-9: Item 1. GENERAL:- VII. Present status of work is not in valid format for Finishing stage")]
        public string IS_WORK_STAT_FINISHING_STAGE { get; set; }
        
        [FieldType(PropertyType = PDFFieldType.RadioButton)]
        [Required(ErrorMessage = "Page-9: Item 1. GENERAL:- VIII. Photographic or video-graphic records (optional) of all hidden items like reinforcements of foundations/ substructures/ super structures etc. are kept by PIU, till Defect liability period is over for bridge for any future reference or not")]
        [RegularExpression(pattern: @"^[A-Za-z]{0,3}$", ErrorMessage = "Page-9: Item 1. GENERAL:- VIII. Maximum three characters is allowed in Photographic or video-graphic records (optional) of all hidden items like reinforcements of foundations/ substructures/ super structures etc. are kept by PIU, till Defect liability period is over for bridge for any future reference or not.")] 
        public string IS_VIDEO_RECORDS { get; set; }
 

    }
}