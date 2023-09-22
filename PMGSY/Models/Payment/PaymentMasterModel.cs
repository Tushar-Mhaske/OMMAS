using PMGSY.Extensions;
using System;
using PMGSY.Common;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PMGSY.DAL.Receipt;
using PMGSY.BLL.Common;

namespace PMGSY.Models
{

    public class AmountCalculationModel
    {
        public AmountCalculationModel()
        {
            TotalAmtToEnterChqAmount = 0;
            TotalAmtToEnterCachAmount = 0;
            TotalAmtToEnterDedAmount = 0;
            TotalAmtToEnterGrossAmount = 0;

            TotalAmtEnteredChqAmount = 0;
            TotalAmtEnteredCachAmount = 0;
            TotalAmtEnteredDedAmount = 0;
            TotalAmtEnteredGrossAmount = 0;

            DiffChqAmount = 0;
            DiffCachAmount = 0;
            DiffDedAmount = 0;
            DiffGrossAmount = 0;

            VoucherFinalized = "N";

        }

        public Decimal TotalAmtToEnterChqAmount { get; set; }
        public Decimal TotalAmtToEnterCachAmount { get; set; }
        public Decimal TotalAmtToEnterDedAmount { get; set; }
        public Decimal TotalAmtToEnterGrossAmount { get; set; }

        public Decimal TotalAmtEnteredChqAmount { get; set; }
        public Decimal TotalAmtEnteredCachAmount { get; set; }
        public Decimal TotalAmtEnteredDedAmount { get; set; }
        public Decimal TotalAmtEnteredGrossAmount { get; set; }

        public Decimal DiffChqAmount { get; set; }
        public Decimal DiffCachAmount { get; set; }
        public Decimal DiffDedAmount { get; set; }
        public Decimal DiffGrossAmount { get; set; }
        public String VoucherFinalized { get; set; }
        public String CashPayment { get; set; }

        //Added By Abhishek kamble 16-Apr-2014
        public int TransactionId { get; set; }
        public bool IsDetailsEntered { get; set; }

    }

    public class PaymentFilterModel
    {
        public string deductionType { get; set; }
        public Int16 Month { get; set; }
        public Int16 Year { get; set; }
        public String FromDate { get; set; }
        public String ToDate { get; set; }
        public Int16 TransId { get; set; }
        public Int32 page { get; set; }
        public Int32 rows { get; set; }
        public String sidx { get; set; }
        public String sord { get; set; }
        public String FilterMode { get; set; }
        public String FundType { get; set; }
        public Int32 AdminNdCode { get; set; }
        public Int16 LevelId { get; set; }
        public Int64 BillId { get; set; }
        public String Bill_type { get; set; }
        public String TransMode { get; set; }
        public String Deduction_Payment { get; set; }
        public String ChequeEpayNumber { get; set; }
        public String AuthorizationStatus { get; set; }

        //AckUnackFlag Added by Abhishek to identify Ack/Unack Operation
        public String AckUnackFlag { get; set; }

        public String Account_Type { get; set; } //Added by Srishti on 03-03-2023

    }

    public class SignPDFModel
    {
        public String PdfFileName { get; set; }
        public Int64 BillID { get; set; }
        public string PiuName { get; set; }
    }

    public class EpaymentOrderModel
    {
        public bool IsConAgency { get; set; }
        public bool IsAccountInactive { get; set; }

        public String EmailRecepient { get; set; }
        public String EmailCC { get; set; } //new added by Vikram
        public String EmailBCC { get; set; } //new added by Vikram
        public String DPIUName { get; set; }
        public String STATEName { get; set; }
        public String EmailDate { get; set; }
        public String Bankaddress { get; set; }
        public String BankAcNumber { get; set; }
        public String BankIFSCCode { get; set; }//new added by Bhushan on 21-04-2023
        public String EpayNumber { get; set; }
        public String EpayDate { get; set; }
        public String EpayState { get; set; }
        public String EpayDPIU { get; set; }
        public String EpayVNumber { get; set; }
        public String EpayVDate { get; set; }
        public String EpayVPackages { get; set; }
        public String EpayConName { get; set; }
        public String EpayConAcNum { get; set; }
        public String EpayConBankName { get; set; }
        public String EpayConBankIFSCCode { get; set; }
        public String EpayAmount { get; set; }
        public String EpayAmountInWord { get; set; }
        public String BankEmail { get; set; }
        public String EmailSubject { get; set; }
        public String StateCode { get; set; }
        public String DPIUCode { get; set; }
        public String BankPdfPassword { get; set; }
        public String SrrdaPassword { get; set; } //new added by Vikram
        public String ShowPassword { get; set; }

