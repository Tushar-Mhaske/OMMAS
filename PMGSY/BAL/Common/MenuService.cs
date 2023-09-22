using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PMGSY.Common;
using PMGSY.DAL.Common;
using PMGSY.Models.Menu;
using System.Resources;
using System.Globalization;
using PMGSY.DAL.Menu;


namespace PMGSY.BLL.Common
{
    public class MenuService : IMenuService
    {
        private IMenubarDAL objMenuDAL;

        public MenuService()
        {
            objMenuDAL = new MenubarDAL();
        }

        public List<MenuDTO> PopulateMenu(int rolecode, int levelcode)
        {
            List<MenuDTO> lst = new List<MenuDTO>();
            lst = objMenuDAL.PopulateMenu(rolecode,levelcode);
            return lst;
        }
    }

    public interface IMenuService
    {
        List<MenuDTO> PopulateMenu(int rolecode, int levelcode);
    }
}
