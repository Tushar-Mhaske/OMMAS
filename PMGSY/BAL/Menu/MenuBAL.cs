using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PMGSY.Models.Menu;
using PMGSY.DAL.Menu;
using PMGSY.DAL.User_Manager;
using System.Web.Mvc;

namespace PMGSY.BAL.Menu
{
    public class MenuBAL : IMenuBAL
    {

        IMenuDAL MenumapDAL = null;

        #region menu role mapping

        /// <summary>
        /// Purpose :   Used to get a list of Menu Items given the parent menu item id and role id
        /// Called  :   By GetMenuItems(...) in the ~\Controllers\MenurController.cs
        /// Author  :   Amol U. Jadhav
        /// </summary>
        /// <param name="objDetails">An object of type RoleActionMappingDTO holding the parent menu item id, role id etc</param>
        /// <returns>List of ROleACtionListDTO</returns>
        public List<RoleActionListDTO> GetMenuItems(RoleActionMappingDTO objDetails)
        {
            List<RoleActionListDTO> lstMenuItems = null;


            try
            {
                MenumapDAL = new MenuDAL();
                lstMenuItems = MenumapDAL.GetMenuItems(objDetails);
                return lstMenuItems;
            }

            catch (Exception Ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                throw Ex;
            }
        }




        /// <summary>
        /// Purpose :   Used to get a list of Menu Items as a Json object
        /// Called  :   By GetMenuItems(...) in the ~\Controllers\UserManagerController.cs
        /// Author  :  Amol U. Jadhav
        /// </summary>
        /// <param name="listMenuItems">List of RoleActionListDTO</param>
        /// <returns>object</returns>
        public object GetJSONMenuItemCollection(List<RoleActionListDTO> listMenuItems)
        {
            int level = 0;
            if (HttpContext.Current.Request.Form.AllKeys.Contains("nodeid"))
            {
                if (!HttpContext.Current.Request.Form["nodeid"].Equals(""))
                {
                    level = Convert.ToInt32(HttpContext.Current.Request.Form["n_level"]) + 1;
                }
            }
            try
            {
                var data = (listMenuItems.Select(c => new
                {
                    id = c.MenuId,
                    cell = new[]
                                            {
                                                c.Description,
                                                c.Active.Equals("N")?"<center><a id='aAdd"+ c.MenuId +"' class='ui-icon ui-icon-plusthick' href='#'></a></center>":"<center><a id='aDelete"+ c.MenuId +"'  class='ui-icon ui-icon-circle-close' href='#'></a></center>",
                                                level.ToString(),
                                                c.ParentId.ToString(),
                                                c.IsLeaf? "true":"false",
                                                false.ToString()
                                            }
                })).ToArray();
                return data;
            }
            catch (Exception Ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                        throw Ex;
            }
        }


        /// <summary>
        /// Purpose :   Used to Map a Role with a Menu Item
        /// Called  :   By MapRoleWithMenuItem(...) in ~\Controllers\UserManagerController.cs
        /// Author  :  Amol U. Jadhav
        /// </summary>
        /// <param name="objDetails">Object of type RoleActionMappingDTO holding the menu id and the role id to map</param>
        /// <returns>Integer</returns>
        public int MapRoleWithMenuItem(RoleActionMappingDTO objDetails)
        {
            int intReturnVal = 0;
            try
            {
                MenumapDAL = new MenuDAL();
                intReturnVal = MenumapDAL.MapRoleWithMenuItem(objDetails);
                return intReturnVal;
            }

            catch (Exception Ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                        throw Ex;
            }
        }



        /// <summary>
        /// Purpose :   Used to Unmap a Role with Menu Item(s)
        /// Called  :   By DeleteMenuItems(...) in ~\Controllers\UserManagerController.cs
        /// Author  :  Amol U. Jadhav
        /// </summary>
        /// <param name="objDetails">Object of type RoleActionMappingDTO holding the menu id and the role id to unmap</param>
        /// <returns>Integer</returns>
        public int DeleteMenuItems(RoleActionMappingDTO objDetails)
        {
            int intReturnVal = 0;
            IMenuDAL objRAMapDTO = null;

            try
            {
                objRAMapDTO = new MenuDAL();
                intReturnVal = objRAMapDTO.DeleteMenuItems(objDetails);
                return intReturnVal;
            }

            catch (Exception Ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
               throw Ex;
            }
        }


        #endregion

        #region menu level mapping