        //Added by Abhishek kamble 12-May-2014
        public String PdfFileName { get; set; }

        //Added by Abhishek kamble 29-May-2014
        public String EpayContLegalHeirName { get; set; }

        //Added by Abhishek kamble 1-Jul-2014 to show/hide package row from dialog box
        //public Int64 EpayMasterTxnID { get; set; }


        //Added by Abhishek kamble 29-Sep-2014 For to identify this is resend Epayment Details
        public String IsNewResend { get; set; }
        public String OrignalEpayDate { get; set; }
        public Int64 BillID { get; set; }

        public string AuthorisedSignName { get; set; }
        public string AuthorisedSignMobile { get; set; }

        public string AgreementNumber { get; set; }
        public long? DetailResendEpayId { get; set; }


    }


    public class EremittnaceOrderModel
    {
        public String EmailRecepient { get; set; }
        public String DPIUName { get; set; }
        public String STATEName { get; set; }
        public String EmailDate { get; set; }
        public String Bankaddress { get; set; }
        public String BankAcNumber { get; set; }

        public String EpayNumber { get; set; }
        public String EpayDate { get; set; }
        public String EpayState { get; set; }
        public String EpayDPIU { get; set; }
        public String EpayVNumber { get; set; }
        public String EpayVDate { get; set; }
        public String EpayVPackages { get; set; }
        public String BankEmail { get; set; }
        public String EmailSubject { get; set; }
        public String StateCode { get; set; }
        public String DPIUCode { get; set; }
        public String BankPdfPassword { get; set; }
        public String ShowPassword { get; set; }

        public String DepartmentName { get; set; }
        public String DepartmentNameFull { get; set; }
        public String DPIUTANNumber { get; set; }
        public String EpayTotalAmountInWord { get; set; }
        public String DepartmentAcNum { get; set; }
        public Decimal TotalAmount { get; set; }
        public String ShowPasswordRow { get; set; }
        public List<EremittnaceContractor> ContractorList { get; set; }

        //Added by Abhishek kamble 12-May-2014
        public String PdfFileName { get; set; }

        //Added by Abhishek kamble 29-Sep-2014 For to identify this is resend Epayment Details
        public String IsNewResend { get; set; }
        public String OrignalEremiDate { get; set; }
        public string AuthorisedSignName { get; set; }
        public string AuthorisedSignMobile { get; set; }
        public String SrrdaPassword { get; set; } //new
        public String EmailCC { get; set; } //new 
        public String EmailBCC { get; set; } //new 
        public Int64 BillID { get; set; }
        public string AgreementNumber { get; set; }
        public long? DetailResendEpayId { get; set; }

    }


    public class EremittnaceContractor
    {

        public String EpayConName { get; set; }
        public String EpayConAcNum { get; set; }
        public String EpayConBankName { get; set; }
        public String EpayConAggreement { get; set; }
        public String EpayAmount { get; set; }
        public String EpayConPanNumber { get; set; }
    }


    public class PaymentMasterModel
    {

        public PaymentMasterModel()
        {
            this.ACC_BILL_DETAILS = new HashSet<ACC_BILL_DETAILS>();
            this.ACC_CANCELLED_CHEQUES = new HashSet<ACC_CANCELLED_CHEQUES>();
            this.ACC_CANCELLED_CHEQUES1 = new HashSet<ACC_CANCELLED_CHEQUES>();
            this.ACC_EPAY_MAIL_MASTER = new HashSet<ACC_EPAY_MAIL_MASTER>();
            this.ACC_TXN_BANK = new HashSet<ACC_TXN_BANK>();
            CommonFunctions objCommon = new CommonFunctions();
            this.CURRENT_DATE = objCommon.GetDateTimeToString(DateTime.Now);
            this.CURRENT_DATE_Hidden = objCommon.GetDateTimeToString(DateTime.Now);
            this.CASH_REQ = false;
            this.CHQ_REQ = false;
            this.DED_REQ = false;
            this.EPAY_REQ = false;
            this.BAR_REQ = false;
            this.REM_REQ = false;
            this.MTXN_REQ = false;
            this.MAST_CON_REQ = false;
            this.MAST_AGREEMENT_REQ = false;
            this.MAST_SUPPLIER_REQ = false;
            this.IS_EREMIT = false;

        }


        #region These are fields only for validation purposes

