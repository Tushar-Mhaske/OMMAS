using PMGSY.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMatrix.WebData;
using System.Web.Security;
using System.Data.Entity;
using PMGSY.DAL;
using System.Data.Entity.Validation;
using System.Diagnostics;
using PMGSY.Extensions;
using PMGSY.BAL.Menu;
using PMGSY.Models.Login;
using PMGSY.Common;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using PMGSY.Areas.ECBriefReport.Controllers;

//using PMGSY.CustomValidators;

namespace PMGSY.Controllers
{
    [HandleError]
    //[RequiredAuthentication]
    public class LoginController : Controller
    {
        /// <summary>
        /// Get Method for Login
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        //[OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult Login()
        {
            if (PMGSYSession.Current.UserName != null)
            {
                Session.Abandon();
                regenerateId();
                Response.Redirect("/Login/Login/");
            }

            LoginModel model = new LoginModel();//Added By Abhishek kamble 24-Apr-2014
            model.ValidateCaptcha = false;
            return View(model);
        }


        [AllowAnonymous]
        [HttpPost]
        public JsonResult GetSessionSalt(string id)
        {
            try
            {
                //Request.Cookies["ASP.NET_SessionId"].Secure = true;
                regenerateId();
                //generate a random number
                //added by PP[07-05-2018]
                Random ran = new Random();
                Int64 i64 = ran.Next(10000000, 99999999);
                i64 = (i64 * 100000000) + ran.Next(0, 999999999);
                var v16 = Math.Abs(i64);
                Session["SessionSalt"] = v16;
                PMGSYSession.Current.SessionSalt = Convert.ToInt64(Session["SessionSalt"]);
                //end
                #region OLD LOGIC
                //Session["SessionSalt"] = new Random().Next(9999, 9999999);
                //PMGSYSession.Current.SessionSalt = Convert.ToInt32(Session["SessionSalt"]);
                //return new JsonResult { Data = PMGSYSession.Current.SessionSalt }; 
                #endregion
                return new JsonResult { Data = PMGSYSession.Current.SessionSalt };
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }



        public ActionResult UserLoginAttemptStatus(string UserName)
        {
            var dbContext = new PMGSYEntities();
            string doubleEncPwdr = string.Empty;
            Login login = new Login();            

            try
            {

                UM_User_Master userMasterModel = (from u in dbContext.UM_User_Master
                                                  where u.UserName.Equals(UserName.Trim())
                                                  select u).FirstOrDefault();
                if (userMasterModel == null)
                {
                    return Json(new { ShowCaptch = false }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                if (userMasterModel.FailedPasswordAttempts >= Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["WrongPasswordAllowdCount"].ToString()) )
                {
                    return Json(new { ShowCaptch=true},JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { ShowCaptch = false }, JsonRequestBehavior.AllowGet);                    
                }   
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { ShowCaptch = false }, JsonRequestBehavior.AllowGet);                    
            }
            finally {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }         
        }


        /// <summary>
        /// Authentication of User
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(string id, LoginModel model)
        {
            UserAuthModel userAuthModel = new UserAuthModel();
            var dbContext = new PMGSYEntities();
            Login login = new Login();

            try
            {
                //Added By Abhishek kamble to check user failed attempts start 24-Apr-2014
                if ((model.ValidateCaptcha == false) || (model.ValidateCaptcha == null))
                {
                    if (ModelState.ContainsKey("Captcha"))
                    {
                        ModelState["Captcha"].Errors.Clear();
                    }
                }


                if (ModelState.IsValid)
                {
                    if (PMGSYSession.Current.SessionSalt == 0)
                    {
                        Response.Cache.SetCacheability(HttpCacheability.NoCache);
                        Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
                        Response.Cache.SetNoServerCaching();
                        Response.Cache.SetNoStore();
                        return Redirect("/Login/SessionExpire");
                    }

                    if (!Session["ASP.NET_SessionId"].ToString().Equals(Request.Cookies["ASP.NET_SessionId"].Value))
                    {
                        throw new Exception("invalid session");
                    }

                    userAuthModel = login.AuthenticateUser(model);


                    //Added By Abhishek kamble 25-Apr-2014 To check is Captcha Required  start   
                    if (userAuthModel.isCaptchaRequired == true)
                    {
                        model.ShowCaptcha = true;
                        model.ValidateCaptcha = true;
                    }
                    else
                    {
                        model.ShowCaptcha = false;
                        model.ValidateCaptcha = false;
                    }

                    if (!userAuthModel.Message.Equals(string.Empty))
                    {
                        ModelState.AddModelError("", userAuthModel.Message);
                        return View(model);
                    }
                    //Added By Abhishek kamble 25-Apr-2014 To check is Captcha Required  end   

                    // Set Number of users
                    HttpContext.Application.Lock();
                    HttpContext.Application["OnlineUsers"] = Convert.ToInt32(HttpContext.Application["OnlineUsers"]) + 1;
                    HttpContext.Application.UnLock();

                    //set cookie for admin user to allow access to elmah.axd
                    FormsAuthentication.SetAuthCookie(PMGSYSession.Current.UserName, true);
                    if (userAuthModel.IsFirstLogin)
                    {
                        //redirect to Change Password Page
                        ChangePasswordModel chpModel = new ChangePasswordModel();
                        chpModel.UserId = PMGSYSession.Current.UserId;

                        chpModel.RoleId = PMGSYSession.Current.RoleCode;
                        chpModel.UserName = PMGSYSession.Current.UserName;


                        //if Already entered password question & answer
                        var PwdrQuesId = (from uup in dbContext.UM_Security_Question_Answer
                                          where uup.UserID == PMGSYSession.Current.UserId
                                          select uup.PasswordQuestionID).FirstOrDefault();
                        if (PwdrQuesId != 0)
                        {
                            chpModel.PwdrQuestionId = PwdrQuesId;
                            chpModel.PwdrAnswer = dbContext.UM_Security_Question_Answer.Where(c => c.PasswordQuestionID == PwdrQuesId && c.UserID == PMGSYSession.Current.UserId).Select(c => c.Answer).FirstOrDefault();
                        }

                        chpModel.QuestionList = new Login().GetPwdrQuestionList();
                        return RedirectToAction("ChangePassword", chpModel);
                    }
                    else
                    {
                        //For SRRDA Login as a PIU
                        if (PMGSYSession.Current.LevelId == 4 && (PMGSYSession.Current.RoleCode == 22 || PMGSYSession.Current.RoleCode == 38 || PMGSYSession.Current.RoleCode == 54))
                        {
                            return RedirectToAction("StateAsPIU");
                        }
                        else
                        {
                            return RedirectToHome();
                        }
                    }
                }
                else
                {
                    //Added By Abhishek kamble 24-Apr-2014
                    GetCaptchaImages();
                    if (model.ValidateCaptcha == true)
                    {
                        model.ShowCaptcha = true;
                    }

                    if (!string.IsNullOrEmpty(model.UserName) && !string.IsNullOrEmpty(model.Password))
                    {
                        // If we got this far, something failed, redisplay form
                        //Commented By Abhishek kamble 24-Apr-2014

                        //ModelState.AddModelError("", "The user name or password provided is incorrect.");                                                   
                    }
                    else
                    {
                        return View(model);
                    }
                }

                return View(model);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ModelState.AddModelError("", "Error occurred while login. Please try again.");
                return View(model);
            }
            finally
            {

                dbContext.Dispose();
            }
        }


        /// <summary>
        /// Get Captch Images 
        /// </summary>
        /// Added By Abhishek kamble 24-Apr-2014
        /// <returns></returns>
        public ActionResult GetCaptchaImages()
        {
            return CaptchaLib.ControllerExtensions.Captcha(this);        
        }


        /// <summary>
        /// Get Method for State as PIU Login
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Audit]
        [RequiredAuthentication]
        public ActionResult StateAsPIU(string id)
        {
            try
            {
                StateAsPIUModel stateAsPIUModel = new StateAsPIUModel();
                stateAsPIUModel.DEPARTMENT_LIST = new CommonFunctions().PopulateDPIUOfSRRDA(PMGSYSession.Current.AdminNdCode);
                stateAsPIUModel.USER_NAME = PMGSYSession.Current.UserName;
                return View("StateAsPIU", stateAsPIUModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }


        /// <summary>
        /// Post method for State as PIU
        /// </summary>
        /// <param name="stateAsPIUModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        [ValidateAntiForgeryToken]
        public ActionResult StateAsPIU(StateAsPIUModel stateAsPIUModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (var dbContext = new PMGSYEntities())
                    {
                        var userInfo = (from u in dbContext.ADMIN_DEPARTMENT
                                        where u.ADMIN_ND_CODE == stateAsPIUModel.ADMIN_ND_CODE
                                        select u).FirstOrDefault();


                        PMGSYSession.Current.AdminNdCode = stateAsPIUModel.ADMIN_ND_CODE;
                        PMGSYSession.Current.DepartmentName = userInfo.ADMIN_ND_NAME;
                        PMGSYSession.Current.DistrictCode = Convert.ToInt32(userInfo.MAST_DISTRICT_CODE == null ? 0 : userInfo.MAST_DISTRICT_CODE);
                        PMGSYSession.Current.DistrictName = (from districtMaster in dbContext.MASTER_DISTRICT where districtMaster.MAST_DISTRICT_CODE == PMGSYSession.Current.DistrictCode select districtMaster.MAST_DISTRICT_NAME).FirstOrDefault();
                        PMGSYSession.Current.LevelId = 5; //District Level
                        //PMGSYSession.Current.RoleCode = 22; // Role as Data Entry
                        PMGSYSession.Current.RoleName = (from roleMaster in dbContext.UM_Role_Master where roleMaster.RoleID == PMGSYSession.Current.RoleCode select roleMaster.RoleName).First();
                    }

                    return RedirectToHome();
                }
                else
                {

                    // If we got this far, something failed, redisplay form
                    ModelState.AddModelError("", "Error Ocurred While Processing Your Request.");
                    return View(stateAsPIUModel);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Redirect("/Login/Error");
            }
        }


        /// <summary>
        /// Get method to render Partial view for Role Switch
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [RequiredAuthentication]
        public ActionResult ValidateRoles(string id)
        {
            try
            {
                LoginRoleModel loginModel = new LoginRoleModel();
                loginModel.RoleList = new Login().GetUserRoleList(id);
                return PartialView("_LoginRolePartial", loginModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Redirect("/Login/SessionExpire");
            }
        }


        /// <summary>
        /// Method to load Home Page for particular user
        /// </summary>
        /// <returns></returns>
        [RequiredAuthentication]
        public ActionResult RedirectToHome()
        {
            try
            {
                Login login = new Login();
                string actionToRedirect = login.GetHomePageAction();

                ////Insert Log
                login.InsertLogDetails();

                //changes by koustubh nakate on 19/08/2013 for role wise home screen

                int roleCode = PMGSYSession.Current.RoleCode;

                if (PMGSYSession.Current.RoleCode == 21 || PMGSYSession.Current.RoleCode == 33 || PMGSYSession.Current.RoleCode == 26 || PMGSYSession.Current.RoleCode == 66)
                {
                    TempData["roleDefaultPage"] = actionToRedirect;
                    return Redirect("~/Accounts/AccountDashBoard");
                }
                else if (roleCode == 46 || roleCode == 10)  //role code 36(itno) replaced by 46(Finance) 
                {
                    return RedirectToAction("FundTypeSelection", "Accounts");
                }
                else if (roleCode == 74)
                {

                    return RedirectToAction("GetRegisterGrievanceLayout", "ContractorGrievances", new { area = "ContractorGrievances" });
                }


                else
                {
                    return Redirect(actionToRedirect);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Redirect("/Login/SessionExpire");
            }
        }


        /// <summary>
        /// Swith Role Activity, reassigns role in session
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        [Audit]
        [RequiredAuthentication]
        public ActionResult SwitchRole()
        {
            try
            {
                LoginModel model = new LoginModel();
                model.UserName = PMGSYSession.Current.UserName;
                
                return View(model);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Redirect("/Login/SessionExpire");
            }
        }

        /// <summary>
        /// Update Log & Abondon Session
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Audit]
        //[OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult Logout()
        {
            
            try
            {
                bool IsUpdateSuccess = new Login().UpdateLogDetails();
                if (IsUpdateSuccess)
                    PMGSYSession.Current.EndSession();

                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
                //Response.Cache.SetNoServerCaching();
                Response.Cache.SetNoStore();
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                PMGSYSession.Current.EndSession();
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
                //Response.Cache.SetNoServerCaching();
                Response.Cache.SetNoStore();
                return RedirectToAction("Login");
            }
        }


        /// <summary>
        /// Recover Security Question Set By User
        /// </summary>
        /// <returns></returns>
        [Audit]
        //[RequiredAuthentication]
        public ActionResult RecoverPwdrQuestion()
        {
            List<SelectListItem> questionList = new List<SelectListItem>();
            questionList = new Login().GetPwdrQuestionList();
            ViewBag.QuestionList = questionList;
            return View("RecoverPwdrQuestion");
        }


        /// <summary>
        /// Check Is User Exists? If not, give error msg
        /// Check Password question corresponding to User, If not, give Corresponding msg
        /// Check Password Answer corresponding to Question, If not, give Corresponding msg
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        [ValidateAntiForgeryToken]
        public ActionResult RecoverPwdrQuestion(RecoverPwdrQuestionModel model)
        {
            UserAuthModel userAuthModel = new UserAuthModel();

            try
            {
                model.PwdrAnswer = DecryptAes(model.PwdrAnswer);//added by PP[08-05-2018]

                if (ModelState.IsValid)
                {
                    if (model.PwdrAnswer.Trim().Equals(""))
                    {
                        // 10.8 User Enumeration (OTG-IDENT-004), (OTG-IDENT-005) 
                        ModelState.AddModelError("", "Invalid details are entered.");
                       // ModelState.AddModelError("", "Invalid Answer.");
                        return RecoverPwdrQuestion();
                    }

                    userAuthModel = new Login().GetPwdrQuestionAnsDetails(model);

                    if (!userAuthModel.IsQuestionSelected)
                    {
                        // 10.8 User Enumeration (OTG-IDENT-004), (OTG-IDENT-005) 
                        ModelState.AddModelError("", "Invalid details are entered.");
                       // ModelState.AddModelError("", "Please select question.");
                        return RecoverPwdrQuestion();
                    }

                    if (!userAuthModel.IsUserExist)
                    {
                      //  ModelState.AddModelError("", "User not exist."); // Change is made on 08 DEC 2020. As per suggestion in Security Mail dated 07 DEC 2020 by Anita Mam to Rohit.
                        // 10.8 User Enumeration (OTG-IDENT-004), (OTG-IDENT-005) 
                        ModelState.AddModelError("", "Invalid details are entered.");
                        return RecoverPwdrQuestion();
                    }

                    if (userAuthModel.IsPwdrQuestionWrong)
                    {
                        // 10.8 User Enumeration (OTG-IDENT-004), (OTG-IDENT-005) 
                        ModelState.AddModelError("", "Invalid details are entered.");
                       // ModelState.AddModelError("", "Selected Question is wrong.");
                        return RecoverPwdrQuestion();
                    }

                    if (userAuthModel.IsPwdrAnswerWrong)
                    {
                        // 10.8 User Enumeration (OTG-IDENT-004), (OTG-IDENT-005) 
                        ModelState.AddModelError("", "Invalid details are entered.");
                       // ModelState.AddModelError("", "Provided Answer is wrong.");
                        return RecoverPwdrQuestion();
                    }

                    RecoverPasswordModel rpModel = new RecoverPasswordModel();
                    rpModel.UserId = userAuthModel.UserId;
                    rpModel.UserName = userAuthModel.UserName;
                    //rpModel.RoleId = userAuthModel.RoleId;
                    return View("RecoverPassword", rpModel);
                }
                else
                {
                    //Added By Abhishek kamble 25-Apr-2014
                    GetCaptchaImages();
                    return RecoverPwdrQuestion();
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return View("Error");
            }
        }


        /// <summary>
        /// Get Method for Recover Password
        /// </summary>
        /// <param name="umModel"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Audit]
        [RequiredAuthentication]
        public ActionResult RecoverPassword(UserMasterModel umModel, string id = "0")
        {
            RecoverPasswordModel model = new RecoverPasswordModel();
            model.UserId = umModel.UserId;
            model.UserName = umModel.UserName;
            return View("RecoverPassword", model);
        }



        /// <summary>
        /// If Password is updated successfully, Give Message and redirect to Login Page
        /// else give Error Message
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        [ValidateAntiForgeryToken]
        public ActionResult RecoverPassword(RecoverPasswordModel model)
        {
            //Put Try Catch
            UserAuthModel userAuthModel = new UserAuthModel();
            try
            {
                if (ModelState.IsValid)
                {
                    if (model.NewPassword.ToUpper().Equals((new Login().EncodePassword(model.UserName)).ToUpper()))
                    {
                        return Json(new { Success = false, ErrorMessage = "Password should not be same as User name." });
                    }

                    userAuthModel = new Login().UpdatePassword(model);

                    if (userAuthModel.IsOldAndNewPwdrSame)
                    {
                        return Json(new { Success = false, ErrorMessage = "New password should not be same as Old password." });
                    }

                    ModelState.Clear();
                    return Json(new { Success = true });
                }
                else
                {
                    StringBuilder errorMessages = new StringBuilder();
                    foreach (var modelStateValue in ModelState.Values)
                    {
                        foreach (var error in modelStateValue.Errors)
                        {
                            errorMessages.Append(error.ErrorMessage);
                        }
                    }
                    return Json(new { Success = false, ErrorMessage = errorMessages.ToString() });
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { Success = false, ErrorMessage = "Error occurred while changing the new password." });
            }
        }


        /// <summary>
        /// Get method for Change Password
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Audit]
        [RequiredAuthentication]
        public ActionResult ChangePassword()
        {
            var dbContext = new PMGSYEntities();
            try{
                    ChangePasswordModel model = new ChangePasswordModel();
                    model.UserId = PMGSYSession.Current.UserId;
                    model.UserName = PMGSYSession.Current.UserName;
                    model.RoleId = PMGSYSession.Current.RoleCode;

                    //if Already entered password question & answer
                    var PwdrQuesId = (from uup in dbContext.UM_Security_Question_Answer 
                                      where uup.UserID == PMGSYSession.Current.UserId
                                      select uup.PasswordQuestionID).FirstOrDefault();
                    if(PwdrQuesId != 0)
                    {
                        model.PwdrQuestionId = PwdrQuesId;
                        model.PwdrAnswer = dbContext.UM_Security_Question_Answer.Where(c => c.PasswordQuestionID == PwdrQuesId && c.UserID == PMGSYSession.Current.UserId).Select(c => c.Answer).FirstOrDefault();
                    }

                    model.QuestionList = new Login().GetPwdrQuestionList();
                   
                    return View(model);
            }
            catch(Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return View("Error");
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        /// <summary>
        /// If Old password is wrong, return view with respective message
        /// Else update password and redirect to Landing page.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        //[ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            
            //Put Try Catch
            UserAuthModel userAuthModel = new UserAuthModel();
            try
            {

                model.PwdrAnswer = DecryptAes(model.PwdrAnswer); //aaded BY PP
                ModelState.Remove("PwdrAnswer");
                //ModelState.SetModelValue("PwdrAnswer", new ValueProviderResult(model.PwdrAnswer, "", System.Globalization.CultureInfo.InvariantCulture));
                ModelState.Add(new KeyValuePair<string, System.Web.Mvc.ModelState>("PwdrAnswer", new System.Web.Mvc.ModelState() { Value =new ValueProviderResult(model.PwdrAnswer,"", System.Globalization.CultureInfo.InvariantCulture)}));
              
                //
                char[] newPassword = model.NewPassword.ToCharArray();

                string finalNewPassword = String.Empty;

                for (int i = newPassword.Length - 1; i >= 0; i--)
                {
                    finalNewPassword += newPassword[i];
                }


                char[] confirmPassword = model.ConfirmPassword.ToCharArray();

                string finalconfirmPassword = String.Empty;

                for (int i = confirmPassword.Length - 1; i >= 0; i--)
                {
                    finalconfirmPassword += confirmPassword[i];
                }

                model.NewPassword = finalNewPassword;
                model.ConfirmPassword = finalconfirmPassword;
                //
                
                TryValidateModel(model);

                if (ModelState.IsValid)
                {
                    if (model.NewPassword.ToUpper().Equals((new Login().EncodePassword(model.UserName)).ToUpper()))
                    {
                        return Json(new { Success = false, ErrorMessage = "Password should not be same as User name." });
                    }


                    if (model.NewPassword.Length < 32 || model.ConfirmPassword.Length < 32)
                    {
                        return Json(new { Success = false, ErrorMessage = "New Password and Confirm New Password fields are Invalid." });
                    }

                    userAuthModel = new Login().ChangePassword(model);

                    if (userAuthModel.IsOldPasswordWrong)
                    {
                        return Json(new { Success = false, ErrorMessage = "Old password entered is wrong." });
                    }

                    if (userAuthModel.IsOldAndNewPwdrSame)
                    {
                        return Json(new { Success = false, ErrorMessage = "New password should not be same as Old password." });
                    }

                    ModelState.Clear();
                    return Json(new { Success = true });
                }
                else
                {
                    StringBuilder errorMessages = new StringBuilder();
                    foreach (var modelStateValue in ModelState.Values)
                    {
                        foreach (var error in modelStateValue.Errors)
                        {
                            errorMessages.Append(error.ErrorMessage);
                        }
                    }                                           
                    return Json(new { Success = false, ErrorMessage = errorMessages.ToString() });
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Login.ChangePassword(ChangePasswordModel model)");
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { Success = false, ErrorMessage = "Error occurred while changing the new password." });
            }

        }


        /// <summary>
        /// If Seesion Expired, method is called which returns message as Session Expired.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult SessionExpire()
        {
            return View();
        }


        /// <summary>
        /// For Accountant Role, load home page as AccountDashBoard
        /// </summary>
        /// <returns></returns>
        [RequiredAuthentication]
        public ActionResult AccountDashBoard()
        {
            ViewBag.ActionToRedirect = TempData["roleDefaultPage"];
            return View();
        }


        /// <summary>
        /// Returns error message for user
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Error(string id)
        {
            return View();
        }


        /// <summary>
        /// Method for checking valid roles
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Audit]
        public ActionResult UnAuthorized()
        {
            return View();
        }

        //code added by Vikram (suggested by Anita Mam) for regenerating ASPNet_SessionId
        void regenerateId()
        {
            
            System.Web.SessionState.SessionIDManager manager = new System.Web.SessionState.SessionIDManager();

            string oldId = manager.GetSessionID(System.Web.HttpContext.Current);

            string newId = manager.CreateSessionID(System.Web.HttpContext.Current);

            bool isAdd = false, isRedir = false;

            manager.SaveSessionID(System.Web.HttpContext.Current, newId, out isRedir, out isAdd);

            HttpApplication ctx = (HttpApplication)System.Web.HttpContext.Current.ApplicationInstance;

            HttpModuleCollection mods = ctx.Modules;

            System.Web.SessionState.SessionStateModule ssm = (System.Web.SessionState.SessionStateModule)mods.Get("Session");

            System.Reflection.FieldInfo[] fields = ssm.GetType().GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            System.Web.SessionState.SessionStateStoreProviderBase store = null;

            System.Reflection.FieldInfo rqIdField = null, rqLockIdField = null, rqStateNotFoundField = null;

            foreach (System.Reflection.FieldInfo field in fields)
            {

                if (field.Name.Equals("_store")) store = (System.Web.SessionState.SessionStateStoreProviderBase)field.GetValue(ssm);

                if (field.Name.Equals("_rqId")) rqIdField = field;

                if (field.Name.Equals("_rqLockId")) rqLockIdField = field;

                if (field.Name.Equals("_rqSessionStateNotFound")) rqStateNotFoundField = field;

            }

            object lockId = rqLockIdField.GetValue(ssm);

            if ((lockId != null) && (oldId != null)) store.ReleaseItemExclusive(System.Web.HttpContext.Current, oldId, lockId);

            rqStateNotFoundField.SetValue(ssm, true);

            rqIdField.SetValue(ssm, newId);

            Session["ASP.NET_SessionId"] = newId;

            

        }

        //added by Pradip Patil[10/05/2018]
        public String DecryptAes(String encryptedString)
        {

            var cipherText = Convert.FromBase64String(encryptedString);
            var key = Encoding.UTF8.GetBytes("7061737323313233");
            var iv = Encoding.UTF8.GetBytes("7061737323313233");

            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
            {
                throw new ArgumentNullException("cipherText");
            }
            if (key == null || key.Length <= 0)
            {
                throw new ArgumentNullException("key");
            }
            if (iv == null || iv.Length <= 0)
            {
                throw new ArgumentNullException("key");
            }

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            // Create an RijndaelManaged object
            // with the specified key and IV.
            using (var rijAlg = new RijndaelManaged())
            {
                //Settings
                rijAlg.Mode = CipherMode.CBC;
                rijAlg.Padding = PaddingMode.PKCS7;
                rijAlg.FeedbackSize = 128;

                rijAlg.Key = key;
                rijAlg.IV = iv;

                // Create a decrytor to perform the stream transform.
                var decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);

                // Create the streams used for decryption.
                using (var msDecrypt = new MemoryStream(cipherText))
                {
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (var srDecrypt = new StreamReader(csDecrypt))
                        {
                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return plaintext;

        }

        //Added by abhinav pathak on 14-DEC-2018
        #region Tendor Publication Report
        [HttpGet]
        public ActionResult TendorPublicationReportLayout()
        {
            try
            {
                TendorPublicationReportModel EC = new TendorPublicationReportModel();
                CommonFunctions comm = new CommonFunctions();
                ECBriefReportController ecModel = new ECBriefReportController();

                if (PMGSYSession.Current.StateCode > 0)
                {
                    EC.StateCode = PMGSYSession.Current.StateCode;
                    EC.StateName = PMGSYSession.Current.StateName;
                    EC.State_Name = PMGSYSession.Current.StateName;

                    List<SelectListItem> lstState = new List<SelectListItem>();
                    lstState.Insert(0, new SelectListItem { Value = Convert.ToString(EC.StateCode), Text = Convert.ToString(EC.StateName) });
                    EC.StateList = new SelectList(lstState, "Value", "Text").ToList();

                    if (PMGSYSession.Current.DistrictCode > 0)
                    {
                        EC.DistrictCode = PMGSYSession.Current.DistrictCode;
                        EC.DistName = PMGSYSession.Current.DistrictName;

                        List<SelectListItem> lstDist = new List<SelectListItem>();
                        lstDist.Insert(0, new SelectListItem { Value = Convert.ToString(EC.DistrictCode), Text = Convert.ToString(EC.DistName) });
                        EC.DistrictList = new SelectList(lstDist, "Value", "Text").ToList();

                        List<SelectListItem> lstBlock = new List<SelectListItem>();
                        EC.BlockList = comm.PopulateBlocks(EC.DistrictCode, true);
                        EC.BlockList.RemoveAt(0);
                        EC.BlockList.Insert(0, new SelectListItem { Value = "0", Text = "All Blocks" });
                    }
                    else
                    {
                        EC.DistrictList = comm.PopulateDistrict(EC.StateCode, true);
                        EC.DistrictList.RemoveAt(0);
                        EC.DistrictList.Insert(0, new SelectListItem { Value = "0", Text = "All Districts" });

                        List<SelectListItem> lstBlock = new List<SelectListItem>();
                        lstBlock.Insert(0, new SelectListItem { Value = "0", Text = "All Blocks" });
                        EC.BlockList = new SelectList(lstBlock, "Value", "Text").ToList();
                    }

                    //EC.AgencyCode = PMGSYSession.Current.AdminNdCode;
                    //EC.AgencyName = PMGSYSession.Current.DepartmentName.Trim();

                    List<SelectListItem> lstAgency = new List<SelectListItem>();
                    EC.AgencyList = comm.PopulateAgenciesByStateAndDepartmentwise(EC.StateCode, PMGSYSession.Current.AdminNdCode, false);
                    EC.AgencyList.RemoveAt(0);

                    //EC.AgencyList = comm.PopulateAgencies(EC.StateCode, true);
                    //EC.AgencyCode = Convert.ToInt32(EC.AgencyList.Where(m => m.Selected == true).Select(m => m.Value).LastOrDefault());
                    //EC.AgencyList.RemoveAt(0);

                    //EC.CollaborationCode = 2;
                    EC.CollaborationList = ecModel.PopulateCollaborationsStateWise(EC.StateCode, true);
                    //EC.CollaborationList.RemoveAt(0);
                }
                else
                {
                    EC.StateList = comm.PopulateStates(true);
                    EC.StateList.RemoveAt(0);
                    EC.StateList.Insert(0, new SelectListItem { Value = "-1", Text = "Select State" });

                    List<SelectListItem> lstDistricts = new List<SelectListItem>();
                    lstDistricts.Insert(0, new SelectListItem { Value = "0", Text = "Select District" });
                    EC.DistrictList = new SelectList(lstDistricts, "Value", "Text").ToList();

                    List<SelectListItem> lstCollab = new List<SelectListItem>();
                    lstCollab.Insert(0, new SelectListItem { Value = "0", Text = "Select Collaboration" });
                    EC.CollaborationList = new SelectList(lstCollab, "Value", "Text").ToList();

                    List<SelectListItem> lstAgency = new List<SelectListItem>();
                    lstAgency.Insert(0, new SelectListItem { Value = "0", Text = "Select Agency" });
                    EC.AgencyList = new SelectList(lstAgency, "Value", "Text").ToList();

                    List<SelectListItem> lstBlock = new List<SelectListItem>();
                    lstBlock.Insert(0, new SelectListItem { Value = "0", Text = "All Blocks" });
                    EC.BlockList = new SelectList(lstBlock, "Value", "Text").ToList();
                }

                EC.YearList = comm.PopulateFinancialYear(true, true).ToList();
                EC.YearList.RemoveAt(0);
                //EC.YearList.Insert(0, new SelectListItem { Value = "0", Text = "All Years" });
                EC.BatchList = comm.PopulateBatch();
                EC.BatchList.RemoveAt(0);
                EC.BatchList.Insert(0, new SelectListItem { Value = "0", Text = "All Batches" });
                EC.SchemeList = comm.PopulateScheme();
                return View(EC);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Login.TendorPublicationReportLayout()");
                return null;
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TendorPublicationReportLayoutPost(TendorPublicationReportModel EC)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    EC.LevelCode = 1;
                    if (PMGSYSession.Current.StateCode > 0)
                    {
                        EC.StateName = PMGSYSession.Current.StateName;
                    }
                    return View(EC);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Login.TendorPublicationReportLayoutPost");
                return null;
            }

        }

        #endregion

        /// PMGSY3
        #region Menu Render PMGSY3
        [Audit]
        public ActionResult MenuSelection()
        {
            SchemewiseMenuSelectionViewModel model = new SchemewiseMenuSelectionViewModel();
            try
            {
                ViewBag.EncryptedProgramme = URLEncrypt.EncryptParameters(new string[] { "P" });
                model.lstPmgsyScheme = new List<SelectListItem>();
                model.lstPmgsyScheme.Insert(0, new SelectListItem() { Text = "PMGSY-1", Value = "1" });
                model.lstPmgsyScheme.Insert(1, new SelectListItem() { Text = "PMGSY-2", Value = "2" });
                model.lstPmgsyScheme.Insert(2, new SelectListItem() { Text = "RCPLWE", Value = "3" });
                model.lstPmgsyScheme.Insert(3, new SelectListItem() { Text = "PMGSY-3", Value = "4" });

                return View(model);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Login.MenuSelection()");
                return null;
            }
        }

        [Audit]
        public ActionResult SetRedirectUrl(string id)
        {
            try
            {
                //string[] strParameters = URLEncrypt.DecryptParameters(new string[] { parameter, hash, key });
                PMGSYSession.Current.PMGSYScheme = Convert.ToByte(id.Trim());
                int roleCode = PMGSYSession.Current.RoleCode;
                string url = string.Empty;

                int stateCode = PMGSYSession.Current.StateCode;

                //Changes by Shreyas for Scheme 5 (Village Vibrent Scheme) on 22-06-23
                if (PMGSYSession.Current.PMGSYScheme == 5)
                {
                    if (!(stateCode == 3 || stateCode == 30 || stateCode == 34 || stateCode == 14 || roleCode == 25 || roleCode == 3  || roleCode == 15))	//15 for PTA
                    {
                        return Json(new { status = true, url = "-", message = "Not Eligible for Village Vibrent Scheme" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    { }

                }


                if (roleCode == 22)
                {
                    url = PMGSYSession.Current.PMGSYScheme == 4 ? "/ExistingRoads/ListExistingRoadsPMGSY3" : PMGSYSession.Current.PMGSYScheme == 3 ? "/ExistingRoads/ListExistingRoadsPMGSY3" : PMGSYSession.Current.PMGSYScheme == 5 ? "/ExistingRoads/ListExistingRoadsPMGSY3" : "/Proposal/ListProposal";
                }
                else if (roleCode == 15)
                {
                    //url = PMGSYSession.Current.PMGSYScheme == 5 ? "/Proposal/ListProposalVibrantVillage" : "/Proposal/ListProposal";

                    url = PMGSYSession.Current.PMGSYScheme == 5 ? "/Proposal/ListProposalVibrantVillage" : "";
                }
                else if (roleCode == 36)
                {
                    url = PMGSYSession.Current.PMGSYScheme == 4 ? "/ExistingRoads/GetTraceMaps" : "/LocationMasterDataEntry/MasterDataEntry";
                }
                else if (roleCode == 2)
                {
                    url = PMGSYSession.Current.PMGSYScheme == 4 ? "/ExistingRoads/GetTraceMaps" : "/Proposal/ListProposal";
                }
                else if (roleCode == 3)
                {
                    url = PMGSYSession.Current.PMGSYScheme == 4 ? "/Proposal/ListProposalPMGSY3" : "/Proposal/ListProposal";
                }

                return Json(new { status = true, url = url }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ErrorLog.LogError(ex, "Login.SetRedirectUrl()");
                return Redirect("login/error");
            }
        }
        #endregion
    }
}

