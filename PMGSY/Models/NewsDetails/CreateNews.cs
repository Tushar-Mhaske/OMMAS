using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace PMGSY.Models.NewsDetails
{
    public class CreateNews
    {
        public int newsId { get; set; }

        public int hdnewsId { get; set; }

        public string newsDBOpr { get; set; }

        [Required(ErrorMessage="Please Enter a News Title")]
        [RegularExpression(@"^([a-zA-Z0-9 ,-]+)$", ErrorMessage = "News Title is not in valid format")]
        public string newsTitle { get; set; }

        [Required(ErrorMessage = "Please Enter a valid News Description")]
        [RegularExpression(@"^([-a-zA-Z0-9,.)( ]+)$", ErrorMessage = "News Description has some invalid characters")]
        public string news_Desc { get; set; }

        //[RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Upload Date is not in valid format")]
        //[Required(ErrorMessage = "Please Select a Upload Date")]
        public string news_Upload_Date { get; set; }

        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "Start Date is not in valid format")]
        [Required(ErrorMessage = "Please Select a Start Date")]
        public string newa_Pub_Start_Date { get; set; }

        [RegularExpression(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", ErrorMessage = "End Date is not in valid format")]
        //[Required(ErrorMessage = "Please Enter a valid End Date")]
        public string newa_Pub_End_Date { get; set; }

        //[Required(ErrorMessage = "Please Enter a valid User Id")]
        public int news_User_Id { get; set; }

        public int mast_state_code { get; set; }

        public int mast_dist_code { get; set; }

        //[Required(ErrorMessage = "Please Enter a valid Approval")]
        public string news_Approval { get; set; }

        public string news_Approval_Date { get; set; }
        public string news_Archived_Date { get; set; }

        //[Required(ErrorMessage = "Please Enter a valid News Status")]
        public string news_Status { get; set; }

    }
}