        public bool CASH_REQ { get; set; }
        public bool CHQ_REQ { get; set; }
        public bool DED_REQ { get; set; }
        public bool EPAY_REQ { get; set; }
        public bool BAR_REQ { get; set; }
        public bool REM_REQ { get; set; }
        public bool MTXN_REQ { get; set; }
        public bool MAST_CON_REQ { get; set; }
        public bool MAST_AGREEMENT_REQ { get; set; }
        public bool MAST_SUPPLIER_REQ { get; set; }

        #endregion

        #region PFMS Validations
        public bool IsAgencyMapped { get; set; }
        public bool IsSRRDABankDetailsFinalized { get; set; }
        public bool IsDSCEnrollmentFinalized { get; set; }
        public bool IsBeneficiaryFinalized { get; set; }
        public bool IsEmailAvailable { get; set; }
        public bool IsPaymentSuccess { get; set; }
        #endregion

        public int conAccountId { get; set; }

        [Display(Name = "Voucher Number")]
        [Required(ErrorMessage = "Voucher Number is Required")]
        [RegularExpression(@"^(?=.*[0-9])[a-zA-Z0-9-/]+$", ErrorMessage = "Only Alphanumeric with atleast one number,'-','/' Allowed")]
        [StringLength(50, ErrorMessage = "Maximum {1} and Minimum {2} character Allowed", MinimumLength = 1)]
        [IsValidVoucherBillNo("BILL_MONTH", "BILL_YEAR", ErrorMessage = "Voucher Number already exists")]
        public string BILL_NO { get; set; }

        public int BILL_ID { get; set; }

        [Range(1, 12, ErrorMessage = "Please Select Month")]    
        public short BILL_MONTH { get; set; }
        public List<SelectListItem> BILL_YEAR_List { get; set; }


        [Range(1, Int16.MaxValue, ErrorMessage = "Please Select Year")]
        public short BILL_YEAR { get; set; }
        public List<SelectListItem> BILL_MONTH_List { get; set; }

