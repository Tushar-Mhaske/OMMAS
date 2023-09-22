using PMGSY.Models.DigSign;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Configuration;
using System.IO;
using PMGSY.Extensions;
using PMGSY.Common;

namespace PMGSY.DigSignDocs
{
    public class CertificateVerifier
    {

        private List<X509Certificate2> _IndiaRootCACertificates = null;
        public CertificateVerifier()    //Allowed Root certificate under CCAIndia only, so in constructor adding those certificates to the List
        {
            _IndiaRootCACertificates = new List<X509Certificate2>(3);

            //--------------------------------------------------------
            String CCAIndia2022Str = "MIIFNDCCAxygAwIBAgIQdiQz69smdlqFYM0KqC/hFzANBgkqhkiG9w0BAQsFADA6MQswCQYDVQQGEwJJTjESMBAGA1UEChMJSW5kaWEgUEtJMRcwFQYDVQQDEw5DQ0EgSW5kaWEgMjAyMjAeFw0yMjAyMDIxMjA0MzdaFw00MjAyMDIxMjA0MzdaMDoxCzAJBgNVBAYTAklOMRIwEAYDVQQKEwlJbmRpYSBQS0kxFzAVBgNVBAMTDkNDQSBJbmRpYSAyMDIyMIICIjANBgkqhkiG9w0BAQEFAAOCAg8AMIICCgKCAgEAv3EBudWC8HY0oSwtJZCqpjQTGpEewl3EdDqUORV0qoFp78mdR/vuATXI83G7nF9RLvmNjgQgKr/bMx6gPO4Y57bMjAsgwEzleFclZka/sqc68iN5rS3huhrCX6MEINLyDOQ71MRA7GJCaNL6E3j1438eTu011mlikeZYBdkhvfpAVjCw90w8wcWDmqx66Y561T/RiXyz2uEhBBZAD43gV58eXStOeOTwAzEZYMrmp232GfmQKabYRfdIRus1avyuGea2nICEsRHE8M2tdzwpGP7oIy2qHBFJJ+3AwmwQA4DjmDkJtCD+58awohQavRNhqjsGD+ZifG3VR4i6WrKv8OWqZzcZj3g3Elr5+fRMlz1GSqkWPBw1Ev8KWTHazSUKF7OMxm3XzyXxQnw7fZF9GOVtx3adpfRPqYGgtbOP34EVkz4wsHvNMrvUrYcKymdOrnkTjlX26fIHUJpKGYkLk9q0jhMNKs4Rn8lj4pJ7YF33/ND4bjpV0ex1EAQz0iZvT37OnxNiuAZ/+4Djf075UuNX2ecWnadOrN1r8NAParZIwUoSUnWhU8TqAWWRqzFURHUZuOMQcA0geg4c9zqtBoUPgtQksbIAEsEXmDuRpwSIFjEkK11f5Eemfmfdg37KyIjQ67TRTmBA+kT9Q5JIm/e7m1ILg/HKckgLUOCnAMsCAwEAAaM2MDQwDwYDVR0TAQH/BAUwAwEB/zARBgNVHQ4ECgQITjtINlziX30wDgYDVR0PAQH/BAQDAgEGMA0GCSqGSIb3DQEBCwUAA4ICAQCdbE8d1c1DysKtrtYlApYIXTlY3N2XHNQ6gKoaVWsKa1TJ/ovrT+FV3bmQLet3aSoEG6pTe/vLZSg8WiF7cn7WuF4XlQS3yA2Uu8/cg/S4owqhQJp6K/Xg6UoSBad9Kog1H8deOfV8Nmb8a89zB4Yf8/AepId+Lr/3I6O7iub+PUT2QBXnksa+cf0yf+49GhyMCILZvctNSQd4Vxr9EgRvBARTrAgNQ9sEOJ6myOz4iTFR7T2pIFP8Cp15e8jEVI1q4IuHu3XlwJNk9f5k3gbwrzoy9P5rP8voQU3u9wh62JZa9U63b+u/Ur1tsKb5Lx0YUedtHvpIiIRurEPxumW0twjrx8TrAcXRrViSL7dsXAoYC0dXo154EE8jBAzgIIur7tJizxgXDEn4i2pu8Yd615YML9ii5BooEJ2j6fQ0nzyPRmx1Egw2Fjlgzzceai4TUOcaCKab86yyu5MZIp+BiPR840nw5MggbRgYH2nFRBA70toVm4VFlbZs3reGmaICm4ST6R395OxYS1iYBm5kXm9tLb4pkIhUxrkgyuiwE+DsWceBjHAYaXnCgUGKtiG9tfBMUw3fChoPb9L1yKdNof3zXDdTloMqEpO4BFrmjco8kt1v0LUQPhNZmQP4nqd4Hqx2384nPmWDXbQ+eePyxRteYGY0hJeDLVpyeYG8VQ==";
            byte[] certArrCCAIndia2022Str = Convert.FromBase64String(CCAIndia2022Str);
            X509Certificate2 x509CertCCAIndia2022 = new X509Certificate2(certArrCCAIndia2022Str);
            _IndiaRootCACertificates.Add(x509CertCCAIndia2022);

            //--------------------------------------------------------

            String CCAIndia2015Str = "MIIDKzCCAhOgAwIBAgICJ7UwDQYJKoZIhvcNAQELBQAwPjELMAkGA1UEBhMCSU4xEjAQBgNVBAoTCUluZGlhIFBLSTEbMBkGA1UEAxMSQ0NBIEluZGlhIDIwMTUgU1BMMB4XDTE1MDEyOTExMzY0M1oXDTI1MDEyOTExMzY0M1owPjELMAkGA1UEBhMCSU4xEjAQBgNVBAoTCUluZGlhIFBLSTEbMBkGA1UEAxMSQ0NBIEluZGlhIDIwMTUgU1BMMIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAl9c2iT03QQ6RP2sgX/w6fjtaXS/DDIdTPBiiyGFzDQgOVh7SbmClV2Xc/s9Qhg9aTqtiiwTKVu/h0Z6udK2q+j9BWNFVcHA+RckbgHP8CIjki3KoCrUpV7CtCpFBGpxfUH2sFxolWyHwlSvn3zesNPMHsPEdwRPvey46xalhkLJ4X1RQPLNaVWO2yxCjb/sN9Lbv5gjeohC/dpd/6tmW0dTDcwJxtKmwrKwfFyClR0bWRAI4mb7LFHj+5l/Ef91v6apHFJAR2B2AwBXUjKCZ4xkOCg/MGgic1FMHAOAfSb+CgkzPm9hu09QRABJsD31gZ0qMUNvyL+C5eYeDw1zCOQIDAQABozMwMTAPBgNVHRMBAf8EBTADAQH/MBEGA1UdDgQKBAhMEXCqj90fBzALBgNVHQ8EBAMCAQYwDQYJKoZIhvcNAQELBQADggEBAC28jBXKNO1MKCK74VMlSzqXqe0moqSrTfsKEm7nNeZnDhiBYjalzMLfGuVoAYFOs5nURxUSnXbpoaYTHbD/fbtX4+9Zm8g7UGnGYzqsoMHlkOhkKVxIQWk3rDu73eNVxteSDZWDsChXwqOplqfK3kwemv8+pS4nXttmZyeF76uKXHN8iM1HZvCdg8yEj+Ip9B9Bb7f1IQ31lFZR4/z/E1i3nwPuKZ/SAbBinpS9GYZaQ/pqiYGw6lwYh8qAlfvtAb8RGt+VFK4u4q/NkZC6syfDbxDY2Q60pXRsgbdfGcz+J06zRZhYGJxXxJJyEs+ngjnV4RCXCoYwRYVjqGtNmvw=";
            byte[] certArrCCAIndia2015Str = Convert.FromBase64String(CCAIndia2015Str);
            X509Certificate2 x509CertCCAIndia2015 = new X509Certificate2(certArrCCAIndia2015Str);
            _IndiaRootCACertificates.Add(x509CertCCAIndia2015);

            //--------------------------------------------------------

            String CCAIndia2014Str = "MIIDIzCCAgugAwIBAgICJ60wDQYJKoZIhvcNAQELBQAwOjELMAkGA1UEBhMCSU4xEjAQBgNVBAoTCUluZGlhIFBLSTEXMBUGA1UEAxMOQ0NBIEluZGlhIDIwMTQwHhcNMTQwMzA1MTAxMDQ5WhcNMjQwMzA1MTAxMDQ5WjA6MQswCQYDVQQGEwJJTjESMBAGA1UEChMJSW5kaWEgUEtJMRcwFQYDVQQDEw5DQ0EgSW5kaWEgMjAxNDCCASIwDQYJKoZIhvcNAQEBBQADggEPADCCAQoCggEBAN7IUL2K/yINrn+sglna9CkJ1AVrbJYBvsylsCF3vhStQC9kb7t4FwX7s+6AAMSakL5GUDJxVVNhMqf/2paerAzFACVNR1AiMLsG7ima4pCDhFn7t9052BQRbLBCPg4wekx6j+QULQFeW9ViLV7hjkEhKffeuoc3YaDmkkPSmA2mz6QKbUWYUu4PqQPRCrkiDH0ikdqR9eyYhWyuI7Gm/pc0atYnp1sru3rtLCaLS0ST/N/ELDEUUY2wgxglgoqEEdMhSSBL1CzaA8Ck9PErpnqC7VL+sbSyAKeJ9n56FttQzkwYjdOHMrgJRZaPb2i5VoVo1ZFkQF3ZKfiJ25VH5+8CAwEAAaMzMDEwDwYDVR0TAQH/BAUwAwEB/zARBgNVHQ4ECgQIQrjFz22zV+EwCwYDVR0PBAQDAgEGMA0GCSqGSIb3DQEBCwUAA4IBAQAdAUjv0myKyt8GC1niIZplrlksOWIR6yXLg4BhFj4ziULxsGK4Jj0sIJGCkNJeHl+Ng9UlU5EI+r89DRdrGBTF/I+g3RHcViPtOne9xEgWRMRYtWD7QZe5FvoSSGkW9aV6D4iGLPBQML6FDUkQzW9CYDCFgGC2+awRMx61dQVXiFv3Nbkqa1Pejcel8NMAmxjfm5nZMd3Ft13hy3fNF6UzsOnBtMbyZWhS8Koj2KFfSUGX+M/DS1TG2ZujwKKXCuKq7+67m0WF6zohoHJbqjkmKX34zkuFnoXaXco9NkOi0RBvLCiqR2lKfzLM7B69bje+z0EqnRNo5+s8PWSdy+xt";
            byte[] certArrCCAIndia2014Str = Convert.FromBase64String(CCAIndia2014Str);
            X509Certificate2 x509CertCCAIndia2014 = new X509Certificate2(certArrCCAIndia2014Str);
            _IndiaRootCACertificates.Add(x509CertCCAIndia2014);

            //--------------------------------------------------------

            String CCAIndia2011Str = "MIIDIzCCAgugAwIBAgICJ44wDQYJKoZIhvcNAQELBQAwOjELMAkGA1UEBhMCSU4xEjAQBgNVBAoTCUluZGlhIFBLSTEXMBUGA1UEAxMOQ0NBIEluZGlhIDIwMTEwHhcNMTEwMzExMDY0ODUyWhcNMTYwMzExMDY0ODUyWjA6MQswCQYDVQQGEwJJTjESMBAGA1UEChMJSW5kaWEgUEtJMRcwFQYDVQQDEw5DQ0EgSW5kaWEgMjAxMTCCASIwDQYJKoZIhvcNAQEBBQADggEPADCCAQoCggEBAIcKHg65oKggmBq4ehX2R/G/0sYn5BJqlgRndc2MxuP03crVxBw17k3p5CLUlzjXrGNpSMB8eWK/xyJJW0bB8RgUThgdLKVakriFFa8XVhOOnxYOIpaBhHkjQuxP3q/2v4lPAF7m1wjSlZSaEywEEYPvkY8IVax6VkSbxfTUF+vkd02U4UFYEemiqgmGLSYhIPXAzMMVzEwJu93c9VIDeWCPPHk6JTQjh0h18kFwTRowCIZ1zlgZqNcVfiPy50m+//5XUnhC8yZ33Xr3YuXLXZYzlJO7xBoWrwhX65zl9yN0jekdC7GKlHOZEZHFaKDxig2BrbRRaG5JhgAGUk4pQ+cCAwEAAaMzMDEwDwYDVR0TAQH/BAUwAwEB/zARBgNVHQ4ECgQITQeoY/LbHN8wCwYDVR0PBAQDAgEGMA0GCSqGSIb3DQEBCwUAA4IBAQBuFNog8khrZP8qdM1WtcU5D35hHzVZGx0obSN0nXi/q62JALlwhfXoFD7k7J+WZFzSsSIgowic6AxLaCiOVQW/PuBO2tic0G4g7P56O8I2R5fYKwizrsLR5gg/Sug6P1b14OU/8mm2eRLg8Dm3GOzS6YQOGDgWikk7b1fVOTu/E/DAp83yPR5skad3Y7yr2VY4D0q5GcArjPuXH8an7IbO043j0ft10fH3pmPEz+ixpJJFaPLojLGtfi0g+7ilVO3KDY8yHWimMW6wyJ7V+r8Gm28pfXoDSb8re1mTFMXqy/FMKYIOKe6KVHw8zVWCJXIOcU4leE3TTllAys7zmdGi";
            byte[] certArrCCAIndia2011Str = Convert.FromBase64String(CCAIndia2011Str);
            X509Certificate2 x509CertCCAIndia2011 = new X509Certificate2(certArrCCAIndia2011Str);
            _IndiaRootCACertificates.Add(x509CertCCAIndia2011);
        }

