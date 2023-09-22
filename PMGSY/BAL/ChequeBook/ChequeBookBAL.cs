using PMGSY.DAL.ChequeBook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PMGSY.Models.ChequeBook;

namespace PMGSY.BAL.ChequeBook
{
    public class ChequeBookBAL : IChequeBookBAL
    {
        IChequeBookDAL objDAL = new ChequeBookDAL();
        public Array ChequeBookList(int page, int rows, string sidx, string sord, string search, out long totalRecords, int AdminNdCode, int LevelId)
        {
            return objDAL.ChequeBookList(page, rows, sidx, sord, search, out totalRecords,AdminNdCode,LevelId);
        }

        public String DeleteChequeBook(int chequeBookId)
        {
            return objDAL.DeleteChequeBook(chequeBookId);
        }

        public String ValidateAddEditChequeBookDetails(ChequeBookViewModel chequeBookViewModel)
        {
            return objDAL.ValidateAddEditChequeBookDetails(chequeBookViewModel);
        }

        public bool AddCBBAL(ChequeBookDetailsViewModel masterAgencyViewModel, ref string message)
        {
            //  objDAL = new MasterDAL();
            return objDAL.ADDCBDAL(masterAgencyViewModel, ref message);
        }


        public Array CBList(int page, int rows, string sidx, string sord, string search, out long totalRecords, int AdminNdCode, int LevelId, string AccChqType)
        {
            return objDAL.CBDAL(page, rows, sidx, sord, search, out totalRecords, AdminNdCode, LevelId, AccChqType);
        }


        public String DeleteCBBAL(int chequeBookId)
        {
            return objDAL.DeleteChequeBookDetails(chequeBookId);
        }

    }

    public interface IChequeBookBAL
    {
        Array ChequeBookList(int page, int rows, string sidx, string sord, string search, out long totalRecords,int AdminNdCode,int LevelId);
        String DeleteChequeBook(int chequeBookId);
        String ValidateAddEditChequeBookDetails(ChequeBookViewModel chequeBookViewModel);

        // 15 March 2018 [Rohit]
        bool AddCBBAL(ChequeBookDetailsViewModel model, ref string message);
        Array CBList(int page, int rows, string sidx, string sord, string search, out long totalRecords, int AdminNdCode, int LevelId, string AccChqType);
        String DeleteCBBAL(int chequeBookId);
        //GetListCB
    }
}