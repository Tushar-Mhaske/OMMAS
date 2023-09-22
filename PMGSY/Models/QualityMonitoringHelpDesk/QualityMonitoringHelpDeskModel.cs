#region File Header
/*
        * Project Id    :
        * Project Name  :   OMMAS II
        * Name          :   QMFIlterViewModel.cs        
        * Description   :   All filters will be loaded using this model
        * Author        :   Deepak Madane
        * Creation Date :   03/April/2013
 **/
#endregion
using PMGSY.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace PMGSY.Models.QualityMonitoringHelpDesk
{
    public class QualityMonitoringHelpDeskModel
    {
        public QualityMonitoringHelpDeskModel()
        {
            List<SelectListItem> ddMeesage = new List<SelectListItem>();
            SelectListItem item = new SelectListItem();
            item = new SelectListItem();
            item.Text = "Select Message";
            item.Value = "0";
            item.Selected = true;
            ddMeesage.Add(item);

            //item = new SelectListItem();
            //item.Text = "BroadCast";
            //item.Value = "B";
            //ddMeesage.Add(item);

            item = new SelectListItem();
            item.Text = "General";
            item.Value = "G";
            ddMeesage.Add(item);

            item = new SelectListItem();
            item.Text = "Application";
            item.Value = "A";
            ddMeesage.Add(item);

            item = new SelectListItem();
            item.Text = "System Generated";
            item.Value = "S";
            ddMeesage.Add(item);          
           
            Message_TypeLIST = ddMeesage;
            List<SelectListItem> ddMonitor = new List<SelectListItem>();
            ddMonitor.Insert(0, (new SelectListItem { Text = "Select Monitor", Value = "0", Selected = true }));
            Monitor_LIST = ddMonitor;
            CommonFunctions objCommonFunctions = new CommonFunctions();
            State_LIST = objCommonFunctions.PopulateStates(false);
            State_LIST.Insert(0, (new SelectListItem { Text = "All State", Value = "0", Selected = true }));

        }
        [Display(Name = "Message Description")]
        [Required(ErrorMessage = "Message Description  is required.")]
        [StringLength(1000, ErrorMessage = "Message Description is ust be less than 1000 characters.")]
        //[RegularExpression(@"^([a-zA-Z0-9 ._!,-]+)$", ErrorMessage = "Message Description is not in valid format.")] 
        //[RegularExpression(@"^(?![0-9]*$)[a-zA-Z0-9 ._!,-]+$", ErrorMessage = "Message Description is not in valid format.")] //working       
        public string MESSAGE_TEXT { get; set; }

        [Display(Name = "Message Type")]
        [RegularExpression("[GAS]", ErrorMessage = "Please select Message Type.")]      
        public string MESSAGE_TYPE { get; set; }              
        public List<SelectListItem> Message_TypeLIST { set; get; }
        
        [Display(Name = "Monitor")]
        [Range(1, 2147483647, ErrorMessage = "Please select Monitor.")]      
        public int Monitor_CODE { get; set; }       
        public List<SelectListItem> Monitor_LIST { get; set; }

        public bool IS_DOWNLOAD { get; set; }

        [Display(Name = "State")]
        [Range(0, 2147483647, ErrorMessage = "Please select State.")]
        public int MAST_STATE_CODE { get; set; }
        public List<SelectListItem> State_LIST { get; set; }

        public string QM_Type{get;set;}
        public int User_Id { get; set; }
        public int Message_Id { get; set; }
        public System.DateTime TIME_STAMP { get; set; }
      
    }
   
}