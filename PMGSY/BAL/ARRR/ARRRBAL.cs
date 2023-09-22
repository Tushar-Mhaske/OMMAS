using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PMGSY.DAL.ARRR;
using PMGSY.Models.ARRR;
using System.Web.Mvc;

namespace PMGSY.BAL.ARRR
{
    public class ARRRBAL : IARRRBAL
    {
        IARRRDAL objIARRRDAL = new ARRRDAL();

        #region
        public List<SelectListItem> PopulateChapterListBAL(bool flg)
        {
            return objIARRRDAL.PopulateChapterList(flg);
        }

        public List<SelectListItem> PopulateItemsListBAL(bool flg, int headCode)
        {
            return objIARRRDAL.PopulateItemsList(flg, headCode);
        }

        public List<SelectListItem> PopulateMajorItemsListBAL(bool flg, int ItemCode)
        {
            return objIARRRDAL.PopulateMajorItemsList(flg, ItemCode);
        }

        public List<SelectListItem> PopulateMajorItemsListItemwiseBAL(bool flg, int itemCode)
        {
            return objIARRRDAL.PopulateMajorItemsListItemwise(flg, itemCode);
        }

        public bool addARRRChaptersBAL(PMGSY.Models.ARRR.ChapterItemsViewModel model, ref string message)
        {
            return objIARRRDAL.addARRRChaptersDAL(model, ref message);
        }

        public bool changeChapterstatusBAL(int ItemCode)
        {
            return objIARRRDAL.changeChapterstatusDAL(ItemCode);
        }

        public bool editARRRChaptersBAL(PMGSY.Models.ARRR.ChapterItemsViewModel model, ref string message)
        {
            return objIARRRDAL.editARRRChaptersDAL(model, ref message);
        }

        public bool deleteARRRChaptersBAL(int ItemCode)
        {
            return objIARRRDAL.deleteARRRChaptersDAL(ItemCode);
        }

        public Array ListChapterItemsBAL(int page, int rows, string sidx, string sord, out long totalRecords)
        {
            return objIARRRDAL.ListChapterItemsDAL(page, rows, sidx, sord, out totalRecords);
        }
        #endregion

        #region
        public List<SelectListItem> PopulateCategoryBAL(bool flg, int lmmType)
        {
            return objIARRRDAL.PopulateCategoryDAL(flg, lmmType);
        }
        public List<SelectListItem> PopulateUsageUnitBAL()
        {
            return objIARRRDAL.PopulateUsageUnitDAL();
        }

        public List<SelectListItem> PopulateOutputUnitBAL()
        {
            return objIARRRDAL.PopulateOutputUnitDAL();
        }

        public bool addARRRMachineryBAL(PMGSY.Models.ARRR.MachineryMasterViewModel model, ref string message)
        {
            return objIARRRDAL.addARRRMachineryDAL(model, ref message);
        }

        public bool editARRRMachineryBAL(PMGSY.Models.ARRR.MachineryMasterViewModel model, ref string message)
        {
            return objIARRRDAL.editARRRMachineryDAL(model, ref message);
        }

        public bool deleteARRRMachineryBAL(int ItemCode)
        {
            return objIARRRDAL.deleteARRRMachineryDAL(ItemCode);
        }

        public Array ListMachineryMasterBAL(int page, int rows, string sidx, string sord, out long totalRecords)
        {
            return objIARRRDAL.ListMachineryMasterDAL(page, rows, sidx, sord, out totalRecords);
        }
        #endregion

        #region Material Rates Master
        public List<SelectListItem> PopulateLMMItemsListBAL(bool flg, int lmmtype, int category)
        {
            return objIARRRDAL.PopulateLMMItemsListDAL(flg, lmmtype, category);
        }

        public bool MaterialRateFinalizeBAL(int rateCode)
        {
            return objIARRRDAL.MaterialRateFinalizeDAL(rateCode);
        }
        public bool addARRRMaterialRatesBAL(PMGSY.Models.ARRR.LMMRateViewModel model, ref string message)
        {
            return objIARRRDAL.addARRRMaterialRatesDAL(model, ref message);
        }

