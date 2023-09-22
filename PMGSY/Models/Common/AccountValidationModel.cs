using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Models.Common
{
    public class AccountValidationModel
    {
        public AccountValidationModel()
        {
            this.OpeningBalanceFinalized = true;
            this.BankDetailsEntered = true;
            this.ChequeBookDetailsEntered = true;
            this.AuthSign = true;
            this.SrrdaOBEntered = true;
        }
        public bool OpeningBalanceFinalized { get; set; }
        public bool BankDetailsEntered  { get; set; }
        public bool ChequeBookDetailsEntered { get; set; }
        public bool AuthSign { get; set; }
        public bool SrrdaOBEntered { get; set; }
    }
}