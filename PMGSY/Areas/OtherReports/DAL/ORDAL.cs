using PMGSY.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace PMGSY.Areas.GeneralReport.DAL
{
    public class ORDAL
    {
        public List<SelectListItem> PopulateStates(bool isPopulateFirstItem = true)
        {
            List<SelectListItem> StatesList = new List<SelectListItem>();
            SelectListItem item;
            var dbContext = new PMGSY.Models.PMGSYEntities();
            if (isPopulateFirstItem)
            {
                item = new SelectListItem();
                item.Text = "Select State";
                item.Value = "0";
                item.Selected = true;
                StatesList.Add(item);
            }

            try
            {

                var query = (from c in dbContext.MASTER_STATE
                             where c.MAST_STATE_ACTIVE == "Y"
                             select new
                             {
                                 Text = c.MAST_STATE_NAME,
                                 Value = c.MAST_STATE_CODE
                             }).OrderBy(c => c.Text).ToList();

                foreach (var data in query)
                {
                    item = new SelectListItem();
                    item.Text = data.Text;
                    item.Value = data.Value.ToString();
                    StatesList.Add(item);
                }
                return StatesList;
            }
            catch
            {
                return StatesList;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public List<SelectListItem> PopulateDistrict(Int32 StateCode, bool isAllSelected = false)
        {
            var dbContext = new PMGSYEntities();
            try
            {

                List<SelectListItem> lstDistrict = null;
                lstDistrict = new SelectList(dbContext.MASTER_DISTRICT.Where(m => m.MAST_STATE_CODE == StateCode && m.MAST_DISTRICT_ACTIVE == "Y").OrderBy(s => s.MAST_DISTRICT_NAME), "MAST_DISTRICT_CODE", "MAST_DISTRICT_NAME").ToList();

                if (isAllSelected == false)
                {
                    lstDistrict.Insert(0, (new SelectListItem { Text = "Select District", Value = "0", Selected = true }));
                }
                else if (isAllSelected == true)
                {
                    lstDistrict.Insert(0, (new SelectListItem { Text = "All District", Value = "0", Selected = true }));
                }
                return lstDistrict;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public List<SelectListItem> PopulateBlocks(int MAST_DISTRICT_CODE, bool isAllBlocksSelected = false)
        {
            var dbContext = new PMGSYEntities();
            List<SelectListItem> BlockList = new List<SelectListItem>();
            SelectListItem item;
            if (!isAllBlocksSelected)
            {
                item = new SelectListItem();
                item.Text = "Select Block";
                item.Value = "0";
                item.Selected = true;
                BlockList.Add(item);
            }
            else
            {
                item = new SelectListItem();
                item.Text = "All Blocks";
                item.Value = "0";
                item.Selected = true;
                BlockList.Add(item);
            }
            try
            {

                var query = (from c in dbContext.MASTER_BLOCK
                             where
                             c.MAST_DISTRICT_CODE == MAST_DISTRICT_CODE && c.MAST_BLOCK_ACTIVE == "Y"
                             select new
                             {
                                 Text = c.MAST_BLOCK_NAME,
                                 Value = c.MAST_BLOCK_CODE
                             }).ToList();

                foreach (var data in query)
                {
                    item = new SelectListItem();
                    item.Text = data.Text;
                    item.Value = data.Value.ToString();
                    BlockList.Add(item);
                }
                return BlockList;
            }
            catch
            {
                return BlockList;
            }
            finally
            {
                dbContext.Dispose();
            }
        }

        public List<SelectListItem> PopulateYears( bool isAllYear = false)
        {
            var dbContext = new PMGSYEntities();
            try
            {
            
                List<SelectListItem> lstYears = new SelectList(dbContext.MASTER_YEAR.Where(m => m.MAST_YEAR_CODE < (DateTime.Now.Year + 1)).OrderByDescending(m => m.MAST_YEAR_CODE).ToList(), "MAST_YEAR_CODE", "MAST_YEAR_CODE",DateTime.Now.Year).ToList();

                //if (isAllYear)
                //{
                //    lstYears.Insert(0, (new SelectListItem { Text = "All Year", Value = "0" }));
                //}
                //else
                //{
                //    lstYears.Insert(0, (new SelectListItem { Text = "Select Year", Value = "0" }));
                //}
                return lstYears;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbContext.Dispose();
            }
        }


        public List<SelectListItem> GetTechnologyList()
        {
              List<SelectListItem> lstTech = new List<SelectListItem>();
          var  dbContext = new PMGSY.Models.PMGSYEntities();
            var list = (from ag in dbContext.MASTER_TECHNOLOGY
                        select new
                        {
                            MAST_TECH_CODE = ag.MAST_TECH_CODE,
                            MAST_TECH_NAME = ag.MAST_TECH_NAME
                        }).Distinct().OrderBy(m => m.MAST_TECH_CODE).ToList();


            lstTech = new SelectList(list.ToList(), "MAST_TECH_CODE", "MAST_TECH_NAME").ToList();
            lstTech.Insert(0, new SelectListItem { Text = "All", Value = "0" });
            return lstTech;
        }

    }
}