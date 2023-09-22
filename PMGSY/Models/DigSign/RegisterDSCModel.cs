using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.Models.DigSign
{
    public class RegisterDSCModel
    {
        public Int32 NodalOfficerCode { get; set; }
        public string NameAsPerCertificate { get; set; }
        public string Designation { get; set; }
        public string Mobile { get; set; }
        public int IsAlreadyRegistered { get; set; }

        // 0 = Auth Sign details are not prsent  
        // 1 = Auth Sign details are present and registered 
        // 2 - Auth Sign detail are present but not registered

        public Boolean? IsValidXmlDscRegistered { get; set; }
        public Boolean? IsValidXmlDscRegisteredREAT { get; set; }
        public string DscAckStatus { get; set; }
        public Boolean? DscDeleteEnabled { get; set; }
        public Boolean? IsDSCInProgress{ get; set; }
        public Boolean DSCDeregCheck { get; set; }
        
    }

    public class RegistrationData
    {
        public string CertificateBase64 { get; set; }
        public string CertificateChainBase64 { get; set; }
        public string PublicKeyBase64 { get; set; }
        public string MobileNumber { get; set; }
        public string Designation { get; set; }
        public string NameAsPerCeritificate { get; set; }
        public string PkcsStandard { get; set; }
        public string PdfKey { get; set; }
    }


    public class AuthorizedSignatoryCertData
    {
        public Int32 NodalOfficerCode { get; set; }
        public string PdfKey { get; set; }
        public string CertificateBase64 { get; set; }
        public string CertificateChainBase64 { get; set; }

        public string AdminNdName { get; set; }
    }



}