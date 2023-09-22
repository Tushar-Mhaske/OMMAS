using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PMGSY.BLL.Common;
using PMGSY.Models.Menu;
using MVCHtmlHelper.Helpers;




namespace PMGSY.Common
{
    public class MenuControl
    {

        IMenuService objMenuService;
      //  IPersonalizationBLL objPersonalization;
        public MenuControl()
        {
            objMenuService = new MenuService();
           // objPersonalization = new PersonalizationBLL();
        }

        /// <summary>
        /// Menu Collection will be returned
        /// </summary>
        /// <param name="roleCode"></param>
        /// <param name="levelID"></param>
        /// <returns></returns>
        public List<Menu> MenuReturn(int roleCode, Int16 levelID)
        {
            List<Menu> menuCollection = new List<Menu>();
            List<MenuDTO> menuCollect = new List<MenuDTO>();
            
            try
            {
                //menuCollect = objMenuService.PopulateMenu(roleCode, levelID).OrderBy(l => l.IntMenuId).OrderBy(l => l.IntMenuSeqNo).ToList();
                menuCollect = objMenuService.PopulateMenu(roleCode, levelID).ToList();

                foreach (MenuDTO pffMaster in menuCollect)
                {
                    if (pffMaster.IntMenuId == 0)
                    {

                        string urlController = string.Empty;
                        string urlAction = string.Empty;

                        menuCollection.Add(new Menu
                        {
                            ModuleID = pffMaster.IntMenuId,
                            ParentModuleID = pffMaster.IntMenuParentId,
                            Name = pffMaster.StrMenuName,
                            ControllerName = urlController,
                            ActionName = urlAction,
                            Title = pffMaster.StrMenuName,
                            StrParentModuleID = pffMaster.IntMenuParentId.ToString(),
                            PayableID = pffMaster.transactionid,
                            ModuleName = pffMaster.ModuleName
                        });
                    }
                    else
                    {

                        menuCollection.Add(new Menu
                        {
                            ModuleID = pffMaster.IntMenuId,
                            ParentModuleID = pffMaster.IntMenuParentId,
                            Name = pffMaster.StrMenuName,
                            ControllerName = pffMaster.StrController,
                            ActionName = pffMaster.StrAction,
                            Title = pffMaster.StrMenuName,
                            StrParentModuleID = pffMaster.IntMenuParentId.ToString(),
                            PayableID = pffMaster.transactionid,
                            ModuleName = pffMaster.ModuleName
                        });

                    }
                }

                return menuCollection;
            }
            catch 
            {
                return null;
            }
        }

    }
}
