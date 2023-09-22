using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PMGSY.Models.Ticket
{
    public class TicketAcceptModel
    {
        public TicketAcceptModel()
        {
            this.listTicketMatserDetail = new List<TicketMatserDetails>();
            this.ForwardToList = new List<SelectListItem>();
            this.listReplymodel=new List<ViewReplyModel>();
            this.AcceptRejectList = new List<SelectListItem>();
            this.AcceptRejectList.Add(new SelectListItem { Text = "Accept", Value = "1" });
            this.AcceptRejectList.Add(new SelectListItem { Text = "Reject", Value = "2" });
            this.AcceptRejectList.Insert(0,new SelectListItem {Text = "Select Action", Value = "-1" });

        }

        public Boolean IsApproved { get; set; }
        public String TicketNo { get; set; }

        // range change by rohit borse on 14-07-2022 
        [Display(Name = "Category :")]
        [Required(ErrorMessage = "Please select category")]
        [Range(1, 9, ErrorMessage = "Please select a valid category")]
        public int Category { get; set; }
        public List<SelectListItem> lstCategory { set; get; }

        [Required(ErrorMessage="Please select accept/reject")]
        [Display(Name="Accept/Reject :")]
        [Range(1,2,ErrorMessage="Please select valid action")]
        public int AcceptReject { get; set; }
        public List<SelectListItem> AcceptRejectList { set; get; }

        //change by roht on 20-07-2022
        [Display(Name = "Forward To :")]
        [Required(ErrorMessage="Please select user.")]
        [Range(-1,int.MaxValue, ErrorMessage = "Please select valid user")]
        public int ForwardTo { get; set; }
        public List<SelectListItem> ForwardToList { set; get; }

        //-------------------------------------------------- remark required commented on 16-12-2022
        //[Required(ErrorMessage = "Please enter Remark")]
        //[RegularExpression(@"^[a-zA-Z0-9,().\-\/?\\ ]+$", ErrorMessage = "Remark should contain only letters")]
        [RegularExpression(@"^[a-zA-Z0-9,_@%&*+()‘’:.\-\/?\\""\'\r\n?|\n ]+$", ErrorMessage = "please enter remark in valid format")]
        [StringLength(1000, ErrorMessage = "Remark can be upto 1000 characters only")]
        [Display(Name = "Remark:")]
        public String ActionTakenRemark { get; set; }

        public List<TicketMatserDetails> listTicketMatserDetail { get; set; }
        public List<ViewReplyModel> listReplymodel { get; set; }


    }

    public class TicketReplyModel
    {
        public TicketReplyModel()
        {
            this.MasterTicketModel = new TicketMatserDetails();
            this.ForwardToList = new List<SelectListItem>();
            this.listReplymodel = new List<ViewReplyModel>();
            this.CurrentStatusList = new List<SelectListItem>();
            this.CurrentStatusList.Add(new SelectListItem { Text = "Opened", Value = "1" });
            this.CurrentStatusList.Add(new SelectListItem { Text = "In Progress", Value = "2" });
            this.CurrentStatusList.Add(new SelectListItem { Text = "Partially Closed", Value = "3" });
            this.CurrentStatusList.Add(new SelectListItem { Text = "Closed", Value = "4" });
            this.CurrentStatusList.Insert(0, new SelectListItem { Text = "Select Status", Value = "-1" });

        }

        public String TicketNo { get; set; }



        [Required(ErrorMessage = "Please select Status")]
        [Display(Name = "Current Status :")]
        [Range(1, 4, ErrorMessage = "Please select valid Status")]
        public int CurrentStatus { get; set; }
        public List<SelectListItem> CurrentStatusList { set; get; }

        // change nullable by rohit borse on 20-07-2022
        [Display(Name = "Forward To :")]
        [Required(ErrorMessage = "Please select user.")]
        [Range(-1, int.MaxValue, ErrorMessage = "Please select valid user")]
        public int? ForwardTo { get; set; }
        public List<SelectListItem> ForwardToList { set; get; }

        //------------------------------------------------------ remark required commented on 16-12-2022
        //[Required(ErrorMessage = "Please enter description")]
        //[RegularExpression(@"^[a-zA-Z0-9,().\-\/?\\ ]+$", ErrorMessage = "description should contain only letters")]        
        [RegularExpression(@"^[a-zA-Z0-9,_@%&*+()‘’:.\-\/?\\""\'\r\n?|\n ]+$", ErrorMessage = "please enter remark in valid format")]
        [StringLength(1000, ErrorMessage = "description can be upto 1000 characters only")]
        [Display(Name = "Reply :")]
        public String TicketReply { get; set; }

        [Display(Name="Document :")]
        public HttpPostedFileBase ReplyFile { get; set; }

         public TicketMatserDetails MasterTicketModel { get; set; }
        public List<ViewReplyModel> listReplymodel { get; set; }

        // added by rohit borse on 14-07-2022 
        [Display(Name = "Category :")]
        [Required(ErrorMessage = "Please select category")]
        [Range(1, 9, ErrorMessage = "Please select a valid category")]
        public int? Category { get; set; }
        public List<SelectListItem> lstCategory { set; get; }
    }

    public class TicketMatserDetails
    {
        public TicketMatserDetails()
        {
            this.FilesUrls = new List<string>();
        }


        [Display(Name = "Ticket No :")]
        public String TicketNo { get; set; }

        [Display(Name = "Name :")]
        public String Name { get; set; }

        [Display(Name = "Mobile :")]
        public String Contact { get; set; }

        [Display(Name = "Email :")]
        public String Email { get; set; }

        [Display(Name = "Module :")]
        public String ModuleName { get; set; }

        [Display(Name = "Category :")]
        public String CategoryName { get; set; }

        [Display(Name = "Description :")]
        public String Description { get; set; }

        [Display(Name = "Subject :")]
        public String Subject { get; set; }

        [Display(Name = "Replied By :")]
        public String RepliedBy { get; set; }

        [Display(Name = "Reply :")]
        public String Reply { get; set; }

        public List<String> FilesUrls {get; set;}
    }


    public class ViewReplyModel
    {

        [Display(Name = "Reply :")]
        public String Reply { get; set; }

        [Display(Name = "Reply By :")]
        public String ReplyBy { get; set; }

        [Display(Name = "Reply Date :")]
        public DateTime? ReplyDate { get; set; }

        [Display(Name = "Forwarded To :")]
        public String ForwardedTo { get; set; }

        [Display(Name = "File :")]
        public String FilesUrls { get; set; }

        [Display(Name="Status")]
        public String Status { get; set; }

    }
}