        [Display(Name = "Voucher Date")]
        [Required(ErrorMessage = "Voucher Date is Required")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Please Enter valid Voucher date")]
        [DataType(DataType.Date, ErrorMessage = "Invalid Date")]
        [IsDateAfter("CURRENT_DATE", true, ErrorMessage = "Voucher Date must be less than or equal to today's date")]
        [IsValidDate("BILL_MONTH", "BILL_YEAR", "CHQ_EPAY", ErrorMessage = "Voucher Date must be within selected month and year")]
        [IsValidChqBookIssueDate("ChequeBookIssueDate", "CHQ_Book_ID", "CHQ_EPAY", ErrorMessage = "Voucher Date must be greater than or equal to Cheque book issue date")]
        public String BILL_DATE { get; set; }


        public String TextConcat { get; set; }

        //added by Abhishek kamble chq issue date validation 10Mar2015
        public String ChequeBookIssueDate { get; set; }

        [Display(Name = "Transaction Type")]
        [Required(ErrorMessage = "Transaction Type is Required")]
        public String TXN_ID { get; set; }

        public List<SelectListItem> txn_ID1 { get; set; }


        [Display(Name = "Cheque Series")]
        // [Required(ErrorMessage = "Cheque Series is Required")]
        [Range(0, Int32.MaxValue, ErrorMessage = "Invalid cheque book series")]
        public Nullable<int> CHQ_Book_ID { get; set; }
        public List<SelectListItem> CHQ_Book_ID_List { get; set; }

        [Display(Name = "Cheque/Epayment/Advice Number")]
       // [RegularExpression(@"^[0-9]+$", ErrorMessage = "Only Numbers Allowed")]
       // [MaxLength(6, ErrorMessage = "Cheque Numer must be 6 digit long")]
        [RegularExpression(@"^[a-zA-Z0-9-/. ]+$", ErrorMessage = "Only Alphanumeric, Space and '-','/','.' Allowed")]
            public string CHQ_NO { get; set; }
        //public List<SelectListItem> chq_NO1 { get; set; }


        [MaxLength(31, ErrorMessage = "Epayment Number can be atmost 31 digit long")]
        //[MaxLength(30, ErrorMessage = "Epayment Number can be atmost 30 digit long")]
        public string EPAY_NO { get; set; }


        [Display(Name = "Cheque/Epayment/Advice Date")]
        //[RequiredIf("CHQ_REQ",true, ErrorMessage = "Cheque/Epayment Date is Required")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Please Enter valid Cheque/Epayment  date")]
        [IsCheque("CHQ_EPAY", ErrorMessage = "Cheque/Epayment Date is Required")]
        // [IsChequeDateGreater("CHQ_EPAY", ErrorMessage = "Cheque/Epayment Date must be greater than current date")]
        [IsDateAfter("CURRENT_DATE", true, ErrorMessage = "Voucher Date must be less than or equal to today's date")]
        [IsValidDate("BILL_MONTH", "BILL_YEAR", "CHQ_EPAY", ErrorMessage = "Cheque/Epayment Date must be within selected month and year")]
        public String CHQ_DATE { get; set; }


        [RegularExpression(@"^\d{0,11}(\.\d{0,2})?$", ErrorMessage = "Only 11 Digit Whole number and 2 Decimal Allowed")]
        //[RequiredIf("CHQ_REQ", true, ErrorMessage = "Cheque/Epayment Amount is Required")]
        public Nullable<decimal> CHQ_AMOUNT { get; set; }

        [RegularExpression(@"^\d{0,11}(\.\d{0,2})?$", ErrorMessage = "Only 11 Digit Whole number and 2 Decimal Allowed")]
        //[RequiredIf("CASH_REQ", true, ErrorMessage = "Cash Amount is Required")]
        public Nullable<decimal> CASH_AMOUNT { get; set; }

        [Display(Name = "Deduction Amount")]
        // [RequiredIf("DED_REQ", true, ErrorMessage = "Deduction Amount is Required")]
        [RegularExpression(@"^\d{0,11}(\.\d{0,2})?$", ErrorMessage = "Only 11 Digit Whole number and 2 Decimal Allowed")]
        public Nullable<decimal> DEDUCTION_AMOUNT { get; set; }


        public Nullable<decimal> GROSS_AMOUNT { get; set; }

        //[RequiredIf("REM_REQ", true, ErrorMessage = "Chalan Number is Required")]
        [RegularExpression(@"^[a-zA-Z0-9-/. ]+$", ErrorMessage = "Only Alphanumeric, Space and '-','/','.' Allowed")]
        [MaxLength(100, ErrorMessage = "Chalan Number can be atmost 100 characters long")]
        [Display(Name = "Challan Number")]
        [IsChallanNoReq("CHALAN_DATE", "TXN_ID", ErrorMessage = "Chalan No is Required")]
        public string CHALAN_NO { get; set; }


        [Display(Name = "Challan Date")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Please Enter valid Chalan Date")]
        [DataType(DataType.Date, ErrorMessage = "Invalid Chalan Date Date")]
        //[IsDateAfter("CURRENT_DATE", true, ErrorMessage = "Chalan Date must be less than or equal to today's date")]
        [IsValidDate("BILL_MONTH", "BILL_YEAR", "CHQ_EPAY", ErrorMessage = "Chalan Date must be within selected month and year")]
        // //[RequiredIf("REM_REQ", true, ErrorMessage = "Chalan Date is Required")]
        [IsChallanDateReq("CHALAN_NO", "TXN_ID", ErrorMessage = "Chalan Date is Required")]
        public String CHALAN_DATE { get; set; }

        [Display(Name = "Department Name")]
        //[RequiredIf("REM_REQ", true, ErrorMessage = "Department Name is Required")]
        [Range(0, byte.MaxValue, ErrorMessage = "Invalid Department")]
        public Nullable<byte> DEPT_ID { get; set; }
        public List<SelectListItem> dept_ID1 { get; set; }

        [Display(Name = "Company Name")]
        public Nullable<int> CON_ID { get; set; }

        [Display(Name = "Company Name (Contractor)")]
        //[RequiredIf("MAST_CON_REQ", true, ErrorMessage = "Company Name is Required")]
       // [RegularExpression(@"^[0-9]+$", ErrorMessage = "invalid Company (Contractor)")]
        //[Range(0, Int32.MaxValue, ErrorMessage = "Invalid Company (Contractor)")]
        public Nullable<int> MAST_CON_ID_C { get; set; }
        public List<SelectListItem> mast_CON_ID_C1 { get; set; }


        [Display(Name = "Account Details")]
        //[RequiredIf("MAST_CON_REQ", true, ErrorMessage = "Company Name is Required")]
        // [RegularExpression(@"^[0-9]+$", ErrorMessage = "invalid Company (Contractor)")]
        //[Range(0, Int32.MaxValue, ErrorMessage = "Invalid Company (Contractor)")]
        public Nullable<int> CONC_Account_ID { get; set; }
        public List<SelectListItem> CONC_Account_ID1 { get; set; }




        [Display(Name = "Company Name (Supplier)")]
   //     [RegularExpression(@"^[-1-9]+$", ErrorMessage = "invalid Company (Contractor)")]
        [Range(-1, Int32.MaxValue, ErrorMessage = "Invalid Company (Supplier)")]
        public Nullable<int> MAST_CON_ID_S { get; set; }
        public List<SelectListItem> mast_CON_ID_S1 { get; set; }

        public Nullable<int> IMS_PR_ROAD_CODE { get; set; }

        [Display(Name = "Payee Name")]
        [MaxLength(100, ErrorMessage = "Payee Name can be atmost 100 characters long")]
        public string PAYEE_NAME { get; set; }

        [Display(Name = "Payee Name(Remittance)")]
        [RegularExpression(@"[0-9a-zA-z&() ]{1,100}", ErrorMessage = "invalid Payee Name  (Remittance)")]
        [MaxLength(100, ErrorMessage = "Payee Name can be atmost 100 characters long")]
        //[RequiredIf("REM_REQ", true, ErrorMessage = "Payee Name is Required")]
        public string PAYEE_NAME_R { get; set; }

        [MaxLength(100, ErrorMessage = "Payee Name can be atmost 100 characters long")]
        //[RegularExpression(@"[0-9a-zA-z&;() ]{1,100}", ErrorMessage = "invalid Payee Name  (Contractor)")]
        [Display(Name = "Payee Name(Contractor)")]
        public string PAYEE_NAME_C { get; set; }

        [Display(Name = "Payee Name (Supplier) ")]
        [RegularExpression(@"[0-9a-zA-z&() ]{1,100}", ErrorMessage = "invalid Payee Name  (Supplier)")]
        [MaxLength(100, ErrorMessage = "Payee Name can be atmost 100 characters long")]
        //[RequiredIf("MAST_SUPPLIER_REQ", true, ErrorMessage = "Payee Name is Required")]
        public string PAYEE_NAME_S { get; set; }


        [Display(Name = "Mode")]
        [Required(ErrorMessage = "Mode of transaction is Required")]
        [MaxLength(1, ErrorMessage = "invalid Mode of transaction")]
        public string CHQ_EPAY { get; set; }

        public string CURRENT_DATE { get; set; }

        public string CURRENT_DATE_Hidden { get; set; }


        public Nullable<Int16> REMIT_TYPE { get; set; }
        public string BILL_FINALIZED { get; set; }
        public string FUND_TYPE { get; set; }
        public int ND_CODE { get; set; }
        public short LVL_ID { get; set; }

        public bool IS_EREMIT { get; set; }

      

        public virtual ICollection<ACC_BILL_DETAILS> ACC_BILL_DETAILS { get; set; }
        public virtual ACC_CHQ_BOOK_DETAILS ACC_CHQ_BOOK_DETAILS { get; set; }
        public virtual ACC_MASTER_FUND_TYPE ACC_MASTER_FUND_TYPE { get; set; }
        public virtual ACC_MASTER_LEVEL ACC_MASTER_LEVEL { get; set; }
        public virtual ACC_MASTER_REM_TYPE ACC_MASTER_REM_TYPE { get; set; }
        public virtual ACC_MASTER_TXN ACC_MASTER_TXN { get; set; }
        public virtual MASTER_CONTRACTOR MASTER_CONTRACTOR { get; set; }
        public virtual ICollection<ACC_CANCELLED_CHEQUES> ACC_CANCELLED_CHEQUES { get; set; }
        public virtual ICollection<ACC_CANCELLED_CHEQUES> ACC_CANCELLED_CHEQUES1 { get; set; }
        public virtual ACC_CHEQUES_ISSUED ACC_CHEQUES_ISSUED { get; set; }
        public virtual ICollection<ACC_EPAY_MAIL_MASTER> ACC_EPAY_MAIL_MASTER { get; set; }
        public virtual ICollection<ACC_TXN_BANK> ACC_TXN_BANK { get; set; }
        public virtual ICollection<MASTER_MONTH> MASTER_MONTH { get; set; }
        public virtual ICollection<MASTER_YEAR> MASTER_YEAR { get; set; }
    }

    //added by hrishikesh to accept total amount in payment
    public class SecurityDepositAccOpeningBalanceEntryModel
    {


        [Display(Name = "Mode")]
        [Required(ErrorMessage = "Mode of transaction is Required")]
        [MaxLength(1, ErrorMessage = "invalid Mode of transaction")]
        public string CHQ_EPAY { get; set; }

        [Display(Name = "Voucher Number")]
        [Required(ErrorMessage = "Voucher Number is Required")]
        [RegularExpression(@"^(?=.*[0-9])[a-zA-Z0-9-/]+$", ErrorMessage = "Only Alphanumeric with atleast one number,'-','/' Allowed")]
        [StringLength(50, ErrorMessage = "Maximum {1} and Minimum {2} character Allowed", MinimumLength = 1)]
        [IsValidVoucherBillNo("BILL_MONTH", "BILL_YEAR", ErrorMessage = "Voucher Number already exists")]
        public string BILL_NO { get; set; }

        [Display(Name = "Voucher Date")]
        [Required(ErrorMessage = "Voucher Date is Required")]
        /* [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Please Enter valid Voucher date")]
         [DataType(DataType.Date, ErrorMessage = "Invalid Date")]
         [IsDateAfter("CURRENT_DATE", true, ErrorMessage = "Voucher Date must be less than or equal to today's date")]
         [IsValidDate("BILL_MONTH", "BILL_YEAR", "CHQ_EPAY", ErrorMessage = "Voucher Date must be within selected month and year")]
         [IsValidChqBookIssueDate("ChequeBookIssueDate", "CHQ_Book_ID", "CHQ_EPAY", ErrorMessage = "Voucher Date must be greater than or equal to Cheque book issue date")]*/
        public String BILL_DATE { get; set; }

        [Display(Name = "Cheque/Epayment/Advice Number")]
        // [RegularExpression(@"^[0-9]+$", ErrorMessage = "Only Numbers Allowed")]
        // [MaxLength(6, ErrorMessage = "Cheque Numer must be 6 digit long")]
        [RegularExpression(@"^[a-zA-Z0-9-/. ]+$", ErrorMessage = "Only Alphanumeric, Space and '-','/','.' Allowed")]
        public string CHQ_NO { get; set; }


        [MaxLength(31, ErrorMessage = "Epayment Number can be atmost 31 digit long")]
        //[MaxLength(30, ErrorMessage = "Epayment Number can be atmost 30 digit long")]
        public string EPAY_NO { get; set; }

        [Display(Name = "Company Name (Contractor)")]
        //[Required(ErrorMessage = "Company Name (Contractor) Is Required")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Invalid Company (Contractor)")]
        public Nullable<int> MAST_CON_ID_C { get; set; }
        public List<SelectListItem> mast_CON_ID_C1 { get; set; }

        [Display(Name = "Account Details")]
        //[RequiredIf("MAST_CON_REQ", true, ErrorMessage = "Company Name is Required")]
        // [RegularExpression(@"^[0-9]+$", ErrorMessage = "invalid Company (Contractor)")]
        //[Range(0, Int32.MaxValue, ErrorMessage = "Invalid Company (Contractor)")]
        public Nullable<int> CONC_Account_ID { get; set; }

        [Required(ErrorMessage = "Select Contractor Bank Account")]
        public List<SelectListItem> CONC_Account_ID1 { get; set; }


        [Display(Name = "Total Amount")]
        // [RequiredIf("DED_REQ", true, ErrorMessage = "Deduction Amount is Required")]
        [Required(ErrorMessage = "Total Amount Is Required")]
        [RegularExpression(@"^\d{0,11}(\.\d{0,2})?$", ErrorMessage = "Only 11 Digit Whole number and 2 Decimal Allowed")]
        public Nullable<decimal> TOTAL_AMOUNT { get; set; }

        public int BILL_MONTH { get; set; }
        public int BILL_YEAR { get; set; }

    }

    //Added By Abhishek kamble 15-Sep-2014 for Reject Resend Epayment
    public class RejectResendModel
    {

        //Filter Elements

        [Range(1, 12, ErrorMessage = "Please Select Month")]
        public short BILL_MONTH { get; set; }
        public List<SelectListItem> BILL_YEAR_List { get; set; }

        [Range(1, Int16.MaxValue, ErrorMessage = "Please Select Year")]
        public short BILL_YEAR { get; set; }
        public List<SelectListItem> BILL_MONTH_List { get; set; }

        //[Range(1, int.MaxValue, ErrorMessage = "Please Select DPIU")]
        public int DPIU { get; set; }
        public List<SelectListItem> DPIU_LIST { get; set; }

        //Below field SRRDA , LEVEL added on 04-01-2022 
        public int SRRDA { get; set; }
        public List<SelectListItem> SRRDA_LIST { get; set; }

        public String LEVEL { get; set; }

        public String ModeOfTransaction { get; set; }

    }

    public class RejectResendFormModel
    {
        public bool isPaymentRejected { get; set; }

        //Reject Resend Form Elements
        [Display(Name = "Reason for Resend")]
        [Required(ErrorMessage = "Please enter Reason.")]
        [StringLength(250, ErrorMessage = "Reason must be less than 250 characters.")]
        [RegularExpression(@"^([a-zA-Z0-9 ._',\r\n&()-]+)$", ErrorMessage = "Reason is not in valid format.")]
        public String Reason { get; set; }

        public String NonEpayCertificate { get; set; }

        [Display(Name = "Remarks")]
        //[Required(ErrorMessage = "Please enter Remark.")]
        [StringLength(250, ErrorMessage = "Remark must be less than 250 characters.")]
        [RegularExpression(@"^([a-zA-Z0-9 ._',\r\n&()-]+)$", ErrorMessage = "Remark is not in valid format.")]
        public String Remark { get; set; }

        [RegularExpression("[CR]", ErrorMessage = "Please select Resend.")]
        public String CancelResend { get; set; }

        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Date is not in valid format")]
        [Required(ErrorMessage = "Please enter date.")]
        [Display(Name = "Resend Date")]
        public String ResendDate { get; set; }

        public String Encrypted_BIllID_EpayID { get; set; }

        public String currentDate { get; set; }

        //
        public string UploadFileName { get; set; }
        public string EpayPDFFileName { get; set; }
        public int Epay_ID { get; set; }

        public string IsEpayErremi { get; set; }

        public string EncBillID { get; set; }

        //populate headID Using Fund     
        [Display(Name="Head")]
        public short HeadId { get; set; }  
        //public SelectList PopulateHeadId
        //{
        //    get {
        //        List<SelectListItem> lstHeadID = new List<SelectListItem>();
        //        PMGSY.DAL.Payment.IPaymentDAL objDAL = new PMGSY.DAL.Payment.PaymentDAL();
        //        lstHeadID = objDAL.populateFundTypeForCancellation(PMGSYSession.Current.FundType);
        //        return new SelectList(lstHeadID, "Value", "Text");
        //    }            
        //}
        public List<SelectListItem> PopulateHeadId { get; set; }

       public String BillDate { get; set; }
    }
}


public class IsValidVoucherBillNo : ValidationAttribute
{
    private readonly string MonthProperty;
    private readonly string YearProperty;

