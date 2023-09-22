using PMGSY.Areas.RCTRC.Models;
using PMGSY.Common;
using PMGSY.Controllers;
using PMGSY.DAL;
using PMGSY.Extensions;
using PMGSY.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace PMGSY.Areas.RCTRC.DAL
{
    public class RctrcDAL : IRctrcDAL
    {
        PMGSYEntities dbContext = null;
        Dictionary<string, string> decryptedParameters = null;
        string[] encryptedParameters = null;

        #region Registration
        public bool CdRCTRCRegistrationDAL(RCTRCRegistrationModel rctrcViewModel, ref string message)
        {
            try
            {
                dbContext = new PMGSYEntities();

                if (dbContext.RCTRC_CONTACT_DETAILS.Where(m => m.RCTRC_Contact_eMail == rctrcViewModel.EMAIL).Any())
                {
                    message = "Email ID ( " + rctrcViewModel.EMAIL + " ) "+ "is already exist.";
                    return false;
                }

                DateTime DOB = DateTime.Parse(rctrcViewModel.DOB);
                DateTime DOJ = DateTime.Parse(rctrcViewModel.JOINING_DATE);
                int result = DateTime.Compare(DOB, DOJ);
                if (result == 1)
                {
                    message = "Date of Joining Services must be greater than Date of Birth";
                    return false;
                }

                DateTime G = DateTime.Parse(rctrcViewModel.GraduationDate);
                DateTime PG = DateTime.Parse(rctrcViewModel.PostGraduationDate);
                int result1 = DateTime.Compare(G, PG);
                if (result1 == 1)
                {
                    message = "Date of Graduation Completion can not be greater than Date of Post Graduation Completion";
                    return false;
                }

                int res = DateTime.Compare(DOB, G);
                if (res == 1)
                {
                    message = "Date of Birth can not be greater than Date of Graduation Completion";
                    return false;
                }


                int res1 = DateTime.Compare(DOB, PG);
                if (res1 == 1)
                {
                    message = "Date of Birth can not be greater than Date of Post Graduation Completion";
                    return false;
                }
                RCTRC_CONTACT_DETAILS master = new RCTRC_CONTACT_DETAILS();
                master.RCTRC_Contact_Id = dbContext.RCTRC_CONTACT_DETAILS.Max(cp => (Int32?)cp.RCTRC_Contact_Id) == null ? 1 : (Int32)dbContext.RCTRC_CONTACT_DETAILS.Max(cp => (Int32?)cp.RCTRC_Contact_Id) + 1;
                master.STATE_CODE = rctrcViewModel.StateCode;
                master.STATE = dbContext.MASTER_STATE.Where(m => m.MAST_STATE_CODE == rctrcViewModel.StateCode).Select(m => m.MAST_STATE_NAME).FirstOrDefault();
                master.DISTRICT_CODE = rctrcViewModel.DistrictCode;
                master.DISTRICT = dbContext.MASTER_DISTRICT.Where(m => m.MAST_DISTRICT_CODE == rctrcViewModel.DistrictCode).Select(m => m.MAST_DISTRICT_NAME).FirstOrDefault();
                master.RCTRC_Contact_Password_ENC = new Login().EncodePassword(rctrcViewModel.Password);
                master.USERID = PMGSYSession.Current.UserId;
                master.RCTRC_Contact_Name = rctrcViewModel.CONTACT_NAME.Trim();
                master.RCTRC_Contact_eMail = rctrcViewModel.EMAIL.Trim();
                master.RCTRC_Contact_Mobile = rctrcViewModel.MOBILE.Trim();
                master.RCTRC_Contact_Phone = null;
                master.RCTRC_Contact_DOB = DateTime.Parse(rctrcViewModel.DOB);
                // master.RCTRC_Contact_Designation = rctrcViewModel.DesignationCode;
                master.RCTRC_Contact_Designation_Text = rctrcViewModel.DesignationText;
                master.RCTRC_Contact_DOJ = DateTime.Parse(rctrcViewModel.JOINING_DATE);
                master.RCTRC_Contact_Deputatoin = rctrcViewModel.DEPUTATION;
                master.RCTRC_Contact_Graduation = rctrcViewModel.GraduationCode;
                master.RCTRC_Contact_Grad_Comp = DateTime.Parse(rctrcViewModel.GraduationDate);
                master.RCTRC_Contact_Graduation_Text = dbContext.RCTRC_MASTER_EDUCATION.Where(m => m.RCTRC_EDUCATION_ID == master.RCTRC_Contact_Graduation).Select(m => m.RCTRC_EDUCATION_NAME).FirstOrDefault();
                master.RCTRC_Contact_PG = rctrcViewModel.PostGraduationCode;
                master.RCTRC_Contact_PG_Comp = DateTime.Parse(rctrcViewModel.PostGraduationDate);
                master.RCTRC_Contact_PG_Text = dbContext.RCTRC_MASTER_EDUCATION.Where(m => m.RCTRC_EDUCATION_ID == master.RCTRC_Contact_PG).Select(m => m.RCTRC_EDUCATION_NAME).FirstOrDefault();
                master.RCTRC_Contact_CompAtHome = rctrcViewModel.ComputerAtHomeCode;
                master.RCTRC_Contact_CompAtOffice = rctrcViewModel.ComputerAtOfficeCode;
                master.RCTRC_Contact_Password = rctrcViewModel.Password;
                dbContext.RCTRC_CONTACT_DETAILS.Add(master);
                dbContext.SaveChanges();
                return true;
            }
            catch (OptimisticConcurrencyException ex)
            {
                ErrorLog.LogError(ex, "RctrcDAL.CdRCTRCRegistrationDAL");
                return false;
            }
            catch (UpdateException ex)
            {
                ErrorLog.LogError(ex, "RctrcDAL.CdRCTRCRegistrationDAL");
                return false;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "RctrcDAL.CdRCTRCRegistrationDAL");
                return false;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }

        }
        public Array ListRCTRCRegistrationDetals(int page, int rows, string sidx, string sord, out long totalRecords)
        {
            try
            {
                dbContext = new PMGSYEntities();
                List<RCTRC_CONTACT_DETAILS> list = dbContext.RCTRC_CONTACT_DETAILS.ToList<RCTRC_CONTACT_DETAILS>();
                IQueryable<RCTRC_CONTACT_DETAILS> query1 = list.AsQueryable<RCTRC_CONTACT_DETAILS>();

                IQueryable<RCTRC_CONTACT_DETAILS> query = query1.Where(m => m.USERID == PMGSYSession.Current.UserId).AsQueryable<RCTRC_CONTACT_DETAILS>();

                totalRecords = list.Count;
                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        query = query.OrderBy(x => x.RCTRC_Contact_Name).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                    }
                    else
                    {
                        query = query.OrderByDescending(x => x.RCTRC_Contact_Name).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                    }
                }
                else
                {
                    query = query.OrderBy(x => x.RCTRC_Contact_eMail).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                }

                var result = query.Select(Details => new
                {
                    Details.RCTRC_Contact_Id,

                    Details.RCTRC_Contact_Name,
                    Details.RCTRC_Contact_DOB,
                    Details.RCTRC_Contact_Designation_Text,
                    Details.RCTRC_Contact_DOJ,
                    Details.RCTRC_Contact_Deputatoin,
                    Details.RCTRC_Contact_Mobile,
                    Details.RCTRC_Contact_eMail,
                    Details.STATE,
                    Details.DISTRICT,

                    Details.RCTRC_Contact_Grad_Comp,
                    Details.RCTRC_Contact_Graduation_Text,
                    Details.RCTRC_Contact_PG_Comp,
                    Details.RCTRC_Contact_PG_Text,
                    Details.RCTRC_Contact_CompAtHome,
                    Details.RCTRC_Contact_CompAtOffice
                   

                }).ToArray();

                return result.Select(Details => new
                {
                    cell = new[]{

                    Convert.ToString(Details.RCTRC_Contact_Name),
                    Convert.ToString(Details.RCTRC_Contact_DOB).Substring(0,10),
                    Convert.ToString(Details.RCTRC_Contact_Designation_Text).ToString(),
                    Convert.ToString(Details.RCTRC_Contact_DOJ).Substring(0,10),
                    Convert.ToString(Details.RCTRC_Contact_Deputatoin),
                    Convert.ToString(Details.RCTRC_Contact_Mobile),
                    Convert.ToString(Details.RCTRC_Contact_eMail),
                    Convert.ToString(Details.STATE),
                    Convert.ToString(Details.DISTRICT),

                  
                    Convert.ToString(Details.RCTRC_Contact_Grad_Comp).Substring(0,10),
                    Convert.ToString( Details.RCTRC_Contact_Graduation_Text),
                    Convert.ToString(Details.RCTRC_Contact_PG_Comp).Substring(0,10),
                    Convert.ToString(Details.RCTRC_Contact_PG_Text),

                   
                   
                    Convert.ToString(Details.RCTRC_Contact_CompAtHome) =="Y"?"Yes":"No",
                    Convert.ToString(Details.RCTRC_Contact_CompAtOffice)=="Y"?"Yes":"No",

                   "<a href='#' title='Click here to delete Details' class='ui-icon ui-icon-trash ui-align-center' onClick=DeleteRCTRCRegistrationDetails('" + URLEncrypt.EncryptParameters1(new string[]{"RCTRCCode="+ Details.RCTRC_Contact_Id.ToString().Trim()}) +"'); return false;'>Delete</a>"
                    // URLEncrypt.EncryptParameters1(new string[]{"RCTRCCode =" + Details.RCTRC_Contact_Id.ToString().Trim()}),
                }
                }).ToArray();
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "RctrcDAL.ListRCTRCRegistrationDetals");
                totalRecords = 0;
                return null;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }
        public string DeleteRegistrationDAL(int RegCode)
        {
            PMGSY.Models.PMGSYEntities dbContext = new PMGSY.Models.PMGSYEntities();
            RCTRC_CONTACT_DETAILS inspMaster = dbContext.RCTRC_CONTACT_DETAILS.Where(m => m.RCTRC_Contact_Id == RegCode).FirstOrDefault();

            try
            {
                if (inspMaster == null)
                {
                    return "Your Request can not be processed right now. This may be due to Invalid Request, Session Timeout or Invalid Data Entry. Please verify and Try Again.";
                }

                dbContext.RCTRC_CONTACT_DETAILS.Remove(inspMaster);
                dbContext.SaveChanges();
                return string.Empty;

            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException)
            {
                return "Details can not be deleted because other details for this record are entered.";

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "RCTRC().DeleteRegistrationDAL");
                return "Your Request can not be processed right now. This may be due to Invalid Request, Session Timeout or Invalid Data Entry. Please verify and Try Again.";

            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }
        #endregion

        #region Training
        public bool AddTrainingDAL(RCTRCTraining rctrcViewModel, ref string message)
        {
            try
            {
                dbContext = new PMGSYEntities();
                if (dbContext.RCTRC_CONTACT_TRAININGS.Where(m => m.RCTRC_Contact_Id == rctrcViewModel.ContactPersonID && m.RCTRC_CONTACT_TRG_TOPIC == rctrcViewModel.TOPIC).Any())
                {
                    message = "Person details and Training Topic Name  already exist.";
                    return false;
                }
                DateTime startDate = DateTime.Parse(rctrcViewModel.START_DATE);
                DateTime endDate = DateTime.Parse(rctrcViewModel.END_DATE);
                int result = DateTime.Compare(startDate, endDate);             
                if (result == 1)
                {
                    message = "Start Date is greater than End Date";
                    return false;
                }
                RCTRC_CONTACT_TRAININGS master = new RCTRC_CONTACT_TRAININGS();
                master.RCTRC_CONTACT_TRG_ID = dbContext.RCTRC_CONTACT_TRAININGS.Max(cp => (Int32?)cp.RCTRC_CONTACT_TRG_ID) == null ? 1 : (Int32)dbContext.RCTRC_CONTACT_TRAININGS.Max(cp => (Int32?)cp.RCTRC_CONTACT_TRG_ID) + 1;
                master.RCTRC_Contact_Id = rctrcViewModel.ContactPersonID;
                master.RCTRC_CONTACT_TRG_TOPIC = rctrcViewModel.TOPIC.Trim();
                master.RCTRC_CONTACT_TRG_DURATION = rctrcViewModel.DURATION.Trim();
                master.RCTRC_CONTACT_TRG_START_DATE = DateTime.Parse(rctrcViewModel.START_DATE);
                master.RCTRC_CONTACT_TRG_END_DATE = DateTime.Parse(rctrcViewModel.END_DATE);
                master.RCTRC_CONTACT_TRG_HOST = rctrcViewModel.HOST.Trim();
                master.RCTRC_CONTACT_TRG_ENTRY_DATE = System.DateTime.Now;
                master.USERID = PMGSYSession.Current.UserId;

                dbContext.RCTRC_CONTACT_TRAININGS.Add(master);
                dbContext.SaveChanges();
                return true;
            }
            catch (OptimisticConcurrencyException ex)
            {
                ErrorLog.LogError(ex, "RctrcDAL.AddTrainingDAL");
                return false;
            }
            catch (UpdateException ex)
            {
                ErrorLog.LogError(ex, "RctrcDAL.AddTrainingDAL");
                return false;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "RctrcDAL.AddTrainingDAL");
                return false;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }

        }
        public Array GetRCTRCTrainingListDAL(int page, int rows, string sidx, string sord, out long totalRecords, int PersonCode)
        {
            try
            {
                dbContext = new PMGSYEntities();
                //List<RCTRC_CONTACT_APPLICATION_PROFECIENCY> list = dbContext.RCTRC_CONTACT_APPLICATION_PROFECIENCY.ToList<RCTRC_CONTACT_APPLICATION_PROFECIENCY>();
                //IQueryable<RCTRC_CONTACT_APPLICATION_PROFECIENCY> query = list.AsQueryable<RCTRC_CONTACT_APPLICATION_PROFECIENCY>();

                if (PersonCode == 0 || PersonCode == -1)
                {
                    var list1 = dbContext.USP_GET_RCTRC_TRAINING_LIST_PER_PERSON().ToList();

                    var list = list1.Where(m => m.USERID == PMGSYSession.Current.UserId).ToList();

                   

                    totalRecords = list.Count;
                    if (sidx.Trim() != string.Empty)
                    {
                        if (sord.ToString() == "asc")
                        {
                            list = list.OrderBy(x => x.Contact_Person).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                        }
                        else
                        {
                            list = list.OrderByDescending(x => x.RCTRC_CONTACT_TRG_TOPIC).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                        }
                    }
                    else
                    {
                        list = list.OrderBy(x => x.RCTRC_CONTACT_TRG_HOST).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                    }

                    var result = list.Select(Details => new
                    {
                        Details.Contact_Person,
                        Details.RCTRC_CONTACT_TRG_TOPIC,
                        Details.RCTRC_CONTACT_TRG_HOST,
                        Details.RCTRC_CONTACT_TRG_DURATION,
                        Details.RCTRC_CONTACT_TRG_START_DATE,
                        Details.RCTRC_CONTACT_TRG_END_DATE

                    }).ToArray();

                    return result.Select(Details => new
                    {
                        cell = new[]{
                    Details.Contact_Person.ToString(),
                    Details.RCTRC_CONTACT_TRG_TOPIC.ToString(),
                    Details.RCTRC_CONTACT_TRG_HOST.ToString(),
                    Details.RCTRC_CONTACT_TRG_DURATION.ToString(),
                     Details.RCTRC_CONTACT_TRG_START_DATE.ToString().Substring(0,10),
                      Details.RCTRC_CONTACT_TRG_END_DATE.ToString().Substring(0,10)
                  // "<a href='#' title='Click here to delete Details' class='ui-icon ui-icon-trash ui-align-center' onClick=DeleteRCTRCRegistrationDetails('" + URLEncrypt.EncryptParameters1(new string[]{"RCTRCCode="+ Details.RCTRC_Contact_Id.ToString().Trim()}) +"'); return false;'>Delete</a>"
                    // URLEncrypt.EncryptParameters1(new string[]{"RCTRCCode =" + Details.RCTRC_Contact_Id.ToString().Trim()}),
                }
                    }).ToArray();
                }
                else
                {
                    var list = dbContext.USP_GET_RCTRC_TRAINING_LIST_PER_PERSON().Where(m => m.RCTRC_Contact_Id == PersonCode && m.USERID == PMGSYSession.Current.UserId).ToList();

                    totalRecords = list.Count;
                    if (sidx.Trim() != string.Empty)
                    {
                        if (sord.ToString() == "asc")
                        {
                            list = list.OrderBy(x => x.Contact_Person).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                        }
                        else
                        {
                            list = list.OrderByDescending(x => x.RCTRC_CONTACT_TRG_TOPIC).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                        }
                    }
                    else
                    {
                        list = list.OrderBy(x => x.RCTRC_CONTACT_TRG_HOST).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                    }

                    var result = list.Select(Details => new
                    {
                        Details.Contact_Person,
                        Details.RCTRC_CONTACT_TRG_TOPIC,
                        Details.RCTRC_CONTACT_TRG_HOST,
                        Details.RCTRC_CONTACT_TRG_DURATION,
                        Details.RCTRC_CONTACT_TRG_START_DATE,
                        Details.RCTRC_CONTACT_TRG_END_DATE

                    }).ToArray();

                    return result.Select(Details => new
                    {
                        cell = new[]{
                    Details.Contact_Person.ToString(),
                    Details.RCTRC_CONTACT_TRG_TOPIC.ToString(),
                    Details.RCTRC_CONTACT_TRG_HOST.ToString(),
                    Details.RCTRC_CONTACT_TRG_DURATION.ToString(),
                     Details.RCTRC_CONTACT_TRG_START_DATE.ToString().Substring(0,10),
                      Details.RCTRC_CONTACT_TRG_END_DATE.ToString().Substring(0,10)
                  // "<a href='#' title='Click here to delete Details' class='ui-icon ui-icon-trash ui-align-center' onClick=DeleteRCTRCRegistrationDetails('" + URLEncrypt.EncryptParameters1(new string[]{"RCTRCCode="+ Details.RCTRC_Contact_Id.ToString().Trim()}) +"'); return false;'>Delete</a>"
                    // URLEncrypt.EncryptParameters1(new string[]{"RCTRCCode =" + Details.RCTRC_Contact_Id.ToString().Trim()}),
                }
                    }).ToArray();
                }



            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "RctrcDAL.GetRCTRCApplicationListDAL");
                totalRecords = 0;
                return null;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }
        #endregion

        #region Applications
        public bool AddApplicationDAL(RCTRCApplications rctrcViewModel, ref string message, FormCollection formCollection)
        {

            try
            {
                using (TransactionScope transactionScope = new TransactionScope())
                {
                    try
                    {
                        dbContext = new PMGSYEntities();


                        if (dbContext.RCTRC_CONTACT_APPLICATION_PROFECIENCY.Where(m => m.RCTRC_Contact_Id == rctrcViewModel.ContactPersonID).Any())
                        {
                            message = "Proficiency in Applications are already added for this Person.";
                            return false;
                        }

                        rctrcViewModel.MASTER_APPLICATIONS_LIST = dbContext.USP_GET_RCTRC_MASTER_APPLICATIONS().ToList<USP_GET_RCTRC_MASTER_APPLICATIONS_Result>();
                        string ItemID = string.Empty;
                        string ItemValue = string.Empty;

                        foreach (var item in rctrcViewModel.MASTER_APPLICATIONS_LIST)
                        {
                            RCTRC_CONTACT_APPLICATION_PROFECIENCY master = new RCTRC_CONTACT_APPLICATION_PROFECIENCY();
                            ItemID = "item" + item.RCTRC_APPLICATION_ID;
                            ItemValue = formCollection[ItemID];
                            master.RCTRC_CONTACT_APPL_ID = dbContext.RCTRC_CONTACT_APPLICATION_PROFECIENCY.Max(cp => (Int32?)cp.RCTRC_CONTACT_APPL_ID) == null ? 1 : (Int32)dbContext.RCTRC_CONTACT_APPLICATION_PROFECIENCY.Max(cp => (Int32?)cp.RCTRC_CONTACT_APPL_ID) + 1;
                            master.RCTRC_Contact_Id = rctrcViewModel.ContactPersonID;
                            master.RCTRC_APPLICATION_ID = item.RCTRC_APPLICATION_ID;
                            master.RCTRC_CONTACT_APPL_PROF = ItemValue;
                            master.RCTRC_CONTACT_APPL_DATE = System.DateTime.Now;
                            master.USERID = PMGSYSession.Current.UserId;
                            dbContext.RCTRC_CONTACT_APPLICATION_PROFECIENCY.Add(master);
                            dbContext.SaveChanges();

                        }

                        transactionScope.Complete();
                        return true;
                    }

                    catch (TransactionException ex)
                    {
                        transactionScope.Dispose();
                        return false;
                    }
                }
            }
            catch (OptimisticConcurrencyException ex)
            {
                ErrorLog.LogError(ex, "RctrcDAL.AddApplicationDAL");
                return false;
            }
            catch (UpdateException ex)
            {
                ErrorLog.LogError(ex, "RctrcDAL.AddApplicationDAL");
                return false;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "RctrcDAL.AddApplicationDAL");
                return false;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }

        }

        public Array GetRCTRCApplicationListDAL(int page, int rows, string sidx, string sord, out long totalRecords, int PersonCode)
        {
            try
            {
                dbContext = new PMGSYEntities();
                //List<RCTRC_CONTACT_APPLICATION_PROFECIENCY> list = dbContext.RCTRC_CONTACT_APPLICATION_PROFECIENCY.ToList<RCTRC_CONTACT_APPLICATION_PROFECIENCY>();
                //IQueryable<RCTRC_CONTACT_APPLICATION_PROFECIENCY> query = list.AsQueryable<RCTRC_CONTACT_APPLICATION_PROFECIENCY>();


                if (PersonCode == 0 || PersonCode == -1)
                {
                    var list1 = dbContext.USP_GET_RCTRC_PROFICIENCY_APPLICATIONS_LIST_PER_PERSON().ToList();

                    var list = list1.Where(m => m.USERID == PMGSYSession.Current.UserId).ToList();

                    totalRecords = list.Count;
                    if (sidx.Trim() != string.Empty)
                    {
                        if (sord.ToString() == "asc")
                        {
                            list = list.OrderBy(x => x.Person_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                        }
                        else
                        {
                            list = list.OrderByDescending(x => x.Proficiency).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                        }
                    }
                    else
                    {
                        list = list.OrderBy(x => x.Application_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                    }

                    var result = list.Select(Details => new
                    {
                        Details.RCTRC_CONTACT_APPL_ID,
                        Details.Person_NAME,
                        Details.Application_NAME,
                        Details.Proficiency

                    }).ToArray();

                    return result.Select(Details => new
                    {
                        cell = new[]{
                    Details.Person_NAME.ToString(),
                    Details.Application_NAME.ToString(),
                    Details.Proficiency.ToString(),
                  // "<a href='#' title='Click here to delete Details' class='ui-icon ui-icon-trash ui-align-center' onClick=DeleteRCTRCRegistrationDetails('" + URLEncrypt.EncryptParameters1(new string[]{"RCTRCCode="+ Details.RCTRC_Contact_Id.ToString().Trim()}) +"'); return false;'>Delete</a>"
                    // URLEncrypt.EncryptParameters1(new string[]{"RCTRCCode =" + Details.RCTRC_Contact_Id.ToString().Trim()}),
                }
                    }).ToArray();
                }
                else
                {
                    //  var list = dbContext.USP_GET_RCTRC_TRAINING_LIST_PER_PERSON().Where(m => m.RCTRC_Contact_Id == PersonCode).ToList();

                    var list = dbContext.USP_GET_RCTRC_PROFICIENCY_APPLICATIONS_LIST_PER_PERSON().Where(m => m.RCTRC_Contact_Id == PersonCode && m.USERID == PMGSYSession.Current.UserId).ToList();

               

                    totalRecords = list.Count;
                    if (sidx.Trim() != string.Empty)
                    {
                        if (sord.ToString() == "asc")
                        {
                            list = list.OrderBy(x => x.Person_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                        }
                        else
                        {
                            list = list.OrderByDescending(x => x.Proficiency).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                        }
                    }
                    else
                    {
                        list = list.OrderBy(x => x.Application_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                    }

                    var result = list.Select(Details => new
                    {
                        Details.RCTRC_CONTACT_APPL_ID,
                        Details.Person_NAME,
                        Details.Application_NAME,
                        Details.Proficiency

                    }).ToArray();

                    return result.Select(Details => new
                    {
                        cell = new[]{
                    Details.Person_NAME.ToString(),
                    Details.Application_NAME.ToString(),
                    Details.Proficiency.ToString(),
                  // "<a href='#' title='Click here to delete Details' class='ui-icon ui-icon-trash ui-align-center' onClick=DeleteRCTRCRegistrationDetails('" + URLEncrypt.EncryptParameters1(new string[]{"RCTRCCode="+ Details.RCTRC_Contact_Id.ToString().Trim()}) +"'); return false;'>Delete</a>"
                    // URLEncrypt.EncryptParameters1(new string[]{"RCTRCCode =" + Details.RCTRC_Contact_Id.ToString().Trim()}),
                }
                    }).ToArray();

                }



            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "RctrcDAL.GetRCTRCApplicationListDAL");
                totalRecords = 0;
                return null;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }
        #endregion

        #region Key Area

        public bool AddKeyAreaDAL(RCTRCKeyAreasOfWork rctrcViewModel, ref string message, FormCollection formCollection)
        {

            try
            {
                using (TransactionScope transactionScope = new TransactionScope())
                {
                    try
                    {
                        dbContext = new PMGSYEntities();


                        if (dbContext.RCTRC_CONTACT_WORK_AREA.Where(m => m.RCTRC_Contact_Id == rctrcViewModel.ContactPersonID).Any())
                        {
                            message = "Key Areas Details are already added for this Person.";
                            return false;
                        }

                        rctrcViewModel.KeyAreaList = dbContext.RCTRC_MASTER_WORK_AREA.ToList<RCTRC_MASTER_WORK_AREA>();
                        string ItemID_PE = string.Empty;
                        string ItemID_PW = string.Empty;
                        string ItemID_PY = string.Empty;

                        string ItemValue_PE = string.Empty;
                        string ItemValue_PW = string.Empty;
                        string ItemValue_PY = string.Empty;

                        foreach (var item in rctrcViewModel.KeyAreaList)
                        {
                            RCTRC_CONTACT_WORK_AREA master = new RCTRC_CONTACT_WORK_AREA();
                            ItemID_PE = "PE" + item.RCTRC_WORK_AREA_ID;
                            ItemID_PW = "PW" + item.RCTRC_WORK_AREA_ID;
                            ItemID_PY = "PY" + item.RCTRC_WORK_AREA_ID;
                            // In case of Checkbox, Name property is used to fetch value of CheckBox
                            ItemValue_PE = formCollection[ItemID_PE];
                            ItemValue_PW = formCollection[ItemID_PW];
                            ItemValue_PY = formCollection[ItemID_PY];

                            master.RCTRC_CONTACT_WORK_AREA_ID = dbContext.RCTRC_CONTACT_WORK_AREA.Max(cp => (Int32?)cp.RCTRC_CONTACT_WORK_AREA_ID) == null ? 1 : (Int32)dbContext.RCTRC_CONTACT_WORK_AREA.Max(cp => (Int32?)cp.RCTRC_CONTACT_WORK_AREA_ID) + 1;
                            master.RCTRC_Contact_Id = rctrcViewModel.ContactPersonID;
                            master.RCTRC_WORK_AREA_ID = item.RCTRC_WORK_AREA_ID;
                            master.RCTRC_CONTACT_WORK_PASTEXP = (ItemValue_PE == null ? "N" : "Y");
                            master.RCTRC_CONTACT_WORK_PRESWORK = (ItemValue_PW == null ? "N" : "Y");
                            master.RCTRC_CONTACT_WORK_PKANNXT5YR = (ItemValue_PY == null ? "N" : "Y");
                            master.RCTRC_CONTACT_WORK_ENTRY_DATE = System.DateTime.Now;
                            master.USERID = PMGSYSession.Current.UserId;
                            dbContext.RCTRC_CONTACT_WORK_AREA.Add(master);
                            dbContext.SaveChanges();
                        }
                        transactionScope.Complete();
                        return true;
                    }

                    catch (TransactionException ex)
                    {
                        transactionScope.Dispose();
                        return false;
                    }
                }
            }
            catch (OptimisticConcurrencyException ex)
            {
                ErrorLog.LogError(ex, "RctrcDAL.AddKeyAreaDAL");
                return false;
            }
            catch (UpdateException ex)
            {
                ErrorLog.LogError(ex, "RctrcDAL.AddKeyAreaDAL");
                return false;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "RctrcDAL.AddKeyAreaDAL");
                return false;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }

        }

        public Array GetRCTRCKeyAreaListDAL(int page, int rows, string sidx, string sord, out long totalRecords, int PersonCode)
        {
            try
            {
                dbContext = new PMGSYEntities();

                if (PersonCode == 0 || PersonCode == -1)
                {
                    var list1 = dbContext.USP_GET_RCTRC_WORK_AREA_LIST_PER_PERSON().ToList();
                    var list = list1.Where(m => m.USERID == PMGSYSession.Current.UserId).ToList();

                    totalRecords = list.Count;
                    if (sidx.Trim() != string.Empty)
                    {
                        if (sord.ToString() == "asc")
                        {
                            list = list.OrderBy(x => x.Person_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                        }
                        else
                        {
                            list = list.OrderByDescending(x => x.Work_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                        }
                    }
                    else
                    {
                        list = list.OrderBy(x => x.Past).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                    }

                    var result = list.Select(Details => new
                    {
                        Details.Person_NAME,
                        Details.Work_NAME,
                        Details.Past,
                        Details.Present,
                        Details.Plans

                    }).ToArray();

                    return result.Select(Details => new
                    {
                        cell = new[]{
                    Details.Person_NAME.ToString(),
                    Details.Work_NAME.ToString(),
                    Details.Past.ToString(),
                    Details.Present.ToString(),
                    Details.Plans.ToString()
                  // "<a href='#' title='Click here to delete Details' class='ui-icon ui-icon-trash ui-align-center' onClick=DeleteRCTRCRegistrationDetails('" + URLEncrypt.EncryptParameters1(new string[]{"RCTRCCode="+ Details.RCTRC_Contact_Id.ToString().Trim()}) +"'); return false;'>Delete</a>"
                    // URLEncrypt.EncryptParameters1(new string[]{"RCTRCCode =" + Details.RCTRC_Contact_Id.ToString().Trim()}),
                }
                    }).ToArray();
                }
                else
                {


                    var list = dbContext.USP_GET_RCTRC_WORK_AREA_LIST_PER_PERSON().Where(m => m.RCTRC_Contact_Id == PersonCode && m.USERID == PMGSYSession.Current.UserId).ToList();

                    
                    totalRecords = list.Count;
                    if (sidx.Trim() != string.Empty)
                    {
                        if (sord.ToString() == "asc")
                        {
                            list = list.OrderBy(x => x.Person_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                        }
                        else
                        {
                            list = list.OrderByDescending(x => x.Work_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                        }
                    }
                    else
                    {
                        list = list.OrderBy(x => x.Past).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                    }

                    var result = list.Select(Details => new
                    {
                        Details.Person_NAME,
                        Details.Work_NAME,
                        Details.Past,
                        Details.Present,
                        Details.Plans

                    }).ToArray();

                    return result.Select(Details => new
                    {
                        cell = new[]{
                    Details.Person_NAME.ToString(),
                    Details.Work_NAME.ToString(),
                    Details.Past.ToString(),
                    Details.Present.ToString(),
                    Details.Plans.ToString()
                  // "<a href='#' title='Click here to delete Details' class='ui-icon ui-icon-trash ui-align-center' onClick=DeleteRCTRCRegistrationDetails('" + URLEncrypt.EncryptParameters1(new string[]{"RCTRCCode="+ Details.RCTRC_Contact_Id.ToString().Trim()}) +"'); return false;'>Delete</a>"
                    // URLEncrypt.EncryptParameters1(new string[]{"RCTRCCode =" + Details.RCTRC_Contact_Id.ToString().Trim()}),
                }
                    }).ToArray();
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "RctrcDAL.GetRCTRCKeyAreaListDAL");
                totalRecords = 0;
                return null;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }
        #endregion

        #region Subdetails
        public bool AddSubdetailsDAL(RCTRCSubdetailsPerMaster rctrcViewModel, ref string message, FormCollection formCollection)
        {

            try
            {
                using (TransactionScope transactionScope = new TransactionScope())
                {
                    try
                    {
                        dbContext = new PMGSYEntities();
                        rctrcViewModel.KeyAreaList = dbContext.RCTRC_MASTER_KEY_AREA.Where(m => m.RCTRC_KEY_AREA_PARENT_ID == rctrcViewModel.masterWorkAreaCode).ToList();
                        if (rctrcViewModel.KeyAreaList.Count == 0)
                        {
                            message = "Key Areas of Work (Subdetails) are Not Available for Selected Master Area Work.";
                            return false;
                        }

                        if (dbContext.RCTRC_CONTACT_KEY_AREA.Where(m => m.RCTRC_Contact_Id == rctrcViewModel.ContactPersonID && m.RCTRC_KEY_AREA_PARENT_ID == rctrcViewModel.masterWorkAreaCode).Any())
                        {
                            message = "Key Areas of Work (Subdetails) are already added for this Person.";
                            return false;
                        }
                        string ItemID = string.Empty;
                        string ItemValue = string.Empty;
                        string CheckboxID = string.Empty;
                        string CheckboxValue = string.Empty;
                        foreach (var item in rctrcViewModel.KeyAreaList)
                        {
                            RCTRC_CONTACT_KEY_AREA master = new RCTRC_CONTACT_KEY_AREA();

                            // Get Radiobox Value
                            ItemID = "item" + item.RCTRC_KEY_AREA_ID;
                            ItemValue = formCollection[ItemID];
                            // Get Checkbox Value : In case of Checkbox, Name property is used to fetch value of CheckBox
                            CheckboxID = "Checkbox" + item.RCTRC_KEY_AREA_ID;
                            CheckboxValue = formCollection[CheckboxID];
                            master.RCTRC_CONTACT_KEY_AREA_ID = dbContext.RCTRC_CONTACT_KEY_AREA.Max(cp => (Int32?)cp.RCTRC_CONTACT_KEY_AREA_ID) == null ? 1 : (Int32)dbContext.RCTRC_CONTACT_KEY_AREA.Max(cp => (Int32?)cp.RCTRC_CONTACT_KEY_AREA_ID) + 1;
                            master.RCTRC_Contact_Id = rctrcViewModel.ContactPersonID;
                            master.RCTRC_KEY_AREA_ID = item.RCTRC_KEY_AREA_ID;
                            master.RCTRC_KEY_AREA_PARENT_ID = rctrcViewModel.masterWorkAreaCode;
                            master.RCTRC_CONTACT_KEY_AREA_GRADE = ItemValue;
                            master.RCTRC_CONTACT_KEY_AREA_ABILITY = (CheckboxValue == null ? "N" : "Y");
                            master.RCTRC_CONTACT_KEY_AREA_ENTRY_DATE = System.DateTime.Now;
                            master.USERID = PMGSYSession.Current.UserId;
                            dbContext.RCTRC_CONTACT_KEY_AREA.Add(master);
                            dbContext.SaveChanges();
                        }
                        transactionScope.Complete();
                        return true;
                    }

                    catch (TransactionException ex)
                    {
                        transactionScope.Dispose();
                        return false;
                    }
                }
            }
            catch (OptimisticConcurrencyException ex)
            {
                ErrorLog.LogError(ex, "RctrcDAL.AddSubdetailsDAL");
                return false;
            }
            catch (UpdateException ex)
            {
                ErrorLog.LogError(ex, "RctrcDAL.AddSubdetailsDAL");
                return false;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "RctrcDAL.AddSubdetailsDAL");
                return false;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }

        }

        public Array GetSubdetailsListDAL(int page, int rows, string sidx, string sord, out long totalRecords, int PersonCode)
        {
            try
            {
                dbContext = new PMGSYEntities();

                if (PersonCode == 0 || PersonCode == -1)
                {
                    var list1 = dbContext.USP_GET_RCTRC_KEY_AREA_LIST_PER_PERSON().ToList();
                    var list = list1.Where(m => m.USERID == PMGSYSession.Current.UserId).ToList();

                    totalRecords = list.Count;
                    if (sidx.Trim() != string.Empty)
                    {
                        if (sord.ToString() == "asc")
                        {
                            list = list.OrderBy(x => x.Person_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                        }
                        else
                        {
                            list = list.OrderByDescending(x => x.MasterElement_Name).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                        }
                    }
                    else
                    {
                        list = list.OrderBy(x => x.SubElement_Name).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                    }

                    var result = list.Select(Details => new
                    {
                        Details.Person_NAME,
                        Details.MasterElement_Name,
                        Details.SubElement_Name,
                        Details.Grade,
                        Details.Ability

                    }).ToArray();

                    return result.Select(Details => new
                    {
                        cell = new[]{
                    Details.Person_NAME.ToString(),
                    Details.MasterElement_Name.ToString(),
                    Details.SubElement_Name.ToString(),
                    Details.Grade.ToString(),
                    Details.Ability.ToString()
                  // "<a href='#' title='Click here to delete Details' class='ui-icon ui-icon-trash ui-align-center' onClick=DeleteRCTRCRegistrationDetails('" + URLEncrypt.EncryptParameters1(new string[]{"RCTRCCode="+ Details.RCTRC_Contact_Id.ToString().Trim()}) +"'); return false;'>Delete</a>"
                    // URLEncrypt.EncryptParameters1(new string[]{"RCTRCCode =" + Details.RCTRC_Contact_Id.ToString().Trim()}),
                }
                    }).ToArray();
                }
                else
                {

                    var list = dbContext.USP_GET_RCTRC_KEY_AREA_LIST_PER_PERSON().Where(m => m.RCTRC_Contact_Id == PersonCode && m.USERID == PMGSYSession.Current.UserId).ToList();
                  

                    totalRecords = list.Count;
                    if (sidx.Trim() != string.Empty)
                    {
                        if (sord.ToString() == "asc")
                        {
                            list = list.OrderBy(x => x.Person_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                        }
                        else
                        {
                            list = list.OrderByDescending(x => x.MasterElement_Name).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                        }
                    }
                    else
                    {
                        list = list.OrderBy(x => x.SubElement_Name).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                    }

                    var result = list.Select(Details => new
                    {
                        Details.Person_NAME,
                        Details.MasterElement_Name,
                        Details.SubElement_Name,
                        Details.Grade,
                        Details.Ability

                    }).ToArray();

                    return result.Select(Details => new
                    {
                        cell = new[]{
                    Details.Person_NAME.ToString(),
                    Details.MasterElement_Name.ToString(),
                    Details.SubElement_Name.ToString(),
                    Details.Grade.ToString(),
                    Details.Ability.ToString()
                  // "<a href='#' title='Click here to delete Details' class='ui-icon ui-icon-trash ui-align-center' onClick=DeleteRCTRCRegistrationDetails('" + URLEncrypt.EncryptParameters1(new string[]{"RCTRCCode="+ Details.RCTRC_Contact_Id.ToString().Trim()}) +"'); return false;'>Delete</a>"
                    // URLEncrypt.EncryptParameters1(new string[]{"RCTRCCode =" + Details.RCTRC_Contact_Id.ToString().Trim()}),
                }
                    }).ToArray();
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "RctrcDAL.GetSubdetailsListDAL");
                totalRecords = 0;
                return null;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }
        #endregion

        #region Performance

        public bool AddPerformanceDAL(RCTRCPerformance rctrcViewModel, ref string message, FormCollection formCollection)
        {

            try
            {
                using (TransactionScope transactionScope = new TransactionScope())
                {
                    try
                    {
                        dbContext = new PMGSYEntities();
                        rctrcViewModel.MASTER_PERFORMANCE_LIST = dbContext.RCTRC_MASTER_PERFORMANCE.ToList();
                        if (rctrcViewModel.MASTER_PERFORMANCE_LIST.Count == 0)
                        {
                            message = "List of Causes of perceived discrepancies in Performance are Not Available.";
                            return false;
                        }

                        if (dbContext.RCTRC_CONTACT_PERFORMANCE.Where(m => m.RCTRC_Contact_Id == rctrcViewModel.ContactPersonID).Any())
                        {
                            message = "Causes of perceived discrepancies in Performance are already added for this Person.";
                            return false;
                        }
                        string ItemID = string.Empty;
                        string ItemValue = string.Empty;

                        foreach (var item in rctrcViewModel.MASTER_PERFORMANCE_LIST)
                        {
                            RCTRC_CONTACT_PERFORMANCE master = new RCTRC_CONTACT_PERFORMANCE();

                            // Get Radiobox Value
                            ItemID = "item" + item.RCTRC_PERF_ID;
                            ItemValue = formCollection[ItemID];
                            master.RCTRC_CONTACT_PERF_ID = dbContext.RCTRC_CONTACT_PERFORMANCE.Max(cp => (Int32?)cp.RCTRC_CONTACT_PERF_ID) == null ? 1 : (Int32)dbContext.RCTRC_CONTACT_PERFORMANCE.Max(cp => (Int32?)cp.RCTRC_CONTACT_PERF_ID) + 1;
                            master.RCTRC_Contact_Id = rctrcViewModel.ContactPersonID;
                            master.RCTRC_PERF_ID = item.RCTRC_PERF_ID;
                            master.RCTRC_CONTACT_PREF_STATUS = ItemValue;
                            master.RCTRC_CONTACT_PERF_ENTRY_DATE = System.DateTime.Now;
                            master.USERID = PMGSYSession.Current.UserId;
                            dbContext.RCTRC_CONTACT_PERFORMANCE.Add(master);
                            dbContext.SaveChanges();
                        }
                        transactionScope.Complete();
                        return true;
                    }

                    catch (TransactionException ex)
                    {
                        transactionScope.Dispose();
                        return false;
                    }
                }
            }
            catch (OptimisticConcurrencyException ex)
            {
                ErrorLog.LogError(ex, "RctrcDAL.AddPerformanceDAL");
                return false;
            }
            catch (UpdateException ex)
            {
                ErrorLog.LogError(ex, "RctrcDAL.AddPerformanceDAL");
                return false;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "RctrcDAL.AddPerformanceDAL");
                return false;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }

        }

        public Array GetPerformanceListDAL(int page, int rows, string sidx, string sord, out long totalRecords, int PersonCode)
        {
            try
            {
                dbContext = new PMGSYEntities();
                if (PersonCode == 0 || PersonCode == -1)
                {
                    var list1 = dbContext.USP_GET_RCTRC_PERFORMACE_LIST_PER_PERSON().ToList();
                    var list = list1.Where(m => m.USERID == PMGSYSession.Current.UserId).ToList();

                    totalRecords = list.Count;
                    if (sidx.Trim() != string.Empty)
                    {
                        if (sord.ToString() == "asc")
                        {
                            list = list.OrderBy(x => x.Contact_Person).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                        }
                        else
                        {
                            list = list.OrderByDescending(x => x.Performance_Cause).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                        }
                    }
                    else
                    {
                        list = list.OrderBy(x => x.Status_Cause).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                    }

                    var result = list.Select(Details => new
                    {
                        Details.Contact_Person,
                        Details.Performance_Cause,
                        Details.Status_Cause


                    }).ToArray();

                    return result.Select(Details => new
                    {
                        cell = new[]{
                    Details.Contact_Person.ToString(),
                    Details.Performance_Cause.ToString(),
                    Details.Status_Cause.ToString()
                   
                  // "<a href='#' title='Click here to delete Details' class='ui-icon ui-icon-trash ui-align-center' onClick=DeleteRCTRCRegistrationDetails('" + URLEncrypt.EncryptParameters1(new string[]{"RCTRCCode="+ Details.RCTRC_Contact_Id.ToString().Trim()}) +"'); return false;'>Delete</a>"
                    // URLEncrypt.EncryptParameters1(new string[]{"RCTRCCode =" + Details.RCTRC_Contact_Id.ToString().Trim()}),
                }
                    }).ToArray();
                }
                else 
                {
                    var list = dbContext.USP_GET_RCTRC_PERFORMACE_LIST_PER_PERSON().Where(m => m.RCTRC_Contact_Id == PersonCode && m.USERID == PMGSYSession.Current.UserId).ToList();
                   

                    totalRecords = list.Count;
                    if (sidx.Trim() != string.Empty)
                    {
                        if (sord.ToString() == "asc")
                        {
                            list = list.OrderBy(x => x.Contact_Person).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                        }
                        else
                        {
                            list = list.OrderByDescending(x => x.Performance_Cause).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                        }
                    }
                    else
                    {
                        list = list.OrderBy(x => x.Status_Cause).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                    }

                    var result = list.Select(Details => new
                    {
                        Details.Contact_Person,
                        Details.Performance_Cause,
                        Details.Status_Cause


                    }).ToArray();

                    return result.Select(Details => new
                    {
                        cell = new[]{
                    Details.Contact_Person.ToString(),
                    Details.Performance_Cause.ToString(),
                    Details.Status_Cause.ToString()
                   
                  // "<a href='#' title='Click here to delete Details' class='ui-icon ui-icon-trash ui-align-center' onClick=DeleteRCTRCRegistrationDetails('" + URLEncrypt.EncryptParameters1(new string[]{"RCTRCCode="+ Details.RCTRC_Contact_Id.ToString().Trim()}) +"'); return false;'>Delete</a>"
                    // URLEncrypt.EncryptParameters1(new string[]{"RCTRCCode =" + Details.RCTRC_Contact_Id.ToString().Trim()}),
                }
                    }).ToArray();
                }

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "RctrcDAL.GetPerformanceListDAL");
                totalRecords = 0;
                return null;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }
        #endregion

        #region Suggestion
        public bool AddAddSuggestionDAL(RCTRCSuggestion rctrcViewModel, ref string message, FormCollection formCollection)
        {
            try
            {
                using (TransactionScope transactionScope = new TransactionScope())
                {
                    try
                    {
                        dbContext = new PMGSYEntities();
                        if (dbContext.RCTRC_CONTACT_SUGGESTION.Where(m => m.RCTRC_Contact_Id == rctrcViewModel.ContactPersonID).Any())
                        {
                            message = "Suggestion Details are already added for this Person.";
                            return false;
                        }
                        rctrcViewModel.KeyAreaList = dbContext.RCTRC_MASTER_SUGGESTION.ToList<RCTRC_MASTER_SUGGESTION>();
                        string ItemID = string.Empty;
                        string ItemValue = string.Empty;
                        foreach (var item in rctrcViewModel.KeyAreaList)
                        {
                            RCTRC_CONTACT_SUGGESTION master = new RCTRC_CONTACT_SUGGESTION();
                            ItemID = "item" + item.RCTRC_SUGG_ID;
                            // In case of Checkbox, Name property is used to fetch value of CheckBox
                            ItemValue = formCollection[ItemID];
                            master.RCTRC_CONTACT_SUGG_ID = dbContext.RCTRC_CONTACT_SUGGESTION.Max(cp => (Int32?)cp.RCTRC_CONTACT_SUGG_ID) == null ? 1 : (Int32)dbContext.RCTRC_CONTACT_SUGGESTION.Max(cp => (Int32?)cp.RCTRC_CONTACT_SUGG_ID) + 1;
                            master.RCTRC_Contact_Id = rctrcViewModel.ContactPersonID;
                            master.RCTRC_SUGG_ID = item.RCTRC_SUGG_ID;
                            master.RCTRC_CONTACT_SUGG_STATUS = (ItemValue == null ? "N" : "Y");
                            master.RCTRC_CONTACT_SUGG_ENTRY_DATE = System.DateTime.Now;
                            master.USERID = PMGSYSession.Current.UserId;
                            dbContext.RCTRC_CONTACT_SUGGESTION.Add(master);
                            dbContext.SaveChanges();
                        }
                        transactionScope.Complete();
                        return true;
                    }

                    catch (TransactionException ex)
                    {
                        transactionScope.Dispose();
                        return false;
                    }
                }
            }
            catch (OptimisticConcurrencyException ex)
            {
                ErrorLog.LogError(ex, "RctrcDAL.AddAddSuggestionDAL");
                return false;
            }
            catch (UpdateException ex)
            {
                ErrorLog.LogError(ex, "RctrcDAL.AddAddSuggestionDAL");
                return false;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "RctrcDAL.AddAddSuggestionDAL");
                return false;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }

        }

        public Array GetRCTRCSuggestionListDAL(int page, int rows, string sidx, string sord, out long totalRecords, int PersonCode)
        {
            try
            {
                dbContext = new PMGSYEntities();
                if (PersonCode == 0 || PersonCode == -1)
                {
                    var list1 = dbContext.USP_GET_RCTRC_SUGGESTION_LIST_PER_PERSON().ToList();
                    var list = list1.Where(m => m.USERID == PMGSYSession.Current.UserId).ToList();

                    totalRecords = list.Count;
                    if (sidx.Trim() != string.Empty)
                    {
                        if (sord.ToString() == "asc")
                        {
                            list = list.OrderBy(x => x.Person_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                        }
                        else
                        {
                            list = list.OrderByDescending(x => x.Suggestion_Name).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                        }
                    }
                    else
                    {
                        list = list.OrderBy(x => x.Status_Suggestion).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                    }

                    var result = list.Select(Details => new
                    {
                        Details.Person_NAME,
                        Details.Suggestion_Name,
                        Details.Status_Suggestion

                    }).ToArray();

                    return result.Select(Details => new
                    {
                        cell = new[]{
                    Details.Person_NAME.ToString(),
                    Details.Suggestion_Name.ToString(),
                    Details.Status_Suggestion.ToString()
                  // "<a href='#' title='Click here to delete Details' class='ui-icon ui-icon-trash ui-align-center' onClick=DeleteRCTRCRegistrationDetails('" + URLEncrypt.EncryptParameters1(new string[]{"RCTRCCode="+ Details.RCTRC_Contact_Id.ToString().Trim()}) +"'); return false;'>Delete</a>"
                    // URLEncrypt.EncryptParameters1(new string[]{"RCTRCCode =" + Details.RCTRC_Contact_Id.ToString().Trim()}),
                }
                    }).ToArray();
                }
                else {

                    var list = dbContext.USP_GET_RCTRC_SUGGESTION_LIST_PER_PERSON().Where(m => m.RCTRC_Contact_Id == PersonCode && m.USERID==PMGSYSession.Current.UserId).ToList();

                   
                    totalRecords = list.Count;
                    if (sidx.Trim() != string.Empty)
                    {
                        if (sord.ToString() == "asc")
                        {
                            list = list.OrderBy(x => x.Person_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                        }
                        else
                        {
                            list = list.OrderByDescending(x => x.Suggestion_Name).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                        }
                    }
                    else
                    {
                        list = list.OrderBy(x => x.Status_Suggestion).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                    }

                    var result = list.Select(Details => new
                    {
                        Details.Person_NAME,
                        Details.Suggestion_Name,
                        Details.Status_Suggestion

                    }).ToArray();

                    return result.Select(Details => new
                    {
                        cell = new[]{
                    Details.Person_NAME.ToString(),
                    Details.Suggestion_Name.ToString(),
                    Details.Status_Suggestion.ToString()
                  // "<a href='#' title='Click here to delete Details' class='ui-icon ui-icon-trash ui-align-center' onClick=DeleteRCTRCRegistrationDetails('" + URLEncrypt.EncryptParameters1(new string[]{"RCTRCCode="+ Details.RCTRC_Contact_Id.ToString().Trim()}) +"'); return false;'>Delete</a>"
                    // URLEncrypt.EncryptParameters1(new string[]{"RCTRCCode =" + Details.RCTRC_Contact_Id.ToString().Trim()}),
                }
                    }).ToArray();
                
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "RctrcDAL.GetRCTRCSuggestionListDAL");
                totalRecords = 0;
                return null;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }
        #endregion

        #region Training Required
        public bool AddTrainingRequiredDAL(RCTRCTrainingRequired rctrcViewModel, ref string message, FormCollection formCollection)
        {

            try
            {
                using (TransactionScope transactionScope = new TransactionScope())
                {
                    try
                    {
                        dbContext = new PMGSYEntities();
                        if (dbContext.RCTRC_CONTACT_TRG_REQ.Where(m => m.RCTRC_Contact_Id == rctrcViewModel.ContactPersonID).Any())
                        {
                            message = "Required Training details are already added for this Person.";
                            return false;
                        }

                        rctrcViewModel.KeyAreaList = dbContext.RCTRC_MASTER_TRAINING.ToList<RCTRC_MASTER_TRAINING>();
                        string ItemID_RPD = string.Empty;
                        string ItemID_EYC = string.Empty;
                        string ItemID_FAI = string.Empty;

                        string ItemValue_RPD = string.Empty;
                        string ItemValue_EYC = string.Empty;
                        string ItemValue_FAI = string.Empty;

                        foreach (var item in rctrcViewModel.KeyAreaList)
                        {
                            RCTRC_CONTACT_TRG_REQ master = new RCTRC_CONTACT_TRG_REQ();
                            ItemID_RPD = "isRPD" + item.RCTRC_TRAINING_ID;
                            ItemID_EYC = "isEYC" + item.RCTRC_TRAINING_ID;
                            ItemID_FAI = "isFAI" + item.RCTRC_TRAINING_ID;
                            // In case of Checkbox, Name property is used to fetch value of CheckBox
                            ItemValue_RPD = formCollection[ItemID_RPD];
                            ItemValue_EYC = formCollection[ItemID_EYC];
                            ItemValue_FAI = formCollection[ItemID_FAI];

                            master.RCTRC_CONTACT_TRG_REQ_ID = dbContext.RCTRC_CONTACT_TRG_REQ.Max(cp => (Int32?)cp.RCTRC_CONTACT_TRG_REQ_ID) == null ? 1 : (Int32)dbContext.RCTRC_CONTACT_TRG_REQ.Max(cp => (Int32?)cp.RCTRC_CONTACT_TRG_REQ_ID) + 1;
                            master.RCTRC_Contact_Id = rctrcViewModel.ContactPersonID;
                            master.RCTRC_TRAINING_ID = item.RCTRC_TRAINING_ID;
                            master.RCTRC_TRG_RPD = (ItemValue_RPD == null ? "N" : "Y");
                            master.RCTRC_TRG_EYC = (ItemValue_EYC == null ? "N" : "Y");
                            master.RCTRC_TRG_FAI = (ItemValue_FAI == null ? "N" : "Y");
                            master.RCTRC_TRG_ENTRY_DATE = System.DateTime.Now;
                            master.USERID = PMGSYSession.Current.UserId;
                            dbContext.RCTRC_CONTACT_TRG_REQ.Add(master);
                            dbContext.SaveChanges();
                        }
                        transactionScope.Complete();
                        return true;
                    }

                    catch (TransactionException ex)
                    {
                        transactionScope.Dispose();
                        return false;
                    }
                }
            }
            catch (OptimisticConcurrencyException ex)
            {
                ErrorLog.LogError(ex, "RctrcDAL.AddTrainingRequiredDAL");
                return false;
            }
            catch (UpdateException ex)
            {
                ErrorLog.LogError(ex, "RctrcDAL.AddTrainingRequiredDAL");
                return false;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "RctrcDAL.AddTrainingRequiredDAL");
                return false;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }

        }

        public Array GetRCTRCTrainingRequiredListDAL(int page, int rows, string sidx, string sord, out long totalRecords, int PersonCode)
        {
            try
            {
                dbContext = new PMGSYEntities();

                if (PersonCode == 0 || PersonCode == -1)
                {
                    var list1 = dbContext.USP_GET_RCTRC_REQUIRED_TRAINING_LIST_PER_PERSON().ToList();
                    var list = list1.Where(m => m.USERID == PMGSYSession.Current.UserId).ToList();

                    totalRecords = list.Count;
                    if (sidx.Trim() != string.Empty)
                    {
                        if (sord.ToString() == "asc")
                        {
                            list = list.OrderBy(x => x.Person_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                        }
                        else
                        {
                            list = list.OrderByDescending(x => x.KeyArea_Name).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                        }
                    }
                    else
                    {
                        list = list.OrderBy(x => x.RPD).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                    }

                    var result = list.Select(Details => new
                    {
                        Details.Person_NAME,
                        Details.KeyArea_Name,
                        Details.RPD,
                        Details.EYC,
                        Details.FAI

                    }).ToArray();

                    return result.Select(Details => new
                    {
                        cell = new[]{
                    Details.Person_NAME.ToString(),
                    Details.KeyArea_Name.ToString(),
                    Details.RPD.ToString(),
                    Details.EYC.ToString(),
                    Details.FAI.ToString()
                  // "<a href='#' title='Click here to delete Details' class='ui-icon ui-icon-trash ui-align-center' onClick=DeleteRCTRCRegistrationDetails('" + URLEncrypt.EncryptParameters1(new string[]{"RCTRCCode="+ Details.RCTRC_Contact_Id.ToString().Trim()}) +"'); return false;'>Delete</a>"
                    // URLEncrypt.EncryptParameters1(new string[]{"RCTRCCode =" + Details.RCTRC_Contact_Id.ToString().Trim()}),
                }
                    }).ToArray();
                }
                else {

                    var list = dbContext.USP_GET_RCTRC_REQUIRED_TRAINING_LIST_PER_PERSON().Where(m => m.RCTRC_Contact_Id == PersonCode && m.USERID == PMGSYSession.Current.UserId).ToList();
              
                    totalRecords = list.Count;
                    if (sidx.Trim() != string.Empty)
                    {
                        if (sord.ToString() == "asc")
                        {
                            list = list.OrderBy(x => x.Person_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                        }
                        else
                        {
                            list = list.OrderByDescending(x => x.KeyArea_Name).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                        }
                    }
                    else
                    {
                        list = list.OrderBy(x => x.RPD).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                    }

                    var result = list.Select(Details => new
                    {
                        Details.Person_NAME,
                        Details.KeyArea_Name,
                        Details.RPD,
                        Details.EYC,
                        Details.FAI

                    }).ToArray();

                    return result.Select(Details => new
                    {
                        cell = new[]{
                    Details.Person_NAME.ToString(),
                    Details.KeyArea_Name.ToString(),
                    Details.RPD.ToString(),
                    Details.EYC.ToString(),
                    Details.FAI.ToString()
                  // "<a href='#' title='Click here to delete Details' class='ui-icon ui-icon-trash ui-align-center' onClick=DeleteRCTRCRegistrationDetails('" + URLEncrypt.EncryptParameters1(new string[]{"RCTRCCode="+ Details.RCTRC_Contact_Id.ToString().Trim()}) +"'); return false;'>Delete</a>"
                    // URLEncrypt.EncryptParameters1(new string[]{"RCTRCCode =" + Details.RCTRC_Contact_Id.ToString().Trim()}),
                }
                    }).ToArray();
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "RctrcDAL.GetRCTRCTrainingRequiredListDAL");
                totalRecords = 0;
                return null;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }
        #endregion

        #region Monitors

        public List<MASTER_STATE> GetAllStateNames()
        {
            try
            {
                dbContext = new PMGSYEntities();
                return (from state in dbContext.MASTER_STATE.Where(m => m.MAST_STATE_ACTIVE == "Y") orderby state.MAST_STATE_NAME select state).ToList<MASTER_STATE>();
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);

                return null;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }

        public List<MASTER_DISTRICT> GetAllDistrictByStateCode(int stateCode)
        {
            try
            {
                dbContext = new PMGSYEntities();
                List<MASTER_DISTRICT> districtList = dbContext.MASTER_DISTRICT.Where(d => d.MAST_STATE_CODE == stateCode && d.MAST_DISTRICT_ACTIVE == "Y").OrderBy(d => d.MAST_DISTRICT_NAME).ToList<MASTER_DISTRICT>();
                //districtList.Insert(0, new MASTER_DISTRICT() {MAST_DISTRICT_CODE= 0, MAST_DISTRICT_NAME = "--Select--" });
                return districtList;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);

                return null;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }


        public List<SelectListItem> GetQmTypes()
        {
            List<SelectListItem> qualityMonitorTypelist = new List<SelectListItem>();

            qualityMonitorTypelist.Add(
                new SelectListItem()
                {
                    Text = "All Types",
                    Value = ""
                }
                );

            qualityMonitorTypelist.Add(
                new SelectListItem()
                {
                    Text = "NQM",
                    Value = "I"
                }
                );
            qualityMonitorTypelist.Add(
             new SelectListItem()
             {
                 Text = "SQM",
                 Value = "S"
             }
             );
            return qualityMonitorTypelist;
        }


        public bool AddMasterQualityMonitor(MasterAdminQualityMonitorViewModel masterQualityMonitorViewModel, ref string message)
        {
            try
            {
                CommonFunctions objCommon = new CommonFunctions();
                dbContext = new PMGSYEntities();
                RCTRC_ADMIN_QUALITY_MONITORS qualityMonitorsModel = new RCTRC_ADMIN_QUALITY_MONITORS();


                qualityMonitorsModel.ADMIN_QM_CODE = dbContext.RCTRC_ADMIN_QUALITY_MONITORS.Max(cp => (Int32?)cp.ADMIN_QM_CODE) == null ? 1 : (Int32)dbContext.RCTRC_ADMIN_QUALITY_MONITORS.Max(cp => (Int32?)cp.ADMIN_QM_CODE) + 1;
                qualityMonitorsModel.ADMIN_QM_TYPE = "S";
                qualityMonitorsModel.MAST_STATE_CODE = masterQualityMonitorViewModel.MAST_STATE_CODE_ADDR;

                qualityMonitorsModel.ADMIN_QM_FNAME = masterQualityMonitorViewModel.ADMIN_QM_FNAME;
                qualityMonitorsModel.ADMIN_QM_MNAME = masterQualityMonitorViewModel.ADMIN_QM_MNAME == string.Empty ? null : masterQualityMonitorViewModel.ADMIN_QM_MNAME;
                qualityMonitorsModel.ADMIN_QM_LNAME = masterQualityMonitorViewModel.ADMIN_QM_LNAME == string.Empty ? null : masterQualityMonitorViewModel.ADMIN_QM_LNAME;
                 
                qualityMonitorsModel.ADMIN_QM_DESG = (int)masterQualityMonitorViewModel.ADMIN_QM_DESG;
                qualityMonitorsModel.ADMIN_QM_ADDRESS1 = masterQualityMonitorViewModel.ADMIN_QM_ADDRESS1 == string.Empty ? null : masterQualityMonitorViewModel.ADMIN_QM_ADDRESS1;
                qualityMonitorsModel.ADMIN_QM_ADDRESS2 = masterQualityMonitorViewModel.ADMIN_QM_ADDRESS2 == string.Empty ? null : masterQualityMonitorViewModel.ADMIN_QM_ADDRESS2;

                qualityMonitorsModel.MAST_DISTRICT_CODE = masterQualityMonitorViewModel.MAST_DISTRICT_CODE == 0 ? null : masterQualityMonitorViewModel.MAST_DISTRICT_CODE;
                qualityMonitorsModel.MAST_STATE_CODE_ADDR = masterQualityMonitorViewModel.MAST_STATE_CODE_ADDR == 0 ? null : masterQualityMonitorViewModel.MAST_STATE_CODE_ADDR;
                qualityMonitorsModel.ADMIN_QM_PIN = masterQualityMonitorViewModel.ADMIN_QM_PIN == string.Empty ? null : masterQualityMonitorViewModel.ADMIN_QM_PIN;

                qualityMonitorsModel.ADMIN_QM_STD1 = masterQualityMonitorViewModel.ADMIN_QM_STD1 == string.Empty ? null : masterQualityMonitorViewModel.ADMIN_QM_STD1;
                qualityMonitorsModel.ADMIN_QM_STD2 = masterQualityMonitorViewModel.ADMIN_QM_STD2 == string.Empty ? null : masterQualityMonitorViewModel.ADMIN_QM_STD2;
                qualityMonitorsModel.ADMIN_QM_PHONE1 = masterQualityMonitorViewModel.ADMIN_QM_PHONE1 == string.Empty ? null : masterQualityMonitorViewModel.ADMIN_QM_PHONE1;
                qualityMonitorsModel.ADMIN_QM_PHONE2 = masterQualityMonitorViewModel.ADMIN_QM_PHONE2 == string.Empty ? null : masterQualityMonitorViewModel.ADMIN_QM_PHONE2;

                qualityMonitorsModel.ADMIN_QM_FAX = masterQualityMonitorViewModel.ADMIN_QM_FAX == string.Empty ? null : masterQualityMonitorViewModel.ADMIN_QM_FAX;
                qualityMonitorsModel.ADMIN_QM_STD_FAX = masterQualityMonitorViewModel.ADMIN_QM_STD_FAX == string.Empty ? null : masterQualityMonitorViewModel.ADMIN_QM_STD_FAX;
                qualityMonitorsModel.ADMIN_QM_MOBILE1 = masterQualityMonitorViewModel.ADMIN_QM_MOBILE1 == string.Empty ? null : masterQualityMonitorViewModel.ADMIN_QM_MOBILE1;
                qualityMonitorsModel.ADMIN_QM_MOBILE2 = masterQualityMonitorViewModel.ADMIN_QM_MOBILE2 == string.Empty ? null : masterQualityMonitorViewModel.ADMIN_QM_MOBILE2;

                qualityMonitorsModel.ADMIN_QM_EMAIL = masterQualityMonitorViewModel.ADMIN_QM_EMAIL == string.Empty ? null : masterQualityMonitorViewModel.ADMIN_QM_EMAIL;
                qualityMonitorsModel.ADMIN_QM_PAN = null;//masterQualityMonitorViewModel.ADMIN_QM_PAN == string.Empty ? null : masterQualityMonitorViewModel.ADMIN_QM_PAN;
                qualityMonitorsModel.ADMIN_QM_PAN_FILE = null;
                qualityMonitorsModel.ADMIN_QM_EMPANELLED = "Y";//masterQualityMonitorViewModel.ADMIN_QM_EMPANELLED;
                qualityMonitorsModel.ADMIN_QM_EMPANELLED_YEAR = null; //masterQualityMonitorViewModel.ADMIN_QM_EMPANELLED_YEAR == 0 ? null : masterQualityMonitorViewModel.ADMIN_QM_EMPANELLED_YEAR;
                qualityMonitorsModel.ADMIN_QM_IMAGE = null;//masterQualityMonitorViewModel.ADMIN_QM_IMAGE == string.Empty ? null : masterQualityMonitorViewModel.ADMIN_QM_IMAGE;
                qualityMonitorsModel.ADMIN_QM_DOCPATH = null; //masterQualityMonitorViewModel.ADMIN_QM_DOCPATH == string.Empty ? null : masterQualityMonitorViewModel.ADMIN_QM_DOCPATH;
                qualityMonitorsModel.ADMIN_QM_AADHAR_NO = null; //masterQualityMonitorViewModel.ADMIN_QM_AADHAR_NO == string.Empty ? null : masterQualityMonitorViewModel.ADMIN_QM_AADHAR_NO;
                qualityMonitorsModel.ADMIN_QM_REMARKS = null;//masterQualityMonitorViewModel.ADMIN_QM_REMARKS == string.Empty ? null : masterQualityMonitorViewModel.ADMIN_QM_REMARKS;
                qualityMonitorsModel.ADMIN_USER_ID = null;//  can be mapped later
                qualityMonitorsModel.USERID = PMGSYSession.Current.UserId;
                qualityMonitorsModel.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
               
                
                qualityMonitorsModel.OLD_ADMIN_QM_CODE = null;
                qualityMonitorsModel.ADMIN_QM_SERVICE_TYPE = null;
                qualityMonitorsModel.ADMIN_QM_BIRTH_DATE = null;
                qualityMonitorsModel.ADMIN_QM_EMPANELLED_MONTH = null;
                qualityMonitorsModel.ADMIN_QM_DEEMPANEL_REMARKS = null;
                qualityMonitorsModel.ADMIN_QM_DEPARTMENT = null;


                dbContext.RCTRC_ADMIN_QUALITY_MONITORS.Add(qualityMonitorsModel);
                dbContext.SaveChanges();
                
                return true;
                
            }

            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "RctrcDAL.AddMasterQualityMonitor");
                return false;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }

        }


        public List<RCTRC_MASTER_DESIGNATION> GetAllQmDesignation()
        {
            try
            {
                dbContext = new PMGSYEntities();
                List<RCTRC_MASTER_DESIGNATION> list = dbContext.RCTRC_MASTER_DESIGNATION.Where(m => m.MAST_DESIG_TYPE.ToUpper() == "QC").OrderBy(s => s.MAST_DESIG_NAME).ToList<RCTRC_MASTER_DESIGNATION>();
                return list;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "RctrcDAL.GetAllQmDesignation");
                return null;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }

        public Array ListQualityMonitor(string qmTypeName, int stateCode, int districtCode, string isEmpanelled, string filters, int? page, int? rows, string sidx, string sord, out long totalRecords)
        {
            try
            {
                JavaScriptSerializer js = null;
                SearchJson test = new SearchJson();
                string firstName = string.Empty;
                if (filters != null)
                {
                    js = new JavaScriptSerializer();
                    test = js.Deserialize<SearchJson>(filters);

                    foreach (rules item in test.rules)
                    {
                        switch (item.field)
                        {
                            case "ADMIN_QM_FNAME": firstName = item.data;
                                break;
                            default:
                                break;
                        }
                    }
                }

                qmTypeName = qmTypeName.Replace("%", "");
                dbContext = new PMGSYEntities();
                var lstQualityMonitorDetails = (from qualityMonitor in dbContext.RCTRC_ADMIN_QUALITY_MONITORS
                                                join state in dbContext.MASTER_STATE
                                                on (qualityMonitor.MAST_STATE_CODE.Equals(null) ? qualityMonitor.MAST_STATE_CODE_ADDR : qualityMonitor.MAST_STATE_CODE) equals state.MAST_STATE_CODE into outerState
                                                from state in outerState.DefaultIfEmpty()
                                                join district in dbContext.MASTER_DISTRICT
                                                on qualityMonitor.MAST_DISTRICT_CODE equals district.MAST_DISTRICT_CODE into outerDistrict
                                                from district in outerDistrict.DefaultIfEmpty()
                                                join designation in dbContext.RCTRC_MASTER_DESIGNATION
                                                on qualityMonitor.ADMIN_QM_DESG equals designation.MAST_DESIG_CODE into outerDesignation
                                                from designation in outerDesignation.DefaultIfEmpty()
                                                where (
                                                    //((firstName == string.Empty ? "%" : qualityMonitor.ADMIN_QM_FNAME.ToUpper()).Contains(firstName == string.Empty ? "%" : firstName.ToUpper())) &&
                                                    //((qmTypeName == string.Empty ? "%" : qualityMonitor.ADMIN_QM_TYPE.ToUpper()).StartsWith(qmTypeName == string.Empty ? "%" : qmTypeName.ToUpper()))

                                               ((firstName == string.Empty ? "%" : qualityMonitor.ADMIN_QM_FNAME.ToUpper()).Contains(firstName == string.Empty ? "%" : firstName.ToUpper()) ||
                                                (firstName == string.Empty ? "%" : qualityMonitor.ADMIN_QM_MNAME.ToUpper()).Contains(firstName == string.Empty ? "%" : firstName.ToUpper()) ||
                                                (firstName == string.Empty ? "%" : qualityMonitor.ADMIN_QM_LNAME.ToUpper()).Contains(firstName == string.Empty ? "%" : firstName.ToUpper())) &&

                                                ((qmTypeName == string.Empty ? "%" : qualityMonitor.ADMIN_QM_TYPE.ToUpper()).StartsWith(qmTypeName == string.Empty ? "%" : qmTypeName.ToUpper()))

                                                &&
                                                 ((stateCode == 0 ? 1 : (stateCode == 90 ? 1 : (stateCode == 91 ? 1 : state.MAST_STATE_CODE))) == (stateCode == 0 ? 1 : (stateCode == 90 ? 1 : (stateCode == 91 ? 1 : stateCode)))) // Added by deendayal on 28/7/2017

                                                &&
                                                ((districtCode == 0 ? 1 : district.MAST_DISTRICT_CODE) == (districtCode == 0 ? 1 : districtCode))
                                                &&
                                                (qualityMonitor.ADMIN_QM_EMPANELLED == isEmpanelled)   // by pradip for filtering Blocked QM // && qualityMonitor.ADMIN_QM_EMPANELLED != "B"
                                                )
                                                &&
                                                  (stateCode == 90 ? qualityMonitor.ADMIN_QM_SERVICE_TYPE.Equals("A") : (stateCode == 91 ? qualityMonitor.ADMIN_QM_SERVICE_TYPE.Equals("C") : (stateCode != 0 ? (qualityMonitor.ADMIN_QM_SERVICE_TYPE.Equals("S") || qualityMonitor.ADMIN_QM_SERVICE_TYPE.Equals("D") || string.IsNullOrEmpty(qualityMonitor.ADMIN_QM_SERVICE_TYPE)) : true)))// Modified on 6/11/2017 to show all NQM list for all Service types(A,c,s,d,NULL)
                                                select new
                                                {
                                                    stateName = qualityMonitor.ADMIN_QM_SERVICE_TYPE.Equals("C") ? "Cental Govt." : (qualityMonitor.ADMIN_QM_SERVICE_TYPE.Equals("A") ? "Central Agency" : state.MAST_STATE_NAME),// Added by deendayal on 28/7/2017
                                                    district.MAST_DISTRICT_NAME,
                                                    designation.MAST_DESIG_NAME,
                                                    qualityMonitor.ADMIN_QM_CODE,
                                                    qualityMonitor.ADMIN_QM_EMPANELLED_MONTH,

                                                    qualityMonitor.ADMIN_QM_FNAME,
                                                    qualityMonitor.ADMIN_QM_MNAME,
                                                    qualityMonitor.ADMIN_QM_LNAME,
                                                    qualityMonitor.ADMIN_QM_ADDRESS1,
                                                    qualityMonitor.ADMIN_QM_ADDRESS2,
                                                    qualityMonitor.ADMIN_QM_PIN,
                                                    qualityMonitor.ADMIN_QM_STD1,
                                                    qualityMonitor.ADMIN_QM_STD2,
                                                    qualityMonitor.ADMIN_QM_PHONE1,
                                                    qualityMonitor.ADMIN_QM_PHONE2,
                                                    qualityMonitor.ADMIN_QM_STD_FAX,
                                                    qualityMonitor.ADMIN_QM_FAX,
                                                    qualityMonitor.ADMIN_QM_MOBILE1,
                                                    qualityMonitor.ADMIN_QM_MOBILE2,
                                                    qualityMonitor.ADMIN_QM_EMAIL,
                                                    qualityMonitor.ADMIN_QM_PAN,
                                                    qualityMonitor.ADMIN_QM_PAN_FILE,
                                                    qualityMonitor.ADMIN_QM_EMPANELLED,
                                                    qualityMonitor.ADMIN_QM_EMPANELLED_YEAR,
                                                    qualityMonitor.ADMIN_QM_REMARKS,
                                                    qualityMonitor.ADMIN_QM_TYPE,
                                                    qualityMonitor.ADMIN_QM_IMAGE,
                                                    qualityMonitor.ADMIN_USER_ID,
                                                    qualityMonitor.ADMIN_QM_DOCPATH,//Added By Abhishek 27-June-2014
                                                    qualityMonitor.ADMIN_QM_BIRTH_DATE,//Added by Anand Singh on May 19, 2015
                                                    qualityMonitor.ADMIN_QM_AADHAR_NO  //Added by Anand Singh on May 19, 2015
                                                }).ToList();
                totalRecords = lstQualityMonitorDetails.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        switch (sidx)
                        {
                            case "stateName":
                                lstQualityMonitorDetails = lstQualityMonitorDetails.OrderBy(x => x.stateName).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;

                            case "MAST_DISTRICT_CODE":
                                lstQualityMonitorDetails = lstQualityMonitorDetails.OrderBy(x => x.MAST_DISTRICT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;

                            case "ADMIN_QM_FNAME":
                                lstQualityMonitorDetails = lstQualityMonitorDetails.OrderBy(x => x.ADMIN_QM_FNAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;

                            case "Address":
                                lstQualityMonitorDetails = lstQualityMonitorDetails.OrderBy(x => x.ADMIN_QM_ADDRESS1).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;


                            case "ADMIN_QM_DESG":
                                lstQualityMonitorDetails = lstQualityMonitorDetails.OrderBy(x => x.MAST_DESIG_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "ADMIN_QM_PIN":
                                lstQualityMonitorDetails = lstQualityMonitorDetails.OrderBy(x => x.ADMIN_QM_PIN).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;

                            case "ADMIN_QM_PHONE1":
                                lstQualityMonitorDetails = lstQualityMonitorDetails.OrderBy(x => x.ADMIN_QM_STD1).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;

                            case "ADMIN_QM_PHONE2":
                                lstQualityMonitorDetails = lstQualityMonitorDetails.OrderBy(x => x.ADMIN_QM_STD2).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;


                            case "ADMIN_QM_FAX":
                                lstQualityMonitorDetails = lstQualityMonitorDetails.OrderBy(x => x.ADMIN_QM_STD_FAX).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;

                            case "ADMIN_QM_MOBILE1":
                                lstQualityMonitorDetails = lstQualityMonitorDetails.OrderBy(x => x.ADMIN_QM_MOBILE1).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;

                            case "ADMIN_QM_MOBILE2":
                                lstQualityMonitorDetails = lstQualityMonitorDetails.OrderBy(x => x.ADMIN_QM_MOBILE2).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "ADMIN_QM_EMAIL":
                                lstQualityMonitorDetails = lstQualityMonitorDetails.OrderBy(x => x.ADMIN_QM_EMAIL).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "PAN":
                                lstQualityMonitorDetails = lstQualityMonitorDetails.OrderBy(x => x.ADMIN_QM_PAN).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;

                            case "ADMIN_QM_EMPANELLED":
                                lstQualityMonitorDetails = lstQualityMonitorDetails.OrderBy(x => x.ADMIN_QM_EMPANELLED).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;

                            case "ADMIN_QM_EMPANELLED_YEAR":
                                lstQualityMonitorDetails = lstQualityMonitorDetails.OrderBy(x => x.ADMIN_QM_EMPANELLED_YEAR).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;

                            case "Remarks":
                                lstQualityMonitorDetails = lstQualityMonitorDetails.OrderBy(x => x.ADMIN_QM_EMPANELLED_YEAR).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;

                            case "ADMIN_QM_TYPE":
                                lstQualityMonitorDetails = lstQualityMonitorDetails.OrderBy(x => x.ADMIN_QM_TYPE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;

                            default:
                                lstQualityMonitorDetails = lstQualityMonitorDetails.OrderBy(x => x.stateName).ThenBy(x => x.ADMIN_QM_FNAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                        }
                    }
                    else
                    {

                        switch (sidx)
                        {
                            case "stateName":
                                lstQualityMonitorDetails = lstQualityMonitorDetails.OrderByDescending(x => x.stateName).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;

                            case "MAST_DISTRICT_CODE":
                                lstQualityMonitorDetails = lstQualityMonitorDetails.OrderByDescending(x => x.MAST_DISTRICT_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;

                            case "ADMIN_QM_FNAME":
                                lstQualityMonitorDetails = lstQualityMonitorDetails.OrderByDescending(x => x.ADMIN_QM_FNAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;

                            case "Address":
                                lstQualityMonitorDetails = lstQualityMonitorDetails.OrderByDescending(x => x.ADMIN_QM_ADDRESS1).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;


                            case "ADMIN_QM_DESG":
                                lstQualityMonitorDetails = lstQualityMonitorDetails.OrderByDescending(x => x.MAST_DESIG_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "ADMIN_QM_PIN":
                                lstQualityMonitorDetails = lstQualityMonitorDetails.OrderByDescending(x => x.ADMIN_QM_PIN).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;

                            case "ADMIN_QM_PHONE1":
                                lstQualityMonitorDetails = lstQualityMonitorDetails.OrderByDescending(x => x.ADMIN_QM_STD1).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;

                            case "ADMIN_QM_PHONE2":
                                lstQualityMonitorDetails = lstQualityMonitorDetails.OrderByDescending(x => x.ADMIN_QM_STD2).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;


                            case "ADMIN_QM_FAX":
                                lstQualityMonitorDetails = lstQualityMonitorDetails.OrderByDescending(x => x.ADMIN_QM_STD_FAX).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;

                            case "ADMIN_QM_MOBILE1":
                                lstQualityMonitorDetails = lstQualityMonitorDetails.OrderByDescending(x => x.ADMIN_QM_MOBILE1).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;

                            case "ADMIN_QM_MOBILE2":
                                lstQualityMonitorDetails = lstQualityMonitorDetails.OrderByDescending(x => x.ADMIN_QM_MOBILE2).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "ADMIN_QM_EMAIL":
                                lstQualityMonitorDetails = lstQualityMonitorDetails.OrderByDescending(x => x.ADMIN_QM_EMAIL).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                            case "PAN":
                                lstQualityMonitorDetails = lstQualityMonitorDetails.OrderByDescending(x => x.ADMIN_QM_PAN).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;

                            case "ADMIN_QM_EMPANELLED":
                                lstQualityMonitorDetails = lstQualityMonitorDetails.OrderByDescending(x => x.ADMIN_QM_EMPANELLED).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;

                            case "ADMIN_QM_EMPANELLED_YEAR":
                                lstQualityMonitorDetails = lstQualityMonitorDetails.OrderByDescending(x => x.ADMIN_QM_EMPANELLED_YEAR).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;

                            case "Remarks":
                                lstQualityMonitorDetails = lstQualityMonitorDetails.OrderByDescending(x => x.ADMIN_QM_EMPANELLED_YEAR).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;

                            case "ADMIN_QM_TYPE":
                                lstQualityMonitorDetails = lstQualityMonitorDetails.OrderByDescending(x => x.ADMIN_QM_TYPE).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();
                                break;
                        }
                    }
                }
                else
                {
                    lstQualityMonitorDetails = lstQualityMonitorDetails.OrderBy(x => x.stateName).ThenBy(x => x.ADMIN_QM_FNAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows)).ToList();

                }

                //Added By Abhishek to show image files
                string VirtualDirectoryUrl = Path.Combine(ConfigurationManager.AppSettings["QUALITY_MONITOR_FILE_UPLOAD_VIRTUAL_DIR_PATH"], "thumbnails");
                string PhysicalPath = ConfigurationManager.AppSettings["QUALITY_MONITOR_FILE_UPLOAD"];

                CommonFunctions commonFunction = new CommonFunctions();

                return lstQualityMonitorDetails.Select(item => new
                {

                    cell = new[]{
                                item.ADMIN_QM_IMAGE==null?"":Path.Combine(VirtualDirectoryUrl, item.ADMIN_QM_IMAGE.ToString()).ToString().Replace(@"\\",@"//").Replace(@"\",@"/"),
                                item.ADMIN_QM_FNAME+" "+item.ADMIN_QM_MNAME+" "+item.ADMIN_QM_LNAME==null?"":item.ADMIN_QM_FNAME+" "+item.ADMIN_QM_MNAME+" "+item.ADMIN_QM_LNAME,
                                item.stateName==null?"-": item.stateName.Trim(),
                                
                                item.MAST_DESIG_NAME==null?"-":item.MAST_DESIG_NAME,
                               // ((item.ADMIN_QM_ADDRESS1+""+item.ADMIN_QM_ADDRESS2)==string.Empty?"-":"Address:-"+(item.ADMIN_QM_ADDRESS1+" "+item.ADMIN_QM_ADDRESS2+",  "))+(item.ADMIN_QM_STD1==null?string.Empty:"Phone1:-"+item.ADMIN_QM_STD1.ToString().Trim()+"-")+(item.ADMIN_QM_PHONE1==null?"-":(item.ADMIN_QM_PHONE1.ToString().Trim())+",  ")+(item.ADMIN_QM_STD2==null?string.Empty:"Phone2:-"+(item.ADMIN_QM_STD2.ToString().Trim())+"-")+(item.ADMIN_QM_PHONE2==null?string.Empty:(item.ADMIN_QM_PHONE2.ToString().Trim())+",  ")+(item.ADMIN_QM_STD_FAX==null?string.Empty:"Fax:-"+(item.ADMIN_QM_STD_FAX.ToString().Trim())+"-")+(item.ADMIN_QM_FAX==null?string.Empty:(item.ADMIN_QM_FAX.ToString().Trim())+",  ")+(item.ADMIN_QM_MOBILE1==null?string.Empty:"Mobile1:-"+(item.ADMIN_QM_MOBILE1.Trim())+",  ")+(item.ADMIN_QM_MOBILE2==null?string.Empty:"Mobile2:-"+ (item.ADMIN_QM_MOBILE2.ToString().Trim())+",  ")+(item.ADMIN_QM_EMAIL==null?string.Empty:"Email:-"+(item.ADMIN_QM_EMAIL.ToString().Trim()+",  ")+(item.MAST_DISTRICT_NAME==null?string.Empty:"District:-"+(item.MAST_DISTRICT_NAME.ToString())+",  ")+(item.ADMIN_QM_PIN==null?string.Empty:"PIN Code:-"+(item.ADMIN_QM_PIN.ToString()))) ,                                                          
                                ((item.ADMIN_QM_ADDRESS1+""+item.ADMIN_QM_ADDRESS2)==string.Empty?"-":"Address:-"+(item.ADMIN_QM_ADDRESS1+" "+item.ADMIN_QM_ADDRESS2+",  ")) + ((item.MAST_DISTRICT_NAME==null?string.Empty:"District:-"+(item.MAST_DISTRICT_NAME.ToString())+",  ")+(item.ADMIN_QM_PIN==null?string.Empty:"PIN Code:-"+(item.ADMIN_QM_PIN.ToString()))) ,                                                          
                                ((item.ADMIN_QM_STD1==null?string.Empty:"Phone1:-"+item.ADMIN_QM_STD1.ToString().Trim()+"-")+(item.ADMIN_QM_PHONE1==null?"-":(item.ADMIN_QM_PHONE1.ToString().Trim())+",  ")+(item.ADMIN_QM_STD2==null?string.Empty:"Phone2:-"+(item.ADMIN_QM_STD2.ToString().Trim())+"-")+(item.ADMIN_QM_PHONE2==null?string.Empty:(item.ADMIN_QM_PHONE2.ToString().Trim())+",  ")+(item.ADMIN_QM_STD_FAX==null?string.Empty:"Fax:-"+(item.ADMIN_QM_STD_FAX.ToString().Trim())+"-")+(item.ADMIN_QM_FAX==null?string.Empty:(item.ADMIN_QM_FAX.ToString().Trim())+",  ")+(item.ADMIN_QM_MOBILE1==null?string.Empty:"Mobile1:-"+(item.ADMIN_QM_MOBILE1.Trim())+",  ")+(item.ADMIN_QM_MOBILE2==null?string.Empty:"Mobile2:-"+ (item.ADMIN_QM_MOBILE2.ToString().Trim())+",  ")),
                                ((item.ADMIN_QM_EMAIL==null?string.Empty:"Email:-"+(item.ADMIN_QM_EMAIL.ToString().Trim()+",  "))),  
                                (item.ADMIN_QM_PAN==null?"-":(item.ADMIN_QM_PAN.Trim()==string.Empty?"---":item.ADMIN_QM_PAN.Trim())),

                                (item.ADMIN_QM_AADHAR_NO==null?"---":(item.ADMIN_QM_AADHAR_NO.Trim()==string.Empty?"-":item.ADMIN_QM_AADHAR_NO.Trim())),

                                (item.ADMIN_QM_BIRTH_DATE==null?"---":commonFunction.GetDateTimeToString((DateTime)item.ADMIN_QM_BIRTH_DATE)),


                                item.ADMIN_QM_PAN_FILE == null 
                                    ? "<a href='#' class='ui-icon ui-icon-plusthick ui-align-center' onclick='UploadQMPAN(\"" + item.ADMIN_QM_CODE.ToString().Trim() + "\"); return false;'>Upload</a>" 
                                    : "<a href='#' class='ui-icon ui-icon-zoomin ui-align-center' onclick='UploadQMPAN(\"" + item.ADMIN_QM_CODE.ToString().Trim() + "\"); return false;'>View</a>" ,

                              //  item.ADMIN_QM_EMPANELLED=="N"?"No":"Yes",                                
                                ( (item.ADMIN_QM_EMPANELLED_MONTH==null || item.ADMIN_QM_EMPANELLED_MONTH==0)?" ":CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(item.ADMIN_QM_EMPANELLED_MONTH.Value))+(item.ADMIN_QM_EMPANELLED_YEAR==null?"-":item.ADMIN_QM_EMPANELLED_YEAR.ToString()),                                
                                item.ADMIN_QM_REMARKS==null?"-":(item.ADMIN_QM_REMARKS.Trim()==string.Empty?"-":item.ADMIN_QM_REMARKS.Trim()),                                
                               // item.ADMIN_QM_TYPE==null?"-":(item.ADMIN_QM_TYPE.Trim()==string.Empty?"-":item.ADMIN_QM_TYPE.Trim()=="I"?"NQM":"SQM"),
                               // dbContext.UM_User_Master.Where(m=>m.UserID==item.ADMIN_USER_ID).Select(s=>s.UserName).FirstOrDefault()==null?"-":dbContext.UM_User_Master.Where(m=>m.UserID==item.ADMIN_USER_ID).Select(s=>s.UserName).FirstOrDefault(),
                                dbContext.UM_User_Master.Where(m=>m.UserID==item.ADMIN_USER_ID).Select(s=>s.UserName).FirstOrDefault()==null?                                                                                                                                         
                                                                                                                                         item.ADMIN_QM_TYPE.Trim()=="I"?
                                                                                                                                         (  dbContext.UM_User_Master.Where(m=>m.UserID==item.ADMIN_USER_ID).Select(s=>s.UserName).FirstOrDefault()==null?
                                                                                                                                         "-":  dbContext.UM_User_Master.Where(m=>m.UserID==item.ADMIN_USER_ID).Select(s=>s.UserName).FirstOrDefault())
                                                                                                                                         
                                                                                                                                         
                                                                                                                                        : item.ADMIN_QM_TYPE.Trim()=="S"?
                                                                                                                                        (item.ADMIN_QM_EMPANELLED=="Y"?
                                                                                                                                        ("<center><table><tr> <td  style='border:none'><span class='ui-icon ui-icon-circle-plus' title='Generate user id' onClick ='AddSQMUserLoginDetails(\"" + URLEncrypt.EncryptParameters1(new string[] { "QmCode="+item.ADMIN_QM_CODE.ToString().Trim()}) + "\");'></span></td></table></center>")
                                                                                                                                        :"<span class='ui-icon ui-icon-locked ui-align-center'></span>")
                                                                                                                                        :dbContext.UM_User_Master.Where(m=>m.UserID==item.ADMIN_USER_ID).Select(s=>s.UserName).FirstOrDefault()==null?
                                                                                                                                        "-":dbContext.UM_User_Master.Where(m=>m.UserID==item.ADMIN_USER_ID).Select(s=>s.UserName).FirstOrDefault()

                                                                                                                                        :dbContext.UM_User_Master.Where(m=>m.UserID==item.ADMIN_USER_ID).Select(s=>s.UserName).FirstOrDefault()==null?
                                                                                                                                         "-":dbContext.UM_User_Master.Where(m=>m.UserID==item.ADMIN_USER_ID).Select(s=>s.UserName).FirstOrDefault()
                                                                                                                                        ,
                                item.ADMIN_QM_IMAGE == null 
                                    ? "<a href='#' class='ui-icon ui-icon-plusthick ui-align-center' onclick='UploadMasterQMFile(\"" + item.ADMIN_QM_CODE.ToString().Trim() + "\"); return false;'>Upload</a>" 
                                    : "<a href='#' class='ui-icon ui-icon-zoomin ui-align-center' onclick='UploadMasterQMFile(\"" + item.ADMIN_QM_CODE.ToString().Trim() + "\"); return false;'>View</a>" ,
                                URLEncrypt.EncryptParameters1(new string[]{"QmCode="+item.ADMIN_QM_CODE.ToString().Trim()}),
                                DateTime.Now.Year*12*30+DateTime.Now.Month*30+DateTime.Now.Day-(item.ADMIN_QM_BIRTH_DATE==null?1900*12*30+1*30+1:((DateTime)item.ADMIN_QM_BIRTH_DATE).Year*12*30+((DateTime)item.ADMIN_QM_BIRTH_DATE).Month*30+((DateTime)item.ADMIN_QM_BIRTH_DATE).Day)>70*12*30?"Y":"N",
                              //  "<a href='#' class='ui-icon ui-icon-cancel ui-align-center'title='Click here to blacklist this monitor' onclick='blockQualityMonitor(\"" + URLEncrypt.EncryptParameters1(new string[]{"PAN="+item.ADMIN_QM_PAN.ToString().Trim()}) + "\"); return false;'>Block</a>" // by pradip
                            }
                }).ToArray();
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                ErrorLog.LogError(ex, "ListQualityMonitor");
                totalRecords = 0;
                return null;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }// By Anand



        public bool AddSQMUserLoginQualityMonitorDAL(int qualityMonitorCode, ref string message)
        {
            using (TransactionScope objScope = new TransactionScope())
            {
                dbContext = new PMGSYEntities();
                PMGSYEntities dbCon = new PMGSYEntities();

                UM_User_Master obj_User_Master = new UM_User_Master();
                try
                {
                    RCTRC_ADMIN_QUALITY_MONITORS qualityMonitorModel = dbCon.RCTRC_ADMIN_QUALITY_MONITORS.Where(qm => qm.ADMIN_QM_CODE == qualityMonitorCode).FirstOrDefault();
                    MasterAdminQualityMonitorViewModel qualityMonitorViewModel = null;
                    string userName = null;
                    if (qualityMonitorModel != null)
                    {

                        int stateCode = Convert.ToInt32(qualityMonitorModel.MAST_STATE_CODE_ADDR);
                        //Get User Name with ADMIN_ND_CODE,

                        string currentYear = DateTime.Now.Year.ToString();

                        if (qualityMonitorCode < 10)
                        {
                            userName = "tna" + currentYear +"00"+qualityMonitorCode;
                        }
                        else if (qualityMonitorCode >= 10 && qualityMonitorCode < 100)
                        {
                            userName = "tna" + currentYear + "0" + qualityMonitorCode;
                        }
                        else if (qualityMonitorCode >= 100 && qualityMonitorCode < 1000)
                        {
                            userName = "tna" + currentYear + qualityMonitorCode;
                        }
                        else if (qualityMonitorCode > 1000)
                        {
                            userName = "tna" + currentYear + qualityMonitorCode;
                        }



                        if (dbContext.UM_User_Master.Any(u => u.UserName == userName.Trim()))
                        {
                            message = "User with same name already exists, Please choose different User Name ";
                            return false;
                        }

                        #region Create User store in UM_USER_MASTER Table
                        obj_User_Master.UserID = ((from userid in dbContext.UM_User_Master select userid.UserID).Max()) + 1;
                        obj_User_Master.UserName = userName;
                        obj_User_Master.LevelID = 4; // State Level
                        obj_User_Master.DefaultRoleID = dbContext.UM_Role_Master.Where(m => m.RoleName == "TNAusers").Select(m => m.RoleID).FirstOrDefault(); //7;
                        obj_User_Master.CreatedBy = PMGSYSession.Current.UserId;
                        obj_User_Master.CreationDate = DateTime.Now;
                        obj_User_Master.IsActive = true;
                        obj_User_Master.IsFirstLogin = true;
                        obj_User_Master.IsLocked = false;

                        if (qualityMonitorModel.MAST_STATE_CODE_ADDR == 0)
                            obj_User_Master.Mast_State_Code = null;
                        else
                            obj_User_Master.Mast_State_Code = qualityMonitorModel.MAST_STATE_CODE_ADDR;

                        obj_User_Master.Mast_District_Code = null;
                        obj_User_Master.Admin_ND_Code = null;

                        obj_User_Master.Password = new Login().EncodePassword(obj_User_Master.UserName);
                        obj_User_Master.FailedPasswordAttempts = 0;
                        obj_User_Master.FailedPasswordAnswerAttempts = 0;
                        obj_User_Master.PreferedLanguageID = 1;
                        obj_User_Master.PreferedCssID = 1;
                        obj_User_Master.MaxConcurrentLoginsAllowed = 1;
                        obj_User_Master.Remarks = "";
                        dbContext.UM_User_Master.Add(obj_User_Master);
                        dbContext.SaveChanges();

                        short RoleIDDetails = dbContext.UM_Role_Master.Where(m => m.RoleName == "TNAusers").Select(m => m.RoleID).FirstOrDefault();

                        //Assign userRoleMapping details
                        UM_User_Role_Mapping userRoleMapping = new UM_User_Role_Mapping();
                        userRoleMapping.ID = ((from uurm in dbContext.UM_User_Role_Mapping select uurm.ID).Max()) + 1;
                        userRoleMapping.UserId = obj_User_Master.UserID;
                        userRoleMapping.RoleId = RoleIDDetails;
                        dbContext.UM_User_Role_Mapping.Add(userRoleMapping);
                        dbContext.SaveChanges();

                        //Assign Security Question Answer
                        UM_Security_Question_Answer secQuestionAnswer = new UM_Security_Question_Answer();
                        secQuestionAnswer.UserID = obj_User_Master.UserID;
                        secQuestionAnswer.PasswordQuestionID = 44;              //Default Question is What is your name?
                        secQuestionAnswer.Answer = obj_User_Master.UserName;    //Default Answer is value of UserName
                        secQuestionAnswer.SetDate = DateTime.Now;
                        secQuestionAnswer.LastUpdatedDate = DateTime.Now;
                        dbContext.UM_Security_Question_Answer.Add(secQuestionAnswer);
                        dbContext.SaveChanges();
                        //Add all entities
                        #endregion

                    }


                    #region Update in RCTRC_ADMIN_QUALITY_MONITORS Column ADMIN_USER_ID

                    qualityMonitorModel.ADMIN_USER_ID = obj_User_Master.UserID;
                    qualityMonitorModel.USERID = PMGSYSession.Current.UserId;
                    qualityMonitorModel.IPADD = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                    dbCon.Entry(qualityMonitorModel).State = System.Data.Entity.EntityState.Modified;
                    dbCon.SaveChanges();
                    #endregion
                    objScope.Complete();
                    return true;
                }
                catch (OptimisticConcurrencyException ex)
                {
                    ErrorLog.LogError(ex, "RctrcDAL.AddSQMUserLoginQualityMonitorDAL().OptimisticConcurrencyException");
                    return false;
                }
                catch (UpdateException ex)
                {
                    ErrorLog.LogError(ex, "RctrcDAL.AddSQMUserLoginQualityMonitorDAL().UpdateException");
                    return false;
                }
                catch (Exception ex)
                {
                    ErrorLog.LogError(ex, "RctrcDAL.AddSQMUserLoginQualityMonitorDAL()");
                    return false;
                }
                finally
                {
                    if (dbContext != null)
                    {
                        dbContext.Dispose();
                    }
                }
            }
        }


        public bool StateUserMapDAL(RCTRCStateUserMapping rctrcViewModel, ref string message, FormCollection formCollection)
        {

            try
            {
               using (TransactionScope transactionScope = new TransactionScope())
               {

                PMGSYEntities dbContext = new PMGSYEntities();

                //if (dbContext.RCTRC_USERID_STATE_MAPPING.Where(m => m.STATE_CODE == rctrcViewModel.StateCode && m.USER_ID == rctrcViewModel.UserID).Any())
                //{
                //    message = "Selected State and Username already mapped";
                //    return false;
                //}

                rctrcViewModel.StateList = PopulateStatesForMapping();
                        string ItemID = string.Empty;
                        string ItemValue = string.Empty;

                foreach (var item in rctrcViewModel.StateList)
                {
                    RCTRC_USERID_STATE_MAPPING master = new RCTRC_USERID_STATE_MAPPING();
                    ItemID = "item" + item.Value;
                    // In case of Checkbox, Name property is used to fetch value of CheckBox
                    ItemValue = formCollection[ItemID];

                    if (ItemValue!=null)
                    {
                        int StateCode = Convert.ToInt16(item.Value);
                        if (dbContext.RCTRC_USERID_STATE_MAPPING.Where(m => m.STATE_CODE == StateCode && m.USER_ID == rctrcViewModel.UserID).Any())
                        {
                            message = "Selected State and Username already mapped";
                            return false;
                        }
                        master.MAPPING_ID = dbContext.RCTRC_USERID_STATE_MAPPING.Max(cp => (Int32?)cp.MAPPING_ID) == null ? 1 : (Int32)dbContext.RCTRC_USERID_STATE_MAPPING.Max(cp => (Int32?)cp.MAPPING_ID) + 1;
                        master.STATE_CODE = Convert.ToInt16(item.Value);
                        master.USER_ID = rctrcViewModel.UserID;

                        dbContext.RCTRC_USERID_STATE_MAPPING.Add(master);
                        dbContext.SaveChanges();
                    }
                }
                transactionScope.Complete();
                return true;
               }
                 

            }
            catch (OptimisticConcurrencyException ex)
            {
                ErrorLog.LogError(ex, "RctrcDAL.CdRCTRCRegistrationDAL");
                return false;
            }
            catch (UpdateException ex)
            {
                ErrorLog.LogError(ex, "RctrcDAL.CdRCTRCRegistrationDAL");
                return false;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "RctrcDAL.CdRCTRCRegistrationDAL");
                return false;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }

        }


               public List<SelectListItem> PopulateStatesForMapping()
        {
            List<SelectListItem> StatesList = new List<SelectListItem>();
            SelectListItem item;


            try
            {
                dbContext = new PMGSYEntities();
                var query = (from c in dbContext.MASTER_STATE
                             where c.MAST_STATE_ACTIVE == "Y"
                             select new
                             {
                                 Text = c.MAST_STATE_NAME,
                                 Value = c.MAST_STATE_CODE
                             }).OrderBy(c => c.Text).ToList();

                foreach (var data in query)
                {
                    item = new SelectListItem();
                    item.Text = data.Text;
                    item.Value = data.Value.ToString();
                    StatesList.Add(item);
                }
                return StatesList;
            }
            catch
            {
                return StatesList;
            }
            finally
            {
                dbContext.Dispose();
            }
        }



        






        public Array GetMappedStateUserListDAL(int page, int rows, string sidx, string sord, out long totalRecords)
        {
            try
            {
                dbContext = new PMGSYEntities();
                List<RCTRC_USERID_STATE_MAPPING> list = dbContext.RCTRC_USERID_STATE_MAPPING.ToList<RCTRC_USERID_STATE_MAPPING>();
                IQueryable<RCTRC_USERID_STATE_MAPPING> query1 = list.AsQueryable<RCTRC_USERID_STATE_MAPPING>();

                IQueryable<RCTRC_USERID_STATE_MAPPING> query = query1.AsQueryable<RCTRC_USERID_STATE_MAPPING>();

                totalRecords = list.Count;
                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        query = query.OrderBy(x => x.UM_User_Master.UserName).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                    }
                    else
                    {
                        query = query.OrderByDescending(x => x.UM_User_Master.UserName).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                    }
                }
                else
                {
                    query = query.OrderBy(x => x.UM_User_Master.UserName).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt16(rows));
                }

                var result = query.Select(Details => new
                {
                    Details.UM_User_Master.UserName,
                    Details.MASTER_STATE.MAST_STATE_NAME
                  

                }).ToArray();

                return result.Select(Details => new
                {
                    cell = new[]{

                    Convert.ToString(Details.UserName),
                    Convert.ToString(Details.MAST_STATE_NAME)
         
                }
                }).ToArray();
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "RctrcDAL.GetMappedStateUserListDAL");
                totalRecords = 0;
                return null;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }
        #endregion
    }

    public interface IRctrcDAL
    {
        #region Registration
        bool CdRCTRCRegistrationDAL(RCTRCRegistrationModel cdworksViewModel, ref string message);
        Array ListRCTRCRegistrationDetals(int page, int rows, string sidx, string sord, out long totalRecords);
        string DeleteRegistrationDAL(int RegCode);
        #endregion

        #region Training
        bool AddTrainingDAL(RCTRCTraining cdworksViewModel, ref string message);
        Array GetRCTRCTrainingListDAL(int page, int rows, string sidx, string sord, out long totalRecords, int PersonCode);
        #endregion

        #region Applications
        bool AddApplicationDAL(RCTRCApplications cdworksViewModel, ref string message, FormCollection formCollection);
        Array GetRCTRCApplicationListDAL(int page, int rows, string sidx, string sord, out long totalRecords, int PersonCode);
        #endregion Applications

        #region Key Area
        bool AddKeyAreaDAL(RCTRCKeyAreasOfWork cdworksViewModel, ref string message, FormCollection formCollection);
        Array GetRCTRCKeyAreaListDAL(int page, int rows, string sidx, string sord, out long totalRecords, int PersonCode);
        #endregion

        #region Subdetails
        bool AddSubdetailsDAL(RCTRCSubdetailsPerMaster cdworksViewModel, ref string message, FormCollection formCollection);
        Array GetSubdetailsListDAL(int page, int rows, string sidx, string sord, out long totalRecords, int PersonCode);
        #endregion

        #region Performance
        bool AddPerformanceDAL(RCTRCPerformance cdworksViewModel, ref string message, FormCollection formCollection);
        Array GetPerformanceListDAL(int page, int rows, string sidx, string sord, out long totalRecords, int PersonCode);
        #endregion

        #region Suggestion
        bool AddAddSuggestionDAL(RCTRCSuggestion cdworksViewModel, ref string message, FormCollection formCollection);
        Array GetRCTRCSuggestionListDAL(int page, int rows, string sidx, string sord, out long totalRecords, int PersonCode);
        #endregion

        #region Training Required
        bool AddTrainingRequiredDAL(RCTRCTrainingRequired cdworksViewModel, ref string message, FormCollection formCollection);
        Array GetRCTRCTrainingRequiredListDAL(int page, int rows, string sidx, string sord, out long totalRecords, int PersonCode);
        #endregion

        #region Monitors
        List<MASTER_STATE> GetAllStateNames();    
        List<SelectListItem> GetQmTypes();
        List<MASTER_DISTRICT> GetAllDistrictByStateCode(int stateCode);

        bool AddMasterQualityMonitor(MasterAdminQualityMonitorViewModel masterQualityMonitorViewModel, ref string message);

        List<RCTRC_MASTER_DESIGNATION> GetAllQmDesignation();


        Array ListQualityMonitor(string qmTypeName, int stateCode, int districtCode, string isEmpanelled, string filters, int? page, int? rows, string sidx, string sord, out long totalRecords);
      



        //public bool StateUserMapDAL(RCTRCStateUserMapping rctrcViewModel, ref string message);

        //public Array GetMappedStateUserListDAL(int page, int rows, string sidx, string sord, out long totalRecords);
        #endregion

    }
}


