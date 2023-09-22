using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PMGSY.Models.Menu;
using PMGSY.Models;
using System.Web.Mvc;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Resources;

namespace PMGSY.DAL.Menu
{
    public class MenuDAL : IMenuDAL
    {
       
         private PMGSYEntities dbContext = null;

       public MenuDAL()
        { 
          dbContext = new PMGSYEntities();
        }
        
        #region menu role mapping

        /// <summary>
        /// Purpose :   Used to get a list of Menu Items given the parent menu item id and role id
        /// Called  :   By GetMenuItems(...) in the ~\BLL\UserManager\RoleActionMappingBAL.cs
        /// Author  :   Amol U.Jadhav
        /// </summary>
        /// <param name="objDetails">An object of type RoleActionMappingDTO holding the parent menu item id, role id etc</param>
        /// <returns>List of ROleACtionListDTO</returns>
        public List<RoleActionListDTO> GetMenuItems(RoleActionMappingDTO objDetails)
        {
            List<RoleActionListDTO> lstMenuItems = new List<RoleActionListDTO>();
            List<sp_get_menuitems_Result> List = null;

            try
            {

                //get the list of the menus for selected parent menu and selected role(to get status if menu is already mapped) 
                List = dbContext.sp_get_menuitems(objDetails.MenuId, objDetails.RoleId).ToList<sp_get_menuitems_Result>();

                foreach (sp_get_menuitems_Result item in List)
                {
                    RoleActionListDTO objListDTO = new RoleActionListDTO();
                    objListDTO.MenuId = Convert.ToInt32(item.MenuID);
                    objListDTO.Description = item.MenuName.ToString();
                    objListDTO.Active = item.isactive.ToString();
                    objListDTO.ParentId = Convert.ToInt32(item.ParentID);
                    objListDTO.IsLeaf = Convert.ToBoolean(item.isleaf.ToString());
                    lstMenuItems.Add(objListDTO);

                }


                return lstMenuItems;
            }
            catch (Exception Ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                throw Ex;
            }
            finally {
                dbContext.Dispose();
            }
            
        }


        /// <summary>
        /// Purpose :   Used to Map a Role with a Menu Item
        /// Called  :   By MapRoleWithMenuItem(...) in ~\BLL\UserManager\RoleActionMappingBAL.cs
        /// Author  :   Amol U.Jadhav
        /// </summary>
        /// <param name="objDetails">Object of type ROleActionMappingDTO holding the menu id and the role id to map</param>
        /// <returns>Integer</returns>
        public int MapRoleWithMenuItem(RoleActionMappingDTO objDetails)
        {
            int? result = 0;
            
            try
            {
               //get the result of the mapping              
                result = dbContext.um_map_menu_role(objDetails.RoleId, Convert.ToInt16(objDetails.MenuId), objDetails.AddChildren).FirstOrDefault(); 
                return result.HasValue?result.Value:0; 
            }
            catch (Exception Ex)
            {

                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                throw Ex;
            }
            finally
            {
                dbContext.Dispose();
            }
           
        }


      
        /// <summary>
        /// Purpose :   Used to Unmap a Role with Menu Item(s)
        /// Called  :   By DeleteMenuItems(...) in ~\BLL\UserManager\RoleActionMappingBAL.cs
        /// Author  :   Amol U.Jadhav
        /// </summary>
        /// <param name="objDetails">Object of type RoleActionMappingDTO holding the menu id and the role id to unmap</param>
        /// <returns>Integer</returns>
        public int DeleteMenuItems(RoleActionMappingDTO objDetails)
        {
            Int32? result = 0;

            try
            {

                //call the database function to unmap the role for the map with its child items
                result = dbContext.um_delete_menuitems(objDetails.RoleId, objDetails.MenuId).FirstOrDefault(); ;

                return result.HasValue ? result.Value : 0; 
                                
            }
            catch (Exception Ex)
            {

                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                throw Ex;
            }
            finally
            {
                dbContext.Dispose();
            }
           
        }

        #endregion

        #region menu level mapping

