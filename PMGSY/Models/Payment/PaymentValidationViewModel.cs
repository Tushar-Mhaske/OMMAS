using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Models.Payment
{
    public class PaymentValidationViewModel
    {
        public bool IsAgencyMapped { get; set; }
        public bool IsSRRDABankDetailsFinalized { get; set; }
        public bool IsDSCEnrollmentFinalized { get; set; }
        public bool IsBeneficiaryFinalized { get; set; }
        public bool IsEmailAvailable { get; set; }

        public bool IsPaymentSuccess { get; set; }
    }
}