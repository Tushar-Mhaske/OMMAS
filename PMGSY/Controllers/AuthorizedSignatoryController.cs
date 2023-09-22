using PMGSY.BAL.AuthorizedSignatory;
using PMGSY.Models.AuthorizedSignatory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PMGSY.Extensions;
using PMGSY.Common;
using System.Globalization;
using PMGSY.Models;
using System.Data.Entity.Validation;
using System.Data.Entity;


namespace PMGSY.Controllers
{
     [RequiredAuthentication]
     [RequiredAuthorization]
    public class AuthorizedSignatoryController : Controller
    {
        //
        // GET: /AuthorizedSignatory/
        private PMGSYEntities dbContext = null;


        public AuthorizedSignatoryController()
        {
            dbContext = new PMGSYEntities();
        }

         [Audit]
        [GenericAccountValidationFilter(InputParameter = new string[] { "BankDetails" })]
        public ActionResult GetAuthorizedSignatoryDetails()
        {
           
            return View();
        }

         [Audit]
         public JsonResult GetAuthorizedSignatoryList(int? page, int? rows, string sidx, string sord)
        {

            //Adde By Abhishek kamble 30-Apr-2014 start  
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            //Adde By Abhishek kamble 30-Apr-2014 end

            List<AuthorizedSignatoryModel> authSigList = new List<AuthorizedSignatoryModel>();
            IAuthorizedSignatoryBAL objChartOfAccountsBAL = new AuthorizedSignatoryBAL();
            CommonFunctions objCommon = new CommonFunctions();
            long  totalRecords=0;
            string ShowActiveAuthSig ="Y";
            string searchString = string.Empty;
            searchString = Request.Params["varSearch"];
            ShowActiveAuthSig =(Request.Params["activeStatus"]);
            int stateCode = PMGSYSession.Current.StateCode;
           //coment out later
            //stateCode = 33;

            authSigList = objChartOfAccountsBAL.GetAuthorizedSignatoryDetails(page - 1, rows, sidx, sord, stateCode, ShowActiveAuthSig, searchString, out totalRecords);


            var data = (authSigList.Select(c => new
            {
                //id = c.ND_CODE,
                cell = new[]
                        {
                            c.ADMIN_ND_NAME == null ? String.Empty:c.ADMIN_ND_NAME.ToString(),
                            c.ADMIN_FULL_NAME == null ? string.Empty:c.ADMIN_FULL_NAME.ToString(),
                           ( c.ADMIN_ACTIVE_STATUS ==null ||  c.ADMIN_NO_DESIGNATION== 0) ? string.Empty:dbContext.MASTER_DESIGNATION.Where(x=>x.MAST_DESIG_CODE==  c.ADMIN_NO_DESIGNATION).Select(v=>v.MAST_DESIG_NAME).FirstOrDefault(),
                            c.START_DATE != null? c.START_DATE:string.Empty,
                            c.END_DATE != null ?c.END_DATE:string.Empty,
                           // c.ADMIN_NO_ADDRESS1 == null ?string.Empty:c.ADMIN_NO_ADDRESS1.ToString(),
                           // c.ADMIN_NO_ADDRESS2== null ?string.Empty:c.ADMIN_NO_ADDRESS2.ToString(),
                            c.ADMIN_MOBILE==null ? string.Empty:c.ADMIN_MOBILE.ToString(),                            
                            c.ADMIN_EMAIL=="NULL"?string.Empty:c.ADMIN_EMAIL==null?string.Empty:c.ADMIN_EMAIL.ToString(),
                            //new added by abhishek (c.START_DATE==null?"Not Available")
                            c.START_DATE==null?"Not Available": c.ADMIN_ACTIVE_STATUS.ToString(),//(c.ADMIN_ACTIVE_STATUS=="Y"?"Currently Working":"Not Working"),
                            c.DSCRegistered.ToString(),
                            //c.DSCRegistered.Trim().ToUpper().ToString() == "REGISTERED" ?"<a href='#' class='ui-icon ui-icon-refresh ui-align-center' onclick='DeRegister(\"" + URLEncrypt.EncryptParameters(new string[]{c.ND_CODE.ToString()+"$"+c.STATE_CODE +"$"+"E" +"$"+c.MAST_DISTRICT_CODE+"$"+c.ADMIN_NO_OFFICER_CODE})+"\");return false;'>Un -Register</a>":"",

                            (c.DSCRegistered.Trim().ToUpper().ToString() == "REGISTERED" && PMGSYSession.Current.LevelId == 4 && PMGSYSession.Current.FundType=="P"  )  ?"<a href='#' class='ui-icon ui-icon-refresh ui-align-center' onclick='DeRegister(\"" + URLEncrypt.EncryptParameters(new string[]{c.ND_CODE.ToString()+"$"+c.STATE_CODE +"$"+"E" +"$"+c.MAST_DISTRICT_CODE+"$"+c.ADMIN_NO_OFFICER_CODE})+"\");return false;'>Un -Register</a>":"",
                            
                            
                            (PMGSYSession.Current.LevelId == 4 && PMGSYSession.Current.FundType=="P") ?   c.START_DATE == null? "<a href='#' class='ui-icon ui-icon-circle-plus ui-align-center' onclick='ADDAuthSignatory(\"" + URLEncrypt.EncryptParameters(new string[]{c.ND_CODE.ToString()+"$"+c.STATE_CODE+"$"+"A" +"$"+c.MAST_DISTRICT_CODE})+"\");return false;'>Add</a>" :
                           // provision to add edit only at SRRDA level & for program fund
                            ( c.END_DATE == null ? "<a href='#' class='ui-icon ui-icon-pencil ui-align-center' onclick='EditAuthSignatory(\"" + URLEncrypt.EncryptParameters(new string[]{c.ND_CODE.ToString()+"$"+c.STATE_CODE +"$"+"E" +"$"+c.MAST_DISTRICT_CODE+"$"+c.ADMIN_NO_OFFICER_CODE})+"\");return false;'>Edit</a>":
                            "<a href='#' class='ui-icon ui-icon-search ui-align-center' onclick='ViewAuthSignatory(\"" + URLEncrypt.EncryptParameters(new string[]{c.ND_CODE.ToString()+"$"+c.STATE_CODE+"$"+"V" +"$"+c.MAST_DISTRICT_CODE+"$"+c.ADMIN_NO_OFFICER_CODE})+"\");return false;'>View</a>") :String.Empty
                                
                                             
                        }
            })).ToArray();


            var jsonData = new
            {
                //total = Math.Ceiling(Convert.ToDouble(totalRecords) / Convert.ToDouble(rows == null ? 1 : rows)),
                page = page == null ? 0 : page,
                records = totalRecords,                
                rows = data,
                total=0

            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Audit]
        public ActionResult AddEditAuthSignatory(String parameter, String hash, String key)
        {

            AuthorizedSignatoryModel model = new AuthorizedSignatoryModel();
            IAuthorizedSignatoryBAL objChartOfAccountsBAL = new AuthorizedSignatoryBAL();
            CommonFunctions objCommFunc = new CommonFunctions();
            try
            {
                int adminNdCode = 0;
                int adminNoOfficerCode = 0;
                int stateCode = 0;
                int DistrictCode = 0;
                String Operation = string.Empty;
                SelectList districtList = null;
                SelectList DesignationList = null;
                int officerCode = 0;

                if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                {
                    String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                    if (urlParams.Length >= 1)
                    {
                        String[] urlSplitParams = urlParams[0].Split('$');
                        adminNdCode = Convert.ToInt32(urlSplitParams[0]);
                        stateCode = Convert.ToInt32(urlSplitParams[1]);
                        Operation = (urlSplitParams[2]).ToString();
                        if (urlSplitParams[3] != string.Empty)
                        {
                            DistrictCode = Convert.ToInt32(urlSplitParams[3]);
                        }

                        if (Operation == "E" || Operation=="V")
                        {
                            adminNoOfficerCode = Convert.ToInt32(urlSplitParams[4]);
                        }
                    }
                }

           
                //if edit operation
                if (Operation == "E" || Operation == "V")
                {

                    //get the authorized signatory details to view edit
                    model = objChartOfAccountsBAL.GetAuthorizedSignatoryDetails(adminNdCode,adminNoOfficerCode, Operation == "E" ? "Y" : "N");
                    officerCode = model.ADMIN_OFFICER_CODE;
                    districtList = new SelectList(dbContext.MASTER_DISTRICT.Where(c => c.MAST_STATE_CODE == stateCode), "MAST_DISTRICT_CODE", "MAST_DISTRICT_NAME", model.MAST_DISTRICT_CODE);
                    DesignationList = new SelectList(dbContext.MASTER_DESIGNATION, "MAST_DESIG_CODE", "MAST_DESIG_NAME", model.ADMIN_NO_DESIGNATION);

                }
                else
                {

                    model.ADMIN_MNAME = string.Empty;
                    model.ADMIN_ADDRESS2 = string.Empty;
                    model.ADMIN_PIN = string.Empty;
                    model.ADMIN_RESIDENCE_STD = string.Empty;
                    model.ADMIN_RESIDENCE_PHONE = string.Empty;
                    model.ADMIN_OFFICE_STD = string.Empty;
                    model.ADMIN_OFFICE_PHONE = string.Empty;
                    model.ADMIN_STD_FAX = string.Empty;
                    model.ADMIN_FAX = string.Empty;
                    model.ADMIN_MOBILE = string.Empty;
                    model.ADMIN_EMAIL = string.Empty;

                    if (dbContext.ADMIN_NODAL_OFFICERS.Any(no => no.ADMIN_ACTIVE_STATUS == "N" && no.ADMIN_ND_CODE == adminNdCode && no.ADMIN_MODULE == "A"))
                    {
                        DateTime? endDate = dbContext.ADMIN_NODAL_OFFICERS.Where(no => no.ADMIN_ACTIVE_STATUS == "N" && no.ADMIN_ND_CODE == adminNdCode && no.ADMIN_MODULE=="A").Max(no => no.ADMIN_ACTIVE_END_DATE);
                        model.START_DATE = endDate == null ? string.Empty : Convert.ToDateTime(endDate).ToString("dd/MM/yyyy");
                    }
                    districtList = new SelectList(dbContext.MASTER_DISTRICT.Where(c => c.MAST_STATE_CODE == stateCode), "MAST_DISTRICT_CODE", "MAST_DISTRICT_NAME");
                    DesignationList = new SelectList(dbContext.MASTER_DESIGNATION, "MAST_DESIG_CODE", "MAST_DESIG_NAME", 26);

                }

                ViewBag.MAST_DISTRICT_CODE = districtList;
                ViewBag.ADMIN_NO_DESIGNATION = DesignationList;
                ViewBag.encryptedNdCode = URLEncrypt.EncryptParameters(new string[] { adminNdCode.ToString() + "$" + stateCode + "$" + Operation + "$" + officerCode + "$" + DistrictCode +"$"+adminNoOfficerCode});
                ViewBag.stateCode = stateCode;
                ViewBag.DistrictCode = DistrictCode;
                ViewBag.isEdit = Operation;
                ViewBag.startDate = model.startDate;
                ViewBag.EndDate = model.EndDate;
                model.CURRENT_DATE =objCommFunc.GetDateTimeToString(System.DateTime.Now);
                return View(model);
            }
            catch(Exception ex)
            {
                string errorMsg = string.Format("Errors: {0}", ex.Message);
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = 500;
                Response.Write(errorMsg);
                return Json(string.Empty);
            }
        }
        
        /// <summary>
        /// Action to Add /edit the aithorized signatory
        /// </summary>
        /// <param name="model"> model to add/ edit</param>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns> Operation Status </returns>
         [HttpPost]
        [Audit]
        public ActionResult AddEditAuthSignatory(AuthorizedSignatoryModel model, String parameter, String hash, String key)
        {

            IAuthorizedSignatoryBAL objChartOfAccountsBAL = new AuthorizedSignatoryBAL();
            int adminNdCode = 0;
            int adminNoOfficerCode = 0; 
            int stateCode = 0;
            int DistrictCode = 0;
            String Operation = string.Empty;
            int officerCode = 0;
            String result =  string.Empty;
            string message = string.Empty;
            try
            {

                if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                {
                    String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                    if (urlParams.Length >= 1)
                    {
                        String[] urlSplitParams = urlParams[0].Split('$');
                        adminNdCode = Convert.ToInt32(urlSplitParams[0]);
                        stateCode = Convert.ToInt32(urlSplitParams[1]);
                        Operation =(urlSplitParams[2]).ToString();
                        officerCode = Convert.ToInt32(urlSplitParams[3]);
                        
                        if (urlSplitParams[4] != string.Empty)
                        {
                            DistrictCode = Convert.ToInt32(urlSplitParams[4]);
                        }
                        adminNoOfficerCode = Convert.ToInt32(urlSplitParams[5]);
                    }
                }

                if (model.ADMIN_AADHAR_PAN_FLAG=="A")
                {
                    if (ModelState.ContainsKey("ADMIN_PAN_NO"))
                        ModelState["ADMIN_PAN_NO"].Errors.Clear();
                }
                else if (model.ADMIN_AADHAR_PAN_FLAG=="P")
                {
                    if (ModelState.ContainsKey("ADMIN_AADHAR_NO"))
                        ModelState["ADMIN_AADHAR_NO"].Errors.Clear();
                }

                if (ModelState.IsValid)
                {
                    if (model.ADMIN_NO_DESIGNATION == null || model.ADMIN_NO_DESIGNATION == 0)
                    {
                        ModelState.AddModelError("ADMIN_NO_DESIGNATION", "Designation Required");
                    }    
                
                    //save update the details in the database

                    result = objChartOfAccountsBAL.AddEditAuthorizedSignatoryDetails(adminNoOfficerCode,adminNdCode, officerCode, DistrictCode,Operation, model, ref message);
                                       
                    ModelState.Clear();

                    if (result == "1")
                    {
                        if (Operation == "A")
                        {
                            return Json(new { Success = true, Operation = "A", message = message });
                        }
                        else
                        {

                            return Json(new { Success = true, Operation = "E", message = message });
                        }
                    }
                    else
                    {
                        return Json(new { Success = false, Operation = "AE", message = message });
                    }

                }
                else
                {
                                  
                    ViewBag.MAST_DISTRICT_CODE = new SelectList(dbContext.MASTER_DISTRICT.Where(c => c.MAST_STATE_CODE == stateCode), "MAST_DISTRICT_CODE", "MAST_DISTRICT_NAME");
                    ViewBag.ADMIN_NO_DESIGNATION = new SelectList(dbContext.MASTER_DESIGNATION, "MAST_DESIG_CODE", "MAST_DESIG_NAME");
                    ViewBag.encryptedNdCode = URLEncrypt.EncryptParameters(new string[] { adminNdCode.ToString() + "$" + stateCode + "$" + Operation + "$" + officerCode + "$" + DistrictCode +"$"+adminNoOfficerCode});
                    ViewBag.isEdit = Operation;
                    return View(model);
                }
            }

            catch (DbEntityValidationException ex)
            {
                
                string exMessege =string.Empty;
                
                foreach (var eve in ex.EntityValidationErrors)
                {
                   
                    foreach (var ve in eve.ValidationErrors)
                    {

                        exMessege += ve.ErrorMessage +" "; 
                        
                    }
                }
                Exception exe = new Exception(exMessege);

                throw exe;
            }
            catch(Exception ex)
            {
                string errorMsg = string.Format("Errors: {0}", ex.Message);
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = 500;
                Response.Write(errorMsg);
                return Json(string.Empty);
            }
        }


         [Audit]
         public JsonResult DeRegisterDSC(String parameter, String hash, String key)
         {
             int adminNoOffCode = 0;
             int result = 0;
             IAuthorizedSignatoryBAL objAuthBAL = new AuthorizedSignatoryBAL();

             if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
             {
                 String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                 if (urlParams.Length >= 1)
                 {
                     adminNoOffCode = Convert.ToInt32(urlParams[0].Split('$')[4]);
                 }

                 result = objAuthBAL.DeRegisterDSC(adminNoOffCode);
                 return Json(new { result = result });

             }
             else
             {

                 throw new Exception("Error While un-registering authorise signatory details");
             }

         }
    }
}