        /// <summary>
        /// function to get all menu items
        /// </summary>
        /// <returns></returns>
        public List<MenuLevelCombinationDTO> GetMenuItems()
        {
            MenumapDAL = new MenuDAL();
            return MenumapDAL.GetMenuItems();
        }

        /// <summary>
        /// BAL function to get menu items with provided menu items
        /// </summary>
        /// <param name="MenuId"></param>
        /// <param name="Level"></param>
        /// <returns></returns>
        public List<MenuLevelCombinationDTO> GetMenuItemsById(Int16 MenuId, Int32 Level)
        {
            MenumapDAL = new MenuDAL();
            return MenumapDAL.GetMenuItemsById(MenuId, Level);
        }


        /// <summary>
        /// purpose: BAL function to add menu level combination
        /// </summary>
        /// <param name="objLevelComb"></param>
        /// <returns></returns>
        public String AddLevelCombination(MenuLevelCombinationListDTO menuLevelCombinationDto)
        {
            MenumapDAL = new MenuDAL();
            try
            {
                return MenumapDAL.AddLevelCombination(menuLevelCombinationDto);
            }
            catch (Exception Ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                throw Ex;
            }
        }

        #endregion


        public bool CreateMenu(Menu_Master um_menu_master)
        {
            MenuDAL menuDAL = new MenuDAL();
            return menuDAL.CreateMenu(um_menu_master);
        }


        public Array GetMenuRights(int UserId, int RoleId, int? page, int? rows, string sidx, string sord, out long totalRecords)
        {
            MenuDAL menuDAL = new MenuDAL();
            try
            {
                return menuDAL.GetMenuRights(UserId, RoleId, page, rows, sidx, sord, out totalRecords);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                throw ex;
            }
        }



        /// <summary>
        /// returns User specific roles
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public List<SelectListItem> GetUserRoleList(int UserId)
        {
            MenuDAL menuDAL = new MenuDAL();
            try
            {
                return menuDAL.GetUserRoleList(UserId);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                throw ex;
            }
        }


        /// <summary>
        /// Update Menu Rights for specific user
        /// </summary>
        /// <param name="objDetails"></param>
        /// <returns></returns>
        public int UpdateMenuRights(Menu_Rights objDetails)
        {
            int intReturnVal = 0;
            MenuDAL menuDAL = new MenuDAL();
            try
            {
                intReturnVal = menuDAL.UpdateMenuRights(objDetails);
                return intReturnVal;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);

                throw ex;
            }
        }



        /// <summary>
        /// Get roles for particular User according to level of user
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public List<SelectListItem> GetUserLevelwiseRoleList(int UserId)
        {
            MenuDAL menuDAL = new MenuDAL();
            try
            {
                return menuDAL.GetUserLevelwiseRoleList(UserId);
            }
            catch (Exception Ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                throw Ex;
            }
        }


        /// <summary>
        /// Update User-Role Mapping
        /// </summary>
        /// <param name="objUserRoleMapping"></param>
        /// <returns></returns>
        public bool UpdateUserRoleMapping(UserRoleMapping objUserRoleMapping)
        {
            MenuDAL menuDAL = new MenuDAL();
            try
            {
                return menuDAL.UpdateUserRoleMapping(objUserRoleMapping);
            }
            catch (Exception Ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(Ex, HttpContext.Current);
                throw Ex;
            }
        }


        /// <summary>
        /// Edit Menu
        /// </summary>
        /// <param name="um_user_master"></param>
        /// <returns></returns>
        public bool EditMenu(Menu_Master um_menu_master)
        {
            MenuDAL menuDAL = new MenuDAL();
            return menuDAL.EditMenu(um_menu_master);
        }

    }

    public interface IMenuBAL
    {

        #region menu role mapping
        List<RoleActionListDTO> GetMenuItems(RoleActionMappingDTO objDetails);
        object GetJSONMenuItemCollection(List<RoleActionListDTO> listMenuItems);
        int MapRoleWithMenuItem(RoleActionMappingDTO objDetails);
        int DeleteMenuItems(RoleActionMappingDTO objDetails);
        #endregion

        #region menu level mapping
        List<MenuLevelCombinationDTO> GetMenuItems();
        List<MenuLevelCombinationDTO> GetMenuItemsById(Int16 MenuId, Int32 Level);
        String AddLevelCombination(MenuLevelCombinationListDTO menuLevelCombinationDto);
        #endregion
    }
}