    public IsValidVoucherBillNo(string monthProperty, string yearProperty)
    {
        this.MonthProperty = monthProperty;
        this.YearProperty = yearProperty;
    }

    public string FormatErrorMessage1(string month, string year)
    {
        return string.Format(month, year);
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        ReceiptDAL objDAL = new ReceiptDAL();
        var monthPropertyInfo = validationContext.ObjectType.GetProperty(this.MonthProperty);
        var yearPropertyInfo = validationContext.ObjectType.GetProperty(this.YearProperty);

        if (monthPropertyInfo == null)
        {
            return new ValidationResult(string.Format("unknown property {0}", this.MonthProperty));
        }
        else if (yearPropertyInfo == null)
        {
            return new ValidationResult(string.Format("unknown property {0}", this.YearProperty));
        }

        if (value == null)
        {
            return ValidationResult.Success;
        }

        var month = monthPropertyInfo.GetValue(validationContext.ObjectInstance, null);
        var year = yearPropertyInfo.GetValue(validationContext.ObjectInstance, null);



        if (objDAL.IsDuplicateBillNo(Convert.ToInt16(month), Convert.ToInt16(year), PMGSYSession.Current.AdminNdCode, PMGSYSession.Current.LevelId, PMGSYSession.Current.FundType, "P", value.ToString()))
        {
            return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
        }
        else
        {
            return ValidationResult.Success;
        }
    }
}

public class IsChallanDateReq : ValidationAttribute
{
    private readonly string ChallanProperty;
    private readonly string TXNIdProperty;

