using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;


namespace PMGSY.Models.Bank
{
    public class SRRDAPdfKeyViewModel
    {
        public string EncryptedMailID { get; set; }

        public int MAIL_ID { get; set; }
        public int ADMIN_ND_CODE { get; set; }

        [Display(Name = "Email")]
        //[EmailAddress(ErrorMessage="Invalid Email Address")]
        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Email address is not in valid format.")]
        [Required(ErrorMessage = "Email address is required.")]
        public string EMAIL_CC { get; set; }

        [Display(Name = "PDF Open Key")]
        [Required(ErrorMessage = "PDF Open key is required.")]
        public string PDF_OPEN_KEY { get; set; }

        public System.DateTime GENERATED_DATE { get; set; }

        public String OperationAddEdit { get; set; }

        public virtual ADMIN_DEPARTMENT ADMIN_DEPARTMENT { get; set; }
    }
}