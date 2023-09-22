using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Models.Authorization
{
    public class AuthorizationRequestListModel
    {
        public Int64 AuthId { get; set; }
        public String EncAuthId { get; set; }
        public String AuthorizationNumber { get; set; }
        public DateTime AuthorizationDate { get; set; }
        public String ContractorName { get; set; }
        public String RoadName { get; set; }
        public String AgreementCode { get; set; }
        public String Package { get; set; }
        public String BankName { get; set; }
        public String BankAccountNo { get; set; }
        public Nullable<Decimal> SanctionedAmount { get; set; }
        public Nullable<Decimal> ExpenditureAmount { get; set; }
        public Nullable<Decimal> PayableAmount { get; set; }
        public Int32 ContractorId { get; set; }
    }
}