        public bool changeMachineryMasterstatusBAL(int lmmCode)
        {
            return objIARRRDAL.changeMachineryMasterstatusDAL(lmmCode);
        }

        public bool editARRRMaterialRatesBAL(PMGSY.Models.ARRR.LMMRateViewModel model, ref string message)
        {
            return objIARRRDAL.editARRRMaterialRatesDAL(model, ref message);
        }

        public bool deleteARRRMaterialRatesBAL(int rateCode)
        {
            return objIARRRDAL.deleteARRRMaterialRatesDAL(rateCode);
        }
        public Array ListMaterialRatesBAL(int page, int rows, string sidx, string sord, int Year, out long totalRecords)
        {
            return objIARRRDAL.ListMaterialRatesDAL(page, rows, sidx, sord, Year, out totalRecords);
        }


        public Array ListMaterialFormBAL(int page, int rows, string sidx, string sord, out long totalRecords)
        {
            return objIARRRDAL.ListMaterialFormDAL(page, rows, sidx, sord, out totalRecords);
        }

        public bool addBulkARRRMaterialRatesBAL(IEnumerable<LabourDetails> materialData, ref string message)
        {
            return objIARRRDAL.addBulkARRRMaterialRatesDAL(materialData, ref message);
        }

        public bool FinalizeAllMaterialRatesBAL(int year)
        {
            return objIARRRDAL.FinalizeAllMaterialRatesDAL(year);
        }


        #endregion

        #region Labour Rates
        public bool LabourRateFinalizeBAL(int rateCode)
        {
            return objIARRRDAL.LabourRateFinalizeDAL(rateCode);
        }
        public bool addARRRLabourRatesBAL(PMGSY.Models.ARRR.LMMRateViewModel model, ref string message)
        {
            return objIARRRDAL.addARRRLabourRatesDAL(model, ref message);
        }
        public bool editARRRLabourRatesBAL(PMGSY.Models.ARRR.LMMRateViewModel model, ref string message)
        {
            return objIARRRDAL.editARRRLabourRatesDAL(model, ref message);
        }

        public bool deleteARRRLabourRatesBAL(int rateCode)
        {
            return objIARRRDAL.deleteARRRLabourRatesDAL(rateCode);
        }

        public Array ListLabourRatesBAL(int page, int rows, string sidx, string sord, int Year, out long totalRecords)
        {
            return objIARRRDAL.ListLabourRatesDAL(page, rows, sidx, sord, Year, out totalRecords);
        }



        public Array ListLabourFormBAL(int page, int rows, string sidx, string sord, out long totalRecords) //Added by Aditi on 21 Aug 2020
        {
            return objIARRRDAL.ListLabourFormDAL(page, rows, sidx, sord, out totalRecords);
        }

        public bool addBulkARRRLabourRatesBAL(IEnumerable<LabourDetails> labourData, ref string message) //Added by Aditi on 27 Aug 2020
        {
            return objIARRRDAL.addBulkARRRLabourRatesDAL(labourData, ref message);
        }

        public bool FinalizeAllLabourRatesBAL(int year) //Added by Aditi on 1 Sept 2020
        {
            return objIARRRDAL.FinalizeAllLabourRatesDAL(year);
        }


        #endregion

        #region Machinery Rates
        public bool MachineryRateFinalizeBAL(int rateCode)
        {
            return objIARRRDAL.MachineryRateFinalizeDAL(rateCode);
        }
        public bool addARRRMachineryRatesBAL(PMGSY.Models.ARRR.LMMRateViewModel model, ref string message)
        {
            return objIARRRDAL.addARRRMachineryRatesDAL(model, ref message);
        }

        public bool changeLMMRatestatusBAL(int rateCode)
        {
            return objIARRRDAL.changeLMMRatestatusDAL(rateCode);
        }

        public bool editARRRMachineryRatesBAL(PMGSY.Models.ARRR.LMMRateViewModel model, ref string message)
        {
            return objIARRRDAL.editARRRMachineryRatesDAL(model, ref message);
        }

