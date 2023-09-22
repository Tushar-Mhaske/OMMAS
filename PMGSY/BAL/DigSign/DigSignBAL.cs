using PMGSY.DAL.DigSign;
using PMGSY.DigSignDocs;
using PMGSY.Models.DigSign;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;

namespace PMGSY.BAL.DigSign
{
    public class DigSignBAL
    {
        public RegisterDSCModel GetDetailsToRegisterDSC()
        {
            DigSignDAL objDAL = new DigSignDAL();
            return objDAL.GetDetailsToRegisterDSC();
        }

        public RegisterDSCModel GetDetailsToRegisterDSCREAT()
        {
            DigSignDAL objDAL = new DigSignDAL();
            return objDAL.GetDetailsToRegisterDSCREAT();
        }
        public bool IsAlreadyRegistered(Int32 nodalOfficerCode)
        {
            DigSignDAL objDAL = new DigSignDAL();
            return objDAL.IsAlreadyRegistered(nodalOfficerCode);
        }

        public string SaveDSCRegistrationDetails(RegistrationData model)
        {
            DigSignDAL objDAL = new DigSignDAL();
            return objDAL.SaveDSCRegistrationDetails(model);
        }


        /// <summary>
        /// Verify Certificate's Validity
        /// Parameters to check are - Validity Peroid, No Self Signed, Chain should be proper, Revocation List
        /// </summary>
        /// <param name="certificate"></param>
        /// <param name="certificateChain"></param>
        /// <returns></returns>
        public string VerifyCertificate(string certificateBase64, string certificateChainBase64 , string nameCertificate)
        {
            CertificateVerifier objVerifier = new CertificateVerifier();
            return objVerifier.VerifyCertificate(certificateBase64, certificateChainBase64, nameCertificate);
        }


        /// <summary>
        /// Verify Registered Certificate against newly input certificate
        /// </summary>
        /// <param name="certificateBase64"></param>
        /// <param name="certificateChainBase64"></param>
        /// <param name="nodalOfficerCode"></param>
        /// <returns></returns>
        public string VerifyRegisteredCertificate(string certificateBase64, string certificateChainBase64, Int32 nodalOfficerCode)
        {
            DigSignDAL objDAL = new DigSignDAL();
            CertificateVerifier objVerifier = new CertificateVerifier();
            AuthorizedSignatoryCertData model = new AuthorizedSignatoryCertData();
            model = objDAL.GetCertificateData(nodalOfficerCode);
            return objVerifier.VerifyRegisteredCertificate(certificateBase64, certificateChainBase64, model);
        }

        /// <summary>
        /// Verify Registered Certificate against newly input certificate
        /// </summary>
        /// <param name="certificateBase64"></param>
        /// <param name="certificateChainBase64"></param>
        /// <param name="nodalOfficerCode"></param>
        /// <returns></returns>
        public string VerifyRegisteredActiveCertificateforDelete(string certificateBase64, string certificateChainBase64, Int32 nodalOfficerCode)
        {
            DigSignDAL objDAL = new DigSignDAL();
            CertificateVerifier objVerifier = new CertificateVerifier();
            List<AuthorizedSignatoryCertData> lstmodel = new List<AuthorizedSignatoryCertData>();
            lstmodel = objDAL.GetCertificateDataActiveList(nodalOfficerCode);
            return objVerifier.VerifyRegisteredActiveCertificateforDelete(certificateBase64, certificateChainBase64, lstmodel);
        }

        /// <summary>
        /// Verify Registered Certificate against newly input certificate
        /// </summary>
        /// <param name="certificateBase64"></param>
        /// <param name="certificateChainBase64"></param>
        /// <param name="nodalOfficerCode"></param>
        /// <returns></returns>
        public string VerifyRegisteredCertificateforDelete(string certificateBase64, string certificateChainBase64, Int32 nodalOfficerCode)
        {
            DigSignDAL objDAL = new DigSignDAL();
            CertificateVerifier objVerifier = new CertificateVerifier();
            List<AuthorizedSignatoryCertData> lstmodel = new List<AuthorizedSignatoryCertData>();
            lstmodel = objDAL.GetCertificateDataList(nodalOfficerCode);
            return objVerifier.VerifyRegisteredCertificateforDelete(certificateBase64, certificateChainBase64, lstmodel);
        }
    }
}