        public List<MenuLevelCombinationDTO> GetMenuItems()
        {

            List<MenuLevelCombinationDTO> lstLevel = new List<MenuLevelCombinationDTO>();
          
            try
            {
                  var lstMenu = from item in dbContext.UM_Menu_Master
                               where item.IsActive == true
                               orderby item.ParentID,item.MenuName,item.Sequence,item.MenuID
                               select item;


                foreach (var item in lstMenu)
                {
                    MenuLevelCombinationDTO objListDTO = new MenuLevelCombinationDTO();
                    objListDTO.MenuId = Convert.ToInt32(item.MenuID.ToString());
                    objListDTO.ParentId = Convert.ToInt32(item.ParentID);
                    objListDTO.Level = Convert.ToInt32(item.Sequence);
                    objListDTO.MenuName = item.MenuName.ToString();
                    lstLevel.Add(objListDTO);
                }
              
                return lstLevel;



            }
            catch (Exception Ex)
            {

                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                throw Ex;
            }
            finally
            {
                dbContext.Dispose();
            }
           



        }

       
        
        /// <summary>
        /// functiuon to get menus by id
        /// </summary>
        /// <param name="MenuId"></param>
        /// <param name="Level"></param>
        /// <returns></returns>
        public List<MenuLevelCombinationDTO> GetMenuItemsById(Int16 MenuId, Int32 Level)
        {

            List<MenuLevelCombinationDTO> lstLevel = new List<MenuLevelCombinationDTO>();
            List<um_get_all_menuitems_by_id_Result> List = null;
            try
            {

                List = dbContext.um_get_all_menuitems_by_id(MenuId).ToList<um_get_all_menuitems_by_id_Result>();



                foreach (um_get_all_menuitems_by_id_Result item  in List)
                {
                    MenuLevelCombinationDTO objListDTO = new MenuLevelCombinationDTO();
                    objListDTO.MenuId = Convert.ToInt32(item.MenuID);
                    objListDTO.ParentId = Convert.ToInt32(item.ParentID);
                    objListDTO.Level = Convert.ToInt32(item.VerticalLevel);
                    objListDTO.MenuName =(item.MenuName);
                    if (item.levelcomb != null)
                        objListDTO.LevelCombination = item.levelcomb.ToString();
                    else
                        objListDTO.LevelCombination = "0";
                    lstLevel.Add(objListDTO);
                }

                return lstLevel;
           }
            catch (Exception Ex)
            {

                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                throw Ex;
            }
            finally
            {
                if (dbContext != null)
                    dbContext.Dispose();
            }
       
        }

       
        
        /// <summary>
        /// function for adding menu level combination
        /// </summary>
        /// <param name="objLevelComb"></param>
        /// <returns></returns>
        public String AddLevelCombination(MenuLevelCombinationListDTO menuLevelCombinationDto)
        {
            dbContext = new PMGSYEntities();
            try
            {
               
                //Get all newly assigned levels
                var levelArr = menuLevelCombinationDto.LevelStr.Split(',');
                List<Int16> lstLevels = new List<Int16>();
                foreach (var item in levelArr)
                {
                    lstLevels.Add(Convert.ToInt16(item));
                }

                UM_Menu_Master um_menu_master = dbContext.UM_Menu_Master.Find(menuLevelCombinationDto.MenuId);
                um_menu_master.LevelGroupCode = GetGroupCode(lstLevels);
                dbContext.Entry(um_menu_master).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();

                return "111";
  
            }
            catch (Exception Ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                return "Error ocurred while updation of levels";
            }
            finally
            {
                dbContext.Dispose();
            }
           
        }

        /// <summary>
        /// Get Existing Levels group code
        /// </summary>
        /// <param name="levels"></param>
        /// <returns></returns>
        public int GetGroupCode(List<short> levels)
        {
            int groupCode = 0;
            IEnumerable<int> ids = (from groupID in dbContext.UM_Level_Combination
                                    select groupID.LevelGroupCode).ToList<int>().Distinct();

            foreach (int item in ids)
            {
                List<short> lstLevels = (from lst in dbContext.UM_Level_Combination
                                         where lst.LevelGroupCode == item
                                         select lst.LevelID).ToList<short>();
                List<short> stat = levels.Intersect(lstLevels).ToList<short>();
                //bool stat = levels.Equals(lstLevels);
                if (stat.Count() == levels.Count() && lstLevels.Count() == stat.Count())
                //if (stat.Count() == levels.Count())
                {
                    groupCode = item;
                    break;
                }
            }

            if (groupCode == 0) //Level Combination not present, So insert new Level Combination 
            {
                int maxGroupCode = ids.ToList<int>().Max() + 1;
                int maxKey = dbContext.UM_Level_Combination.Max(m => m.ID);
                foreach (int item in levels)
                {
                    UM_Level_Combination obj = new UM_Level_Combination();
                    obj.ID = ++maxKey;
                    obj.LevelGroupCode = maxGroupCode;
                    obj.LevelID = Convert.ToByte(item);
                    dbContext.UM_Level_Combination.Add(obj);
                    dbContext.SaveChanges();
                }
                groupCode = maxGroupCode;
            }
            return groupCode;

        }    