        /// <summary>
        /// Verify Certificate's Validity
        /// Parameters to check are - Validity Peroid, No Self Signed, Chain should be proper, Revocation List
        /// </summary>
        /// <param name="certificate"></param>
        /// <param name="certificateChain"></param>
        /// <returns></returns>
        public string VerifyCertificate(string certificateBase64, string certificateChainBase64, string nameCertificate)
        {
            try
            {

                //Generate Certificate from Base64 input string for individual certificate
                byte[] certArr = Convert.FromBase64String(certificateBase64);
                X509Certificate2 x509Cert = new X509Certificate2(certArr);

                string[] splitArr = { "$$$" };
                string[] arrCertificateChainBase64 = certificateChainBase64.Split(splitArr, StringSplitOptions.None);
                int i = 0;
                X509Certificate2[] certChainArr = new X509Certificate2[arrCertificateChainBase64.Length];
                foreach (var item in arrCertificateChainBase64)
                {
                    byte[] certChainByteArr = Convert.FromBase64String(item);
                    certChainArr[i] = new X509Certificate2(certChainByteArr);
                    i++;
                }


                string strCertName = VerifyName(x509Cert, nameCertificate);

                if (!(strCertName.Equals(string.Empty)))
                {
                    return "Name of authorised signatory is not same as name on certificate -" + strCertName.Trim() + ", name in OMMMAS: " + nameCertificate.Trim();
                }

                if (!IsValidBetweenDates(x509Cert))
                {
                    return "Invalid Validity Period.";
                }


                if (IsSelfSigned(x509Cert))
                {
                    return "You are not allowed to sign using Self Signed Certificate";
                }


                try
                {
                    //Verify Certificate Chain
                    List<X509Certificate2> trustedRootCerts = new List<X509Certificate2>();
                    List<X509Certificate2> intermediateCerts = new List<X509Certificate2>();
                    foreach (var additionalCert in certChainArr)
                    {
                        if (IsSelfSigned(additionalCert))
                        {
                            if (_IndiaRootCACertificates != null && _IndiaRootCACertificates.Count() != 0)
                            {
                                foreach (X509Certificate2 rootCACertificate in _IndiaRootCACertificates)
                                {
                                    if (rootCACertificate.Equals(additionalCert))
                                    {
                                        trustedRootCerts.Add(additionalCert);
                                    }
                                }
                            }
                        }
                        else
                        {
                            intermediateCerts.Add(additionalCert);
                        }
                    }

                    if (trustedRootCerts.Count == 0)
                    {
                        return "Root Certificate is not issued by Trusted Certifying Authority of India.";
                    }
                }
                catch (Exception ex)
                {

                }





                //if (!VerifyCertificateChain(x509Cert, intermediateCerts))
                //{
                //    return "Certificate is not issued by Trusted Certifying Authority.";
                //}

                return string.Empty;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        /// <summary>
        /// Check Date is valid between From and To Dates
        /// </summary>
        /// <param name="cert"></param>
        /// <returns></returns>
        private bool IsValidBetweenDates(X509Certificate2 cert)
        {
            try
            {
                //DateTime fromDate = Convert.ToDateTime(cert.GetEffectiveDateString());
                //DateTime toDate = Convert.ToDateTime(cert.GetExpirationDateString());

                //DateTime currentDate = DateTime.Now;
                //if (currentDate >= fromDate && currentDate <= toDate)
                //{
                //    return true;
                //}

                if (DateTime.Now >= cert.NotBefore && DateTime.Now <= cert.NotAfter)
                {
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {

                throw new Exception("Validity period of certificate is invalid");

            }
        }


        /// <summary>
        /// Is certificate is self signed
        /// </summary>
        /// <param name="cert"></param>
        /// <returns></returns>
        private bool IsSelfSigned(X509Certificate2 cert)
        {

            try
            {
                return (cert.Issuer.Equals(cert.Subject) ? true : false);
            }
            catch (Exception ex)
            {

                //Log the exception
                throw new Exception("Certificate is Self Signed.");
            }
        }


        private string VerifyName(X509Certificate2 cert, string nameRegistered)
        {
            try
            {
                if (cert.GetNameInfo(X509NameType.SimpleName, false).Trim().ToUpper().Equals(nameRegistered.Trim().ToUpper()))
                {
                    return string.Empty;
                }
                else
                {

                    return cert.GetNameInfo(X509NameType.SimpleName, false).Trim().ToUpper();
                }
            }
            catch (Exception ex)
            {

                return cert.GetNameInfo(X509NameType.SimpleName, false).Trim().ToUpper();
            }
        }

        /// <summary>
        /// Verify Certificate Chain with Trusted Roots and IntermediateCertificates
        /// </summary>
        /// <param name="primaryCert"></param>
        /// <param name="trustedRoot"></param>
        /// <param name="additionalCertificates"></param>
        /// <returns></returns>
        private bool VerifyCertificateChain(X509Certificate2 primaryCert, List<X509Certificate2> intermediateCertificates)
        {
            var chain = new X509Chain();

            //code commented by anita
            // Performs a X.509 chain validation using basic validation policy
            // if (!primaryCert.Verify())
            //   return false;

            foreach (var cert in intermediateCertificates)
            {
                chain.ChainPolicy.ExtraStore.Add(cert);
            }

            // You can alter how the chain is built/validated.
            chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
            chain.ChainPolicy.VerificationFlags = X509VerificationFlags.IgnoreWrongUsage;

            // Do the preliminary validation.
            //var primaryCert = new X509Certificate2(x509Cert);
            if (!chain.Build(primaryCert))
                return false;

            // Make sure we have the same number of elements.
            if (chain.ChainElements.Count != chain.ChainPolicy.ExtraStore.Count + 1)
                return false;

            //// Make sure all the thumbprints of the CAs match up.
            //// The first one should be 'primaryCert', leading up to the root CA.
            //for (var i = 1; i < chain.ChainElements.Count; i++)
            //{
            //    //Console.WriteLine("Element certificate is valid: {0}", chain.ChainElements[i].Certificate.Verify());
            //    if (chain.ChainElements[i].Certificate.Thumbprint != chain.ChainPolicy.ExtraStore[i-1].Thumbprint)
            //        return false;
            //}

            return true;
        }


        /// <summary>
        /// Verify Certificate is equals to registered certificate of authorized signatory
        /// </summary>
        /// <param name="certificateBase64"></param>
        /// <param name="certificateChainBase64"></param>
        /// <returns></returns>
        public string VerifyRegisteredCertificate(string certificateBase64, string certificateChainBase64, AuthorizedSignatoryCertData model)
        {
            try
            {


                if (model == null)
                {
                    return "Error occurred while getting DSC Registration Details.";
                }

                //Generate Certificate from Base64 input string for individual certificate
                byte[] certArr = Convert.FromBase64String(certificateBase64);
                X509Certificate2 x509Cert = new X509Certificate2(certArr);

                byte[] registeredCertArr = Convert.FromBase64String(model.CertificateBase64);
                X509Certificate2 registeredX509Cert = new X509Certificate2(registeredCertArr);
                if (!x509Cert.Equals(registeredX509Cert))
                {

                    return "Registered Certificate for Authorized Signatory and currently provided Certificate for Signing are having differences. Please use proper Certificate to Sign.";
                }

                string[] splitArr = { "$$$" };
                string[] arrCertificateChainBase64 = certificateChainBase64.Split(splitArr, StringSplitOptions.None);
                int i = 0;
                X509Certificate2[] certChainArr = new X509Certificate2[arrCertificateChainBase64.Length];
                foreach (var item in arrCertificateChainBase64)
                {
                    byte[] certChainByteArr = Convert.FromBase64String(item);
                    certChainArr[i] = new X509Certificate2(certChainByteArr);
                    i++;
                }

                if (!IsValidBetweenDates(x509Cert))
                {
                    return "Invalid Validity Period.";
                }

                if (IsSelfSigned(x509Cert))
                {
                    return "You are not allowed to sign using Self Signed Certificate";
                }

                //Verify Certificate Chain
                List<X509Certificate2> trustedRootCerts = new List<X509Certificate2>();
                List<X509Certificate2> intermediateCerts = new List<X509Certificate2>();
                foreach (var additionalCert in certChainArr)
                {
                    if (IsSelfSigned(additionalCert))
                    {
                        if (_IndiaRootCACertificates != null && _IndiaRootCACertificates.Count() != 0)
                        {
                            foreach (X509Certificate2 rootCACertificate in _IndiaRootCACertificates)
                            {
                                if (rootCACertificate.Equals(additionalCert))
                                {
                                    trustedRootCerts.Add(additionalCert);
                                }
                            }
                        }
                    }
                    else
                    {
                        intermediateCerts.Add(additionalCert);
                    }
                }

                //if (trustedRootCerts.Count == 0) anita 31-aug-2016
                //{
                //    return "Root Certificate is not issued by Trusted Certifying Authority of India.";
                //}


                //Commented by anita 31-aug-2016
                //if (!VerifyCertificateChain(x509Cert, intermediateCerts))
                //{
                //    return "Certificate is not issued by Trusted Certifying Authority.";
                //}

                return string.Empty;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Verify Certificate is equals to registered certificate of authorized signatory
        /// </summary>
        /// <param name="certificateBase64"></param>
        /// <param name="certificateChainBase64"></param>
        /// <returns></returns>
        public string VerifyRegisteredActiveCertificateforDelete(string certificateBase64, string certificateChainBase64, List<AuthorizedSignatoryCertData> lstmodel)
        {
            bool flag = false;
            try
            {

                if (lstmodel == null)
                {
                    //return "Error occurred while getting DSC Registration Details.";
                    return "";
                }
                foreach (AuthorizedSignatoryCertData model in lstmodel)
                {
                    flag = true;
                    //Generate Certificate from Base64 input string for individual certificate
                    byte[] certArr = Convert.FromBase64String(certificateBase64);
                    X509Certificate2 x509Cert = new X509Certificate2(certArr);

                    byte[] registeredCertArr = Convert.FromBase64String(model.CertificateBase64);
                    X509Certificate2 registeredX509Cert = new X509Certificate2(registeredCertArr);
                    if (!x509Cert.Equals(registeredX509Cert))
                    {

                        flag = false;
                        //return "Registered Certificate for Authorized Signatory and currently provided Certificate for Signing are having differences. Please use proper Certificate to Sign.";
                    }

                    string[] splitArr = { "$$$" };
                    string[] arrCertificateChainBase64 = certificateChainBase64.Split(splitArr, StringSplitOptions.None);
                    int i = 0;
                    X509Certificate2[] certChainArr = new X509Certificate2[arrCertificateChainBase64.Length];
                    foreach (var item in arrCertificateChainBase64)
                    {
                        byte[] certChainByteArr = Convert.FromBase64String(item);
                        certChainArr[i] = new X509Certificate2(certChainByteArr);
                        i++;
                    }

                    if (!IsValidBetweenDates(x509Cert))
                    {
                        flag = false;
                        //return "Invalid Validity Period.";
                    }

                    if (IsSelfSigned(x509Cert))
                    {
                        flag = false;
                        //return "You are not allowed to sign using Self Signed Certificate";
                    }
                    if (flag)
                    {
                        //Verify Certificate Chain
                        List<X509Certificate2> trustedRootCerts = new List<X509Certificate2>();
                        List<X509Certificate2> intermediateCerts = new List<X509Certificate2>();
                        foreach (var additionalCert in certChainArr)
                        {
                            if (IsSelfSigned(additionalCert))
                            {
                                if (_IndiaRootCACertificates != null && _IndiaRootCACertificates.Count() != 0)
                                {
                                    foreach (X509Certificate2 rootCACertificate in _IndiaRootCACertificates)
                                    {
                                        if (rootCACertificate.Equals(additionalCert))
                                        {
                                            trustedRootCerts.Add(additionalCert);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                intermediateCerts.Add(additionalCert);
                            }
                        }
                        //return "Registered Certificate for Authorized Signatory and currently provided Certificate for Signing are having differences. Please use proper Certificate to Sign.";
                        return "Cannot Deregister DSC as Officer is active for " + model.AdminNdName;
                    }
                }
                return ""; ;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "VerifyRegisteredActiveCertificateforDelete()");
                throw;
            }
        }

        /// <summary>
        /// Verify Certificate is equals to registered certificate of authorized signatory
        /// </summary>
        /// <param name="certificateBase64"></param>
        /// <param name="certificateChainBase64"></param>
        /// <returns></returns>
        public string VerifyRegisteredCertificateforDelete(string certificateBase64, string certificateChainBase64, List<AuthorizedSignatoryCertData> lstmodel)
        {
            bool flag = false;
            try
            {



                if (lstmodel == null)
                {
                    using (StreamWriter sw = System.IO.File.AppendText(ConfigurationManager.AppSettings["DigSignErrorLogPath"].ToString() + "PdfSignerAppletErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt"))
                    {
                        sw.WriteLine("Error occurred while getting DSC Registration Details." + DateTime.Now.ToString());
                        sw.Close();
                    }
                    return "Error occurred while getting DSC Registration Details.";
                }
                foreach (AuthorizedSignatoryCertData model in lstmodel)
                {
                    flag = true;
                    //Generate Certificate from Base64 input string for individual certificate
                    byte[] certArr = Convert.FromBase64String(certificateBase64);
                    X509Certificate2 x509Cert = new X509Certificate2(certArr);

                    byte[] registeredCertArr = Convert.FromBase64String(model.CertificateBase64);
                    X509Certificate2 registeredX509Cert = new X509Certificate2(registeredCertArr);
                    if (!x509Cert.Equals(registeredX509Cert))
                    {

                        flag = false;
                        //return "Registered Certificate for Authorized Signatory and currently provided Certificate for Signing are having differences. Please use proper Certificate to Sign.";
                    }

                    string[] splitArr = { "$$$" };
                    string[] arrCertificateChainBase64 = certificateChainBase64.Split(splitArr, StringSplitOptions.None);
                    int i = 0;
                    X509Certificate2[] certChainArr = new X509Certificate2[arrCertificateChainBase64.Length];
                    foreach (var item in arrCertificateChainBase64)
                    {
                        byte[] certChainByteArr = Convert.FromBase64String(item);
                        certChainArr[i] = new X509Certificate2(certChainByteArr);
                        i++;
                    }

                    if (!IsValidBetweenDates(x509Cert))
                    {
                        flag = false;
                        //return "Invalid Validity Period.";
                    }

                    if (IsSelfSigned(x509Cert))
                    {
                        flag = false;
                        //return "You are not allowed to sign using Self Signed Certificate";
                    }
                    if (flag)
                    {
                        //Verify Certificate Chain
                        List<X509Certificate2> trustedRootCerts = new List<X509Certificate2>();
                        List<X509Certificate2> intermediateCerts = new List<X509Certificate2>();
                        foreach (var additionalCert in certChainArr)
                        {
                            if (IsSelfSigned(additionalCert))
                            {
                                if (_IndiaRootCACertificates != null && _IndiaRootCACertificates.Count() != 0)
                                {
                                    foreach (X509Certificate2 rootCACertificate in _IndiaRootCACertificates)
                                    {
                                        if (rootCACertificate.Equals(additionalCert))
                                        {
                                            trustedRootCerts.Add(additionalCert);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                intermediateCerts.Add(additionalCert);
                            }
                        }
                        return string.Empty;
                    }
                }
                return "Registered Certificate for Authorized Signatory and currently provided Certificate for Signing are having differences. Please use proper Certificate to Sign."; ;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "VerifyRegisteredCertificateforDelete()");
                throw;
            }
        }
    }
}