    public IsChallanDateReq(string challanProperty, string txnIdProperty)
    {
        this.ChallanProperty = challanProperty;
        this.TXNIdProperty = txnIdProperty;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        ReceiptDAL objDAL = new ReceiptDAL();
        var challanPropertyInfo = validationContext.ObjectType.GetProperty(this.ChallanProperty);
        var txnPropertyInfo = validationContext.ObjectType.GetProperty(this.TXNIdProperty);

        var chalanNo = challanPropertyInfo.GetValue(validationContext.ObjectInstance, null);
        var txnId = txnPropertyInfo.GetValue(validationContext.ObjectInstance, null);


        //if (chalanNo == null)
        //{
        //    return ValidationResult.Success;
        //}

        //if (value == null)
        //{
        //    if (chalanNo != null)
        //    {
        //        return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
        //    }
        //    else
        //    {
        //        return ValidationResult.Success;
        //    }
        //}
        //return ValidationResult.Success;



        if (txnId.ToString() == "109$Q")
        {
            if (value == null)
            {
                if (chalanNo != null)
                {
                    return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                }
                else
                {
                    return ValidationResult.Success;
                }
            }
        }
        else
        {
            return ValidationResult.Success;
        }
        return ValidationResult.Success;
    }
}


public class IsChallanNoReq : ValidationAttribute
{
    private readonly string ChallanProperty;
    private readonly string TXNIdProperty;

