using PMGSY.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PMGSY.Filters;

namespace PMGSY.Models.Accounts
{
    public class DashBoardModel
    {

        public DashBoardModel()
        {

            CommonFunctions objCommon = new CommonFunctions();
            this.MONTH = Convert.ToInt16(DateTime.Now.Month);
            this.YEAR = Convert.ToInt16(DateTime.Now.Year);
        }

        [Range(0, Int16.MaxValue, ErrorMessage = "Invalid Month")]
        public short MONTH { get; set; }
        public List<SelectListItem> YEAR_LIST { get; set; }


        [Range(0, Int16.MaxValue, ErrorMessage = "Invalid Year")]
        public short YEAR { get; set; }
        public List<SelectListItem> MONTH_LIST { get; set; }


        [Range(0, Int16.MaxValue, ErrorMessage = "Invalid DPIU")]
        public short DPIU { get; set; }
        public List<SelectListItem> DPIU_LIST { get; set; }

        public short LEVEL { get; set; }

        //Added By Abhishek kamble 27-Feb-2014
        public int SRRDA { get; set; }
        public List<SelectListItem> SRRDA_LIST { get; set; }
    }

    public class charModel
    {
        public string x { get; set; }
        public string name { get; set; }
        public string y { get; set; }
        public string fundType { get; set; }
        public string AssetOrLia { get; set; }
        public string z { get; set; }
        public string headCode { get; set; }
        public int id { get; set; }
        public charModel()
        {

        }
    }

}