using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PMGSY.Models.Menu;
using System.Configuration;
using PMGSY.Models;
using PMGSY.Extensions;

namespace PMGSY.DAL.Common
{
    public class MenubarDAL : IMenubarDAL
    {
        
       
        public List<MenuDTO> PopulateMenu(int rolecode, int levelcode)
        {
             PMGSYEntities dbContext = null;
            try
            {
                              
                List<MenuDTO> objListMenuDTO = new List<MenuDTO>();

                List<um_acc_populatemenu_Result> result = new List<um_acc_populatemenu_Result>();
                dbContext = new PMGSYEntities();
                String FundType = "0";
                if(PMGSYSession.Current.FundType != null && (!PMGSYSession.Current.FundType.Equals(string.Empty)))
                {
                    FundType = PMGSYSession.Current.FundType;
                }
                ///PMGSY3
                result = dbContext.um_acc_populatemenu(rolecode, levelcode, FundType, PMGSYSession.Current.PMGSYScheme).ToList<um_acc_populatemenu_Result>();

                foreach (um_acc_populatemenu_Result item in result)
                {


                    MenuDTO objMenuDTO = new MenuDTO();
                    objMenuDTO.IntMenuId = Convert.ToInt32(item.IntMenuId);
                    objMenuDTO.StrMenuCombinationCode = Convert.ToString(item.StrMenuCombinationCode);
                    objMenuDTO.IntMenuParentId = Convert.ToInt32(item.IntMenuParentId);
                    objMenuDTO.IntLevelId = Convert.ToInt32(item.IntLevelId);
                    objMenuDTO.IntRoleId = Convert.ToInt32(item.IntRoleId = item.IntRoleId.HasValue ? item.IntRoleId : 0);
                    objMenuDTO.IntMenuSeqNo = Convert.ToInt32(item.IntMenuSeqNo);
                    objMenuDTO.BoolStatus = Convert.ToBoolean(item.BoolStatus);
                    objMenuDTO.IntMenuLevel = Convert.ToInt32(item.IntMenuLevel);
                    objMenuDTO.StrMenuName = Convert.ToString(item.StrMenuName);
                    objMenuDTO.StrController = Convert.ToString(item.StrController);
                    objMenuDTO.StrAction = Convert.ToString(item.StrAction);
                    objMenuDTO.ModuleName = item.ModuleName;

                    objListMenuDTO.Add(objMenuDTO);
                }

                return objListMenuDTO;

            }catch(Exception ex)
            {

                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                throw ex;
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
     
    public interface IMenubarDAL
    {
        List<MenuDTO> PopulateMenu(int rolecode, int levelcode);
    }
}
