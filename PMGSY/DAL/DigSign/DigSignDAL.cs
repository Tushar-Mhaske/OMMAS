using PMGSY.Extensions;
using PMGSY.Models;
using PMGSY.Models.DigSign;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Configuration;
using PMGSY.Common;
using System.Data.Entity;

namespace PMGSY.DAL.DigSign
{
    public class DigSignDAL
    {
        PMGSYEntities dbContext = null;


        /// <summary>
        /// Get Details like Name as per certificate, Designation and Mobile number of Logged in Admin Nodal Officer
        /// </summary>
        /// <returns></returns>
        public RegisterDSCModel GetDetailsToRegisterDSC()
        {
            //bool dscActive = false;
            dbContext = new PMGSYEntities();
            RegisterDSCModel model = new RegisterDSCModel();
            Int64 dscRegFileId = 0; 
            try
            {
                if (dbContext.ADMIN_NODAL_OFFICERS.Where(c => c.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode && c.ADMIN_ACTIVE_STATUS.Equals("Y") && c.ADMIN_MODULE.Equals("A")).Any())
                {
                    var nodalOfficerDetails = dbContext.ADMIN_NODAL_OFFICERS.Where(c => c.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode && c.ADMIN_ACTIVE_STATUS.Equals("Y") && c.ADMIN_MODULE.Equals("A")).First();
                    model.NodalOfficerCode = nodalOfficerDetails.ADMIN_NO_OFFICER_CODE;

                    model.IsDSCInProgress = false;//Initial value added on 07-12-2021
                    // this code commented for DBT module 
                    //if (dbContext.PFMS_OMMAS_DSC_MAPPING.Any(x => x.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode))// && /*x.ACK_DSC_STATUS == "RJCT"*/ x.IS_ACTIVE == false)
                    //{
                    //   // var dscActive = dbContext.PFMS_OMMAS_DSC_MAPPING.Where(x => x.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode).OrderByDescending(x => x.FILE_PROCESS_DATE).Select(c => c.IS_ACTIVE).FirstOrDefault();
                    //    var dscActive = dbContext.PFMS_OMMAS_DSC_MAPPING.Where(x => x.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode).OrderByDescending(x => x.FILE_PROCESS_DATE).Select(c => c.IS_ACTIVE).FirstOrDefault();
                    //    if ((dscActive == null || dscActive == false) && (nodalOfficerDetails.IS_VALID_XML == null || nodalOfficerDetails.IS_VALID_XML == false))
                    //    {
                    //        model.IsValidXmlDscRegistered = false;
                    //    }
                    //}
                    //else
                    //{
                    //    if (nodalOfficerDetails.IS_VALID_XML == null || nodalOfficerDetails.IS_VALID_XML == false)
                    //    {
                    //        model.IsValidXmlDscRegistered = false;
                    //    }
                    //    else
                    //    {
                    //        model.IsValidXmlDscRegistered = true;
                    //    }
                    //}

                    if (dbContext.REAT_OMMAS_DSC_MAPPING.Any(x => x.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode))// && /*x.ACK_DSC_STATUS == "RJCT"*/ x.IS_ACTIVE == false)
                    {
                        // var dscActive = dbContext.REAT_OMMAS_DSC_MAPPING.Where(x => x.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode && x.ADMIN_NO_OFFICER_CODE == nodalOfficerDetails.ADMIN_NO_OFFICER_CODE ).OrderByDescending(x => x.FILE_ID).Select(c => c.IS_ACTIVE).FirstOrDefault();
                      //  var dscActive = dbContext.REAT_OMMAS_DSC_MAPPING.Where(x => x.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode && x.ADMIN_NO_OFFICER_CODE == nodalOfficerDetails.ADMIN_NO_OFFICER_CODE  && x.IS_ACTIVE == true ).OrderByDescending(x => x.FILE_ID).Select(c => c.IS_ACTIVE).FirstOrDefault();
                        var dscActive = dbContext.REAT_OMMAS_DSC_MAPPING.Where(x => x.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode && x.ADMIN_NO_OFFICER_CODE == nodalOfficerDetails.ADMIN_NO_OFFICER_CODE && x.FUND_TYPE == "P" ).OrderByDescending(x => x.FILE_ID).Select(c => c.IS_ACTIVE).FirstOrDefault();
                        if (dscActive == null || dscActive == false)
                        {
                            model.IsValidXmlDscRegisteredREAT = false;
                            model.IsDSCInProgress = false;
                        }
                        else
                        {


                            dscRegFileId = dbContext.REAT_OMMAS_DSC_MAPPING.Where(x => x.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode && x.ADMIN_NO_OFFICER_CODE == nodalOfficerDetails.ADMIN_NO_OFFICER_CODE && x.FUND_TYPE == "P").OrderByDescending(x => x.FILE_ID).Select(c => c.FILE_ID).FirstOrDefault();

                            string  ackstatus = dbContext.REAT_OMMAS_DSC_MAPPING.Where(x => x.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode && x.ADMIN_NO_OFFICER_CODE == nodalOfficerDetails.ADMIN_NO_OFFICER_CODE && x.FUND_TYPE == "P").OrderByDescending(x => x.FILE_ID).Select(c => c.ACK_DSC_STATUS).FirstOrDefault();
                           
                            model.DscAckStatus = ackstatus;

                            //Below Condition modified on 26-11-2021
                            if (model.DscAckStatus == null )
                            {
                                model.IsValidXmlDscRegisteredREAT = false;
                                model.IsDSCInProgress = true;
                            }
                            else if (model.DscAckStatus.ToUpper().Equals( "RJCT"))
                            {
                                model.IsValidXmlDscRegisteredREAT = false;
                                model.IsDSCInProgress = false;
                            }
                            else if (model.DscAckStatus.ToUpper().Equals("ACCP") || model.DscAckStatus.ToUpper().Equals("ACPT"))
                            {
                                model.IsValidXmlDscRegisteredREAT = true;
                                model.IsDSCInProgress = true;
                            }
                        }
                        //Below Condition Added on 26-11-2021
                        if (dbContext.REAT_OMMAS_DSC_MAPPING.Any(x => x.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode && x.ADMIN_NO_OFFICER_CODE == nodalOfficerDetails.ADMIN_NO_OFFICER_CODE && x.FUND_TYPE == "D"))// && /*x.ACK_DSC_STATUS == "RJCT"*/ x.IS_ACTIVE == false)
                        {

                            var deRegDetails = dbContext.REAT_OMMAS_DSC_MAPPING.Where(x => x.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode && x.ADMIN_NO_OFFICER_CODE == nodalOfficerDetails.ADMIN_NO_OFFICER_CODE && x.FUND_TYPE == "D").OrderByDescending(x => x.FILE_ID).FirstOrDefault();

                            //model.DscAckStatus = deRegDetails.ACK_DSC_STATUS;
                            

                            //  var deRegDetails = dbContext.REAT_OMMAS_DSC_MAPPING.Where(x => x.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode && x.ADMIN_NO_OFFICER_CODE == nodalOfficerDetails.ADMIN_NO_OFFICER_CODE && x.FUND_TYPE == "D").First();
                            if (deRegDetails.ACK_DSC_STATUS == null || deRegDetails.ACK_DSC_STATUS == "ACCP" || deRegDetails.ACK_DSC_STATUS == "ACPT" || deRegDetails.ACK_DSC_STATUS.Equals(""))
                            {
                                model.DscDeleteEnabled = false;
                            }
                            else
                            {
                                model.DscDeleteEnabled = true;
                            }

                            if (deRegDetails.FILE_ID > dscRegFileId && (deRegDetails.ACK_DSC_STATUS == "ACCP" || deRegDetails.ACK_DSC_STATUS == null))
                            {
                                model.DSCDeregCheck = false;
                            }
                            else
                            {
                                model.DSCDeregCheck = true;
                            }
                        }
                        else
                        {
                            model.DscDeleteEnabled = true;
                            model.DSCDeregCheck = true;
                        }

                    }
                    else
                    {
                            model.IsValidXmlDscRegisteredREAT = false;
                    }

                    // model.NameAsPerCertificate = nodalOfficerDetails.ADMIN_NO_FNAME + " " + (nodalOfficerDetails.ADMIN_NO_MNAME == null ? "" : (nodalOfficerDetails.ADMIN_NO_MNAME + " ")) + nodalOfficerDetails.ADMIN_NO_LNAME;
                    if (!(nodalOfficerDetails.ADMIN_NO_FNAME.Equals(string.Empty) || nodalOfficerDetails.ADMIN_NO_FNAME == null))
                    {
                        model.NameAsPerCertificate = nodalOfficerDetails.ADMIN_NO_FNAME.Trim();
                    }
                    if (!(nodalOfficerDetails.ADMIN_NO_MNAME == null))
                    {
                        if (!(nodalOfficerDetails.ADMIN_NO_MNAME.Equals(string.Empty) || nodalOfficerDetails.ADMIN_NO_MNAME == null))
                        {
                            model.NameAsPerCertificate = model.NameAsPerCertificate.Trim() + " " + nodalOfficerDetails.ADMIN_NO_MNAME.Trim();
                        }
                    }
                    if (!(nodalOfficerDetails.ADMIN_NO_LNAME == null))
                    {
                        if (!(nodalOfficerDetails.ADMIN_NO_LNAME.Equals(string.Empty) || nodalOfficerDetails.ADMIN_NO_LNAME == null))
                        {
                            model.NameAsPerCertificate = model.NameAsPerCertificate.Trim() + " " + nodalOfficerDetails.ADMIN_NO_LNAME.Trim();
                        }
                    }

                    model.Mobile = (nodalOfficerDetails.ADMIN_NO_MOBILE == null ? "" : nodalOfficerDetails.ADMIN_NO_MOBILE);
                    model.Designation = nodalOfficerDetails.MASTER_DESIGNATION.MAST_DESIG_NAME.Trim();

                    return model;
                }
                else
                {

                   return model;
                }

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "DigSignDAL.GetDetailsToRegisterDSC()");
                throw;
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        public RegisterDSCModel GetDetailsToRegisterDSCREAT()
        {

       
            //bool dscActive = false;
            dbContext = new PMGSYEntities();
            RegisterDSCModel model = new RegisterDSCModel();
            try
            {
                if (dbContext.ADMIN_NODAL_OFFICERS.Where(c => c.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode && c.ADMIN_ACTIVE_STATUS.Equals("Y") && c.ADMIN_MODULE.Equals("A")).Any())
                {
                    var nodalOfficerDetails = dbContext.ADMIN_NODAL_OFFICERS.Where(c => c.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode && c.ADMIN_ACTIVE_STATUS.Equals("Y") && c.ADMIN_MODULE.Equals("A")).First();
                    model.NodalOfficerCode = nodalOfficerDetails.ADMIN_NO_OFFICER_CODE;

                    if (dbContext.REAT_OMMAS_DSC_MAPPING.Any(x => x.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode))// && /*x.ACK_DSC_STATUS == "RJCT"*/ x.IS_ACTIVE == false)
                    {
                        var dscActive = dbContext.REAT_OMMAS_DSC_MAPPING.Where(x => x.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode).OrderByDescending(x => x.FILE_PROCESS_DATE).Select(c => c.IS_ACTIVE).FirstOrDefault();
                        if ((dscActive == null || dscActive == false) && (nodalOfficerDetails.IS_VALID_XML == null || nodalOfficerDetails.IS_VALID_XML == false))
                        {
                            model.IsValidXmlDscRegistered = false;
                        }
                    }
                    else
                    {
                        if (nodalOfficerDetails.IS_VALID_XML == null || nodalOfficerDetails.IS_VALID_XML == false)
                        {
                            model.IsValidXmlDscRegistered = false;
                        }
                        else
                        {
                            model.IsValidXmlDscRegistered = true;
                        }
                    }
                   // Added on 26 March 2020 
                    model.IsValidXmlDscRegisteredREAT = true;
      

                    ////REAT DSC
                    if (dbContext.REAT_OMMAS_DSC_MAPPING.Where(x => x.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode && x.ADMIN_NO_OFFICER_CODE == model.NodalOfficerCode && x.IS_ACTIVE == true).Any())// && /*x.ACK_DSC_STATUS == "RJCT"*/ x.IS_ACTIVE == false)
                    {
                        model.IsValidXmlDscRegisteredREAT = true;
                    }
                    else
                    {
                        model.IsValidXmlDscRegisteredREAT = false;
                    }
                    //end

                    // model.NameAsPerCertificate = nodalOfficerDetails.ADMIN_NO_FNAME + " " + (nodalOfficerDetails.ADMIN_NO_MNAME == null ? "" : (nodalOfficerDetails.ADMIN_NO_MNAME + " ")) + nodalOfficerDetails.ADMIN_NO_LNAME;
                    if (!(nodalOfficerDetails.ADMIN_NO_FNAME.Equals(string.Empty) || nodalOfficerDetails.ADMIN_NO_FNAME == null))
                    {
                        model.NameAsPerCertificate = nodalOfficerDetails.ADMIN_NO_FNAME.Trim();
                    }
                    if (!(nodalOfficerDetails.ADMIN_NO_MNAME == null))
                    {
                        if (!(nodalOfficerDetails.ADMIN_NO_MNAME.Equals(string.Empty) || nodalOfficerDetails.ADMIN_NO_MNAME == null))
                        {
                            model.NameAsPerCertificate = model.NameAsPerCertificate.Trim() + " " + nodalOfficerDetails.ADMIN_NO_MNAME.Trim();
                        }
                    }
                    if (!(nodalOfficerDetails.ADMIN_NO_LNAME == null))
                    {
                        if (!(nodalOfficerDetails.ADMIN_NO_LNAME.Equals(string.Empty) || nodalOfficerDetails.ADMIN_NO_LNAME == null))
                        {
                            model.NameAsPerCertificate = model.NameAsPerCertificate.Trim() + " " + nodalOfficerDetails.ADMIN_NO_LNAME.Trim();
                        }
                    }

                    model.Mobile = (nodalOfficerDetails.ADMIN_NO_MOBILE == null ? "" : nodalOfficerDetails.ADMIN_NO_MOBILE);
                    model.Designation = nodalOfficerDetails.MASTER_DESIGNATION.MAST_DESIG_NAME.Trim();



                    return model;
                }
                else
                {
               
                    return model;
                }

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "DigSignDAL.GetDetailsToRegisterDSCREAT()");
                throw;
            }
            finally
            {

            

                dbContext.Dispose();
            }
        }

