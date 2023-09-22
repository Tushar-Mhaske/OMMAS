using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Models.MIS
{
    public class MISDetailModel
    {
        //Chart 1
        public String[] StateNameArr1 { get; set; }
        public String[] ChequeAmountArr1 { get; set; }
        public String[] TotalPaymentMadeArr1 { get; set; }
        public string StateNamestr1 { get; set; }
        public string ChequeAmountstr1 { get; set; }
        public string TotalPaymentMadestr1 { get; set; }
        public string sumChqAmount1 { get; set; }
        public string sumPaymentMade1 { get; set; }




        //Chart 2
        public String[] FinancialYearArr2 { get; set; }
        public string FinancialYearstr2 { get; set; }
        public String[] ChequeAmountArr2 { get; set; }
        public string ChequeAmountstr2 { get; set; }
        public String[] TotalPaymentMadeArr2 { get; set; }
        public string TotalPaymentMadestr2 { get; set; }
        public string sumChqAmount2 { get; set; }
        public string sumPaymentMade2 { get; set; }


        //Chart 3
        public String[] TotalPaymentMadeArr3 { get; set; }
        public string TotalPaymentMadestr3 { get; set; }
        public String[] MonthArr3 { get; set; }
        public string Monthstr3 { get; set; }
        public string sumPaymentMade3 { get; set; }


        //Chart 4
        public String[] MonthArr4 { get; set; }
        public string Monthstr4 { get; set; }
        public String[] ChequeAmountArr4 { get; set; }
        public string ChequeAmountstr4 { get; set; }
        public string sumChqAmount4 { get; set; }


        //Chart DSC
        public String[] DSCStateNameArr { get; set; }
        public String[] DSCFinalizedArr { get; set; }
        public String[] DSCVerifiedArr { get; set; }
        public string DSCStateNamestr { get; set; }
        public string DSCFinalizedstr { get; set; }
        public string DSCVerifiedstr { get; set; }
        public string sumDSCVerified { get; set; }
        public string sumDSCFinalized { get; set; }




        //Chart Beneficiary
        public String[] BeneficiaryStateNameArr { get; set; }
        public String[] BeneficiaryFinalizedArr { get; set; }
        public String[] BeneficiaryVerifiedArr { get; set; }
        public String[] BeneficiaryTotalArr { get; set; }

        public string BeneficiaryStateNamestr { get; set; }
        public string BeneficiaryFinalizedstr { get; set; }
        public string BeneficiaryVerifiedstr { get; set; }
        public string BeneficiaryTotalstr { get; set; }
    
        public string sumTotalBeneficiary { get; set; }
        public string sumBeneficiaryFinalized { get; set; }
        public string sumBeneficiaryVerified { get; set; }
        
        


        

        

    }
}