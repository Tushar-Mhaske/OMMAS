using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.Ticket
{
    public enum Operation
    {
        TicketEntry,Approval,Reply
    }



    public class TicketViewModel
    {

        public TicketViewModel()
        {
            this.lstCategory = new List<SelectListItem>();
            this.LstModule = new List<SelectListItem>();
        }
        [Display(Name="Name")]
        [Required(ErrorMessage="Please enter name")]
        [RegularExpression(@"^[a-zA-Z ]+$", ErrorMessage = "Name should contain only letters")]
        [StringLength(50, ErrorMessage = "Name can be upto 50 characters only")]
        public String Name { get; set; }


        [Display(Name="Mobile")]
        [Required(ErrorMessage="Mobile no is required")]
        [RegularExpression(@"^[6-9]\d{9}",ErrorMessage= "Please enter a valid number")]
        [StringLength(10,ErrorMessage = "Mobile no Should be 10 digit only.")]
        public String Contact { get; set; }

        [Required(ErrorMessage = "Please enter email address")]
        [Display(Name="Email")]
        [EmailAddress(ErrorMessage="Please enter valid email address")]
        [DataType(DataType.EmailAddress)]
        public String Email { get; set; }

        [Display(Name="Module")]
        [Required(ErrorMessage="Please enter module")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid module")]
        public int ModuleID { get; set; }
        public List<SelectListItem> LstModule { set; get; }


        [Required(ErrorMessage = "Please select category")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid category")]
        public int Category { get; set; }
        public List<SelectListItem> lstCategory { set; get; }

        //commented by rohit borse on 20-07-2022
        [Required(ErrorMessage = "Please enter remarks")]
        //[RegularExpression(@"^[a-zA-Z0-9,().\-\/?\\ ]+$", ErrorMessage = "Remarks should contain only letters")]        
        [RegularExpression(@"^[a-zA-Z0-9,_@%&*+()‘’:.\-\/?\\""\'\r\n?|\n ]+$", ErrorMessage = "please enter remark in valid format")]
        [StringLength(1000, ErrorMessage = "Remarks can be upto 1000 characters only")]
        public String Remarks { get; set; }


        [Required(ErrorMessage = "Please enter subject")]
        [RegularExpression(@"^[a-zA-Z0-9,_().\-\/?\\ ]+$", ErrorMessage = "only letters, numbers , ()\\.-/? are allowed")]        
        [StringLength(100, ErrorMessage = "Subject can be upto 100 characters only")]
        public String Subject { get; set; }

        public string  RoleType { get; set; }
    }
}