        #endregion


        /// <summary>
        /// Save details of newly created menu
        /// </summary>
        /// <param name="um_role_master"></param>
        /// <returns></returns>
        public bool CreateMenu(Menu_Master um_menu_master)
        {
            UM_Menu_Master obj_Menu_Master = new UM_Menu_Master();
            var dbContext = new PMGSYEntities();
            try
            {
                //Assign roleLevelapping details, for one Role multiple levels may be there.
                var levelArr = um_menu_master.LevelGroupCode.Split(',');


                List<Int16> lstLevels = new List<Int16>();
                foreach(var item in levelArr)
                {
                    lstLevels.Add(Convert.ToInt16(item));
                }
                
                obj_Menu_Master.MenuID = Convert.ToInt16(((from menu in dbContext.UM_Menu_Master select menu.MenuID).Max()) + 1);
                obj_Menu_Master.MenuName = um_menu_master.MenuName;
                if (um_menu_master.ParentID == -1)
                {
                    obj_Menu_Master.ParentID = 0;
                    obj_Menu_Master.HorizontalSequence = Convert.ToInt16(((from menu in dbContext.UM_Menu_Master select menu.HorizontalSequence).Max()) + 1);
                }
                else
                {
                    obj_Menu_Master.ParentID = um_menu_master.ParentID;
                    obj_Menu_Master.HorizontalSequence = Convert.ToInt16(0);
                }
                obj_Menu_Master.LevelGroupCode = GetGroupCode(lstLevels);
                obj_Menu_Master.Sequence = um_menu_master.Sequence;

                obj_Menu_Master.VerticalLevel = Convert.ToInt16(um_menu_master.VerticalLevel);
                obj_Menu_Master.IsActive = true;
                obj_Menu_Master.MenucombinationCode = um_menu_master.MenucombinationCode + obj_Menu_Master.MenuID;      //Add newly added menu to Combination Code
                //Add obj_Role_Master entity

                dbContext.UM_Menu_Master.Add(obj_Menu_Master);
                dbContext.SaveChanges();

                //int insertCnt = dbContext.SaveChanges();

                //if (insertCnt > 0)
                //{
                //    using (ResXResourceWriter resx = new ResXResourceWriter(@".\CarResources.resx"))
                //    {
                //        resx.AddResource("Title", "Classic American Cars");
                //        resx.AddResource("HeaderString1", "Make");
                //        resx.AddResource("HeaderString2", "Model");
                //        resx.AddResource("HeaderString3", "Year");
                //        resx.AddResource("HeaderString4", "Doors");
                //        resx.AddResource("HeaderString5", "Cylinders");
                //        resx.AddResource("Information", SystemIcons.Information);
                //        resx.AddResource("EarlyAuto1", car1);
                //        resx.AddResource("EarlyAuto2", car2);
                //    }
                //}
                return true;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return false;
            }
            finally
            {
                dbContext.Dispose();
            }
        }


       

        /// <summary>
        /// Display Add, Update, Delete Rights for the Menu of particular user for specific role
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="RoleId"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <param name="sidx"></param>
        /// <param name="sord"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public Array GetMenuRights(int userId, int RoleId, int? page, int? rows, string sidx, string sord, out long totalRecords)
        {
            try
            {
                using (var dbContext = new PMGSYEntities())
                {
                    var userMenuRightsItemList = dbContext.sp_get_menu_rights(userId, RoleId).ToList<sp_get_menu_rights_Result>().ToList();  // Remove Last character from string

                    totalRecords = userMenuRightsItemList.Count();

                    if (sidx.Trim() != string.Empty)
                    {
                        if (sord.ToString() == "asc")
                        {
                            userMenuRightsItemList = userMenuRightsItemList.OrderBy(x => x.MenuName).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                        }
                        else
                        {
                            userMenuRightsItemList = userMenuRightsItemList.OrderByDescending(x => x.MenuName).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                        }
                    }
                    else
                    {
                        userMenuRightsItemList = userMenuRightsItemList.OrderBy(x => x.MenuName).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }


                    var result = userMenuRightsItemList.Select(model => new
                    {
                        model.MenuID,
                        model.MenuName,
                        model.isAdd,
                        model.isEdit,
                        model.isDelete
                    }).ToArray();


                    return result.Select(model => new
                    {
                        id = model.MenuID,
                        cell = new[] {
                                        model.MenuName,
                                        model.isAdd.ToString(),
                                        model.isEdit.ToString(),
                                        model.isDelete.ToString(),
                                        "<center><a id='aAdd-"+ model.MenuID +"' class='ui-icon ui-icon-plusthick' href='#'></a></center>"
                             }
                    }).ToArray();
                }
            }
            catch (Exception Ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                throw Ex;
            }

        }


