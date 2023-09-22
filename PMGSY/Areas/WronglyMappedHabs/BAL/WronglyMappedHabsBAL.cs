using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Web.Mvc;
using PMGSY.Extensions;
using PMGSY.Areas.WronglyMappedHabs.Models;
using PMGSY.Areas.WronglyMappedHabs.DAL;
using System.Collections.Generic;



namespace PMGSY.Areas.WronglyMappedHabs.BAL
{
    public class WronglyMappedHabsBAL : IWronglyMappedHabsBAL
    {
        private IWronglyMappedHabsDAL objDAL = null;
         
        private IWronglyMappedHabsBAL objBAL = null;

        public Array ListHabs(int stateCode, int? page, int? rows, string sidx, string sord, out long totalRecords)
        {
            objDAL = new WronglyMappedHabsDAL();
            return objDAL.ListHabsDAL(stateCode, page, rows, sidx, sord, out totalRecords);

        }

        public Boolean DeleteHabsBAL(int habCode, int roadCode)
        {
            objDAL = new WronglyMappedHabsDAL();
            return objDAL.DeleteHabsDAL(habCode,roadCode);
        }





    }
    public interface IWronglyMappedHabsBAL
    {
        //ListHabs
        Array ListHabs(int stateCode, int? page, int? rows, string sidx, string sord, out long totalRecords);
        Boolean DeleteHabsBAL(int habCode,int roadCode);

      



    }
}