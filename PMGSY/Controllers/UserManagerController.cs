using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PMGSY.Models;
using System.Data.Entity.Validation;
using System.Diagnostics;
using PMGSY.Models.UserManager;
using PMGSY.BAL.User_Manager;
using PMGSY.Extensions;
using PMGSY.Common;
using System.Text;
using PMGSY.DAL;
using PMGSY.DAL.User_Manager;
using System.Net;
using System.Net.Mail;
using System.Configuration;

namespace PMGSY.Controllers
{
    [RequiredAuthentication]
    [RequiredAuthorization]
    public class UserManagerController : Controller
    {
        public UserManagerController()
        {
            PMGSYSession.Current.ModuleName = "User Manager";
        }

        private PMGSYEntities db = new PMGSYEntities();

       #region Create new user

    
        /// <summary>
        /// Get method to Create new user
        /// </summary>
        /// <returns></returns>
        [Audit]
        public ActionResult Create()
        {
            CommonFilterWrapper commonFilterWrapper = new CommonFilterWrapper();
            CommonFiltersController commonFilterController = new CommonFiltersController();
            UserManagerBAL objUMBAL = new UserManagerBAL();
            db = new PMGSYEntities();
            try
            {
                ViewBag.Operation = "A";

                ViewBag.PreferedCssID = new SelectList(db.UM_Css_Master, "CssID", "CssName");
                ViewBag.PreferedLanguageID = new SelectList(db.UM_Language_Master, "LanguageID", "LanguageName");

                User_Master umUserMaster = new User_Master();

                List<SelectListItem> StateDefaultListItem = new List<SelectListItem>();
                StateDefaultListItem.Add(new SelectListItem { Text = "Select State", Value = "0" });
                umUserMaster.StateList = StateDefaultListItem;//commonFilterWrapper.GetStates(0);

                umUserMaster.SQMList = new List<SelectListItem>();
                umUserMaster.SQMList.Insert(0, (new SelectListItem { Text = "Select SQM", Value = "0" }));

                umUserMaster.DistrictList = commonFilterWrapper.GetDistricts(0, 0);
                umUserMaster.LevelList = new SelectList(db.UM_Level_Master.Where(c => c.IsActive == true).OrderBy(c => c.LevelName), "LevelID", "LevelName").ToList();
                umUserMaster.DepartmentList = objUMBAL.GetNDCode(0, 0, 0, 0);
                umUserMaster.RoleList = objUMBAL.GetRoles(0);

                umUserMaster.CSSList = new SelectList(db.UM_Css_Master, "CssID", "CssName").ToList();
                umUserMaster.LanguageList = new SelectList(db.UM_Language_Master, "LanguageID", "LanguageName").ToList();


                return View(umUserMaster);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                throw ex;
            }
            finally 
            {
                db.Dispose();
            }
        }