        public List<SelectListItem> GetUserRoleList(int UserId)
        {
            List<SelectListItem> lstRoles = new List<SelectListItem>();
            try
            {
                var query = (from uurm in dbContext.UM_User_Role_Mapping
                                join urm in dbContext.UM_Role_Master on uurm.RoleId equals urm.RoleID
                                where uurm.UserId == UserId
                                orderby urm.RoleName
                                select new
                                {
                                    Value = urm.RoleID,
                                    Text = urm.RoleName
                                }).Distinct().OrderBy(c => c.Text).ToList();


                SelectListItem item = new SelectListItem();
                item.Text = "Select Role";
                item.Value = "0";
                item.Selected = true;
                lstRoles.Add(item);

                if (UserId == 0)
                {
                    return lstRoles;
                }
                else
                {
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
            catch (Exception Ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                return null;
            }
        }


        /// <summary>
        /// Update Menu rights against particular user and role
        /// </summary>
        /// <param name="objDetails"></param>
        /// <returns></returns>
        public int UpdateMenuRights(Menu_Rights objDetails)
        {
            int? result = 0;
            try
            {
                //get the result of the mapping              
                result = dbContext.sp_set_menu_rights(objDetails.UserID, objDetails.RoleID, objDetails.MenuID, objDetails.IsAdd, objDetails.IsEdit, objDetails.IsDelete);
                return result.HasValue ? result.Value : 0;
            }
            catch (Exception Ex)
            {

                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                throw Ex;
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        /// <summary>
        /// Get roles for particular User according to level of user, 
        /// If any role matches with already assigned roles, then it appends value $$S as selected,
        /// for client side identification
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public List<SelectListItem> GetUserLevelwiseRoleList(int UserId)
        {
            List<SelectListItem> lstRoles = new List<SelectListItem>();
            try
            {
                //var query = (from uum in dbContext.UM_User_Master
                //             join urlm in dbContext.UM_Role_Level_Mapping on uum.LevelID equals urlm.LevelID
                //             join urm in dbContext.UM_Role_Master  on urlm.RoleID equals urm.RoleID
                //             where uum.UserID == UserId
                //             orderby urm.RoleName
                //             select new
                //             {
                //                 Value = urm.RoleID,
                //                 Text = urm.RoleName
                //             }).ToList();

                var query = (from urm in dbContext.UM_Role_Master 
                             orderby urm.RoleName
                             select new
                             {
                                 Value = urm.RoleID,
                                 Text = urm.RoleName
                             }).ToList();


                var assignedRoles = (from uurm in dbContext.UM_User_Role_Mapping
                                     where uurm.UserId == UserId
                                     select uurm.RoleId
                                     ).ToList();

                if (UserId == 0)
                {
                    return lstRoles;
                }
                else
                {
                    SelectListItem item = new SelectListItem();
                    foreach (var data in query)
                    {
                        item = new SelectListItem();
                        item.Text = data.Text;
                        if (assignedRoles.Contains(data.Value))
                            item.Value = data.Value.ToString() + "$$S";
                        else
                            item.Value = data.Value.ToString();
                        lstRoles.Add(item);
                    }
                    return lstRoles;
                }
            }
            catch (Exception Ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                return null;
            }
        }


        /// <summary>
        /// Update User-Role Mapping
        /// </summary>
        /// <param name="objDetails"></param>
        /// <returns></returns>
        public bool UpdateUserRoleMapping(UserRoleMapping objUserRoleMapping)
        {
            var context = new PMGSYEntities();
            try
            {
                var roleArr = objUserRoleMapping.RoleID.Split(',');

                //Delete alrady mapped roles for user
                List<UM_User_Role_Mapping> userRoleMappingEntity = (from uurm in context.UM_User_Role_Mapping
                                                              where uurm.UserId == objUserRoleMapping.UserID 
                                                              select uurm).ToList();

                List<Int16> existingRoleList = new List<Int16>();
                foreach (var existingRole in userRoleMappingEntity)
                {
                    existingRoleList.Add(existingRole.RoleId);
                }

                List<Int16> newRoleList = new List<Int16>();
                foreach (var role in roleArr)
                {
                    newRoleList.Add(Convert.ToInt16(role));
                }

                List<Int16> existingDiffRoleList = existingRoleList.Except(newRoleList).ToList();      //items in existingRoleList that are not in newRoleList
                List<Int16> newDiffRoleList = newRoleList.Except(existingRoleList).ToList();           //items in newRoleList that are not in existingRoleList

                //In Existing But not in new List
                if (existingDiffRoleList.Count > 0)
                {
                    foreach (var item in existingDiffRoleList)
                    {
                        //remove difference items
                        Int16 roleToRemove = item;
                        UM_User_Role_Mapping obj_userRole_mapping = context.UM_User_Role_Mapping.Where(c => c.UserId == objUserRoleMapping.UserID && c.RoleId == roleToRemove).First();
                        context.UM_User_Role_Mapping.Remove(obj_userRole_mapping);
                        context.SaveChanges();
                    }
                }

                //In New But not in Existing
                if (newDiffRoleList.Count > 0)
                {
                    foreach (var item in newDiffRoleList)
                    {
                        UM_User_Role_Mapping obj_userRole_mapping = new UM_User_Role_Mapping();
                       
                        obj_userRole_mapping.ID = (context.UM_User_Role_Mapping.Select(c => c.ID).Max()) + 1;
                        obj_userRole_mapping.UserId = objUserRoleMapping.UserID;
                        obj_userRole_mapping.RoleId = item;

                        //Add obj_userRole_mapping entity
                        context.UM_User_Role_Mapping.Add(obj_userRole_mapping);
                        context.SaveChanges();
                    }
                }

                
                return true;
            }
            catch (Exception Ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                return false;
            }
            finally
            {
                if (context != null)
                    context.Dispose();
            }
        }


        /// <summary>
        /// Update Menu Details
        /// </summary>
        /// <param name="um_role_master"></param>
        /// <returns></returns>
        public bool EditMenu(Menu_Master obj_menu_master)
        {
            var dbContext = new PMGSYEntities();
            try
            {

               //---------------------------------------------------------------------------------------

               ////Get all newly assigned levels
                var levelArr = obj_menu_master.LevelGroupCode.Split(',');

                List<Int16> lstLevels = new List<Int16>();
                foreach (var item in levelArr)
                {
                    lstLevels.Add(Convert.ToInt16(item));
                }

                //---------------------------------------------------------------------------------------

                //Update in Menu_Master
                UM_Menu_Master um_menu_master = dbContext.UM_Menu_Master.Find(obj_menu_master.MenuID);
                um_menu_master.MenuName = obj_menu_master.MenuName;
                um_menu_master.ParentID = obj_menu_master.ParentID == -1 ? 0 : obj_menu_master.ParentID;
                um_menu_master.Sequence = obj_menu_master.Sequence;
                um_menu_master.HorizontalSequence = obj_menu_master.HorizontalSequence;  //Keep Same as previous
                um_menu_master.VerticalLevel = Convert.ToInt16(obj_menu_master.VerticalLevel);
                um_menu_master.MenucombinationCode = obj_menu_master.MenucombinationCode + um_menu_master.MenuID;      //Add newly added menu to Combination Code
                um_menu_master.LevelGroupCode = GetGroupCode(lstLevels);
                dbContext.Entry(um_menu_master).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();

                return true;
            }
            catch (DbEntityValidationException dbEx)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(dbEx, HttpContext.Current);

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
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                return false;
            }
            finally
            {
                if (dbContext != null)
                    dbContext.Dispose();
            }
        }


    }   //class ends here





    public interface IMenuDAL
    {
        #region menu role mapping
        List<RoleActionListDTO> GetMenuItems(RoleActionMappingDTO objDetails);
        int MapRoleWithMenuItem(RoleActionMappingDTO objDetails);
        int DeleteMenuItems(RoleActionMappingDTO objDetails);
        #endregion

        #region menu level mapping
        String AddLevelCombination(MenuLevelCombinationListDTO menuLevelCombinationDto);        
        List<MenuLevelCombinationDTO> GetMenuItems();
        List<MenuLevelCombinationDTO> GetMenuItemsById(Int16 MenuId, Int32 Level);
        #endregion
    }
}