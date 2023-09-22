using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using PMGSY.DAL.ARRR;

namespace PMGSY.Models.ARRR
{
    public class ChapterItemsViewModel
    {
        public string EncryptedItemCode { get; set; }

        public string User_Action { get; set; }

        public int Parent { get; set; }

        public int ItemCode { get; set; }


        public int HeadCode { get; set; }

        public int hdnMajorItem { get; set; }

        public int MajorSubitemCode { get; set; }
        public int MinorSubitemCode { get; set; }

        [StringLength(8000, ErrorMessage = "Item Description should be maximum 8000 characters.")]
        public string ItemDesc { get; set; }

        public int ItemUnit { get; set; }

        public string hdnItemType { get; set; }

        [RegularExpression(@"^([IMN]+)$", ErrorMessage = "Please select valid Item")]
        public string ItemType { get; set; }

        [Display(Name = "Chapter")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select valid Chapter.")]
        public int Chapter { get; set; }
        public List<SelectListItem> ChapterList { get; set; }

        [Display(Name = "Item")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select valid Item.")]
        public int Item { get; set; }
        public List<SelectListItem> ItemList { get; set; }

        [Display(Name = "Major Item")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select valid Major Item.")]
        public int MajorItem { get; set; }
        public List<SelectListItem> MajorItemList { get; set; }

        [Display(Name = "Minor Item")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select valid Minor Item.")]
        public int MinorItem { get; set; }
        public List<SelectListItem> MinorItemList { get; set; }

        [Display(Name = "Item Name")]
        [StringLength(255, ErrorMessage = "Item Name should be maximum 255 characters.")]
        [Required(ErrorMessage = "Please enter Item Name.")]
        [RegularExpression(@"^([a-zA-Z0-9-._/(), ]+)$", ErrorMessage = "Invalid Item Name.")]
        public string ItemName { get; set; }

        [Display(Name = "Active Flag")]
        [RegularExpression(@"^([YN]+)$", ErrorMessage = "Invalid Active Flag selected")]
        public string ItemActive { get; set; }
        public List<SelectListItem> ItemActiveList { get; set; }

        [RegularExpression(@"^([0-9. ]+)$", ErrorMessage = "Please select valid reference number")]
        public string mordRef { get; set; }

        [Display(Name = "Item User Code")]
        //[RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,4})?\s*$", ErrorMessage = "Invalid Item User Code")]
        // [Range(1, 999999999999.9999, ErrorMessage = "Invalid Item User Code")]
        [Required(ErrorMessage = "Please enter Item User Code.")]
        public string itemUserCode { get; set; }
    }

    public class MachineryMasterViewModel
    {
        [Required(ErrorMessage = "Code is required.")]
        [RegularExpression(@"^([a-zA-Z0-9-., ]+)$", ErrorMessage = "Code is not in valid format.")]
        public string lCode { get; set; }

        public string EncryptedlmmCode { get; set; }

        public string User_Action { get; set; }

        public int lmmCode { get; set; }
        public int lmmType { get; set; }
        public int lmmActyCode { get; set; }

        public string hdnflg { get; set; }

        [Display(Name = "Item Description defined")]
        //[Range(0, int.MaxValue, ErrorMessage = "Please select valid Item.")]
        [RegularExpression(@"^([YN]+)$", ErrorMessage = "Invalid Item Description defined")]
        public string flag { get; set; }
        public List<SelectListItem> flagList { get; set; }

        [Display(Name = "Description")]
        [StringLength(50, ErrorMessage = "Document Name should be maximum 50 characters.")]
        [Required(ErrorMessage = "Description is required.")]
        [RegularExpression(@"^([a-zA-Z0-9-._/(), ]+)$", ErrorMessage = "Invalid Description entered.")]
        public string Description { get; set; }

        [Display(Name = "Activity")]
        [StringLength(50, ErrorMessage = "Activity should be maximum 50 characters.")]
        // [Required(ErrorMessage = "Activity is required.")]
        [RegularExpression(@"^([a-zA-Z0-9-._/(), ]+)$", ErrorMessage = "Invalid Activity entered.")]
        public string Activity { get; set; }

        [Display(Name = "Output Unit")]
        //[StringLength(9, ErrorMessage = "Output Unit be maximum 9 digits.")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select valid Output Unit.")]
        public int OutputUnit { get; set; }
        public List<SelectListItem> OutputUnitList { get; set; }

        [Display(Name = "Output Rate")]
        //[MaxLength(13, ErrorMessage = "Output Rate should be maximum 13 characters.")]
        [Required(ErrorMessage = "Output Rate is required.")]
        //[RegularExpression(@"^([0-9-., ]+)$", ErrorMessage = "Invalid Activity entered.")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select valid Output Rate.")]
        public decimal OutputRate { get; set; }

        [Display(Name = "Usage Unit")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select valid Usage Unit.")]
        public int UsageUnit { get; set; }
        public List<SelectListItem> UsageUnitList { get; set; }

        [Display(Name = "Category")]
        //[StringLength(9, ErrorMessage = "Output Unit be maximum 9 digits.")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select valid Category.")]
        public int Category { get; set; }
        public List<SelectListItem> CategoryList { get; set; }

        [Display(Name = "Activity Details")]
        [StringLength(50, ErrorMessage = "Activity Details should be maximum 50 characters.")]
        //  [Required(ErrorMessage = "Activity Details required.")]
        [RegularExpression(@"^([a-zA-Z0-9-._/(), ]+)$", ErrorMessage = "Invalid Activity Details entered.")]
        public string ActivityDetails { get; set; }
    }

    public class LMMRateViewModel
    {
        public int hdnCategory { get; set; }
        public int hdnItem { get; set; }
        public string EncryptedRateCode { get; set; }

        public string User_Action { get; set; }
        public string status { get; set; }

        public int stateCode { get; set; }

        public int rateCode { get; set; }
        public int rateType { get; set; }

        public string hdnflg { get; set; }

        [Display(Name = "Item Description defined")]
        //[Range(0, int.MaxValue, ErrorMessage = "Please select valid Item.")]
        [RegularExpression(@"^([YN]+)$", ErrorMessage = "Invalid Item Description defined")]
        public string flag { get; set; }
        public List<SelectListItem> flagList { get; set; }

        [Display(Name = "Items")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select valid Item.")]
        public int Item { get; set; }
        public List<SelectListItem> ItemList { get; set; }

        [Display(Name = "Rate")]
        [Required(ErrorMessage = "Rate is required.")]
        //[Range(0, int.MaxValue, ErrorMessage = "Please select valid Rate.")]
        [Range(1, 999999999999.9999, ErrorMessage = "Invalid Rate, Can only contains Numeric values and 4 digit after decimal place")]
        public decimal Rate { get; set; }

        /*[Display(Name = "Date")]
        [Required(ErrorMessage = "Date is required.")]
        //[Range(0, int.MaxValue, ErrorMessage = "Please select valid Date.")]
        public string Date { get; set; }

        [Display(Name = "Date")]
        //[Required(ErrorMessage = "Till Date is required.")]
        //[Range(0, int.MaxValue, ErrorMessage = "Please select valid Date.")]
        public string TillDate { get; set; }*/

        [Display(Name = "Year")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select valid Year.")]
        public int Year { get; set; }
        public List<SelectListItem> yearList { get; set; }

        [Display(Name = "Category")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select valid Category.")]
        public int Category { get; set; }
        public List<SelectListItem> CategoryList { get; set; }
    }

    public class ScheduleRatesViewModel
    {
        public int hdlmmType { get; set; }
        public int hdnItemCodeMaterial { get; set; }
        public int hdnItemCodeMachinery { get; set; }
        public int hdnItemTypeCode { get; set; }

        public int ItemTypeCode { get; set; }

        public string chapterName { get; set; }
        public string itemName { get; set; }

        public int recordCount { get; set; }

        public string hdnlmmType { get; set; }

        public string EncryptedCode { get; set; }

        public string User_Action { get; set; }
        public string status { get; set; }

        public int stateCode { get; set; }
        public int arrrCode { get; set; }
        public int ItemCode { get; set; }

        public int rateCode { get; set; }

        //[Range(1, int.MaxValue, ErrorMessage = "Please select valid Quantity.")]
        //public int quantity { get; set; }
        [Range(0.001, 999999999999.9999, ErrorMessage = "Invalid Quantity, Can only contains Numeric values and 2 digit after decimal place")]
        public decimal quantity { get; set; }

        public int scheduleCode { get; set; }

        [RegularExpression(@"^([YN]+)$", ErrorMessage = "Please select valid Approval")]
        public string Approved { get; set; }

        [Display(Name = "Chapter")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select valid Chapter.")]
        public int Chapter { get; set; }
        public List<SelectListItem> ChapterList { get; set; }

        [RegularExpression(@"^([IMN]+)$", ErrorMessage = "Please select valid Item")]
        public string ItemType { get; set; }

        //[RegularExpression(@"^([123]+)$", ErrorMessage = "Please select valid LMM Type")]
        //public string lmmType { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Please select valid LMM Type.")]
        public int lmmType { get; set; }
        public List<SelectListItem> lmmTypeList { get; set; }

        [Display(Name = "Is Finalize")]
        [RegularExpression(@"^([YN]+)$", ErrorMessage = "Invalid finalize value")]
        public string finalize { get; set; }
        public List<SelectListItem> finalizeList { get; set; }

        [Display(Name = "Items")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select valid Item.")]
        public int Item { get; set; }
        public List<SelectListItem> ItemList { get; set; }

        [Display(Name = "Major Item")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select valid Major Item.")]
        public int MajorItem { get; set; }
        public List<SelectListItem> MajorItemList { get; set; }

        [Display(Name = "Minor Item")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select valid Minor Item.")]
        public int MinorItem { get; set; }
        public List<SelectListItem> MinorItemList { get; set; }

        [Display(Name = "Category")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select valid Category.")]
        public int Category { get; set; }
        public List<SelectListItem> CategoryList { get; set; }
    }

    public class TaxScheduleViewModel
    {
        public int hdnCategory { get; set; }


        public string EncryptedTaxCode { get; set; }

        public string User_Action { get; set; }
        public string status { get; set; }

        public int taxCode { get; set; }

        [Display(Name = "Tax Type")]
        //[Range(0, int.MaxValue, ErrorMessage = "Please select valid Item.")]
        [RegularExpression(@"^([PL]+)$", ErrorMessage = "Invalid Tax Type")]
        public string taxType { get; set; }
        public List<SelectListItem> taxTypeList { get; set; }

        [Display(Name = "Rate")]
        [Required(ErrorMessage = "Rate is required.")]
        //[Range(0, int.MaxValue, ErrorMessage = "Please select valid Rate.")]
        [Range(0, 100.00, ErrorMessage = "Invalid Rate Percentage, Can only contains Numeric values and cannot exceed 100 %")]
        public decimal RatePer { get; set; }

        public decimal Rate { get; set; }

        [Display(Name = "Rate")]
        [Required(ErrorMessage = "Rate is required.")]
        //[Range(0, int.MaxValue, ErrorMessage = "Please select valid Rate.")]
        [Range(0, 999999999999.9999, ErrorMessage = "Invalid Rate, Can only contains Numeric values and 2 digit after decimal place")]
        public decimal Ratelmsm { get; set; }

        [Display(Name = "Tax")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select valid Tax.")]
        public int tax { get; set; }
        public List<SelectListItem> taxList { get; set; }

        public int itemCode { get; set; }
        public int hdnitemCode { get; set; }
    }

    #region Rohit Code


    public class Chapter
    {
        [Display(Name = "Chapter Description")]
        [Required(ErrorMessage = "Please enter the Chapter Description.")]
        [RegularExpression(@"^([a-zA-Z0-9-.,_/() ]+)$", ErrorMessage = "Invalid Chapter Description.")]
        public string ChapterName { get; set; }

        public int ChapterCode { get; set; }

        public string EncryptedChapterCode { get; set; }

        public string Operation { get; set; }

    }

    public class Labour
    {
        [Display(Name = "Labour Description")]
        [Required(ErrorMessage = "Please enter the Labour Description.")]
        [RegularExpression(@"^([a-zA-Z0-9-._/() ]+)$", ErrorMessage = "Invalid Labour Description.")]
        public string MaterialName { get; set; }


        [Display(Name = "Code")]
        [Required(ErrorMessage = "Please enter the Code.")]
        [RegularExpression(@"^([a-zA-Z0-9-._/() ]+)$", ErrorMessage = "Invalid Code.")]
        [StringLength(5, ErrorMessage = "Code should not be more than 5 characters.")]
        public string LMM_CODE_NAME { get; set; }

        public int MaterialCode { get; set; }
        public string EncryptedMaterialCode { get; set; }


        public string Operation { get; set; }


        [Display(Name = "Unit")]
        [Required(ErrorMessage = "Please Select the Unit.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please Select the Unit.")]
        public int UnitCode { get; set; }

        public string UnitName { get; set; }

        public virtual MASTER_UNITS MASTER_UNITS { get; set; }

        public SelectList Units
        {
            get
            {
                List<MASTER_UNITS> UnitsList = new List<MASTER_UNITS>();

                ARRRDAL objMaster = new ARRRDAL();

                UnitsList = objMaster.GetAllUnitsLabour();

                UnitsList.Insert(0, new PMGSY.Models.MASTER_UNITS()
                {
                    MAST_UNIT_CODE = 0,
                    MAST_UNIT_NAME = "--Select--"

                });

                return new SelectList(UnitsList, "MAST_UNIT_CODE", "MAST_UNIT_NAME", this.UnitCode);
            }

        }

        //Category Dropdown
        [Display(Name = "Category ")]
        [Required(ErrorMessage = "Please Select the Category.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please Select the Category.")]
        public int TypeCodes { get; set; }


        public string TypeNames { get; set; }
        public virtual MASTER_ARRR_LMM_CATEGORY MASTER_ARRR_LMM_CATEGORY { get; set; }

        public SelectList Typess
        {
            get
            {
                List<MASTER_ARRR_LMM_CATEGORY> CatsList = new List<MASTER_ARRR_LMM_CATEGORY>();

                ARRRDAL objMaster = new ARRRDAL();

                CatsList = objMaster.GetAllCategoryTypesLabour();

                CatsList.Insert(0, new PMGSY.Models.MASTER_ARRR_LMM_CATEGORY()
                {
                    MAST_LMM_CAT_CODE = 0,
                    MAST_LMM_CATEGORY = "--Select--"

                });

                return new SelectList(CatsList, "MAST_LMM_CAT_CODE", "MAST_LMM_CATEGORY", this.TypeCodes);
            }
        }
    }

    public class Material
    {
        //Material Name
        [Display(Name = "Material Description")]
        [Required(ErrorMessage = "Please enter the Material Description.")]
        [RegularExpression(@"^([a-zA-Z0-9-._/() ]+)$", ErrorMessage = "Invalid Material Description.")]
        public string MaterialName { get; set; }

        //Material Code (Primary Key)
        public int MaterialCode { get; set; }
        public string EncryptedMaterialCode { get; set; }

        // A variable to distinguish UI (Save-Reset , Update-Cancel)
        public string Operation { get; set; }


        [Display(Name = "Code")]
        [Required(ErrorMessage = "Please enter the Code.")]
        [RegularExpression(@"^([a-zA-Z0-9-._/() ]+)$", ErrorMessage = "Invalid Code.")]
        [StringLength(5, ErrorMessage = "Code should not be more than 5 characters.")]
        public string LMM_CODE_NAME { get; set; }

        // Unit Dropdown Population 
        [Display(Name = "Unit")]
        [Required(ErrorMessage = "Please Select the Unit.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please Select the Unit.")]
        public int UnitCode { get; set; }

        public string UnitName { get; set; }

        public virtual MASTER_UNITS MASTER_UNITS { get; set; }

        public SelectList Units
        {
            get
            {
                List<MASTER_UNITS> UnitsList = new List<MASTER_UNITS>();

                ARRRDAL objMaster = new ARRRDAL();

                UnitsList = objMaster.GetAllUnits();

                UnitsList.Insert(0, new PMGSY.Models.MASTER_UNITS()
                {
                    MAST_UNIT_CODE = 0,
                    MAST_UNIT_NAME = "--Select--"

                });

                return new SelectList(UnitsList, "MAST_UNIT_CODE", "MAST_UNIT_NAME", this.UnitCode);
            }

        }


        // Category Population

        [Display(Name = "Category ")]
        [Required(ErrorMessage = "Please Select the Category.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please Select the Category.")]
        public int TypeCodes1 { get; set; }


        public string TypeNames1 { get; set; }

        public virtual MASTER_ARRR_LMM_CATEGORY MASTER_ARRR_LMM_CATEGORY { get; set; }

        public SelectList Typess1
        {
            get
            {
                List<MASTER_ARRR_LMM_CATEGORY> CatsList = new List<MASTER_ARRR_LMM_CATEGORY>();

                ARRRDAL objMaster = new ARRRDAL();

                CatsList = objMaster.GetAllCategoryTypesMaterial();

                CatsList.Insert(0, new PMGSY.Models.MASTER_ARRR_LMM_CATEGORY()
                {
                    MAST_LMM_CAT_CODE = 0,
                    MAST_LMM_CATEGORY = "--Select--"

                });

                return new SelectList(CatsList, "MAST_LMM_CAT_CODE", "MAST_LMM_CATEGORY", this.TypeCodes1);
            }
        }
        //Category Population

    }

    public class CategoryViewModel
    {
        [Display(Name = "Category Description")]
        [Required(ErrorMessage = "Please enter the Category Description.")]
        [RegularExpression(@"^([a-zA-Z0-9-._/() ]+)$", ErrorMessage = "Category Description is invalid.")]
        [StringLength(50, ErrorMessage = "Category Description should not be more than 50 characters.")]
        public string CategoryName { get; set; }

        [UIHint("hidden")]
        public int CategoryCode { get; set; }
        public string EncryptedCategoryCode { get; set; }

        // A variable to distinguish UI (Save-Reset , Update-Cancel)
        public string Operation { get; set; }


        [Display(Name = "Type")]
        [Required(ErrorMessage = "Please Select the Type.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please Select the Type.")]
        public int TypeCode { get; set; }
        public string TypeName { get; set; }



        public SelectList Types
        {
            get
            {
                List<SelectListItem> classTypeList = new List<SelectListItem>();
                ARRRDAL objMaster = new ARRRDAL();
                classTypeList = objMaster.GetTypesCode();

                return new SelectList(classTypeList, "Value", "Text");
            }

        }


    }

    public class TaxViewModel
    {
        [Display(Name = "Other Charges Description")]
        [Required(ErrorMessage = "Please enter Other Charges Description.")]
        [RegularExpression(@"^([a-zA-Z0-9-._/() ]+)$", ErrorMessage = "Invalid Other Charges Description.")]
        public string TaxName { get; set; }

        public int TaxCode { get; set; }

        public string EncryptedTaxCode { get; set; }

        public string TaxIsActiveFlag { get; set; }

        public string Operation { get; set; }

    }


    #endregion

    public class LabourDetails
    {
        public string Category { get; set; }
        public string Labour { get; set; }
        [Range(1, 999999999999.99, ErrorMessage = "Invalid Rate, Can only contains Numeric values and 2 digit after decimal place")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,2})?\s*$", ErrorMessage = "Rate ,Can only contains Numeric values and 2 digits after decimal place")]
        public decimal Rate { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Please select valid Year.")]
        public int Year { get; set; }
        public string File { get; set; }
        public virtual LMMRateViewModel Form { get; set; }
        public string Mast_Lmm_Code { get; set; }
    }

    public class AnalysisOfRates
    {

        [Display(Name = "State")]
        [Required(ErrorMessage = "Please select State. ")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select  State.")]
        public int StateCode { get; set; }
        public List<SelectListItem> StateList { get; set; }

        [Display(Name = "Chapter")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select valid Chapter.")]
        public int Chapter { get; set; }
        public List<SelectListItem> ChapterList { get; set; }

        [Display(Name = "Year")]
        [Range(2000, 2090, ErrorMessage = "Please select Year.")]
        [Required(ErrorMessage = "Please select Year.")]
        [RegularExpression("^([0-9]+)$", ErrorMessage = "Year must be valid number.")]
        public int PhaseYear { get; set; }
        public List<SelectListItem> PhaseYearList { get; set; }
    }
}