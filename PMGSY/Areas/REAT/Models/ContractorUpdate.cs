using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace PMGSY.Areas.REAT.Models
{
    public class ContractorUpdate
    {

       // public string IFSCcode { set; get; }
        public string ContractorName { set; get; }
        public string BankAccountNum { set; get; }
        public string ContractorCompanyName { set; get; }
        public string PAN { set; get; }
        public int ContractorID  { set; get; }

        public int DETAIL_ID { get; set; }


        public int StateCode { get; set; }
        public int AgencyCode { get; set; }
        public int AccountID { get; set; }
        public int UserID { get; set; }
        public string IPAddress { get; set; }

        [Required(ErrorMessage = "Contractor Name is required")]
        [StringLength(100, ErrorMessage = "Contractor Name must be less than 100 characters.")]
        [RegularExpression(@"^([a-zA-Z ./]+)$", ErrorMessage = "Contractor Name is not in valid format. Only alphabets, . and / are allowed.")]
        public string reatContractorName { get; set; }

        [Required(ErrorMessage = "IFSC Code is required")]
     //   [StringLength(11, ErrorMessage = "IFSC Code must be 11 characters.")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "IFSC Code must be 11 characters.")]
        [RegularExpression(@"^([a-zA-Z0-9]+)$", ErrorMessage = "IFSC Code is not in valid format.")]
        public string reatIFSC { get; set; }


        [Display(Name = "Bank Name")]
        [Required(ErrorMessage = "Bank Name is required")]
        [RegularExpression(@"^([a-zA-Z ,_()&-;'.]+)$", ErrorMessage = "Bank Name is not in valid format.")]
        [StringLength(100, ErrorMessage = "Bank Name must be less than 100 characters.")]
        public string MAST_BANK_NAME { get; set; }
        public List<SelectListItem> lstBankNames { get; set; }

    }
}