        public bool deleteARRRMachineryRatesBAL(int rateCode)
        {
            return objIARRRDAL.deleteARRRMachineryRatesDAL(rateCode);
        }

        public Array ListMachineryRatesBAL(int page, int rows, string sidx, string sord, int Year, out long totalRecords)
        {
            return objIARRRDAL.ListMachineryRatesDAL(page, rows, sidx, sord, Year, out totalRecords);
        }

        public Array ListMachineryFormBAL(int page, int rows, string sidx, string sord, out long totalRecords)
        {
            return objIARRRDAL.ListMachineryFormDAL(page, rows, sidx, sord, out totalRecords);
        }

        public bool addBulkARRRMachineryRatesBAL(IEnumerable<LabourDetails> machineryData, ref string message)
        {
            return objIARRRDAL.addBulkARRRMachineryRatesDAL(machineryData, ref message);
        }

        public bool FinalizeAllMachineryRatesBAL(int year)
        {
            return objIARRRDAL.FinalizeAllMachineryRatesDAL(year);
        }


        #endregion

        #region Analysis of Rates
        /*
        public bool copyARRRAnalysisRatesBAL(int frmyear, int toyear, ref string message)
        { 
            return objIARRRDAL.copyARRRAnalysisRatesDAL(frmyear, toyear, ref message);
        }
        public List<SelectListItem> PopulateAnalysisYearsBAL(bool flg)
        {
            return objIARRRDAL.PopulateAnalysisYearsDAL(flg);
        }

        public bool AnalysisRateApproveBAL(int rateCode)
        {
            return objIARRRDAL.AnalysisRateApproveDAL(rateCode);
        }

        public bool AnalysisRateFinalizeBAL(int rateCode)
        {
            return objIARRRDAL.AnalysisRateFinalizeDAL(rateCode);
        }

        public bool addARRRAnalysisRatesBAL(PMGSY.Models.ARRR.AnalysisRatesViewModel model, ref string message)
        {
            return objIARRRDAL.addARRRAnalysisRatesDAL(model, ref message);
        }

        public bool editARRRAnalysisRatesBAL(PMGSY.Models.ARRR.AnalysisRatesViewModel model, ref string message)
        {
            return objIARRRDAL.editARRRAnalysisRatesDAL(model, ref message);
        }

        public bool deleteARRRAnalysisRatesBAL(int rateCode)
        {
            return objIARRRDAL.deleteARRRAnalysisRatesDAL(rateCode);
        }

        public Array ListAnalysisRatesBAL(int page, int rows, string sidx, string sord, out long totalRecords, int year)
        {
            return objIARRRDAL.ListAnalysisRatesDAL(page, rows, sidx, sord, out totalRecords, year);
        }
        */
        #endregion

        #region Schedule of Rates
        public List<SelectListItem> PopulateScheduleMajorItemsListBAL(bool flg, int itemCode)
        {
            return objIARRRDAL.PopulateScheduleMajorItemsList(flg, itemCode);
        }
        public List<SelectListItem> PopulateMinorItemsListBAL(bool flg, int ItemCode)
        {
            return objIARRRDAL.PopulateMinorItemsList(flg, ItemCode);
        }

        public Array ListScheduleChapterItemsBAL(int page, int rows, string sidx, string sord, out long totalRecords, int chapter)
        {
            return objIARRRDAL.ListScheduleChapterItemsDAL(page, rows, sidx, sord, out totalRecords, chapter);
        }

        public Array ListScheduleLMMBAL(int page, int rows, string sidx, string sord, out long totalRecords, int lmmType, int itemCode)
        {
            return objIARRRDAL.ListScheduleLMMDAL(page, rows, sidx, sord, out totalRecords, lmmType, itemCode);
        }

        public bool addScheduleLMMBAL(PMGSY.Models.ARRR.ScheduleRatesViewModel model, ref string message)
        {
            return objIARRRDAL.addScheduleLMMDAL(model, ref message);
        }

        public bool editScheduleLMMBAL(PMGSY.Models.ARRR.ScheduleRatesViewModel model, ref string message)
        {
            return objIARRRDAL.editScheduleLMMDAL(model, ref message);
        }