    public IsChallanNoReq(string challanProperty, string txnIdProperty)
    {
        this.ChallanProperty = challanProperty;
        this.TXNIdProperty = txnIdProperty;
    }
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        ReceiptDAL objDAL = new ReceiptDAL();
        var challanPropertyInfo = validationContext.ObjectType.GetProperty(this.ChallanProperty);
        var txnPropertyInfo = validationContext.ObjectType.GetProperty(this.TXNIdProperty);

        var chalanDate = challanPropertyInfo.GetValue(validationContext.ObjectInstance, null);
        var txnId = txnPropertyInfo.GetValue(validationContext.ObjectInstance, null);

        if (txnId.ToString() == "109$Q")
        {
            if (value == null)
            {
                if (chalanDate != null)
                {
                    return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                }
                else
                {
                    return ValidationResult.Success;
                }
            }
        }
        else
        {
            return ValidationResult.Success;
        }

        return ValidationResult.Success;
    }
}



public class IsValidChqBookIssueDate : ValidationAttribute, IClientValidatable
{
    private readonly string ChequeBookIssueDateProperty;
    private readonly string ChequeBookIdProperty;
    private readonly string ChequeProperty;
    private bool IsChqBookIssueDateValid;

    public IsValidChqBookIssueDate(string chequeBookIssueDateProperty, string chequeBookIdProperty, string chqepayProperty)
    {
        this.ChequeBookIssueDateProperty = chequeBookIssueDateProperty;
        this.ChequeBookIdProperty = chequeBookIdProperty;
        this.ChequeProperty = chqepayProperty;
        this.IsChqBookIssueDateValid = true;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        try
        {
            CommonFunctions objCommon = new CommonFunctions();
            var chequeBookIssueDatePropertyInfo = validationContext.ObjectType.GetProperty(this.ChequeBookIssueDateProperty);
            var chequeBookIdPropertyInfo = validationContext.ObjectType.GetProperty(this.ChequeBookIdProperty);
            var chqepayPropertyInfo = validationContext.ObjectType.GetProperty(this.ChequeProperty);

            //if (chequeBookIdPropertyInfo == null)
            //{
            //    //return new ValidationResult(string.Format("unknown property {0}", this.MonthProperty));
            //    return ValidationResult.Success;
            //}            

            var chqBookIssueDate = chequeBookIssueDatePropertyInfo.GetValue(validationContext.ObjectInstance, null);
            var chequeBookId = chequeBookIdPropertyInfo.GetValue(validationContext.ObjectInstance, null);
            var chqepay = chqepayPropertyInfo.GetValue(validationContext.ObjectInstance, null);

            DateTime billDate = objCommon.GetStringToDateTime(value.ToString());

            if (chqepay.ToString().ToLower().Equals("q") && (chequeBookId != null && Convert.ToInt32(chequeBookId) != 0) && ((DateTime.Compare(billDate, objCommon.GetStringToDateTime(chqBookIssueDate.ToString()))) < 0))
            {
                //if (validationContext.DisplayName.ToLower().Equals("cheque date"))
                //{
                //    return new ValidationResult(String.Format("Cheque Date must be within {0} month and {1} year", monthText, year.ToString()));
                //}  

                this.IsChqBookIssueDateValid = false;
                return new ValidationResult(String.Format("Voucher Date must be greater than or equal to Cheque book issued date", billDate, chqBookIssueDate, this.IsChqBookIssueDateValid));
            }
            else
            {
                return ValidationResult.Success;
            }

        }
        catch (Exception)
        {

            return null;
        }
    }

    public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
    {
        var rule = new ModelClientValidationRule
        {
            ErrorMessage = this.ErrorMessageString,
            ValidationType = "isvalidchqissuedate"
        };
        rule.ValidationParameters["cheissuedate"] = this.ChequeBookIssueDateProperty;
        rule.ValidationParameters["chqepay"] = this.ChequeProperty;
        rule.ValidationParameters["ischqissuedatevalid"] = this.IsChqBookIssueDateValid;
        yield return rule;
    }
}