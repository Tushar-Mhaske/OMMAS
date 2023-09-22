using PMGSY.Models;
//using PMGSY.Models.Login;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;
using PMGSY.Extensions;
using System.Data.Entity.Core.Objects;
using PMGSY.Common;
using System.Data.Entity.Core;

namespace PMGSY.DAL
{

    public class Login
    {
        /// <summary>
        /// Check for IsUserExist
        ///           IsUserLocked
        ///           IsUserActive
        ///           Cuncurrent Logins less than Max Cunccurent logins
        ///           Password verification
        ///           IsFirstLogin
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public UserAuthModel AuthenticateUser(LoginModel model)
        {
            UserAuthModel userAuthModel = new UserAuthModel();
            string doubleEncPwdr = string.Empty;
            userAuthModel.Message = string.Empty;
            try
            {
                using (var dbContext = new PMGSYEntities())
                {

                    UM_User_Master userMaster = (from u in dbContext.UM_User_Master
                                                 where u.UserName.Equals(model.UserName.Trim())
                                                 select u).FirstOrDefault();

                    if (userMaster == null)
                    {
                        userAuthModel.Message = "The user name or password provided is incorrect.";
                        return userAuthModel;
                    }

                    if (userMaster.DefaultRoleID == 74)//Added for contractor grievance
                    {

                        CONTRACTOR_REGISTRATION_DETAILS objCont = (from cd in dbContext.CONTRACTOR_REGISTRATION_DETAILS where cd.USER_ID == userMaster.UserID select cd).FirstOrDefault();
                        userAuthModel.UserName = objCont.CON_FIRM_NAME;
                        //PMGSYSession.Current.UserName = userAuthModel.UserName; 

                    }



                    //if (userMaster == null)
                    //{
                    //    userAuthModel.Message = "The user name or password provided is incorrect.";
                    //    return userAuthModel;
                    //}

                    //Added By Abhishek kamble 25-Apr-2014 start         
                    if (userMaster.FailedPasswordAttempts >= Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["WrongPasswordAllowdCount"].ToString()))
                    {
                        //return Json(new { ShowCaptch = true }, JsonRequestBehavior.AllowGet);
                        userAuthModel.isCaptchaRequired = true;
                    }
                    else
                    {
                        userAuthModel.isCaptchaRequired = false;
                        //return Json(new { ShowCaptch = false }, JsonRequestBehavior.AllowGet);
                    }
                    //Added By Abhishek kamble 25-Apr-2014 end

                    //Check for User exists?
                    if (!(userMaster.DefaultRoleID == 74))
                    {
                        if (!userMaster.UserName.Equals(model.UserName))
                        {
                            userAuthModel.Message = "The user name or password provided is incorrect.";
                            return userAuthModel;
                        }

                    }
                    //Check for Active User
                    if (!userMaster.IsActive)
                    {
                        userAuthModel.Message = "The user is inactive. Please contact Ommas Help.";
                        return userAuthModel;
                    }

                    //Check for Is User Locked
                    if (userMaster.IsLocked)
                    {
                        userAuthModel.Message = "The user is locked. Please contact Ommas Help.";
                        return userAuthModel;
                    }





                    ////Concurrent Logins
                    ////Concurrent Login Exceeded Logic needs to implement Properly. Now it is not proper.
                    ////Cause is -- If user directly closed browser instead of Logout then it is not managable 
                    //if (userMaster.ConcurrentLoginCount > userMaster.MaxConcurrentLoginsAllowed)
                    //{
                    //    userAuthModel.Message = "Concurrent users exceeded. Please try after some time.";
                    //    return userAuthModel;
                    //}


                    doubleEncPwdr = EncodePassword(userMaster.Password + PMGSYSession.Current.SessionSalt.ToString());
                    //Use Name & Password verification (Failure)
                    if (!(userMaster.UserName.ToLower() == model.UserName.ToLower() && doubleEncPwdr == model.Password.ToUpper()))
                    {
                        //increment FailedPasswordAttempts 
                        ++(userMaster.FailedPasswordAttempts);

                        dbContext.Entry(userMaster).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();

                        //if (userMaster.FailedPasswordAttempts > 15)
                        ///Changed by SAMMED PATIL to allow configuration of lock count through web.config
                        if (userMaster.FailedPasswordAttempts > Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["LockUserCount"].ToString()))
                        {
                            userMaster.IsLocked = true;
                            dbContext.Entry(userMaster).State = System.Data.Entity.EntityState.Modified;
                            dbContext.SaveChanges();
                        }

                        int validAttempts = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["LockUserCount"].ToString()) - Convert.ToInt32(userMaster.FailedPasswordAttempts);
                        if (userAuthModel.isCaptchaRequired == true)
                        {
                            userAuthModel.Message = validAttempts < 0 ? "The user name or password provided is incorrect, user is locked" : "The user name or password provided is incorrect, user will be locked after " + (validAttempts + 1) + " invalid attempts";
                        }
                        else
                        {
                            userAuthModel.Message = "The user name or password provided is incorrect.";
                        }
                        return userAuthModel;
                    }
                    else                                       //success
                    {
                        ////Concurrent Login Exceeded Logic needs to implement Properly. Now it is not proper.
                        ////Cause is -- If user directly closed browser instead of Logout then it is not managable
                        //++(userMaster.ConcurrentLoginCount);

                        //On success, update FailedPasswordAttempts to 0 & increment ConcurrentLoginCount
                        userMaster.FailedPasswordAttempts = 0;
                        userMaster.LastLoginDate = DateTime.Now;
                        dbContext.Entry(userMaster).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();

                        if (userMaster.IsFirstLogin)
                            userAuthModel.IsFirstLogin = true;
                        else
                            userAuthModel.IsFirstLogin = false;

                        InitializeSession(userMaster, model.DefaultRoleId); //Initialize Session Variables

                        if (userMaster.DefaultRoleID == 74)//Added for contractor grievance
                        {
                            PMGSYSession.Current.UserName = userAuthModel.UserName;
                        }
                        return userAuthModel;
                    }
                }

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "LoginDAL.AuthenticateUser()");
                userAuthModel.Message = "Error occurred while login. Please try again.";
                return userAuthModel;
            }
        }


        /// <summary>
        /// Method to encode password as MD5
        /// </summary>
        /// <param name="OriginalPasswordWithSalt"></param>
        /// <returns></returns>
        public string EncodePassword(string OriginalPasswordWithSalt)
        {
            //Declarations
            Byte[] OriginalBytes;
            Byte[] EncodedBytes;
            MD5 md5;

            //Instantiate MD5CryptoServiceProvider, get bytes for original password and compute hash (encoded password)
            md5 = new MD5CryptoServiceProvider();
            OriginalBytes = ASCIIEncoding.Default.GetBytes(OriginalPasswordWithSalt);
            EncodedBytes = md5.ComputeHash(OriginalBytes);

            //Convert encoded bytes back to a 'readable' string           
            StringBuilder hashCode = new StringBuilder(32);

            foreach (byte b in EncodedBytes)
                hashCode.Append(b.ToString("x2").ToUpper());

            return hashCode.ToString();
        }


        /// <summary>
        /// Method Populates Question List for Password recovery
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetPwdrQuestionList()
        {
            try
            {
                List<SelectListItem> questionList = new List<SelectListItem>();
                using (var dbContext = new PMGSYEntities())
                {
                    questionList = dbContext.UM_Security_Question_Master.ToList().Select(x => new SelectListItem
                    {
                        Value = x.PasswordQuestionID.ToString(),
                        Text = x.PasswordQuestion
                    }).ToList<SelectListItem>();
                }

                //questionList.Insert(0, new SelectListItem { Selected = false, Text = "Select Question", Value = "0" });
                return questionList;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "LoginDAL.GetPwdrQuestionList");
                return null;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public UserAuthModel GetPwdrQuestionAnsDetails(RecoverPwdrQuestionModel model)
        {
            UserAuthModel userAuthModel = new UserAuthModel();

            try
            {
                if (model.PwdrQuestionId == 0)
                {
                    userAuthModel.IsQuestionSelected = false;
                    return userAuthModel;
                }

                using (var dbContext = new PMGSYEntities())
                {
                    userAuthModel.IsQuestionSelected = true;

                    var userMaster = (from u in dbContext.UM_User_Master
                                      where u.UserName.Equals(model.UserName)
                                      select u).FirstOrDefault();


                    var quesAnsMaster = (from u in dbContext.UM_Security_Question_Answer
                                         where u.UserID == userMaster.UserID
                                         select u).FirstOrDefault();


                    if (userMaster.UserID == 0)
                    {
                        userAuthModel.IsQuestionSelected = true;
                        userAuthModel.IsUserExist = false;
                        return userAuthModel;
                    }

                    if (Convert.ToInt32(quesAnsMaster.PasswordQuestionID) != model.PwdrQuestionId)
                    {
                        userAuthModel.IsQuestionSelected = true;
                        userAuthModel.IsUserExist = true;
                        userAuthModel.IsPwdrQuestionWrong = true;
                        return userAuthModel;
                    }

                    if (!quesAnsMaster.Answer.Trim().Equals(model.PwdrAnswer.Trim()))
                    {
                        userAuthModel.IsQuestionSelected = true;
                        userAuthModel.IsUserExist = true;
                        userAuthModel.IsPwdrQuestionWrong = false;
                        userAuthModel.IsPwdrAnswerWrong = true;
                        return userAuthModel;
                    }


                    userAuthModel.IsQuestionSelected = true;
                    userAuthModel.IsUserExist = true;
                    userAuthModel.IsPwdrQuestionWrong = false;
                    userAuthModel.IsPwdrAnswerWrong = false;

                    userAuthModel.UserId = userMaster.UserID;
                    userAuthModel.UserName = userMaster.UserName;
                    //userAuthModel.RoleId = pwdrChangeData.SAS_DEFAULT_ROLE;
                }

                return userAuthModel;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "LoginDAL.GetPwdrQuestionAnsDetails");
                userAuthModel.IsException = true;
                return userAuthModel;
            }



        }


        /// <summary>
        /// Update Password in Databse on Recover Password
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public UserAuthModel UpdatePassword(RecoverPasswordModel model)
        {
            UserAuthModel userAuthModel = new UserAuthModel();

            try
            {
                using (var dbContext = new PMGSYEntities())
                {
                    var userMaster = (from u in dbContext.UM_User_Master
                                      where u.UserID == model.UserId
                                      select u).FirstOrDefault();


                    //Is New Password same as Old Password
                    if (userMaster.Password.ToUpper().Equals(model.NewPassword.ToUpper()))
                    {
                        userAuthModel.IsOldAndNewPwdrSame = true;
                        return userAuthModel;
                    }

                    //Added By Abhishek kamble 25-Apr-2014 To Check Last few Old Passwords not same as new password start

                    var UserMasterShadow = dbContext.UM_User_Master_SHADOW.Where(m => m.UserID == model.UserId).Select(s => new { s.Password, s.LastPasswordChangedDate }).Distinct().ToList();
                    int LastOldPasswordCount = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["LastOldPasswordCount"].ToString());

                    if (UserMasterShadow.Count() >= LastOldPasswordCount)
                    {
                        UserMasterShadow = UserMasterShadow.OrderByDescending(o => o.LastPasswordChangedDate).Take(LastOldPasswordCount).ToList();

                        foreach (var UserMasterShadowModel in UserMasterShadow)
                        {
                            //Is New Password same as Old Password
                            if (UserMasterShadowModel.Password.ToUpper().Equals(model.NewPassword.ToUpper()))
                            {
                                userAuthModel.IsOldAndNewPwdrSame = true;
                                return userAuthModel;
                            }
                        }
                    }

                    //Added By Abhishek kamble 25-Apr-2014 To Check Last few Old Passwords not same as new password end



                    userAuthModel.IsOldAndNewPwdrSame = false;
                    userMaster.Password = model.NewPassword.ToUpper();
                    userMaster.LastPasswordChangedDate = DateTime.Now;
                    dbContext.Entry(userMaster).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();
                }

                return userAuthModel;

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "LoginDAL.UpdatePassword");
                userAuthModel.IsException = true;
                return userAuthModel;
            }
        }


        /// <summary>
        /// Change Password checks all validations for passwords & update new password in DB
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public UserAuthModel ChangePassword(ChangePasswordModel model)
        {
            UserAuthModel userAuthModel = new UserAuthModel();
            var dbContext = new PMGSYEntities();
            try
            {

                var userMaster = (from u in dbContext.UM_User_Master
                                  where u.UserID == model.UserId
                                  select u).FirstOrDefault();

                //Is Old Password Wrong
                if (!userMaster.Password.ToUpper().Equals(model.OldPassword.ToUpper()))
                {
                    userAuthModel.IsOldPasswordWrong = true;
                    return userAuthModel;
                }

                //Is New Password same as Old Password
                if (userMaster.Password.ToUpper().Equals(model.NewPassword.ToUpper()))
                {
                    userAuthModel.IsOldPasswordWrong = false;
                    userAuthModel.IsOldAndNewPwdrSame = true;
                    return userAuthModel;
                }


                //Added By Abhishek kamble 25-Apr-2014 To Check Last few Old Passwords not same as new password start
                var UserMasterShadow = (IEnumerable<dynamic>)null;
                if (dbContext.UM_User_Master_SHADOW.Any(m => m.UserID == model.UserId))
                {
                    UserMasterShadow = dbContext.UM_User_Master_SHADOW.Where(m => m.UserID == model.UserId).Select(s => new { s.Password, s.LastPasswordChangedDate }).Distinct().ToList();
                }
                int LastOldPasswordCount = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["LastOldPasswordCount"].ToString());

                if (UserMasterShadow.Count() >= LastOldPasswordCount)
                {
                    UserMasterShadow = UserMasterShadow.OrderByDescending(o => o.LastPasswordChangedDate).Take(LastOldPasswordCount).ToList();

                    foreach (var UserMasterShadowModel in UserMasterShadow)
                    {
                        //Is New Password same as Old Password
                        if (UserMasterShadowModel.Password.ToUpper().Equals(model.NewPassword.ToUpper()))
                        {
                            userAuthModel.IsOldAndNewPwdrSame = true;
                            return userAuthModel;
                        }
                    }
                }

                //Added By Abhishek kamble 25-Apr-2014 To Check Last few Old Passwords not same as new password end


                userAuthModel.IsOldPasswordWrong = false;
                userAuthModel.IsOldAndNewPwdrSame = false;


                //update Security Queation Answer
                UM_Security_Question_Answer secQuestionAnswer = new UM_Security_Question_Answer();
                secQuestionAnswer = dbContext.UM_Security_Question_Answer.Find(model.UserId);
                secQuestionAnswer.PasswordQuestionID = model.PwdrQuestionId;         //Default Question is What is your name?
                secQuestionAnswer.Answer = model.PwdrAnswer;                        //Default Answer is value of UserName
                secQuestionAnswer.SetDate = DateTime.Now;
                secQuestionAnswer.LastUpdatedDate = DateTime.Now;
                dbContext.Entry(secQuestionAnswer).State = System.Data.Entity.EntityState.Modified;

                //dbContext.SaveChanges();

                //Update new Password
                userMaster.Password = model.NewPassword.ToUpper();
                userMaster.IsFirstLogin = false;
                userMaster.LastPasswordChangedDate = DateTime.Now;
                dbContext.Entry(userMaster).State = System.Data.Entity.EntityState.Modified;

                dbContext.SaveChanges();

                return userAuthModel;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "LoginDAL.ChangePassword(ChangePasswordModel model)");
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                userAuthModel.IsException = true;
                return userAuthModel;
            }
            finally
            {
                dbContext.SaveChanges();
            }

        }


        /// <summary>
        /// Insert Log Details
        /// </summary>
        /// <returns></returns>
        public bool InsertLogDetails()
        {
            try
            {
                using (var dbContext = new PMGSYEntities())
                {
                    UM_User_Log userLogEntity = new UM_User_Log();

                    long maxLogId = dbContext.UM_User_Log.Max(x => (Int64?)x.LogID) ?? 0;

                    userLogEntity.LogID = ++maxLogId;
                    userLogEntity.UserID = PMGSYSession.Current.UserId;
                    userLogEntity.LevelID = Convert.ToByte(PMGSYSession.Current.LevelId);
                    userLogEntity.LoginDateTime = DateTime.Now;
                    userLogEntity.IpAddress = System.Web.HttpContext.Current.Request.UserHostAddress;
                    userLogEntity.LogDate = DateTime.Now;


                    dbContext.UM_User_Log.Add(userLogEntity);
                    dbContext.SaveChanges();

                    PMGSYSession.Current.LogId = userLogEntity.LogID;
                }

                return true;
            }
            catch (DbEntityValidationException dbEx)
            {
                ErrorLog.LogError(dbEx, "LoginDAL.InsertLogDetails.DbEntityValidationException");
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "LoginDAL.InsertLogDetails");
                return false;
            }

        }

        /// <summary>
        /// Update Log
        /// </summary>
        /// <returns></returns>
        public bool UpdateLogDetails()
        {
            try
            {
                using (var dbContext = new PMGSYEntities())
                {
                    var userMaster = (from u in dbContext.UM_User_Master
                                      where u.UserName.Equals(PMGSYSession.Current.UserName)
                                      select u).FirstOrDefault();

                    //decrement ConcurrentLoginCount
                    //--(userMaster.ConcurrentLoginCount);
                    dbContext.Entry(userMaster).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();

                    var userLog = (from u in dbContext.UM_User_Log
                                   where u.LogID == PMGSYSession.Current.LogId
                                   select u).FirstOrDefault();

                    //decrement ConcurrentLoginCount
                    userLog.LogoutDateTime = DateTime.Now;
                    dbContext.Entry(userLog).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();
                }

                return true;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "LoginDAL.UpdateLogDetails");
                return false;
            }

        }


        /// <summary>
        /// Get Home Page For particular Role
        /// </summary>
        /// <returns></returns>
        public string GetHomePageAction()
        {
            string actionHomePage = null;
            try
            {

                using (var dbContext = new PMGSYEntities())
                {
                    var actionMaster = (from uhm in dbContext.UM_HomePage_Master
                                        join uam in dbContext.UM_Action_Master on uhm.HomePageId equals uam.ActionID
                                        where uhm.RoleId == PMGSYSession.Current.RoleCode
                                        select new
                                        {
                                            uam.ControllerName,
                                            uam.ActionName
                                        }).FirstOrDefault();

                    if (actionMaster != null)
                        actionHomePage = "~/" + actionMaster.ControllerName + "/" + actionMaster.ActionName;
                    else
                        actionHomePage = "~/Home/Default/";

                    PMGSYSession.Current.HomePageURL = actionHomePage;  //set Home page action in session.

                    return actionHomePage;
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "LoginDAL.GetHomePageAction");
                throw ex;
            }
        }


        /// <summary>
        /// Get All mapped roles for particlar user
        /// </summary>
        /// <param name="UserName"></param>
        /// <returns></returns>
        public List<SelectListItem> GetUserRoleList(string UserName)
        {
            List<SelectListItem> lstRoles = new List<SelectListItem>();
            try
            {

                using (var dbContext = new PMGSYEntities())
                {

                    var query = (from uum in dbContext.UM_User_Master
                                 join uurm in dbContext.UM_User_Role_Mapping on uum.UserID equals uurm.UserId
                                 join urm in dbContext.UM_Role_Master on uurm.RoleId equals urm.RoleID
                                 where uum.UserName == UserName
                                 orderby urm.RoleName
                                 select new
                                 {
                                     Value = urm.RoleID,
                                     Text = urm.RoleName
                                 }).ToList();


                    SelectListItem item = new SelectListItem();
                    foreach (var data in query)
                    {
                        item = new SelectListItem();
                        item.Text = data.Text;
                        item.Value = data.Value.ToString();
                        lstRoles.Add(item);
                    }


                    return lstRoles;
                }
            }
            catch (EntityCommandExecutionException ex)
            {
                ErrorLog.LogError(ex, "LoginDAL.GetUserRoleList().EntityCommandExecutionException");
                return null;
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "LoginDAL.GetUserRoleList()");
                //using (System.IO.StreamWriter sw = System.IO.File.AppendText(System.IO.Path.Combine("D:\\ErrorLog\\OMMASErrorLog_" + System.DateTime.Now.Date.ToString("dd_MM_yyyy") + ".txt", "")))
                //{
                //    sw.WriteLine("Date :" + DateTime.Now.ToString());
                //    sw.WriteLine("Method : " + "DAL_Error()");
                //    if (ex != null)
                //        sw.WriteLine("Exception : " + ex.StackTrace.ToString());
                //    //if (innerException != null)
                //    //    sw.WriteLine("innerException : " + innerException.Message.ToString());
                //    //if (httpException != null)
                //    //    sw.WriteLine("httpException : " + httpException.Message.ToString());
                //    sw.WriteLine("---------------------------------------------------------------------------------------");
                //    sw.Close();
                //}

                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return null;
            }
        }



        /// <summary>
        /// Initialize all session variables
        /// </summary>
        /// <param name="userMaster"></param>
        public void InitializeSession(UM_User_Master userMaster, Int32 DefaultRoleId)
        {
            try
            {
                ////First initialize with default value (Required to Switch Role by State as PIU)
                PMGSYSession.Current.AdminNdCode = 0;
                PMGSYSession.Current.DistrictCode = 0;
                PMGSYSession.Current.DistrictName = null;
                PMGSYSession.Current.RoleCode = 0;
                PMGSYSession.Current.RoleName = null;
                PMGSYSession.Current.LevelId = 0;
                PMGSYSession.Current.MastAgencyCode = 0;

                PMGSYSession.Current.UserName = userMaster.UserName;
                PMGSYSession.Current.UserId = userMaster.UserID;
                PMGSYSession.Current.LevelId = userMaster.LevelID;
                PMGSYSession.Current.StateCode = Convert.ToInt16(userMaster.Mast_State_Code);
                PMGSYSession.Current.DistrictCode = Convert.ToInt32(userMaster.Mast_District_Code);
                PMGSYSession.Current.AdminNdCode = Convert.ToInt32(userMaster.Admin_ND_Code);
                PMGSYSession.Current.CssID = Convert.ToInt16(userMaster.PreferedCssID);
                PMGSYSession.Current.LanguageID = Convert.ToInt16(userMaster.PreferedLanguageID);

                using (var dbContext = new PMGSYEntities())
                {
                    PMGSYSession.Current.PMGSYScheme = 1; //default scheme is 1
                    if (DefaultRoleId == 0)
                    {
                        var roleMapping = (from u in dbContext.UM_User_Master
                                           join urm in dbContext.UM_Role_Master on u.DefaultRoleID equals urm.RoleID
                                           where u.UserID == userMaster.UserID
                                           select new { u.DefaultRoleID, urm.RoleName }).FirstOrDefault();

                        PMGSYSession.Current.RoleCode = Convert.ToInt16(roleMapping.DefaultRoleID);
                        PMGSYSession.Current.RoleName = roleMapping.RoleName;
                    }
                    else
                    {
                        var roleMapping = (from urm in dbContext.UM_Role_Master
                                           where urm.RoleID == DefaultRoleId
                                           select new { urm.RoleID, urm.RoleName }).FirstOrDefault();

                        PMGSYSession.Current.RoleCode = Convert.ToInt16(roleMapping.RoleID);
                        PMGSYSession.Current.RoleName = roleMapping.RoleName;
                    }
                    ///Added by SAMMED A. PATIL on 29 MAR2017 for RCPLWE PIU/SRRDA/ITNO Role
                    PMGSYSession.Current.PMGSYScheme = (PMGSYSession.Current.RoleCode == 54 || PMGSYSession.Current.RoleCode == 55 || PMGSYSession.Current.RoleCode == 56 || PMGSYSession.Current.RoleCode == 69) ? (byte)3 : PMGSYSession.Current.PMGSYScheme;

                    var languageMaster = (from u in dbContext.UM_Language_Master
                                          where u.LanguageID == userMaster.PreferedLanguageID
                                          select u).FirstOrDefault();

                    PMGSYSession.Current.Language = languageMaster.LanguageName;

                    var cssMaster = (from u in dbContext.UM_Css_Master
                                     where u.CssID == userMaster.PreferedCssID
                                     select u).FirstOrDefault();

                    PMGSYSession.Current.CssName = cssMaster.CssName;


                    var masterState = (from ms in dbContext.MASTER_STATE
                                       where ms.MAST_STATE_CODE == userMaster.Mast_State_Code
                                       select ms).FirstOrDefault();

                    if (masterState != null)
                    {
                        PMGSYSession.Current.StateName = masterState.MAST_STATE_NAME;
                        PMGSYSession.Current.StateShortCode = masterState.MAST_STATE_SHORT_CODE.Trim();
                    }

                    var masterDistrict = (from md in dbContext.MASTER_DISTRICT
                                          where md.MAST_DISTRICT_CODE == userMaster.Mast_District_Code
                                          select md).FirstOrDefault();

                    if (masterDistrict != null)
                    {
                        PMGSYSession.Current.DistrictName = masterDistrict.MAST_DISTRICT_NAME;
                    }

                    var masterDepartment = (from ad in dbContext.ADMIN_DEPARTMENT
                                            where ad.ADMIN_ND_CODE == userMaster.Admin_ND_Code
                                            select ad).FirstOrDefault();

                    if (masterDepartment != null)
                    {
                        PMGSYSession.Current.DepartmentName = masterDepartment.ADMIN_ND_NAME;
                        PMGSYSession.Current.ParentNDCode = masterDepartment.MAST_PARENT_ND_CODE;
                        PMGSYSession.Current.MastAgencyCode = masterDepartment.MAST_AGENCY_CODE;
                        PMGSYSession.Current.MastAgencyName = masterDepartment.MASTER_AGENCY.MAST_AGENCY_NAME;
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "LoginDAL.InitializeSession");
                PMGSYSession.Current.UserId = 0;
            }
        }


        /// <summary>
        /// Authorize Role, i.e for Session Hijacking
        /// </summary>
        /// <param name="objAuthorizationModel"></param>
        /// <returns></returns>
        public bool Authorize(AuthorizationModel objAuthorizationModel)
        {
            var dbContext = new PMGSYEntities();
            try
            {

                //----------------  Check Role of user --------------

                var userDetails = (from userData in dbContext.UM_User_Master
                                   join userRoleData in dbContext.UM_User_Role_Mapping on userData.UserID equals userRoleData.UserId
                                   where userData.UserID == PMGSYSession.Current.UserId
                                   select userRoleData).ToList();

                //----------------------------------------------------------------------


                //----------------  Check is Controller & Action entered is correct --------------
                Int16 currentRole = Convert.ToInt16(PMGSYSession.Current.RoleCode);
                var actionControllerDetails = (from actionData in dbContext.UM_Role_Action_Mapping
                                               where actionData.RoleId == currentRole
                                               select actionData).ToList();
                //----------------------------------------------------------------------

                //if valid role & valid mapped Controller & Action then only return true else false
                if ((userDetails.Where(c => c.RoleId == objAuthorizationModel.RoleCode).Any()) &&
                     (actionControllerDetails.Where(c => c.Controller == objAuthorizationModel.ControllerName && c.Action == objAuthorizationModel.ActionName).Any()))
                {
                    return true;
                }
                else
                {
                    // Url Referer empty, means its directly called from Browser URL, 
                    // So don't allow such URL directly from browser
                    // OtherwiseURl is called from some internal link, so allow it.
                    if (HttpContext.Current.Request.UrlReferrer == null || HttpContext.Current.Request.UrlReferrer.ToString() == string.Empty)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }

                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "LoginDAL.Authorize");
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }


        }


        /// <summary>
        /// Set Session of particular user for Admin Login
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public string SetSessionForAdmin(int userId)
        {
            try
            {
                using (var dbContext = new PMGSYEntities())
                {
                    UM_User_Master userMaster = (from u in dbContext.UM_User_Master
                                                 where u.UserID == userId
                                                 select u).FirstOrDefault();

                    InitializeSession(userMaster, userMaster.DefaultRoleID); //Initialize Session Variables

                    return string.Empty;
                }

            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "LoginDAL.SetSessionForAdmin");
                return "Error Occurred While Processing Your Request";
            }
        }

    }
}