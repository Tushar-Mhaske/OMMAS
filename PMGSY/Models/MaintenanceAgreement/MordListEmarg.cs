using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.MaintenanceAgreement
{
    public class MordListEmarg
    {

        public string StateName { get; set; }
        public int Mast_State_Code { get; set; }



        [Required(ErrorMessage = "State required")]
        [Range(0, int.MaxValue, ErrorMessage = "Invalid state")]
        [Display(Name = "State")]
        public int StateCode { get; set; }
        public List<SelectListItem> StateList { get; set; }

        [Required(ErrorMessage = "Please select District.")]
        [Range(0, int.MaxValue, ErrorMessage = "Please select District.")]
   
        [Display(Name = "District")]
        public int[] DistrictCode { get; set; }
     
     //   public List<MultiSelectList> DistrictList { get; set; }

        //[Display(Name = "Package List")]
        //public string[] PackageListArray { get; set; }


        //public MultiSelectList PackageList
        //{
        //    get
        //    {
        //        PMGSY.DAL.MaintenanceAgreement.MaintenanceAgreementDAL objDAL = new PMGSY.DAL.MaintenanceAgreement.MaintenanceAgreementDAL();
        //        var distList = objDAL.MordEmargDALPackage(this.DistrictCode);

        //     //   
        //       // distList = ;
        //        return new SelectList(distList, "MAST_DISTRICT_CODE", "MAST_DISTRICT_NAME", this.PackageListArray);

        //    }


        //}



        public MultiSelectList DistrictList
        {
            get
            {

                List<MASTER_DISTRICT> distList = new List<MASTER_DISTRICT>();

                PMGSY.DAL.Master.IMasterDAL objDAL = new PMGSY.DAL.Master.MasterDAL();
                distList = objDAL.GetAllDistrictByStateCode(this.StateCode);
                return new SelectList(distList, "MAST_DISTRICT_CODE", "MAST_DISTRICT_NAME", this.DistrictCode);

            }


        }

        //public MultiSelectList DistrictList
        //{
        //    get
        //    {

        //        List<MASTER_DISTRICT> distList = new List<MASTER_DISTRICT>();

        //         PMGSY.DAL.Master.IMasterDAL objDAL = new PMGSY.DAL.Master.MasterDAL();
        //         distList = objDAL.GetAllDistrictByStateCode(this.StateCode);
        //        return new SelectList(distList, "MAST_DISTRICT_CODE", "MAST_DISTRICT_NAME", this.DistrictCode);

        //    }
           

        //}

       

       


    }
}