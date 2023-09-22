
/*----------------------------------------------------------------------------------------
 * Project Id             :

 * Project Name           : OMMAS-II

 * File Name              : TenderEquipmentViewModel.cs
 
 * Author                 : Abhishek Kamble

 * Creation Date          : 20/Nov/2013

 * Desc                   : This class is used as model to apply server side validation for Tender Equipment Details.
 
 * ---------------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using PMGSY.DAL.FortyPointChecklist;

namespace PMGSY.Models.FortyPointCheckList
{
    public class TenderEquipmentViewModel
    {

        [UIHint("hidden")]
        public string EncryptedEquipmentCode { get; set; }

        public int MAST_STATE_CODE { get; set; }
        public int ADMIN_ND_CODE { get; set; }
        public int TEND_EQUIPMENT_ID { get; set; }
        public int TEND_CHECKLIST_POINTID { get; set; }

        [Display(Name="Type of Equipment")]
        [Required(ErrorMessage="Type of Equipment is required")]
        [RegularExpression("[A-Za-z0-9- .()]{1,200}", ErrorMessage = "Only alphanumeric characters are allowed.")]
        public string TEND_EQUIPMENT_TYPE { get; set; }
        
        [Display(Name="Number Of Equipment")]
        [Range(1,Int32.MaxValue,ErrorMessage="Not in valid range")]
        [RegularExpression("[0-9]{1,10}", ErrorMessage = "Only digits are allowed.")]
        public int TEND_EQUIPMENT_NUMBERS { get; set; }

        [RegularExpression("[CL]", ErrorMessage = "Please select equipment type")]  
        [Display(Name="Equipment")]     
        [Required(ErrorMessage="Equipment is required.")]                                                                
        public string TEND_EQUIPMENT_FLAG { get; set; }

        public int CheckListId { get; set; }
        public string CheckListIssue { get; set; }

        public SelectList Equipments
        {
            get {

                List<SelectListItem> lstEquipments = new List<SelectListItem>();
                IFortyPointChecklistDAL objDAL = new FortyPointChecklistDAL();
                lstEquipments = objDAL.PopulateEquipmentType();

                lstEquipments.Insert(0, new SelectListItem { Text="--Select--",Value="0"});
                return new SelectList(lstEquipments, "Value", "Text");
            }
        }

        public virtual ADMIN_DEPARTMENT ADMIN_DEPARTMENT { get; set; }
        public virtual MASTER_CHECKLIST_POINTS MASTER_CHECKLIST_POINTS { get; set; }
        public virtual MASTER_STATE MASTER_STATE { get; set; }
    }
}