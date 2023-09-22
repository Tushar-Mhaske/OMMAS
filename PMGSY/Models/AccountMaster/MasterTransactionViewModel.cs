using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using PMGSY.DAL.AccountMaster;

namespace PMGSY.Models.AccountMaster
{

    public class MasterTransactionViewModel
    {

        public String EncryptedTxnID { get; set; }

        public short TXN_ID { get; set; }

        [Display(Name = "Transaction")]
        [Required(ErrorMessage = "Please select Parent Transaction.")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Please select Parent Transaction.")]        
        public Nullable<short> TXN_PARENT_ID { get; set; }

        [Display(Name = "Cash / Cheque")]
        //[RegularExpression("[CQD]", ErrorMessage = "Please select Cash / Cheque.")]
        [RegularExpression(@"^([A-Z]+)$", ErrorMessage = "Please select Cash / Cheque.")] 
        [Required(ErrorMessage = "Please select Cash / Cheque.")]        
        public string CASH_CHQ { get; set; }
        
        [Display(Name = "Bill Type")]
        [RegularExpression("[ACDIJNOPR]", ErrorMessage = "Please Bill Type.")]
        [Required(ErrorMessage = "Please select Bill Type.")] 
        public string BILL_TYPE { get; set; }
        
        [Display(Name = "Description")]
        [Required(ErrorMessage = "Please enter Description.")]
        [RegularExpression(@"^([a-zA-Z &./,()-]+)$", ErrorMessage = "Only Alphabets, Space and '-','/','.','(',')',',' Allowed")] 
        public string TXN_DESC { get; set; }

        [Display(Name = "Narration")]
        //[Required(ErrorMessage = "Please enter Narration.")]
        [RegularExpression(@"^([a-zA-Z &./,()-]+)$", ErrorMessage = "Only Alphabets, Space and '-','/','.','(',')',',' Allowed")] 
        public string TXN_NARRATION { get; set; }

        [Display(Name = "Fund Type")]
        [RegularExpression("[PAM]", ErrorMessage = "Please select Fund Type.")]
        [Required(ErrorMessage = "Please select Fund Type.")]        
        public string FUND_TYPE { get; set; }

        [Display(Name = "Is Operational")]
        [Required(ErrorMessage = "Please select Is Operational.")]
        public bool IS_OPERATIONAL { get; set; }

        [Display(Name="Transaction Order")]
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Only Digits are allowed.")]        
        public byte TXN_ORDER { get; set; }
                                                     
        [Display(Name = "Level")]
        //[Required(ErrorMessage="Please select Level")]
        [RegularExpression(@"^([0-9]+)$", ErrorMessage = "Please select level.")]                
        public byte OP_LVL_ID { get; set; }
                                                                     
        [Display(Name = "Is Req. After Porting")]
        [Required(ErrorMessage = "Please select Is Req. After Porting.")]
        public bool IS_REQ_AFTER_PORTING { get; set; }

        [RegularExpression("[PS]", ErrorMessage = "Please select Parent / Sub Transaction.")]
        public string ParentSubTransaction { get; set; }

        public bool IsParentTransaction { get; set; }

        public SelectList PopulateParentTxn
        {
            get
            {
                List<SelectListItem> lstParentTxn = new List<SelectListItem>();
                IAccountMasterDAL objDAL = new AccountMasterDAL();
                lstParentTxn = objDAL.PopulateParentTransaction(this.FUND_TYPE);
                return new SelectList(lstParentTxn, "Value", "Text");
            }
        }

        public SelectList PopulateCashCheque
        {
            get
            {
                List<SelectListItem> lstCashCheque = new List<SelectListItem>();
                IAccountMasterDAL objDAL = new AccountMasterDAL();
                lstCashCheque = objDAL.PopulateCashCheque();
                return new SelectList(lstCashCheque, "Value", "Text");
            }
        }

        public SelectList PopulateBillType
        {
            get
            {
                List<SelectListItem> lstBillType = new List<SelectListItem>();
                IAccountMasterDAL objDAL = new AccountMasterDAL();
                lstBillType = objDAL.PopulateBillType();
                return new SelectList(lstBillType, "Value", "Text");
            }
        }      

        public SelectList PopulateFundType
        {
            get
            {
                List<SelectListItem> lstFundType = new List<SelectListItem>();
                IAccountMasterDAL objDAL = new AccountMasterDAL();
                lstFundType = objDAL.PopulateFundType();
                return new SelectList(lstFundType, "Value", "Text");
            }
        }

        public SelectList PopulateLevel
        {
            get
            {
                List<SelectListItem> lstLevel= new List<SelectListItem>();
                IAccountMasterDAL objDAL = new AccountMasterDAL();
                lstLevel = objDAL.PopulateLevel();
                return new SelectList(lstLevel, "Value", "Text");
            }
        }  

    }

}