        public bool ScheduleFinalizeBAL(int chapter)
        {
            return objIARRRDAL.ScheduleFinalizeDAL(chapter);
        }

        public bool deleteScheduleLMMBAL(int scheduleCode)
        {
            return objIARRRDAL.deleteScheduleLMMDAL(scheduleCode);
        }

        public bool addScheduleTaxBAL(PMGSY.Models.ARRR.TaxScheduleViewModel model, ref string message)
        {
            return objIARRRDAL.addScheduleTaxDAL(model, ref message);
        }
        public bool editScheduleTaxBAL(PMGSY.Models.ARRR.TaxScheduleViewModel model, ref string message)
        {
            return objIARRRDAL.editScheduleTaxDAL(model, ref message);
        }
        public bool deleteScheduleTaxBAL(int taxCode)
        {
            return objIARRRDAL.deleteScheduleTaxDAL(taxCode);
        }
        public Array ListScheduleTaxBAL(int page, int rows, string sidx, string sord, out long totalRecords, int itemCode)
        {
            return objIARRRDAL.ListScheduleTaxDAL(page, rows, sidx, sord, out totalRecords, itemCode);
        }
        public List<SelectListItem> PopulateTaxListBAL(bool flg)
        {
            return objIARRRDAL.PopulateTaxListDAL(flg);
        }

        #endregion

        #region Rohit Code

        #region Add Chapter Screen
        public bool AddChapterDetailsBAL(Chapter ch, ref string message)
        {
            objIARRRDAL = new ARRRDAL();
            return objIARRRDAL.AddChapterDetailsDAL(ch, ref message);
        }

        public Array GetChapterDetailsListBAL(int? page, int? rows, string sidx, string sord, out long totalRecords)
        {
            objIARRRDAL = new ARRRDAL();
            return objIARRRDAL.GetChapterDetailsListDAL(page, rows, sidx, sord, out totalRecords);
        }

        public Chapter GetChapterDetailsBAL(int chapterCode)
        {
            objIARRRDAL = new ARRRDAL();
            return objIARRRDAL.GetChapterDetailsDAL(chapterCode);
        }
        public bool EditChapterDetailsBAL(Chapter chapterModel, ref string message)
        {
            objIARRRDAL = new ARRRDAL();
            return objIARRRDAL.EditChapterDetailsDAL(chapterModel, ref message);
        }

        public bool DeleteChapterDetailsBAL(int chapterCode)
        {
            objIARRRDAL = new ARRRDAL();
            return objIARRRDAL.DeleteChapterDetailsDAL(chapterCode);
        }

        #endregion

        #region Add Material Screen
        public bool AddMaterialDetailsBAL(Material ch, ref string message)
        {
            objIARRRDAL = new ARRRDAL();
            return objIARRRDAL.AddMaterialDetailsDAL(ch, ref message);
        }


        public Array GetMaterialDetailsListBAL(int? page, int? rows, string sidx, string sord, out long totalRecords)
        {
            objIARRRDAL = new ARRRDAL();
            return objIARRRDAL.GetMaterialDetailsListDAL(page, rows, sidx, sord, out totalRecords);
        }

        public Material GetMaterialDetailsBAL(int materialCode)
        {
            objIARRRDAL = new ARRRDAL();
            return objIARRRDAL.GetMaterialDetailsDAL(materialCode);
        }
        public bool EditMaterialDetailsBAL(Material chapterModel, ref string message)
        {
            objIARRRDAL = new ARRRDAL();
            return objIARRRDAL.EditMaterialDetailsDAL(chapterModel, ref message);
        }

        public bool DeleteMaterialDetailsBAL(int materialCode)
        {
            objIARRRDAL = new ARRRDAL();
            return objIARRRDAL.DeleteMaterialDetailsDAL(materialCode);
        }
        #endregion

        #region Add Labour Screen

        public bool ADDLabourBAL(Labour ch, ref string message)
        {
            objIARRRDAL = new ARRRDAL();
            return objIARRRDAL.AddLabourDetailsDAL(ch, ref message);
        }


