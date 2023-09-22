using PMGSY.Models;
using PMGSY.Models.Home;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.DAL.Home
{
    public class HomeDAL
    {
        PMGSYEntities dbContext;


        /// <summary>
        /// CBR Min Max Details
        /// </summary>
        /// <param name="state"></param>
        /// <param name="district"></param>
        /// <param name="block"></param>
        /// <returns></returns>
        public List<USP_HIGHMAP_CBR_DETAILS_Result> GetCBRDataDAL(int state, int district, int block)
        {
            dbContext = new PMGSYEntities();
            try
            {
                List<USP_HIGHMAP_CBR_DETAILS_Result> lstResult = new List<USP_HIGHMAP_CBR_DETAILS_Result>();
                lstResult = dbContext.USP_HIGHMAP_CBR_DETAILS(3, state, district).ToList<USP_HIGHMAP_CBR_DETAILS_Result>();
                return lstResult;
            }
            catch
            {
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }
        
        /// <summary>
        /// Column Chart Data
        /// </summary>
        /// <param name="state"></param>
        /// <param name="district"></param>
        /// <param name="block"></param>
        /// <returns></returns>
        public List<CBRColumnChartModel> GetCBRColumnChartDataDAL(int state, int district, int block)
        {
            dbContext = new PMGSYEntities();
            try
            {
                List<USP_HIGHMAP_CBR_PIE_Result> lstResult = new List<USP_HIGHMAP_CBR_PIE_Result>();
                lstResult = dbContext.USP_HIGHMAP_CBR_PIE(3, state, district, block, 0, 0, 0, "%", 1).ToList<USP_HIGHMAP_CBR_PIE_Result>();

                List<CBRColumnChartModel> lstChart = new List<CBRColumnChartModel>();

                int srNo = 1;
                foreach (var item in lstResult)
                {
                    CBRColumnChartModel model = new CBRColumnChartModel();
                    model.SrNo = srNo.ToString();
                    model.Range = item.PCI_RANGE.ToString();
                    model.NCount = item.NCOUNT.ToString();
                    model.NPerc = item.NPERC.ToString();
                    model.UCount = item.UCOUNT.ToString();
                    model.UPerc = item.UPERC.ToString();

                    lstChart.Add(model);

                    srNo++;
                }

                return lstChart;
            }
            catch
            {
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }



        /// <summary>
        /// CBR Grid Data 
        /// </summary>
        /// <param name="state"></param>
        /// <param name="district"></param>
        /// <param name="block"></param>
        /// <returns></returns>
        public List<CBRColumnChartModel> GetCBRGridDataDAL(int state, int district, int block)
        {
            dbContext = new PMGSYEntities();
            try
            {
                List<USP_HIGHMAP_CBR_PIE_Result> lstResult = new List<USP_HIGHMAP_CBR_PIE_Result>();
                lstResult = dbContext.USP_HIGHMAP_CBR_PIE(3, state, district, block, 0, 0, 0, "%", 1).ToList<USP_HIGHMAP_CBR_PIE_Result>();

                List<CBRColumnChartModel> lstChart = new List<CBRColumnChartModel>();

                int srNo = 1;
                int totNCount = 0, totUCount = 0;
                decimal totNPerc = 0, totUPerc = 0;
                foreach (var item in lstResult)
                {

                    CBRColumnChartModel model = new CBRColumnChartModel();
                    model.SrNo = srNo.ToString();
                    model.Range = item.PCI_RANGE.ToString();
                    model.RangeName = item.RANGE_NAME.ToString();
                    model.NCount = item.NCOUNT.ToString();
                    model.NPerc = item.NPERC.ToString();
                    model.UCount = item.UCOUNT.ToString();
                    model.UPerc = item.UPERC.ToString();


                    totNCount = totNCount + Convert.ToInt32(item.NCOUNT);
                    totNPerc = totNPerc + Convert.ToDecimal(item.NPERC);
                    totUCount = totUCount + Convert.ToInt32(item.UCOUNT);
                    totUPerc = totUPerc + Convert.ToDecimal(item.UPERC);

                    lstChart.Add(model);

                    srNo++;
                }

                //Add Totals to Model & then to List
                CBRColumnChartModel mod = new CBRColumnChartModel();
                mod.SrNo = "";
                mod.RangeName = "<b>Total</b>";
                mod.NCount = totNCount.ToString();
                mod.NPerc = (totNPerc > 100) ? "100.00" : totNPerc.ToString();
                mod.UCount = totUCount.ToString();
                mod.UPerc = (totUPerc > 100) ? "100.00" : totUPerc.ToString();
                lstChart.Add(mod);

                return lstChart;
            }
            catch
            {
                return null;
            }
            finally
            {
                dbContext.Dispose();
            }
        }
    }
}