        /// <summary>
        /// Is User Already Registered DSC against nodal officer Code
        /// </summary>
        /// <returns></returns>
        public bool IsAlreadyRegistered(Int32 nodalOfficerCode)
        {
            dbContext = new PMGSYEntities();
            RegisterDSCModel model = new RegisterDSCModel();
            try
            {
                if (dbContext.ACC_CERTIFICATE_DETAILS.Where(c => c.ADMIN_NO_OFFICER_CODE == nodalOfficerCode).Any())
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "DigSignDAL.IsAlreadyRegistered()");
                //return false;
                throw;
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        /// <summary>
        /// Save DSC Registration Details
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public string SaveDSCRegistrationDetails(RegistrationData model)
        {
           
            dbContext = new PMGSYEntities();
            try
            {
                var nodalOfficerCode = dbContext.ADMIN_NODAL_OFFICERS.Where(c => c.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode && c.ADMIN_ACTIVE_STATUS.Equals("Y") && c.ADMIN_MODULE.Equals("A")).Select(c => c.ADMIN_NO_OFFICER_CODE).First();
                if (dbContext.ACC_CERTIFICATE_DETAILS.Where(c => c.ADMIN_NO_OFFICER_CODE == nodalOfficerCode).Any())
                {
                    return "DSC registration for the nodal officer is already done";
                }

                ACC_CERTIFICATE_DETAILS accCertificateDetails = new ACC_CERTIFICATE_DETAILS();
                accCertificateDetails.ADMIN_NO_OFFICER_CODE = nodalOfficerCode;
                accCertificateDetails.CERTIFICATE = model.CertificateBase64;
                accCertificateDetails.CERTIFICATE_CHAIN = model.CertificateChainBase64; //Chain of certificates having Base64String seperated by "$$$"
                accCertificateDetails.PUBLIC_KEY = model.PublicKeyBase64;
                accCertificateDetails.USERID = PMGSYSession.Current.UserId;
                accCertificateDetails.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                dbContext.ACC_CERTIFICATE_DETAILS.Add(accCertificateDetails);

                #region PFMS
                //PFMS_OMMAS_DSC_MAPPING dsc = dbContext.PFMS_OMMAS_DSC_MAPPING.Where(x => x.ADMIN_ND_CODE == PMGSYSession.Current.AdminNdCode && /*x.ACK_DSC_STATUS == "RJCT"*/ x.IS_ACTIVE == false).FirstOrDefault();
                //if (dsc != null)
                //{
                //    dsc.IS_ACTIVE = true;
                //    dbContext.Entry(dsc).State = System.Data.Entity.EntityState.Modified;
                //}
                #endregion

                dbContext.SaveChanges();

                using (StreamWriter sw = System.IO.File.AppendText(ConfigurationManager.AppSettings["DigSignErrorLogPath"].ToString() +
                    "PdfSignerAppletErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt"))
                {
                    sw.WriteLine(" Out of SaveDSCRegistrationDetails :" + DateTime.Now.ToString() + " UserName:- " + PMGSYSession.Current.UserName);
                    sw.Close();
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "DigSignDAL.SaveDSCRegistrationDetails()");
                return "Error occurred saving DSC Registration Details.";
                throw;
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        /// <summary>
        /// fetch certificate data
        /// </summary>
        /// <param name="nodalOfficerCode"></param>
        /// <returns></returns>
        public AuthorizedSignatoryCertData GetCertificateData(Int32 nodalOfficerCode)
        {


        

            dbContext = new PMGSYEntities();
            AuthorizedSignatoryCertData model = new AuthorizedSignatoryCertData();
            try
            {
                if (dbContext.ACC_CERTIFICATE_DETAILS.Where(c => c.ADMIN_NO_OFFICER_CODE == nodalOfficerCode).Any())
                {
                    var certDetails = dbContext.ACC_CERTIFICATE_DETAILS.Where(c => c.ADMIN_NO_OFFICER_CODE == nodalOfficerCode).First();
                    model.NodalOfficerCode = nodalOfficerCode;
                    model.CertificateBase64 = certDetails.CERTIFICATE;
                    model.CertificateChainBase64 = certDetails.CERTIFICATE_CHAIN;
                     return model;
                }
                else
                {

                    return null;
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "DigSignDAL.GetCertificateData()");
                throw;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        /// <summary>
        /// fetch certificate data
        /// </summary>
        /// <param name="nodalOfficerCode"></param>
        /// <returns></returns>
        public List<AuthorizedSignatoryCertData> GetCertificateDataActiveList(Int32 nodalOfficerCode)
        {
           

            dbContext = new PMGSYEntities();
            List<AuthorizedSignatoryCertData> lstmodel = new List<AuthorizedSignatoryCertData>();
            AuthorizedSignatoryCertData model = null;
            try
            {
                //if (dbContext.ACC_CERTIFICATE_DETAILS.Where(c => c.ADMIN_NO_OFFICER_CODE == nodalOfficerCode).Any())
                if (dbContext.ACC_CERTIFICATE_DETAILS.Where(c => c.ADMIN_NODAL_OFFICERS.ADMIN_DEPARTMENT.MAST_STATE_CODE == PMGSYSession.Current.StateCode).Any())
                {
                   // var certDetails = dbContext.ACC_CERTIFICATE_DETAILS.Where(c =>/* c.ADMIN_NO_OFFICER_CODE == nodalOfficerCode &&*/ c.ADMIN_NODAL_OFFICERS.ADMIN_DEPARTMENT.MAST_STATE_CODE == PMGSYSession.Current.StateCode && c.ADMIN_NODAL_OFFICERS.ADMIN_MODULE == "A" && c.ADMIN_NODAL_OFFICERS.ADMIN_ACTIVE_STATUS == "Y").OrderBy(v => v.ADMIN_NODAL_OFFICERS.ADMIN_ND_CODE).ToList();
                    var certDetails = dbContext.ACC_CERTIFICATE_DETAILS.Where(c => c.ADMIN_NO_OFFICER_CODE == nodalOfficerCode).ToList();


                    foreach (var itm in certDetails)
                    {
                        model = new AuthorizedSignatoryCertData();
                        model.NodalOfficerCode = itm.ADMIN_NO_OFFICER_CODE;
                        model.CertificateBase64 = itm.CERTIFICATE;
                        model.CertificateChainBase64 = itm.CERTIFICATE_CHAIN;
                        model.AdminNdName = itm.ADMIN_NODAL_OFFICERS.ADMIN_DEPARTMENT.ADMIN_ND_NAME;

                      
                        lstmodel.Add(model);
                    }
                    lstmodel = lstmodel.OrderBy(x=>x.NodalOfficerCode).ToList();
                    return lstmodel;
                }
                else
                {
                    using (StreamWriter sw = System.IO.File.AppendText(ConfigurationManager.AppSettings["DigSignErrorLogPath"].ToString() +
                       "PdfSignerAppletErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt"))
                    {
                        sw.WriteLine("Date :" + DateTime.Now.ToString() + " UserName:- " + PMGSYSession.Current.UserName);
                        sw.WriteLine("Method : " + " Out of  GetCertificateDataActiveList()");
                        sw.WriteLine("____________________________________________________");
                        sw.Close();
                    }

                    return null;
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "DigSignDAL.GetCertificateDataActiveList()");
                throw;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        /// <summary>
        /// fetch certificate data
        /// </summary>
        /// <param name="nodalOfficerCode"></param>
        /// <returns></returns>
        public List<AuthorizedSignatoryCertData> GetCertificateDataList(Int32 nodalOfficerCode)
        {


            using (StreamWriter sw = System.IO.File.AppendText(ConfigurationManager.AppSettings["DigSignErrorLogPath"].ToString() + "PdfSignerAppletErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt"))
            {
                sw.WriteLine("Date :" + DateTime.Now.ToString() + " UserName:- " + PMGSYSession.Current.UserName);
                sw.WriteLine("Method : " + " In GetCertificateDataList()");
                sw.WriteLine("____________________________________________________");
                sw.Close();
            }

            dbContext = new PMGSYEntities();
            List<AuthorizedSignatoryCertData> lstmodel = new List<AuthorizedSignatoryCertData>();
            AuthorizedSignatoryCertData model = null;
            try
            {
                if (dbContext.ACC_CERTIFICATE_DETAILS_SHADOW.Where(c => c.ADMIN_NO_OFFICER_CODE == nodalOfficerCode).Any())
                {
                    var certDetails = dbContext.ACC_CERTIFICATE_DETAILS_SHADOW.Where(c => c.ADMIN_NO_OFFICER_CODE == nodalOfficerCode && c.AuditAction == "D").OrderByDescending(v => v.AuditDate).ToList();
                    if (certDetails == null || certDetails.Count() == 0)
                    {
                        return null;
                    }
                    foreach (var itm in certDetails)
                    {
                        model = new AuthorizedSignatoryCertData();
                        model.NodalOfficerCode = nodalOfficerCode;
                        model.CertificateBase64 = itm.CERTIFICATE;
                        model.CertificateChainBase64 = itm.CERTIFICATE_CHAIN;
                        using (StreamWriter sw = System.IO.File.AppendText(ConfigurationManager.AppSettings["DigSignErrorLogPath"].ToString() +
                            "PdfSignerAppletErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt"))
                        {
                            sw.WriteLine("Date :" + DateTime.Now.ToString() + " UserName:- " + PMGSYSession.Current.UserName);
                            sw.WriteLine("Method : " + " Out of  GetCertificateDataList()");
                            sw.WriteLine("____________________________________________________");
                            sw.Close();
                        }
                        lstmodel.Add(model);
                    }
                    return lstmodel;
                }
                else
                {
                    using (StreamWriter sw = System.IO.File.AppendText(ConfigurationManager.AppSettings["DigSignErrorLogPath"].ToString() +
                       "PdfSignerAppletErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt"))
                    {
                        sw.WriteLine("Date :" + DateTime.Now.ToString() + " UserName:- " + PMGSYSession.Current.UserName);
                        sw.WriteLine("Method : " + " Out of  GetCertificateDataList()");
                        sw.WriteLine("____________________________________________________");
                        sw.Close();
                    }

                    return null;
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "DigSignDAL.GetCertificateDataList()");
                throw;
            }
            finally
            {
                dbContext.Dispose();
            }
        }
    }
}