        public Array GetLabourListBAL(int? page, int? rows, string sidx, string sord, out long totalRecords)
        {
            objIARRRDAL = new ARRRDAL();
            return objIARRRDAL.GetLabourDetailsListDAL(page, rows, sidx, sord, out totalRecords);
        }

        public Labour GetLabourDetailsBAL(int materialCode)
        {
            objIARRRDAL = new ARRRDAL();
            return objIARRRDAL.GetLabourDetailsDAL(materialCode);
        }
        public bool EditLabourDetailsBAL(Labour chapterModel, ref string message)
        {
            objIARRRDAL = new ARRRDAL();
            return objIARRRDAL.EditLabourDetailsDAL(chapterModel, ref message);
        }

        public bool DeleteLabDetailsBAL(int materialCode)
        {
            objIARRRDAL = new ARRRDAL();
            return objIARRRDAL.DeleteLabourDetailsDAL(materialCode);
        }

        #endregion

        #region Add Category Screen
        //Save
        public bool AddCategoryDetailsBAL(CategoryViewModel ch, ref string message)
        {
            objIARRRDAL = new ARRRDAL();
            return objIARRRDAL.AddCategoryDetailsDAL(ch, ref message);
        }

        //List
        public Array GetCategoryDetailsListBAL(int? page, int? rows, string sidx, string sord, out long totalRecords)
        {
            objIARRRDAL = new ARRRDAL();
            return objIARRRDAL.GetCategoryDetailsListDAL(page, rows, sidx, sord, out totalRecords);
        }

        //Edit
        public CategoryViewModel GetCategoryDetailsBAL(int categoryCode)
        {
            objIARRRDAL = new ARRRDAL();
            return objIARRRDAL.GetCategoryDetailsBAL(categoryCode);
        }

        //Update
        public bool EditCategoryDetailsBAL(CategoryViewModel chapterModel, ref string message)
        {
            objIARRRDAL = new ARRRDAL();
            return objIARRRDAL.EditCategoryDetailsDAL(chapterModel, ref message);
        }


        //Delete
        public bool DeleteCategoryDetailsBAL(int categoryCode)
        {
            objIARRRDAL = new ARRRDAL();
            return objIARRRDAL.DeleteCategoryDetailsDAL(categoryCode);
        }


        #endregion

        #region Add Tax Screen

        public bool AddTaxDetailsBAL(TaxViewModel ch, ref string message)
        {
            objIARRRDAL = new ARRRDAL();
            return objIARRRDAL.AddTaxDetailsDAL(ch, ref message);
        }

        public Array GetTaxDetailsListBAL(int? page, int? rows, string sidx, string sord, out long totalRecords)
        {
            objIARRRDAL = new ARRRDAL();
            return objIARRRDAL.GetTaxDetailsListDAL(page, rows, sidx, sord, out totalRecords);
        }

        public TaxViewModel GetTaxDetailsBAL(int taxCode)
        {
            objIARRRDAL = new ARRRDAL();
            return objIARRRDAL.GetTaxDetailsDAL(taxCode);
        }

        public bool EditTaxDetailsBAL(TaxViewModel chapterModel, ref string message)
        {
            objIARRRDAL = new ARRRDAL();
            return objIARRRDAL.EditTaxDetailsDAL(chapterModel, ref message);
        }

        public bool DeleteTaxDetailsBAL(int taxCode)
        {
            objIARRRDAL = new ARRRDAL();
            return objIARRRDAL.DeleteTaxDetailsDAL(taxCode);
        }

        #endregion

        #endregion
    }

    public interface IARRRBAL
    {
        #region
        List<SelectListItem> PopulateChapterListBAL(bool flg);
        List<SelectListItem> PopulateItemsListBAL(bool flg, int headCode);
        List<SelectListItem> PopulateMajorItemsListBAL(bool flg, int ItemCode);
        List<SelectListItem> PopulateMajorItemsListItemwiseBAL(bool flg, int itemCode);

        bool addARRRChaptersBAL(PMGSY.Models.ARRR.ChapterItemsViewModel model, ref string message);
        bool changeChapterstatusBAL(int ItemCode);
        bool editARRRChaptersBAL(PMGSY.Models.ARRR.ChapterItemsViewModel model, ref string message);

