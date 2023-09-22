using Google.Protobuf;
using PMGSY.BAL.DigSign;
using PMGSY.DigSignDocs;
using PMGSY.Models.DigSign;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using PMGSY.Extensions;
using PMGSY.Common;

namespace PMGSY.Controllers
{
    //[RequiredAuthentication]
    //[RequiredAuthorization]
    public class DigSignController : Controller
    {
        /// <summary>
        /// Render View to Register Digital Sign Certificate
        /// </summary>
        /// <returns></returns>
        public ActionResult RegisterDSC()
        {
            DigSignBAL objBAL = new DigSignBAL();
            RegisterDSCModel model = new RegisterDSCModel();
            try
            {

                model = objBAL.GetDetailsToRegisterDSC();
                if (model.NodalOfficerCode == 0)
                {
                    model.IsAlreadyRegistered = 0;
                }
                else
                {
                    if (objBAL.IsAlreadyRegistered(model.NodalOfficerCode))
                    {
                        model.IsAlreadyRegistered = 1;

                    }
                    else
                    {
                        model.IsAlreadyRegistered = 2;
                    }
                }

                return View(model);
                //if ((model.IsAlreadyRegistered == 2) || (model.IsAlreadyRegistered == 0))
                //{
                //    return View(model);
                //}
                //else
                //{
                //    //return View("RegisterDSC", model);
                //    return View(model);
                //}
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "DigSignController.RegisterDSC()");
                return null;
            }
        }


        /// <summary>
        /// Save Registration Details
        /// </summary>
        /// <returns></returns>
        // [HttpPost]
        public JsonResult RegisterDSCDetails()
        {
            using (StreamWriter sw = System.IO.File.AppendText(ConfigurationManager.AppSettings["DigSignErrorLogPath"].ToString() + "PdfSignerAppletErrorLog.txt"))
            {
                sw.WriteLine(" in RegisterDSCDetails :" + DateTime.Now.ToString() + " UserName:- " + PMGSYSession.Current.UserName);
                sw.Close();
            }

            DigSignBAL objBAL = new DigSignBAL();
            try
            {
                RegistrationData model = new RegistrationData();
                model.CertificateBase64 = string.IsNullOrEmpty(Request.Params["certificate"]) ? string.Empty : Request.Params["certificate"].Trim();
                model.CertificateChainBase64 = string.IsNullOrEmpty(Request.Params["certificateChain"]) ? string.Empty : Request.Params["certificateChain"].Trim();
                model.PublicKeyBase64 = string.IsNullOrEmpty(Request.Params["publicKey"]) ? string.Empty : Request.Params["publicKey"].Trim();
                model.MobileNumber = string.IsNullOrEmpty(Request.Params["mobileNumber"]) ? string.Empty : Request.Params["mobileNumber"].Trim();
                model.Designation = string.IsNullOrEmpty(Request.Params["designation"]) ? string.Empty : Request.Params["designation"].Trim();
                model.NameAsPerCeritificate = string.IsNullOrEmpty(Request.Params["nameAsPerCeritificate"]) ? string.Empty : Request.Params["nameAsPerCeritificate"].Trim();
                model.PkcsStandard = string.IsNullOrEmpty(Request.Params["pkcsStandard"]) ? string.Empty : Request.Params["pkcsStandard"].Trim();


                if (string.IsNullOrEmpty(model.CertificateBase64) || string.IsNullOrEmpty(model.CertificateChainBase64) || string.IsNullOrEmpty(model.PublicKeyBase64))
                {
                    return Json(new { status = "error", message = "Certificate details from client to server are null or empty." });
                }

                // verify Certificate, if suucessful, then only store it in DB else flag error message
                string verificationResult = objBAL.VerifyCertificate(model.CertificateBase64, model.CertificateChainBase64, model.NameAsPerCeritificate);

                string result = "Error occurred during DSC Registration.";
                if (verificationResult.Equals(string.Empty))
                {
                    result = objBAL.SaveDSCRegistrationDetails(model);
                }
                else
                {
                    //return Json(new { status = "error", message = verificationResult });
                    return Json(new { message = verificationResult });
                }

                if (result.Equals(string.Empty))
                {
                    string strMsg = "Digital Certificate registered successfully.";
                    return Json(new { message = strMsg });
                }
                else
                {
                    // return Json(new { status = "error", message = result });
                    return Json(new { message = result });
                }


            }
            catch (Exception ex)
            {
                using (StreamWriter sw = System.IO.File.AppendText(ConfigurationManager.AppSettings["DigSignErrorLogPath"].ToString() + "PdfSignerAppletErrorLog.txt"))
                {
                    sw.WriteLine("Date :" + DateTime.Now.ToString() + " UserName:- " + PMGSYSession.Current.UserName);
                    sw.WriteLine("Method : " + "RegisterDSCDetails()");
                    sw.WriteLine("Exception Message : " + ex.Message);
                    sw.WriteLine("Exception : " + ex.StackTrace);
                    sw.WriteLine("____________________________________________________");
                    sw.Close();
                }

                return Json(new { status = "error", message = "Error occurred during DSC Registration." });

            }
        }


        /// <summary>
        /// Display already Registerd Details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult RegisteredDetails(string id)
        {
            DigSignBAL objBAL = new DigSignBAL();
            RegisterDSCModel model = new RegisterDSCModel();
            try
            {

                model = objBAL.GetDetailsToRegisterDSC();
                if (objBAL.IsAlreadyRegistered(Convert.ToInt32(id)))
                {
                    model.IsAlreadyRegistered = 1;
                }
                else
                {
                    model.IsAlreadyRegistered = 2;
                }
                return View(model);

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "DigSignController.RegisteredDetails()");
                return null;
            }
        }


        /// <summary>
        /// Action called by Applet to Sign Pdf 
        /// will return Status, Error Message if any, Pdf's Base64 string, Encrypted Hash
        /// </summary>
        /// <returns></returns>



        /// <summary>
        /// Failure URL for Signing
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult SignFailure()
        {
            string PdfKey = Request.Params["pdfKey"];
            ViewBag.Message = string.IsNullOrEmpty(PMGSYSession.Current.AppletErrMessage) ? "Error occurred while Signing the Document." : PMGSYSession.Current.AppletErrMessage;
            return View();
        }

    }
}
