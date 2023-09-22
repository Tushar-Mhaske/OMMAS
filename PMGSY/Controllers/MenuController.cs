
#region file header

/*  * Name         :   MenuController.cs
     * Path         :   ~\Controller\MenuController
     * Description  :   Action methods for menu action mapping, menu role mapping,adding menu,creating menu etc are handled here.
     * Methods      :   GetMenuRoleMapping();
     *                  GetMenuLevelMapping();
     *                  AddMenu();
     *                      
    * Author        :   Amol U.Jadhav
    * Organization  :   CDAC, Pune - EGOV
    * Modified      :   06/07/2013
    * Modified By   :   Shyam Yadav
 */

#endregion




using PMGSY.Common;
using PMGSY.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PMGSY.Models.Menu;
using PMGSY.BAL.Menu;
using System.Data.Entity;
using System.Data.Entity.Validation;
using PMGSY.Models.UserManager;
using PMGSY.BAL.User_Manager;
using System.Text;

namespace PMGSY.Controllers
{
    [RequiredAuthentication]
    public class MenuController : Controller
    {
        private PMGSYEntities dbContext = null;

        public MenuController()
        { 
          dbContext = new PMGSYEntities();
        }


        #region menu role mapping
        [HttpGet]
        [Audit]
        public JsonResult GetMenuItems()
        {
            return Json(String.Empty);
        }
        
        
        