        bool deleteARRRChaptersBAL(int ItemCode);

        Array ListChapterItemsBAL(int page, int rows, string sidx, string sord, out long totalRecords);
        #endregion

        #region
        List<SelectListItem> PopulateCategoryBAL(bool flg, int lmmType);
        List<SelectListItem> PopulateUsageUnitBAL();
        List<SelectListItem> PopulateOutputUnitBAL();

        bool addARRRMachineryBAL(PMGSY.Models.ARRR.MachineryMasterViewModel model, ref string message);
        bool changeMachineryMasterstatusBAL(int lmmCode);
        bool editARRRMachineryBAL(PMGSY.Models.ARRR.MachineryMasterViewModel model, ref string message);

        bool deleteARRRMachineryBAL(int ItemCode);

        Array ListMachineryMasterBAL(int page, int rows, string sidx, string sord, out long totalRecords);
        #endregion

        #region Machinery Rates
        List<SelectListItem> PopulateLMMItemsListBAL(bool flg, int lmmtype, int category);
        bool MaterialRateFinalizeBAL(int rateCode);
        bool addARRRMaterialRatesBAL(PMGSY.Models.ARRR.LMMRateViewModel model, ref string message);
        bool changeLMMRatestatusBAL(int rateCode);
        bool editARRRMaterialRatesBAL(PMGSY.Models.ARRR.LMMRateViewModel model, ref string message);

        bool deleteARRRMaterialRatesBAL(int rateCode);

        Array ListMaterialRatesBAL(int page, int rows, string sidx, string sord, int Year, out long totalRecords);

        Array ListMaterialFormBAL(int page, int rows, string sidx, string sord, out long totalRecords);
        bool addBulkARRRMaterialRatesBAL(IEnumerable<LabourDetails> materialData, ref string message);
        bool FinalizeAllMaterialRatesBAL(int year); 
        
        #endregion

        #region Labour Rates
        bool LabourRateFinalizeBAL(int rateCode);
        bool addARRRLabourRatesBAL(PMGSY.Models.ARRR.LMMRateViewModel model, ref string message);
        bool editARRRLabourRatesBAL(PMGSY.Models.ARRR.LMMRateViewModel model, ref string message);

        bool deleteARRRLabourRatesBAL(int rateCode);

       

        Array ListLabourRatesBAL(int page, int rows, string sidx, string sord, int Year, out long totalRecords);

        Array ListLabourFormBAL(int page, int rows, string sidx, string sord, out long totalRecords); //Added by Aditi on 21 Aug 2020
        bool addBulkARRRLabourRatesBAL(IEnumerable<LabourDetails> labourData, ref string message); //Added by Aditi on 27 Aug 2020
        bool FinalizeAllLabourRatesBAL(int year); //Added by Aditi on 1 Sept 2020
        
        #endregion

        #region Machinery Rates
        bool MachineryRateFinalizeBAL(int rateCode);
        bool addARRRMachineryRatesBAL(PMGSY.Models.ARRR.LMMRateViewModel model, ref string message);
        bool editARRRMachineryRatesBAL(PMGSY.Models.ARRR.LMMRateViewModel model, ref string message);

        bool deleteARRRMachineryRatesBAL(int rateCode);

        Array ListMachineryRatesBAL(int page, int rows, string sidx, string sord, int Year, out long totalRecords);

        Array ListMachineryFormBAL(int page, int rows, string sidx, string sord, out long totalRecords);
        bool addBulkARRRMachineryRatesBAL(IEnumerable<LabourDetails> machineryData, ref string message);
        bool FinalizeAllMachineryRatesBAL(int year); 
        #endregion

