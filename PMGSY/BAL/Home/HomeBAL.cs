using PMGSY.DAL.Home;
using PMGSY.Models;
using PMGSY.Models.Home;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.BAL.Home
{
    public class HomeBAL
    {
        private HomeDAL objDAL;
        private PMGSYEntities dbContext;


        public List<USP_HIGHMAP_CBR_DETAILS_Result> GetCBRDataBAL(int state, int district, int block)
        {
            objDAL = new HomeDAL();
            return objDAL.GetCBRDataDAL(state, district, block);
        }

        public List<CBRColumnChartModel> GetCBRColumnChartDataBAL(int state, int district, int block)
        {
            objDAL = new HomeDAL();
            return objDAL.GetCBRColumnChartDataDAL(state, district, block);
        }


        public List<CBRColumnChartModel> GetCBRGridDataBAL(int state, int district, int block)
        {
            objDAL = new HomeDAL();
            return objDAL.GetCBRGridDataDAL(state, district, block);
        }
    }
}