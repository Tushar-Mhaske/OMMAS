using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PMGSY.DAL.Ticket;
using PMGSY.Models.Ticket;
using System.Web.Mvc;
namespace PMGSY.BAL.Ticket
{
    public interface ITicketBAL
    {
        Array GetTicketListBAL(int? page, int? rows, String sidx, String sord, out long totalrecords, string filters);
        Array GetALLTicketListBAL(int? page, int? rows, String sidx, String sord, out long totalrecords, string filters);
        List<SelectListItem> GetCategory();
        List<SelectListItem> GetModule();
        string GetUserRoleInfo();

        Boolean SaveTicketDetailsBAL(TicketViewModel model);
        Boolean SaveTicketFile(int ticketNo, HttpPostedFileBase file, out String message);
        Array GetTicketFileListBAL(int? page, int? rows, String sidx, String sord,int tktno ,out long totalrecords);
        TicketAcceptModel GetTicketAcceptDetailBAL(int ticketNo,out Boolean IsApproved);
        TicketReplyModel GetTicketReplyDetailBAL(int ticketNo);

        Boolean SaveApproveDetailsBAL(TicketAcceptModel model,out String message);
        Boolean SaveTicketReplyDetailsBAL(TicketReplyModel model, out string message);

        void GetReportStatisticsBAL(TicketReportViewModel model);
        Array GetTicketReportListBAL(int? page, int? rows, String sidx, String sord, out long totalrecords, string filters, string Level, String Designation, String Category, String Module, String State);
        
        //added by rohit borse on 14-07-2022
        string GetForwardToDetailsBAL(int moduleId);
    }


    public class TicketingBAL   :  ITicketBAL
    {
        ITicketDAL objTicketDAL = null;

        public Array GetTicketListBAL(int? page, int? rows, string sidx, string sord, out long totalrecords, string filters)
        {
 	       objTicketDAL = new TicketingDAL();
            return objTicketDAL.GetTicketListDAL(page:page,rows:rows,sidx:sidx,sord:sord,totalrecords: out totalrecords,filters:filters);
        }


        public Array GetALLTicketListBAL(int? page, int? rows, string sidx, string sord, out long totalrecords, string filters)
        {
            objTicketDAL = new TicketingDAL();
            return objTicketDAL.GetAllTicketListDAL(page: page, rows: rows, sidx: sidx, sord: sord, totalrecords: out totalrecords, filters: filters);
        }

       public List<SelectListItem> GetCategory()
        {
            objTicketDAL = new TicketingDAL();
            return objTicketDAL.GetCategory();
        }

       public List<SelectListItem> GetModule()
        {
            objTicketDAL = new TicketingDAL();
            return objTicketDAL.GetModule();
        }

       public String GetUserRoleInfo()
       {
           objTicketDAL = new TicketingDAL();
           return objTicketDAL.GetUserRoleInfo();
       }


       public bool SaveTicketDetailsBAL(TicketViewModel model)
       {
           objTicketDAL = new TicketingDAL();
           return objTicketDAL.SaveTicketDetailsDAL(model);
       }


       public bool SaveTicketFile(int ticketNo, HttpPostedFileBase file, out string message)
       {
           objTicketDAL = new TicketingDAL();
           return objTicketDAL.SaveTicketFile(ticketNo, file, out message); ;
       }

       public Array GetTicketFileListBAL(int? page, int? rows, String sidx, String sord,int tktno ,out long totalrecords)
       {
            objTicketDAL = new TicketingDAL();
            return objTicketDAL.GetTicketFileListDAL(page, rows, sidx, sord, tktno, out totalrecords);    
       }


       public TicketAcceptModel GetTicketAcceptDetailBAL(int ticketNo,out Boolean IsApproved)
       {
           objTicketDAL = new TicketingDAL();
           return objTicketDAL.GetTicketAcceptDetailDAL(ticketNo, out IsApproved);    

       }


       public bool SaveApproveDetailsBAL(TicketAcceptModel model, out String message)
       {
           objTicketDAL = new TicketingDAL();
           return objTicketDAL.SaveApproveDetailsDAL(model,out message);   
       }


       public TicketReplyModel GetTicketReplyDetailBAL(int ticketNo)
       {
           objTicketDAL = new TicketingDAL();
           return objTicketDAL.GetTicketReplyDetailDAL(ticketNo);
       }


       public bool SaveTicketReplyDetailsBAL(TicketReplyModel model, out string message)
       {
           objTicketDAL = new TicketingDAL();
           return objTicketDAL.SaveTicketReplyDetailsDAL(model, out message);   
       }

       public void GetReportStatisticsBAL(TicketReportViewModel model)
       {
           objTicketDAL = new TicketingDAL();
           objTicketDAL.GetReportStatisticsDAL(model);
       }

       public Array GetTicketReportListBAL(int? page, int? rows, string sidx, string sord, out long totalrecords, string filters, string Level, String Designation, String Category, String Module,String State)
       {
           objTicketDAL = new TicketingDAL();
           return objTicketDAL.GetTicketReportListDAL(page, rows, sidx, sord, out totalrecords, filters, Level, Designation, Category, Module,State);
       }

        //added by rohit borse on 14-07-2022
        public string GetForwardToDetailsBAL(int moduleId)
        {
            objTicketDAL = new TicketingDAL();
            return objTicketDAL.GetForwardToDetailsDAL(moduleId);
        }
    }
}