        /// <summary>
        /// Purpose :   Get a list of Menu Items at a level. Parent = 0 -> child = 1 -> child = 2 etc
        /// Called  :   By loadGrid() in the jQuery change event of ddlRoles in RoleActionMapping.js file. Also in the deleteMenuItem() and addMenuItem()
        /// Author  :  Amol U. Jadhav
        /// </summary>
        /// <param name="page">Total number of pages in the grid</param>
        /// <param name="rows">Total number of rows per page currently selected by user</param>
        /// <param name="sidx">The field on which the records are to be sorted</param>
        /// <param name="sord">The order of sort i.e., ascending or descending</param>
        /// <returns>JsonResult - list of menuitems</returns>
       [HttpPost]
        [Audit]
        public JsonResult GetMenuItems(int? page, int? rows, string sidx, string sord)
        {
           // SessionDTO objSessionDTO = (SessionDTO)Session["User"];
            RoleActionMappingDTO objRAMapDTO = new RoleActionMappingDTO();
            IMenuBAL objMenuMapBAL = new MenuBAL();
            String[] strPathParams = null;
            try
            {
                //Added By Abhishek kamble 1-May-2014 start
                using (CommonFunctions commonFunction = new CommonFunctions())
                {
                    if (!commonFunction.ValidateGridParameters(new GridParams(page, rows, sidx, sord, Convert.ToBoolean(Request.Params["_search"]), Convert.ToInt64(Request.Params["nd"]))))
                    {
                        return null;
                    }
                }
                //Added By Abhishek kamble 1-May-2014 end

                strPathParams = Request.Url.AbsolutePath.Split('/');
                if (strPathParams != null && strPathParams.Length >= 4)
                {
                    objRAMapDTO.RoleId = Convert.ToInt16(strPathParams[3]);
                }

                //nodeid is a field that has been internally created and set by the treegrid to indicate the id of the node that has been expanded
                if (!Request.Form.AllKeys.Contains("nodeid") || Request.Form["nodeid"].Equals(""))
                {
                    if (strPathParams != null && strPathParams.Length >= 5)
                    {
                        objRAMapDTO.MenuId = Convert.ToInt32(strPathParams[4]);
                    }
                }
                else
                {
                    objRAMapDTO.MenuId = Convert.ToInt32(Request.Form["nodeid"]);
                }

                List<RoleActionListDTO> lstMenuItems = objMenuMapBAL.GetMenuItems(objRAMapDTO);
                var jsonData = new
                {
                    rows = objMenuMapBAL.GetJSONMenuItemCollection(lstMenuItems)
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
        /// Purpose :   Used to Map a Menu Item with a role
        /// Called  :   By addMenuItem() in the jQuery click event of Add Link in the TreeGrid in RoleActionMapping.js file
        /// Author  :   Amol U. Jadhav
        /// </summary>
        /// <param name="id1">The encrypted string of RoleId with MenuId </param>
        /// <param name="hash">hash</param>
        /// <param name="key">key</param>
        /// <returns>JsonResult - 1: if role and menu item were successfully mapped; 2: if parent menu item not mapped</returns>
        [HttpPost]
       [Audit]
       public JsonResult MapRoleWithMenuItem(String parameter, String hash, String key)
        {
           // SessionDTO objSessionDTO = (SessionDTO)Session["User"];
            RoleActionMappingDTO objRAMapDTO = new RoleActionMappingDTO();
            IMenuBAL objMenuMapBAL = new MenuBAL();
            int intReturnVal = 0;
            try
            {
                if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                {
                    String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                    if (urlParams.Length >= 1)
                    {
                        String[] urlSplitParams = urlParams[0].Split('$');
                        if (urlSplitParams.Length >= 3)
                        {
                            objRAMapDTO.RoleId = Convert.ToInt16(urlSplitParams[0]);
                            objRAMapDTO.MenuId = Convert.ToInt32(urlSplitParams[1]);
                            objRAMapDTO.AddChildren = urlSplitParams[2].Equals("Y") ? true : false;
                        }
                    }
                }

                intReturnVal = objMenuMapBAL.MapRoleWithMenuItem(objRAMapDTO);
                return Json(intReturnVal.ToString());
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
        /// Purpose :   Used to Unmap a Menu Item with a role
        /// Called  :   By deleteMenuItem() in the jQuery click event of Add Link in the TreeGrid in RoleActionMapping.js file
        /// Author  :   Amol U.Jadhav
        /// </summary>
        /// <param name="id1">The encrypted string of RoleId with MenuId </param>
        /// <param name="hash">hash</param>
        /// <param name="key">key</param>
        /// <returns>JsonResult - 1: if role and menu item(s) were successfully unmapped</returns>
        [HttpPost]
        [Audit]
        public JsonResult UnmapMenuFromRole(String parameter, String hash, String key)
        {
            //SessionDTO objSessionDTO = (SessionDTO)Session["User"];
            RoleActionMappingDTO objRAMapDTO = new RoleActionMappingDTO();
            IMenuBAL objMenuMapBAL = new MenuBAL();
            int intReturnVal = 0;
            try
            {
                if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                {
                    String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                    if (urlParams.Length >= 1)
                    {
                        String[] urlSplitParams = urlParams[0].Split('$');
                        if (urlSplitParams.Length >= 2)
                        {
                            objRAMapDTO.RoleId = Convert.ToInt16(urlSplitParams[0]);
                            objRAMapDTO.MenuId = Convert.ToInt32(urlSplitParams[1]);
                        }
                    }
                }

                intReturnVal = objMenuMapBAL.DeleteMenuItems(objRAMapDTO);
                return Json(intReturnVal.ToString());
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
        ///Purpose: get method for menu role mapping
        ///Called :by menu item click
        /// </summary>
        /// <returns> action result </returns>
        [Audit]
        public ActionResult GetMenuRoleMapping(string id)
        {
            try
            {
                //List<SelectListItem> listRoles = new List<SelectListItem>();
                
                //var lstRoles = (from item in dbContext.UM_Role_Master
                //               where item.IsActive == true
                //               select item).OrderByDescending(item => item.RoleName);

                //foreach (var item in lstRoles)
                //{
                //    listRoles.Insert(0, new SelectListItem() { Text = item.RoleName, Value = item.RoleID.ToString(), Selected = false });
                //}

                //listRoles.Insert(0, new SelectListItem() { Text = "--Select--", Value = "0", Selected = true });

                //ViewData["ddlRole"] = listRoles;

                RoleActionMappingDTO roleActionMappingdto = new RoleActionMappingDTO();
                roleActionMappingdto.RoleId = Convert.ToInt16(id);
                return View(roleActionMappingdto);
            }

            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                throw ex;

            }
        }


        #endregion

        #region Menu level mapping 
        
       

        /// <summary>
        /// action menthod to return action for menu level mapping 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Audit]
        public ActionResult GetMenuLevelMapping(string id)
        {
            dbContext = new PMGSYEntities();
            MenuLevelCombinationListDTO menuActionMappingdto = new MenuLevelCombinationListDTO();
            menuActionMappingdto.MenuId = Convert.ToInt16(id);
            var levelMaster = (from ulm in dbContext.UM_Level_Master
                               orderby ulm.LevelName
                               where ulm.IsActive == true
                               select new
                               {
                                   Value = ulm.LevelID,
                                   Text = ulm.LevelName
                               }).ToList();


            var assignedLevels = (from umm in dbContext.UM_Menu_Master
                                  join ulc in dbContext.UM_Level_Combination on umm.LevelGroupCode equals ulc.LevelGroupCode
                                  where umm.MenuID == menuActionMappingdto.MenuId
                                  select ulc.LevelID).Distinct().ToList();

            List<SelectListItem> lstLevels = new List<SelectListItem>();
            SelectListItem item = new SelectListItem();
            foreach (var data in levelMaster)
            {
                item = new SelectListItem();
                item.Text = data.Text;
                item.Value = data.Value.ToString();
                if (assignedLevels.Contains(data.Value))
                    menuActionMappingdto.LevelStr = menuActionMappingdto.LevelStr + data.Value + "$$"; //set selected values(to hidden variable) for Mutiselect ListBox
                lstLevels.Add(item);
            }

            ViewBag.LevelMaster = lstLevels;
            return View(menuActionMappingdto);
            //return View();
        }





        //[HttpPost]
        // public ActionResult GetMenuItemDetails(string id)
        //{
        //    String[] param = id.Split('$');
        //    Int16 MenuId = Convert.ToInt16(param[0]);
        //    Int32 Level = Convert.ToInt32(param[1]);
        //    IMenuBAL objMenuMapBAL = new MenuBAL();
        //    List<MenuLevelCombinationDTO> lstMenuItems = new List<MenuLevelCombinationDTO>();
        //    lstMenuItems = objMenuMapBAL.GetMenuItemsById(MenuId, Level);
        //    ViewData["MenuItemDetails"] = lstMenuItems;
        //    return PartialView();
        //}


        /// <summary>
        /// Add level Group Code to UM_Level_Combination
        /// </summary>
        /// <param name="frmCollect"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public String AddLevelCombination(MenuLevelCombinationListDTO menuLevelCombinationDto)
        {

           // SessionDTO objSessionDTO = Session["User"] as SessionDTO;
            string validationSummary = string.Empty;
            IMenuBAL objMenuMapBAL = new MenuBAL();
            try
            {
                 if (validationSummary.Equals(string.Empty))
                {
                    return objMenuMapBAL.AddLevelCombination(menuLevelCombinationDto).ToString();
                }
                else
                {
                    throw new Exception(validationSummary);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                string errorMsg = string.Format("Errors: {0}", ex.Message);
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = 500;
                Response.Write(errorMsg);
                return string.Empty;
            }
        }


        
       

        #endregion


        #region Menu Creation, Rights

        [HttpGet]
        [Audit]
        public ActionResult CreateMenu()
        {
            var context = new PMGSYEntities();
            ViewBag.Operation = "A";
            try
            {
                Menu_Master menuMasterModel = new Menu_Master();
                menuMasterModel.ParentID = -1;
                menuMasterModel.Sequence = 1;
                menuMasterModel.MenuCombinationLevelList1 = 0;
                menuMasterModel.MenuCombinationLevelList2 = 0;
                menuMasterModel.MenuCombinationLevelList3 = 0;

                List<SelectListItem> lstMenus = new List<SelectListItem>(new SelectList(context.UM_Menu_Master.Where(c => c.IsActive == true).OrderBy(c => c.MenuName), "MenuID", "MenuName"));
                lstMenus.Insert(0, new SelectListItem { Value = "-1", Text = "Self" });
                menuMasterModel.MenuList = lstMenus;
                ViewBag.LevelGroupList = new SelectList(context.UM_Level_Master.Where(c => c.IsActive == true).OrderBy(c => c.LevelName), "LevelID", "LevelName");

                List<SelectListItem> lstCombiantion = new List<SelectListItem>();
                lstCombiantion.Insert(0, new SelectListItem { Value = "0", Text = "Select Combination Level" });

                menuMasterModel.MenuCombinationList1 = lstCombiantion;  // GetMenuLevelCombinationList();
                menuMasterModel.MenuCombinationList2 = lstCombiantion;  //GetMenuLevelCombinationList();
                menuMasterModel.MenuCombinationList3 = lstCombiantion;  //GetMenuLevelCombinationList();

                return View(menuMasterModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                throw ex;
            }
            
        }


        /// <summary>
        /// Post to Create Menu
        /// </summary>
        /// <param name="um_menu_master"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult CreateMenu(Menu_Master um_menu_master)
        {
            MenuBAL objMenuBAL = new MenuBAL();
            var context = new PMGSYEntities();
            
            try
            {
                if (ModelState.IsValid)
                {
                    if (context.UM_Menu_Master.Any(u => u.MenuName == um_menu_master.MenuName.Trim()))
                    {
                        return Json(new { Success = false, ErrorMessage = "Menu with same name already exists, Please choose different Menu Name." });
                    }
                    else
                    {
                        bool isCreated = objMenuBAL.CreateMenu(um_menu_master);
                        ModelState.Clear();

                        if (isCreated)
                        {
                            return Json(new { Success = true }); //RedirectToAction("Create");
                        }
                        else
                        {
                            return Json(new { Success = false, ErrorMessage = "Error occured while creation of new menu." });
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
                return Json(new { Success = false, ErrorMessage = "Error occured while creation of new menu." });
            }
            finally
            {
                if (context != null)
                    context.Dispose();
            }
        }





        /// <summary>
        /// Get List of Menus for Combination Dropdown
        /// </summary>
        /// <returns></returns>
        
        public List<SelectListItem> GetMenuLevelCombinationList(Int32 selectedMenu)
        {
            var context = new PMGSYEntities();
            try
            {
                List<SelectListItem> lstMenus = new List<SelectListItem>(new SelectList(context.UM_Menu_Master.Where(c => c.IsActive == true && c.ParentID == selectedMenu).OrderBy(c => c.MenuName), "MenuID", "MenuName"));
                lstMenus.Insert(0, new SelectListItem { Value = "0", Text = "Select Combination Level" });
                return lstMenus;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
            finally
            {
                if (context != null)
                    context.Dispose();
            }
        }


        public List<SelectListItem> GetParentMenuList(Int32 selectedMenu)
        {
            var context = new PMGSYEntities();
            try
            {
                List<SelectListItem> lstMenus = new List<SelectListItem>(new SelectList(context.UM_Menu_Master.Where(c => c.IsActive == true && c.MenuID == selectedMenu).OrderBy(c => c.MenuName), "MenuID", "MenuName"));
                return lstMenus;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
            finally
            {
                if (context != null)
                    context.Dispose();
            }
        }

        [Audit]
        public JsonResult PopulateParentMenu(Int32 selectedMenu)
        {
            var context = new PMGSYEntities();
            try
            {
                List<SelectListItem> lstMenus = new List<SelectListItem>(new SelectList(context.UM_Menu_Master.Where(c => c.IsActive == true && c.MenuID == selectedMenu).OrderBy(c => c.MenuName), "MenuID", "MenuName"));
                return Json(lstMenus);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
            finally
            {
                if (context != null)
                    context.Dispose();
            }
        }


        [Audit]
        public JsonResult PopulateMenuListForCombination(Int32 selectedMenu)
        {
            var context = new PMGSYEntities();
            try
            {
                return Json(GetMenuLevelCombinationList(selectedMenu));
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
            finally
            {
                if (context != null)
                    context.Dispose();
            }
        }

        /// <summary>
        /// Get method for returning view for User & Role List in Update menu Rights
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Audit]
        public ActionResult MenuRights(string id)
        {
            var context = new PMGSYEntities();
            MenuBAL umBAL = new MenuBAL();
            try
            {
                UserRoleMapping userRoleMappingModel = new UserRoleMapping();
                userRoleMappingModel.UserID = Convert.ToInt32(id);
                ViewBag.RoleList = umBAL.GetUserRoleList(userRoleMappingModel.UserID);
                return View(userRoleMappingModel);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
            finally
            {
                if (context != null)
                    context.Dispose();
            }
        }


        /// <summary>
        /// Populate Roles of particular user
        /// </summary>
        /// <param name="selectedUser"></param>
        /// <returns></returns>
        [Audit]
        public JsonResult GetUserRoles(int selectedUser)
        {
            MenuBAL nenuBAL = new MenuBAL();
            List<SelectListItem> lstRoles = new List<SelectListItem>();
            try
            {
                SelectListItem item = new SelectListItem();

                if (selectedUser == 0)
                {
                    return Json(lstRoles);
                }
                else
                {
                    lstRoles = nenuBAL.GetUserRoleList(selectedUser);
                    return Json(lstRoles);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }


        /// <summary>
        /// Returns Json of Menu Rights of particular User 
        /// </summary>
        /// <param name="id1"></param>
        /// <param name="id2"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult GetMenuRights(int id1, int id2, FormCollection homeFormCollection)
        {
            MenuBAL umBAL = new MenuBAL();
            Menu_Rights menuRightsModel = new Menu_Rights();
            String searchParameters = string.Empty;
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            long totalRecords;
            try
            {
               
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
                    rows = umBAL.GetMenuRights(id1, id2, Convert.ToInt32(homeFormCollection["page"]) - 1,
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
                throw ex;
            }
        }

        /// <summary>
        /// Update menu rights for particular user
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult UpdateMenuRights(String parameter, String hash, String key)
        {
            var context = new PMGSYEntities();
            Menu_Rights objMenuRights = new Menu_Rights();
            MenuBAL menuBAL = new MenuBAL();
            int intReturnVal = 0;
            try
            {
                if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                {
                    String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                    if (urlParams.Length >= 1)
                    {
                        String[] urlSplitParams = urlParams[0].Split('$');
                        if (urlSplitParams.Length >= 3)
                        {
                            objMenuRights.UserID = Convert.ToInt16(urlSplitParams[0]);
                            objMenuRights.RoleID = Convert.ToInt16(urlSplitParams[1]);
                            objMenuRights.MenuID = Convert.ToInt32(urlSplitParams[2]);
                            objMenuRights.IsAdd = urlSplitParams[3].Equals("Yes") ? true : false;
                            objMenuRights.IsEdit = urlSplitParams[4].Equals("Yes") ? true : false;
                            objMenuRights.IsDelete = urlSplitParams[5].Equals("Yes") ? true : false;

                        }
                    }
                }

                intReturnVal = menuBAL.UpdateMenuRights(objMenuRights);
                return Json(intReturnVal.ToString());
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
        /// Get for User Role Mapping
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Audit]
        public ActionResult UserRoleMapping(string id)
        {
            var context = new PMGSYEntities();
            MenuBAL menuBAL = new MenuBAL();
            try
            {
                UserRoleMapping objUserRoleMapping = new UserRoleMapping();
                objUserRoleMapping.UserID = Convert.ToInt32(id);


                var roleMaster = (from ulm in dbContext.UM_Role_Master
                                   where ulm.IsActive == true
                                   select new
                                   {
                                       Value = ulm.RoleID,
                                       Text = ulm.RoleName
                                   }).OrderBy(c => c.Text).ToList();


                var assignedRoles = (from uurm in dbContext.UM_User_Role_Mapping
                                      where uurm.UserId == objUserRoleMapping.UserID
                                      select uurm.RoleId).Distinct().ToList();

                List<SelectListItem> lstRoles = new List<SelectListItem>();
                SelectListItem item = new SelectListItem();
                foreach (var data in roleMaster)
                {
                    item = new SelectListItem();
                    item.Text = data.Text;
                    item.Value = data.Value.ToString();
                    if (assignedRoles.Contains(data.Value))
                        objUserRoleMapping.RoleStr = objUserRoleMapping.RoleStr + data.Value + "$$"; //set selected values(to hidden variable) for Mutiselect ListBox
                    lstRoles.Add(item);
                }

                ViewBag.RoleList = lstRoles; //menuBAL.GetUserLevelwiseRoleList(objUserRoleMapping.UserID);
                return View(objUserRoleMapping);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
            finally
            {
                if (context != null)
                    context.Dispose();
            }
        }


        /// <summary>
        /// Save User Role Mapping
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        public ActionResult UserRoleMapping(UserRoleMapping objUserRoleMapping)
        {
            MenuBAL menuBAL = new MenuBAL();
            try
            {
                if (ModelState.IsValid)
                {
                    bool isUpdated = menuBAL.UpdateUserRoleMapping(objUserRoleMapping);
                    if (isUpdated)
                    {
                        return Json(new { Success = true });
                    }
                    else
                    {
                        return Json(new { Success = false, ErrorMessage = "Error occured while updation of user-role mapping." });
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
                return Json(new { Success = false, ErrorMessage = "Error occured while updation of user-role mapping." });
            }
           

        }


        /// <summary>
        /// Get role list for specific level for specific user
        /// </summary>
        /// <param name="selectedUser"></param>
        /// <returns></returns>
        [Audit]
        public List<SelectListItem> GetUserLevelwiseRoleList(int selectedUser)
        {
            MenuBAL menuBAL = new MenuBAL();
            List<SelectListItem> lstRoles = new List<SelectListItem>();
            try
            {
                SelectListItem item = new SelectListItem();

                if (selectedUser == 0)
                {
                    return lstRoles;
                }
                else
                {
                    lstRoles = menuBAL.GetUserLevelwiseRoleList(selectedUser);
                    return lstRoles;
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return null;
            }
        }


        /// <summary>
        /// Get Method to display Edit Menu Screen
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [Audit]
        public ActionResult EditMenu(String parameter, String hash, String key)
        {
            MenuBAL objMenuBAL = new MenuBAL();
            var context = new PMGSYEntities();
            ViewBag.Operation = "U";
            int menuId = 0;
            try
            {
                if (!String.IsNullOrEmpty(parameter) && !String.IsNullOrEmpty(hash) && !String.IsNullOrEmpty(key))
                {
                    String[] urlParams = URLEncrypt.DecryptParameters(new String[] { parameter, hash, key });
                    menuId = Convert.ToInt16(urlParams[0]);
                }

                UM_Menu_Master um_menu_master = context.UM_Menu_Master.Find(menuId);
                Menu_Master obj_menu_master = new Menu_Master();

                if (um_menu_master == null)
                {
                    return HttpNotFound();
                }

                obj_menu_master.MenuID = um_menu_master.MenuID;
                obj_menu_master.MenuName = um_menu_master.MenuName;
                obj_menu_master.Sequence = um_menu_master.Sequence;
                obj_menu_master.HorizontalSequence = um_menu_master.HorizontalSequence;
                obj_menu_master.ParentID = um_menu_master.ParentID;
                obj_menu_master.VerticalLevel = um_menu_master.VerticalLevel.ToString();
                obj_menu_master.IsActive = um_menu_master.IsActive;
                obj_menu_master.MenucombinationCode = um_menu_master.MenucombinationCode;

                //if (obj_menu_master.ParentID == 0)
                //{
                    List<SelectListItem> menuList = new List<SelectListItem>(new SelectList(context.UM_Menu_Master.Where(c => c.IsActive == true).OrderBy(c => c.MenuName), "MenuID", "MenuName"));
                    menuList.Insert(0, new SelectListItem { Value = "-1", Text = "Self", Selected = true });
                    obj_menu_master.MenuList = menuList;
                //}
                //else
                //{
                //    obj_menu_master.MenuList = new SelectList(context.UM_Menu_Master.Where(c => c.IsActive == true).OrderBy(c => c.MenuName), "MenuID", "MenuName").ToList();
                //}

                //Set ViewBag to populate all levels
                ViewBag.LevelGroupList = new SelectList(context.UM_Level_Master.Where(c => c.IsActive == true).OrderBy(c => c.LevelName), "LevelID", "LevelName");


                //Code to get already assigned levels in Group Code
                //-----------------------------------------------------------------------------------------------------
                var levelGroupCode = (from ulc in context.UM_Level_Combination
                                      join umm in context.UM_Menu_Master on ulc.LevelGroupCode equals umm.LevelGroupCode
                                      where umm.MenuID == obj_menu_master.MenuID
                                      select new { ulc.LevelID }).ToList();

                foreach (var data in levelGroupCode)
                {
                    obj_menu_master.LevelGroupCode = obj_menu_master.LevelGroupCode + data.LevelID + "$$"; //set selected values(to hidden variable) for Mutiselect ListBox
                }

                //-----------------------------------------------------------------------------------------------------


                //split MenuCombinationCode & set is accordingly as selected
                //-----------------------------------------------------------------------------------------------------
                List<string> menuCombinationList = new List<string>(um_menu_master.MenucombinationCode.Split(','));
                menuCombinationList.RemoveAt(3);  //Remove last elemnt as it is Current MenuId Itself
               
                List<string> actualParentsList = new List<string>();
                foreach (var item in menuCombinationList)
                {
                    if (!item.Equals("0"))
                    {
                        actualParentsList.Add(item);
                    }
                }

                List<SelectListItem> lstCombiantion = new List<SelectListItem>();
                lstCombiantion.Insert(0, new SelectListItem { Value = "0", Text = "Select Combination Level" });

                //means no parent
                if (actualParentsList.Count == 0)
                {
                    obj_menu_master.MenuCombinationList1 = lstCombiantion; 
                    obj_menu_master.MenuCombinationList2 = lstCombiantion; 
                    obj_menu_master.MenuCombinationList3 = lstCombiantion; 
                }
                else if (actualParentsList.Count == 1)
                {
                    obj_menu_master.MenuCombinationLevelList1 = Convert.ToInt32(actualParentsList[0]);

                    obj_menu_master.MenuCombinationList1 = GetParentMenuList(obj_menu_master.ParentID);
                    obj_menu_master.MenuCombinationList2 = lstCombiantion; // lstMenus2;
                    obj_menu_master.MenuCombinationList3 = lstCombiantion; // lstMenus3;
                }
                else if (actualParentsList.Count == 2)
                {
                    obj_menu_master.MenuCombinationLevelList1 = Convert.ToInt32(actualParentsList[0]);
                    obj_menu_master.MenuCombinationLevelList2 = Convert.ToInt32(actualParentsList[1]);

                    obj_menu_master.MenuCombinationList1 = GetParentMenuList(obj_menu_master.ParentID);  
                    obj_menu_master.MenuCombinationList2 = GetMenuLevelCombinationList(Convert.ToInt32(actualParentsList[0]));
                    obj_menu_master.MenuCombinationList3 = lstCombiantion; 

                }
                else if (actualParentsList.Count == 3)
                {
                    obj_menu_master.MenuCombinationLevelList1 = Convert.ToInt32(actualParentsList[0]);
                    obj_menu_master.MenuCombinationLevelList2 = Convert.ToInt32(actualParentsList[1]);
                    obj_menu_master.MenuCombinationLevelList3 = Convert.ToInt32(actualParentsList[2]);

                    obj_menu_master.MenuCombinationList1 = GetParentMenuList(obj_menu_master.ParentID);
                    obj_menu_master.MenuCombinationList2 = GetMenuLevelCombinationList(Convert.ToInt32(actualParentsList[0]));
                    obj_menu_master.MenuCombinationList3 = GetMenuLevelCombinationList(Convert.ToInt32(actualParentsList[1])); 
                }
                
                
                //-----------------------------------------------------------------------------------------------------
                

                return View("CreateMenu", obj_menu_master);
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
        }



        /// <summary>
        /// Post to Edit Menu
        /// </summary>
        /// <param name="um_menu_master"></param>
        /// <returns></returns>
        [HttpPost]
        [Audit]
        [ValidateAntiForgeryToken]
        public ActionResult EditMenu(Menu_Master um_menu_master)
        {
            MenuBAL objMenuBAL = new MenuBAL();
            var context = new PMGSYEntities();

            ViewBag.Operation = "U";
            ViewBag.LevelMaster = new SelectList(context.UM_Level_Master.Where(c => c.IsActive == true).OrderBy(c => c.LevelName), "LevelID", "LevelName");

            try
            {
                if (ModelState.IsValid)
                {
                    bool isCreated = objMenuBAL.EditMenu(um_menu_master);

                    if (isCreated)
                    {
                        return Json(new { Success = true });
                    }
                    else
                    {
                        return Json(new { Success = false, ErrorMessage = "Error occured while updation of menu." });
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

                return View("CreateMenu", um_menu_master);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.ApplicationInstance.Context);
                return Json(new { Success = false, ErrorMessage = "Error occured while updation of menu." });
            }
            finally
            {
                if (context != null)
                    context.Dispose();
            }
        }



        #endregion
    }
}
