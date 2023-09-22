using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using PMGSY.DAL.AccountMaster;

namespace PMGSY.Models.AccountMaster
{

    public class MasterHeadViewModel
    {

        public String EncryptedHeadID { get; set; }
        
        public short HEAD_ID { get; set; }         
                                            
        [Display(Name="Head Code")]
        //[Range(1,int.MaxValue,ErrorMessage="Please enter Head Code.")]
        [Required(ErrorMessage = "Please enter Head Code.")]
        [RegularExpression(@"^\s*(?=.*[0-9])\d*(?:\.\d{1,2})?\s*$", ErrorMessage = "Invalid Head,Can only contains Numeric values and 2 digits after decimal place")]
        [Range(0.01, 999.99, ErrorMessage = "Invalid Head Code.")]
        public string HEAD_CODE { get; set; }
        public List<SelectListItem> lstParentHead { get; set; }    

        [Display(Name = "Head Code Refferance")]
        public string HEAD_CODE_REF { get; set; }
        
        [Display(Name="Head Name")]
        [Required(ErrorMessage="Please enter Head Name.")]
        [RegularExpression(@"^([a-zA-Z &./,()-]+)$", ErrorMessage = "Only Alphabets, Space and '-','/','.','(',')',',' Allowed")] 
        public string HEAD_NAME { get; set; }

        [Display(Name = "Parent Head")]
        [Required(ErrorMessage = "Please select Parent Head.")]
        [Range(1,Int32.MaxValue,ErrorMessage="Please select Parent Head.")]
        public Nullable<short> PARENT_HEAD_ID { get; set; }

        [Display(Name = "Fund Type")]
        [RegularExpression("[PAM]", ErrorMessage = "Please select Fund Type.")]
        [Required(ErrorMessage = "Please select Fund Type.")]        
        public string FUND_TYPE { get; set; }                
        public List<SelectListItem> lstFundType { get; set; }

        [Display(Name = "Credit / Debit")]
        [Required(ErrorMessage="Please select Credit / Debit.")]
        [RegularExpression("[CD]",ErrorMessage="Please select Credit / Debit.")]
        public string CREDIT_DEBIT { get; set; }

        
        [Display(Name="Level")]
        //[Required(ErrorMessage="Please select Level")]
        [Range(1,3,ErrorMessage="Please select Level")]
        public Nullable<byte> OP_LVL_ID { get; set; }
        public List<SelectListItem> lstOperationalLevel { get; set; }


        [Display(Name = "Is Operational")]
        [Required(ErrorMessage="Please select Is Operational.")]
        //[IsBooleanValidator(ErrorMessage="Please select is ")]
        public bool IS_OPERATIONAL { get; set; }

        [Display(Name = "Head Category")]        
        [Range(1, Int32.MaxValue, ErrorMessage = "Please select Head Category.")]
        public Nullable<int> HEAD_CATEGORY_ID { get; set; }
        public List<SelectListItem> lstHeadCategory { get; set; }

        [RegularExpression("[PS]", ErrorMessage = "Please select Parent / Sub Head.")]
        public string ParentSubHead { get; set; }

        public bool IsParentHead { get; set; }

        public SelectList PopulateFundType {

            get {

                List<SelectListItem> lstFundType = new List<SelectListItem>();
                IAccountMasterDAL objDAL = new AccountMasterDAL();
                lstFundType = objDAL.PopulateFundType();
                return new SelectList(lstFundType, "Value", "Text");            
            }        
        }

        public SelectList PopulateHeadCategory
        {
            get
            {
                List<SelectListItem> lstHeadCategory = new List<SelectListItem>();
                IAccountMasterDAL objDAL = new AccountMasterDAL();
                lstHeadCategory = objDAL.PopulateHeadCategory();
                return new SelectList(lstHeadCategory, "Value", "Text");
            }
        }

        public SelectList PopulateParentHead
        {
            get
            {
                List<SelectListItem> lstParentHead = new List<SelectListItem>();
                IAccountMasterDAL objDAL = new AccountMasterDAL();
                lstParentHead = objDAL.PopulateParentHead(this.FUND_TYPE);
                return new SelectList(lstParentHead, "Value", "Text");
            }
        }

        public SelectList PopulateOperationalLevel
        {
            get
            {
                List<SelectListItem> lstOperationalLevel = new List<SelectListItem>();
                IAccountMasterDAL objDAL = new AccountMasterDAL();
                lstOperationalLevel = objDAL.PopulateOperationalLevel();
                return new SelectList(lstOperationalLevel, "Value", "Text");
            }
        }


        //public virtual ICollection<ACC_ASSET_DETAILS> ACC_ASSET_DETAILS { get; set; }
        //public virtual ICollection<ACC_AUTH_REQUEST_DETAILS> ACC_AUTH_REQUEST_DETAILS { get; set; }
        //public virtual ICollection<ACC_BILL_DETAILS> ACC_BILL_DETAILS { get; set; }
        //public virtual ACC_MASTER_FUND_TYPE ACC_MASTER_FUND_TYPE { get; set; }
        //public virtual ICollection<ACC_MASTER_HEAD> ACC_MASTER_HEAD1 { get; set; }
        //public virtual ACC_MASTER_HEAD ACC_MASTER_HEAD2 { get; set; }
        //public virtual ICollection<ACC_RPT_FA_HEAD_BALANCES> ACC_RPT_FA_HEAD_BALANCES { get; set; }
        //public virtual ICollection<ACC_RPT_REPORT_HEADS> ACC_RPT_REPORT_HEADS { get; set; }
        //public virtual ICollection<ACC_TEO_SCREEN_TXN_VALIDATIONS> ACC_TEO_SCREEN_TXN_VALIDATIONS { get; set; }
        //public virtual ICollection<ACC_TEO_SCREEN_TXN_VALIDATIONS> ACC_TEO_SCREEN_TXN_VALIDATIONS1 { get; set; }
        //public virtual ICollection<ACC_TXN_HEAD_MAPPING> ACC_TXN_HEAD_MAPPING { get; set; }
    }

}