#region File Header
/*
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   FormReportsBAL.cs        
        * Description   :   Business Logic for all types of Form Reports
        * Author        :   Shyam Yadav 
        * Creation Date :   28/August/2013
 **/
#endregion

using PMGSY.DAL.FormReports;
using PMGSY.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMGSY.BAL.FormReports
{
    public class FormReportsBAL : IFormReportsBAL
    {
        private IFormReportsDAL formReportsDAL;
        private PMGSYEntities dbContext;


        #region Form1

        public Array Form1StateLevelListingBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords)
        {
            formReportsDAL = new FormReportsDAL();
            return formReportsDAL.Form1StateLevelListingDAL(page, rows, sidx, sord, out totalRecords);
        }


        public Array Form1DistrictLevelListingBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int stateCode)
        {
            formReportsDAL = new FormReportsDAL();
            return formReportsDAL.Form1DistrictLevelListingDAL(page, rows, sidx, sord, out totalRecords, stateCode);
        }


        public Array Form1BlockLevelListingBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int stateCode, int districtCode)
        {
            formReportsDAL = new FormReportsDAL();
            return formReportsDAL.Form1BlockLevelListingDAL(page, rows, sidx, sord, out totalRecords, stateCode, districtCode);
        }


        public Array Form1VillageLevelListingBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int stateCode, int districtCode, int blockCode)
        {
            formReportsDAL = new FormReportsDAL();
            return formReportsDAL.Form1VillageLevelListingDAL(page, rows, sidx, sord, out totalRecords, stateCode, districtCode, blockCode);
        }

        #endregion



        #region Form2


        public Array Form2StateLevelListingBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, string constType, int constCode)
        {
            formReportsDAL = new FormReportsDAL();
            return formReportsDAL.Form2StateLevelListingDAL(page, rows, sidx, sord, out totalRecords,constType,constCode);
        }


        public Array Form2DistrictLevelListingBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int stateCode, string constType, int constCode)
        {
            formReportsDAL = new FormReportsDAL();
            return formReportsDAL.Form2DistrictLevelListingDAL(page, rows, sidx, sord, out totalRecords, stateCode, constType,constCode);
        }


        public Array Form2ConstituencyLevelListingBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int stateCode, int constCode, string constType)
        {
            formReportsDAL = new FormReportsDAL();
            return formReportsDAL.Form2ConstituencyLevelListingDAL(page, rows, sidx, sord, out totalRecords, stateCode, constCode, constType);
        }


        #endregion


        #region Form3

        public Array Form3StateLevelListingBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords)
        {
            formReportsDAL = new FormReportsDAL();
            return formReportsDAL.Form3StateLevelListingDAL(page, rows, sidx, sord, out totalRecords);
        }


        public Array Form3DistrictLevelListingBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int stateCode)
        {
            formReportsDAL = new FormReportsDAL();
            return formReportsDAL.Form3DistrictLevelListingDAL(page, rows, sidx, sord, out totalRecords, stateCode);
        }


        public Array Form3BlockLevelListingBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int stateCode, int districtCode)
        {
            formReportsDAL = new FormReportsDAL();
            return formReportsDAL.Form3BlockLevelListingDAL(page, rows, sidx, sord, out totalRecords, stateCode, districtCode);
        }


        #endregion



        #region Form4

        public Array Form4StateLevelListingBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int stateCode, int districtCode, int blockCode, int year, int batch, int collaboration, string propType)
        {
            formReportsDAL = new FormReportsDAL();
            return formReportsDAL.Form4StateLevelListingDAL(page, rows, sidx, sord, out totalRecords,stateCode,districtCode,blockCode,year,batch,collaboration,propType);
        }


        public Array Form4DistrictLevelListingBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int stateCode, int districtCode, int blockCode, int year, int batch, int collaboration, string propType)
        {
            formReportsDAL = new FormReportsDAL();
            return formReportsDAL.Form4DistrictLevelListingDAL(page, rows, sidx, sord, out totalRecords, stateCode, districtCode, blockCode, year, batch, collaboration, propType);
        }


        public Array Form4BlockLevelListingBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int stateCode, int districtCode, int blockCode, int year, int batch, int collaboration, string propType)
        {
            formReportsDAL = new FormReportsDAL();
            return formReportsDAL.Form4BlockLevelListingDAL(page, rows, sidx, sord, out totalRecords, stateCode, districtCode, blockCode, year, batch, collaboration, propType);
        }

        public Array Form4FinalLevelListingBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int stateCode, int districtCode, int blockCode, int year, int batch, int collaboration, string propType)
        {
            formReportsDAL = new FormReportsDAL();
            return formReportsDAL.Form4FinalLevelListingDAL(page, rows, sidx, sord, out totalRecords, stateCode, districtCode, blockCode, year, batch, collaboration, propType);
   
        }
       
        #endregion



        #region Form5/6

        public Array Form5StateLevelListingBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int year, string constType, int constCode)
        {
            formReportsDAL = new FormReportsDAL();
            return formReportsDAL.Form5StateLevelListingDAL(page, rows, sidx, sord, out totalRecords, year,constType,constCode);
        }


        public Array Form5DistrictLevelListingBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int year, int stateCode , string constType, int constCode)
        {
            formReportsDAL = new FormReportsDAL();
            return formReportsDAL.Form5DistrictLevelListingDAL(page, rows, sidx, sord, out totalRecords, year, stateCode, constType,constCode);
        }


        public Array Form5ConstituencyLevelListingBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int year, int stateCode, int constCode, string constType)
        {
            formReportsDAL = new FormReportsDAL();
            return formReportsDAL.Form5ConstituencyLevelListingDAL(page, rows, sidx, sord, out totalRecords, year, stateCode, constCode, constType);
        }


        #endregion



        #region Form7

        public Array Form7StateLevelListingBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, string propType, int batch, int year, int collaboration)
        {
            formReportsDAL = new FormReportsDAL();
            return formReportsDAL.Form7StateLevelListingDAL(page, rows, sidx, sord, out totalRecords,propType,batch,year,collaboration);
        }


        public Array Form7DistrictLevelListingBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int stateCode, string propType, int batch, int year, int collaboration)
        {
            formReportsDAL = new FormReportsDAL();
            return formReportsDAL.Form7DistrictLevelListingDAL(page, rows, sidx, sord, out totalRecords, stateCode,propType,batch,year,collaboration);
        }


        public Array Form7BlockLevelListingBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int stateCode, int districtCode, string ProposalType, int batch, int year, int collaboration)
        {
            formReportsDAL = new FormReportsDAL();
            return formReportsDAL.Form7BlockLevelListingDAL(page, rows, sidx, sord, out totalRecords, stateCode, districtCode, ProposalType, batch, year, collaboration);
        }
        public Array Form7FinalLevelListingBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int stateCode, int districtCode, int blockCode, string propType, int batch, int year, int collaboration)
        {
            formReportsDAL = new FormReportsDAL();
            return formReportsDAL.Form7FinalLevelListingDAL(page, rows, sidx, sord, out totalRecords, stateCode, districtCode,blockCode, propType, batch, year, collaboration);
      
        }

        #endregion



        #region Form8

        public Array Form8StateLevelListingBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, string propType, int batch, int year, int collaboration)
        {
            formReportsDAL = new FormReportsDAL();
            return formReportsDAL.Form8StateLevelListingDAL(page, rows, sidx, sord, out totalRecords, propType, batch, year, collaboration);
        }


        public Array Form8DistrictLevelListingBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int stateCode, string propType, int batch, int year, int collaboration)
        {
            formReportsDAL = new FormReportsDAL();
            return formReportsDAL.Form8DistrictLevelListingDAL(page, rows, sidx, sord, out totalRecords, stateCode, propType, batch, year, collaboration);
        }


        public Array Form8BlockLevelListingBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int stateCode, int districtCode, string ProposalType, int batch, int year, int collaboration)
        {
            formReportsDAL = new FormReportsDAL();
            return formReportsDAL.Form8BlockLevelListingDAL(page, rows, sidx, sord, out totalRecords, stateCode, districtCode, ProposalType, batch, year, collaboration);
        }
        public   Array Form8FinalLevelListingBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int stateCode, int districtCode, int blockCode, string proposalType, int batch, int year, int collaboration)
        {
            formReportsDAL = new FormReportsDAL();
            return formReportsDAL.Form8FinalLevelListingDAL(page, rows, sidx, sord, out totalRecords, stateCode, districtCode,blockCode, proposalType, batch, year, collaboration);
        }


        #endregion

    }



    public interface IFormReportsBAL
    {
        #region Form1

        Array Form1StateLevelListingBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords);
        Array Form1DistrictLevelListingBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int stateCode);
        Array Form1BlockLevelListingBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int stateCode, int districtCode);
        Array Form1VillageLevelListingBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int stateCode, int districtCode, int blockCode);
        #endregion


        #region Form2

        Array Form2StateLevelListingBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, string constType, int constCode);
        Array Form2DistrictLevelListingBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int stateCode, string constType, int constCode);
        Array Form2ConstituencyLevelListingBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int stateCode, int constCode, string constType);
        #endregion


        #region Form3
        Array Form3StateLevelListingBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords);
        Array Form3DistrictLevelListingBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int stateCode);
        Array Form3BlockLevelListingBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int stateCode, int districtCode);
        #endregion


        #region Form4
        Array Form4StateLevelListingBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int stateCode, int districtCode, int blockCode, int year, int batch, int collaboration, string propType);
        Array Form4DistrictLevelListingBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int stateCode, int districtCode, int blockCode, int year, int batch, int collaboration, string propType);
        Array Form4BlockLevelListingBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int stateCode, int districtCode, int blockCode, int year, int batch, int collaboration, string propType);
        Array Form4FinalLevelListingBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int stateCode, int districtCode, int blockCode, int year, int batch, int collaboration, string propType);
        #endregion


        #region Form5/6
        Array Form5StateLevelListingBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int year, string constType, int constCode);
        Array Form5DistrictLevelListingBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int year, int stateCode, string constType, int constCode);
        Array Form5ConstituencyLevelListingBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int year, int stateCode, int constCode, string constType);
        #endregion


        #region Form7
        Array Form7StateLevelListingBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, string propType, int batch, int year, int collaboration);
        Array Form7DistrictLevelListingBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int stateCode, string propType, int batch, int year, int collaboration);
        Array Form7BlockLevelListingBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int stateCode, int districtCode, string ProposalType, int batch, int year, int collaboration);
        Array Form7FinalLevelListingBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int stateCode, int districtCode, int blockCode, string propType, int batch, int year, int collaboration);
        #endregion


        #region Form8
        Array Form8StateLevelListingBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, string propType, int batch, int year, int collaboration);
        Array Form8DistrictLevelListingBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int stateCode, string propType, int batch, int year, int collaboration);
        Array Form8BlockLevelListingBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int stateCode, int districtCode, string ProposalType, int batch, int year, int collaboration);
        Array Form8FinalLevelListingBAL(int page, int rows, string sidx, string sord, out Int32 totalRecords, int stateCode, int districtCode, int blockCode, string proposalType, int batch, int year, int collaboration);
        #endregion
    }
}