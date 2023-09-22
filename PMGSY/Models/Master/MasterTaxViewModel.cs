using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PMGSY.Models.Master
{
    public class MasterTaxViewModel
    {
        public string EncryptedTaxCode { get; set; }

        public string Operation { get; set; }

        public int Tax_Id { get; set; }
        
        [Required(ErrorMessage="TDS is required.")]
        [Display(Name="TDS")]
        [Range(0.00,99999.99,ErrorMessage="Invalid value.Only digits with two decimals are allowed.")]
        public decimal TDS { get; set; }

        [Required(ErrorMessage="Service Charge is required.")]
        [Display(Name="Service Charge")]
        [Range(0.00, 99999.99, ErrorMessage = "Invalid value.Only digits with two decimals are allowed.")]
        public decimal Service_Charge { get; set; }

        [Display(Name = "Service Tax")]
        [RegularExpression("([a-zA-Z0-9.&,-;() ]{1,255})", ErrorMessage = "Service Tax  No. is not in valid format.")]
        [Range(0.00, 99999.99, ErrorMessage = "Invalid value.Only digits with two decimals are allowed.")]
        public decimal Service_TAX { get; set; }


        [Required(ErrorMessage="Effective Date is required.")]
        [Display(Name="Effective Date")]
        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Effective Date must be in dd/mm/yyyy format.")]
        public string Effective_Date { get; set; }
    }
}