        /// <summary>
        /// Post to Create New User
        /// </summary>
        /// <param name="um_user_master"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult Create(User_Master um_user_master)
        {
            UserManagerBAL objUMBAL = new UserManagerBAL();
            var context = new PMGSYEntities();
            CommonFilterWrapper commonFilterWrapper = new CommonFilterWrapper();
            try
            {
                if (um_user_master.SQMID ==0)
                {
                    ModelState["SQMID"].Errors.Clear();
                }
                if (ModelState.IsValid)
                {
                    if (context.UM_User_Master.Any(u => u.UserName == um_user_master.UserName.Trim()))
                    {
                        return Json(new { Success = false, ErrorMessage = "User with same name already exists, Please choose different User Name " });
                    }
                    else
                    {
                        bool isCreated = objUMBAL.CreateUser(um_user_master);
                        ModelState.Clear();

                        if (isCreated)
                        {
                            return Json(new { Success = true }); //RedirectToAction("Create");
                        }
                        else
                        {
                            return Json(new { Success = false, ErrorMessage = "Error occured while creation of new user." });
                        }
                    }
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
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                ErrorLog.LogError(ex, "UserManager.Create()");
                ModelState.Clear();
                return Json(new { Success = false, ErrorMessage = "Error occured while creation of new user." });
            }
            finally
            {
                if (context != null)
                    context.Dispose();
            }
        }


        /// <summary>
        /// Populate Departments
        /// </summary>
        /// <param name="selectedState"></param>
        /// <returns></returns>
        [Audit]
        public JsonResult GetNDCode(Int32 selectedState, Int32 selectedDistrict,  Int32 selectedLevel,Int32 selectedRole)
        {
            List<SelectListItem> lstNDCode = new List<SelectListItem>();
            try
            {
                SelectListItem item = new SelectListItem();
                if (selectedState == 0)
                {
                    return Json(lstNDCode);
                }
                else
                {
                    UserManagerBAL objUMBAL = new UserManagerBAL();
                    lstNDCode = objUMBAL.GetNDCode(selectedState, selectedDistrict, selectedLevel, selectedRole);
                    return Json(lstNDCode);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }


        /// <summary>
        /// Populate Sub Modules (Actions)
        /// </summary>
        /// <param name="selectedState"></param>
        /// <returns></returns>
        [Audit]
        public JsonResult GetSubModules(Int32 moduleId)
        {
            List<SelectListItem> lstSubModules = new List<SelectListItem>();
            try
            {
                SelectListItem item = new SelectListItem();
                if (moduleId == 0)
                {
                    return Json(lstSubModules);
                }
                else
                {
                    UserManagerBAL objUMBAL = new UserManagerBAL();
                    lstSubModules = objUMBAL.GetSubModules(moduleId);
                    return Json(lstSubModules);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            } 
         }


        [Audit]
        public JsonResult PopulateStates(int selectedLevel)
        {
            CommonFilterWrapper commonFilterWrapper = new CommonFilterWrapper();
            List<SelectListItem> lstStates = new List<SelectListItem>();
            try
            {
                SelectListItem item = new SelectListItem();

                if (selectedLevel == 4 || selectedLevel == 5) //Only For Level State, District
                {
                    lstStates = commonFilterWrapper.GetStates(0);
                    return Json(lstStates);
                }
                else
                {
                    lstStates.Add(new SelectListItem { Text = "Select State", Value = "0" });
                    return Json(lstStates);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }


        /// <summary>
        /// Populate Districts for selected State
        /// </summary>
        /// <param name="selectedState"></param>
        /// <returns></returns>
        [Audit]
        public List<SelectListItem> GetDistrictList(int selectedState)
        {
            List<SelectListItem> lstDistricts = new List<SelectListItem>();

            if (selectedState == 0)
            {
                SelectListItem item = new SelectListItem();
                item.Text = "Select District";
                item.Value = "0";
                item.Selected = true;
                lstDistricts.Add(item);
            }
            else
            {
                using (var dbContext = new PMGSYEntities())
                {
                    lstDistricts = dbContext.MASTER_DISTRICT.ToList().Where(x => x.MAST_STATE_CODE == selectedState && x.MAST_DISTRICT_ACTIVE.Equals("Y")).OrderBy(x => x.MAST_DISTRICT_NAME).Select(x => new SelectListItem
                    {
                        Value = x.MAST_DISTRICT_CODE.ToString(),
                        Text = x.MAST_DISTRICT_NAME
                    }).ToList<SelectListItem>();
                }
            }

            return lstDistricts;
        }


        /// <summary>
        /// Populate Roles for selected level
        /// </summary>
        /// <param name="selectedLevel"></param>
        /// <returns></returns>
        [Audit]
        public JsonResult GetRolesList(int selectedLevel)
        {
            UserManagerBAL umBAL = new  UserManagerBAL();
            List<SelectListItem> lstRoles = new List<SelectListItem>();
            try
            {
                SelectListItem item = new SelectListItem();
          
                if (selectedLevel == 0)
                {
                    return Json(lstRoles);
                }
                else
                {
                    lstRoles = umBAL.GetRoles(selectedLevel);
                    return Json(lstRoles);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

#endregion


        #region Admin Home Page

        /// <summary>
        /// Returns Empty Json for Levels
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Audit]
        public JsonResult GetLevels()
        {
            return Json(String.Empty);
        }



        /// <summary>
        /// Use sp_GetLevelWiseRoles
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public JsonResult GetLevels(int? page, int? rows, string sidx, string sord)
        {
            LevelRolesMappingDTO objLRMapDTO = new LevelRolesMappingDTO();
            UserManagerBAL objUMBAL = new UserManagerBAL();
            String[] strPathParams = null;
            try
            {
                //Adde By Abhishek kamble 1-May-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 1-May-2014 end
                strPathParams = Request.Url.AbsolutePath.Split('/');

                //nodeid is a field that has been internally created and set by the treegrid to indicate the id of the node that has been expanded
                if (!Request.Form.AllKeys.Contains("nodeid") || Request.Form["nodeid"].Equals(""))
                {
                    if (strPathParams != null && strPathParams.Length >= 5)
                    {
                        objLRMapDTO.RoleID = Convert.ToString(strPathParams[4]);
                    }
                }
                else
                {
                    objLRMapDTO.RoleID = Convert.ToString(Request.Form["nodeid"]);
                }

                if (objLRMapDTO.RoleID != null)   // For Leaf Nodes
                {
                    if (objLRMapDTO.RoleID.Contains("R"))
                        return Json(String.Empty);
                }

                List<LevelRolesListDTO> lstItems = objUMBAL.GetRoleItems(objLRMapDTO);

                var jsonData = new
                {
                    rows = objUMBAL.GetJSONRoleItemCollection(lstItems)
                };

                return Json(jsonData);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                string errorMsg = string.Format("Errors: {0}", ex.Message);
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = 500;
                Response.Write(errorMsg);
                return Json(string.Empty);
            }
        }


        /// <summary>
        /// Display Role Details 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Audit]
        public ActionResult RoleDetails()
        {
            RoleDetailsModel roleDetailsModel = new RoleDetailsModel();
            return PartialView("_RoleDetailsPartial", roleDetailsModel);
        }


        /// <summary>
        /// On selection of tree node, returns Role details
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="levelId"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]     
        public ActionResult RoleDetails(string roleId, string levelId)
        {
            UserManagerBAL objUMBAL = new UserManagerBAL();
            RoleDetailsModel roleDetailsModel = new RoleDetailsModel();
            try
            {
                roleDetailsModel = objUMBAL.RoleDetails(roleId, levelId);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                throw ex;
            }
            
            //ViewBag.RoleDetailsModel = roleDetailsModel;
            
            return PartialView("_RoleDetailsPartial", roleDetailsModel);
        }


        /// <summary>
        /// Returns Roles mapped with menus for populating in Grid (Listing)
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="levelId"></param>
        /// <param name="homeFormCollection"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult RoleMenuMappingDetails(string roleId, string levelId, FormCollection homeFormCollection)
        {
            UserManagerBAL objUMBAL = new UserManagerBAL();
            String searchParameters = string.Empty;
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            long totalRecords;

            try
            {
                if (roleId == null)
                {
                    return Json(String.Empty);
                }


                if (!string.IsNullOrEmpty(homeFormCollection["searchField"]))
                {
                    searchParameters = HttpUtility.HtmlDecode(homeFormCollection["searchField"]);
                    searchParameters = searchParameters.Replace("%2F", "/");
                    string[] str = (searchParameters.ToString().Split('&'));
                    for (int i = 0; i < str.Length; ++i)
                    {
                        string[] splitParameter = str[i].Split('=');
                        parameters.Add(splitParameter[0].Trim(), splitParameter[1].Trim());
                    }

                }

                var jsonData = new
                {
                    rows = objUMBAL.RoleMenuMappingDetails(roleId, levelId, Convert.ToInt32(homeFormCollection["page"]) - 1,
                                            Convert.ToInt32(homeFormCollection["rows"]),
                                            homeFormCollection["sidx"],
                                            homeFormCollection["sord"], out totalRecords),
                    //RegistredDocumentsDetails.GetSearchResultList(UserName, Convert.ToInt32(homeFormCollection["page"])- 1, Convert.ToInt32(homeFormCollection["rows"]),
                    //homeFormCollection["sidx"], homeFormCollection["sord"], out totalRecords),
                    total = totalRecords <=
                    Convert.ToInt32(homeFormCollection["rows"]) ? 1 : totalRecords /
                    Convert.ToInt32(homeFormCollection["rows"]) + 1,
                    page = Convert.ToInt32(homeFormCollection["page"]),
                    records = totalRecords
                };


                return Json(jsonData);

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                throw ex;
            }

        }



        /// <summary>
        /// Update Role & Menu Mapping
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="levelId"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult RoleMenuMapping(string roleId, string levelId)
        {
            ViewBag.RoleId = roleId;
            ViewBag.LevelId = levelId;
            return PartialView("_RoleMenuMappingDetailsPartial");
        }


        /// <summary>
        /// Update Role & User Mapping
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult RoleUserMapping(string roleId)
        {
            ViewBag.RoleId = roleId;
            return PartialView("_RoleUserMappingDetailsPartial");
        }



        /// <summary>
        /// Listing of Rolewise User mapping details
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="homeFormCollection"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult RoleUserMappingDetails(string roleId, FormCollection homeFormCollection)
        {
            UserManagerBAL objUMBAL = new UserManagerBAL();
            String searchParameters = string.Empty;
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            long totalRecords;

            try
            {
                if (roleId == null)
                {
                    return Json(String.Empty);
                }


                if (!string.IsNullOrEmpty(homeFormCollection["searchField"]))
                {
                    searchParameters = HttpUtility.HtmlDecode(homeFormCollection["searchField"]);
                    searchParameters = searchParameters.Replace("%2F", "/");
                    string[] str = (searchParameters.ToString().Split('&'));
                    for (int i = 0; i < str.Length; ++i)
                    {
                        string[] splitParameter = str[i].Split('=');
                        parameters.Add(splitParameter[0].Trim(), splitParameter[1].Trim());
                    }

                }

                var jsonData = new
                {
                    rows = objUMBAL.RoleUserMappingDetails(roleId,Convert.ToInt32(homeFormCollection["page"]) - 1,
                                            Convert.ToInt32(homeFormCollection["rows"]),
                                            homeFormCollection["sidx"],
                                            homeFormCollection["sord"], out totalRecords),
                    total = totalRecords <=
                    Convert.ToInt32(homeFormCollection["rows"]) ? 1 : totalRecords /
                    Convert.ToInt32(homeFormCollection["rows"]) + 1,
                    page = Convert.ToInt32(homeFormCollection["page"]),
                    records = totalRecords
                };
                
                
                return Json(jsonData);

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(string.Empty);
            }
        }



        /// <summary>
        /// Get for Role Summary
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [RequiredAuthorization]
        [Audit]
        public ActionResult RoleSummary()
        {
            return View("RoleSummary");
        }


        


        #endregion


        # region Role Mapping

        /// <summary>
        /// Get to Create Role
        /// </summary>
        /// <returns></returns>
        public ActionResult CreateRole()
        {
            db = new PMGSYEntities();
            try
            {
                ViewBag.Operation = "A";
                ViewBag.LevelMaster = new SelectList(db.UM_Level_Master.Where(c => c.IsActive == true).OrderBy(c => c.LevelName), "LevelID", "LevelName");
                return View(new Role_Master());
            }
            catch{
                return null;
            }
            finally
            {
                //db.Dispose();
            }
        }


        /// <summary>
        /// Save Role Details to UM_Role_Master
        /// </summary>
        /// <param name="um_role_master"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult CreateRole(Role_Master um_role_master)
        {
            UserManagerBAL objUMBAL = new UserManagerBAL();
            var context = new PMGSYEntities();
            try
            {
                ViewBag.Operation = "A";
                ViewBag.LevelMaster = new SelectList(context.UM_Level_Master.Where(c => c.IsActive == true).OrderBy(c => c.LevelName), "LevelID", "LevelName");
                if (ModelState.IsValid)
                {
                    if (context.UM_Role_Master.Any(u => u.RoleName == um_role_master.RoleName.Trim()))
                    {
                        ModelState.AddModelError("RoleName", "Role with same name already exists, Please enter different Role.");
                        return View(um_role_master);
                    }
                    else
                    {
                        bool isCreated = objUMBAL.CreateRole(um_role_master);
                        ModelState.Clear();
                        if (isCreated)
                        {
                            return Json(new { Success = true });
                        }
                        else
                        {
                            return Json(new { Success = false, ErrorMessage = "Error occured while creation of new user." });
                        }
                    }
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
                return Json(new { Success = false, ErrorMessage = "Error occured while creation of new user." });
            }
            finally
            {
                if (context != null)
                    context.Dispose();
            }
        }


        /// <summary>
        /// Get method to render view for User list
        /// </summary>
        /// <returns></returns>
        [Audit]
        public ActionResult ShowUserList()
        {
            try
            {
                return PartialView("_UserListPartial");
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                throw ex;
            }
        }


        /// <summary>
        /// Populate Users in Grid
        /// </summary>
        /// <param name="homeFormCollection"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult UserList(FormCollection homeFormCollection)
        {
            UserManagerBAL umBAL = new UserManagerBAL();
            long totalRecords;
           
            try
            {
                //Adde By Abhishek kamble 1-May-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(homeFormCollection["page"]), Convert.ToInt32(homeFormCollection["rows"]), homeFormCollection["sidx"], homeFormCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble 1-May-2014 end
                var jsonData = new
                {
                    rows = umBAL.UserList(Convert.ToInt32(homeFormCollection["page"]) - 1,
                                            Convert.ToInt32(homeFormCollection["rows"]),
                                            homeFormCollection["sidx"],
                                            homeFormCollection["sord"], out totalRecords, homeFormCollection["filters"]),
                    total = totalRecords <=
                    Convert.ToInt32(homeFormCollection["rows"]) ? 1 : totalRecords /
                    Convert.ToInt32(homeFormCollection["rows"]) + 1,
                    page = Convert.ToInt32(homeFormCollection["page"]),
                    records = totalRecords
                };
                return Json(jsonData);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                throw ex;
            }
        }


        /// <summary>
        /// Get method to render View for ROle List
        /// </summary>
        /// <returns></returns>
        [Audit]
        public ActionResult ShowRoleList()
        {
            try
            {
                return PartialView("_RoleListPartial");
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                throw ex;
            }
        }


        /// <summary>
        /// Populate Roles in Grid
        /// </summary>
        /// <param name="homeFormCollection"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult RoleList(FormCollection homeFormCollection)
        {
            UserManagerBAL umBAL = new UserManagerBAL();
            long totalRecords;

            try
            {
                //Adde By Abhishek kamble 1-May -2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(homeFormCollection["page"]), Convert.ToInt32(homeFormCollection["rows"]), homeFormCollection["sidx"], homeFormCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble end
                var jsonData = new
                {
                    rows = umBAL.RoleList(Convert.ToInt32(homeFormCollection["page"]) - 1,
                                            Convert.ToInt32(homeFormCollection["rows"]),
                                            homeFormCollection["sidx"],
                                            homeFormCollection["sord"], out totalRecords, homeFormCollection["filters"]),
                    total = totalRecords <=
                    Convert.ToInt32(homeFormCollection["rows"]) ? 1 : totalRecords /
                    Convert.ToInt32(homeFormCollection["rows"]) + 1,
                    page = Convert.ToInt32(homeFormCollection["page"]),
                    records = totalRecords
                };
                return Json(jsonData);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// Get method to render View for menu List
        /// </summary>
        /// <returns></returns>
        [Audit]
        public ActionResult ShowMenuList()
        {
            try
            {
                return PartialView("_MenuListPartial");
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                throw ex;
            }
        }


        /// <summary>
        /// Populate Menus in Grid
        /// </summary>
        /// <param name="homeFormCollection"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult MenuList(FormCollection homeFormCollection)
        {
            UserManagerBAL umBAL = new UserManagerBAL();
            long totalRecords;

            try
            {
                //Adde By Abhishek kamble 1-May -2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(homeFormCollection["page"]), Convert.ToInt32(homeFormCollection["rows"]), homeFormCollection["sidx"], homeFormCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble end
                var jsonData = new
                {
                    rows = umBAL.MenuList(Convert.ToInt32(homeFormCollection["page"]) - 1,
                                            Convert.ToInt32(homeFormCollection["rows"]),
                                            homeFormCollection["sidx"],
                                            homeFormCollection["sord"], out totalRecords, homeFormCollection["filters"]),
                     total = totalRecords <=
                    Convert.ToInt32(homeFormCollection["rows"]) ? 1 : totalRecords /
                    Convert.ToInt32(homeFormCollection["rows"]) + 1,
                    page = Convert.ToInt32(homeFormCollection["page"]),
                    records = totalRecords
                };
                return Json(jsonData);

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                throw ex;
            }
        }


        /// <summary>
        /// Populate Roles with corresponding home pages in Grid
        /// </summary>
        /// <param name="homeFormCollection"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult RoleHomePageList(FormCollection homeFormCollection)
        {
            UserManagerBAL umBAL = new UserManagerBAL();
            String searchParameters = string.Empty;
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            long totalRecords;

            try
            {
                //Adde By Abhishek kamble 1-May -2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(homeFormCollection["page"]), Convert.ToInt32(homeFormCollection["rows"]), homeFormCollection["sidx"], homeFormCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Adde By Abhishek kamble end
                if (!string.IsNullOrEmpty(homeFormCollection["searchField"]))
                {
                    searchParameters = HttpUtility.HtmlDecode(homeFormCollection["searchField"]);
                    searchParameters = searchParameters.Replace("%2F", "/");
                    string[] str = (searchParameters.ToString().Split('&'));
                    for (int i = 0; i < str.Length; ++i)
                    {
                        string[] splitParameter = str[i].Split('=');
                        parameters.Add(splitParameter[0].Trim(), splitParameter[1].Trim());
                    }

                }
                
                var jsonData = new
                {
                    rows = umBAL.RoleHomePageList(Convert.ToInt32(homeFormCollection["page"]) - 1,
                                            Convert.ToInt32(homeFormCollection["rows"]),
                                            homeFormCollection["sidx"],
                                            homeFormCollection["sord"], out totalRecords, homeFormCollection["filters"]),
                    total = totalRecords <=
                    Convert.ToInt32(homeFormCollection["rows"]) ? 1 : totalRecords /
                    Convert.ToInt32(homeFormCollection["rows"]) + 1,
                    page = Convert.ToInt32(homeFormCollection["page"]),
                    records = totalRecords
                };
                return Json(jsonData);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                throw ex;
            }
        }


        /// <summary>
        /// Get methods for RoleHomePage
        /// </summary>
        /// <returns></returns>
        [Audit]
        public ActionResult RoleHomePage()
        {
            UserManagerBAL objUMBAL = new UserManagerBAL();
            db = new PMGSYEntities();
            try
            {
                ViewBag.Operation = "A";
                RoleHomePageModel obj_homepage_master = new RoleHomePageModel();
                obj_homepage_master.RoleList = new List<SelectListItem>(new SelectList(db.UM_Role_Master.Where(c => c.IsActive == true).OrderBy(c => c.RoleName), "RoleID", "RoleName"));
                obj_homepage_master.ModuleList = objUMBAL.GetModules();
                obj_homepage_master.SubModuleList = objUMBAL.GetSubModules(-1);

                return PartialView("_RoleHomePagePartial", obj_homepage_master);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                throw ex;
            }
            finally
            {
                db.Dispose();
            }
        }


        /// <summary>
        /// Post method for setting Role Home Page
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult RoleHomePage(RoleHomePageModel roleHomePageModel)
        {
            UserManagerBAL objUMBAL = new UserManagerBAL();
            try
            {
                if (ModelState.IsValid)
                {
                    string isCreated = objUMBAL.RoleHomePage(roleHomePageModel);

                    if (isCreated.Equals("0"))
                    {
                        return Json(new { Failure = "Home Page with same name already mapped with choosen role." });
                    }
                    else
                    {
                        ModelState.Clear();
                        if (isCreated.Equals("1"))
                        {
                            return Json(new { Success = true });
                        }
                        else
                        {
                            return Json(new { Failure = "Error occured while mapping Home Page." });
                        }
                    }
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
                return Json(new { Success = false, ErrorMessage = "Error occured while mapping Home Page." });
            }
        }

        #endregion


        #region Edit User, Roles, Menus


        /// <summary>
        /// Get method to edit User
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult Edit(String parameter, String hash, String key)
        {
            UserManagerBAL objUMBAL = new UserManagerBAL();
            ViewBag.Operation = "U";
            int userId = 0;
            CommonFilterWrapper commonFilterWrapper = new CommonFilterWrapper();
            db = new PMGSYEntities();
            try
            {

                if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                {
                    String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                    userId = Convert.ToInt16(urlParams[0]);
                }

                UM_User_Master um_user_master = db.UM_User_Master.Find(userId);
                User_Master obj_User_Master = new User_Master();

                if (um_user_master == null)
                {
                    return HttpNotFound();
                }

                obj_User_Master.UserID = Convert.ToInt32(um_user_master.UserID);
                obj_User_Master.UserName = um_user_master.UserName;
                obj_User_Master.LevelID = um_user_master.LevelID;
                //obj_User_Master.RoleID = (from c in db.UM_User_Role_Mapping where c.UserId == obj_User_Master.UserID select c.RoleId).First();
                obj_User_Master.RoleID = (from c in db.UM_User_Master where c.UserID == obj_User_Master.UserID select c.DefaultRoleID).First();
                obj_User_Master.Mast_State_Code = um_user_master.Mast_State_Code;
                obj_User_Master.Mast_District_Code = um_user_master.Mast_District_Code;
                obj_User_Master.Admin_ND_Code = um_user_master.Admin_ND_Code;
                obj_User_Master.PreferedLanguageID = um_user_master.PreferedLanguageID;
                obj_User_Master.PreferedCssID = um_user_master.PreferedCssID;
                obj_User_Master.MaxConcurrentLoginsAllowed = um_user_master.MaxConcurrentLoginsAllowed;
                obj_User_Master.Remarks = um_user_master.Remarks;


                
                obj_User_Master.StateList = commonFilterWrapper.GetStates(0);
                obj_User_Master.DistrictList = commonFilterWrapper.GetDistricts(0, Convert.ToInt32(obj_User_Master.Mast_State_Code));
                obj_User_Master.LevelList = new SelectList(db.UM_Level_Master.Where(c => c.IsActive == true).OrderBy(c => c.LevelName), "LevelID", "LevelName").ToList();
                obj_User_Master.DepartmentList = objUMBAL.GetNDCode(Convert.ToInt32(obj_User_Master.Mast_State_Code == null ? 0 : obj_User_Master.Mast_State_Code), Convert.ToInt32(obj_User_Master.Mast_District_Code == null ? 0 : obj_User_Master.Mast_District_Code), Convert.ToInt32(obj_User_Master.LevelID == null ? 0 : obj_User_Master.LevelID), Convert.ToInt32(obj_User_Master.RoleID == null ? 0 : obj_User_Master.RoleID));
                obj_User_Master.RoleList = objUMBAL.GetRoles(um_user_master.LevelID);               

                if (obj_User_Master.RoleID == 7)
                {
                    obj_User_Master.SQMList = PopulateMonitorsList(true, "S", obj_User_Master.Mast_State_Code.Value);
                    string qmcode = obj_User_Master.UserName.ToString().Substring(2, 5);
                    obj_User_Master.SQMID = Convert.ToInt32(qmcode);
                    obj_User_Master.hiddenSQMId = obj_User_Master.SQMID;
                }
                else
                {
                    obj_User_Master.SQMList = new List<SelectListItem>();
                    obj_User_Master.SQMList.Insert(0, (new SelectListItem { Text = "Select SQM", Value = "0" }));
                }
                
                obj_User_Master.CSSList = new SelectList(db.UM_Css_Master, "CssID", "CssName").ToList();
                obj_User_Master.LanguageList = new SelectList(db.UM_Language_Master, "LanguageID", "LanguageName").ToList();
                return View("Create", obj_User_Master);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json("Error Ocurred While Processing Your Request.");
            }
            finally
            {
                db.Dispose();
            }
        }


        /// <summary>
        /// Post method to update User details
        /// </summary>
        /// <param name="um_user_master"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult Edit(User_Master um_user_master)
        {
            UserManagerBAL objUMBAL = new UserManagerBAL();
            var context = new PMGSYEntities();
            CommonFilterWrapper commonFilterWrapper = new CommonFilterWrapper();
            try
            {
                ViewBag.Operation = "U";
                if (um_user_master.SQMID == 0)
                {
                    ModelState["SQMID"].Errors.Clear();
                }
                else
                {
                    if (um_user_master.RoleID == 7)
                    {
                        if (um_user_master.SQMID != um_user_master.hiddenSQMId)
                        {
                            return Json(new { Success = false, ErrorMessage = "You can not be update User Details" });
                        }
                    }
                }
            
                if (ModelState.IsValid)
                {
                    bool isCreated = objUMBAL.EditUser(um_user_master);

                    if (isCreated)
                    {
                        return Json(new { Success = true });
                    }
                    else
                    { 
                        return Json(new { Success = false, ErrorMessage = "Error Ocurred While Updation of User Details" });
                    }
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
                return Json("Error Ocurred While Processing Your Request.");
            }
            finally
            {
                if (context != null)
                    context.Dispose();
            }
        }




        /// <summary>
        /// Get method to update Roles
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult EditRole(String parameter, String hash, String key)
        {
            UserManagerBAL objUMBAL = new UserManagerBAL();
            ViewBag.Operation = "U";
            int roleId = 0;
            db = new PMGSYEntities();
            try
            {
                if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                {
                    String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                    roleId = Convert.ToInt16(urlParams[0]);
                }

                UM_Role_Master um_role_master = db.UM_Role_Master.Find(roleId);
                Role_Master obj_role_master = new Role_Master();

                if (um_role_master == null)
                {
                    return HttpNotFound();
                }

                obj_role_master.RoleID = um_role_master.RoleID;
                obj_role_master.RoleName = um_role_master.RoleName;
                obj_role_master.IsActive = um_role_master.IsActive;
                obj_role_master.Remark = um_role_master.Remark;
               

                var levelMaster = (from ulm in db.UM_Level_Master
                                   orderby ulm.LevelName
                                   where ulm.IsActive == true
                                   select new
                                   {
                                       Value = ulm.LevelID,
                                       Text = ulm.LevelName
                                   }).ToList();

                
                var assignedLevels = (from urlm in db.UM_Role_Level_Mapping 
                                     join ulm in db.UM_Level_Master on urlm.LevelID equals ulm.LevelID
                                     where urlm.RoleID == um_role_master.RoleID
                                     select ulm.LevelID).Distinct().ToList();
                
                List<SelectListItem> lstLevels = new List<SelectListItem>();
                SelectListItem item = new SelectListItem();
                foreach (var data in levelMaster)
                {
                    item = new SelectListItem();
                    item.Text = data.Text;
                    item.Value = data.Value.ToString();
                    if (assignedLevels.Contains(data.Value))
                        obj_role_master.LevelId = obj_role_master.LevelId + data.Value + "$$"; //set selected values(to hidden variable) for Mutiselect ListBox
                     lstLevels.Add(item);             
                }

                ViewBag.LevelMaster = lstLevels;
                 
                return View("CreateRole", obj_role_master);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json("Error Ocurred While Processing Your Request.");
            }
            finally
            {
                db.Dispose();
            }
        }


        /// <summary>
        /// Post method to update Role master
        /// </summary>
        /// <param name="um_role_master"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult EditRole(Role_Master um_role_master)
        {
            UserManagerBAL objUMBAL = new UserManagerBAL();
            var context = new PMGSYEntities();
            try
            {
                ViewBag.Operation = "U";
                if (ModelState.IsValid)
                {
                    ViewBag.LevelMaster = new SelectList(context.UM_Level_Master.Where(c => c.IsActive == true).OrderBy(c => c.LevelName), "LevelID", "LevelName");

                    bool isCreated = objUMBAL.EditRole(um_role_master);

                    if (isCreated)
                    {
                        return Json(new { Success = true });
                    }
                    else
                    {
                        return Json("Error Ocurred While Processing Your Request.");
                    }
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
                return Json("Error Ocurred While Processing Your Request.");
            }
            finally
            {
                if (context != null)
                    context.Dispose();
            }
        }




        /// <summary>
        /// Get method to update Home page for corresponding roles
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult EditRoleHomePage(String parameter, String hash, String key)
        {
            UserManagerBAL objUMBAL = new UserManagerBAL();
            ViewBag.Operation = "U";
            int Id = 0;
            db = new PMGSYEntities();
            try
            {
                if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                {
                    String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                    Id = Convert.ToInt16(urlParams[0]);
                }

                UM_HomePage_Master um_homepage_master = db.UM_HomePage_Master.Find(Id);
                RoleHomePageModel obj_homepage_master = new RoleHomePageModel();

                if (um_homepage_master == null)
                {
                    return HttpNotFound();
                }

                obj_homepage_master.ID = Convert.ToInt16(um_homepage_master.Id);
                obj_homepage_master.RoleID = um_homepage_master.RoleId;

                var moduleMaster = (from uhm in db.UM_HomePage_Master
                                    join uam in db.UM_Action_Master on uhm.HomePageId equals uam.ActionID
                                    join umm in db.UM_Module_Master on uam.ModuleID equals umm.ModuleID
                                    where uhm.HomePageId == um_homepage_master.HomePageId
                                    select umm).First();

                if (moduleMaster.ParentID == 0)
                    obj_homepage_master.ModuleID = moduleMaster.ModuleID;
                else
                    obj_homepage_master.ModuleID = moduleMaster.ParentID;

 
                obj_homepage_master.SubModuleID = moduleMaster.ModuleID;

                List<SelectListItem> lstRoles = new List<SelectListItem>(new SelectList(db.UM_Role_Master.Where(c => c.IsActive == true).OrderBy(c => c.RoleName), "RoleID", "RoleName", obj_homepage_master.RoleID));
                obj_homepage_master.RoleList = lstRoles;
                obj_homepage_master.ModuleList = new List<SelectListItem>(new SelectList(db.UM_Module_Master.OrderBy(c => c.ModuleName), "ModuleID", "ModuleName"));
                obj_homepage_master.SubModuleList = objUMBAL.GetSubModules(obj_homepage_master.ModuleID);

                return PartialView("_RoleHomePagePartial", obj_homepage_master);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                string errorMsg = string.Format("Errors: {0}", ex.Message);
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = 500;
                Response.Write(errorMsg);
                return null;
            }
            finally
            {
                db.Dispose();
            }
        }


        /// <summary>
        /// Post method to update Role Home Page
        /// </summary>
        /// <param name="um_role_master"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult EditRoleHomePage(RoleHomePageModel obj_homepage_master)
        {
            UserManagerBAL objUMBAL = new UserManagerBAL();
            var context = new PMGSYEntities();
            try
            {
                ViewBag.Operation = "U";
                if (ModelState.IsValid)
                {
                    bool isCreated = objUMBAL.EditRoleHomePage(obj_homepage_master);

                    if (isCreated)
                    {
                        return Json(new { Success = true });
                    }
                    else
                    {
                        return Json(new { Failure = "Error occured while updation of Home Page." });
                    }
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
                return Json("Error Ocurred While Processing Your Request.");
            }
            finally
            {
                if (context != null)
                    context.Dispose();
            }
        }



        /// <summary>
        /// Lock Unlock User functionality
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [Audit]
        public JsonResult LockUnLockUser(String parameter, String hash, String key)
        {
            UserManagerBAL objUMBAL = new UserManagerBAL();
            ViewBag.Operation = "U";
            int id = 0;
            bool isLocked = false;
            db = new PMGSYEntities();
            try
            {
                if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                {
                    String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                    id = Convert.ToInt16(urlParams[0]);
                }
            
                int tempUserId = Convert.ToInt32(id);

                UM_User_Master user = db.UM_User_Master.Single(e => e.UserID == tempUserId);
                if (user.DefaultRoleID == 6 || user.DefaultRoleID == 7)
                {
                    string IsEmpannled = db.ADMIN_QUALITY_MONITORS.Where(x => x.ADMIN_USER_ID == tempUserId).Select(x => x.ADMIN_QM_EMPANELLED).FirstOrDefault();
                    if (IsEmpannled == "N" && IsEmpannled != null)
                    {
                        if (user.IsLocked == true)
                        {
                            return Json(new { success = "User is De-Empanelled, It Can't be unlocked." }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            user.IsLocked = true;
                            isLocked = true;
                        }
                        db.SaveChanges();
                    }
                    else
                    {
                        if (user.IsLocked == true)
                        {
                            user.IsLocked = false;
                            isLocked = false;
                            user.FailedPasswordAttempts = 0;
                        }
                        else
                        {
                            user.IsLocked = true;
                            isLocked = true;
                        }
                        db.SaveChanges();
                    }
                }
                else
                {
                    if (user.IsLocked == true)
                    {
                        user.IsLocked = false;
                        isLocked = false;
                        user.FailedPasswordAttempts = 0;
                    }
                    else
                    {
                        user.IsLocked = true;
                        isLocked = true;
                    }

                    db.SaveChanges();
                }
                //if (user.IsLocked == true)
                //{
                //    user.IsLocked = false;
                //    isLocked = false;
                //    //Added By Abhishek kamble 26-Apr 2014 To Reset Failed Attempted to 0
                //    user.FailedPasswordAttempts = 0;   
                //}
                //else
                //{
                //    user.IsLocked = true;
                //    isLocked = true;
                //}

                //db.SaveChanges();

                if (isLocked)
                {
                    return Json(new { success="User locked successfully" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = "User unlocked successfully" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (DbEntityValidationException dbEx)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(dbEx, HttpContext.ApplicationInstance.Context);
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                    }
                }
                return Json(new { success = "Error ocurred while processing your request." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = "Error ocurred while processing your request." }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                db.Dispose();
            }
        }



        /// <summary>
        /// Activate-Deactivate User functionality
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [Audit]
        public JsonResult UpdateActiveStatus(String parameter, String hash, String key)
        {
            UserManagerBAL objUMBAL = new UserManagerBAL();
            ViewBag.Operation = "U";
            int id = 0;
            bool isActive = false;
            db = new PMGSYEntities();
            try
            {
                if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                {
                    String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                    id = Convert.ToInt16(urlParams[0]);
                }

                int tempUserId = Convert.ToInt32(id);

                UM_User_Master user = db.UM_User_Master.Single(e => e.UserID == tempUserId);

                if (user.DefaultRoleID == 6 || user.DefaultRoleID == 7)
                {
                    string IsEmpannlled = db.ADMIN_QUALITY_MONITORS.Where(x => x.ADMIN_USER_ID == tempUserId).Select(x => x.ADMIN_QM_EMPANELLED).FirstOrDefault();
                    if (IsEmpannlled == "N" && IsEmpannlled != null)
                    {
                        if (user.IsActive == false)
                        {
                            return Json(new { success = "User is De-Empannlled, It Can't Activate" }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            user.IsActive = false;
                            isActive = false;
                        }
                        db.SaveChanges();
                    }
                    else
                    {
                        if (user.IsActive == true)
                        {
                            user.IsActive = false;
                            isActive = false;
                        }
                        else
                        {
                            user.IsActive = true;
                            isActive = true;
                            user.FailedPasswordAttempts = 0;
                        }
                        db.SaveChanges();
                    }
                }
                else
                {
                    if (user.IsActive == true)
                    {
                        user.IsActive = false;
                        isActive = false;
                    }
                    else
                    {
                        user.IsActive = true;
                        isActive = true;
                        user.FailedPasswordAttempts = 0;
                    }
                    db.SaveChanges();
                }
                //if (user.IsActive == true)
                //{
                //    user.IsActive = false;
                //    isActive = false;
                //}
                //else
                //{
                //    user.IsActive = true;
                //    isActive = true;
                //    user.FailedPasswordAttempts = 0;
                //}

                
                //db.SaveChanges();

                if (isActive)
                {
                    return Json(new { success = "User activated successfully" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = "User deactivated successfully" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (DbEntityValidationException dbEx)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(dbEx, HttpContext.ApplicationInstance.Context);
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                    }
                }
                return Json(new { success = "Error ocurred while processing your request." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = "Error ocurred while processing your request." }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                db.Dispose();
            }
        }


        /// <summary>
        /// Populate partial view with User profile details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult GetUserProfile(String id)
        {
            UserManagerBAL umBAL = new UserManagerBAL();
            UserProfileModel upModel = new UserProfileModel();
            try
            {
                if (id.Equals("0"))
                {
                    return null;
                }
                else
                {
                    upModel = umBAL.GetUserProfile(Convert.ToInt32(id));
                }

                return PartialView("_UserProfilePartial", upModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }



#endregion



        /// <summary>
        /// Get for Independent UserName Mapping
        /// </summary>
        /// <returns></returns>
        [Audit]
        public ActionResult IndependentUsersMapping(string id)
        {
            UserManagerBAL objUMBAL = new UserManagerBAL();
            CommonFunctions objCommonFunctions = new CommonFunctions();
            db = new PMGSYEntities();
            try
            {
                string [] strUser = id.Split('$');
                int UserId = Convert.ToInt32(strUser[0]);               
                int StateCode =strUser[1]==""?0:Convert.ToInt32(strUser[1]);
                ViewBag.Operation = "A";
                IndependentUsersModel indpendentUsersModel =  new IndependentUsersModel();
                indpendentUsersModel.UserID = Convert.ToInt32(strUser[0]);
                var userDetails = (from umm in db.UM_User_Master
                                   where umm.UserID == indpendentUsersModel.UserID
                                   select umm).First();

                string userType = string.Empty;
                if(userDetails.DefaultRoleID == 6) //NQM
                {
                    userType = "I";

                }else if(userDetails.DefaultRoleID == 7) //SQM
                {
                    userType = "S";

                }else if(userDetails.DefaultRoleID == 8) //SQC
                {
                    userType = "S";     //temporarily Taken, needs to be replace as in database

                }else if(userDetails.DefaultRoleID == 3) //STA
                {
                    userType = "S";

                }else if(userDetails.DefaultRoleID == 15) //PTA
                {
                    userType = "P";
                }


                if (userDetails.DefaultRoleID == 6 || userDetails.DefaultRoleID == 7) //NQM, SQM
                {
                    var existingUser = (from aqm in db.ADMIN_QUALITY_MONITORS
                                           where aqm.ADMIN_QM_TYPE == userType
                                           && aqm.ADMIN_USER_ID == indpendentUsersModel.UserID
                                           select aqm).FirstOrDefault();

                    if (existingUser != null)
                    {
                        indpendentUsersModel.UserProfileID = existingUser.ADMIN_QM_CODE;
                    }
                }
                else if (userDetails.DefaultRoleID == 3 || userDetails.DefaultRoleID == 15)        //STA
                {
                    var existingUser = (from ata in db.ADMIN_TECHNICAL_AGENCY
                                           where ata.ADMIN_TA_TYPE == userType
                                           && ata.ADMIN_USER_ID == indpendentUsersModel.UserID
                                        select ata).FirstOrDefault();

                    if (existingUser != null)
                    {
                        indpendentUsersModel.UserProfileID = existingUser.ADMIN_TA_CODE;
                    }
                }
                else if (userDetails.DefaultRoleID == 8) //SQC
                {
                    var existingUser = (from aqm in db.ADMIN_SQC
                                        where aqm.ADMIN_QC_TYPE == userType
                                        && aqm.ADMIN_USER_ID == indpendentUsersModel.UserID
                                        select aqm).FirstOrDefault();

                    if (existingUser != null)
                    {
                        indpendentUsersModel.UserProfileID = existingUser.ADMIN_QC_CODE;
                    }
                }

                //var independentUserId = (from umm in db.UM_User_Master
                //                         where umm.UserID == indpendentUsersModel.UserID
                //                         select umm).FirstOrDefault();


                indpendentUsersModel.UsersList = objUMBAL.GetUserProfileNames(userDetails.DefaultRoleID,StateCode);
                indpendentUsersModel.StateList= objCommonFunctions.PopulateStates(false);
                indpendentUsersModel.StateList.Insert(0, (new SelectListItem { Text = "All", Value = "0", Selected = true }));
               
                indpendentUsersModel.Mast_State = StateCode;
                //if (StateCode > 0) 
                //{
                //    indpendentUsersModel.StateList.Find(x => x.Value == StateCode.ToString()).Selected = true;
                //}
                indpendentUsersModel.UserRoleID = userDetails.DefaultRoleID;
                return PartialView("_IndependentUsersPartial", indpendentUsersModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
            finally
            {
                db.Dispose();
            }
        }



        /// <summary>
        /// Save Independent UserName Mapping
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult IndependentUsersMapping(IndependentUsersModel independentUsersModel)
        {
            UserManagerBAL objUMBAL = new UserManagerBAL();
            var context = new PMGSYEntities();
            try
            {
                ViewBag.Operation = "A";
               
                if (ModelState.IsValid)
                {

                    string isMapped = objUMBAL.IndependentUsersMapping(independentUsersModel);
                    
                    if (isMapped.Equals("duplicate"))
                    {
                        return Json(new { Success = false, ErrorMessage = "Username already mapped with independent user. Please choose other Username." });
                    }
                    else if (isMapped.Equals("mapped"))
                    {
                        return Json(new { Success = "Username mapped successfully." });
                    }
                    else 
                    {
                        return Json(new { Success = false, ErrorMessage = "Error occured while mapping Username." });
                    }
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
                return Json(new { Success = "Error occured while mapping Username." });
            }
            finally
            {
                if (context != null)
                    context.Dispose();
            }
        }


        /// <summary>
        /// Populate Independent Users of Specific Role
        /// </summary>
        /// <returns></returns>
        [Audit]
        public ActionResult GetUserProfileNames(int selectedRole, string query)
        {
            List<SelectListItem> lstUserProfNames = new List<SelectListItem>();
            try
            {
                SelectListItem item = new SelectListItem();
                if (selectedRole == 0)
                {
                    return Json(lstUserProfNames);
                }
                else
                {
                    UserManagerBAL objUMBAL = new UserManagerBAL();
                    lstUserProfNames = objUMBAL.GetUserProfileNames(selectedRole,0);
                    return Json(lstUserProfNames.ToArray(), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }


        [Audit]
        public ActionResult GetStateWiseUserProfileNames(int selectedRole, int selectedState)
        {
            List<SelectListItem> lstUserProfNames = new List<SelectListItem>();
            try
            {
                SelectListItem item = new SelectListItem();
                if (selectedRole == 0)
                {
                    return Json(lstUserProfNames);
                }
                else
                {
                    UserManagerBAL objUMBAL = new UserManagerBAL();
                    lstUserProfNames = objUMBAL.GetUserProfileNames(selectedRole,selectedState);
                    return Json(lstUserProfNames.ToArray(), JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }

        ///// <summary>
        ///// Get For Set Session of Particular User for Admin 
        ///// </summary>
        ///// <returns></returns>
        //public ActionResult SwitchAdminAsUser()
        //{
        //    SwitchAdminAsUserModel switchAdminAsUser = new SwitchAdminAsUserModel();
        //    var dbContext = new PMGSYEntities();
        //    try
        //    {
        //        switchAdminAsUser.UsersList = new SelectList(dbContext.UM_User_Master.OrderBy(c => c.UserName), "UserID", "UserName").ToList(); ;
        //        return View(switchAdminAsUser);
        //    }
        //    catch (Exception)
        //    {
        //        return null;
        //    }
        //    finally
        //    {
        //        dbContext.Dispose();
        //    }
        //}


        /// <summary>
        /// Set Session of Particular User for Admin 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult SwitchAdminAsUser()
        {
            try
            {
                int userId = Convert.ToInt32(Request.Params["id"]);
                Login login = new Login();
                string isSessionInitialized = login.SetSessionForAdmin(userId);

                if (isSessionInitialized.Equals(string.Empty))
                {
                    return Redirect("/Login/RedirectToHome");
                }
                else
                {
                    return Redirect("/Login/Error"); ; 
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Redirect("/Login/Error"); 
            }
        }



        /// <summary>
        /// Reset Password as UserName
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [Audit]
        public JsonResult ResetPassword(String parameter, String hash, String key)
        {
            UserManagerBAL objUMBAL = new UserManagerBAL();
            int id = 0;
            int updateStatus = 0;
            db = new PMGSYEntities();
            try
            {
                if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                {
                    String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                    id = Convert.ToInt16(urlParams[0]);
                }

                int tempUserId = Convert.ToInt32(id);

                UM_User_Master userMaster = db.UM_User_Master.Find(tempUserId);

                userMaster.Password = new Login().EncodePassword(userMaster.UserName);
                userMaster.IsFirstLogin = true;
                db.Entry(userMaster).State = System.Data.Entity.EntityState.Modified;
                updateStatus = db.SaveChanges();

                if (updateStatus > 0)
                {
                    return Json(new { success = "Password reset successfully" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = "Error ocurred while resetting password." }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (DbEntityValidationException dbEx)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(dbEx, HttpContext.ApplicationInstance.Context);
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                    }
                }
                return Json(new { success = "Error ocurred while processing your request." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = "Error ocurred while processing your request." }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                db.Dispose();
            }
        }

        #region Login Report
        /// <summary>
        /// Display User Log Details 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Audit]
        public ActionResult ShowUserLogDetails()
        {
            CommonFunctions objCommonFunctions = new CommonFunctions();
            UserManagerDAL objUMDAL = new UserManagerDAL();

            List<SelectListItem> ddState = objCommonFunctions.PopulateStates(false);
            ddState.Insert(0, new SelectListItem
            {
                Selected = true,
                Text = "All State",
                Value = "0"
            });
            List<SelectListItem> ddRole = objUMDAL.GetUserRoles();
            ddRole.Insert(0, (new SelectListItem { Text = "Select Role", Value = "0",Selected=true }));

            List<SelectListItem> ddUser = new List<SelectListItem>();
            SelectListItem allUSer = new SelectListItem
            {
                Selected = true,
                Text = "All User",
                Value = "0"
            };
            ddUser.Insert(0, allUSer);
            List<SelectListItem> ddYear = objUMDAL.PopulateLoginYears(true).ToList();
            ddYear.Find(x => x.Value == DateTime.Now.Year.ToString()).Selected = true;

             List<SelectListItem> ddMonth = objCommonFunctions.PopulateMonths(true).ToList();
             ddMonth.Find(x => x.Value == DateTime.Now.Month.ToString()).Selected = true;
          
            
            ViewData["STATE"] = ddState;
            ViewData["ROLE"] = ddRole;
            ViewData["USER"] = ddUser;
            ViewData["YEAR"] = ddYear;
            ViewData["MONTH"] = ddMonth;

            return PartialView("_UserLogListPartial");
        }

        /// <summary>
        /// Display User Log Listing Details 
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UserLogReportListing(FormCollection formCollection)
        {
            UserManagerBAL objUMBAL = new UserManagerBAL();

            //Adde By Abhishek kamble 1-May -2014 start
            using (CommonFunctions commonFunction = new CommonFunctions())
            {
                if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(formCollection["page"]), Convert.ToInt32(formCollection["rows"]), formCollection["sidx"], formCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                {
                    return null;
                }
            }
            //Adde By Abhishek kamble end
            int roleCode = Convert.ToInt32(formCollection["RoleCode"]);
            int stateCode = Convert.ToInt32(formCollection["StateCode"]);
            int userCode = Convert.ToInt32(formCollection["UserCode"]);
            int year = Convert.ToInt32(formCollection["Year"]);
            int month = Convert.ToInt32(formCollection["Month"]);
            int page = Convert.ToInt32(formCollection["page"]) - 1;
            int rows = Convert.ToInt32(formCollection["rows"]);
            string sidx = formCollection["sidx"];
            string sord = formCollection["sord"];
            int totalRecords;
            string filter="";
            var jsonData = new
            {
                rows = objUMBAL.UserLogListingBAL(roleCode,stateCode,userCode,year, month, page, rows, sidx, sord, out totalRecords, filter),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords
            };

            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public ActionResult PopulateRoleWiseUSer(FormCollection frmCollection)
        {
            UserManagerDAL objUMDAL = new UserManagerDAL();

            List<SelectListItem> list = objUMDAL.GetRolewiseUser(Convert.ToInt32(frmCollection["RoleCode"]), Convert.ToInt32(frmCollection["StateCode"]));
            // list.Find(x => x.Value == "-1").Value = "0";
            list.Insert(0, new SelectListItem
            {
                Selected = true,
                Text = "All User",
                Value = "0"
            });
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        #endregion
        #region Log Module Access Report
        /// <summary>
        /// Display User Log Details 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Audit]
        public ActionResult ShowUserLogAccessDetails()
        {
            CommonFunctions objCommonFunctions = new CommonFunctions();
            UserManagerDAL objUMDAL = new UserManagerDAL();
            List<SelectListItem> ddModule = objUMDAL.GetModules();
            ddModule.Insert(0, (new SelectListItem { Text = "Select Module", Value = "0", Selected = true }));

            List<SelectListItem> ddYear = objUMDAL.PopulateLoginYears(true).ToList();
            ddYear.Find(x => x.Value == DateTime.Now.Year.ToString()).Selected = true;

            List<SelectListItem> ddMonth = objCommonFunctions.PopulateMonths(true).ToList();
            ddMonth.Find(x => x.Value == DateTime.Now.Month.ToString()).Selected = true;

            ViewData["MODULE"] = ddModule;
            ViewData["YEAR"] = ddYear;
            ViewData["MONTH"] = ddMonth;
            return PartialView("_UserLogAccessListPartial");
        }

        /// <summary>
        /// Display User Log Listing Details 
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UserLogAccessReportListing(FormCollection formCollection)
        {
            UserManagerBAL objUMBAL = new UserManagerBAL();

            int moduleCode = Convert.ToInt32(formCollection["ModuleCode"]);
            int year = Convert.ToInt32(formCollection["Year"]);
            int month = Convert.ToInt32(formCollection["Month"]);
            int page = Convert.ToInt32(formCollection["page"]) - 1;
            int rows = Convert.ToInt32(formCollection["rows"]);
            string sidx = formCollection["sidx"];
            string sord = formCollection["sord"];
            int totalRecords;
            string filter = "";
            var jsonData = new
            {
                rows = objUMBAL.UserLogAccessListingBAL(moduleCode, year, month, page, rows, sidx, sord, out totalRecords, filter),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords
            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        #endregion
        // ----------------------Elmah Error Section --------------
       // Added By Rohit Jadhav 9-May-2014
        [HttpGet]
        [Audit]
        public ActionResult ShowElmahErrorLog()
        {
            CommonFunctions objCommonFunctions = new CommonFunctions();
            UserManagerDAL objUMDAL = new UserManagerDAL();

            List<SelectListItem> ddYear = objUMDAL.PopulateLoginYears(true).ToList();
            ddYear.Find(x => x.Value == DateTime.Now.Year.ToString()).Selected = true;



            List<SelectListItem> ddMonth = objCommonFunctions.PopulateMonths(true).ToList();
            ddMonth.Find(x => x.Value == DateTime.Now.Month.ToString()).Selected = true;


            ViewData["YEAR"] = ddYear;
            ViewData["MONTH"] = ddMonth;


            return PartialView("ShowElmahErrorLog");


        }


        [HttpPost]

        public ActionResult ErrorList(FormCollection formCollection)
        {
            UserManagerBAL objUMBAL = new UserManagerBAL();

            int year = Convert.ToInt32(formCollection["Year"]);
            int month = Convert.ToInt32(formCollection["Month"]);
            int page = Convert.ToInt32(formCollection["page"]) - 1;
            int rows = Convert.ToInt32(formCollection["rows"]);
            string sidx = formCollection["sidx"];
            string sord = formCollection["sord"];
            int totalRecords;
            string filter = "";
            var jsonData = new
            {
                rows = objUMBAL.ErrorBal(year, month, page, rows, sidx, sord, out totalRecords, filter),
                total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                page = page + 1,
                records = totalRecords
            };
            var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
            
        }



        [HttpPost]
        public ActionResult DeleteErrorRecord(String id)
        {
            try
            {
                UserManagerBAL objBAL = new UserManagerBAL();
               

                
                    if (!objBAL.DeleteErrorRecord(id))
                    {
                        ModelState.AddModelError(string.Empty, "You can not delete this record.");

                        return Json(new { success = false, message = "You can not delete this record." }, JsonRequestBehavior.AllowGet);

                    }
               
                return Json(new { success = true, message = "Record deleted successfully." }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, message = "You can not delete this record." }, JsonRequestBehavior.AllowGet);
            }

        }

        // Error Elmah section Ends 



        #region PROPOSAL_ND_CODE_UPDATE

        public ActionResult ListDistrictUsers()
        {
            ProposalUpdateViewModel model = new ProposalUpdateViewModel();
            try
            {
                CommonFunctions objCommon = new CommonFunctions();
                model.lstStates = objCommon.PopulateStates(true);
                model.lstDistricts = objCommon.PopulateDistrict(0, false);
                return View(model);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public ActionResult GetDistrictUserList(int? page, int? rows, string sidx, string sord)
        {
            try
            {
                int State = Convert.ToInt32(Request.Params["State"]);
                int District = Convert.ToInt32(Request.Params["District"]);
                int Agency = Convert.ToInt32(Request.Params["Agency"]);
                UserManagerBAL objUMBAL = new UserManagerBAL();
                long totalRecords;
                var jsonData = new
                {
                    rows = objUMBAL.GetDistrictUserListBAL(page, rows, sidx, sord, out totalRecords,State,District,Agency),
                    total = totalRecords <= rows ? 1 : (totalRecords % rows) == 0 ? (totalRecords /rows) : (totalRecords / rows + 1),
                    page = page,
                    records = totalRecords
                };
                return Json(jsonData);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public ActionResult GetProposalPIUList(int? page, int? rows, string sidx, string sord)
        {
            try
            {
                int State = Convert.ToInt32(Request.Params["State"]);
                int District = Convert.ToInt32(Request.Params["District"]);
                int Agency = Convert.ToInt32(Request.Params["Agency"]);
                UserManagerBAL objUMBAL = new UserManagerBAL();
                long totalRecords;
                var jsonData = new
                {
                    rows = objUMBAL.GetProposalPIUListBAL(page, rows, sidx, sord, out totalRecords, State, District, Agency),
                    total = totalRecords <= rows ? 1 : (totalRecords % rows) == 0 ? (totalRecords / rows) : (totalRecords / rows + 1),
                    page = page,
                    records = totalRecords
                };
                return Json(jsonData);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public ActionResult GetDistrictsByState(int state)
        {
            CommonFunctions objCommon = new CommonFunctions();
            try
            {
                return Json(new SelectList(objCommon.PopulateDistrict(state, false), "Value", "Text"), JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public ActionResult GetAgenciesByState(int state)
        {
            CommonFunctions objCommon = new CommonFunctions();
            try
            {
                return Json(new SelectList(objCommon.PopulateAgencies(state, false), "Value", "Text"), JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public ActionResult GetPIUOfDistrict(int district,int agency)
        {
            UserManagerDAL objUMDAL = new UserManagerDAL();
            try
            {
                return Json(new SelectList(objUMDAL.PopulateDPIUOfDistrict(district,agency), "Value", "Text"), JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return null;
            } 
        }

        [HttpPost]
        public ActionResult ChangeProposalPIUMapping()
        {
            try
            {
                int NEW_PIU_CODE = 0;
                int OLD_PIU_CODE = 0;
                int state = 0;
                int district = 0;
                UserManagerBAL objUMBAL = new UserManagerBAL();
                if (!String.IsNullOrEmpty(Request.Params["OLD_PIU_CODE"]))
                {
                    OLD_PIU_CODE = Convert.ToInt32(Request.Params["OLD_PIU_CODE"]);
                }

                if (!String.IsNullOrEmpty(Request.Params["NEW_PIU_CODE"]))
                {
                    NEW_PIU_CODE = Convert.ToInt32(Request.Params["NEW_PIU_CODE"]);
                }

                if (!String.IsNullOrEmpty(Request.Params["State"]))
                {
                    state = Convert.ToInt32(Request.Params["State"]);
                }

                if (!String.IsNullOrEmpty(Request.Params["District"]))
                {
                    district = Convert.ToInt32(Request.Params["District"]);
                }

                bool success = objUMBAL.ChangeProposalPIUMapping(OLD_PIU_CODE,NEW_PIU_CODE,state,district);

                if (success == true)
                {
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false });
                }

                
            }
            catch (Exception)
            {
                return Json(new { success = false});
            }
        }


        #endregion


        /// <summary>
        /// Get method to render view for User list
        /// </summary>
        /// <returns></returns>
        [Audit]
        public ActionResult ShowListforITNO()
        {
            try
            {
                return View("ShowListforITNO");
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                throw ex;
            }
        }

        /// <summary>
        /// Get method to render view for User list
        /// Added by Sammed Patil 
        /// </summary>
        /// <returns></returns>
        [Audit]
        public ActionResult getITNOUserList(FormCollection homeFormCollection)
        {
            UserManagerBAL umBAL = new UserManagerBAL();
            long totalRecords;

            try
            {
                
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(homeFormCollection["page"]), Convert.ToInt32(homeFormCollection["rows"]), homeFormCollection["sidx"], homeFormCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                
                var jsonData = new
                {
                    rows = umBAL.ITNOUserListingBAL(Convert.ToInt32(homeFormCollection["page"]) - 1,
                                            Convert.ToInt32(homeFormCollection["rows"]),
                                            homeFormCollection["sidx"],
                                            homeFormCollection["sord"], out totalRecords, homeFormCollection["filters"]),
                    total = totalRecords <=
                    Convert.ToInt32(homeFormCollection["rows"]) ? 1 : totalRecords /
                    Convert.ToInt32(homeFormCollection["rows"]) + 1,
                    page = Convert.ToInt32(homeFormCollection["page"]),
                    records = totalRecords
                };
                return Json(jsonData);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                throw ex;
            }
        }
        /// <summary>
        ///  Poulate Sqm Monitors
        /// </summary>
        /// <param name="isPopulateFirstSelect"></param>
        /// <param name="qmType"></param>
        /// <param name="stateCode"></param>
        /// <returns></returns>
        public JsonResult PopulateMonitors(bool isPopulateFirstSelect, string qmType, int stateCode)
        {
            List<SelectListItem> lstProfileNames = new List<SelectListItem>();
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                SelectListItem item = new SelectListItem();
                if (isPopulateFirstSelect)
                {
                    item.Text = "Select Monitor";
                    item.Value = "0";
                    item.Selected = true;
                    lstProfileNames.Add(item);
                }
                else 
                {
                    item.Text = "All Monitors";
                    item.Value = "0";
                    item.Selected = true;
                    lstProfileNames.Add(item);
                }
                var query = (from c in dbContext.ADMIN_QUALITY_MONITORS
                             where c.ADMIN_QM_TYPE == qmType
                             && c.ADMIN_QM_EMPANELLED == "Y" &&
                             //c.ADMIN_USER_ID != null &&
                             c.ADMIN_USER_ID == null &&
                             ((stateCode == 0 ? 1 : c.MAST_STATE_CODE) == (stateCode == 0 ? 1 : stateCode))
                             select new
                             {
                                 Value = c.ADMIN_QM_CODE,
                                 Text = (c.ADMIN_QM_LNAME.Equals(null) ? "" : c.ADMIN_QM_LNAME.Trim() + " ") + (c.ADMIN_QM_FNAME.Equals(null) ? "" : c.ADMIN_QM_FNAME.Trim() + " ") + (c.ADMIN_QM_MNAME.Equals(null) ? "" : c.ADMIN_QM_MNAME.Trim())
                             }).OrderBy(c => c.Text.Trim()).Distinct().ToList();

                query = query.OrderBy(c => c.Text).ToList();
                foreach (var data in query)
                {
                    item = new SelectListItem();
                    item.Text = data.Text;
                    item.Value = data.Value.ToString();
                    lstProfileNames.Add(item);
                }
                return Json(lstProfileNames, JsonRequestBehavior.AllowGet);
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                    }
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public List<SelectListItem> PopulateMonitorsList(bool isPopulateFirstSelect, string qmType, int stateCode)
        {
            List<SelectListItem> lstProfileNames = new List<SelectListItem>();
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {
                SelectListItem item = new SelectListItem();
                if (isPopulateFirstSelect)
                {
                    item.Text = "Select Monitor";
                    item.Value = "0";
                    item.Selected = true;
                    lstProfileNames.Add(item);
                }
                else
                {
                    item.Text = "All Monitors";
                    item.Value = "0";
                    item.Selected = true;
                    lstProfileNames.Add(item);
                }
                var query = (from c in dbContext.ADMIN_QUALITY_MONITORS
                             where c.ADMIN_QM_TYPE == qmType
                             && c.ADMIN_QM_EMPANELLED == "Y" &&
                             c.ADMIN_USER_ID != null &&
                             ((stateCode == 0 ? 1 : c.MAST_STATE_CODE) == (stateCode == 0 ? 1 : stateCode))
                             select new
                             {
                                 Value = c.ADMIN_QM_CODE,
                                 Text = (c.ADMIN_QM_LNAME.Equals(null) ? "" : c.ADMIN_QM_LNAME.Trim() + " ") + (c.ADMIN_QM_FNAME.Equals(null) ? "" : c.ADMIN_QM_FNAME.Trim() + " ") + (c.ADMIN_QM_MNAME.Equals(null) ? "" : c.ADMIN_QM_MNAME.Trim())
                             }).OrderBy(c => c.Text.Trim()).Distinct().ToList();

                query = query.OrderBy(c => c.Text).ToList();
                foreach (var data in query)
                {
                    item = new SelectListItem();
                    item.Text = data.Text;
                    item.Value = data.Value.ToString();
                    lstProfileNames.Add(item);
                }
                return lstProfileNames;
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                    }
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        [HttpPost]
        public ActionResult GetSQMUserName(int selectedState,int selectedSQM,string qmType)
        {
            PMGSYEntities dbContext = new PMGSYEntities();
            try
            {

                string strUser = string.Empty;
                var shotStateCode = (from c in dbContext.MASTER_STATE
                                    where c.MAST_STATE_CODE == selectedState
                                    select c.MAST_STATE_SHORT_CODE.ToLower()).FirstOrDefault();
                var strEmpanlledyear = (from c in dbContext.ADMIN_QUALITY_MONITORS
                             where c.ADMIN_QM_TYPE == qmType
                             && c.ADMIN_QM_EMPANELLED == "Y" &&
                            // c.ADMIN_USER_ID != null &&
                             c.ADMIN_QM_CODE == selectedSQM  // &&
                             //c.MAST_STATE_CODE==SelectedState
                             select c.ADMIN_QM_EMPANELLED_YEAR).FirstOrDefault();
                string lenSqmCode = selectedSQM.ToString().Length.ToString();
                switch (lenSqmCode)
                {
                    case "1":
                        strUser = shotStateCode.ToString() + "0000" + selectedSQM + "" + strEmpanlledyear.ToString().Substring(strEmpanlledyear.ToString().Length - 2);
                        break;
                    case "2":
                        strUser = shotStateCode.ToString() + "000" + selectedSQM + "" + strEmpanlledyear.ToString().Substring(strEmpanlledyear.ToString().Length - 2);
                        break;
                    case "3":
                        strUser = shotStateCode.ToString() + "00" + selectedSQM + "" + strEmpanlledyear.ToString().Substring(strEmpanlledyear.ToString().Length - 2);
                        break;
                    case "4":
                        strUser = shotStateCode.ToString() + "0" + selectedSQM + "" + strEmpanlledyear.ToString().Substring(strEmpanlledyear.ToString().Length - 2);
                        break;
                    case "5":
                        strUser = shotStateCode.ToString() + "" + selectedSQM + "" + strEmpanlledyear.ToString().Substring(strEmpanlledyear.ToString().Length - 2);
                        break;
                    default:
                        break;
                }

                return Json(new { success = true,data=strUser }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { success = false, message = "You can not delete this record." }, JsonRequestBehavior.AllowGet);
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }


        #region Account Refresh Data 

        public ActionResult AccountRefreshDataLayout()
        {
            return View("AccountRefreshDataLayout");

        }

        public ActionResult AccountRefreshDataPostMethod(AccountRefreshData model)
        {
           UserManagerBAL objUMBAL = new UserManagerBAL();
           bool status = objUMBAL.RefreshAccountData(model.Report);
               

            return Json(new { status = status });

        }


        #endregion Account Refresh Data 
        
        #region Test Mail


        public ActionResult TestMail()
        {


            return View();
        }

        [HttpPost]
        public ActionResult SendTestMail()
        {
            try
            {
                //String toEmail="kabhishek.cdac.in";
                //String ccEmail = "ommashelp@gmail.com";
                
                String toEmail=Request.Params["ToEmail"];
                String ccEmail = Request.Params["CcEmail"];

                //var mailMessage = new Mvc.Mailer.MvcMailMessage
                //{
                //    Subject = "This is Test email." 
                //};
                //mailMessage.To.Add(toEmail);

                //if (ccEmail != String.Empty && ccEmail != null)
                //{
                //    mailMessage.CC.Add(ccEmail);
                //}
                //mailMessage.Body = "This is Test email.";
                //mailMessage.Send();
                //return Json(new {status=true,message="Email Send Successfully..."}) ;


                //---------------

            

                //Edited by Shreyas on 09-03-2023
                ServicePointManager.ServerCertificateValidationCallback += (s, ce, ca, p) => true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;// | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                MailMessage mailMessage = new MailMessage();
               
                mailMessage.Subject = "This is Test email.";
               
                SmtpClient client = new SmtpClient();
                //client.Host = "smtp.cdac.in";
                //client.Port = 587;
                //client.Host = "relay.nic.in";
                //client.Port = 25;


                mailMessage.To.Add(toEmail);

                if (ccEmail != String.Empty && ccEmail != null)
                {
                    mailMessage.CC.Add(ccEmail);
                }
                mailMessage.IsBodyHtml = true;
                mailMessage.Body = "This is Test email.";

                client.UseDefaultCredentials = false;
                client.EnableSsl = true;

                string e_EuthMailUserName =ConfigurationManager.AppSettings["e_EuthMailUserName"];
                string e_EuthMailPassword =ConfigurationManager.AppSettings["e_EuthMailPassword"];
                client.Credentials = new NetworkCredential(e_EuthMailUserName, e_EuthMailPassword);
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.Send(mailMessage);

                //mailMessage.Send();
                return Json(new { status = true, message = "Email Send Successfully..." });
            }
            catch (Exception ex)
            {
                return Json(new { status = false, message = "An Error occured While processing Your Request." });
            }
        }

        #endregion Test Mail

        #region DATA_DIFFERENCE

        /// <summary>
        /// returns the filters along with the results of Proposal Data difference
        /// </summary>
        /// <returns></returns>
        public ActionResult ListProposalDataGap()
        {
            try
            {
                return View();
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// returns the list of reports along with the count of proposals
        /// </summary>
        /// <param name="homeFormCollection"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetProposalDataGapList(FormCollection homeFormCollection)
        {
            UserManagerBAL umBAL = new UserManagerBAL();
            long totalRecords;

            try
            {
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(Convert.ToInt32(homeFormCollection["page"]), Convert.ToInt32(homeFormCollection["rows"]), homeFormCollection["sidx"], homeFormCollection["sord"], Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                
                List<USP_PROPDATAGAP_REPORT_Result> rows = db.USP_PROPDATAGAP_REPORT(1, 0, 0, 0, 0, 0, 0, 0, "%", "%", "%", 1).ToList();

                var result = rows.Select(m => new { 
                
                    m.RptNo,
                    m.RptName,
                    m.Proposals
                
                }).ToArray();


                var array = rows.Select(m => new { 

                    id=m.RptNo.ToString(),
                    cell = new[]{
                
                    m.RptNo.ToString(),
                    m.RptName.ToString(),
                    m.Proposals.ToString()
                    
                    }
                
                }).ToArray();

                totalRecords = rows.Count();

                var jsonData = new
                {
                    rows = array,
                    total = totalRecords <= Convert.ToInt32(homeFormCollection["rows"]) ? 1 : totalRecords /
                   Convert.ToInt32(homeFormCollection["rows"]) + 1,
                    page = Convert.ToInt32(homeFormCollection["page"]),
                    records = totalRecords
                };
                return Json(jsonData);

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                throw ex;
            }
        }

        /// <summary>
        /// returns the list of proposals for viewing details
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        public ActionResult GetProposalDataGapDetailsList(FormCollection formCollection)
        {
            UserManagerDAL objDAL = new UserManagerDAL();

            try
            {
                int page = Convert.ToInt32(formCollection["page"]) - 1;
                int rows = Convert.ToInt32(formCollection["rows"]);
                int rptNo = Convert.ToInt32(formCollection["RptNo"]);
                string sidx = formCollection["sidx"];
                string sord = formCollection["sord"];
                long totalRecords;
               
                var jsonData = new
                {
                    rows = objDAL.GetProposalDataGapDetailsList(page, rows, sidx, sord, out totalRecords,rptNo,formCollection),
                    total = totalRecords <= Convert.ToInt32(formCollection["rows"]) ? 1 : (totalRecords % Convert.ToInt32(formCollection["rows"]) == 0 ? totalRecords / Convert.ToInt32(formCollection["rows"]) : (totalRecords / Convert.ToInt32(formCollection["rows"]) + 1)),
                    page = page + 1,
                    records = totalRecords
                };
                var jsonResult = Json(jsonData, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            
            }
            catch (Exception)
            {
                return null;
            }
        }

        #endregion

    }


    /// <summary>
    /// Class Required for Filtering in JqGrid
    /// </summary>
    public class SearchJsonString
    {
        public string groupOp { get; set; }
        public List<SearchRules> rules { get; set; }
    }

    /// <summary>
    ///  Class Required for Filtering in JqGrid
    /// </summary>
    public class SearchRules
    {
        public string field { get; set; }
        public string op { get; set; }
        public string data { get; set; }
    }
}