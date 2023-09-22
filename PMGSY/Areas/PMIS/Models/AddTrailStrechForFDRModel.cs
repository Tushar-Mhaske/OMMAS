using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Areas.PMIS.Models
{
    public class AddTrailStrechForFDRModel
    {

        //Newly Created By Hrishikesh To Add "Trail Strech For FDR" --05-06-2023
        public int IMS_PR_ROAD_CODE { get; set; }

        [Required(ErrorMessage = "Please select Additive used")]
        //public int ADDITIVE_ID { get; set; }
        public string ADDITIVE_ID { get; set; }
        public List<SelectListItem> ADDITIVE_ID_LIST { get; set; }
        //public int JMF_ID { get; set; }
        [Range(1, 100, ErrorMessage = "Please select JMF prepared.")]
        [Required(ErrorMessage = "Please select JMF prepared")]
        public string JMF_ID { get; set; }
        public List<SelectListItem> JMF_ID_LIST { get; set; }

        [Required(ErrorMessage = "Please Enter Cement Content")]
        [Range(0, 100, ErrorMessage = "The Percentage value must be between 0 and 100")]
        [RegularExpression(@"^100(\.0{1,2})?$|^\d{1,2}(\.\d{1,2})?$", ErrorMessage = "Value Should not be more that 100 and upto 2 decimals")]
        public Nullable<decimal> PERC_CEMENT_CONT { get; set; }

        //[Required(ErrorMessage = "Please Select Additive Content Unit Type")]
        public string IS_ADDITIVE_CONT_PERC_ML { get; set; }


        //[Required(ErrorMessage = "Please Enter Additive Content")]
        [Range(0, 100, ErrorMessage = "The Percentage value must be between 0 and 100")]
        [RegularExpression(@"^100(\.0{1,2})?$|^\d{1,2}(\.\d{1,2})?$", ErrorMessage = "Value Should not be more that 100 and upto 2 decimals")]
        public string PERC_ADDITIVE_CONT { get; set; }

        public string ADDITIVE_CONT_ML_CUM { get; set; }

        public string ADDITIVE_CONT_UNIT { get; set; }

        public string IS_AVG_UCS_7D_28D { get; set; }

        public string AVG_UCS_7D { get; set; }

        public string AVG_UCS_28D { get; set; }

        public string IS_UCS_TEST_CUBE_CYLINDER { get; set; }

        public string UCS_TEST_CUBE { get; set; }

        public string UCS_TEST_CYLINDER { get; set; }

        [Required(ErrorMessage = "Please Enter Maximum Dry Density (MDD) of Mix.")]
        public string MDD_MIX { get; set; }

        [Required(ErrorMessage = "Please Enter Optimum Moisture Content (OMC) of Mix.")]
        public string OMC_MIX { get; set; }

        public string TEST_RESULT_FILE_NAME { get; set; }
        public string TEST_RESULT_FILE_PATH { get; set; }

        public string IS_TS_UCS_STRENGTH_CUBE_CYLINDER { get; set; }

        public string UCS_STRENGTH_CUBE { get; set; }

        public string UCS_STRENGTH_CYLINDER { get; set; }

        public string IS_TS_UCS_7D_28D { get; set; }

        public string TS_UCS_7D { get; set; }

        public string TS_UCS_28D { get; set; }

        public string TS_CORE_TRIAL_STRETCH_FILE_NAME { get; set; }

        public string TS_CORE_TRIAL_STRETCH_FILE_PATH { get; set; }

        [Required(ErrorMessage = "Please Enter Residual Strength after 12 cycle Wetting drying(MPa).")]
        public string TS_RESD_STRENGTH_WETT { get; set; }

        public string TS_RESD_STRENGTH_WETT_FILE_NAME { get; set; }

        public string TS_RESD_STRENGTH_WETT_FILE_PATH { get; set; }


        [Required(ErrorMessage = "Please Enter Cement Type")]
        [StringLength(250, ErrorMessage = "The Cement Type field must be between 0 and 250 characters.")]
        public string CEMENT_TYPE { get; set; }

        [Required(ErrorMessage = "Please Enter Cement Grade")]
        [StringLength(250, ErrorMessage = "The Cement Grade field must be between 0 and 250 characters.")]
        public string CEMENT_GRADE { get; set; }

        [Required(ErrorMessage = "Please Enter Average Thickness of Existing Crust(Excluding subgrade)")]
        [Range(0, 300, ErrorMessage = "Average thickness of existing crust should be 0 to 300")]
        //[RegularExpression(@"^(?:300(?:\.0{1,2})?|(?:[0-2]?[0-9]{0,2}(?:\.[0-9]{1,2})?))$", ErrorMessage = "Average thickness of existing crust should be 0 to 300")]
        [RegularExpression(@"^(?:300(?:\.0{1,3})?|(?:[0-2]?[0-9]{0,2}(?:\.[0-9]{1,3})?))$", ErrorMessage = "Average thickness of existing crust should be 0 to 300")]
        //[RegularExpression(@"^300(\.0{1,2})?$|^\d{1,2}(\.\d{1,2})?$", ErrorMessage = "Average thickness of existing crust should be 0 to 300")]
        public string AVG_THICK_CRUST { get; set; }

        [Required(ErrorMessage = "Please Enter Plasticity Index Reclaimed Soil")]
        [Range(0, 100, ErrorMessage = "The Percentage value must be between 0 and 100")]
        [RegularExpression(@"^100(\.0{1,2})?$|^\d{1,2}(\.\d{1,2})?$", ErrorMessage = "Value Should not be more that 100 and upto 2 decimals")]
        public string PERC_PLASTICITY_RECLAM_SOIL { get; set; }

        [Required(ErrorMessage = "Please Enter Plasticity Index of Subgrade Soil")]
        [Range(0, 100, ErrorMessage = "The Percentage value must be between 0 and 100")]
        [RegularExpression(@"^100(\.0{1,2})?$|^\d{1,2}(\.\d{1,2})?$", ErrorMessage = "Value Should not be more that 100 and upto 2 decimals")]
        public string PERC_INDEX_SUBGRADE_SOIL { get; set; }

        public string IS_GRAD_MATERIAL_SPEC_LIMIT { get; set; }
        public string CRACK_RELIEF_LAYER { get; set; }

        public string OTHER_CRACK_LAYER { get; set; }

        [Required(ErrorMessage = "Please Enter Length of Trial strech in km")]
        public string STRETCH_LENGTH { get; set; }

        [Required(ErrorMessage = "Please Select Date of Construction of trial strech")]
        public string STRETCH_CONSTR_DATE { get; set; }

        public string UCS_TEST_CONDUCTED { get; set; }
        public string UCS_7DAY_VALUE { get; set; }
        public string UCS_28DAY_VALUE { get; set; }


        public string JMF_FILE_NAME { get; set; }
        public string JMF_FILE_PATH { get; set; }
        /* [Display(Name = "Upload Score File: ")]
         [Required(ErrorMessage = "File is Required")]
         [DataType(DataType.Upload)]*/
        public HttpPostedFileBase UPLOADED_FILE1 { get; set; }
        public HttpPostedFileBase UPLOADED_FILE2 { get; set; }
        public HttpPostedFileBase UPLOADED_FILE3 { get; set; }

        //
        public string IS_FDR_FILLED { get; set; }
        public string IS_Finalized { get; set; }
        public string SELECTED_ADDITIVES_DB { get; set; }

    }
}