        #region Analysis of Rates
        /*
        List<SelectListItem> PopulateAnalysisYearsBAL(bool flg);
        bool AnalysisRateApproveBAL(int rateCode);
        
        bool AnalysisRateFinalizeBAL(int rateCode);
        bool addARRRAnalysisRatesBAL(PMGSY.Models.ARRR.AnalysisRatesViewModel model, ref string message);
        bool editARRRAnalysisRatesBAL(PMGSY.Models.ARRR.AnalysisRatesViewModel model, ref string message);

        bool deleteARRRAnalysisRatesBAL(int rateCode);

        Array ListAnalysisRatesBAL(int page, int rows, string sidx, string sord, out long totalRecords, int year);

        bool copyARRRAnalysisRatesBAL(int frmyear, int toyear, ref string message);
        */
        #endregion

        #region Schedule of Rates
        List<SelectListItem> PopulateScheduleMajorItemsListBAL(bool flg, int itemCode);
        List<SelectListItem> PopulateMinorItemsListBAL(bool flg, int ItemCode);
        Array ListScheduleChapterItemsBAL(int page, int rows, string sidx, string sord, out long totalRecords, int chapter);
        Array ListScheduleLMMBAL(int page, int rows, string sidx, string sord, out long totalRecords, int lmmType, int itemCode);

        bool addScheduleLMMBAL(PMGSY.Models.ARRR.ScheduleRatesViewModel model, ref string message);
        bool editScheduleLMMBAL(PMGSY.Models.ARRR.ScheduleRatesViewModel model, ref string message);
        bool ScheduleFinalizeBAL(int chapter);
        bool deleteScheduleLMMBAL(int scheduleCode);

        List<SelectListItem> PopulateTaxListBAL(bool flg);
        Array ListScheduleTaxBAL(int page, int rows, string sidx, string sord, out long totalRecords, int itemCode);
        bool addScheduleTaxBAL(PMGSY.Models.ARRR.TaxScheduleViewModel model, ref string message);
        bool editScheduleTaxBAL(PMGSY.Models.ARRR.TaxScheduleViewModel model, ref string message);
        bool deleteScheduleTaxBAL(int scheduleCode);
        #endregion

        #region Rohit Code

        #region Add Chapter Screen
        bool AddChapterDetailsBAL(Chapter ch, ref string message);
        Array GetChapterDetailsListBAL(int? page, int? rows, string sidx, string sord, out long totalRecords);
        Chapter GetChapterDetailsBAL(int chapterCode);
        bool EditChapterDetailsBAL(Chapter chapterModel, ref string message);
        bool DeleteChapterDetailsBAL(int chapterCode);
        #endregion

        #region Add Material Screen

        bool AddMaterialDetailsBAL(Material ch, ref string message);
        Array GetMaterialDetailsListBAL(int? page, int? rows, string sidx, string sord, out long totalRecords);
        Material GetMaterialDetailsBAL(int materialCode);

        bool EditMaterialDetailsBAL(Material chapterModel, ref string message);
        bool DeleteMaterialDetailsBAL(int materialCode);

        #endregion

        #region Add Labour Screen

        bool ADDLabourBAL(Labour ch, ref string message);

        Array GetLabourListBAL(int? page, int? rows, string sidx, string sord, out long totalRecords);

        Labour GetLabourDetailsBAL(int materialCode);

        bool EditLabourDetailsBAL(Labour chapterModel, ref string message);
        bool DeleteLabDetailsBAL(int materialCode);


        #endregion

        #region Add Category Screen

        bool AddCategoryDetailsBAL(CategoryViewModel ch, ref string message);
        Array GetCategoryDetailsListBAL(int? page, int? rows, string sidx, string sord, out long totalRecords);
        CategoryViewModel GetCategoryDetailsBAL(int categoryCode);

        bool EditCategoryDetailsBAL(CategoryViewModel chapterModel, ref string message);

        bool DeleteCategoryDetailsBAL(int categoryCode);


        #endregion

        #region Add Tax Screen

        bool AddTaxDetailsBAL(TaxViewModel ch, ref string message);

        Array GetTaxDetailsListBAL(int? page, int? rows, string sidx, string sord, out long totalRecords);
        TaxViewModel GetTaxDetailsBAL(int taxCode);
        bool EditTaxDetailsBAL(TaxViewModel chapterModel, ref string message);
        bool DeleteTaxDetailsBAL(int taxCode);

        #endregion

        #endregion
    }


}