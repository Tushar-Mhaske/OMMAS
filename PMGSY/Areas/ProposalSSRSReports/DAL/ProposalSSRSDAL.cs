using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PMGSY.Models;
//using PMGSY.Areas.ProposalSSRSReports.DAL.ProposalSSRSDAL;
using PMGSY.Extensions;
using System.Data.Entity.Infrastructure;
using System.Web.Mvc;
using System.Transactions;
using System.Data.SqlClient;
using System.Collections.Generic;
//using PMGSY.Areas.ProposalSSRSReports.Models.ProposalReports;

namespace PMGSY.Areas.ProposalSSRSReports.DAL
{
    public class ProposalSSRSDAL : IProposalSSRSReportsDAL
    {
        PMGSY.Models.PMGSYEntities dbContext;

        public Array PCIPropAnalysisListingDAL(int stateCode, int districtCode, int blockCode, string flag, string routeType, int page, int rows, string sidx, string sord, out int totalRecords)
        {
            dbContext = new PMGSY.Models.PMGSYEntities();
            try
            {
                int State = stateCode;
                int District = districtCode;
                int Block = blockCode;
                string RouteType = routeType;
                string Flage = flag;
                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 180;

                var listPCIAbstractReports = dbContext.USP_CN_PROP_PCI_ANALYSIS(State, District, Block, Flage, RouteType, PMGSY).ToList<USP_CN_PROP_PCI_ANALYSIS_Result>();

                totalRecords = listPCIAbstractReports.Count();

                if (sidx.Trim() != string.Empty)
                {
                    if (sord.ToString() == "asc")
                    {
                        listPCIAbstractReports = listPCIAbstractReports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                    else
                    {
                        listPCIAbstractReports = listPCIAbstractReports.OrderByDescending(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                    }
                }
                else
                {
                    listPCIAbstractReports = listPCIAbstractReports.OrderBy(x => x.LOCATION_NAME).Skip(Convert.ToInt32(page * rows)).Take(Convert.ToInt32(rows)).ToList();
                }

                return listPCIAbstractReports.Select(x => new
                {
                    id = x.LOCATION_CODE.ToString() + "$" + x.LOCATION_NAME.ToString() + "$" + x.PCI_LENGTH.ToString(),
                    cell = new[]{
                       // "<a href='#' style='text-decoration:none;' title='Click here to view details' onClick='PCIAbstractDistrictReportListing(\""+x.LOCATION_CODE.ToString().Trim()+"\",\""+x.LOCATION_NAME.ToString().Trim()+"\",\""+Route.ToString().Trim()+"\"); return false;'>" + x.LOCATION_NAME + "</a>",                       
                      x.LOCATION_NAME==null?"":  x.LOCATION_NAME.ToString(),
                      x.LINK_ROADS==null?"": x.LINK_ROADS.ToString(),
                       x.PLAN_RD_LENGTH==null?"": x.PLAN_RD_LENGTH.ToString(),
                        x.PCI_ROAD==null?"": x.PCI_ROAD.ToString(),
                      x.PCI_LENGTH==null?"0": x.PCI_LENGTH.ToString(),
                       x.LEN_ONE==null?"0": x.LEN_ONE.ToString(),
                       x.LEN_TWO==null?"0": x.LEN_TWO.ToString(),
                       x.LEN_THREE==null?"0": x.LEN_THREE.ToString(),
                        x.LEN_FOUR==null?"0": x.LEN_FOUR.ToString(),                    
                      x.LEN_FIVE==null?"0": x.LEN_FIVE.ToString(),
                        x.TOT_LENGTH_YEAR1==null?"":x.TOT_LENGTH_YEAR1.ToString(),
                       x.TOT_LENGTH_YEAR2==null?"": x.TOT_LENGTH_YEAR2.ToString(),
                       x.TOT_LENGTH_YEAR3==null?"": x.TOT_LENGTH_YEAR3.ToString()
                    
                    }
                }).ToArray();
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                totalRecords = 0;
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public List<USP_CN_PROP_PCI_ANALYSIS_Result> GetPropPCIAnalysisChartDAL(int stateCode, int districtCode, int blockCode, string flag, string routeType)
        {

            try
            {
                dbContext = new PMGSYEntities();
                int State = stateCode;
                int District = districtCode;
                int Block = blockCode;
                string RouteType = routeType;
                string Flage = flag;
                byte PMGSY = PMGSYSession.Current.PMGSYScheme;
                ((IObjectContextAdapter)dbContext).ObjectContext.CommandTimeout = 180;

                List<USP_CN_PROP_PCI_ANALYSIS_Result> resultlist = dbContext.USP_CN_PROP_PCI_ANALYSIS(State, District, Block, Flage, RouteType, PMGSY).ToList<USP_CN_PROP_PCI_ANALYSIS_Result>();

                return resultlist;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                throw ex;
            }
            finally
            {
                dbContext.Dispose();
            }
        }
    }

    public interface IProposalSSRSReportsDAL
    {
        Array PCIPropAnalysisListingDAL(int stateCode, int districtCode, int blockCode, string flag, string routeType, int page, int rows, string sidx, string sord, out int totalRecords);
        List<USP_CN_PROP_PCI_ANALYSIS_Result> GetPropPCIAnalysisChartDAL(int stateCode, int districtCode, int blockCode, string flag, string routeType);
    }
}